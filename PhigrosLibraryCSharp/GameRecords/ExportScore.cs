namespace PhigrosLibraryCSharp.GameRecords;

/// <summary>
/// A more excel friendly form of the score.
/// </summary>
public struct ExportScore
{
	/// <summary>
	/// The ID of the song. ex. Stasis.Maozon
	/// </summary>
	public string ID;
	/// <summary>
	/// The name of the song. ex. Stasis
	/// </summary>
	public string Name;
	/// <summary>
	/// The difficulty name. ex. AT
	/// </summary>
	public string Difficulty;
	/// <summary>
	/// The chart constant. ex. 11.4
	/// </summary>
	public float ChartConstant;
	/// <summary>
	/// The score.
	/// </summary>
	public int Score;
	/// <summary>
	/// Accuracy.
	/// </summary>
	public double Acc;
	/// <summary>
	/// The rks given.
	/// </summary>
	public double RksGiven;
	/// <summary>
	/// The status. ex. A
	/// </summary>
	public string Stat;
}
