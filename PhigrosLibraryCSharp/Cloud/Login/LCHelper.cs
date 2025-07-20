using PhigrosLibraryCSharp.Extensions;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PhigrosLibraryCSharp.Cloud.Login;
/// <summary>
/// A helper to assist you login at LeanCloud service (for Phigros).
/// </summary>
public static class LCHelper
{
	#region Constants/Definiton
	internal const string AppKey = @"Qr9AEqtuoSVS3zeD6iVbM4ZC0AtkJcQ89tywVyi0";
	internal const string ClientId = @"rAK3FfdieFob2Nn8Am";

	internal const string InternationalAppKey = @"tG9CTm0LDD736k9HMM9lBZrbeBGRmUkjSfNLDNib";
	internal const string InternationalClientId = @"kviehleldgxsagpozb";

	internal static string GetClientId(bool isChina)
		=> isChina ? ClientId : InternationalClientId;
	internal static string GetAppKey(bool isChina)
		=> isChina ? AppKey : InternationalAppKey;

	private static Task<string> MD5HashHexStringDefaultGetter(string input)
		=> Task.FromResult(_md5.ComputeHash(Encoding.UTF8.GetBytes(input)).ToHex());
	private static readonly HttpClient Client = new()
	{
		BaseAddress = new(Save.CloudServerAddress)
	};
	private static readonly HttpClient InternationalClient = new()
	{
		BaseAddress = new(Save.InternationalCloudServerAddress)
	};
	private static HttpClient GetClient(bool isChina)
		=> isChina ? Client : InternationalClient;

	internal static readonly MD5 _md5;
	#endregion

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	static LCHelper()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
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
	public static Func<string, Task<string>> GetMD5HashHexString { get; set; } = MD5HashHexStringDefaultGetter;

	/// <summary>
	/// Login with combined data of <see cref="TapTapHelper.GetProfile(TapTapTokenData.TokenData, int, bool)"/>
	/// and <see cref="TapTapHelper.CheckQRCodeResult(CompleteQRCodeData, bool)"/>. <br/>
	/// See also: <see cref="LCCombinedAuthData(TapTapProfileData.ProfileData, TapTapTokenData.TokenData)"/>
	/// </summary>
	/// <param name="data">
	/// Combined data of <see cref="TapTapHelper.GetProfile(TapTapTokenData.TokenData, int, bool)"/>
	/// and <see cref="TapTapHelper.CheckQRCodeResult(CompleteQRCodeData, bool)"/>. <br/>
	/// See also: <see cref="LCCombinedAuthData(TapTapProfileData.ProfileData, TapTapTokenData.TokenData)"/>
	/// </param>
	/// <param name="useChinaEndpoint">Use China endpoints to login or not.</param>
	/// <param name="failOnNotExist">[Unknown]</param>
	/// <returns>A <see cref="Dictionary{TKey, TValue}"/> containing the logged user information.</returns>
	public static async Task<JsonNode> LoginWithAuthData(LCCombinedAuthData data, bool useChinaEndpoint = true, bool failOnNotExist = false)
	{
		Dictionary<string, object> authData = new()
		{
			{ "taptap", data }
		};
		string path = failOnNotExist ? "users?failOnNotExist=true" : "users";
		JsonNode response = await Request<JsonNode>(
			path,
			HttpMethod.Post,
			useChinaEndpoint,
			headers: new() { ["X-LC-Id"] = useChinaEndpoint ? ClientId : InternationalClientId },
			data: new Dictionary<string, object> {
				{ "authData", authData }
			}
		);

		return response;
	}
	/// <summary>
	/// Login with combined data of <see cref="TapTapHelper.GetProfile(TapTapTokenData.TokenData, int, bool)"/>
	/// and <see cref="TapTapHelper.CheckQRCodeResult(CompleteQRCodeData, bool)"/>, then get the token. <br/>
	/// See also: <see cref="LCCombinedAuthData(TapTapProfileData.ProfileData, TapTapTokenData.TokenData)"/>
	/// </summary>
	/// <param name="data">
	/// Combined data of <see cref="TapTapHelper.GetProfile(TapTapTokenData.TokenData, int, bool)"/>
	/// and <see cref="TapTapHelper.CheckQRCodeResult(CompleteQRCodeData, bool)"/>. <br/>
	/// See also: <see cref="LCCombinedAuthData(TapTapProfileData.ProfileData, TapTapTokenData.TokenData)"/>
	/// </param>
	/// <param name="useChinaEndpoint">Use China endpoints to login or not.</param>
	/// <param name="failOnNotExist">[Unknown]</param>
	/// <returns>The session token of the user.</returns>
	public static async Task<string> LoginAndGetToken(LCCombinedAuthData data, bool useChinaEndpoint = true, bool failOnNotExist = false)
		=> (await LoginWithAuthData(data, useChinaEndpoint, failOnNotExist))["sessionToken"].EnsureNotNull().GetValue<string>();

	internal static async Task<T> Request<T>(
		string path,
		HttpMethod method,
		bool useChinaEndpoint,
		Dictionary<string, object>? headers = null,
		object? data = null,
		Dictionary<string, object>? queryParams = null,
		bool withAPIVersion = true)
	{
		HttpClient client = GetClient(useChinaEndpoint);
		string url = BuildUrl(path, useChinaEndpoint, queryParams!, withAPIVersion);
		HttpRequestMessage request = new()
		{
			RequestUri = new Uri(url),
			Method = method,
		};
		// request.SetNoCors();
		await FillHeaders(request.Headers, useChinaEndpoint, headers);

		string? content = null;
		if (data != null)
		{
			content = JsonSerializer.Serialize(data, Save.SerializerSettings);
			StringContent requestContent = new(content);
			requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			request.Content = requestContent;
		}
		// LCHttpUtils.PrintRequest(client, request, content);
		HttpResponseMessage response;
		if (TapTapHelper.Proxy is not null)
		{
			response = await TapTapHelper.Proxy(client, request);
		}
		else
		{
			response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
		}
		request.Dispose();

		string resultString = await response.Content.ReadAsStringAsync();
		HttpStatusCode statusCode = response.StatusCode;
		response.Dispose();
		// LCHttpUtils.PrintResponse(response, resultString);

		if (response.IsSuccessStatusCode)
		{
			T ret = JsonSerializer.Deserialize<T>(resultString, Save.SerializerSettings).EnsureNotNull();
			return ret;
		}
		throw new ApplicationException($"{statusCode}: {resultString}");
	}
	private static string BuildUrl(string path, bool useChinaEndpoint, Dictionary<string, object> queryParams, bool withAPIVersion)
	{
		StringBuilder urlSB = new(Save.GetCloudServerAddress(useChinaEndpoint));
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
	private static async Task FillHeaders(HttpRequestHeaders headers, bool useChinaEndpoint, Dictionary<string, object>? reqHeaders = null)
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
		string data = $"{timestamp}{GetAppKey(useChinaEndpoint)}";
		string hash = await GetMD5HashHexString(data);
		string sign = $"{hash},{timestamp}";
		headers.Add("X-LC-Sign", sign);
	}

}
