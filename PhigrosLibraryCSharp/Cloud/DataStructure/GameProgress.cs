using PhigrosLibraryCSharp.GameRecords;

namespace PhigrosLibraryCSharp.Cloud.DataStructure;

/// <summary>
/// The Phigros currency.
/// </summary>
/// <param name="KiB">KiB count.</param>
/// <param name="MiB">MiB count.</param>
/// <param name="GiB">GiB count.</param>
/// <param name="TiB">TiB count.</param>
/// <param name="PiB">PiB count.</param>
public record struct Money(short KiB, short MiB, short GiB, short TiB, short PiB)
{
	/// <inheritdoc/>
	public override readonly string ToString()
	{
		return (this.KiB, this.MiB, this.GiB, this.TiB, this.PiB) switch
		{
			(_, _, _, _, > 0) => $"{this.PiB} PiB, {this.TiB} TiB, {this.GiB} GiB, {this.MiB} MiB, {this.KiB} KiB",
			(_, _, _, > 0, _) => $"{this.TiB} TiB, {this.GiB} GiB, {this.MiB} MiB, {this.KiB} KiB",
			(_, _, > 0, _, _) => $"{this.GiB} GiB, {this.MiB} MiB, {this.KiB} KiB",
			(_, > 0, _, _, _) => $"{this.MiB} MiB, {this.KiB} KiB",
			(_, _, _, _, _) => $"{this.KiB} KiB"
		};
	}
}

/// <summary>
/// The user's game progress.
/// </summary>
/// <param name="IsFirstRun"></param>
/// <param name="LegacyChapterFinished"></param>
/// <param name="AlreadyShowCollectionTip"></param>
/// <param name="AlreadyShowAutoUnlockINTip"></param>
/// <param name="Completed"></param>
/// <param name="SongUpdateInfo"></param>
/// <param name="ChallengeModeRank"></param>
/// <param name="Money"></param>
/// <param name="UnlockFlagOfSpasmodic"></param>
/// <param name="UnlockFlagOfIgallta"></param>
/// <param name="UnlockFlagOfRrharil"></param>
/// <param name="FlagOfSongRecordKey"></param>
/// <param name="RandomVersionUnlocked"></param>
/// <param name="Chapter8UnlockBegin"></param>
/// <param name="Chapter8UnlockSecondPhase"></param>
/// <param name="Chapter8Passed"></param>
/// <param name="Chapter8SongUnlockFlag"></param>
public record class GameProgress(
	bool IsFirstRun,
	bool LegacyChapterFinished,
	bool AlreadyShowCollectionTip,
	bool AlreadyShowAutoUnlockINTip,
	string Completed,
	short SongUpdateInfo,
	Challenge ChallengeModeRank,
	Money Money,
	byte UnlockFlagOfSpasmodic,
	byte UnlockFlagOfIgallta,
	byte UnlockFlagOfRrharil,
	byte FlagOfSongRecordKey,
	byte RandomVersionUnlocked,
	bool Chapter8UnlockBegin,
	bool Chapter8UnlockSecondPhase,
	bool Chapter8Passed,
	byte Chapter8SongUnlockFlag,
	byte FlagOfSongRecordKeyTakumi);
