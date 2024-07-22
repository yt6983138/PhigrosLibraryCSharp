using System.Diagnostics;

namespace PhigrosLibraryCSharp.HttpServiceProvider.Test;

[TestClass]
public class QrCodeTest
{
	public const string JsonType = "application/json";

	public static short Port { get; set; } = 5015;
	public static string Host { get; set; } = "127.0.0.1";
	public HttpClient HttpClient { get; set; } = new();

	public string Template { get; set; } = $"http://{Host}:{Port}/api/LoginQrCode";

	public string EndPointGetNew => $"{this.Template}/GetNewQRCode";
	public string EndPointCheckQrcode => $"{this.Template}/CheckQRCode";
	public string EndPointGetTapTapProfile => $"{this.Template}/GetTapTapProfile";
	public string EndPointGetPhigrosToken => $"{this.Template}/GetPhigrosToken";

	[TestMethod]
	public async Task TestAll()
	{
		string qrcode = await this.HttpClient.GetStringAsync(this.EndPointGetNew);
		Console.WriteLine("Raw qrcode");
		Console.WriteLine(qrcode);

		Process.Start(
			new ProcessStartInfo((string)qrcode.AsDictionary()["url"])
			{
				UseShellExecute = true
			});

		string checkResult;
		while (true)
		{
			await Task.Delay(3000);
			checkResult = await this.HttpClient.PostWithStringAsContentAsync(this.EndPointCheckQrcode, qrcode, JsonType);
			Console.WriteLine("checking:");
			Console.WriteLine(checkResult);

			if (checkResult.AsDictionary().TryGetValue("data", out object? _))
				break;
		}

		Console.WriteLine("profile");
		Console.WriteLine(await this.HttpClient.PostWithStringAsContentAsync(this.EndPointGetTapTapProfile, checkResult, JsonType));
		Console.WriteLine("token:");
		Console.WriteLine(await this.HttpClient.PostWithStringAsContentAsync(this.EndPointGetPhigrosToken, checkResult, JsonType));
	}
}
