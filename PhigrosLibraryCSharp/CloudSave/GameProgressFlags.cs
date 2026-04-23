namespace PhigrosLibraryCSharp.CloudSave;

/// <summary>
/// Difficulty unlock flag, used to store the unlock state of each difficulty of a song. 
/// Each bit represents a difficulty, 0 = locked, 1 = unlocked.
/// </summary>
[Flags]
public enum DifficultyUnlockFlag : byte
{
	/// <summary>
	/// <see cref="Difficulty.EZ"/> difficulty.
	/// </summary>
	EZ = 1 << 0,
	/// <summary>
	/// <see cref="Difficulty.HD"/> difficulty.
	/// </summary>
	HD = 1 << 1,
	/// <summary>
	/// <see cref="Difficulty.IN"/> difficulty.
	/// </summary>
	IN = 1 << 2,
	/// <summary>
	/// <see cref="Difficulty.AT"/> difficulty.
	/// </summary>
	AT = 1 << 3,
}
/// <summary>
/// Song <c>Random</c> unlocked version flag. If a bit is set, the corresponding version of <c>Random</c> is unlocked.
/// </summary>
[Flags]
public enum RandomVersionFlag : byte
{
	/// <summary>
	/// The normal version, always unlocked.
	/// </summary>
	Normal = 0,
	/// <summary>
	/// The <c>R</c> version.
	/// </summary>
	R = 1 << 0,
	/// <summary>
	/// The <c>A</c> version.
	/// </summary>
	A = 1 << 1,
	/// <summary>
	/// The <c>M</c> version.
	/// </summary>
	N = 1 << 2,
	/// <summary>
	/// The <c>D</c> version.
	/// </summary>
	D = 1 << 3,
	/// <summary>
	/// The <c>O</c> version.
	/// </summary>
	O = 1 << 4,
	/// <summary>
	/// The <c>M</c> version.
	/// </summary>
	M = 1 << 5,
}
/// <summary>
/// The flag of various song record status, used in various places.
/// </summary>
[Flags]
public enum SongRecordFlag : byte
{
	/// <summary>
	/// Indicates player has got <see cref="ScoreStatus.S"/> on <c>You are the Miserable</c> on <see cref="Difficulty.IN"/> or not.
	/// Local key: <c>YouaretheMiserableINSGrade</c>
	/// </summary>
	YATMINSGrade = 1 << 0,
	/// <summary>
	/// Indicates player has got <see cref="ScoreStatus.S"/> on <c>Stasis</c> on <see cref="Difficulty.IN"/> or not.
	/// Local key: <c>StasisINSGrade</c>
	/// </summary>
	StasisINSGrade = 1 << 1,
	/// <summary>
	/// Indicates player has got <see cref="ScoreStatus.S"/> on <c>Shadow</c> on <see cref="Difficulty.IN"/> or not.
	/// Local key: <c>ShadowINSGrade</c>
	/// </summary>
	ShadowINSGrade = 1 << 2,
	/// <summary>
	/// Indicates player has got <see cref="ScoreStatus.S"/> on <c>心之所向</c> on <see cref="Difficulty.IN"/> or not.
	/// Local key: <c>心之所向INSGrade</c>
	/// </summary>
	XinZhiSuoXiangINSGrade = 1 << 3,
	/// <summary>
	/// Indicates player has got <see cref="ScoreStatus.S"/> on <c>Inferior</c> on <see cref="Difficulty.IN"/> or not.
	/// Local key: <c>inferiorINSGrade</c>
	/// </summary>
	InferiorINSGrade = 1 << 4,
	/// <summary>
	/// Indicates player has got <see cref="ScoreStatus.S"/> on <c>DESTRUCTION 3,2,1</c> on <see cref="Difficulty.IN"/> or not.
	/// Local key: <c>DESTRUCTION321INSGrade</c>
	/// </summary>
	Destruction321INSGrade = 1 << 5,
	/// <summary>
	/// Indicates player has got <see cref="ScoreStatus.S"/> on <c>Distorted Fate</c> on <see cref="Difficulty.IN"/> or not.
	/// Local key: <c>DistortedFateINSGrade</c>
	/// </summary>
	DistortedFateINSGrade = 1 << 6,
}
/// <summary>
/// Chapter 8 unlock flag, used to store the unlock state of chapter 8.
/// </summary>
[Flags]
public enum Chapter8UnlockFlag : byte
{
	/// <summary>
	/// Nothing unlocked.
	/// </summary>
	None = 0,
	/// <summary>
	/// The first phase has been unlocked.
	/// </summary>
	UnlockBegin = 1 << 0,
	/// <summary>
	/// The second phase has been unlocked.
	/// </summary>
	UnlockSecondPhase = 1 << 1,
}
/// <summary>
/// The song unlock flag for Takumi, used to store the unlock state of Takumi's songs.
/// </summary>
[Flags]
public enum TakumiUnlockFlag : byte
{
	/// <summary>
	/// Indicates player has got <see cref="ScoreStatus.S"/> on <c>Cuvism</c> on <see cref="Difficulty.IN"/> or not.
	/// Local key: <c>CuvismINSGrade</c>
	/// </summary>
	CuvismINSGrade = 1 << 0,
	/// <summary>
	/// Indicates player has got <see cref="ScoreStatus.S"/> on <c>iL-Artifact</c> on <see cref="Difficulty.IN"/> or not.
	/// Local key: <c>iLArtifactINSGrade</c>
	/// </summary>
	ILArtifactINSGrade = 1 << 1,
	/// <summary>
	/// Indicates player has got <see cref="ScoreStatus.S"/> on <c>a truth seeker -Communication with Utopia will be lost-</c> on <see cref="Difficulty.IN"/> or not.
	/// Local key: <c>atruthseekerCommunicationwithUtopiawillbelostINSGrade</c>
	/// </summary>
	ATruthSeekerINSGrade = 1 << 2,
}
