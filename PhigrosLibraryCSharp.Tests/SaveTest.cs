using PhigrosLibraryCSharp.CloudSave;
using PhigrosLibraryCSharp.CloudSave.HttpModels;
using PhigrosLibraryCSharp.CloudSave.Login;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace PhigrosLibraryCSharp.Tests;

[TestClass]
public class SaveTest
{
	private record class SaveCredentials(string Token, bool IsInternational);

	private Func<Save, HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> GetMockHandler(
		SaveCredentials cred,
		HttpRequestMessage expectedMessage,
		HttpResponseMessage response)
	{
		return Core;
		async Task<HttpResponseMessage> Core(Save save, HttpRequestMessage request, CancellationToken _)
		{
			Assert.AreEqual(expectedMessage.Method, request.Method);
			Assert.AreEqual(expectedMessage.RequestUri, request.RequestUri);
			Assert.AreSequenceEqual(expectedMessage.Headers.GetValues("X-LC-Id"), [cred.IsInternational ? LCHelper.InternationalClientId : LCHelper.ClientId]);
			Assert.AreSequenceEqual(expectedMessage.Headers.GetValues("X-LC-Key"), [cred.IsInternational ? LCHelper.InternationalAppKey : LCHelper.AppKey]);
			Assert.AreSequenceEqual(expectedMessage.Headers.GetValues("X-LC-Session"), [cred.Token]);
			// ua is important to not trigger anti bot measures
			Assert.AreSequenceEqual(expectedMessage.Headers.GetValues("User-Agent"), ["LeanCloud-CSharp-SDK/1.0.3"]);

			using MemoryStream requestMemoryStream = new();
			using MemoryStream expectedMemoryStream = new();
			using Stream requestContentStream = request.Content?.ReadAsStream() ?? Stream.Null;
			using Stream expectedContentStream = expectedMessage.Content?.ReadAsStream() ?? Stream.Null;

			await requestContentStream.CopyToAsync(requestMemoryStream);
			await expectedContentStream.CopyToAsync(expectedMemoryStream);

			Assert.AreSequenceEqual(expectedMemoryStream.ToArray(), requestMemoryStream.ToArray());

			return response;
		}
	}
	private ExposedSave PrepareSave(out SaveCredentials creds)
	{
		string tokenString = new("testToken1234567890abcdef");
		bool isInternational = true;

		creds = new(tokenString, isInternational);

		// to stop the compiler from complaining about the closure capturing out var
		SaveCredentials credClone = creds;
		return new(tokenString, isInternational);
	}

	[DataRow("cGhpZ3JvcyBvLw==", "2xn+1g7UNjkrRmEO7Cw/oA==")]
	[TestMethod]
	public async Task TestDefaultEncryption(string input, string expected)
	{
		ExposedSave save = this.PrepareSave(out SaveCredentials? creds);

		byte[] realInput = Convert.FromBase64String(input);
		byte[] realExpected = Convert.FromBase64String(expected);

		byte[] encrypted = await save.Encrypt(realInput);
		Assert.AreSequenceEqual(realExpected, encrypted);
		byte[] decrypted = await save.Decrypt(encrypted);
		Assert.AreSequenceEqual(realInput, decrypted);
	}

	[DataRow("bx6cP/eVyZULZpDFRPsehwW1hih+MLwNSV+51jcv8/I=", "bm9tIG5vbSBub20gbm9tIG1pc3VpIHl1bW15")]
	[TestMethod]
	public async Task TestDefaultDecryption(string input, string expected)
	{
		ExposedSave save = this.PrepareSave(out SaveCredentials? creds);

		byte[] realInput = Convert.FromBase64String(input);
		byte[] realExpected = Convert.FromBase64String(expected);

		byte[] decrypted = await save.Decrypt(realInput);
		Assert.AreSequenceEqual(realExpected, decrypted);
		byte[] encrypted = await save.Encrypt(decrypted);
		Assert.AreSequenceEqual(realInput, encrypted);
	}

