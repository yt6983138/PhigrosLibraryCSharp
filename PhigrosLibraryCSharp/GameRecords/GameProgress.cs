namespace PhigrosLibraryCSharp.GameRecords;

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
/// <param name="Version">Version of the game progress.</param>
/// <param name="IsFirstRun">Indicates if the user is running the game for the first time.</param>
/// <param name="LegacyChapterFinished">Indicates that the user has done legacy chapter or not.</param>
/// <param name="AlreadyShowCollectionTip">Indicates that unlock tips for collections has shown or not.</param>
/// <param name="AlreadyShowAutoUnlockINTip">Indicates that unlock tips for <see cref="Difficulty.IN"/> has shown or not.</param>
/// <param name="Completed">[Unexplained]</param>
/// <param name="SongUpdateInfo">[Unexplained]</param>
/// <param name="ChallengeModeRank">The challenge mode rank.</param>
/// <param name="Money">The money count.</param>
/// <param name="UnlockFlagOfSpasmodic">[Unexplained]</param>
/// <param name="UnlockFlagOfIgallta">[Unexplained]</param>
/// <param name="UnlockFlagOfRrharil">[Unexplained]</param>
/// <param name="FlagOfSongRecordKey">[Unexplained]</param>
/// <param name="Node2">Next node of GameProgress.</param>
public record class GameProgress(
	byte Version,
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
	GameProgressNodeVersion2? Node2);

/// <summary>
/// Version 2 node for GameProgress.
/// </summary>
/// <param name="RandomVersionUnlocked">[Unexplained]</param>
/// <param name="Node3">Next node of GameProgress.</param>
public record class GameProgressNodeVersion2(
	byte RandomVersionUnlocked,
	GameProgressNodeVersion3? Node3);

/// <summary>
/// Version 4 node for GameProgress.
/// </summary>
/// <param name="Chapter8UnlockBegin">[Unexplained]</param>
/// <param name="Chapter8UnlockSecondPhase">[Unexplained]</param>
/// <param name="Chapter8Passed">Indicates that the user has passed chapter 8 or not.</param>
/// <param name="Chapter8SongUnlockFlag">[Unexplained]</param>
/// <param name="Node4">Next node of GameProgress.</param>
public record class GameProgressNodeVersion3(
	bool Chapter8UnlockBegin,
	bool Chapter8UnlockSecondPhase,
	bool Chapter8Passed,
	byte Chapter8SongUnlockFlag,
	GameProgressNodeVersion4? Node4);

/// <summary>
/// Version 4 node for GameProgress.
/// </summary>
/// <param name="FlagOfSongRecordKeyTakumi">[Unexplained]</param>
public record class GameProgressNodeVersion4(
	byte FlagOfSongRecordKeyTakumi);
