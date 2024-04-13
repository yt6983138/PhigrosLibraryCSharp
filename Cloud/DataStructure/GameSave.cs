namespace PhigrosLibraryCSharp.Cloud.DataStructure;

public class GameSave
{
	public required List<InternalScoreFormat> Records { get; set; }
	public required DateTime CreationDate { get; set; }
	public required DateTime ModificationTime { get; set; }
	public string Summary { get; set; } = ""; // unused, unknown	
}