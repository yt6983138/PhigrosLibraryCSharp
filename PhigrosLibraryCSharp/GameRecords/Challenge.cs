namespace PhigrosLibraryCSharp.GameRecords;

/// <summary>
/// The challenge rank of the user's save.
/// </summary>
public enum ChallengeRank
{
	/// <summary>
	/// The white rank (cannot be obtained in normal way), or no challenge code present.
	/// </summary>
	WhiteOrNone = 0,
	/// <summary>
	/// The green rank (average score >= 820000)
	/// </summary>
	Green = 1,
	/// <summary>
	/// The blue rank (average score >= 900000)
	/// </summary>
	Blue = 2,
	/// <summary>
	/// The red rank (average score >= 950000)
	/// </summary>
	Red = 3,
	/// <summary>
	/// The gold rank (average score >= 980000)
	/// </summary>
	Gold = 4,
	/// <summary>
	/// The rainbow rank (average score = 1000000)
	/// </summary>
	Rainbow = 5
}
/// <summary>
/// The challenge wrapper for challenge code.
/// </summary>
public struct Challenge
{
	/// <summary>
	/// The raw, underlying code. ex. 548, 446, 114, 514.
	/// </summary>
	public short RawCode;

	/// <summary>
	/// The challenge rank parsed from the code.
	/// </summary>
	public readonly ChallengeRank Rank => (ChallengeRank)(this.RawCode / 100);
	/// <summary>
	/// The challenge level parsed from the code.
	/// </summary>
	public readonly byte Level => unchecked((byte)(this.RawCode % 100));
	/// <summary>
	/// If user have ever done challenge (and reached rank <see cref="ChallengeRank.Green"/>), 
	/// returns <see langword="true"/>, otherwise <see langword="false"/>.
	/// </summary>
	public readonly bool Done => this.RawCode != 0;

	/// <summary>
	/// Constructs a new instance of <see cref="Challenge"/>.
	/// </summary>
	/// <param name="code">The raw code.</param>
	public Challenge(ushort code)
	{
		this.RawCode = unchecked((short)code);
	}
	/// <summary>
	/// Constructs a new instance of <see cref="Challenge"/>.
	/// </summary>
	/// <param name="code">The raw code.</param>
	public Challenge(short code)
	{
		this.RawCode = code;
	}
}
