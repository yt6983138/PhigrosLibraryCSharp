using System.Text.Json.Serialization;

namespace PhigrosLibraryCSharp.CloudSave.HttpModels;

/// <summary>
/// JSON serialization context for cloud save related models. Does not contain TapTap and LC related models.
/// </summary>
[JsonSerializable(typeof(PlayerInfo))]
[JsonSerializable(typeof(SaveInfoContainer))]
[JsonSourceGenerationOptions(AllowTrailingCommas = true, PropertyNameCaseInsensitive = true, IncludeFields = true)]
public partial class CloudSaveSerializationContext : JsonSerializerContext
{
}
