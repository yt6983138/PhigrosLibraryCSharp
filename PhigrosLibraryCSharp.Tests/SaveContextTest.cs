using PhigrosLibraryCSharp.CloudSave;

namespace PhigrosLibraryCSharp.Tests;

[TestClass]
public class SaveContextTest
{
	private byte[] _testZip1 = [];
	private readonly SaveContext.CipherFunction _encryptor;
	private readonly SaveContext.CipherFunction _decryptor;

	public SaveContextTest()
	{
		Save save = new("0000000000000000000000000", false);
		this._encryptor = save.Encrypt;
		this._decryptor = save.Decrypt;
	}

	[TestInitialize]
	public void TestInitialize()
	{
		this._testZip1 = File.ReadAllBytes("CloudSaveExample1.zip");
	}

	private async Task<SaveContext> PrepareSaveContext(string summary = "BVoBW65xQVEQ5re35LmxLUNvbmZ1c2lvbmYAWQBFAIQASgAcAMYATwAVAA0AAQABAA==")
	{
		return await SaveContext.FromZipAsync(this._testZip1, new()
		{
			CreatedAt = default,
			GameFile = null!,
			ModifiedAt = null!,
			Name = null!,
			ObjectId = null!,
			UpdatedAt = default,
			User = null!,
			Summary = summary
		}, this._decryptor);
	}

	public async Task TestSaveToZipRoundTrip()
	{
		SaveContext oldContext = await this.PrepareSaveContext();
		using MemoryStream stream = new();
		await oldContext.SaveToStreamAsync(stream, this._encryptor);

		stream.Seek(0, SeekOrigin.Begin);
		SaveContext newContext = await SaveContext.FromZipAsync(stream, oldContext.OriginalCloudObject, this._decryptor);

		Assert.AreEquivalent(oldContext.RawSummary, newContext.RawSummary);
		Assert.AreEquivalent(oldContext.DecryptedDataEntries, newContext.DecryptedDataEntries);
		Assert.AreEquivalent(oldContext.OriginalCloudObject, newContext.OriginalCloudObject);
	}

	[TestMethod]
	public async Task TestGameSettingsSerialize()
	{
		byte[] expected = [0x0A, 0x0B, 0x6D, 0x79, 0x20, 0x64, 0x65, 0x76, 0x69, 0x63, 0x65, 0x20, 0x31, 0x00, 0x00, 0x00, 0x3F, 0x00, 0x00, 0x80, 0x3E, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x99, 0x99, 0xBE, 0x00, 0x00, 0x80, 0x3E];
		GameSettings model = new(1, false, true, false, true, "my device 1", 0.5f, 0.25f, 1f, 0f, -0.3f, 0.25f);
		SaveContext context = await this.PrepareSaveContext();
		context.SaveGameSettings(model);
		Assert.AreSequenceEqual(expected, context.DecryptedGameSettings.Data);
		Assert.AreEqual(model.Version, context.DecryptedGameSettings.ObjectVersion);
	}
	[TestMethod]
	public async Task TestGameSettingsDeserialize()
	{
		byte[] data = [0x0A, 0x0B, 0x6D, 0x79, 0x20, 0x64, 0x65, 0x76, 0x69, 0x63, 0x65, 0x20, 0x31, 0x00, 0x00, 0x00, 0x3F, 0x00, 0x00, 0x80, 0x3E, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x99, 0x99, 0xBE, 0x00, 0x00, 0x80, 0x3E];
		GameSettings expected = new(1, false, true, false, true, "my device 1", 0.5f, 0.25f, 1f, 0f, -0.3f, 0.25f);
		SaveContext context = await this.PrepareSaveContext();
		context.DecryptedGameSettings = new(1, data);
		Assert.AreEquivalent(expected, context.ReadGameSettings());
	}

