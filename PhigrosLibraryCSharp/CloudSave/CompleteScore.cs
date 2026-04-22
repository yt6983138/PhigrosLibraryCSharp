namespace PhigrosLibraryCSharp.CloudSave;

/// <summary>
/// Key used to look up chart constant, which is used to calculate rks. Consists of song id and difficulty.
/// </summary>
/// <param name="SongId">The id of the song. (<i>Includes</i> <c>.0</c> or other numeric suffix)</param>
/// <param name="Difficulty">The difficulty of the song.</param>
public record struct ChartConstantKey(string SongId, Difficulty Difficulty);
/// <summary>
/// A complete score containing all the information needed to calculate RKS and display the score.
/// This was separated from <see cref="SongScore"/> to avoid having to store redundant information 
/// such as chart constant and song name, also enable dynamic song name/chart constant updates.
/// </summary>
public struct CompleteScore : IComparable<CompleteScore>
{
	private readonly IReadOnlyDictionary<ChartConstantKey, float> _constantMap;
	private readonly IReadOnlyDictionary<string, string> _nameMap;

	/// <summary>
	/// The core score information, which contains song id, difficulty, accuracy, status and other basic information.
	/// </summary>
	public SongScore Score { get; set; }
	/// <summary>
	/// Gets name of the song. If the name map doesn't contain the song id, returns the song id instead.
	/// </summary>
	public readonly string NameOrDefault => this._nameMap.TryGetValue(this.Score.Id, out string? name) ? name : this.Score.Id;
	/// <summary>
	/// Gets name of the song. If the name map doesn't contain the song id, throws an exception.
	/// </summary>
	public readonly string Name => this._nameMap[this.Score.Id];
	/// <summary>
	/// Gets the chart constant of the song. If the constant map doesn't contain the song id and difficulty, throws an exception.
	/// This is also used to calculate the RKS, so it is necessary to not supply a mock or fake constant map.
	/// </summary>
	public readonly float ChartConstant => this._constantMap[new(this.Score.Id, this.Score.Difficulty)];

	/// <summary>
	/// The rks given from the score.
	/// </summary>
	public readonly double Rks =>
		this.Score.Accuracy < 70 ? 0 : Math.Pow((this.Score.Accuracy - 55) / 45, 2) * this.ChartConstant;


	/// <summary>
	/// Initializes a new instance of the <see cref="CompleteScore"/> struct.
	/// </summary>
	/// <param name="score">The core score information.</param>
	/// <param name="constantMap">The dictionary used to look up chart constants for RKS calculation.
	/// Please do not supply a fake or mock map, as <see cref="GameRecord"/> use RKS to sort scores,
	/// otherwise operation will fail.</param>
	/// <param name="nameMap">The dictionary used to look up song names for display.</param>
	public CompleteScore(
		SongScore score,
		IReadOnlyDictionary<ChartConstantKey, float> constantMap,
		IReadOnlyDictionary<string, string> nameMap)
	{
		this.Score = score;
		this._constantMap = constantMap;
		this._nameMap = nameMap;
	}

	/// <inheritdoc/>
	public int CompareTo(CompleteScore other)
	{
		return other.Rks.CompareTo(this.Rks);
	}
}
