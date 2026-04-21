namespace PhigrosLibraryCSharp.CloudSave;

public record struct ChartConstantKey(string SongId, Difficulty Difficulty);
public struct CompleteScore
{
	private readonly IReadOnlyDictionary<ChartConstantKey, float> _constantMap;
	private readonly IReadOnlyDictionary<string, string> _nameMap;

	public SongScore Score { get; set; }
	public readonly string NameOrDefault => this._nameMap.TryGetValue(this.Score.Id, out string? name) ? name : this.Score.Id;
	public readonly string Name => this._nameMap[this.Score.Id];
	public readonly float ChartConstant => this._constantMap[new(this.Score.Id, this.Score.Difficulty)];

	/// <summary>
	/// The rks given from the score.
	/// </summary>
	public readonly double Rks =>
		this.Score.Accuracy < 70 ? 0 : Math.Pow((this.Score.Accuracy - 55) / 45, 2) * this.ChartConstant;

	public CompleteScore(
		SongScore score,
		IReadOnlyDictionary<ChartConstantKey, float> constantMap,
		IReadOnlyDictionary<string, string> nameMap)
	{
		this.Score = score;
		this._constantMap = constantMap;
		this._nameMap = nameMap;
	}
}