	[TestMethod]
	public async Task TestSummarySerialize()
	{
		byte[] expected = [0x03, 0x24, 0x02, 0x52, 0xb8, 0x76, 0x41, 0xfc, 0xf2, 0x01, 0x0c, 0x49, 0x6e, 0x74, 0x72, 0x6f, 0x64, 0x75, 0x63, 0x74, 0x69, 0x6f, 0x6e, 0x0a, 0x00, 0x05, 0x00, 0x02, 0x00, 0x14, 0x00, 0x08, 0x00, 0x03, 0x00, 0x0f, 0x00, 0x06, 0x00, 0x01, 0x00, 0x05, 0x00, 0x02, 0x00, 0x00, 0x00];
		Summary model = new(
			3,
			new((ushort)548),
			15.42f,
			31100,
			"Introduction",
			new(10, 5, 2),
			new(20, 8, 3),
			new(15, 6, 1),
			new(5, 2, 0));
		SaveContext context = await this.PrepareSaveContext();
		context.SaveSummary(model);
		Assert.AreSequenceEqual(expected, context.RawSummary);
	}
	[TestMethod]
	public async Task TestSummaryDeserialize()
	{
		byte[] data = [0x03, 0x24, 0x02, 0x52, 0xb8, 0x76, 0x41, 0xfc, 0xf2, 0x01, 0x0c, 0x49, 0x6e, 0x74, 0x72, 0x6f, 0x64, 0x75, 0x63, 0x74, 0x69, 0x6f, 0x6e, 0x0a, 0x00, 0x05, 0x00, 0x02, 0x00, 0x14, 0x00, 0x08, 0x00, 0x03, 0x00, 0x0f, 0x00, 0x06, 0x00, 0x01, 0x00, 0x05, 0x00, 0x02, 0x00, 0x00, 0x00];
		Summary expected = new(
			3,
			new((ushort)548),
			15.42f,
			31100,
			"Introduction",
			new(10, 5, 2),
			new(20, 8, 3),
			new(15, 6, 1),
			new(5, 2, 0));
		SaveContext context = await this.PrepareSaveContext();
		context.RawSummary = data;
		Assert.AreEquivalent(expected, context.ReadSummary());
	}

	[TestMethod]
	public async Task TestGameRecordSerialize()
	{
		byte[] expected = [0x02, 0x0f, 0x53, 0x74, 0x61, 0x73, 0x69, 0x73, 0x2e, 0x4d, 0x61, 0x6f, 0x7a, 0x6f, 0x6e, 0x2e, 0x30, 0x12, 0x03, 0x01, 0x20, 0xf4, 0x0e, 0x00, 0x00, 0x00, 0xc5, 0x42, 0xf0, 0x7e, 0x0e, 0x00, 0x00, 0x00, 0xbe, 0x42, 0x16, 0x44, 0x69, 0x73, 0x74, 0x6f, 0x72, 0x74, 0x65, 0x64, 0x46, 0x61, 0x74, 0x65, 0x2e, 0x53, 0x75, 0x6e, 0x73, 0x65, 0x74, 0x2e, 0x30, 0x0a, 0x04, 0x04, 0x40, 0x42, 0x0f, 0x00, 0x00, 0x00, 0xc8, 0x42];
		GameRecord model = new(
		[ // the serialization should fix ordering (reversed hd, ez), do not touch the order of these records
			new(950000, 95.0f, "Stasis.Maozon.0", false, Difficulty.HD),
			new(980000, 98.5f, "Stasis.Maozon.0", true, Difficulty.EZ),
			new(1000000, 100f, "DistortedFate.Sunset.0", true, Difficulty.IN),
		], 1);
		SaveContext context = await this.PrepareSaveContext();
		context.SaveGameRecord(model);
		Assert.AreSequenceEqual(expected, context.DecryptedGameRecord.Data);
		Assert.AreEqual(model.Version, context.DecryptedGameRecord.ObjectVersion);
	}
	[TestMethod]
	public async Task TestGameRecordDeserialize()
	{
		byte[] data = [0x02, 0x0f, 0x53, 0x74, 0x61, 0x73, 0x69, 0x73, 0x2e, 0x4d, 0x61, 0x6f, 0x7a, 0x6f, 0x6e, 0x2e, 0x30, 0x12, 0x03, 0x01, 0x20, 0xf4, 0x0e, 0x00, 0x00, 0x00, 0xc5, 0x42, 0xf0, 0x7e, 0x0e, 0x00, 0x00, 0x00, 0xbe, 0x42, 0x16, 0x44, 0x69, 0x73, 0x74, 0x6f, 0x72, 0x74, 0x65, 0x64, 0x46, 0x61, 0x74, 0x65, 0x2e, 0x53, 0x75, 0x6e, 0x73, 0x65, 0x74, 0x2e, 0x30, 0x0a, 0x04, 0x04, 0x40, 0x42, 0x0f, 0x00, 0x00, 0x00, 0xc8, 0x42];
		GameRecord expected = new(
		[
			new(980000, 98.5f, "Stasis.Maozon.0", true, Difficulty.EZ),
			new(950000, 95.0f, "Stasis.Maozon.0", false, Difficulty.HD),
			new(1000000, 100f, "DistortedFate.Sunset.0", true, Difficulty.IN),
		], 1);
		SaveContext context = await this.PrepareSaveContext();
		context.DecryptedGameRecord = new(1, data);
		Assert.AreEquivalent(expected, context.ReadGameRecord());
	}

