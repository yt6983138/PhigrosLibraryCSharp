namespace PhigrosLibraryCSharp.GameRecords;

/// <summary>
/// A more excel friendly form of the score.
/// </summary>
public class ExportScore
{
	/// <summary>
	/// The ID of the song. ex. Stasis.Maozon
	/// </summary>
	public required string ID { get; set; }
	/// <summary>
	/// The name of the song. ex. Stasis
	/// </summary>
	public required string Name { get; set; }
	/// <summary>
	/// The difficulty name. ex. AT
	/// </summary>
	public required string Difficulty { get; set; }
	/// <summary>
	/// The chart constant. ex. 11.4
	/// </summary>
	public required float ChartConstant { get; set; }
	/// <summary>
	/// The score.
	/// </summary>
	public required int Score { get; set; }
	/// <summary>
	/// Accuracy.
	/// </summary>
	public required double Acc { get; set; }
	/// <summary>
	/// The rks given.
	/// </summary>
	public required double RksGiven { get; set; }
	/// <summary>
	/// The status. ex. A
	/// </summary>
	public required string Status { get; set; }
}
