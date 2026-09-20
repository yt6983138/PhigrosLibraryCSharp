using PhigrosLibraryCSharp.CloudSave;
using PhigrosLibraryCSharp.CloudSave.HttpModels;
using PhigrosLibraryCSharp.CloudSave.Login;
using PhigrosLibraryCSharp.LocalSave;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
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
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code

	internal static SaveContext.Entry Clone(this SaveContext.Entry entry)
	{
		return new SaveContext.Entry(entry.ObjectVersion, entry.Data.ToArray());
	}

	extension(HttpResponseMessage)
	{
		public static HttpResponseMessage CreateJson([StringSyntax(StringSyntaxAttribute.Json)] string json, HttpStatusCode statusCode = HttpStatusCode.OK)
		{
			HttpResponseMessage response = new(statusCode)
			{
				Content = new StringContent(json)
			};
			response.Content.Headers.ContentType = new("application/json");
			return response;
		}
	}
	extension(HttpRequestHeaders self)
	{
		public bool UnorderedSequenceEqual(HttpRequestHeaders second)
		{
			List<KeyValuePair<string, IEnumerable<string>>> orderedFirst = self.OrderBy(h => h.Key).ToList();
			List<KeyValuePair<string, IEnumerable<string>>> orderedSecond = second.OrderBy(h => h.Key).ToList();
			if (orderedFirst.Count != orderedSecond.Count)
				return false;

			using List<KeyValuePair<string, IEnumerable<string>>>.Enumerator firstEnumerator = orderedFirst.GetEnumerator();
			using List<KeyValuePair<string, IEnumerable<string>>>.Enumerator secondEnumerator = orderedSecond.GetEnumerator();

			while (firstEnumerator.MoveNext())
			{
				secondEnumerator.MoveNext();
				if (firstEnumerator.Current.Key != secondEnumerator.Current.Key)
					return false;
				if (!firstEnumerator.Current.Value.Order().SequenceEqual(secondEnumerator.Current.Value.Order()))
					return false;
			}

			return true;
		}
	}
}

[TestClass]
public class Initializer
{
	[AssemblyInitialize]
	public static void AssemblyInit(TestContext testContext)
	{
		Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, "..", "..", "..");
	}
}
