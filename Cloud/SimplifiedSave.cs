namespace PhigrosLibraryCSharp;

public class SimplifiedSave
{
	public required PhiCloudObj GameSave { get; set; }
	public required DateTime CreationDate { get; set; }
	public required DateTime ModificationTime { get; set; }
	public string Summary { get; set; } = ""; // unused, unknown	
}
public struct PhiCloudObj
{
	public required string Url { get; set; }
}
