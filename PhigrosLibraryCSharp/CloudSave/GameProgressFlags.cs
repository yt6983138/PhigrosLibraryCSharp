namespace PhigrosLibraryCSharp.CloudSave;

[Flags]
public enum DifficultyUnlockFlag : byte
{
	EZ = 1 << 0,
	HD = 1 << 1,
	IN = 1 << 2,
	AT = 1 << 3,
}
[Flags]
public enum RandomVersionFlag : byte
{
	Normal = 0,
	R = 1 << 0,
	A = 1 << 1,
	N = 1 << 2,
	D = 1 << 3,
	O = 1 << 4,
	M = 1 << 5,
}
[Flags]
public enum SongRecordFlag : byte
{
	//YouaretheMiserableINSGrade
	YATMINSGrade = 1 << 0,
	//StasisINSGrade
	StasisINSGrade = 1 << 1,
	//ShadowINSGrade
	ShadowINSGrade = 1 << 2,
	//心之所向INSGrade
	XinZhiSuoXiangINSGrade = 1 << 3,
	//inferiorINSGrade
	InferiorINSGrade = 1 << 4,
	//DESTRUCTION321INSGrade
	Destruction321INSGrade = 1 << 5,
	//DistortedFateINSGrade
	DistortedFateINSGrade = 1 << 6,
}
[Flags]
public enum Chapter8UnlockFlag : byte
{
	None = 0,
	UnlockBegin = 1 << 0,
	UnlockSecondPhase = 1 << 1,
}
[Flags]
public enum TakumiUnlockFlag : byte
{
	CuvismINSGrade = 1 << 0,
	/// <summary>
	/// iLArtifactINSGrade
	/// </summary>
	ILArtifactINSGrade = 1 << 1,
	/// <summary>
	/// atruthseekerCommunicationwithUtopiawillbelostINSGrade
	/// </summary>
	ATruthSeekerINSGrade = 1 << 2,
}
