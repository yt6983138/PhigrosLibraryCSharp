using PhigrosLibraryCSharp.CloudSave.RawData;
using PhigrosLibraryCSharp.Serialization;
using System.IO.Compression;

namespace PhigrosLibraryCSharp.CloudSave;
/// <summary>
/// Represents the context of a save file, including raw and decrypted data.
/// </summary>
public class SaveContext
{
	public struct Entry
	{
		public byte ObjectVersion { get; set; }
		public byte[] Data { get; set; }

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
	/// The original raw save data object.
	/// </summary>
	public RawSave OriginalCloudObject { get; set; }

	/// <summary>
	/// A dictionary containing decrypted data entries corresponding to the raw data entries.
	/// </summary>
	public Dictionary<string, Entry> DecryptedDataEntries { get; }

	/// <summary>
	/// Gets the decrypted game record data.
	/// </summary>
	public Entry DecryptedGameRecord { get => this.DecryptedDataEntries["gameRecord"]; set => this.DecryptedDataEntries["gameRecord"] = value; }

	/// <summary>
	/// Gets the decrypted game progress data.
	/// </summary>
	public Entry DecryptedGameProgress { get => this.DecryptedDataEntries["gameProgress"]; set => this.DecryptedDataEntries["gameProgress"] = value; }

	/// <summary>
	/// Gets the decrypted game key data.
	/// </summary>
	public Entry DecryptedGameKey { get => this.DecryptedDataEntries["gameKey"]; set => this.DecryptedDataEntries["gameKey"] = value; }

	/// <summary>
	/// Gets the decrypted game settings data.
	/// </summary>
	public Entry DecryptedGameSettings { get => this.DecryptedDataEntries["settings"]; set => this.DecryptedDataEntries["settings"] = value; }

	/// <summary>
	/// Gets the decrypted user information data.
	/// </summary>
	public Entry DecryptedGameUserInfo { get => this.DecryptedDataEntries["user"]; set => this.DecryptedDataEntries["user"] = value; }

	/// <summary>
	/// Initializes a new instance of the <see cref="SaveContext"/> class.
	/// </summary>
	/// <param name="rawZip">The raw ZIP file data.</param>
	/// <param name="originalData">The original raw save data object.</param>
	/// <param name="decryptor">A function to decrypt the raw data entries.</param>
	public SaveContext(Dictionary<string, Entry> decryptedEntries, RawSave originalData)
	{
		this.DecryptedDataEntries = decryptedEntries;
		this.OriginalCloudObject = originalData;
		this.RawSummary = Convert.FromBase64String(originalData.summary);
	}
	public static async Task<SaveContext> FromZipAsync(byte[] rawZip, RawSave originalData, Func<byte[], Task<byte[]>> decryptor)
	{
		Dictionary<string, Entry> decryptedEntries = [];

		using ZipArchive archive = new(new MemoryStream(rawZip), ZipArchiveMode.Read);
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
	/// <param name="difficulties">Parsed difficulties CSV from <see href="https://github.com/3035936740/Phigros_Resource/"/>.</param>
	/// <param name="exceptionHandler">The exception handler when something was wrong in the parser (instead just throwing)</param>
	/// <returns>The player's game records.</returns>
	public GameRecord ReadGameRecord()
	{
		Entry entry = this.DecryptedGameRecord;
		ByteReader reader = new(entry.Data, entry.ObjectVersion);
		return GameRecord.FromReader(reader);
	}

	/// <summary>
	/// Reads the player's game settings.
	/// </summary>
	/// <returns>The player's game settings.</returns>
	public GameSettings ReadGameSettings()
	{
		Entry entry = this.DecryptedGameSettings;
		ByteReader reader = new(entry.Data, entry.ObjectVersion);
		return GameSettings.FromReader(reader);
	}

	/// <summary>
	/// Reads the player's game progress.
	/// </summary>
	/// <returns>The player's game progress.</returns>
	public GameProgress ReadGameProgress()
	{
		Entry entry = this.DecryptedGameProgress;
		ByteReader reader = new(entry.Data, entry.ObjectVersion);
		return GameProgress.FromReader(reader);
	}

	/// <summary>
	/// Reads the player's game key.
	/// </summary>
	/// <returns>The player's game key.</returns>
	public GameKey ReadGameKey()
	{
		Entry entry = this.DecryptedGameKey;
		ByteReader reader = new(entry.Data, entry.ObjectVersion);
		return GameKey.FromReader(reader);
	}

	/// <summary>
	/// Reads the player's game user info.
	/// </summary>
	/// <returns>Player's game user info.</returns>
	public GameUserInfo ReadGameUserInfo()
	{
		Entry entry = this.DecryptedGameUserInfo;
		ByteReader reader = new(entry.Data, entry.ObjectVersion);
		return GameUserInfo.FromReader(reader);
	}
}
