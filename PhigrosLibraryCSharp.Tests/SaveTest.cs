using PhigrosLibraryCSharp.CloudSave;
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
	[TestMethod]
	public async Task TestSave()
	{
		Save save = new(Secret.Token, false);
		SaveContext ctx = await save.GetSaveContextAsync(0);

		GameProgress progress = ctx.ReadGameProgress();
		GameSettings settings = ctx.ReadGameSettings();
		GameUserInfo userInfo = ctx.ReadGameUserInfo();
		GameKey gameKey = ctx.ReadGameKey();
		GameRecord gameRecord = ctx.ReadGameRecord();

		SaveContext.Entry progressData = ctx.DecryptedGameProgress.Clone();
		SaveContext.Entry settingsData = ctx.DecryptedGameSettings.Clone();
		SaveContext.Entry userInfoData = ctx.DecryptedGameUserInfo.Clone();
		SaveContext.Entry gameKeyData = ctx.DecryptedGameKey.Clone();
		SaveContext.Entry gameRecordData = ctx.DecryptedGameRecord.Clone();

		ctx.SaveGameProgress(progress);
		ctx.SaveGameSettings(settings);
		ctx.SaveGameUserInfo(userInfo);
		ctx.SaveGameKey(gameKey);
		ctx.SaveGameRecord(gameRecord);

		Dump(ctx.DecryptedGameProgress);
		Dump(ctx.DecryptedGameSettings);
		Dump(ctx.DecryptedGameUserInfo);
		Dump(ctx.DecryptedGameKey);
		Dump(ctx.DecryptedGameRecord);

		Dump(progressData);
		Dump(settingsData);
		Dump(userInfoData);
		Dump(gameKeyData);
		Dump(gameRecordData);

		AssertSame(progressData, ctx.DecryptedGameProgress);
		AssertSame(settingsData, ctx.DecryptedGameSettings);
		AssertSame(userInfoData, ctx.DecryptedGameUserInfo);
		AssertSame(gameKeyData, ctx.DecryptedGameKey);
		AssertSame(gameRecordData, ctx.DecryptedGameRecord);

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
