using PhigrosLibraryCSharp.CloudSave.HttpModels;
using PhigrosLibraryCSharp.Serialization;
using System.IO.Compression;

namespace PhigrosLibraryCSharp.CloudSave;
/// <summary>
/// Represents the context of a save file, including decrypted data.
/// </summary>
public class SaveContext
{
	/// <summary>
	/// Function to encrypt or decrypt data. The input is the raw data, and the output is the processed data.
	/// </summary>
	/// <param name="data">The data to be processed.</param>
	/// <returns>The processed data.</returns>
	public delegate Task<byte[]> CipherFunction(byte[] data);
	/// <summary>
	/// An entry of the decrypted save data, containing the object version and the decrypted data bytes.
	/// </summary>
	public struct Entry
	{
		/// <summary>
		/// The version of the save file.
		/// </summary>
		public byte ObjectVersion { get; set; }
		/// <summary>
		/// The decrypted data bytes of the save entry.
		/// </summary>
		public byte[] Data { get; set; }

		/// <summary>
		/// Constructs a new instance of the <see cref="Entry"/> struct with the specified object version and decrypted data.
		/// </summary>
		/// <param name="objectVersion">The version of the save file.</param>
		/// <param name="data">The decrypted data bytes of the save entry.</param>
		public Entry(byte objectVersion, byte[] data)
		{
			this.ObjectVersion = objectVersion;
			this.Data = data;
		}
	}

	/// <summary>
	/// The raw summary data extracted from the save.
	/// </summary>
	public byte[] RawSummary { get; set; }

	/// <summary>
	/// The original save info object.
	/// </summary>
	public SaveInfo OriginalCloudObject { get; set; }

	/// <summary>
	/// A dictionary containing decrypted data entries corresponding to the raw data entries.
	/// </summary>
	public Dictionary<string, Entry> DecryptedDataEntries { get; }

	/// <summary>
	/// Gets the decrypted game record entry.
	/// </summary>
	public Entry DecryptedGameRecord { get => this.DecryptedDataEntries["gameRecord"]; set => this.DecryptedDataEntries["gameRecord"] = value; }

	/// <summary>
	/// Gets the decrypted game progress entry.
	/// </summary>
	public Entry DecryptedGameProgress { get => this.DecryptedDataEntries["gameProgress"]; set => this.DecryptedDataEntries["gameProgress"] = value; }

	/// <summary>
	/// Gets the decrypted game key entry.
	/// </summary>
	public Entry DecryptedGameKey { get => this.DecryptedDataEntries["gameKey"]; set => this.DecryptedDataEntries["gameKey"] = value; }

	/// <summary>
	/// Gets the decrypted game settings entry.
	/// </summary>
	public Entry DecryptedGameSettings { get => this.DecryptedDataEntries["settings"]; set => this.DecryptedDataEntries["settings"] = value; }

	/// <summary>
	/// Gets the decrypted user information entry.
	/// </summary>
	public Entry DecryptedGameUserInfo { get => this.DecryptedDataEntries["user"]; set => this.DecryptedDataEntries["user"] = value; }

	/// <summary>
	/// Initializes a new instance of the <see cref="SaveContext"/> class. Recommend to use
	/// static factory methods like <see cref="FromZipAsync(byte[], SaveInfo, CipherFunction)"/> to 
	/// construct the object instead of calling this constructor directly.
	/// </summary>
	/// <param name="decryptedEntries">The decrypted entries.</param>
	/// <param name="originalData">The original save info object.</param>
	public SaveContext(Dictionary<string, Entry> decryptedEntries, SaveInfo originalData)
	{
		this.DecryptedDataEntries = decryptedEntries;
		this.OriginalCloudObject = originalData;
		this.RawSummary = Convert.FromBase64String(originalData.Summary);
	}
	/// <summary>
	/// Creates a new instance of the <see cref="SaveContext"/> class by reading and decrypting the provided zip data. 
	/// </summary>
	/// <param name="rawZip">Zip buffer containing encrypted entries.</param>
	/// <param name="originalData">The original save info object.</param>
	/// <param name="decryptor">A function to decrypt the raw data entries.</param>
	/// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="SaveContext"/> instance.</returns>
	public static async Task<SaveContext> FromZipAsync(byte[] rawZip, SaveInfo originalData, CipherFunction decryptor) =>
		await FromZipAsync(new MemoryStream(rawZip), originalData, decryptor);
	/// <summary>
	/// Creates a new instance of the <see cref="SaveContext"/> class by reading and decrypting the provided zip data. 
	/// </summary>
	/// <param name="rawZip">Zip stream containing encrypted entries.</param>
	/// <param name="originalData">The original save info object.</param>
	/// <param name="decryptor">A function to decrypt the raw data entries.</param>
	/// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="SaveContext"/> instance.</returns>
	public static async Task<SaveContext> FromZipAsync(Stream rawZip, SaveInfo originalData, CipherFunction decryptor)
	{
		Dictionary<string, Entry> decryptedEntries = [];

		using ZipArchive archive = new(rawZip, ZipArchiveMode.Read, leaveOpen: true);
		foreach (ZipArchiveEntry item in archive.Entries)
		{
			using Stream stream = item.Open();
			byte[] decompressed = new byte[item.Length];
			await stream.ReadExactlyAsync(decompressed);
			decryptedEntries.Add(item.Name, new(decompressed[0], await decryptor.Invoke(decompressed[1..])));
		}

		return new(decryptedEntries, originalData);
	}
	/// <summary>
	/// Encrypt and saves the decrypted data entries to the provided zip archive. 
	/// The entries will be encrypted using the provided encryptor function before being written to the archive.
	/// </summary>
	/// <param name="archive">The zip archive to save the encrypted entries to.</param>
	/// <param name="encryptor">A function to encrypt the data entries.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	public async Task SaveToZipAsync(ZipArchive archive, CipherFunction encryptor)
	{
		foreach (KeyValuePair<string, Entry> item in this.DecryptedDataEntries)
		{
			ZipArchiveEntry entry = archive.CreateEntry(item.Key);
			using Stream stream = entry.Open();
			byte[] encryptedData = await encryptor.Invoke(item.Value.Data);
			stream.WriteByte(item.Value.ObjectVersion);
			await stream.WriteAsync(encryptedData);
		}
	}
	/// <summary>
	/// Encrypt and saves the decrypted data entries to the provided zip archive. 
	/// The entries will be encrypted using the provided encryptor function before being written to the archive.
	/// </summary>
	/// <param name="zipStream">The stream to save the encrypted entries to.</param>
	/// <param name="encryptor">A function to encrypt the data entries.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	public async Task SaveToStreamAsync(Stream zipStream, CipherFunction encryptor)
	{
		using ZipArchive archive = new(zipStream, ZipArchiveMode.Create, leaveOpen: true);
		await this.SaveToZipAsync(archive, encryptor);
	}