	[TestMethod]
	public void TestTokenSemantics()
	{
		Assert.IsFalse(Save.IsSemanticallyValidToken("not  validvalidvalidvalid"));
		Assert.IsFalse(Save.IsSemanticallyValidToken("too1loooooooooooooooooooog"));
		Assert.IsFalse(Save.IsSemanticallyValidToken("too1shoooooooooooooooort"));
		Assert.IsFalse(Save.IsSemanticallyValidToken("some@sus#symbols!=)()*&&#"));

		Assert.IsTrue(Save.IsSemanticallyValidToken("FullyValidTokenTokenToken"));
		Assert.IsTrue(Save.IsSemanticallyValidToken("1234567890123456789012345"));
		Assert.IsTrue(Save.IsSemanticallyValidToken("a1B2c3D4dasdad65ASDtttttt"));
	}

	[DataRow("GET", "http://test.com")]
	[DataRow("POST", "https://example.com")]
	[DataRow("DELETE", "https://www.google.com/search")]
	[TestMethod]
	public void TestInternalDefaultMessage(string method, string url)
	{
		ExposedSave save = this.PrepareSave(out SaveCredentials? creds);

		HttpMethod realMethod = HttpMethod.Parse(method);
		Uri realUrl = new(url);

		HttpRequestMessage request = save.CreateDefaultRequest(realMethod, url);
		Assert.AreSequenceEqual(request.Headers.GetValues("X-LC-Id"), [creds.IsInternational ? LCHelper.InternationalClientId : LCHelper.ClientId]);
		Assert.AreSequenceEqual(request.Headers.GetValues("X-LC-Key"), [creds.IsInternational ? LCHelper.InternationalAppKey : LCHelper.AppKey]);
		Assert.AreSequenceEqual(request.Headers.GetValues("X-LC-Session"), [creds.Token]);
		Assert.AreSequenceEqual(request.Headers.GetValues("Accept"), ["application/json"]);
		Assert.AreSequenceEqual(request.Headers.GetValues("User-Agent"), ["LeanCloud-CSharp-SDK/1.0.3"]);
		Assert.AreEqual(request.Method, realMethod);
		Assert.AreEqual(request.RequestUri, realUrl);
	}


	private ExposedSave PrepareGetPlayerInfoSave(string objectId, string? nickname, string username, DateTime createdAt, DateTime updatedAt)
	{
		ExposedSave save = this.PrepareSave(out SaveCredentials? creds);
		// default message already tested, so we can just trust it
		HttpRequestMessage expected = save.CreateDefaultRequest(HttpMethod.Get, Save.GetAddress(Save.CloudMeAddress, creds.IsInternational));

		// bruh json colorization doesn't work with interpolated raw string literal
		save.RequestHandler = this.GetMockHandler(creds, expected, HttpResponseMessage.CreateJson($$"""
			{
				"objectId": "{{objectId}}",
				{{(nickname is null ? "" : $"\"nickname\": \"{nickname}\",")}}
				"username": "{{username}}",
				"createdAt": "{{createdAt:O}}",
				"updatedAt": "{{updatedAt:O}}"
			}
			"""));
		return save;
	}
	private async Task TestGetPlayerInfo(string objectId, string? nickname, string username, DateTime createdAt, DateTime updatedAt)
	{
		ExposedSave save = this.PrepareGetPlayerInfoSave(objectId, nickname, username, createdAt, updatedAt);
		PlayerInfo info = await save.GetPlayerInfoAsync();

		Assert.AreEqual(objectId, info.ObjectId);
		Assert.AreEqual(nickname, info.NickName);
		Assert.AreEqual(username, info.UserName);
		Assert.AreEqual(createdAt, info.CreationTime);
		Assert.AreEqual(updatedAt, info.ModificationTime);
		Assert.AreEqual(save.CachedUserId, objectId);
	}
	[TestMethod]
	public async Task TestGetPlayerInfoWithNickname()
	{
		await this.TestGetPlayerInfo("testObjectId", "test nick name", "testUsername", DateTime.Parse("2026/05/04 05:23"), DateTime.Parse("2026/08/01 13:10"));
	}
	[TestMethod]
	public async Task TestGetPlayerInfoWithoutNickname()
	{
		await this.TestGetPlayerInfo("obj_id", null, "user 123", DateTime.Parse("2023/06/04 05:23"), DateTime.Parse("2024/08/01 13:10"));
	}

