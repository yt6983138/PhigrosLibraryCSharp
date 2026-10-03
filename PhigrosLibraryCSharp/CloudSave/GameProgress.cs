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
		int songUpdateInfo,
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
	public int SongUpdateInfo { get; set; }

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

	/// <inheritdoc/>
	public static GameProgress FromReader(BinaryReader reader, byte objectVersion)
	{
		return new(
			objectVersion,
			reader.ReadFromPackedBoolNoJump(0),
			reader.ReadFromPackedBoolNoJump(1),
			reader.ReadFromPackedBoolNoJump(2),
			reader.ReadFromPackedBoolThenJump(3),
			reader.ReadString(),
			reader.Read7BitEncodedInt(),
			Challenge.FromReader(reader, objectVersion),
			Money.FromReader(reader, objectVersion),
			reader.ReadEnum<DifficultyUnlockFlag>(),
			reader.ReadEnum<DifficultyUnlockFlag>(),
			reader.ReadEnum<DifficultyUnlockFlag>(),
			reader.ReadEnum<SongRecordFlag>(),
			!reader.HasMore ? null :
			new(
				reader.ReadEnum<RandomVersionFlag>(),
				!reader.HasMore ? null : new(
					reader.ReadEnum<Chapter8UnlockFlag>(),
					reader.ReadEnum<DifficultyUnlockFlag>(),
					!reader.HasMore ? null : new(
						reader.ReadEnum<TakumiUnlockFlag>(),
						!reader.HasMore ? null : new(
							reader.ReadEnum<Chapter9UnlockFlag>(),
							reader.ReadEnum<Chapter9SongUnlockFlag>(),
							reader.ReadByte(),
							reader.ReadString())))));
	}
	/// <inheritdoc/>
	public void Serialize(BinaryWriter writer, out byte objectVersion)
	{
		objectVersion = this.Version;

		writer.WritePackedBools(this.IsFirstRun, this.LegacyChapterFinished, this.AlreadyShowCollectionTip, this.AlreadyShowAutoUnlockINTip);
		writer.Write(this.GameCompleted);
		writer.Write7BitEncodedInt(this.SongUpdateInfo);
		this.ChallengeModeRank.Serialize(writer, out _);
		this.Money.Serialize(writer, out _);
		writer.WriteEnum(this.UnlockFlagOfSpasmodic);
		writer.WriteEnum(this.UnlockFlagOfIgallta);
		writer.WriteEnum(this.UnlockFlagOfRrharil);
		writer.WriteEnum(this.FlagOfSongRecordKey);

		GameProgressNodeVersion2? node2 = this.Node2;
		GameProgressNodeVersion3? node3 = node2?.Node3;
		GameProgressNodeVersion4? node4 = node3?.Node4;
		GameProgressNodeVersion5? node5 = node4?.Node5;

		if (node2 is null) return;
		writer.WriteEnum(node2.RandomVersionUnlocked);

		if (node3 is null) return;
		writer.WriteEnum(node3.Chapter8UnlockFlag);
		writer.WriteEnum(node3.Chapter8SongUnlockFlag);

		if (node4 is null) return;
		writer.WriteEnum(node4.FlagOfSongRecordKeyTakumi);

		if (node5 is null) return;
		writer.WriteEnum(node5.Chapter9UnlockFlag);
		writer.WriteEnum(node5.Chapter9SongUnlockFlag);
		writer.Write((byte)((node5.Chapter9SecretChallengeSelectedLifeTier << 4) | (node5.Chapter9SecretChallengeLifeTier & 0x0F)));
		writer.Write(node5.Chapter9SecretPassword);
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

	/// <summary>Unlocked <c>Random</c> song versions.</summary>
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
	public GameProgressNodeVersion4(TakumiUnlockFlag flagOfSongRecordKeyTakumi, GameProgressNodeVersion5? node5)
	{
		this.FlagOfSongRecordKeyTakumi = flagOfSongRecordKeyTakumi;
		this.Node5 = node5;
	}

	/// <summary>Unlocked Takumi songs.</summary>
	public TakumiUnlockFlag FlagOfSongRecordKeyTakumi { get; set; }

	/// <summary>Next node of GameProgress.</summary>
	public GameProgressNodeVersion5? Node5 { get; set; }
}

/// <summary>
/// Version 5 node for GameProgress.
/// </summary>
public class GameProgressNodeVersion5
{

	/// <summary>
	/// Initializes a new instance of the <see cref="GameProgressNodeVersion5"/> class.
	/// </summary>
	public GameProgressNodeVersion5(Chapter9UnlockFlag chapter9UnlockFlag, Chapter9SongUnlockFlag chapter9SongUnlockFlag, byte packedLifeTier, string chapter9SecretPassword)
	{
		this.Chapter9UnlockFlag = chapter9UnlockFlag;
		this.Chapter9SongUnlockFlag = chapter9SongUnlockFlag;
		this.Chapter9SecretChallengeLifeTier = (byte)(packedLifeTier & 0x0F);
		this.Chapter9SecretChallengeSelectedLifeTier = (byte)((packedLifeTier >> 4) & 0x0F);
		this.Chapter9SecretPassword = chapter9SecretPassword;
	}

	/// <summary>
	/// Game progress in chapter 9.
	/// </summary>
	public Chapter9UnlockFlag Chapter9UnlockFlag { get; set; }
	/// <summary>
	/// Unlocked songs in chapter 9.
	/// </summary>
	public Chapter9SongUnlockFlag Chapter9SongUnlockFlag { get; set; }
	/// <summary>
	/// Local key: <c>C9SecretChallengeLifeTier</c>
	/// </summary>
	public byte Chapter9SecretChallengeLifeTier { get; set; }
	/// <summary>
	/// Local key: <c>C9SecretChallengeSelectedLifeTier</c>
	/// </summary>
	public byte Chapter9SecretChallengeSelectedLifeTier { get; set; }
	/// <summary>
	/// Secret password for chapter 9 last stage.
	/// </summary>
	public string Chapter9SecretPassword { get; set; }
}