using PhigrosLibraryCSharp.CloudSave;

namespace PhigrosLibraryCSharp.Tests;

[TestClass]
public class SaveTest
{
	[TestMethod]
	public async Task TestSave()
	{
		Save save = new(Secret.Token, false);
		SaveContext ctx = await save.GetSaveContextAsync(0);
		Console.WriteLine("Progress:");
		Console.WriteLine(ctx.ReadGameProgress().ToJson());
		Console.WriteLine("Settings:");
		Console.WriteLine(ctx.ReadGameSettings().ToJson());
		Console.WriteLine("Game userinfo:");
		Console.WriteLine(ctx.ReadGameUserInfo().ToJson());
		Console.WriteLine("Userinfo:");
		Console.WriteLine((await save.GetUserInfoAsync()).ToJson());
		Console.WriteLine("Game key:");
		Console.WriteLine(ctx.ReadGameKey().ToJson());
	}
}
