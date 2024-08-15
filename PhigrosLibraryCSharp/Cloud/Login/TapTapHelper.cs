﻿using Newtonsoft.Json;
using PhigrosLibraryCSharp.Cloud.Login.DataStructure;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using static PhigrosLibraryCSharp.Cloud.Login.DataStructure.RequestException;

namespace PhigrosLibraryCSharp.Cloud.Login;
/// <summary>
/// A helper to assist you login at TapTap.
/// </summary>
public static class TapTapHelper
{
	#region Constants
	internal const string TapSDKVersion = "2.1";

	internal static readonly string AssemblyVersion = (typeof(TapTapHelper).Assembly.GetName().Version?.ToString() ?? "unknown_version")
		.Replace(" ", "_", StringComparison.InvariantCultureIgnoreCase);
	internal static readonly string AssemblyName = (typeof(TapTapHelper).Assembly.GetName().Name ?? "unknown_dll")
		.Replace(" ", "_", StringComparison.InvariantCultureIgnoreCase);
	private static readonly HttpClient _client = new();
	#endregion

	#region Endpoints
	/// <summary>
	/// When on WASM, you may need to set this property to have cors disabled. (Browser limit)
	/// Or, when you just want to use a proxy.
	/// </summary>
	public static Func<HttpClient, HttpRequestMessage, Task<HttpResponseMessage>>? Proxy { get; set; }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

	public static string ChinaWebHost { get; } = @"https://accounts.tapapis.cn";
	public static string ChinaApiHost { get; } = @"https://open.tapapis.cn";
	public static string ChinaCodeUrl { get; } = ChinaWebHost + @"/oauth2/v1/device/code";
	public static string ChinaTokenUrl { get; } = ChinaWebHost + @"/oauth2/v1/token";
	public static string GetChinaProfileUrl(bool havePublicProfile = true)
		=> havePublicProfile ? ChinaApiHost + "/account/profile/v1?client_id=" : ChinaApiHost + "/account/basic-info/v1?client_id=";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
	#endregion

	/// <summary>
	/// Request a url for user to login.
	/// </summary>
	/// <param name="permissions">Extra permissions to ask, provide <see langword="null"/> to use default permissions.</param>
	/// <returns>A completed collection of QRCode data. See <see cref="CompleteQRCodeData"/> for more information.</returns>
	/// <seealso cref="CompleteQRCodeData"/>
	public static async Task<CompleteQRCodeData> RequestLoginQrCode(string[]? permissions = null)
	{
		string clientId = $"{AssemblyName}-{AssemblyVersion}-" +
			$"{(DateTime.UtcNow - DateTime.UnixEpoch).TotalMilliseconds}-{Random.Shared.Next(0, 114514):N}";
		Dictionary<string, object> parameters = new()
		{
			{ "client_id", LCHelper.ClientId },
			{ "response_type", "device_code" },
			{ "scope", string.Join(",", permissions ?? new string[] { "public_profile" }) },
			{ "version", TapSDKVersion },
			{ "platform", "unity" },
			{ "info", "{\"device_id\":\"" + clientId + "\"}" } 
			// ^ https://github.com/taptap/TapSDK-UE4/blob/f66d15048ebff4628f1614ca8df8a7a07dabf6cb/TapCommon/Source/TapCommon/Tools/TUDeviceInfo.h#L30 
		};
		return new(await Request<PartialTapTapQRCodeData>(ChinaCodeUrl, HttpMethod.Post, data: parameters), clientId);
	}
	/// <summary>
	/// Check if the user has logged in through QRCode.
	/// </summary>
	/// <param name="qrCodeData">The completed QRCode data from <see cref="RequestLoginQrCode(string[])"/>.</param>
	/// <returns><see langword="null"/> if not verified, the token data if verified.</returns>
	/// <exception cref="Exception">Received unknown response</exception>
	public static async Task<TapTapTokenData?> CheckQRCodeResult(CompleteQRCodeData qrCodeData)
	{
		Dictionary<string, string> data = new()
		{
			{ "grant_type", "device_token" },
			{ "client_id", LCHelper.ClientId },
			{ "secret_type", "hmac-sha-1" },
			{ "code", qrCodeData.DeviceCode },
			{ "version", "1.0" },
			{ "platform", "unity" },
			{ "info", "{\"device_id\":\"" + qrCodeData.DeviceID + "\"}" }
		};
		try
		{
			TapTapTokenData token = await Request<TapTapTokenData>(ChinaTokenUrl, HttpMethod.Post, data: data);
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
	/// <param name="token">The token gotten from <see cref="CheckQRCodeResult(CompleteQRCodeData)"/>.</param>
	/// <param name="timestamp">[Unknown]</param>
	/// <returns>The TapTap user profile.</returns>
	public static async Task<TapTapProfileData> GetProfile(TapTapTokenData.TokenData token, int timestamp = 0)
	{
		ArgumentNullException.ThrowIfNull(token, nameof(token));
		bool hasPublicProfile = token.Scope.Contains("public_profile");
		string url = GetChinaProfileUrl(hasPublicProfile) + LCHelper.ClientId;
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
		TapTapProfileData response = await Request<TapTapProfileData>(url, HttpMethod.Get, headers: headers);
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
		string nonce = new System.Random().Next().ToString();

		string normalizedString = $"{timestamp}\n{nonce}\n{method}\n{uri}\n{host}\n{port}\n\n";

		HashAlgorithm hashGenerator;
		switch (macAlgorithm)
		{
			case "hmac-sha-256":
				hashGenerator = new HMACSHA256(Encoding.ASCII.GetBytes(macKey));
				break;
			case "hmac-sha-1":
				hashGenerator = new HMACSHA1(Encoding.ASCII.GetBytes(macKey));
				break;
			default:
				throw new InvalidOperationException("Unsupported MAC algorithm");
		}

		string hash = Convert.ToBase64String(hashGenerator.ComputeHash(Encoding.ASCII.GetBytes(normalizedString)));

		StringBuilder authorizationHeader = new();
		authorizationHeader.AppendFormat(@"id=""{0}"",ts=""{1}"",nonce=""{2}"",mac=""{3}""",
			kid, timestamp, nonce, hash);

		return authorizationHeader.ToString();
	}
	internal static async Task<T> Request<T>(string url, // why do we have 2 same helper doing this bruh
			HttpMethod method,
			Dictionary<string, object>? headers = null,
			object? data = null,
			Dictionary<string, object>? queryParams = null)
	{
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
			content = JsonConvert.SerializeObject(data);
			Dictionary<string, string> formData = JsonConvert.DeserializeObject<Dictionary<string, object>>(content)!
				.ToDictionary(item => item.Key, item => item.Value.ToString()!);
			FormUrlEncodedContent requestContent = new(formData);
			request.Content = requestContent;
		}
		HttpResponseMessage response;
		if (Proxy is not null)
		{
			response = await Proxy(_client, request);
		}
		else
		{
			response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
		}
		request.Dispose();

		string resultString = await response.Content.ReadAsStringAsync();
		HttpStatusCode statusCode = response.StatusCode;
		response.Dispose();

		if (response.IsSuccessStatusCode)
		{
			T ret = JsonConvert.DeserializeObject<T>(resultString)!;
			return ret;
		}
		TapTapTokenErrorResponse parsed = JsonConvert.DeserializeObject<TapTapTokenErrorResponse>(resultString)!;
		FailingType type = parsed.Data.Error switch
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
			IEnumerable<string> queryPairs = queryParams.Select(kv => $"{kv.Key}={kv.Value}");
			string queries = string.Join("&", queryPairs);
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
