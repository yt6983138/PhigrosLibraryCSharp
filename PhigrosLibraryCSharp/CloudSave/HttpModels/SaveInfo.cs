using System.Text.Json.Serialization;

namespace PhigrosLibraryCSharp.CloudSave.HttpModels;

/// <summary>
/// A container containing <see cref="SaveInfo"/>s.
/// </summary>
public struct SaveInfoContainer
{
	/// <summary>
	/// Player's <see cref="SaveInfo"/>s.
	/// </summary>
	[JsonPropertyName("results")]
	public List<SaveInfo> Results { get; set; }

	/// <summary>
	/// Get <see cref="SimplifiedSaveInfo"/>s by parsing them.
	/// </summary>
	/// <returns>A <see cref="List{T}"/> of <see cref="SimplifiedSaveInfo"/>s containing parsed saves.</returns>
	public readonly List<SimplifiedSaveInfo> GetParsedSaves()
	{
		List<SimplifiedSaveInfo> saves = [];
		foreach (SaveInfo item in this.Results) saves.Add(item.Simplify());
		return saves;
	}
}
/// <summary>
/// Save info directly converted from cloud object.
/// </summary>
public class SaveInfo
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	[JsonPropertyName("createdAt")]
	public required DateTime CreatedAt { get; set; }
	[JsonPropertyName("gameFile")]
	public required GameFile GameFile { get; set; }
	[JsonPropertyName("modifiedAt")]
	public required TapTapSaveTime ModifiedAt { get; set; }
	[JsonPropertyName("name")]
	public required string Name { get; set; } = "";
	[JsonPropertyName("objectId")]
	public required string ObjectId { get; set; } = "";
	[JsonPropertyName("summary")]
	public required string Summary { get; set; } = "";
	[JsonPropertyName("updatedAt")]
	public required DateTime UpdatedAt { get; set; }
	[JsonPropertyName("user")]
	public required TapTapUserInfo User { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

	/// <summary>
	/// Convert to <see cref="SimplifiedSaveInfo"/>.
	/// </summary>
	/// <returns>A converted <see cref="SimplifiedSaveInfo"/>.</returns>
	public SimplifiedSaveInfo Simplify()
	{
		return new SimplifiedSaveInfo()
		{
			GameSave = new PhiCloudObj()
			{
				Url = this.GameFile.Url
			},
			CreationDate = this.CreatedAt,
			ModificationTime = this.UpdatedAt,
			Summary = this.Summary
		};
	}
}
// sorry i cant remember what those are anymore
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public class GameFile
{
	[JsonPropertyName("__type")]
	public required string Type { get; set; }
	[JsonPropertyName("bucket")]
	public required string Bucket { get; set; }
	[JsonPropertyName("createdAt")]
	public required DateTime CreatedAt { get; set; }
	[JsonPropertyName("key")]
	public required string Key { get; set; }
	[JsonPropertyName("metaData")]
	public required GameFileMetaData MetaData { get; set; }
	[JsonPropertyName("mime_type")]
	public required string MimeType { get; set; }
	[JsonPropertyName("name")]
	public required string Name { get; set; }
	[JsonPropertyName("objectId")]
	public required string ObjectId { get; set; }
	[JsonPropertyName("provider")]
	public required string Provider { get; set; }
	[JsonPropertyName("updatedAt")]
	public required DateTime UpdatedAt { get; set; }
	[JsonPropertyName("url")]
	public required string Url { get; set; }
}
public class GameFileMetaData
{
	[JsonPropertyName("_checksum")]
	public required string Checksum { get; set; }
	[JsonPropertyName("prefix")]
	public required string Prefix { get; set; }
	[JsonPropertyName("size")]
	public required int Size { get; set; }
}
public class TapTapSaveTime
{
	[JsonPropertyName("__type")]
	public required string Type { get; set; }
	[JsonPropertyName("iso")]
	public required DateTime Time { get; set; }
}
public class TapTapUserInfo
{
	[JsonPropertyName("__type")]
	public required string Type { get; set; }
	[JsonPropertyName("className")]
	public required string ClassName { get; set; }
	[JsonPropertyName("objectId")]
	public required string ObjectId { get; set; }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