	[TestMethod]
	public async Task TestGameProgressSerialize()
	{
		byte[] expected = [0x06, 0x03, 0x33, 0x2e, 0x30, 0x2a, 0x43, 0x01, 0x64, 0x32, 0x0a, 0x01, 0x00, 0x03, 0x04, 0x08, 0x01, 0x11, 0x02, 0x06, 0x04];
		GameProgress model = new(
			4,
			false,
			true,
			true,
			false,
			"3.0",
			42,
			new Challenge((ushort)323),
			new Money(100, 50, 10, 1, 0),
			DifficultyUnlockFlag.EZ | DifficultyUnlockFlag.HD,
			DifficultyUnlockFlag.IN,
			DifficultyUnlockFlag.AT,
			SongRecordFlag.YATMINSGrade,
			new(RandomVersionFlag.R | RandomVersionFlag.O, new(Chapter8UnlockFlag.UnlockSecondPhase, DifficultyUnlockFlag.IN | DifficultyUnlockFlag.HD, new(TakumiUnlockFlag.ATruthSeekerINSGrade))));
		SaveContext context = await this.PrepareSaveContext();
		context.SaveGameProgress(model);
		Assert.AreSequenceEqual(expected, context.DecryptedGameProgress.Data);
		Assert.AreEqual(model.Version, context.DecryptedGameProgress.ObjectVersion);
	}
	[TestMethod]
	public async Task TestGameProgressDeserialize()
	{
		byte[] data = [0x06, 0x03, 0x33, 0x2e, 0x30, 0x2a, 0x43, 0x01, 0x64, 0x32, 0x0a, 0x01, 0x00, 0x03, 0x04, 0x08, 0x01, 0x11, 0x02, 0x06, 0x04];
		GameProgress expected = new(
			4,
			false,
			true,
			true,
			false,
			"3.0",
			42,
			new Challenge((ushort)323),
			new Money(100, 50, 10, 1, 0),
			DifficultyUnlockFlag.EZ | DifficultyUnlockFlag.HD,
			DifficultyUnlockFlag.IN,
			DifficultyUnlockFlag.AT,
			SongRecordFlag.YATMINSGrade,
			new(RandomVersionFlag.R | RandomVersionFlag.O, new(Chapter8UnlockFlag.UnlockSecondPhase, DifficultyUnlockFlag.IN | DifficultyUnlockFlag.HD, new(TakumiUnlockFlag.ATruthSeekerINSGrade))));
		SaveContext context = await this.PrepareSaveContext();
		context.DecryptedGameProgress = new(4, data);
		Assert.AreEquivalent(expected, context.ReadGameProgress());
	}

