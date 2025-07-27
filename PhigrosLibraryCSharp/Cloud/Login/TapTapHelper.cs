using PhigrosLibraryCSharp.Extensions;
using System.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;
using static PhigrosLibraryCSharp.Cloud.Login.RequestException;

namespace PhigrosLibraryCSharp.Cloud.Login;
/// <summary>
/// A helper to assist you login at TapTap.
/// </summary>
public static class TapTapHelper // TODO: Add callback login
{
	#region Constants
	internal const string TapSDKVersion = "2.1";

	internal static readonly string AssemblyVersion = (typeof(TapTapHelper).Assembly.GetName().Version?.ToString() ?? "unknown_version")
		.Replace(" ", "_", StringComparison.InvariantCultureIgnoreCase);
	internal static readonly string AssemblyName = (typeof(TapTapHelper).Assembly.GetName().Name ?? "unknown_dll")
		.Replace(" ", "_", StringComparison.InvariantCultureIgnoreCase);

	private static readonly HttpClient _client = new();
	private static readonly HttpClient _internationalClient = new();

	private static HttpClient GetClient(bool useChinaEndpoint)
		=> useChinaEndpoint ? _client : _internationalClient;
	#endregion

	#region Endpoints
	/// <summary>
	/// When on WASM, you may need to set this property to have cors disabled. (Browser limit)
	/// Or, when you just want to use a proxy.
	/// </summary>
	public static Func<HttpClient, HttpRequestMessage, Task<HttpResponseMessage>>? Proxy { get; set; }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	public static string WebHost { get; } = @"https://accounts.tapapis.com";
	public static string ChinaWebHost { get; } = @"https://accounts.tapapis.cn";
	public static string AccountHost { get; } = @"https://accounts.taptap.io";
	public static string ChinaAccountHost { get; } = @"https://accounts.taptap.com";
	public static string ApiHost { get; } = @"https://open.tapapis.com";
	public static string ChinaApiHost { get; } = @"https://open.tapapis.cn";
	public static string CodeUrl => WebHost + @"/oauth2/v1/device/code";
	public static string ChinaCodeUrl => ChinaWebHost + @"/oauth2/v1/device/code";
	public static string TokenUrl => WebHost + @"/oauth2/v1/token";
	public static string ChinaTokenUrl => ChinaWebHost + @"/oauth2/v1/token";
	public static string AccountUrl => AccountHost + @"/authorize";
	public static string ChinaAccountUrl => ChinaAccountHost + @"/authorize";

	public static string GetWebHost(bool useChinaEndpoint)
		=> useChinaEndpoint ? ChinaWebHost : WebHost;
	public static string GetAccountHost(bool useChinaEndpoint)
		=> useChinaEndpoint ? ChinaAccountHost : AccountHost;
	public static string GetApiHost(bool useChinaEndpoint)
		=> useChinaEndpoint ? ChinaApiHost : ApiHost;
	public static string GetCodeUrl(bool useChinaEndpoint)
		=> useChinaEndpoint ? ChinaCodeUrl : CodeUrl;
	public static string GetTokenUrl(bool useChinaEndpoint)
		=> useChinaEndpoint ? ChinaTokenUrl : TokenUrl;
	public static string GetInternationalProfileUrl(bool havePublicProfile = true)
		=> havePublicProfile ? ApiHost + "/account/profile/v1?client_id=" : ApiHost + "/account/basic-info/v1?client_id=";
	public static string GetChinaProfileUrl(bool havePublicProfile = true)
		=> havePublicProfile ? ChinaApiHost + "/account/profile/v1?client_id=" : ChinaApiHost + "/account/basic-info/v1?client_id=";
	public static string GetProfileUrl(bool useChinaEndpoint, bool havePublicProfile = true)
		=> useChinaEndpoint ? GetChinaProfileUrl(havePublicProfile) : GetInternationalProfileUrl(havePublicProfile);
	public static string GetAccountUrl(bool useChinaEndpoint)
		=> useChinaEndpoint ? ChinaAccountUrl : AccountUrl;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
	#endregion