	[TestMethod]
	public async Task TestGetUserObjectIdNoCache()
	{
		string expected = "testObjectId";
		ExposedSave save = this.PrepareGetPlayerInfoSave(expected, "test nick name", "testUsername", DateTime.Parse("2026/05/04 05:23"), DateTime.Parse("2026/08/01 13:10"));
		string objectId = await save.GetUserObjectId();

		Assert.AreEqual(expected, objectId);
		Assert.AreEqual(expected, save.CachedUserId);
	}
	[TestMethod]
	public async Task TestGetUserObjectIdCached()
	{
		string expected = "testObjectId";
		ExposedSave save = this.PrepareGetPlayerInfoSave(expected, "test nick name", "testUsername", DateTime.Parse("2026/05/04 05:23"), DateTime.Parse("2026/08/01 13:10"));
		await save.GetPlayerInfoAsync();

		save.RequestHandler = (_, _, _) => throw new InvalidOperationException("Request handler should not be called when user object id is cached");
		string objectId = await save.GetUserObjectId();

		Assert.AreEqual(expected, objectId);
	}

	private ExposedSave PrepareGetSaveInfoSave(string userObjectId, Dictionary<string, string> queries, [StringSyntax(StringSyntaxAttribute.Json)] string? response = null)
	{
		response ??= """
			{
				"results": [
					{
						"createdAt": "2026-06-08T06:34:40.864Z",
						"gameFile": {
							"__type": "File",
							"bucket": "example_bucket",
							"createdAt": "2026-06-08T06:34:38.630Z",
							"key": "gamesaves/example_key/.save",
							"metaData": {
								"_checksum": "00000000000000000000000000000000",
								"prefix": "gamesaves",
								"size": 69420
							},
							"mime_type": "application/octet-stream",
							"name": ".save",
							"objectId": "example_file_object1",
							"provider": "qiniu",
							"updatedAt": "2026-06-08T06:34:38.630Z",
							"url": "https://phigros.test/1"
						},
						"modifiedAt": {
							"__type": "Date",
							"iso": "2026-06-08T06:34:28.393Z"
						},
						"name": ".save",
						"objectId": "example_object_id",
						"summary": "BVoBW65xQVEQ5re35LmxLUNvbmZ1c2lvbmYAWQBFAIQASgAcAMYATwAVAA0AAQABAA==",
						"updatedAt": "2026-06-08T06:34:40.864Z",
						"user": {
							"__type": "Pointer",
							"className": "_User",
							"objectId": "{{userid}}"
						}
					},
					{
						"createdAt": "2026-06-02T06:34:40.864Z",
						"gameFile": null,
						"modifiedAt": {
							"__type": "Date",
							"iso": "2026-06-02T06:34:28.393Z"
						},
						"name": ".save",
						"objectId": "example_object_id",
						"summary": "BVoBW65xQVEQ5re35LmxLUNvbmZ1c2lvbmYAWQBFAIQASgAcAMYATwAVAA0AAQABAA==",
						"updatedAt": "2026-06-02T06:34:40.864Z",
						"user": {
							"__type": "Pointer",
							"className": "_User",
							"objectId": "{{userid}}"
						}
					}
				]
			}
			""".Replace("{{userid}}", userObjectId); // sorry but i dont wanna lose colorization

		ExposedSave save = this.PrepareSave(out SaveCredentials? creds);
		UriBuilder uriBuilder = new(Save.GetAddress(Save.CloudGameSaveAddress, creds.IsInternational));
		StringBuilder builder = new();
		foreach (KeyValuePair<string, string> item in queries)
		{
			builder.Append($"{item.Key}={Uri.EscapeDataString(item.Value)}&");
		}
		builder.Length--; // remove the last '&'

		uriBuilder.Query = builder.ToString();

		HttpRequestMessage expected = save.CreateDefaultRequest(HttpMethod.Get, uriBuilder.ToString());
		save.RequestHandler = this.GetMockHandler(creds, expected, HttpResponseMessage.CreateJson(response));
		save.CachedUserId = userObjectId;
		return save;
	}
	[TestMethod]
	public async Task TestGetSaveInfoDefaultQuery()
	{
		string objectId = "testing object id";
		ExposedSave save = this.PrepareGetSaveInfoSave(objectId,
			new() { ["where"] = $"{{\"user\":{{\"__type\":\"Pointer\",\"className\":\"_User\",\"objectId\":\"{objectId}\"}}}}" });

		SaveInfoContainer container = await save.GetSaveInfoFromCloudAsync();

		// asserting only important things here
		Assert.HasCount(2, container.Results);
		Assert.AreEqual("https://phigros.test/1", container.Results[0].GameFile?.Url);
		Assert.IsNull(container.Results[1].GameFile);
	}
	[TestMethod]
	public async Task TestGetSaveInfoMultipleQuery()
	{
		string objectId = "testing object id2";
		Dictionary<string, string> queries = new()
		{
			["where"] = $"{{\"user\":{{\"__type\":\"Pointer\",\"className\":\"_User\",\"objectId\":\"{objectId}\"}}}}",
			["limit"] = "10",
			["order"] = "-createdAt" // just an example, don't use this
		};
		ExposedSave save = this.PrepareGetSaveInfoSave(objectId, queries);

		SaveInfoContainer container = await save.GetSaveInfoFromCloudAsync(queries);
		// skip asserting the content of the container, only tests the query construction and request sending
	}

