using Newtonsoft.Json;

namespace PhigrosLibraryCSharp.Tests;
internal static class Helper
{
	internal static string ToJson<T>(this T obj)
	{
		return JsonConvert.SerializeObject(obj, Formatting.Indented);
	}
}
