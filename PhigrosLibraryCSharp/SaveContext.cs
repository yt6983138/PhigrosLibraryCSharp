using PhigrosLibraryCSharp.Cloud.RawData;
using PhigrosLibraryCSharp.Extensions;
using PhigrosLibraryCSharp.GameRecords;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;

namespace PhigrosLibraryCSharp;
/// <summary>
/// Represents the context of a save file, including raw and decrypted data.
/// </summary>
public class SaveContext
{
	/// <summary>
	/// The raw ZIP file data of the save.
	/// </summary>
	public byte[] RawZip { get; }

	/// <summary>
	/// The raw summary data extracted from the save.
	/// </summary>
	public byte[] RawSummary { get; }

	/// <summary>
	/// The original raw save data object.
	/// </summary>
	public RawSave OriginalData { get; }

	/// <summary>
	/// A dictionary containing raw data entries extracted from the ZIP file.
	/// </summary>
	public Dictionary<string, byte[]> RawDataEntries { get; } = [];

	/// <summary>
	/// A dictionary containing decrypted data entries corresponding to the raw data entries.
	/// </summary>
	public Dictionary<string, byte[]> DecryptedDataEntries { get; } = [];

	/// <summary>
	/// Gets the decrypted game record data.
	/// </summary>
	public byte[] DecryptedGameRecord => this.DecryptedDataEntries["gameRecord"];

	/// <summary>
	/// Gets the decrypted game progress data.
	/// </summary>
	public byte[] DecryptedGameProgress => this.DecryptedDataEntries["gameProgress"];

	/// <summary>
	/// Gets the decrypted game settings data.
	/// </summary>
	public byte[] DecryptedGameSettings => this.DecryptedDataEntries["settings"];

	/// <summary>
	/// Gets the decrypted user information data.
	/// </summary>
	public byte[] DecryptedGameUserInfo => this.DecryptedDataEntries["user"];

	/// <summary>
	/// Gets the raw game record data.
	/// </summary>
	public byte[] RawGameRecord => this.RawDataEntries["gameRecord"];

	/// <summary>
	/// Gets the raw game progress data.
	/// </summary>
	public byte[] RawGameProgress => this.RawDataEntries["gameProgress"];

	/// <summary>
	/// Gets the raw game settings data.
	/// </summary>
	public byte[] RawGameSettings => this.RawDataEntries["settings"];

	/// <summary>
	/// Gets the raw user information data.
	/// </summary>
	public byte[] RawGameUserInfo => this.RawDataEntries["user"];

	/// <summary>
	/// Initializes a new instance of the <see cref="SaveContext"/> class.
	/// </summary>
	/// <param name="rawZip">The raw ZIP file data.</param>
	/// <param name="originalData">The original raw save data object.</param>
	/// <param name="decryptor">A function to decrypt the raw data entries.</param>
	public SaveContext(byte[] rawZip, RawSave originalData, Func<byte[], Task<byte[]>> decryptor)
	{
		this.RawZip = rawZip;
		this.OriginalData = originalData;
		this.RawSummary = Convert.FromBase64String(this.OriginalData.ToParsed().Summary);

		using ZipArchive archive = new(new MemoryStream(this.RawZip), ZipArchiveMode.Read);
		foreach (ZipArchiveEntry item in archive.Entries)
		{
			using Stream stream = item.Open();
			byte[] decompressed = new byte[item.Length];
			stream.ReadExactly(decompressed);

			this.RawDataEntries.Add(item.Name, decompressed);
			this.DecryptedDataEntries.Add(item.Name, decryptor.Invoke(decompressed[1..]).GetAwaiter().GetResult());
		}
	}

	/// <summary>
	/// Reads the player's play summary.
	/// </summary>
	/// <returns>The player's summary.</returns>
	public Summary ReadSummary()
	{
		int offset = 0;

		int firstStructSize = Marshal.SizeOf<RawSummaryFirst>();
		int lastStructSize = Marshal.SizeOf<RawSummaryLast>();

		offset += firstStructSize - 3; // ??? the size is larger than expected
		RawSummaryFirst firstPart = SerialHelper.ByteToStruct<RawSummaryFirst>(this.RawSummary[..firstStructSize]);
		string avatarInfo = Encoding.UTF8.GetString(this.RawSummary[offset..(offset + firstPart.AvatarStringSize)]);
		RawSummaryLast lastPart = SerialHelper.ByteToStruct<RawSummaryLast>(this.RawSummary[^lastStructSize..]);

		return new(
			firstPart.SaveVersion,
			firstPart.GameVersion,
			new(firstPart.ChallengeCode),
			avatarInfo,
			[.. lastPart.Scores]);
	}
	/// <summary>
	/// Read the player's game records.
	/// </summary>
	/// <param name="difficulties">Parsed difficulties CSV from <see href="https://github.com/3035936740/Phigros_Resource/"/>.</param>
	/// <param name="exceptionHandler">The exception handler when something was wrong in the parser (instead just throwing)</param>
	/// <returns>The player's game records.</returns>
	public GameRecord ReadGameRecord(IReadOnlyDictionary<string, float[]> difficulties, Action<string, Exception?, object?>? exceptionHandler = null)
	{
		ByteReader reader = new(this.DecryptedGameRecord, version: this.RawGameRecord[0]);
		return new()
		{
			Version = reader.ObjectVersion,
			Records = reader.ReadAllGameRecord(difficulties, exceptionHandler),
			Summary = this.OriginalData.ToParsed().Summary
		};
	}

	/// <summary>
	/// Reads the player's game settings.
	/// </summary>
	/// <returns>The player's game settings.</returns>
	public GameSettings ReadGameSettings()
	{
		ByteReader reader = new(this.DecryptedGameSettings, version: this.RawGameSettings[0]);
		return reader.ReadGameSettings();
	}

	/// <summary>
	/// Reads the player's game progress.
	/// </summary>
	/// <returns>The player's game progress.</returns>
	public GameProgress ReadGameProgress()
	{
		ByteReader reader = new(this.DecryptedGameProgress, version: this.RawGameProgress[0]);
		return reader.ReadGameProgress();
	}

	/// <summary>
	/// Reads the player's game user info.
	/// </summary>
	/// <returns>Player's game user info.</returns>
	public GameUserInfo ReadGameUserInfo()
	{
		ByteReader reader = new(this.DecryptedGameUserInfo, version: this.RawGameUserInfo[0]);
		return reader.ReadGameUserInfo();
	}
}
