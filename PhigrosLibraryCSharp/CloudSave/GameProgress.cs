using PhigrosLibraryCSharp.Serialization;

namespace PhigrosLibraryCSharp.CloudSave;

/// <summary>
/// The user's game progress.
/// </summary>
public class GameProgress : IPhigrosCustomSerialization<GameProgress>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="GameProgress"/> class.
	/// </summary>
	public GameProgress(
		byte version,
		bool isFirstRun,
		bool legacyChapterFinished,
		bool alreadyShowCollectionTip,
		bool alreadyShowAutoUnlockINTip,
		string completed,
		short songUpdateInfo,
		Challenge challengeModeRank,
		Money money,
		DifficultyUnlockFlag unlockFlagOfSpasmodic,
		DifficultyUnlockFlag unlockFlagOfIgallta,
		DifficultyUnlockFlag unlockFlagOfRrharil,
		SongRecordFlag flagOfSongRecordKey,
		GameProgressNodeVersion2? node2)
	{
		this.Version = version;
		this.IsFirstRun = isFirstRun;
		this.LegacyChapterFinished = legacyChapterFinished;
		this.AlreadyShowCollectionTip = alreadyShowCollectionTip;
		this.AlreadyShowAutoUnlockINTip = alreadyShowAutoUnlockINTip;
		this.GameCompleted = completed;
		this.SongUpdateInfo = songUpdateInfo;
		this.ChallengeModeRank = challengeModeRank;
		this.Money = money;
		this.UnlockFlagOfSpasmodic = unlockFlagOfSpasmodic;
		this.UnlockFlagOfIgallta = unlockFlagOfIgallta;
		this.UnlockFlagOfRrharil = unlockFlagOfRrharil;
		this.FlagOfSongRecordKey = flagOfSongRecordKey;
		this.Node2 = node2;
	}

	/// <summary>Version of the game progress. Latest: 4</summary>
	public byte Version { get; set; }

	/// <summary>Indicates if the user is running the game for the first time.</summary>
	public bool IsFirstRun { get; set; }

	/// <summary>Indicates that the user has done legacy chapter or not.</summary>
	public bool LegacyChapterFinished { get; set; }

	/// <summary>Indicates that unlock tips for collections has shown or not.</summary>
	public bool AlreadyShowCollectionTip { get; set; }

	/// <summary>Indicates that unlock tips for <see cref="Difficulty.IN"/> has shown or not.</summary>
	public bool AlreadyShowAutoUnlockINTip { get; set; }

	/// <summary>Known values: <see cref="string.Empty"/>, <c>3.0</c>
	/// Known usages: If this is not <see cref="string.Empty"/>, legacy song selection is unlocked.
	/// AboutUs scene will set this to <c>3.0</c> when it starts playing.</summary>
	public string GameCompleted { get; set; }

	/// <summary>Seems to save the count of song update information, <i>may be wrong.</i></summary>
	public short SongUpdateInfo { get; set; }

	/// <summary>The challenge mode rank.</summary>
	public Challenge ChallengeModeRank { get; set; }

	/// <summary>The money count.</summary>
	public Money Money { get; set; }

	/// <summary>Difficulty unlock flag for Spasmodic.</summary>
	public DifficultyUnlockFlag UnlockFlagOfSpasmodic { get; set; }

	/// <summary>Difficulty unlock flag for Igallta.</summary>
	public DifficultyUnlockFlag UnlockFlagOfIgallta { get; set; }

	/// <summary>Difficulty unlock flag for Rrharil.</summary>
	public DifficultyUnlockFlag UnlockFlagOfRrharil { get; set; }

	/// <summary>Stores various song record status, used in various places.</summary>
	public SongRecordFlag FlagOfSongRecordKey { get; set; }

	/// <summary>Next node of GameProgress.</summary>
	public GameProgressNodeVersion2? Node2 { get; set; }

	public static GameProgress FromReader(ByteReader reader)
	{
		return new(
			reader.ObjectVersion,
			reader.ReadFromPackedBoolNoJump(0),
			reader.ReadFromPackedBoolNoJump(1),
			reader.ReadFromPackedBoolNoJump(2),
			reader.ReadFromPackedBoolThenJump(3),
			reader.ReadString(),
			reader.ReadVariedInteger(),
			Challenge.FromReader(reader),
			Money.FromReader(reader),
			reader.ReadUnmanaged<DifficultyUnlockFlag>(),
			reader.ReadUnmanaged<DifficultyUnlockFlag>(),
			reader.ReadUnmanaged<DifficultyUnlockFlag>(),
			reader.ReadUnmanaged<SongRecordFlag>(),
			!reader.HasMore ? null :
			new(
				reader.ReadUnmanaged<RandomVersionFlag>(),
				!reader.HasMore ? null : new(
					reader.ReadUnmanaged<Chapter8UnlockFlag>(),
					reader.ReadUnmanaged<DifficultyUnlockFlag>(),
					!reader.HasMore ? null : new(
						reader.ReadUnmanaged<TakumiUnlockFlag>()))));
	}
	public void Serialize(ByteWriter writer)
	{
		writer.ObjectVersion = this.Version;

		writer.WritePackedBools(this.IsFirstRun, this.LegacyChapterFinished, this.AlreadyShowCollectionTip, this.AlreadyShowAutoUnlockINTip);
		writer.WriteString(this.GameCompleted);
		writer.WriteVariedInteger(this.SongUpdateInfo);
		this.ChallengeModeRank.Serialize(writer);
		this.Money.Serialize(writer);
		writer.WriteUnmanaged(this.UnlockFlagOfSpasmodic);
		writer.WriteUnmanaged(this.UnlockFlagOfIgallta);
		writer.WriteUnmanaged(this.UnlockFlagOfRrharil);
		writer.WriteUnmanaged(this.FlagOfSongRecordKey);

		if (this.Node2 is null) return;
		writer.WriteUnmanaged(this.Node2.RandomVersionUnlocked);

		if (this.Node2.Node3 is null) return;
		writer.WriteUnmanaged(this.Node2.Node3.Chapter8UnlockFlag);
		writer.WriteUnmanaged(this.Node2.Node3.Chapter8SongUnlockFlag);

		if (this.Node2.Node3.Node4 is null) return;
		writer.WriteUnmanaged(this.Node2.Node3.Node4.FlagOfSongRecordKeyTakumi);
	}
}

