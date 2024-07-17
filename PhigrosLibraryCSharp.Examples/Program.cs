using PhigrosLibraryCSharp.Cloud.DataStructure;
using PhigrosLibraryCSharp.Cloud.Login;
using PhigrosLibraryCSharp.Cloud.Login.DataStructure;

namespace PhigrosLibraryCSharp.Examples;

internal class Program
{
	private static async Task Main(string[] args)
	{
		Console.CancelKeyPress += (object? _, ConsoleCancelEventArgs _2) => Environment.Exit(0);

		int number;
		if (args.Length == 0)
		{
			Console.WriteLine(
				"""
				Which example you wish to run:
				1. Get token
				2. Show misc user info
				3. Show best 5 score
				""");
			number = int.Parse(Console.ReadLine()!.Trim());
		}
		else
		{
			number = int.Parse(args[0]);
		}

		switch (number)
		{
			case 1:
				await GetToken();
				break;
			case 2:
				await ShowMiscInfo();
				break;
			case 3:
				await ShowBest5();
				break;
			default:
				Console.WriteLine($"We don't have example {number}!");
				return;
		}
	}

	private static async Task GetToken()
	{
		// taptap part
		CompleteQRCodeData qrcode = await TapTapHelper.RequestLoginQrCode();
		Console.WriteLine($"Please login using url {qrcode.Url}");
		Console.WriteLine($"Expires in {qrcode.ExpiresInSeconds} seconds");

		TapTapTokenData? data;
		while (true)
		{
			data = await TapTapHelper.CheckQRCodeResult(qrcode);
			if (data is not null) break;

			await Task.Delay(3000);
		}
		Console.WriteLine($"Your taptap token: {data.Data.Token}");

		TapTapProfileData profile = await TapTapHelper.GetProfile(data.Data);
		// lc part
		string token = await LCHelper.LoginAndGetToken(new(profile.Data, data.Data));
		Console.WriteLine($"Your phigros token: {token}");
	}
	private static async Task ShowMiscInfo()
	{
		Console.WriteLine("What is your phigros token? (can be acquired from example 1)");
		Save save = new(Console.ReadLine()!);

		Console.WriteLine("Progress:");
		Console.WriteLine((await save.GetGameProgressAsync(0)).ToJson());
		Console.WriteLine("Settings:");
		Console.WriteLine((await save.GetGameSettingsAsync(0)).ToJson());
		Console.WriteLine("Game user info:");
		Console.WriteLine((await save.GetGameUserInfoAsync(0)).ToJson());
		Console.WriteLine("User info:");
		Console.WriteLine((await save.GetUserInfoAsync()).ToJson());
	}
	private static async Task ShowBest5()
	{
		Console.WriteLine("What is your phigros token? (can be acquired from example 1)");
		Save save = new(Console.ReadLine()!);
		Console.WriteLine("Where is difficulty.tsv?");
		string tsvPath = Console.ReadLine()!;

		// parse tsv
		Dictionary<string, float[]> difficulties = new();
		foreach (string line in File.ReadAllLines(tsvPath))
		{
			string[] splitted = line.Split('\t', 2);
			if (splitted.Length < 2) continue;

			difficulties.Add(splitted[0].Trim(),
				splitted[1]
					.Replace("\n", "")
					.Split('\t')
					.Select(x => float.Parse(x.Trim()))
					.ToArray());
		}

		// scores
		(Summary Summary, GameSave Save) data = await save.GetGameSaveAsync(difficulties, 0);
		Console.WriteLine("Your summary:");
		Console.WriteLine(data.Summary.ToJson());
		Console.WriteLine("Your top 5 scores:");
		int i = 0;
		data.Save.Records.Sort((x, y) => y.Rks.CompareTo(x.Rks));
		Console.WriteLine(data.Save.Records
			.GetRange(0, 5)
			.Select(x => $"{++i}:\n{x}")
			.Join("\n"));
	}
}
