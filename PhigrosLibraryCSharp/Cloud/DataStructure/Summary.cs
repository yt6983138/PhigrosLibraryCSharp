using PhigrosLibraryCSharp.GameRecords;
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
public class Summary
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
	public Challenge Challenge { get; set; }
	/// <summary>
	/// Avatar id.
	/// </summary>
	public string Avatar { get; set; }
	/// <summary>
	/// The cleared counts of songs.
	/// </summary>
	public List<ushort> Clears { get; set; }

	/// <summary>
	/// Construct a new instance of <see cref="Summary"/>.
	/// </summary>
	/// <param name="saveVersion"></param>
	/// <param name="gameVersion"></param>
	/// <param name="challenge"></param>
	/// <param name="avatar"></param>
	/// <param name="clears"></param>
	public Summary(short saveVersion, short gameVersion, Challenge challenge, string avatar, List<ushort> clears)
	{
		this.SaveVersion = saveVersion;
		this.GameVersion = gameVersion;
		this.Challenge = challenge;
		this.Avatar = avatar;
		this.Clears = clears;
	}

	/// <summary>
	/// The default player <see cref="Summary"/>.
	/// </summary>
	public static Summary Default
	{
		get
		{
			return new Summary(0, 0, default, "", new());
		}
	}
}