	[TestMethod]
	public async Task TestGameKeySerialize()
	{
		byte[] expected = [0x02, 0x07, 0x41, 0x76, 0x61, 0x74, 0x61, 0x72, 0x31, 0x03, 0x12, 0x32, 0x45, 0x03, 0x6f, 0x77, 0x6f, 0x02, 0x02, 0x3e, 0x3f, 0x01, 0x00, 0x01];
		GameKey model = new(
			1,
			new()
			{
				{ "Avatar1", new((byte)(GameKeyFlagType.HasUnlockedSingle | GameKeyFlagType.HasUnlockedAvatar), [50, 69]) },
				{ "owo", new((byte)GameKeyFlagType.HasUnlockedSingle , [62]) },
			},
			0x3F,
			new(true, new(false, true)));
		SaveContext context = await this.PrepareSaveContext();
		context.SaveGameKey(model);
		Assert.AreSequenceEqual(expected, context.DecryptedGameKey.Data);
		Assert.AreEqual(model.Version, context.DecryptedGameKey.ObjectVersion);
	}
	[TestMethod]
	public async Task TestGameKeyDeserialize()
	{
		byte[] data = [0x02, 0x07, 0x41, 0x76, 0x61, 0x74, 0x61, 0x72, 0x31, 0x03, 0x12, 0x32, 0x45, 0x03, 0x6f, 0x77, 0x6f, 0x02, 0x02, 0x3e, 0x3f, 0x01, 0x00, 0x01];
		GameKey expected = new(
			1,
			new()
			{
				{ "Avatar1", new((byte)(GameKeyFlagType.HasUnlockedSingle | GameKeyFlagType.HasUnlockedAvatar), [50, 69]) },
				{ "owo", new((byte)GameKeyFlagType.HasUnlockedSingle , [62]) },
			},
			0x3F,
			new(true, new(false, true)));
		SaveContext context = await this.PrepareSaveContext();
		context.DecryptedGameKey = new(1, data);
		Assert.AreEquivalent(expected, context.ReadGameKey());
	}

	[TestMethod]
	public async Task TestGameUserInfoSerialize()
	{
		byte[] expected = [0x01, 0x0b, 0x48, 0x65, 0x6c, 0x6c, 0x6f, 0x20, 0x77, 0x6f, 0x72, 0x6c, 0x64, 0x0c, 0x49, 0x6e, 0x74, 0x72, 0x6f, 0x64, 0x75, 0x63, 0x74, 0x69, 0x6f, 0x6e, 0x04, 0x62, 0x67, 0x6d, 0x31];
		GameUserInfo model = new(1, true, "Hello world", "Introduction", "bgm1");
		SaveContext context = await this.PrepareSaveContext();
		context.SaveGameUserInfo(model);
		Assert.AreSequenceEqual(expected, context.DecryptedGameUserInfo.Data);
		Assert.AreEqual(model.Version, context.DecryptedGameUserInfo.ObjectVersion);
	}
	[TestMethod]
	public async Task TestGameUserInfoDeserialize()
	{
		byte[] data = [0x01, 0x0b, 0x48, 0x65, 0x6c, 0x6c, 0x6f, 0x20, 0x77, 0x6f, 0x72, 0x6c, 0x64, 0x0c, 0x49, 0x6e, 0x74, 0x72, 0x6f, 0x64, 0x75, 0x63, 0x74, 0x69, 0x6f, 0x6e, 0x04, 0x62, 0x67, 0x6d, 0x31];
		GameUserInfo expected = new(1, true, "Hello world", "Introduction", "bgm1");
		SaveContext context = await this.PrepareSaveContext();
		context.DecryptedGameUserInfo = new(1, data);
		Assert.AreEquivalent(expected, context.ReadGameUserInfo());
	}
}
