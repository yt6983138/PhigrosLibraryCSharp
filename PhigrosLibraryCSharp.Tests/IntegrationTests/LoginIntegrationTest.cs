using PhigrosLibraryCSharp.CloudSave.Login;
using System.Diagnostics;

namespace PhigrosLibraryCSharp.Tests.IntegrationTests;

[TestClass]
[TestCategory("Manual")]
public class LoginIntegrationTest // TODO: Add international test
{
	private TapTapTokenData? _data;
	private TapTapProfileData? _profile;
	private string? _token;

	[TestMethod]
	public async Task TestChinaMode()
	{
		await this.TestTapTapHelper(false);
		await this.TestLCHelper(false);
	}
	[TestMethod]
	public async Task TestInternationalMode()
	{
		await this.TestTapTapHelper(true);
		await this.TestLCHelper(true);
	}

	private async Task TestTapTapHelper(bool useChinaEndpoint)
	{
		CompleteQRCodeData qrcode = await TapTapHelper.RequestLoginQrCode(useChinaEndpoint: useChinaEndpoint);
		Console.WriteLine("Raw qrcode:");
		Console.WriteLine(qrcode.ToJson());

		// open url
		Process.Start(
			new ProcessStartInfo(qrcode.Url)
			{
				UseShellExecute = true
			});

		while ((this._data = await TapTapHelper.CheckQRCodeResult(qrcode, useChinaEndpoint)) is null)
		{
			await Task.Delay(3000);
		}

		Console.WriteLine("Token:");
		Console.WriteLine(this._data.ToJson());

		Console.WriteLine("Profile:");
		Console.WriteLine((this._profile = await TapTapHelper.GetProfile(this._data.Data, useChinaEndpoint: useChinaEndpoint)).ToJson());
	}
	private async Task TestLCHelper(bool useChinaEndpoint)
	{
		this._token = await LCHelper.LoginAndGetToken(new(this._profile!.Data, this._data!.Data), useChinaEndpoint: useChinaEndpoint);
		Console.WriteLine($"Token: {this._token}");
	}
}
