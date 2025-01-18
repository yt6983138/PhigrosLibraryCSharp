using PhigrosLibraryCSharp.GameRecords.Raw;

namespace PhigrosLibraryCSharp.GameRecords;

/// <summary>
/// The normalized, for use score.
/// </summary>
public class CompleteScore : IComparable<CompleteScore>
{
	/// <summary>
	/// The default empty score.
	/// </summary>
	public static CompleteScore Empty => new(0, 0, 0, "", Difficulty.EZ, ScoreStatus.False);

	internal bool _isFc = false;

	/// <summary>
	/// Scores, 0 ~ 1000000
	/// </summary>
	public int Score { get; set; } = 0;
	/// <summary>
	/// Accuracy, 0 ~ 100
	/// </summary>
	public double Accuracy { get; set; } = 0;
	/// <summary>
	/// Chart constant, ex. 11.4
	/// </summary>
	public float ChartConstant { get; set; } = 0;
	/// <summary>
	/// ex. Stasis.Maozon (no .0)
	/// </summary>
	public string Id { get; set; } = "";
	/// <summary>
	/// ex. AT
	/// </summary>
	public Difficulty Difficulty { get; set; } = default;
	/// <summary>
	/// ex. ScoreStatus.A
	/// </summary>
	public ScoreStatus Status
	{
		get => ScoreHelper.ParseStatus(this.Score, this.Accuracy, this._isFc);
		set
		{
			if (value == ScoreStatus.Fc || (value == ScoreStatus.Phi && this.Score == 1000000 && this.Accuracy == 100d))
			{
				this._isFc = true;
				return;
			}
			this._isFc = false;
		}
	}

	/// <summary>
	/// The rks given from the score.
	/// </summary>
	public double Rks
		=> this.Accuracy < 70
		? 0
		: Math.Pow((this.Accuracy - 55) / 45, 2) * this.ChartConstant;

	/// <summary>
	/// Constructs with raw scores.
	/// </summary>
	/// <param name="score">Score</param>
	/// <param name="acc">Accuracy</param>
	/// <param name="chartConstant">Chart constant</param>
	/// <param name="id">Song id, ex. Stasis.Maozon (no .0)</param>
	/// <param name="difficulty">Difficulty id</param>
	/// <param name="status">Score status</param>
	public CompleteScore(
		int score,
		double acc,
		float chartConstant,
		string id,
		Difficulty difficulty,
		ScoreStatus status)
	{
		this.Score = score;
		this.Accuracy = acc;
		this.ChartConstant = chartConstant;
		this.Id = id;
		this.Status = status;
		this.Difficulty = difficulty;
	}
	internal CompleteScore(
		MoreInfoPartialGameRecord record,
		string id,
		float chartConstant,
		Func<int, Difficulty> difficultyTranslator)
	{
		this._isFc = record.IsFc;
		this.Score = record.Score;
		this.Accuracy = record.Acc;
		this.ChartConstant = chartConstant;
		this.Id = id;
		this.Difficulty = difficultyTranslator.Invoke(record.LevelType);
	}

	/// <summary>
	/// Export the score to more excel friendly format.
	/// </summary>
	/// <param name="name">The display id of the chart. ex. Distorted Fate </param>
	/// <returns>The excel friendly format of the score.</returns>
	public ExportScore Export(string name)
	{
		return new ExportScore()
		{
			ID = this.Id,
			Name = name,
			Difficulty = this.Difficulty.ToString(),
			ChartConstant = this.ChartConstant,
			Score = this.Score,
			Acc = this.Accuracy,
			RksGiven = this.Rks,
			Status = this.Status.ToString()
		};
	}
	/// <inheritdoc/>
	public override string ToString()
	{
		return $"Score: {this.Score}, " +
			$"Acc: {this.Accuracy}, " +
			$"Status: {this.Status}, " +
			$"cc: {this.ChartConstant}, " +
			$"Rks: {this.Rks}";
	}
	/// <inheritdoc/>
	public int CompareTo(CompleteScore? other)
	{
		if (other is null) throw new ArgumentNullException(nameof(other));
		return this.Rks.CompareTo(other.Rks);
	}
}
