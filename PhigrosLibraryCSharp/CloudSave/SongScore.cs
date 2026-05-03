using System.Numerics;

namespace PhigrosLibraryCSharp.CloudSave;

/// <summary>
/// Score with version-independent properties (no chart constant, name, and RKS)
/// </summary>
public class SongScore : IEquatable<SongScore>, IEqualityOperators<SongScore, SongScore, bool>
{
	/// <summary>
	/// The default empty score. Note: this returns a new value each time, changing the 
	/// properties of this instance does not affect the default instance.
	/// </summary>
	public static SongScore Default => new(0, 0, "", Difficulty.EZ, ScoreStatus.False);

	internal bool _isFc;

	/// <summary>
	/// Scores, 0 ~ 1000000
	/// </summary>
	public int Score { get; set; }
	/// <summary>
	/// Accuracy, 0 ~ 100
	/// </summary>
	public float Accuracy { get; set; }
	/// <summary>
	/// ex. <c>Stasis.Maozon.0</c>
	/// </summary>
	public string Id { get; set; }
	/// <summary>
	/// ex. AT
	/// </summary>
	public Difficulty Difficulty { get; set; }
	/// <summary>
	/// ex. ScoreStatus.A
	/// </summary>
	public ScoreStatus Status
	{
		get => ScoreHelper.ParseStatus(this.Score, this.Accuracy, this._isFc);
		set
		{
			if (value == ScoreStatus.Fc || (value == ScoreStatus.Phi && this.Score == 1000000 && this.Accuracy >= 100f))
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
	/// <param name="id">Song id, ex. <c>Stasis.Maozon.0</c></param>
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
	/// <summary>
	/// Constructs with raw scores.
	/// </summary>
	/// <param name="score">Score</param>
	/// <param name="acc">Accuracy</param>
	/// <param name="id">Song id, ex. <c>Stasis.Maozon.0</c></param>
	/// <param name="isFc">Has player full combo'ed this chart or not.</param>
	/// <param name="difficulty">Difficulty id</param>
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

	/// <inheritdoc/>
	public bool Equals(SongScore? other)
	{
		if (other is null) return false;
		return this.Score == other.Score
			&& this.Accuracy == other.Accuracy
			&& this.Id == other.Id
			&& this.Difficulty == other.Difficulty
			&& this._isFc == other._isFc;
	}
	/// <inheritdoc/>
	public override bool Equals(object? obj)
	{
		return obj is SongScore score && this.Equals(score);
	}
	/// <inheritdoc/>
	public override int GetHashCode()
	{
		return HashCode.Combine(this.Score, this.Accuracy, this.Id, this.Difficulty, this._isFc);
	}

	/// <inheritdoc/>
	public static bool operator ==(SongScore? left, SongScore? right)
	{
		if (left is null) return right is null;
		return left.Equals(right);
	}
	/// <inheritdoc/>
	public static bool operator !=(SongScore? left, SongScore? right)
	{
		return !(left == right);
	}
}
