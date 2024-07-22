namespace PhigrosLibraryCSharp.GameRecords;

/// <summary>
/// A more excel friendly form of the score.
/// </summary>
public struct ExportScore
{
	/// <summary>
	/// The ID of the song. ex. Stasis.Maozon
	/// </summary>
	public string ID { get; set; }
	/// <summary>
	/// The name of the song. ex. Stasis
	/// </summary>
	public string Name { get; set; }
	/// <summary>
	/// The difficulty name. ex. AT
	/// </summary>
	public string Difficulty { get; set; }
	/// <summary>
	/// The chart constant. ex. 11.4
	/// </summary>
	public float ChartConstant { get; set; }
	/// <summary>
	/// The score.
	/// </summary>
	public int Score { get; set; }
	/// <summary>
	/// Accuracy.
	/// </summary>
	public double Acc { get; set; }
	/// <summary>
	/// The rks given.
	/// </summary>
	public double RksGiven { get; set; }
	/// <summary>
	/// The status. ex. A
	/// </summary>
	public string Stat { get; set; }
}
