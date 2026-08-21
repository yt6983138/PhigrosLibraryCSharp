using PhigrosLibraryCSharp.CloudSave.HttpModels;
using PhigrosLibraryCSharp.CloudSave.Login;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PhigrosLibraryCSharp.CloudSave;

/// <summary>
/// A helper that can be used to query save data or decrypt local save.
/// </summary>
public class Save : IDisposable
{
	#region Constants
	/// <summary>
	/// The address of Phigros' save server.
	/// </summary>
	public const string CloudServerAddress = @"https://rak3ffdi.cloud.tds1.tapapis.cn";
	/// <summary>
	/// The address of Phigros' save server (International)
	/// </summary>
	public const string InternationalCloudServerAddress = @"https://kviehlel.cloud.ap-sg.tapapis.com";
	/// <summary>
	/// Gets the cloud server address based on the endpoint.
	/// </summary>
	/// <param name="useChinaEndpoint">If <see langword="true"/>, returns the China endpoint; otherwise, returns the international endpoint.</param>
	/// <returns>The cloud server address as a string.</returns>
	public static string GetCloudServerAddress(bool useChinaEndpoint)
		=> useChinaEndpoint ? CloudServerAddress : InternationalCloudServerAddress;

	internal const string CloudMeAddress = @"/1.1/users/me";
	internal const string CloudGameSaveAddress = @"/1.1/classes/_GameSave";
	internal const string CloudAESKey = @"6Jaa0qVAJZuXkZCLiOa/Ax5tIZVu+taKUN1V1nqwkks=";
	internal const string CloudAESIV = @"Kk/wisgNYwcAV8WVGMgyUw==";

	internal static readonly byte[] Iv = Convert.FromBase64String(CloudAESIV);
	internal static readonly byte[] Key = Convert.FromBase64String(CloudAESKey);

	internal static string GetAddress(string address, bool useInternational = false)
		=> (useInternational ? InternationalCloudServerAddress : CloudServerAddress) + address;
	internal string GetAddress(string address)
		=> GetAddress(address, this.IsInternational);
	#endregion

