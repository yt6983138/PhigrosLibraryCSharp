using System.Runtime.InteropServices;

namespace PhigrosLibraryCSharp.GameRecords.Raw;

[StructLayout(LayoutKind.Sequential)]
internal struct PartialGameRecord
{
	//[FieldOffset(0)]
	internal int Score;
	//[FieldOffset(4)]
	internal float Acc;
}
internal struct MoreInfoPartialGameRecord
{
	internal int Score;
	internal float Acc;
	internal bool IsFc;
	internal int LevelType;

	internal MoreInfoPartialGameRecord(PartialGameRecord data, bool isfc, int levelType)
	{
		this.Score = data.Score;
		this.Acc = data.Acc;
		this.IsFc = isfc;
		this.LevelType = levelType;
	}
}