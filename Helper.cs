using System.Text;

namespace PhigrosLibraryCSharp;

public static class Helper
{
	public static ScoreStatus ParseStatus(ScoreFormat record)
	{
		if (record.a == 100)
		{
			if (record.s == 1000000) { return ScoreStatus.Phi; }
			return ScoreStatus.Bugged;
		}
		if (record.c == ScoreStatus.Fc) { return ScoreStatus.Fc; }
		if (record.s >= 960000) { return ScoreStatus.Vu; }
		if (record.s >= 920000) { return ScoreStatus.S; }
		if (record.s >= 880000) { return ScoreStatus.A; }
		if (record.s >= 820000) { return ScoreStatus.B; }
		if (record.s >= 700000) { return ScoreStatus.C; }
		if (record.s >= 0) { return ScoreStatus.False; }
		return ScoreStatus.Bugged;
	}
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
