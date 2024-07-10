namespace PhigrosLibraryCSharp.Cloud.DataStructure.Raw;

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
	/// [Unknown]
	/// </summary>
	public string Summary { get; set; } = ""; // unused, unknown	
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
