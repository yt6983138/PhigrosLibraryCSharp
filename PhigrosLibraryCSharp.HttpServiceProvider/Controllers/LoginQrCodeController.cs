using Microsoft.AspNetCore.Mvc;
using PhigrosLibraryCSharp.Cloud.Login;
using PhigrosLibraryCSharp.Cloud.Login.DataStructure;

namespace PhigrosLibraryCSharp.HttpServiceProvider.Controllers;

[ApiController]
public class LoginQrCodeController : Controller
{
	private ILogger<LoginQrCodeController> _logger;
	public LoginQrCodeController(ILogger<LoginQrCodeController> logger)
	{
		this._logger = logger;
	}

	[HttpGet]
	[Route("api/[controller]/GetNewQRCode")]
	public async Task<IActionResult> GetNewQRCode()
	{
		CompleteQRCodeData qrcode = await TapTapHelper.RequestLoginQrCode();
		this._logger.LogInformation("{ip} requested a login QrCode. Url: {url}", this.HttpContext.GetIP(), qrcode.Url);

		return this.Json(qrcode);
	}

	[HttpPost]
	[Route("api/[controller]/CheckQRCode")]
	public async Task<IActionResult> CheckQRCode()
	{
		(IActionResult? action, CompleteQRCodeData? body) = await this.ReadBodyAs<CompleteQRCodeData, LoginQrCodeController>(this._logger);
		if (body is null)
			return action!;

		TapTapTokenData? result;
		try
		{
			result = await TapTapHelper.CheckQRCodeResult(body);
		}
		catch (Exception ex)
		{
			return this.Error(this._logger, ex.Message, code: ErrorCode.LoginOtherError);
		}
		if (result is null)
		{
			return this.Error(this._logger, "", "Login progress not done", code: ErrorCode.LoginProcessNotDone);
		}
		this._logger.LogInformation("{ip} checked successfully.", this.HttpContext.GetIP());
		return this.Json(result);
	}

	[HttpPost]
	[Route("api/[controller]/GetTapTapProfile")]
	public async Task<IActionResult> GetTapTapProfile()
	{
		(IActionResult? action, TapTapTokenData? body) = await this.ReadBodyAs<TapTapTokenData, LoginQrCodeController>(this._logger);
		if (body is null)
			return action!;

		TapTapProfileData? result;
		try
		{
			result = await TapTapHelper.GetProfile(body.Data);
		}
		catch (Exception ex)
		{
			return this.Error(this._logger, ex.Message, code: ErrorCode.GetProfileError);
		}
		this._logger.LogInformation("{ip} got profile successfully.", this.HttpContext.GetIP());
		return this.Json(result);
	}

	[HttpPost]
	[Route("api/[controller]/GetPhigrosToken")]
	public async Task<IActionResult> GetPhigrosToken()
	{
		(IActionResult? action, TapTapTokenData? body) = await this.ReadBodyAs<TapTapTokenData, LoginQrCodeController>(this._logger);
		if (body is null)
			return action!;

		TapTapProfileData result;
		try
		{
			result = await TapTapHelper.GetProfile(body.Data);
		}
		catch (Exception ex)
		{
			return this.Error(this._logger, ex.Message, code: ErrorCode.GetProfileError);
		}
		string token;
		try
		{
			token = await LCHelper.LoginAndGetToken(new(result.Data, body.Data));
		}
		catch (Exception ex)
		{
			return this.Error(this._logger, ex.Message, code: ErrorCode.LCLoginError);
		}
		this._logger.LogInformation("{ip} got token successfully. Token: {token}", this.HttpContext.GetIP(), token);
		return this.Content(token);
	}
}
