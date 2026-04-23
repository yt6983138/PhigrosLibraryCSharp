using Newtonsoft.Json;
using PhigrosLibraryCSharp.CloudSave;

namespace PhigrosLibraryCSharp.Tests;
internal static class Helper
{
	internal static string ToJson<T>(this T obj)
	{
		return JsonConvert.SerializeObject(obj, Formatting.Indented);
	}
	internal static SaveContext.Entry Clone(this SaveContext.Entry entry)
	{
		return new SaveContext.Entry(entry.ObjectVersion, entry.Data.ToArray());
	}
}
