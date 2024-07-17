using PhigrosLibraryCSharp.Cloud.Login;
using PhigrosLibraryCSharp.Cloud.Login.DataStructure;
using System.Diagnostics;

namespace PhigrosLibraryCSharp.Tests;

[TestClass]
public class LoginTest
{
	private TapTapTokenData? _data;
	private TapTapProfileData? _profile;
	private string? _token;

	[TestMethod]
	public async Task TestAll()
	{
		await this.TestTapTapHelper();
		await this.TestLCHelper();
		await this.TestSave();
	}

	private async Task TestTapTapHelper()
	{
		CompleteQRCodeData qrcode = await TapTapHelper.RequestLoginQrCode();
		Console.WriteLine("Raw qrcode:");
		Console.WriteLine(qrcode.ToJson());

		// open url
		Process.Start(
			new ProcessStartInfo(qrcode.Url)
			{
				UseShellExecute = true
			});

		while ((this._data = await TapTapHelper.CheckQRCodeResult(qrcode)) is null)
		{
			await Task.Delay(3000);
		}

		Console.WriteLine("Token:");
		Console.WriteLine(this._data.ToJson());

		Console.WriteLine("Profile:");
		Console.WriteLine((this._profile = await TapTapHelper.GetProfile(this._data.Data)).ToJson());
	}
	private async Task TestLCHelper()
	{
		this._token = await LCHelper.LoginAndGetToken(new(this._profile!.Data, this._data!.Data));
		Console.WriteLine($"Token: {this._token}");
	}
	private async Task TestSave()
	{
		Save save = new(this._token!);
		Console.WriteLine("Progress:");
		Console.WriteLine((await save.GetGameProgressAsync(0)).ToJson());
		Console.WriteLine("Settings:");
		Console.WriteLine((await save.GetGameSettingsAsync(0)).ToJson());
		Console.WriteLine("Game userinfo:");
		Console.WriteLine((await save.GetGameUserInfoAsync(0)).ToJson());
		Console.WriteLine("Userinfo:");
		Console.WriteLine((await save.GetUserInfoAsync()).ToJson());
	}
}
