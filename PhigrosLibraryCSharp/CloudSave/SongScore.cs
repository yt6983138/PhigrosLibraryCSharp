namespace PhigrosLibraryCSharp.CloudSave;

/// <summary>
/// Score with version-independent properties (no chart constant, name, and RKS)
/// </summary>
public class SongScore
{
	/// <summary>
	/// The default empty score.
	/// </summary>
	public static SongScore Default => new(0, 0, "", Difficulty.EZ, ScoreStatus.False);

	internal bool _isFc = false;

	/// <summary>
	/// Scores, 0 ~ 1000000
	/// </summary>
	public int Score { get; set; } = 0;
	/// <summary>
	/// Accuracy, 0 ~ 100
	/// </summary>
	public float Accuracy { get; set; } = 0;
	/// <summary>
	/// ex. Stasis.Maozon.0
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
	/// Constructs with raw scores.
	/// </summary>
	/// <param name="score">Score</param>
	/// <param name="acc">Accuracy</param>
	/// <param name="chartConstant">Chart constant</param>
	/// <param name="id">Song id, ex. Stasis.Maozon (no .0)</param>
	/// <param name="difficulty">Difficulty id</param>
	/// <param name="status">Score status</param>
	public SongScore(
		int score,
		float acc,
		string id,
		Difficulty difficulty,
		ScoreStatus status)
	{
		this.Score = score;
		this.Accuracy = acc;
		this.Id = id;
		this.Status = status;
		this.Difficulty = difficulty;
	}
	public SongScore(int score, float acc, string id, bool isFc, Difficulty difficulty)
	{
		this.Score = score;
		this.Accuracy = acc;
		this.Id = id;
		this._isFc = isFc;
		this.Difficulty = difficulty;
	}

	/// <inheritdoc/>
	public override string ToString()
	{
		return $$"""
			{
				Score: {{this.Score}},
				Accuracy: {{this.Accuracy}},
				Id: {{this.Id}},
				Difficulty: {{this.Difficulty}},
				Status: {{this.Status}}
			}
			""";
	}
}
