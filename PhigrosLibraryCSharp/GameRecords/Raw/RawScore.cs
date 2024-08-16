namespace PhigrosLibraryCSharp.GameRecords.Raw;

/// <summary>
/// The raw score format converted directly from local save.
/// Don't blame me for the field names please.
/// </summary>
public struct RawScore
{
	/// <summary>
	/// CompleteScore, ex: 996105
	/// </summary>
	public int s;
	/// <summary>
	/// Accuracy, ex: 99.56718444824219
	/// </summary>
	public double a;
	/// <summary>
	/// Score Status, 0: not fc, 1: fc...
	/// </summary>
	public ScoreStatus c;

	/// <summary>
	/// Converts to <see cref="CompleteScore"/>.
	/// </summary>
	/// <param name="chartConstant">The chart constant of the chart. ex. 11.4 </param>
	/// <param name="songId">The id of the song. ex. Stasis.Maozon (no .0)</param>
	/// <param name="difficulty">The difficulty name of the chart. ex. AT</param>
	/// <returns></returns>
	public CompleteScore ToCompleteScore(float chartConstant, string songId, Difficulty difficulty)
	{
		return new CompleteScore(
			this.s,
			this.a,
			chartConstant,
			songId,
			difficulty,
			ScoreHelper.ParseStatus(this));
	}
}
