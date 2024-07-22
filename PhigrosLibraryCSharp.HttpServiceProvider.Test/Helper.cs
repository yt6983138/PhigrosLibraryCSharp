using Newtonsoft.Json;
using System.Text;

namespace PhigrosLibraryCSharp.HttpServiceProvider.Test;
internal static class Helper
{
	internal static string ToJson<T>(this T obj)
	{
		return JsonConvert.SerializeObject(obj, Formatting.Indented);
	}
	internal static Dictionary<string, object> AsDictionary(this string str)
	{
		return JsonConvert.DeserializeObject<Dictionary<string, object>>(str)!;
	}
	internal static async Task<string> PostWithStringAsContentAsync(this HttpClient client, string endpoint, string data, string mediaType)
	{
		return await (await client.PostAsync(endpoint, new StringContent(data, Encoding.UTF8, mediaType))).Content.ReadAsStringAsync();
	}
}
