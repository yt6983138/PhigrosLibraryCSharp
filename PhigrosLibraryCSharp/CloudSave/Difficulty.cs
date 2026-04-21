namespace PhigrosLibraryCSharp.CloudSave;

/// <summary>
/// The phigros difficulties.
/// </summary>
public enum Difficulty
{
	/// <summary>
	/// EZ, easy
	/// </summary>
	EZ = 0,
	/// <summary>
	/// HD, hard
	/// </summary>
	HD = 1,
	/// <summary>
	/// IN, insane
	/// </summary>
	IN = 2,
	/// <summary>
	/// AT, another
	/// </summary>
	AT = 3,
	/// <summary>
	/// Legacy, only for some old charts, not used anymore, kept for serialization
	/// </summary>
	Legacy = 4,
	/// <summary>
	/// Special, only for special charts such as April fools chart, kept for serialization
	/// </summary>
	SP = 5
}