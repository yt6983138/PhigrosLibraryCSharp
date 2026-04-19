namespace PhigrosLibraryCSharp.GameRecords;

/// <summary>
/// The flag of the game key, which is used to store various state of the game.
/// Multiple illustrations/avatars/collections can share same key thus they have to be stored in a single flag.
/// </summary>
public struct GameKeyFlag
{
	/// <summary>
	/// The raw flag data storing collectible item state.
	/// Currently, this has not been further parsed into individual item states, 
	/// because I can't find all places that involve this flag in IDA.
	/// </summary>
	public ulong Flag { get; set; }

	/// <summary>
	/// Create a new instance of <see cref="GameKeyFlag"/> with the given raw flag data.
	/// </summary>
	/// <param name="rawFlags">The raw flag data to initialize the <see cref="GameKeyFlag"/>.</param>
	public GameKeyFlag(byte[] rawFlags)
	{
		for (int i = 0; i < rawFlags.Length; i++)
		{
			this.Flag |= (ulong)rawFlags[i] << (i * 8);
		}
	}
	/// <summary>
	/// Create a new instance of <see cref="GameKeyFlag"/> with the given raw flag data.
	/// </summary>
	/// <param name="flag">The raw flag data to initialize the <see cref="GameKeyFlag"/>.</param>
	public GameKeyFlag(ulong flag)
	{
		this.Flag = flag;
	}
}
