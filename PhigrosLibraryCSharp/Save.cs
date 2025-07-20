using PhigrosLibraryCSharp.Cloud.Login;
using PhigrosLibraryCSharp.Cloud.RawData;
using PhigrosLibraryCSharp.Extensions;
using PhigrosLibraryCSharp.GameRecords;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PhigrosLibraryCSharp;

/// <summary>
/// A helper that can be used to query save data or decrypt local save.
/// </summary>
public class Save
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

	internal static readonly JsonSerializerOptions SerializerSettings = new()
	{
		AllowTrailingCommas = true,
		PropertyNameCaseInsensitive = true,
		IncludeFields = true,
	};
	#endregion

	/// <summary>
	/// The user's session token.
	/// </summary>
	public string? SessionToken { get; private set; } = null;
	/// <summary>
	/// Indicates whether the save is for the international server or not.
	/// </summary>
	public bool IsInternational { get; private set; }

	private HttpClient Client { get; set; }
	/// <summary>
	/// A delegate that can be used on WASM platform or other platforms where AES is not supported.
	/// </summary>
	/// <param name="key">Decoded <see cref="CloudAESKey"/>.</param>
	/// <param name="iv">Decoded <see cref="CloudAESIV"/>.</param>
	/// <param name="data">Decoded data for decrypting.</param>
	/// <returns>Decrypted data.</returns>
	public delegate Task<byte[]> CustomDecrypter(byte[] key, byte[] iv, byte[] data);
	/// <summary>
	/// On WASM or other platform where AES is not supported, you can set this property to other function
	/// that is capable to decrypt data.
	/// </summary>
	public CustomDecrypter Decrypter { get; init; } = DecryptDefaultImplementation;

	private static Task<byte[]> DecryptDefaultImplementation(byte[] key, byte[] iv, byte[] data)
	{
		byte[] decrypted;
		using (Aes aes = Aes.Create())
		{
			aes.Key = key;
			aes.IV = iv;
			aes.Padding = PaddingMode.PKCS7;

			using MemoryStream ms = new();
			using (CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
			{
				cs.Write(data, 0, data.Length);
				cs.FlushFinalBlock();
			}
			decrypted = ms.ToArray();
		}
		return Task.FromResult(decrypted);
	}

	/// <summary>
	/// Initialize the helper. Warning: Only check token semantically, does NOT do a connect test.
	/// </summary>
	/// <param name="sessionToken">Session token gotten from .userdata or somewhere else like 
	/// <see cref="LCHelper.LoginAndGetToken(LCCombinedAuthData, bool, bool)"/>.</param>
	/// <param name="isInternational">Use international server or not.</param>
	/// <exception cref="ArgumentException">Thrown if the token format is invalid.</exception>
	public Save(string sessionToken, bool isInternational)
	{
		sessionToken = sessionToken.Trim();
		this.IsInternational = isInternational;
		this.SessionToken = sessionToken;
		this.Client = new();
		this.Client.DefaultRequestHeaders.Add("X-LC-Id", isInternational ? LCHelper.InternationalClientId : LCHelper.ClientId);
		this.Client.DefaultRequestHeaders.Add("X-LC-Key", isInternational ? LCHelper.InternationalAppKey : LCHelper.AppKey);
		this.Client.DefaultRequestHeaders.Add("User-Agent", "LeanCloud-CSharp-SDK/1.0.3");
		this.Client.DefaultRequestHeaders.Add("Accept", "application/json");
		this.Client.DefaultRequestHeaders.Add("X-LC-Session", sessionToken);

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

	#region Raw operation
	/// <summary>
	/// Get the raw save from cloud.
	/// </summary>
	/// <returns><see cref="RawSaveContainer"/> containing all raw information.</returns>
	/// <exception cref="ArgumentNullException">Thrown if the helper is not initialized.</exception>
	public async Task<RawSaveContainer> GetRawSaveFromCloudAsync()
	{
		if (this.SessionToken == null) throw new ArgumentNullException("Session token cannot be null.");
		HttpResponseMessage response = await this.Client.GetAsync(this.GetAddress(CloudGameSaveAddress));
		string content = await response.Content.ReadAsStringAsync();
		if (!response.IsSuccessStatusCode) throw new HttpRequestException($"Failed to fetch: {content}", null, response.StatusCode);
		RawSaveContainer container = JsonSerializer.Deserialize<RawSaveContainer>(content, SerializerSettings);
		return container;
	}
	/// <summary>
	/// Get the raw item at <paramref name="address"/>.
	/// </summary>
	/// <param name="address">Address of item.</param>
	/// <returns><see cref="byte"/> array of item.</returns>
	/// <exception cref="ArgumentNullException">Thrown if the helper is not initialized.</exception>
	public async Task<byte[]> GetRawAddressAsync(string address)
	{
		if (this.SessionToken == null) throw new ArgumentNullException(nameof(this.SessionToken), "Session token cannot be null.");
		HttpResponseMessage response = await this.Client.GetAsync(address);
		if (!response.IsSuccessStatusCode) throw new HttpRequestException($"Failed to fetch.", null, response.StatusCode);
		byte[] content = await response.Content.ReadAsByteArrayAsync();

		return content;
	}
	/// <summary>
	/// Get raw zip from cloud.
	/// </summary>
	/// <param name="obj">Target cloud object.</param>
	/// <returns>An array of <see cref="byte"/> of zip's raw data.</returns>
	public Task<byte[]> GetSaveRawZipAsync(PhiCloudObj obj)
		=> this.GetRawAddressAsync(obj.Url);
	/// <summary>
	/// Get raw zip from cloud.
	/// </summary>
	/// <param name="obj">Target save.</param>
	/// <returns>An array of <see cref="byte"/> of zip's raw data.</returns>
	public Task<byte[]> GetSaveRawZipAsync(SimplifiedSave obj)
		=> this.GetRawAddressAsync(obj.GameSave.Url);
	/// <summary>
	/// Decrypt using Phigros' key and iv.
	/// </summary>
	/// <param name="data">The data to decrypt.</param>
	/// <returns>Decrypted data.</returns>
	public Task<byte[]> Decrypt(byte[] data)
		=> this.Decrypter.Invoke(UtilityExtension.QuickCopy(Key), UtilityExtension.QuickCopy(Iv), data);
	#endregion

	/// <summary>
	/// Get the <see cref="UserInfo"/> of the user.
	/// </summary>
	/// <returns><see cref="UserInfo"/> of the user.</returns>
	/// <exception cref="ArgumentNullException">Thrown if the helper is not initalized.</exception>
	public async Task<UserInfo> GetUserInfoAsync()
	{
		if (this.SessionToken == null) throw new ArgumentNullException("Session token cannot be null.");
		HttpResponseMessage response = await this.Client.GetAsync(this.GetAddress(CloudMeAddress));
		string content = await response.Content.ReadAsStringAsync();
		if (!response.IsSuccessStatusCode) throw new HttpRequestException($"Failed to fetch: {content}", null, response.StatusCode);
		JsonNode node = JsonNode.Parse(content).EnsureNotNull();

		return new UserInfo()
		{
			NickName = node["nickname"].EnsureNotNull().GetValue<string>(),
			UserName = node["username"].EnsureNotNull().GetValue<string>(),
			CreationTime = node["createdAt"].EnsureNotNull().GetValue<DateTime>(),
			ModificationTime = node["updatedAt"].EnsureNotNull().GetValue<DateTime>()
		};
	}
	/// <summary>
	/// Retrieves the save for the specified index.
	/// </summary>
	/// <param name="index">The index of the save to retrieve.</param>
	/// <returns>A <see cref="SaveContext"/> object containing the save data.</returns>
	/// <exception cref="MaxValueArgumentOutOfRangeException">
	/// Thrown if the specified index is out of range.
	/// </exception>
	/// <exception cref="ArgumentNullException">
	/// Thrown if the session token is null.
	/// </exception>
	public async Task<SaveContext> GetSaveContextAsync(int index)
	{
		List<RawSave> rawSaves = (await this.GetRawSaveFromCloudAsync()).results;
		if (index < 0 || index >= rawSaves.Count)
			throw new MaxValueArgumentOutOfRangeException(nameof(index), index, rawSaves.Count); // raw count

		RawSave rawSave = rawSaves[index];
		return await this.GetSaveContextAsync(rawSave);
	}
	/// <summary>
	/// Retrieves the save context for the specified raw save.
	/// </summary>
	/// <param name="rawSave">The raw save object to retrieve the context for.</param>
	/// <returns>A <see cref="SaveContext"/> object containing the save data.</returns>
	public async Task<SaveContext> GetSaveContextAsync(RawSave rawSave)
	{
		SimplifiedSave simplifiedSave = rawSave.ToParsed();
		byte[] rawZip = await this.GetSaveRawZipAsync(simplifiedSave);

		return new(rawZip, rawSave, this.Decrypt);
	}
}
