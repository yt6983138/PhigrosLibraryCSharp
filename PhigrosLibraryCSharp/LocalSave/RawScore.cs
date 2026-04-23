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

	/// <summary>
	/// Constructs a <see cref="RawScore"/> from json string. 
	/// The json string should be in the format of local save, ex: <c>{"s":996105,"a":99.56718444824219,"c":1}</c>
	/// </summary>
	/// <param name="json"></param>
	/// <returns>A constructed <see cref="RawScore"/> from json string.</returns>
	public static RawScore FromJson(string json)
	{
		return JsonSerializer.Deserialize<RawScore>(json);
	}
	/// <summary>
	/// Converts to a <see cref="SongScore"/>, for the ease of use.
	/// </summary>
	/// <param name="songId">The id of the song. ex. <c>Stasis.Maozon.0</c></param>
	/// <param name="difficulty">The difficulty of this score. ex. <see cref="Difficulty.AT"/></param>
	/// <returns>A constructed <see cref="SongScore"/> from <see cref="RawScore"/>.</returns>
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
