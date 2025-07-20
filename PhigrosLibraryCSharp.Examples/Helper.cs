using System.Text.Json;

namespace PhigrosLibraryCSharp.Examples;
internal static class Helper
{
	private static readonly JsonSerializerOptions _jsonOption = new()
	{
		WriteIndented = true
	};

	internal static string ToJson<T>(this T obj)
	{
		return JsonSerializer.Serialize(obj, _jsonOption);
	}
	internal static string Join<T>(this IEnumerable<T> values, string separator)
		=> string.Join(separator, values);
}
