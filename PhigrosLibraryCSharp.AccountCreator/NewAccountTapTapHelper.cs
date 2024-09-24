using Newtonsoft.Json;
using PhigrosLibraryCSharp.Cloud.Login;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace PhigrosLibraryCSharp.AccountCreator;
public class NewAccountTapTapHelper
{
	private static HttpClient _sApiHttpClient = new();
	private static HttpClient _apiHttpClient = new();

	private AndroidProfile? _androidProfile;

	#region Endpoints
	public static string ApiDomain => "https://api.taptapdada.com";
	public static string SApiDomain => "https://sapi.taptap.com";

	public static string OAuthLoginV2 => $"{ApiDomain}/oauth/v1/token";

	public static string AndroidProfile => $"{SApiDomain}/v3/profile/android";
	#endregion

	internal static string PN => "TapTap";
	internal static int VersionCode => 206012000;
	internal static string ClientId => "4cdbf1a615fcd55b75";
	internal static string ClientSecret => "aOqilgPqN4grnMG5VdLlhLptAU3WHorK";
	internal static string FormattedTime => (DateTime.Now.AsMillisecondsSinceUTC() / 1000).ToString("D10");

	public static async Task<object> FetchIdentifier()
	{
		Dictionary<string, string> bodyParams = GetV2GeneralPostBodyParams();

		bodyParams.Add("client_id", ClientId);
		bodyParams.Add("client_secret", ClientSecret);
		bodyParams.Add("grant_type", "client_credentials");
		bodyParams.Add("secret_type", "hmac-sha-1");

		string uuid = Guid.NewGuid().ToString();
		Dictionary<string, string> info = new()
		{
			{ "uuid", uuid },
			{ "brand", "samsung" },
			{ "model", $"PLCS_{TapTapHelper.AssemblyVersion}" },
			{ "screen", "1145x514" },
			{ "cpu", "armeabi-v7a" },
			{ "android_id", Random.Shared.NextInt64().ToHexString() }
		};

		bodyParams.Add("info", JsonConvert.SerializeObject(info));
		SignV2(bodyParams, uuid);

		string url = $"{OAuthLoginV2}?X-UA={WebUtility.UrlEncode(GetXUAQueryValue(CultureInfo.CurrentCulture, info["uuid"]))}";
		string @params = BodyParamsToString(bodyParams);
		HttpRequestMessage message = new()
		{
			Content = new StringContent(@params),
			Method = HttpMethod.Post,
			RequestUri = new(url)
		};

		HttpResponseMessage data = await _apiHttpClient.SendAsync(message);
		string str = await data.Content.ReadAsStringAsync();
		return str;
	}
	internal static void SignV2(Dictionary<string, string> bodyParams, string uuid)
	{
		bodyParams.Add("sign", ToSignedString(bodyParams, uuid));
	}
	internal static string ToSignedString(Dictionary<string, string> bodyParams, string uuid)
	{
		Dictionary<string, string> cloned = new(bodyParams)
		{
			{ "X-UA", GetXUAQueryValue(CultureInfo.CurrentCulture, uuid) }
		};

		return BodyParamsToString(cloned);
	}
	internal static Dictionary<string, string> GetV2GeneralPostBodyParams()
	{
		return new()
		{
			{ "time", FormattedTime },
			{ "nonce", GenerateRandomAsciiString(5) }
		};
	}
	//internal string GetAndroidProfile()
	//{
	//	if (this._androidProfile is not null) return this._androidProfile;

	//	Task<string> result = _sApiHttpClient.GetStringAsync();
	//}
	internal static string BodyParamsToString(Dictionary<string, string> bodyParams)
	{
		StringBuilder sb = new();

		int i = 0;
		foreach (KeyValuePair<string, string> item in bodyParams)
		{
			sb.Append(WebUtility.UrlEncode(item.Key));
			sb.Append('=');
			sb.Append(WebUtility.UrlEncode(item.Value));

			if (i != bodyParams.Count - 1)
				sb.Append('&');
			i++;
		}
		return sb.ToString();
	}
	internal static string GetXUAQueryValue(CultureInfo region, string guidString)
	{
		StringBuilder sb = new();
		sb.Append("V=1&PN=");
		sb.Append(PN);
		sb.Append("&VN_CODE=");
		sb.Append(VersionCode);
		sb.Append("&LOC=");
		sb.Append(region.TwoLetterISOLanguageName.ToUpper());
		sb.Append("&LANG=");
		sb.Append(ConvertLanguageTag(region.IetfLanguageTag));
		sb.Append("&CH=default&UID=");
		sb.Append(guidString);

		return sb.ToString();
	}
	internal static string ConvertLanguageTag(string tag)
	{
		string[] splitted = tag.Split('-');
		StringBuilder sb = new();
		for (int i = 0; i < splitted.Length; i++)
		{
			if (i == 0)
			{
				sb.Append(splitted[i]);
				continue;
			}

			sb.Append('_');
			sb.Append(splitted[i].ToUpper());
		}
		return sb.ToString();
	}
	internal static string GetAuthorization(Uri uri, string mac, string id, string macKey)
	{
		string timeString = FormattedTime;
		string random = GenerateRandomAsciiString(5);
		string mergeSign = MergeToSignCompatibleString(timeString, random, mac, uri, "");
		string signed = Sign(mergeSign, macKey);

		StringBuilder sb = new("MAC ");
		sb.Append(GetAuthorizationParameter("id", id));
		sb.Append(',');
		sb.Append(GetAuthorizationParameter("ts", timeString));
		sb.Append(',');
		sb.Append(GetAuthorizationParameter("nonce", random));
		sb.Append(',');
		sb.Append(GetAuthorizationParameter("mac", mac));

		return sb.ToString();
	}
	internal static string Sign(string compatibleString, string key)
	{
		using HMACSHA1 hmac = new(key.ToUtf8Bytes());
		return Convert.ToBase64String(hmac.ComputeHash(new MemoryStream(compatibleString.ToUtf8Bytes())));
	}
	internal static string MergeToSignCompatibleString(string formattedTime, string randomString, string unknown, Uri uri, string unknownEmpty)
	{
		StringBuilder sb = new();
		sb.AppendLine(formattedTime);
		sb.AppendLine(randomString);
		sb.AppendLine(unknown);
		sb.AppendLine(uri.PathAndQuery);
		sb.AppendLine(uri.Host);
		sb.AppendLine(uri.Scheme.Contains("https", StringComparison.InvariantCultureIgnoreCase) ? "443" : "80");
		sb.AppendLine(unknownEmpty);
		return sb.ToString();
	}
	internal static string GenerateRandomAsciiString(int length, Random? random = null)
	{
		const string AllowedChars = "abcdefghijklmnopqrstuvwxyz0123456789";

		random ??= Random.Shared;

		Span<char> str = stackalloc char[length];
		for (int i = 0; i < length; i++)
		{
			str[i] = AllowedChars[random.Next(AllowedChars.Length)];
		}
		return new string(str);
	}
	internal static string GetAuthorizationParameter(string id, string value)
		=> $"{id}=\"{value}\"";
}
