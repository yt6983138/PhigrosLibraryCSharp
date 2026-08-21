using PhigrosLibraryCSharp.CloudSave;
using PhigrosLibraryCSharp.CloudSave.HttpModels;
using PhigrosLibraryCSharp.CloudSave.Login;
using PhigrosLibraryCSharp.LocalSave;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace PhigrosLibraryCSharp.Tests;
internal static class Helper
{
	// i know it's a bad idea to suppress those warnings, but it doesn't deliver to clients so it's fine i guess
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
	private static readonly JsonSerializerOptions _options = new()
	{
		WriteIndented = true,
		TypeInfoResolver = JsonTypeInfoResolver.Combine(
			CloudSaveSerializationContext.Default,
			LocalSaveSerializationContext.Default,
			LoginSerializationContext.Default,
			new DefaultJsonTypeInfoResolver())
	};

	internal static string ToJson<T>(this T obj)
	{
		return JsonSerializer.Serialize(obj, _options);
	}
	internal static SaveContext.Entry Clone(this SaveContext.Entry entry)
	{
		return new SaveContext.Entry(entry.ObjectVersion, entry.Data.ToArray());
	}
}
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