	#region Read operations
	/// <summary>
	/// Reads the player's play summary.
	/// </summary>
	/// <returns>The player's summary.</returns>
	public Summary ReadSummary()
	{
		ByteReader reader = new(this.RawSummary);

		return Summary.FromReader(reader);
	}
	/// <summary>
	/// Read the player's game records.
	/// </summary>
	/// <returns>The player's game records.</returns>
	public GameRecord ReadGameRecord()
	{
		Entry entry = this.DecryptedGameRecord;
		ByteReader reader = new(entry.Data, version: entry.ObjectVersion);
		return GameRecord.FromReader(reader);
	}

	/// <summary>
	/// Reads the player's game settings.
	/// </summary>
	/// <returns>The player's game settings.</returns>
	public GameSettings ReadGameSettings()
	{
		Entry entry = this.DecryptedGameSettings;
		ByteReader reader = new(entry.Data, version: entry.ObjectVersion);
		return GameSettings.FromReader(reader);
	}

	/// <summary>
	/// Reads the player's game progress.
	/// </summary>
	/// <returns>The player's game progress.</returns>
	public GameProgress ReadGameProgress()
	{
		Entry entry = this.DecryptedGameProgress;
		ByteReader reader = new(entry.Data, version: entry.ObjectVersion);
		return GameProgress.FromReader(reader);
	}

	/// <summary>
	/// Reads the player's game key.
	/// </summary>
	/// <returns>The player's game key.</returns>
	public GameKey ReadGameKey()
	{
		Entry entry = this.DecryptedGameKey;
		ByteReader reader = new(entry.Data, version: entry.ObjectVersion);
		return GameKey.FromReader(reader);
	}

	/// <summary>
	/// Reads the player's game user info.
	/// </summary>
	/// <returns>Player's game user info.</returns>
	public GameUserInfo ReadGameUserInfo()
	{
		Entry entry = this.DecryptedGameUserInfo;
		ByteReader reader = new(entry.Data, version: entry.ObjectVersion);
		return GameUserInfo.FromReader(reader);
	}
	#endregion

	#region Save operations
	/// <summary>
	/// Saves the player's play summary to this context.
	/// </summary>
	/// <param name="summary">The player's play summary to save.</param>
	public void SaveSummary(Summary summary)
	{
		using MemoryStream stream = new();
		ByteWriter writer = new(stream);
		summary.Serialize(writer);
		this.RawSummary = stream.ToArray();
	}
	/// <summary>
	/// Saves the player's game record to this context.
	/// </summary>
	/// <param name="record">The player's game record to save.</param>
	public void SaveGameRecord(GameRecord record)
	{
		using MemoryStream stream = new();
		ByteWriter writer = new(stream);
		record.Serialize(writer);
		this.DecryptedGameRecord = new(record.Version, stream.ToArray());
	}
	/// <summary>
	/// Saves the player's game settings to this context.
	/// </summary>
	/// <param name="settings">The player's game settings to save.</param>
	public void SaveGameSettings(GameSettings settings)
	{
		using MemoryStream stream = new();
		ByteWriter writer = new(stream);
		settings.Serialize(writer);
		this.DecryptedGameSettings = new(settings.Version, stream.ToArray());
	}
	/// <summary>
	/// Saves the player's game progress to this context.
	/// </summary>
	/// <param name="progress">The player's game progress to save.</param>
	public void SaveGameProgress(GameProgress progress)
	{
		using MemoryStream stream = new();
		ByteWriter writer = new(stream);
		progress.Serialize(writer);
		this.DecryptedGameProgress = new(progress.Version, stream.ToArray());
	}
	/// <summary>
	/// Saves the player's game key to this context.
	/// </summary>
	/// <param name="key">The player's game key to save.</param>
	public void SaveGameKey(GameKey key)
	{
		using MemoryStream stream = new();
		ByteWriter writer = new(stream);
		key.Serialize(writer);
		this.DecryptedGameKey = new(key.Version, stream.ToArray());
	}
	/// <summary>
	/// Saves the player's game user info to this context.
	/// </summary>
	/// <param name="userInfo">The player's game user info to save.</param>
	public void SaveGameUserInfo(GameUserInfo userInfo)
	{
		using MemoryStream stream = new();
		ByteWriter writer = new(stream);
		userInfo.Serialize(writer);
		this.DecryptedGameUserInfo = new(userInfo.Version, stream.ToArray());
	}
	#endregion
}
