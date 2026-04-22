using PhigrosLibraryCSharp.CloudSave;
using PhigrosLibraryCSharp.CloudSave.RawData;
using System.Runtime.CompilerServices;

namespace PhigrosLibraryCSharp.Tests;

[TestClass]
public class SaveTest
{
	private static volatile bool _isCurrentDirectoryEnsured = false;

	private static void EnsureCurrentDirectory()
	{
		if (_isCurrentDirectoryEnsured) return;
		_isCurrentDirectoryEnsured = true;
		Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, "..", "..", "..");
	}
	private static void AssertSame(SaveContext.Entry expected, SaveContext.Entry actual)
	{
		Assert.AreEqual(expected.ObjectVersion, actual.ObjectVersion);
		Assert.IsTrue(expected.Data.SequenceEqual(actual.Data));
	}
	private static void Dump(SaveContext.Entry entry, [CallerArgumentExpression(nameof(entry))] string name = "")
	{
		EnsureCurrentDirectory();
		using Stream stream = File.OpenWrite($"TestData/{name}.bin");
		stream.WriteByte(entry.ObjectVersion);
		stream.Write(entry.Data);
	}
	private static void Dump(byte[] data, [CallerArgumentExpression(nameof(data))] string name = "")
	{
		EnsureCurrentDirectory();
		using Stream stream = File.OpenWrite($"TestData/{name}.bin");
		stream.Write(data);
	}
	[TestMethod]
	public async Task TestSaveToZip()
	{
		Save save = new(Secret.Token, false);

		SaveInfo saveInfo = (await save.GetSaveInfoFromCloudAsync()).Results[0];
		byte[] rawZip = await save.GetSaveZipAsync(saveInfo.Simplify());
		SaveContext ctx = await SaveContext.FromZipAsync(rawZip, saveInfo, save.Decrypt);

		EnsureCurrentDirectory();

		using FileStream rawZipStream = File.Open("TestData/raw.zip", FileMode.Create, FileAccess.ReadWrite);
		rawZipStream.Write(rawZip);

		using FileStream zip = File.Open("TestData/save.zip", FileMode.Create, FileAccess.ReadWrite);
		await ctx.SaveToStreamAsync(zip, save.Encrypt);
		zip.Seek(0, SeekOrigin.Begin);

		SaveContext ctx2 = await SaveContext.FromZipAsync(zip, saveInfo, save.Decrypt);

		AssertSame(ctx.DecryptedGameProgress, ctx2.DecryptedGameProgress);
		AssertSame(ctx.DecryptedGameSettings, ctx2.DecryptedGameSettings);
		AssertSame(ctx.DecryptedGameUserInfo, ctx2.DecryptedGameUserInfo);
		AssertSame(ctx.DecryptedGameKey, ctx2.DecryptedGameKey);
		AssertSame(ctx.DecryptedGameRecord, ctx2.DecryptedGameRecord);
		Assert.IsTrue(ctx.RawSummary.SequenceEqual(ctx2.RawSummary));
	}
	[TestMethod]
	public async Task TestSerialization()
	{
		Save save = new(Secret.Token, false);
		SaveContext ctx = await save.GetSaveContextAsync(0);

		GameProgress progress = ctx.ReadGameProgress();
		GameSettings settings = ctx.ReadGameSettings();
		GameUserInfo userInfo = ctx.ReadGameUserInfo();
		GameKey gameKey = ctx.ReadGameKey();
		GameRecord gameRecord = ctx.ReadGameRecord();
		Summary summary = ctx.ReadSummary();

		SaveContext.Entry progressData = ctx.DecryptedGameProgress.Clone();
		SaveContext.Entry settingsData = ctx.DecryptedGameSettings.Clone();
		SaveContext.Entry userInfoData = ctx.DecryptedGameUserInfo.Clone();
		SaveContext.Entry gameKeyData = ctx.DecryptedGameKey.Clone();
		SaveContext.Entry gameRecordData = ctx.DecryptedGameRecord.Clone();
		byte[] summaryData = ctx.RawSummary.ToArray();

		ctx.SaveGameProgress(progress);
		ctx.SaveGameSettings(settings);
		ctx.SaveGameUserInfo(userInfo);
		ctx.SaveGameKey(gameKey);
		ctx.SaveGameRecord(gameRecord);
		ctx.SaveSummary(summary);

		Dump(ctx.DecryptedGameProgress);
		Dump(ctx.DecryptedGameSettings);
		Dump(ctx.DecryptedGameUserInfo);
		Dump(ctx.DecryptedGameKey);
		Dump(ctx.DecryptedGameRecord);
		Dump(ctx.RawSummary);

		Dump(progressData);
		Dump(settingsData);
		Dump(userInfoData);
		Dump(gameKeyData);
		Dump(gameRecordData);
		Dump(summaryData);

		AssertSame(progressData, ctx.DecryptedGameProgress);
		AssertSame(settingsData, ctx.DecryptedGameSettings);
		AssertSame(userInfoData, ctx.DecryptedGameUserInfo);
		AssertSame(gameKeyData, ctx.DecryptedGameKey);
		AssertSame(gameRecordData, ctx.DecryptedGameRecord);
		Assert.IsTrue(summaryData.SequenceEqual(ctx.RawSummary));

		Console.WriteLine("Summary");
		Console.WriteLine(summary.ToJson());
		Console.WriteLine("Progress:");
		Console.WriteLine(progress.ToJson());
		Console.WriteLine("Settings:");
		Console.WriteLine(settings.ToJson());
		Console.WriteLine("Game userinfo:");
		Console.WriteLine(userInfo.ToJson());
		Console.WriteLine("Userinfo:");
		Console.WriteLine((await save.GetUserInfoAsync()).ToJson());
		Console.WriteLine("Game key:");
		Console.WriteLine(gameKey.ToJson());
	}
}