/// <summary>
/// Version 2 node for GameProgress.
/// </summary>
public class GameProgressNodeVersion2
{
	/// <summary>
	/// Initializes a new instance of the <see cref="GameProgressNodeVersion2"/> class.
	/// </summary>
	public GameProgressNodeVersion2(RandomVersionFlag randomVersionUnlocked, GameProgressNodeVersion3? node3)
	{
		this.RandomVersionUnlocked = randomVersionUnlocked;
		this.Node3 = node3;
	}

	/// <summary>[Unexplained]</summary>
	public RandomVersionFlag RandomVersionUnlocked { get; set; }

	/// <summary>Next node of GameProgress.</summary>
	public GameProgressNodeVersion3? Node3 { get; set; }
}

/// <summary>
/// Version 3 node for GameProgress.
/// </summary>
public class GameProgressNodeVersion3
{
	/// <summary>
	/// Initializes a new instance of the <see cref="GameProgressNodeVersion3"/> class.
	/// </summary>
	public GameProgressNodeVersion3(
		Chapter8UnlockFlag chapter8UnlockFlag,
		DifficultyUnlockFlag chapter8SongUnlockFlag,
		GameProgressNodeVersion4? node4)
	{
		this.Chapter8UnlockFlag = chapter8UnlockFlag;
		this.Chapter8SongUnlockFlag = chapter8SongUnlockFlag;
		this.Node4 = node4;
	}

	/// <summary>
	/// User's progress in chapter 8.
	/// </summary>
	public Chapter8UnlockFlag Chapter8UnlockFlag { get; set; }

	/// <summary>Difficulty unlock flag for chapter 8 songs.</summary>
	public DifficultyUnlockFlag Chapter8SongUnlockFlag { get; set; }

	/// <summary>Next node of GameProgress.</summary>
	public GameProgressNodeVersion4? Node4 { get; set; }
}

/// <summary>
/// Version 4 node for GameProgress.
/// </summary>
public class GameProgressNodeVersion4
{
	/// <summary>
	/// Initializes a new instance of the <see cref="GameProgressNodeVersion4"/> class.
	/// </summary>
	public GameProgressNodeVersion4(TakumiUnlockFlag flagOfSongRecordKeyTakumi)
	{
		this.FlagOfSongRecordKeyTakumi = flagOfSongRecordKeyTakumi;
	}

	/// <summary>Unlocked Takumi songs.</summary>
	public TakumiUnlockFlag FlagOfSongRecordKeyTakumi { get; set; }
}
