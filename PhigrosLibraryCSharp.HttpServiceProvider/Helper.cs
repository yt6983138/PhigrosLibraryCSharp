using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace PhigrosLibraryCSharp.HttpServiceProvider;

public enum ErrorCode
{
	RequestError = -1,
	Unspecified = 0,
	LoginProcessNotDone = 1,
	LoginOtherError = 11,
	GetProfileError = 114,
	LCLoginError = 1145,
	DecryptingError = 114514,
	PhigrosTokenError = 1145141,
	PhigrosLibraryInternalError = 11451419,
	ArgumentOutOfRange = 114514191
}
public static class Helper
{
	public static async Task<string> ReadBodyAsString(this HttpRequest request)
	{
		request.EnableBuffering();

		using StreamReader requestReader = new(request.Body, leaveOpen: true);
		string body = await requestReader.ReadToEndAsync();
		request.Body.Seek(0, SeekOrigin.Begin);

		return body;
	}
	public static async Task<(IActionResult?, T?)> ReadBodyAs<T, TController>(this TController controller, ILogger<TController> logger) where TController : Controller
	{
		T? body;
		try
		{
			body = await controller.Request.ReadFromJsonAsync<T>();
		}
		catch (Exception ex)
		{
			return (controller.Error(logger, ex.Message, code: ErrorCode.RequestError), default);
		}
		if (body == null)
		{
			logger.LogInformation("{ip} sent empty request.", controller.HttpContext.GetIP());
			return (controller.Error(logger, "Body is null", code: ErrorCode.RequestError), default);
		}
		return (null, body);
	}
	public static string GetIP(this HttpContext context)
	{
		return context.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress?.ToString()!;
	}
	public static IActionResult Error<T>(this T controller, ILogger<T> logger, string error, string message = "Error processing request", ErrorCode code = ErrorCode.Unspecified) where T : Controller
	{
		logger.LogInformation("{ip} went error: {message}; {error}; {code}", controller.HttpContext.GetIP(), message, error, code);
		return controller.Json(new { Message = message, Error = error, Code = (int)code, CodeName = code.ToString() });
	}
}
