using PhigrosLibraryCSharp.GameRecords.Raw;

namespace PhigrosLibraryCSharp.GameRecords;

/// <summary>
/// 
/// </summary>
public struct CompleteScore
{
	/// <summary>
	/// Scores, 0 ~ 1000000
	/// </summary>
	public int Score = 0;
	/// <summary>
	/// Accuracy, 0 ~ 100
	/// </summary>
	public double Accuracy = 0;
	/// <summary>
	/// Chart constant, ex. 11.4
	/// </summary>
	public float ChartConstant = 0;
	/// <summary>
	/// ex. Stasis.Maozon (no .0)
	/// </summary>
	public string Name = "Unset";
	/// <summary>
	/// ex. AT
	/// </summary>
	public string DifficultyName = "Unset";
	/// <summary>
	/// ex. ScoreStatus.A
	/// </summary>
	public ScoreStatus Status = ScoreStatus.False;

	/// <summary>
	/// The rks given from the score.
	/// </summary>
	public readonly double Rks
		=> this.Accuracy < 70
		? 0
		: Math.Pow((this.Accuracy - 55) / 45, 2) * this.ChartConstant;

	/// <summary>
	/// Constructs with raw scores.
	/// </summary>
	/// <param name="score">Score</param>
	/// <param name="acc">Accuracy</param>
	/// <param name="chartConstant">Chart constant</param>
	/// <param name="name">Name (id)</param>
	/// <param name="difficultyName">Difficulty name</param>
	/// <param name="status">Score status</param>
	public CompleteScore(
		int score,
		double acc,
		float chartConstant,
		string name,
		string difficultyName,
		ScoreStatus status)
	{
		this.Score = score;
		this.Accuracy = acc;
		this.ChartConstant = chartConstant;
		this.Name = name;
		this.Status = status;
		this.DifficultyName = difficultyName;
	}
	internal CompleteScore(
		MoreInfoPartialGameRecord record,
		string name,
		float chartConstant,
		in IReadOnlyDictionary<int, string> levelTranslateTable)
	{
		this.Score = record.Score;
		this.Accuracy = record.Acc;
		this.ChartConstant = chartConstant;
		this.Name = name;
		this.Status = ScoreHelper.ParseStatus(new RawScore()
		{
			a = this.Accuracy,
			s = this.Score,
			c = record.IsFc ? ScoreStatus.Fc : ScoreStatus.NotFc
		});
		this.DifficultyName = levelTranslateTable[record.LevelType];
	}

	/// <summary>
	/// Export the score to more excel friendly format.
	/// </summary>
	/// <param name="name">The display name of the chart. ex. Distorted Fate </param>
	/// <returns>The excel friendly format of the score.</returns>
	public ExportScore Export(string name)
	{
		return new ExportScore()
		{
			ID = this.Name,
			Name = name,
			Difficulty = this.DifficultyName,
			ChartConstant = this.ChartConstant,
			Score = this.Score,
			Acc = this.Accuracy,
			RksGiven = this.Rks,
			Stat = this.Status.ToString()
		};
	}
	/// <inheritdoc/>
	public override string ToString()
	{
		return $"Score: {this.Score}, " +
			$"Acc: {this.Accuracy}, " +
			$"Status: {nameof(this.Status)}, " +
			$"cc: {this.ChartConstant}, " +
			$"Rks: {this.Rks}";
	}
}
