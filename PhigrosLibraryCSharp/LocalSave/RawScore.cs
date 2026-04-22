using PhigrosLibraryCSharp.CloudSave;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhigrosLibraryCSharp.LocalSave;

/// <summary>
/// The raw score format converted directly from local save.
/// You should use <see cref="FromJson(string)"/> to construct this struct from json string,
/// rather than using your own json serializer, as we only set the property names using 
/// <see cref="JsonPropertyNameAttribute"/> (aka no Newtonsoft.Json support)
/// </summary>
public struct RawScore
{
	/// <summary>
	/// Score, ex: 996105
	/// </summary>
	[JsonPropertyName("s")]
	public int Score { get; set; }
	/// <summary>
	/// Accuracy, ex: 99.56718444824219
	/// </summary>
	[JsonPropertyName("a")]
	public float Accuracy { get; set; }
	/// <summary>
	/// Score Status, 0: not fc, 1: fc...
	/// </summary>
	[JsonPropertyName("c")]
	public ScoreStatus Status { get; set; }

	public static RawScore FromJson(string json)
	{
		return JsonSerializer.Deserialize<RawScore>(json);
	}
	/// <summary>
	/// Converts to <see cref="SongScore"/>.
	/// </summary>
	/// <param name="chartConstant">The chart constant of the chart. ex. 11.4 </param>
	/// <param name="songId">The id of the song. ex. Stasis.Maozon (no .0)</param>
	/// <param name="difficulty">The difficulty name of the chart. ex. AT</param>
	/// <returns></returns>
	public SongScore ToSongScore(string songId, Difficulty difficulty)
	{
		return new(
			this.Score,
			this.Accuracy,
			songId,
			difficulty,
			ScoreHelper.ParseStatus(this));
	}
}
