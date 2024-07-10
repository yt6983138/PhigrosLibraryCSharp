namespace PhigrosLibraryCSharp.GameRecords;

/// <summary>
/// A enum representing the status of a score.
/// </summary>
public enum ScoreStatus
{
	/// <summary>
	/// Bugged, this usually happens when a score have 100% accuracy but not 1,000,000 score.
	/// </summary>
	Bugged = -1,
	/// <summary>
	/// Presenting that the score isn't fc'ed. Not used outside raw score parsing.
	/// </summary>
	NotFc = 0,
	/// <summary>
	/// Full combo.
	/// </summary>
	Fc = 1,
	/// <summary>
	/// Phi, all perfect (aka 100% accuracy)
	/// </summary>
	Phi = 2,
	/// <summary>
	/// Score >= 960,000
	/// </summary>
	Vu = 3,
	/// <summary>
	/// Score >= 920,000
	/// </summary>
	S = 4,
	/// <summary>
	/// Score >= 880,000
	/// </summary>
	A = 5,
	/// <summary>
	/// Score >= 820,000
	/// </summary>
	B = 6,
	/// <summary>
	/// Score >= 700,000
	/// </summary>
	C = 7,
	/// <summary>
	/// Score &lt; 700,000
	/// </summary>
	False = 8
}