	internal static string GenerateDeviceId() => $"{AssemblyName}-{AssemblyVersion}-" +
			$"{(DateTime.UtcNow - DateTime.UnixEpoch).TotalMilliseconds}-{Random.Shared.Next(0, 114514):N}";
	internal static string GenerateCodeVerifier()
	{
		const string CharList = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~";
		Span<char> @string = stackalloc char[128];
		for (int i = 0; i < @string.Length; i++)
		{
			@string[i] = CharList[Random.Shared.Next(CharList.Length)];
		}
		return new(@string);
	}
	internal static string GetCodeChallenge(string codeVerifier)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(codeVerifier);
		byte[] hash = SHA256.HashData(bytes);
		return Convert.ToBase64String(hash).Replace("=", "").Replace('+', '-').Replace('/', '_');
	}

	/// <summary>
	/// Generate a callback login url for user to login with OAuth 2.0 flow.
	/// </summary>
	/// <param name="callbackUrl">Callback url for user to redirect to when the login ended.
	/// Usually you would be hosting a controller/listener to listen this url, 
	/// and handle the code query passed to <see cref="HandleCallbackLogin(CallbackLoginData, string, bool)"/>.</param>
	/// <param name="useChinaEndpoint">Use China endpoints or not.</param>
	/// <param name="permissions">Permissions to grant. Default: public_profile</param>
	/// <returns>Callback login data to begin login with.</returns>
	public static CallbackLoginData GenerateCallbackLoginUrl(string callbackUrl, bool useChinaEndpoint = true, string[]? permissions = null)
	{
		string codeVerifier = GenerateCodeVerifier();
		string challenge = GetCodeChallenge(codeVerifier);
		string state = Guid.NewGuid().ToString("N");
		string scope = string.Join(",", permissions ?? ["public_profile"]);

		Dictionary<string, object> data = new()
		{
			{ "client_id", LCHelper.GetClientId(useChinaEndpoint) },
			{ "response_type", "code" },
			{ "redirect_uri", callbackUrl },
			{ "state", state },
			{ "code_challenge", challenge },
			{ "code_challenge_method", "S256" },
			{ "scope", scope },
			{ "flow", "pc_localhost" }
		};
		return new(codeVerifier, callbackUrl, state, challenge, scope,
			BuildUrl(GetAccountUrl(useChinaEndpoint), data));
	}
	/// <summary>
	/// Handle the callback login with the code received from the callback url.
	/// </summary>
	/// <param name="loginData">The login data generated by <see cref="GenerateCallbackLoginUrl(string, bool, string[])"/>.</param>
	/// <param name="code">Code parsed from your listener/controller. It is the code query parameter.</param>
	/// <param name="useChinaEndpoint">Use China endpoints or not.</param>
	/// <returns>TapTap token data.</returns>
	public static async Task<TapTapTokenData> HandleCallbackLogin(CallbackLoginData loginData, string code, bool useChinaEndpoint = true)
	{
		Dictionary<string, string> @params = new()
		{
			{ "client_id", LCHelper.GetClientId(useChinaEndpoint) },
			{ "grant_type", "authorization_code" },
			{ "secret_type", "hmac-sha-1" },
			{ "code", code },
			{ "redirect_uri", loginData.RedirectUrl },
			{ "code_verifier", loginData.CodeVerifier }
		};

		// the request method might go wrong since its not for this purpose but ill test first
		return await Request<TapTapTokenData>(GetTokenUrl(useChinaEndpoint), HttpMethod.Post, useChinaEndpoint, data: @params);
	}

	/// <summary>
	/// Request a url for user to login.
	/// </summary>
	/// <param name="permissions">Extra permissions to ask, provide <see langword="null"/> to use default permissions.</param>
	/// <param name="useChinaEndpoint">Use China endpoints to login or not.</param>
	/// <returns>A completed collection of QRCode data. See <see cref="CompleteQRCodeData"/> for more information.</returns>
	/// <seealso cref="CompleteQRCodeData"/>
	public static async Task<CompleteQRCodeData> RequestLoginQrCode(string[]? permissions = null, bool useChinaEndpoint = true)
	{
		string deviceId = GenerateDeviceId();
		Dictionary<string, object> parameters = new()
		{
			{ "client_id", LCHelper.GetClientId(useChinaEndpoint) },
			{ "response_type", "device_code" },
			{ "scope", string.Join(",", permissions ?? ["public_profile"]) },
			{ "version", TapSDKVersion },
			{ "platform", "unity" },
			{ "info", "{\"device_id\":\"" + deviceId + "\"}" } 
			// ^ https://github.com/taptap/TapSDK-UE4/blob/f66d15048ebff4628f1614ca8df8a7a07dabf6cb/TapCommon/Source/TapCommon/Tools/TUDeviceInfo.h#L30 
		};
		return new(await Request<PartialTapTapQRCodeData>(GetCodeUrl(useChinaEndpoint), HttpMethod.Post, useChinaEndpoint, data: parameters), deviceId);
	}
	/// <summary>
	/// Check if the user has logged in through QRCode.
	/// </summary>
	/// <param name="qrCodeData">The completed QRCode data from <see cref="RequestLoginQrCode(string[], bool)"/>.</param>
	/// <param name="useChinaEndpoint">Use China endpoints to login or not.</param>
	/// <returns><see langword="null"/> if not verified, the token data if verified.</returns>
	/// <exception cref="Exception">Received unknown response</exception>
	public static async Task<TapTapTokenData?> CheckQRCodeResult(CompleteQRCodeData qrCodeData, bool useChinaEndpoint = true)
	{
		Dictionary<string, string> data = new()
		{
			{ "grant_type", "device_token" },
			{ "client_id", LCHelper.GetClientId(useChinaEndpoint) },
			{ "secret_type", "hmac-sha-1" },
			{ "code", qrCodeData.DeviceCode },
			{ "version", "1.0" },
			{ "platform", "unity" },
			{ "info", "{\"device_id\":\"" + qrCodeData.DeviceID + "\"}" }
		};
		try
		{
			TapTapTokenData token = await Request<TapTapTokenData>(GetTokenUrl(useChinaEndpoint), HttpMethod.Post, useChinaEndpoint, data: data);
			return token;
		}
		catch (RequestException ex)
		{
			if (ex.Failing == FailingType.None) // shouldnt happen
				return null;
			else if (ex.Failing == FailingType.Unknown)
				throw;

			return null;
		}
	}
	/// <summary>
	/// Get the profile data of the user.
	/// </summary>
	/// <param name="token">The token gotten from <see cref="CheckQRCodeResult(CompleteQRCodeData, bool)"/>.</param>
	/// <param name="useChinaEndpoint">Use China endpoints to login or not.</param>
	/// <param name="timestamp">[Unknown]</param>
	/// <returns>The TapTap user profile.</returns>
	public static async Task<TapTapProfileData> GetProfile(TapTapTokenData.TokenData token, int timestamp = 0, bool useChinaEndpoint = true)
	{
		ArgumentNullException.ThrowIfNull(token, nameof(token));
		bool hasPublicProfile = token.Scope.Contains("public_profile");
		string url = GetProfileUrl(useChinaEndpoint, hasPublicProfile) + LCHelper.GetClientId(useChinaEndpoint);
		Uri uri = new(url);
		int ts = timestamp;
		if (ts == 0)
		{
			TimeSpan dt = DateTime.UtcNow - new DateTime(1970, 1, 1);
			ts = (int)dt.TotalSeconds;
		}
		string sign = "MAC " + GetAuthorizationHeader(token.Kid,
			token.MacKey,
			token.MacAlgorithm,
			"GET",
			uri.PathAndQuery,
			uri.Host,
			"443",
			ts
		);
		Dictionary<string, object> headers = new()
		{
			{ "Authorization", sign }
		};
		TapTapProfileData response = await Request<TapTapProfileData>(url, HttpMethod.Get, useChinaEndpoint, headers: headers);
		return response;
	}
	internal static string GetAuthorizationHeader(string kid,
		string macKey,
		string macAlgorithm,
		string method,
		string uri,
		string host,
		string port,
		int timestamp)
	{
		string nonce = new Random().Next().ToString();

		string normalizedString = $"{timestamp}\n{nonce}\n{method}\n{uri}\n{host}\n{port}\n\n";
		HashAlgorithm hashGenerator = macAlgorithm switch
		{
			"hmac-sha-256" => new HMACSHA256(Encoding.ASCII.GetBytes(macKey)),
			"hmac-sha-1" => new HMACSHA1(Encoding.ASCII.GetBytes(macKey)),
			_ => throw new InvalidOperationException("Unsupported MAC algorithm"),
		};
		string hash = Convert.ToBase64String(hashGenerator.ComputeHash(Encoding.ASCII.GetBytes(normalizedString)));

		StringBuilder authorizationHeader = new();
		authorizationHeader.AppendFormat(@"id=""{0}"",ts=""{1}"",nonce=""{2}"",mac=""{3}""",
			kid, timestamp, nonce, hash);

		return authorizationHeader.ToString();
	}
	internal static async Task<T> Request<T>(string url, // why do we have 2 same helper doing this bruh
			HttpMethod method,
			bool useChinaEndpoint,
			Dictionary<string, object>? headers = null,
			object? data = null,
			Dictionary<string, object>? queryParams = null)
	{
		HttpClient client = GetClient(useChinaEndpoint);
		url = BuildUrl(url, queryParams);
		HttpRequestMessage request = new()
		{
			RequestUri = new Uri(url),
			Method = method,
		};
		// request.SetNoCors();
		await FillHeaders(request.Headers, headers);

		string? content = null;
		if (data != null)
		{
			content = JsonSerializer.Serialize(data, Save.SerializerSettings);
			Dictionary<string, string> formData = JsonSerializer.Deserialize<Dictionary<string, object>>(content, Save.SerializerSettings)!
				.ToDictionary(item => item.Key, item => item.Value.ToString()!);
			FormUrlEncodedContent requestContent = new(formData);
			request.Content = requestContent;
		}
		HttpResponseMessage response;
		if (Proxy is not null)
		{
			response = await Proxy(client, request);
		}
		else
		{
			response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
		}
		request.Dispose();

		string resultString = await response.Content.ReadAsStringAsync();
		HttpStatusCode statusCode = response.StatusCode;
		response.Dispose();

		if (response.IsSuccessStatusCode)
		{
			T ret = JsonSerializer.Deserialize<T>(resultString).EnsureNotNull();
			return ret;
		}
		JsonNode parsed = JsonNode.Parse(resultString).EnsureNotNull();
		FailingType type = parsed["data"].EnsureNotNull()["error"].EnsureNotNull().GetValue<string>() switch
		{
			"authorization_pending" => FailingType.Pending,
			"authorization_waiting" => FailingType.Waiting,
			"access_denied" => FailingType.Denied,
			"" => FailingType.None,
			_ => FailingType.Unknown
		};
		throw new RequestException(resultString, statusCode, type);
	}

	private static string BuildUrl(string url, Dictionary<string, object>? queryParams)
	{
		if (queryParams != null)
		{
			IEnumerable<string> queryPairs = queryParams.Select(kv => $"{HttpUtility.UrlEncode(kv.Key)}={HttpUtility.UrlEncode(kv.Value.ToString())}");
			string queries = string.Join("&", queryPairs);

			if (string.IsNullOrEmpty(url))
				url = queries;
			else
				url = $"{url}?{queries}";
		}
		return url;
	}

	private static async Task FillHeaders(HttpRequestHeaders headers, Dictionary<string, object>? reqHeaders = null)
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
		string hash = await LCHelper.GetMD5HashHexString(timestamp.ToString());
		string sign = $"{hash},{timestamp}";
		headers.Add("X-LC-Sign", sign);
	}
}
