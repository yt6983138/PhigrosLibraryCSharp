namespace PhigrosLibraryCSharp;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
// #pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
public enum ScoreStatus
{
	Bugged = -1,
	NotFc = 0,
	Fc = 1,
	Phi = 2,
	Vu = 3,
	S = 4,
	A = 5,
	B = 6,
	C = 7,
	False = 8
}
public struct RawScoreFormat
{
	/// <summary>
	/// CompleteScore, ex: 996105
	/// </summary>
	public int s;
	/// <summary>
	/// Acc, ex: 99.56718444824219
	/// </summary>
	public double a;
	/// <summary>
	/// Score Status, 0: not fc, 1: fc...
	/// </summary>
	public ScoreStatus c;
	public CompleteScore ToCompleteScore(float chartConstant, string songName, string diffcultyName)
	{
		return new CompleteScore { Score = this.s, Acc = this.a, Status = ScoreHelper.ParseStatus(this), ChartConstant = chartConstant, Name = songName, DifficultyName = diffcultyName };
	}
}
public struct CompleteScore
{
	/// <summary>
	/// 0 ~ 1000000
	/// </summary>
	public int Score = 0;
	/// <summary>
	/// note: format: 99.x% = 99.xxxx...
	/// </summary>
	public double Acc = 0;
	/// <summary>
	/// ex. 11.4
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
	public CompleteScore(int score, double acc, float chartConstant, string name, string diffcultyName, ScoreStatus status)
	{
		this.Score = score;
		this.Acc = acc;
		this.ChartConstant = chartConstant;
		this.Name = name;
		this.Status = status;
		this.DifficultyName = diffcultyName;
	}
	internal CompleteScore(MoreInfoPartialGameRecord record, string name, float chartConstant, in IReadOnlyDictionary<int, string> levelTranslateTable)
	{
		this.Score = record.Score;
		this.Acc = record.Acc;
		this.ChartConstant = chartConstant;
		this.Name = name;
		this.Status = ScoreHelper.ParseStatus(new RawScoreFormat() { a = this.Acc, s = this.Score, c = record.IsFc ? ScoreStatus.Fc : ScoreStatus.NotFc });
		this.DifficultyName = levelTranslateTable[record.LevelType];
	}
	public double GetRksCalculated()
	{
		if (this.Acc < 70)
		{
			return 0;
		}
		return Math.Pow((this.Acc - 55) / 45, 2) * this.ChartConstant;
	}
	public ExportScore Export(string name)
	{
		return new ExportScore()
		{
			ID = this.Name,
			Name = name,
			Difficulty = this.DifficultyName,
			ChartConstant = this.ChartConstant,
			Score = this.Score,
			Acc = this.Acc,
			RksGiven = this.GetRksCalculated(),
			Stat = this.Status.ToString()
		};
	}
	public override string ToString()
	{
		return $"Score: {this.Score}, Acc: {this.Acc}, Status: {nameof(this.Status)}, cc: {this.ChartConstant}, calcedRks: {this.GetRksCalculated()}";
	}
}
public struct ExportScore
{
	// ID, Name, Difficulty, Chart Constant, CompleteScore, Acc, Rks Given, Stat
	public string ID;
	public string Name;
	public string Difficulty;
	public float ChartConstant;
	public int Score;
	public double Acc;
	public double RksGiven;
	public string Stat;
}