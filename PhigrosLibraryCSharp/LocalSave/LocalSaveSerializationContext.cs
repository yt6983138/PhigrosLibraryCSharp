using System.Text.Json.Serialization;

namespace PhigrosLibraryCSharp.LocalSave;

/// <summary>
/// JSON serialization context for all types that are used in local save. 
/// Currently only <see cref="RawScore"/> is included.
/// </summary>
[JsonSerializable(typeof(RawScore))]
public partial class LocalSaveSerializationContext : JsonSerializerContext
{
}
