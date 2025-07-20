namespace PhigrosLibraryCSharp.Cloud.RawData;

/// <summary>
/// The save parsed from cloud object.
/// </summary>
public class SimplifiedSave
{
	/// <summary>
	/// The URL of the game save.
	/// </summary>
	public required PhiCloudObj GameSave { get; set; }
	/// <summary>
	/// The creation date of the save.
	/// </summary>
	public required DateTime CreationDate { get; set; }
	/// <summary>
	/// The last modified time of the save.
	/// </summary>
	public required DateTime ModificationTime { get; set; }
	/// <summary>
	/// Player's summary in base64 format.
	/// </summary>
	public required string Summary { get; set; }
}
/// <summary>
/// A container containing the URL of object.
/// </summary>
public struct PhiCloudObj
{
	/// <summary>
	/// The URL of object.
	/// </summary>
	public required string Url { get; set; }
}
