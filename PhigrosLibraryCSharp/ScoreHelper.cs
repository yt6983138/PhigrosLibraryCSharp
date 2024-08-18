using PhigrosLibraryCSharp.GameRecords;
using PhigrosLibraryCSharp.GameRecords.Raw;
using System.Text;

namespace PhigrosLibraryCSharp;

/// <summary>
/// A helper class can be used to assist you.
/// </summary>
public static class ScoreHelper
{
	/// <summary>
	/// Get <see cref="ScoreStatus"/> of a raw record.
	/// </summary>
	/// <param name="record">The game record.</param>
	/// <returns>A <see cref="ScoreStatus"/> of the record.</returns>
	public static ScoreStatus ParseStatus(RawScore record)
		=> ParseStatus(record.s, record.a, record.c == ScoreStatus.Fc);
	/// <summary>
	/// Get <see cref="ScoreStatus"/> of a raw record.
	/// </summary>
	/// <param name="accuracy">The accuracy of the score, ex. 11.45, 99.114514, 100</param>
	/// <param name="isFc">If fc'ed, <see langword="true"/>, otherwise <see langword="false"/>.</param>
	/// <param name="score">The score, ex. 920000, 1000000, 69420, 1145</param>
	/// <returns>A <see cref="ScoreStatus"/> of the record.</returns>
	public static ScoreStatus ParseStatus(int score, double accuracy, bool isFc)
	{
		if (accuracy == 100)
		{
			if (score == 1000000) { return ScoreStatus.Phi; }
			return ScoreStatus.Bugged;
		}
		if (isFc) { return ScoreStatus.Fc; }
		if (score >= 960000) { return ScoreStatus.Vu; }
		if (score >= 920000) { return ScoreStatus.S; }
		if (score >= 880000) { return ScoreStatus.A; }
		if (score >= 820000) { return ScoreStatus.B; }
		if (score >= 700000) { return ScoreStatus.C; }
		if (score >= 0) { return ScoreStatus.False; }
		return ScoreStatus.Bugged;
	}
	/// <summary>
	/// Convert difficulty string to index, ex EZ, HD, IN...
	/// </summary>
	/// <param name="diff">Difficulty string, ex EZ, HD, IN...</param>
	/// <returns>A <see cref="byte"/> presenting the difficulty index.</returns>
	public static byte DifficultStringToIndex(string diff)
	{
		switch (diff.ToUpper())
		{
			case "EZ": return 0;
			case "HD": return 1;
			case "IN": return 2;
			case "AT": return 3;
			default: goto case "EZ";
		}
	}
	internal static string ToHex(this byte[] bytes)
	{
		StringBuilder sb = new();
		for (int i = 0; i < bytes.Length; i++)
		{
			sb.Append(bytes[i].ToString("x2"));
		}
		return sb.ToString();
	}
}
