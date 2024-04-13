using Newtonsoft.Json;
using PhigrosLibraryCSharp.Cloud.Login.DataStructure;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace PhigrosLibraryCSharp.Cloud.Login;
/// <summary>
/// A helper to assist you login at LeanCloud service (for Phigros).
/// </summary>
public static class LCHelper
{
	#region Constants/Definiton
	internal const string AppKey = @"Qr9AEqtuoSVS3zeD6iVbM4ZC0AtkJcQ89tywVyi0";
	internal const string ClientId = @"rAK3FfdieFob2Nn8Am";

	private static string MD5HashHexStringDefaultGetter(string input)
		=> _md5.ComputeHash(Encoding.UTF8.GetBytes(input)).ToHex();
	private static HttpClient Client { get; } = new()
	{
		BaseAddress = new(SaveHelper.CloudServerAddress)
	};
	private static Dictionary<string, object> UniversalHeaders { get; } = new()
	{
		{ "X-LC-Id", ClientId }
	};
	internal readonly static MD5 _md5 = MD5.Create();
	#endregion

	static LCHelper()
	{
		try
		{
			_md5 = MD5.Create();
		}
		catch { }
	}

	/// <summary>
	/// A function to get MD5 hash string that can be changed if you are on unsupported platform (ex. WASM).
	/// </summary>
	public static Func<string, string> GetMD5HashHexString { get; set; } = MD5HashHexStringDefaultGetter;

	/// <summary>
	/// Login with combined data of <see cref="TapTapHelper.GetProfile(TapTapTokenData.TokenData, bool, int)"/>
	/// and <see cref="TapTapHelper.CheckQRCodeResult(CompleteQRCodeData, bool)"/>. <br/>
	/// See also: <see cref="LCCombinedAuthData(TapTapProfileData.ProfileData, TapTapTokenData.TokenData))"/>
	/// </summary>
	/// <param name="data">
	/// Combined data of <see cref="TapTapHelper.GetProfile(TapTapTokenData.TokenData, bool, int)"/>
	/// and <see cref="TapTapHelper.CheckQRCodeResult(CompleteQRCodeData, bool)"/>. <br/>
	/// See also: <see cref="LCCombinedAuthData(TapTapProfileData.ProfileData, TapTapTokenData.TokenData))"/>
	/// </param>
	/// <param name="failOnNotExist">[Unknown]</param>
	/// <returns>A <see cref="Dictionary{TKey, TValue}"/> containing the logged user information.</returns>
	public static async Task<Dictionary<string, object>> LoginWithAuthData(LCCombinedAuthData data, bool failOnNotExist = false)
	{
		Dictionary<string, object> authData = new()
		{
			{ "taptap", data }
		};
		string path = failOnNotExist ? "users?failOnNotExist=true" : "users";
		Dictionary<string, object> response = await Request<Dictionary<string, object>>(
			path,
			HttpMethod.Post,
			headers: UniversalHeaders,
			data: new Dictionary<string, object> {
				{ "authData", authData }
			}
		);

		return response;
	}
	/// <summary>
	/// Login with combined data of <see cref="TapTapHelper.GetProfile(TapTapTokenData.TokenData, bool, int)"/>
	/// and <see cref="TapTapHelper.CheckQRCodeResult(CompleteQRCodeData, bool)"/>, then get the token. <br/>
	/// See also: <see cref="LCCombinedAuthData(TapTapProfileData.ProfileData, TapTapTokenData.TokenData))"/>
	/// </summary>
	/// <param name="data">
	/// Combined data of <see cref="TapTapHelper.GetProfile(TapTapTokenData.TokenData, bool, int)"/>
	/// and <see cref="TapTapHelper.CheckQRCodeResult(CompleteQRCodeData, bool)"/>. <br/>
	/// See also: <see cref="LCCombinedAuthData(TapTapProfileData.ProfileData, TapTapTokenData.TokenData))"/>
	/// </param>
	/// <param name="failOnNotExist">[Unknown]</param>
	/// <returns>The session token of the user.</returns>
	public static async Task<string> LoginAndGetToken(LCCombinedAuthData data, bool failOnNotExist = false)
		=> (string)(await LoginWithAuthData(data, failOnNotExist))["sessionToken"];

	public static async Task<T> Request<T>(
		string path,
		HttpMethod method,
		Dictionary<string, object>? headers = null,
		object? data = null,
		Dictionary<string, object>? queryParams = null,
		bool withAPIVersion = true)
	{
		string url = BuildUrl(path, queryParams!, withAPIVersion);
		HttpRequestMessage request = new()
		{
			RequestUri = new Uri(url),
			Method = method,
		};
		FillHeaders(request.Headers, headers);

		string? content = null;
		if (data != null)
		{
			content = JsonConvert.SerializeObject(data);
			StringContent requestContent = new(content);
			requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			request.Content = requestContent;
		}
		// LCHttpUtils.PrintRequest(client, request, content);
		HttpResponseMessage response = await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
		request.Dispose();

		string resultString = await response.Content.ReadAsStringAsync();
		System.Net.HttpStatusCode statusCode = response.StatusCode;
		response.Dispose();
		// LCHttpUtils.PrintResponse(response, resultString);

		if (response.IsSuccessStatusCode)
		{
			T ret = JsonConvert.DeserializeObject<T>(resultString)!;
			return ret;
		}
		throw new Exception($"{statusCode}: {resultString}");
	}
	private static string BuildUrl(string path, Dictionary<string, object> queryParams, bool withAPIVersion)
	{
		StringBuilder urlSB = new(SaveHelper.CloudServerAddress);
		if (withAPIVersion)
		{
			urlSB.Append("/1.1"); // https://github.com/leancloud/csharp-sdk/blob/master/Common/Common/LCCore.cs
		}
		urlSB.Append($"/{path}");
		string url = urlSB.ToString();
		if (queryParams != null)
		{
			IEnumerable<string> queryPairs = queryParams
				.Where(kv => kv.Value != null)
				.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value.ToString()!)}");
			string queries = string.Join("&", queryPairs);
			url = $"{url}?{queries}";
		}
		return url;
	}
	private static void FillHeaders(HttpRequestHeaders headers, Dictionary<string, object>? reqHeaders = null)
	{
		// 额外 headers
		if (reqHeaders != null)
		{
			foreach (KeyValuePair<string, object> kv in reqHeaders)
			{
				headers.Add(kv.Key, kv.Value.ToString());
			}
		}
		// 签名
		long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		string data = $"{timestamp}{AppKey}";
		string hash = GetMD5HashHexString(data);
		string sign = $"{hash},{timestamp}";
		headers.Add("X-LC-Sign", sign);
	}

}
