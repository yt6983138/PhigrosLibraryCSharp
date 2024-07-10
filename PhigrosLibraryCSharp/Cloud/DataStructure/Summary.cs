using System.Runtime.InteropServices;

namespace PhigrosLibraryCSharp.Cloud.DataStructure;


[StructLayout(LayoutKind.Explicit)]
internal struct RawSummaryFirst
{
	[FieldOffset(0)]
	internal byte SaveVersion;
	[FieldOffset(1)]
	internal ushort ChallengeCode;
	[FieldOffset(3)]
	internal float Rks;
	[FieldOffset(7)]
	internal byte GameVersion;
	[FieldOffset(8)]
	internal byte AvatarStringSize;
}
[StructLayout(LayoutKind.Sequential)]
internal struct RawSummaryLast
{
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
	internal ushort[] Scores;
}
/// <summary>
/// The player's summary.
/// </summary>
public struct Summary
{
	/// <summary>
	/// The version of save.
	/// </summary>
	public short SaveVersion { get; set; }
	/// <summary>
	/// The version of game.
	/// </summary>
	public short GameVersion { get; set; }
	/// <summary>
	/// The player challenge code, example: 123 <br/>
	/// 1 is the type of challenge, 0 = none, 1 = green... etc. <br/>
	/// And the 23 part is level.
	/// </summary>
	public ushort ChallengeCode { get; set; }
	/// <summary>
	/// Avatar id.
	/// </summary>
	public string Avatar { get; set; }
	/// <summary>
	/// The cleared counts of songs.
	/// </summary>
	public List<ushort> Clears { get; set; }

	/// <summary>
	/// The default player <see cref="Summary"/>.
	/// </summary>
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
