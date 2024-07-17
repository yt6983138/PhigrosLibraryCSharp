using Newtonsoft.Json;

namespace PhigrosLibraryCSharp.Examples;
internal static class Helper
{
	internal static string ToJson<T>(this T obj)
	{
		return JsonConvert.SerializeObject(obj, Formatting.Indented);
	}
	internal static string Join<T>(this IEnumerable<T> values, string separator)
		=> string.Join(separator, values);
}
