using System.Runtime.InteropServices;

namespace PhigrosLibraryCSharp.Cloud.DataStructure;


[StructLayout(LayoutKind.Explicit)]
public struct RawSummaryFirst
{
	[FieldOffset(0)]
	public byte SaveVersion;
	[FieldOffset(1)]
	public ushort ChallengeCode;
	[FieldOffset(3)]
	public float Rks;
	[FieldOffset(7)]
	public byte GameVersion;
	[FieldOffset(8)]
	public byte AvatarStringSize;
}
[StructLayout(LayoutKind.Sequential)]
public struct RawSummaryLast
{
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
	public ushort[] Scores;
}
public struct Summary
{
	public short SaveVersion { get; set; }
	public short GameVersion { get; set; }
	public ushort ChallengeCode { get; set; }
	public string Avatar { get; set; }
	public List<ushort> Clears { get; set; }

	public static Summary Default
	{
		get
		{
			return new Summary()
			{
				SaveVersion = 0,
				GameVersion = 0,
				ChallengeCode = 000,
				Avatar = string.Empty,
				Clears = new()
			};
		}
	}
}