	private ExposedSave PrepareGetRawSave(string url, out byte[] data)
	{
		data = Enumerable.Range(Random.Shared.Next(0, 255), Random.Shared.Next(5, 20)).Select(x => (byte)x).ToArray();
		ExposedSave save = this.PrepareSave(out SaveCredentials? creds);
		save.RequestHandler = this.GetMockHandler(creds, save.CreateDefaultRequest(HttpMethod.Get, url), new() { Content = new ByteArrayContent(data) });
		return save;
	}

	[TestMethod]
	public async Task TestGetRaw()
	{
		string url = "https://phigros.local/test3";
		ExposedSave save = this.PrepareGetRawSave(url, out byte[]? data);
		Assert.AreSequenceEqual(data, await save.GetRawAddressAsync(url));
	}

	[TestMethod]
	public async Task TestGetZipCloudObj()
	{
		string url = "https://phigros.local/test4";
		ExposedSave save = this.PrepareGetRawSave(url, out byte[]? data);
		Assert.AreSequenceEqual(data, await save.GetSaveZipAsync(new PhiCloudObj() { Url = url }));
	}
	[TestMethod]
	public async Task TestGetZipSimplified()
	{
		string url = "https://phigros.local/test5";
		ExposedSave save = this.PrepareGetRawSave(url, out byte[]? data);
		Assert.AreSequenceEqual(data, await save.GetSaveZipAsync(new SimplifiedSaveInfo()
		{
			CreationDate = DateTime.Parse("2025/04/02 16:00"),
			ModificationTime = DateTime.Parse("2025/04/03 12:00"),
			Summary = "this shouldn't care about summary",
			GameSave = new() { Url = url }
		}));
	}

	[TestMethod]
	public async Task TestGetSaveContextOutOfRange()
	{
		string objectId = "testing object id";
		ExposedSave save = this.PrepareGetSaveInfoSave(objectId,
			new() { ["where"] = $"{{\"user\":{{\"__type\":\"Pointer\",\"className\":\"_User\",\"objectId\":\"{objectId}\"}}}}" });

		MaxValueArgumentOutOfRangeException ex = await Assert.ThrowsAsync<MaxValueArgumentOutOfRangeException>(async () =>
		{
			await save.GetSaveContextAsync(20);
		});
		Assert.AreEqual(2, ex.MaxValue);
	}

	// TODO: more tests for get save context
}
