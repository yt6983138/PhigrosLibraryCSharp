using PhigrosLibraryCSharp.GameRecords;

namespace PhigrosLibraryCSharp.Cloud.DataStructure;

/// <summary>
/// A container containing save records.
/// </summary>
public class GameSave
{
	/// <summary>
	/// The player song records.
	/// </summary>
	public required List<CompleteScore> Records { get; set; }
	/// <summary>
	/// The creation date of the save.
	/// </summary>
	public required DateTime CreationDate { get; set; }
	/// <summary>
	/// The creation date of the save.
	/// </summary>
	public required DateTime ModificationTime { get; set; }
	/// <summary>
	/// [Unknown]
	/// </summary>
	public string Summary { get; set; } = ""; // unused, unknown	
}