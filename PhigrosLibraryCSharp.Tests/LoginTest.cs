using PhigrosLibraryCSharp.CloudSave;
using PhigrosLibraryCSharp.CloudSave.Login;
using System.Collections.Specialized;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Web;

namespace PhigrosLibraryCSharp.Tests;

[TestClass]
[DoNotParallelize]
public class LoginTest
{
	[TestInitialize]
	public void Initialize()
	{
		LCHelper.GetMD5HashHexString = (input, ct) => Task.FromResult("example_md5");
	}
	private Func<HttpClient, HttpRequestMessage, Task<HttpResponseMessage>> GetTapTapMockProxy(
		HttpResponseMessage response,
		Uri url,
		HttpMethod method,
		Func<HttpRequestMessage, Task>? bodyTester = null,
		Func<HttpRequestMessage, Task>? headerTester = null)
	{
		return Core;

		async Task<HttpResponseMessage> Core(HttpClient client, HttpRequestMessage request)
		{
			Assert.AreEqual(method, request.Method);
			Assert.AreEqual(url, request.RequestUri);

			Regex lcSignRegex = new("example_md5,([0-9]+)");
			GroupCollection lcSignGroups = lcSignRegex.Match(request.Headers.GetValues("X-LC-Sign").First()).Groups;
			Assert.HasCount(2, lcSignGroups);
			Assert.AreEqual((double)long.Parse(lcSignGroups[1].Value), DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), 50);

			if (bodyTester is not null) await bodyTester.Invoke(request);
			if (headerTester is not null) await headerTester.Invoke(request);
			return response;
		}
	}

	#region TapTap

	[TestMethod]
	public void TestGenerateCallbackLogin()
	{
		// permissions are fake
		CallbackLoginData result = TapTapHelper.GenerateCallbackLoginUrl("http://localhost:6942/complete", false, ["public_profile", "private_profile"]);

		Uri beginUri = new(result.BeginUrl);
		NameValueCollection beginQuery = HttpUtility.ParseQueryString(beginUri.Query);

		Assert.AreEqual(result.RedirectUrl, beginQuery["redirect_uri"]);
		Assert.AreEqual(result.State, beginQuery["state"]);
		Assert.AreEqual(result.CodeChallenge, beginQuery["code_challenge"]);
		Assert.AreEqual(result.Scope, beginQuery["scope"]);

		Assert.AreEqual("kviehleldgxsagpozb", beginQuery["client_id"]);
		Assert.AreEqual("code", beginQuery["response_type"]);
		Assert.AreEqual("http://localhost:6942/complete", beginQuery["redirect_uri"]);
		Assert.AreEqual("S256", beginQuery["code_challenge_method"]);
		Assert.AreEqual("pc_localhost", beginQuery["flow"]);

		Assert.IsTrue(Guid.TryParse(result.State, out _));
		Assert.AreEqual("public_profile,private_profile", result.Scope);
		Assert.IsTrue(Regex.IsMatch(result.CodeVerifier, @"[0-9a-zA-Z~_\.\-]{128}"));
		Assert.AreEqual(
			Convert.ToBase64String(SHA256.HashData(Encoding.ASCII.GetBytes(result.CodeVerifier))).Replace("=", "").Replace('+', '-').Replace('/', '_'),
			result.CodeChallenge);
	}

	[TestMethod]
	public async Task TestHandleCallbackLogin()
	{
		CallbackLoginData loginData = new("code_verifier1", "http://localhost:6942/complete", "state1", "code_challenge1", "scope1,scope2", "http://real.taptap.com/");

		TapTapHelper.Proxy = this.GetTapTapMockProxy(HttpResponseMessage.CreateJson("""
			{
				"data": {
					"kid": "example_kid",
					"access_token": "example_access_token",
					"token_type": "Bearer",
					"mac_key": "example_mac_key",
					"mac_algorithm": "hmac-sha-1",
					"scope": "public_profile,private_profile"
				}
			}
			"""),
			new Uri(TapTapHelper.GetTokenUrl(false)),
			HttpMethod.Post,
			async message =>
			{
				string content = await message.Content!.ReadAsStringAsync();
				NameValueCollection decoded = HttpUtility.ParseQueryString(content);
				Assert.AreEqual(LCHelper.GetClientId(false), decoded["client_id"]);
				Assert.AreEqual("authorization_code", decoded["grant_type"]);
				Assert.AreEqual("hmac-sha-1", decoded["secret_type"]);
				Assert.AreEqual("login_code123", decoded["code"]);
				Assert.AreEqual(loginData.RedirectUrl, decoded["redirect_uri"]);
				Assert.AreEqual(loginData.CodeVerifier, decoded["code_verifier"]);
			});

		TapTapTokenData result = await TapTapHelper.HandleCallbackLogin(loginData, "login_code123", false);

		Assert.AreEqual("example_kid", result.Data.Kid);
		Assert.AreEqual("example_access_token", result.Data.Token);
		Assert.AreEqual("Bearer", result.Data.TokenType);
		Assert.AreEqual("example_mac_key", result.Data.MacKey);
		Assert.AreEqual("hmac-sha-1", result.Data.MacAlgorithm);
		Assert.AreEqual("public_profile,private_profile", result.Data.Scope);
	}

	[TestMethod]
	public async Task TestRequestLoginQrCode()
	{
		TapTapHelper.Proxy = this.GetTapTapMockProxy(HttpResponseMessage.CreateJson("""
			{
				"data": {
					"device_code": "example_device_code",
					"expires_in": 300,
					"qrcode_url": "https://taptap.local/qr/example",
					"interval": 5
				}
			}
			"""),
			new Uri(TapTapHelper.GetCodeUrl(false)),
			HttpMethod.Post,
			async message =>
			{
				string content = await message.Content!.ReadAsStringAsync();
				NameValueCollection decoded = HttpUtility.ParseQueryString(content);
				Assert.AreEqual(LCHelper.GetClientId(false), decoded["client_id"]);
				Assert.AreEqual("device_code", decoded["response_type"]);
				Assert.AreEqual("public_profile", decoded["scope"]);
				Assert.AreEqual(TapTapHelper.TapSDKVersion, decoded["version"]);
				Assert.AreEqual("unity", decoded["platform"]);
				Assert.IsNotNull(decoded["info"]);

				// verify info is a JSON object containing device_id
				string infoJson = decoded["info"]!;
				JsonNode infoNode = JsonNode.Parse(infoJson)!.AsObject();
				string deviceId = infoNode["device_id"]!.GetValue<string>();
				Assert.IsFalse(string.IsNullOrEmpty(deviceId));
			});

		CompleteQRCodeData result = await TapTapHelper.RequestLoginQrCode(useChinaEndpoint: false);

		Assert.AreEqual("example_device_code", result.DeviceCode);
		Assert.AreEqual(300, result.ExpiresInSeconds);
		Assert.AreEqual("https://taptap.local/qr/example", result.Url);
		Assert.AreEqual(5, result.Interval);
		Assert.IsFalse(string.IsNullOrEmpty(result.DeviceID));
	}

	[TestMethod]
	public async Task TestCheckQRCodeResultSuccess()
	{
		CompleteQRCodeData qrCodeData = new()
		{
			DeviceID = "test_device_id",
			DeviceCode = "test_device_code",
			ExpiresInSeconds = 300,
			Url = "https://taptap.local/qr/test",
			Interval = 5,
		};

		TapTapHelper.Proxy = this.GetTapTapMockProxy(HttpResponseMessage.CreateJson("""
			{
				"data": {
					"kid": "qr_kid",
					"access_token": "qr_access_token",
					"token_type": "Bearer",
					"mac_key": "qr_mac_key",
					"mac_algorithm": "hmac-sha-1",
					"scope": "public_profile"
				}
			}
			"""),
			new Uri(TapTapHelper.GetTokenUrl(false)),
			HttpMethod.Post,
			async message =>
			{
				string content = await message.Content!.ReadAsStringAsync();
				NameValueCollection decoded = HttpUtility.ParseQueryString(content);
				Assert.AreEqual(LCHelper.GetClientId(false), decoded["client_id"]);
				Assert.AreEqual("device_token", decoded["grant_type"]);
				Assert.AreEqual("hmac-sha-1", decoded["secret_type"]);
				Assert.AreEqual("test_device_code", decoded["code"]);
				Assert.AreEqual("1.0", decoded["version"]);
				Assert.AreEqual("unity", decoded["platform"]);

				string infoJson = decoded["info"]!;
				JsonNode infoNode = JsonNode.Parse(infoJson)!.AsObject();
				Assert.AreEqual("test_device_id", infoNode["device_id"]!.GetValue<string>());
			});

		TapTapTokenData? result = await TapTapHelper.CheckQRCodeResult(qrCodeData, useChinaEndpoint: false);

		Assert.IsNotNull(result);
		Assert.AreEqual("qr_kid", result!.Data.Kid);
		Assert.AreEqual("qr_access_token", result.Data.Token);
		Assert.AreEqual("Bearer", result.Data.TokenType);
		Assert.AreEqual("qr_mac_key", result.Data.MacKey);
		Assert.AreEqual("hmac-sha-1", result.Data.MacAlgorithm);
		Assert.AreEqual("public_profile", result.Data.Scope);
	}

	[DataRow("authorization_pending", null)]
	[DataRow("authorization_waiting", null)]
	[DataRow("access_denied", null)]
	[TestMethod]
	public async Task TestCheckQRCodeResultFailingReturnsNull(string error, object? _)
	{
		CompleteQRCodeData qrCodeData = new()
		{
			DeviceID = "test_device_id",
			DeviceCode = "test_device_code",
			ExpiresInSeconds = 300,
			Url = "https://taptap.local/qr/test",
			Interval = 5,
		};

		TapTapHelper.Proxy = this.GetTapTapMockProxy(
			HttpResponseMessage.CreateJson($$"""
				{
					"data": {
						"error": "{{error}}"
					}
				}
				""", HttpStatusCode.BadRequest),
			new Uri(TapTapHelper.GetTokenUrl(false)),
			HttpMethod.Post);

		TapTapTokenData? result = await TapTapHelper.CheckQRCodeResult(qrCodeData, useChinaEndpoint: false);
		Assert.IsNull(result);
	}

	[TestMethod]
	public async Task TestCheckQRCodeResultUnknownErrorThrows()
	{
		CompleteQRCodeData qrCodeData = new()
		{
			DeviceID = "test_device_id",
			DeviceCode = "test_device_code",
			ExpiresInSeconds = 300,
			Url = "https://taptap.local/qr/test",
			Interval = 5,
		};

		TapTapHelper.Proxy = this.GetTapTapMockProxy(
			HttpResponseMessage.CreateJson("""
				{
					"data": {
						"error": "some_unknown_error"
					}
				}
				""", HttpStatusCode.InternalServerError),
			new Uri(TapTapHelper.GetTokenUrl(false)),
			HttpMethod.Post);

		await Assert.ThrowsAsync<RequestException>(async () =>
			await TapTapHelper.CheckQRCodeResult(qrCodeData, useChinaEndpoint: false));
	}

	[TestMethod]
	public async Task TestGetProfile()
	{
		TapTapTokenData.TokenData token = new()
		{
			Kid = "profile_kid",
			Token = "profile_access_token",
			TokenType = "Bearer",
			MacKey = "profile_mac_key",
			MacAlgorithm = "hmac-sha-1",
			Scope = "public_profile",
		};

		Uri expectedUrl = new(TapTapHelper.GetProfileUrl(false, true) + LCHelper.GetClientId(false));

		TapTapHelper.Proxy = this.GetTapTapMockProxy(HttpResponseMessage.CreateJson("""
			{
				"data": {
					"openid": "example_open_id",
					"unionid": "example_union_id",
					"name": "example_name",
					"gender": "male",
					"avatar": "https://taptap.local/avatar.png"
				}
			}
			"""),
			expectedUrl,
			HttpMethod.Get,
			bodyTester: null,
			headerTester: message =>
			{
				IEnumerable<string> authHeaders = message.Headers.GetValues("Authorization");
				string auth = authHeaders.First();
				Assert.StartsWith("MAC ", auth, $"Expected MAC authorization, got: {auth}");
				Assert.Contains("id=\"profile_kid\"", auth, "Authorization should contain kid");
				Assert.Contains("mac=\"", auth, "Authorization should contain mac");
				Assert.Contains("ts=\"", auth, "Authorization should contain ts");
				Assert.Contains("nonce=\"", auth, "Authorization should contain nonce");
				return Task.CompletedTask;
			});

		TapTapProfileData result = await TapTapHelper.GetProfile(token, useChinaEndpoint: false);

		Assert.AreEqual("example_open_id", result.Data.OpenId);
		Assert.AreEqual("example_union_id", result.Data.UnionId);
		Assert.AreEqual("example_name", result.Data.Name);
		Assert.AreEqual("male", result.Data.Gender);
		Assert.AreEqual("https://taptap.local/avatar.png", result.Data.Avatar);
	}

	[TestMethod]
	public async Task TestGetProfileBasicInfoUrl()
	{
		// when the token scope does not contain public_profile, the basic-info endpoint should be used
		TapTapTokenData.TokenData token = new()
		{
			Kid = "basic_kid",
			Token = "basic_access_token",
			TokenType = "Bearer",
			MacKey = "basic_mac_key",
			MacAlgorithm = "hmac-sha-1",
			Scope = "private_profile",
		};

		Uri expectedUrl = new(TapTapHelper.GetProfileUrl(false, false) + LCHelper.GetClientId(false));
		Assert.Contains("basic-info", expectedUrl.AbsolutePath);

		TapTapHelper.Proxy = this.GetTapTapMockProxy(HttpResponseMessage.CreateJson("""
			{
				"data": {
					"openid": "basic_open_id",
					"unionid": "basic_union_id",
					"name": "basic_name",
					"gender": "female",
					"avatar": "https://taptap.local/basic.png"
				}
			}
			"""),
			expectedUrl,
			HttpMethod.Get);

		TapTapProfileData result = await TapTapHelper.GetProfile(token, useChinaEndpoint: false);

		Assert.AreEqual("basic_open_id", result.Data.OpenId);
		Assert.AreEqual("basic_name", result.Data.Name);
	}

	#endregion

	#region LC

	[TestMethod]
	public void TestGetClientId()
	{
		Assert.AreEqual(LCHelper.InternationalClientId, LCHelper.GetClientId(false));
		Assert.AreEqual(LCHelper.ClientId, LCHelper.GetClientId(true));
	}
	[TestMethod]
	public void TestGetAppKey()
	{
		Assert.AreEqual(LCHelper.InternationalAppKey, LCHelper.GetAppKey(false));
		Assert.AreEqual(LCHelper.AppKey, LCHelper.GetAppKey(true));
	}

	private static LCCombinedAuthData CreateTestAuthData()
	{
		TapTapProfileData.ProfileData profile = new()
		{
			OpenId = "lc_open_id",
			UnionId = "lc_union_id",
			Name = "lc_name",
			Gender = "male",
			Avatar = "https://taptap.local/lc.png",
		};
		TapTapTokenData.TokenData token = new()
		{
			Kid = "lc_kid",
			Token = "lc_access_token",
			TokenType = "Bearer",
			MacKey = "lc_mac_key",
			MacAlgorithm = "hmac-sha-1",
			Scope = "public_profile",
		};
		return new(profile, token);
	}

	[TestMethod]
	public async Task TestLoginWithAuthData()
	{
		LCCombinedAuthData authData = CreateTestAuthData();
		bool useChina = false;

		string expectedUrl = $"{Save.GetCloudServerAddress(useChina)}/1.1/users";

		TapTapHelper.Proxy = this.GetTapTapMockProxy(HttpResponseMessage.CreateJson("""
			{
				"objectId": "lc_object_id",
				"sessionToken": "lc_session_token_12345",
				"username": "lc_username"
			}
			"""),
			new Uri(expectedUrl),
			HttpMethod.Post,
			async message =>
			{
				IEnumerable<string> lcIdHeaders = message.Headers.GetValues("X-LC-Id");
				Assert.AreEqual(LCHelper.GetClientId(useChina), lcIdHeaders.First());

				string content = await message.Content!.ReadAsStringAsync();
				JsonNode root = JsonNode.Parse(content)!.AsObject();
				JsonNode authDataNode = root["authData"]!.AsObject();
				JsonNode taptapNode = authDataNode["taptap"]!.AsObject();

				Assert.AreEqual("lc_kid", taptapNode["kid"]!.GetValue<string>());
				Assert.AreEqual("lc_access_token", taptapNode["access_token"]!.GetValue<string>());
				Assert.AreEqual("Bearer", taptapNode["token_type"]!.GetValue<string>());
				Assert.AreEqual("lc_mac_key", taptapNode["mac_key"]!.GetValue<string>());
				Assert.AreEqual("hmac-sha-1", taptapNode["mac_algorithm"]!.GetValue<string>());
				Assert.AreEqual("lc_open_id", taptapNode["openid"]!.GetValue<string>());
				Assert.AreEqual("lc_name", taptapNode["name"]!.GetValue<string>());
				Assert.AreEqual("https://taptap.local/lc.png", taptapNode["avatar"]!.GetValue<string>());
				Assert.AreEqual("lc_union_id", taptapNode["unionid"]!.GetValue<string>());
			});

		JsonNode result = await LCHelper.LoginWithAuthData(authData, useChinaEndpoint: useChina);

		Assert.AreEqual("lc_object_id", result["objectId"]!.GetValue<string>());
		Assert.AreEqual("lc_session_token_12345", result["sessionToken"]!.GetValue<string>());
	}

	[TestMethod]
	public async Task TestLoginWithAuthDataFailOnNotExist()
	{
		LCCombinedAuthData authData = CreateTestAuthData();
		bool useChina = false;

		string expectedUrl = $"{Save.GetCloudServerAddress(useChina)}/1.1/users?failOnNotExist=true";

		TapTapHelper.Proxy = this.GetTapTapMockProxy(HttpResponseMessage.CreateJson("""
			{
				"objectId": "lc_object_id_2",
				"sessionToken": "lc_session_token_67890"
			}
			"""),
			new Uri(expectedUrl),
			HttpMethod.Post,
			headerTester: message =>
			{
				IEnumerable<string> lcIdHeaders = message.Headers.GetValues("X-LC-Id");
				Assert.AreEqual(LCHelper.GetClientId(useChina), lcIdHeaders.First());
				return Task.CompletedTask;
			});

		JsonNode result = await LCHelper.LoginWithAuthData(authData, useChinaEndpoint: useChina, failOnNotExist: true);
		Assert.AreEqual("lc_object_id_2", result["objectId"]!.GetValue<string>());
	}

	[TestMethod]
	public async Task TestLoginAndGetToken()
	{
		LCCombinedAuthData authData = CreateTestAuthData();
		bool useChina = false;

		string expectedUrl = $"{Save.GetCloudServerAddress(useChina)}/1.1/users";

		TapTapHelper.Proxy = this.GetTapTapMockProxy(HttpResponseMessage.CreateJson("""
			{
				"objectId": "lc_object_id",
				"sessionToken": "lc_session_token_12345"
			}
			"""),
			new Uri(expectedUrl),
			HttpMethod.Post,
			headerTester: message =>
			{
				IEnumerable<string> lcIdHeaders = message.Headers.GetValues("X-LC-Id");
				Assert.AreEqual(LCHelper.GetClientId(useChina), lcIdHeaders.First());
				return Task.CompletedTask;
			});

		string token = await LCHelper.LoginAndGetToken(authData, useChinaEndpoint: useChina);
		Assert.AreEqual("lc_session_token_12345", token);
	}

	[TestMethod]
	public async Task TestLoginWithAuthDataChinaEndpoint()
	{
		LCCombinedAuthData authData = CreateTestAuthData();
		bool useChina = true;

		string expectedUrl = $"{Save.GetCloudServerAddress(useChina)}/1.1/users";

		TapTapHelper.Proxy = this.GetTapTapMockProxy(HttpResponseMessage.CreateJson("""
			{
				"sessionToken": "china_session_token"
			}
			"""),
			new Uri(expectedUrl),
			HttpMethod.Post,
			headerTester: message =>
			{
				// China endpoint should use China client id
				IEnumerable<string> lcIdHeaders = message.Headers.GetValues("X-LC-Id");
				Assert.AreEqual(LCHelper.GetClientId(useChina), lcIdHeaders.First());
				return Task.CompletedTask;
			});

		string token = await LCHelper.LoginAndGetToken(authData, useChinaEndpoint: useChina);
		Assert.AreEqual("china_session_token", token);
	}

	#endregion
}
