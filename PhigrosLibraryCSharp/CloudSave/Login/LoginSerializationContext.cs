using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace PhigrosLibraryCSharp.CloudSave.Login;

/// <summary>
/// JSON serialization context for login related models. Contains TapTap and LC related models.
/// </summary>
[JsonSerializable(typeof(LCCombinedAuthData))]
[JsonSerializable(typeof(CallbackLoginData))]
[JsonSerializable(typeof(CompleteQRCodeData))]
[JsonSerializable(typeof(PartialTapTapQRCodeData))]
[JsonSerializable(typeof(TapTapProfileData))]
[JsonSerializable(typeof(TapTapTokenData))]
[JsonSerializable(typeof(JsonObject))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(JsonNode))]
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true, AllowTrailingCommas = true, IncludeFields = true)]
public partial class LoginSerializationContext : JsonSerializerContext
{
}