	#region Default implementations
	private static Task<HttpResponseMessage> DefaultRequestHandler(Save save, HttpRequestMessage request, CancellationToken ct = default)
	{
		return SharedClient.SendAsync(request, ct);
	}
	private static async Task<byte[]> DecryptDefaultImplementation(byte[] key, byte[] iv, byte[] data, CancellationToken ct = default)
	{
		using Aes aes = Aes.Create();
		aes.Key = key;
		aes.IV = iv;
		aes.Padding = PaddingMode.PKCS7;

		using MemoryStream ms = new();
		using (CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
		{
			await cs.WriteAsync(data, ct);
			cs.FlushFinalBlock();
		}
		return ms.ToArray();
	}
	private static async Task<byte[]> EncryptDefaultImplementation(byte[] key, byte[] iv, byte[] data, CancellationToken ct = default)
	{
		using Aes aes = Aes.Create();
		aes.Key = key;
		aes.IV = iv;
		aes.Padding = PaddingMode.PKCS7;
		using MemoryStream ms = new();
		using (CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
		{
			await cs.WriteAsync(data, ct);
			cs.FlushFinalBlock();
		}
		return ms.ToArray();
	}
	#endregion

	/// <summary>
	/// A delegate that can be used on WASM platform or other platforms where AES is not supported.
	/// </summary>
	/// <param name="key">Decoded <see cref="CloudAESKey"/>.</param>
	/// <param name="iv">Decoded <see cref="CloudAESIV"/>.</param>
	/// <param name="data">Decoded data for decrypting or encrypting.</param>
	/// <param name="ct">The cancellation token to cancel the operation.</param>
	/// <returns>Decrypted or encrypted data.</returns>
	public delegate Task<byte[]> AESCipherFunction(byte[] key, byte[] iv, byte[] data, CancellationToken ct = default);

	/// <summary>
	/// Shared <see cref="HttpClient"/> instance that is used for making requests to the cloud server by all <see cref="Save"/> instances.
	/// Do not dispose or replace this client unless you know what you are doing. If you need to intercept or proxy the requests, 
	/// consider using the <see cref="RequestHandler"/> property instead.
	/// </summary>
	public static HttpClient SharedClient { get; set; } = new(SocketsHttpHandler.CreateFromLifeTime(TimeSpan.FromMinutes(2)));

	/// <summary>
	/// Cached user object ID, to make subsequent calls to <see cref="GetUserObjectId"/> faster.
	/// </summary>
	protected string? _userObjectId = null;

	/// <summary>
	/// The user's session token.
	/// </summary>
	public string SessionToken { get; private init; }
	/// <summary>
	/// Indicates whether the save is for the international server or not.
	/// </summary>
	public bool IsInternational { get; private init; }

	/// <summary>
	/// Custom request handler that can be used to intercept requests to the cloud server. 
	/// This can be useful for platforms where <see cref="HttpClient"/> is not fully supported, such as WASM.
	/// </summary>
	public Func<Save, HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> RequestHandler { get; set; } = DefaultRequestHandler;

	/// <summary>
	/// On WASM or other platform where AES is not supported, you can set this property to other function
	/// that is capable to decrypt data.
	/// </summary>
	public AESCipherFunction Decryptor { get; set; } = DecryptDefaultImplementation;
	/// <summary>
	/// On WASM or other platform where AES is not supported, you can set this property to other function
	/// that is capable to encrypt data.
	/// </summary>
	public AESCipherFunction Encryptor { get; set; } = EncryptDefaultImplementation;

	/// <summary>
	/// Initialize the helper. Warning: Only check token semantically, does NOT do a connect test.
	/// </summary>
	/// <param name="sessionToken">Session token gotten from .userdata or somewhere else like 
	/// <see cref="LCHelper.LoginAndGetToken(LCCombinedAuthData, bool, bool, CancellationToken)"/>.</param>
	/// <param name="isInternational">Use international server or not.</param>
	/// <exception cref="ArgumentException">Thrown if the token format is invalid.</exception>
	public Save(string sessionToken, bool isInternational)
	{
		sessionToken = sessionToken.Trim();
		this.IsInternational = isInternational;
		this.SessionToken = sessionToken;

		if (!IsSemanticallyValidToken(sessionToken))
			throw new ArgumentException("Invalid token.", nameof(sessionToken));
	}

	/// <summary>
	/// Check if the token is semantically correct. Does NOT do a connect test.
	/// </summary>
	/// <param name="sessionToken">The Phigros session token.</param>
	/// <returns><see langword="true"/> if the token is semantically correct, otherwise <see langword="false"/>.</returns>
	public static bool IsSemanticallyValidToken(string sessionToken)
	{
		return sessionToken.Length == 25 &&
			sessionToken.All(char.IsLetterOrDigit);
	}

	private static string BuildQueryString(string url, ICollection<KeyValuePair<string, string>> queries)
	{
		if (queries.Count == 0)
			return url;

		StringBuilder sb = new(url);
		if (url[^1] != '?')
			sb.Append('?');

		foreach (KeyValuePair<string, string> item in queries)
		{
			sb.Append(Uri.EscapeDataString(item.Key));
			sb.Append('=');
			sb.Append(Uri.EscapeDataString(item.Value));
			sb.Append('&');
		}

		if (sb[^1] == '&')
			sb.Length--;

		return sb.ToString();
	}

	/// <inheritdoc/>
	public virtual void Dispose()
	{
		GC.SuppressFinalize(this);
	}

	#region Raw operation
	/// <summary>
	/// Creates a default <see cref="HttpRequestMessage"/> with the necessary headers for Phigros cloud requests.
	/// </summary>
	/// <param name="method">The HTTP method to use for the request.</param>
	/// <param name="address">The URL address for the request.</param>
	/// <returns>A configured <see cref="HttpRequestMessage"/> instance.</returns>
	protected HttpRequestMessage CreateDefaultMessage(HttpMethod method, string address)
	{
		HttpRequestMessage message = new(method, address);
		message.Headers.Add("X-LC-Id", this.IsInternational ? LCHelper.InternationalClientId : LCHelper.ClientId);
		message.Headers.Add("X-LC-Key", this.IsInternational ? LCHelper.InternationalAppKey : LCHelper.AppKey);
		message.Headers.Add("User-Agent", "LeanCloud-CSharp-SDK/1.0.3");
		message.Headers.Add("Accept", "application/json");
		message.Headers.Add("X-LC-Session", this.SessionToken);

		return message;
	}
	private Task<HttpResponseMessage> GetAsync(string address, CancellationToken ct = default)
	{
		return this.RequestHandler.Invoke(this, this.CreateDefaultMessage(HttpMethod.Get, address), ct);
	}

	/// <summary>
	/// Get the raw save from cloud.
	/// </summary>
	/// <param name="queries">The queries to filter saves. If <see langword="null"/>, will get saves of current user. Default query looks like this:
	/// <code>
	/// where={"user":{"__type":"Pointer","className":"_User","objectId":"[User object id]"}}
	/// </code>
	/// The method will handle encoding for you.
	/// 
	/// Other queries can be added as needed, such as <c>skip</c> and <c>limit</c>. 
	/// However, specifying this parameter will not use the default user filter, and you would need to add it yourself.
	/// </param>
	/// <param name="ct">The cancellation token to cancel the operation.</param>
	/// <returns><see cref="SaveInfoContainer"/> containing all raw information.</returns>
	public async Task<SaveInfoContainer> GetSaveInfoFromCloudAsync(ICollection<KeyValuePair<string, string>>? queries = null, CancellationToken ct = default)
	{
		if (queries is null)
		{
			string userId = await this.GetUserObjectId();
			queries = [new("where", $"{{\"user\":{{\"__type\":\"Pointer\",\"className\":\"_User\",\"objectId\":\"{userId}\"}}}}")];
		}

		string address = BuildQueryString(this.GetAddress(CloudGameSaveAddress), queries);

		using HttpResponseMessage response = await this.GetAsync(address, ct);
		string content = await response.Content.ReadAsStringAsync(ct);
		if (!response.IsSuccessStatusCode) throw new HttpRequestException($"Failed to fetch: {content}", null, response.StatusCode);
		SaveInfoContainer container = JsonSerializer.Deserialize(content, CloudSaveSerializationContext.Default.SaveInfoContainer);
		return container;
	}
	/// <summary>
	/// Get the raw content at <paramref name="address"/>.
	/// </summary>
	/// <param name="address">Address to make request to.</param>
	/// <param name="ct">The cancellation token to cancel the operation.</param>
	/// <returns><see cref="byte"/> array of content.</returns>
	public async Task<byte[]> GetRawAddressAsync(string address, CancellationToken ct = default)
	{
		using HttpResponseMessage response = await this.GetAsync(address, ct);
		if (!response.IsSuccessStatusCode) throw new HttpRequestException($"Failed to fetch.", null, response.StatusCode);
		byte[] content = await response.Content.ReadAsByteArrayAsync(ct);

		return content;
	}
	/// <summary>
	/// Get encrypted save zip from cloud.
	/// </summary>
	/// <param name="obj">Target cloud object.</param>
	/// <param name="ct">The cancellation token to cancel the operation.</param>
	/// <returns>An array of <see cref="byte"/> of zip's raw data.</returns>
	public Task<byte[]> GetSaveZipAsync(PhiCloudObj obj, CancellationToken ct = default)
		=> this.GetRawAddressAsync(obj.Url.EnsureNotNull(), ct);
	/// <summary>
	/// Get encrypted save zip from cloud.
	/// </summary>
	/// <param name="obj">Target save.</param>
	/// <param name="ct">The cancellation token to cancel the operation.</param>
	/// <returns>An array of <see cref="byte"/> of zip's raw data.</returns>
	public Task<byte[]> GetSaveZipAsync(SimplifiedSaveInfo obj, CancellationToken ct = default)
		=> this.GetRawAddressAsync(obj.GameSave.Url.EnsureNotNull(obj.ToString()), ct);
	/// <summary>
	/// Decrypt using Phigros' key and iv.
	/// </summary>
	/// <param name="data">The data to decrypt.</param>
	/// <param name="ct">The cancellation token to cancel the operation.</param>
	/// <returns>Decrypted data.</returns>
	public Task<byte[]> Decrypt(byte[] data, CancellationToken ct = default)
		=> this.Decryptor.Invoke(UtilityExtension.QuickCopy(Key), UtilityExtension.QuickCopy(Iv), data, ct);
	/// <summary>
	/// Encrypt using Phigros' key and iv.
	/// </summary>
	/// <param name="data">The data to encrypt.</param>
	/// <param name="ct">The cancellation token to cancel the operation.</param>
	/// <returns>Encrypted data.</returns>
	public Task<byte[]> Encrypt(byte[] data, CancellationToken ct = default)
		=> this.Encryptor.Invoke(UtilityExtension.QuickCopy(Key), UtilityExtension.QuickCopy(Iv), data, ct);
	#endregion

	/// <summary>
	/// This method will get the user's object ID. It will cache the result, so subsequent calls will be faster.
	/// </summary>
	/// <remarks>
	/// <see cref="GetPlayerInfoAsync(CancellationToken)"/> will also set the user object ID cache.
	/// </remarks>
	/// <returns>The user's object ID.</returns>
	public async ValueTask<string> GetUserObjectId()
	{
		if (this._userObjectId is not null)
			return this._userObjectId;

		// the get player info will set the _userObjectId, so we can just call it and return the result
		return (await this.GetPlayerInfoAsync()).ObjectId;
	}
	/// <summary>
	/// Get the <see cref="PlayerInfo"/> of the user.
	/// </summary>
	/// <remarks>
	/// This method will set the user object ID cache, so subsequent calls to <see cref="GetUserObjectId"/> will be faster.
	/// </remarks>
	/// <param name="ct">The cancellation token to cancel the operation.</param>
	/// <returns><see cref="PlayerInfo"/> of the user.</returns>
	public async Task<PlayerInfo> GetPlayerInfoAsync(CancellationToken ct = default)
	{
		using HttpResponseMessage response = await this.GetAsync(this.GetAddress(CloudMeAddress), ct);
		string content = await response.Content.ReadAsStringAsync(ct);
		if (!response.IsSuccessStatusCode) throw new HttpRequestException($"Failed to fetch: {content}", null, response.StatusCode);
		JsonNode node = JsonNode.Parse(content).EnsureNotNull(content);

		string objectId = node["objectId"].EnsureNotNull(content).GetValue<string>();
		this._userObjectId = objectId;

		return new PlayerInfo()
		{
			NickName = node["nickname"]?.GetValue<string>(),
			UserName = node["username"].EnsureNotNull(content).GetValue<string>(),
			CreationTime = node["createdAt"].EnsureNotNull(content).GetValue<DateTime>(),
			ModificationTime = node["updatedAt"].EnsureNotNull(content).GetValue<DateTime>(),
			ObjectId = objectId,
		};
	}
	/// <summary>
	/// Retrieves the save for the specified index.
	/// </summary>
	/// <param name="index">The index of the save to retrieve.</param>
	/// <param name="queries"><inheritdoc cref="GetSaveInfoFromCloudAsync(ICollection{KeyValuePair{string, string}}?, CancellationToken)"/></param>
	/// <param name="ct">The cancellation token to cancel the operation.</param>
	/// <returns>A <see cref="SaveContext"/> object containing the save data.</returns>
	/// <exception cref="MaxValueArgumentOutOfRangeException">
	/// Thrown if the specified index is out of range.
	/// </exception>
	public async Task<SaveContext> GetSaveContextAsync(int index, ICollection<KeyValuePair<string, string>>? queries = null, CancellationToken ct = default)
	{
		List<SaveInfo> rawSaves = (await this.GetSaveInfoFromCloudAsync(queries, ct)).Results;
		if (index < 0 || index >= rawSaves.Count)
			throw new MaxValueArgumentOutOfRangeException(nameof(index), index, rawSaves.Count); // raw count

		SaveInfo rawSave = rawSaves[index];
		return await this.GetSaveContextAsync(rawSave, ct);
	}
	/// <summary>
	/// Retrieves the save context for the specified raw save.
	/// </summary>
	/// <param name="rawSave">The raw save object to retrieve the context for.</param>
	/// <param name="ct">The cancellation token to cancel the operation.</param>
	/// <returns>A <see cref="SaveContext"/> object containing the save data.</returns>
	public async Task<SaveContext> GetSaveContextAsync(SaveInfo rawSave, CancellationToken ct = default)
	{
		SimplifiedSaveInfo simplifiedSave = rawSave.Simplify();
		byte[] rawZip = await this.GetSaveZipAsync(simplifiedSave, ct);

		return await SaveContext.FromZipAsync(rawZip, rawSave, this.Decrypt);
	}
}
