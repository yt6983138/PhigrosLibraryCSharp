using PhigrosLibraryCSharp.Serialization;

namespace PhigrosLibraryCSharp.CloudSave;

/// <summary>
/// The type of the game key flag, which is used to indicate the type of the payload in the flag.
/// Multiple types may exist in a single flag.
/// </summary>
[Flags]
public enum GameKeyFlagType : byte
{
	/// <summary>
	/// Has stored the count of <i>read</i> collection pieces.
	/// </summary>
	HasReadCollectionPieceCount = 1 << 0,
	/// <summary>
	/// Has unlocked the song in Single collection.
	/// </summary>
	HasUnlockedSingle = 1 << 1,
	/// <summary>
	/// Has stored the count of <i>unlocked</i> collection pieces. Note: This is different from <see cref="HasReadCollectionPieceCount"/>.
	/// </summary>
	HasUnlockedCollectionPieceCount = 1 << 2,
	/// <summary>
	/// Has unlocked the illustration.
	/// </summary>
	HasUnlockedIllustration = 1 << 3,
	/// <summary>
	/// Has unlocked the avatar.
	/// </summary>
	HasUnlockedAvatar = 1 << 4
}

/// <summary>
/// The flag of the game key, which is used to store various state of the game.
/// Multiple illustrations/avatars/collections can share same key thus they have to be stored in a single flag.
/// Note: This does not map directly to save binary structure, so do not use Read/WriteMarshalable or Read/WriteUnmanaged 
/// to serialize/deserialize this struct, use the provided FromReader and Serialize methods instead.
/// </summary>
public struct GameKeyFlag : IPhigrosCustomSerialization<GameKeyFlag>
{
	/// <summary>
	/// Types of the payload in the flag. Multiple types may exist in a single flag.
	/// </summary>
	public GameKeyFlagType Type { get; set; }
	/// <summary>
	/// The packed payload of the flag, please use <see cref="ReadPayload"/> and <see cref="WritePayload"/> 
	/// to read/write the payload instead of manipulating this field directly.
	/// </summary>
	public ulong Payload { get; set; }

	/// <summary>
	/// The count of the payload in the flag, which is determined by the number of bits set in the <see cref="Type"/>.
	/// </summary>
	public readonly byte PayloadCount
	{
		get
		{
			byte flag = (byte)this.Type;

			byte count = 0;
			for (byte i = 0; i < 8; i++)
			{
				if ((flag & (1 << i)) != 0)
					count++;
			}
			return count;
		}
	}

	/// <summary>
	/// Create a new instance of <see cref="GameKeyFlag"/> with the given raw flag data.
	/// </summary>
	/// <param name="packedFlag">The packed flag data representing the types of the payload.</param>
	/// <param name="data">The raw payload data to initialize the <see cref="GameKeyFlag"/>.</param>
	public GameKeyFlag(byte packedFlag, byte[] data)
	{
		this.Type = (GameKeyFlagType)packedFlag;

		int flagCount = 0;
		for (int i = 0; i < 8; i++)
		{
			if ((packedFlag & (1 << i)) == 0) continue;

			byte payload = data[flagCount];
			this.Payload |= (ulong)payload << (i * 8);

			flagCount++;
		}
	}
	/// <summary>
	/// Create a new instance of <see cref="GameKeyFlag"/> with the given raw flag data.
	/// </summary>
	/// <param name="data">The raw data containing the types and payload to initialize the <see cref="GameKeyFlag"/>.</param>
	public GameKeyFlag(byte[] data)
		: this(data[0], data[1..]) { }

	/// <summary>
	/// 
	/// </summary>
	/// <param name="position"></param>
	/// <returns>offset of the bit</returns>
	/// <exception cref="ArgumentException"></exception>
	private static int ValidatePositionHasOnly1Bit(GameKeyFlagType position)
	{
		int bitPosition = -1;
		for (int i = 0; i < 8; i++)
		{
			if (((byte)position & (1 << i)) == 0) continue;

			if (bitPosition != -1) throw new ArgumentException("The position has more than 1 bit set.", nameof(position));
			bitPosition = i;
		}
		if (bitPosition == -1) throw new ArgumentException("The position has no bit set.", nameof(position));

		return bitPosition;
	}

	/// <summary>
	/// Write the payload to the flag at the specified position. 
	/// This will set the corresponding bit in <see cref="Type"/> and update the <see cref="Payload"/> accordingly.
	/// </summary>
	/// <param name="position">The position in the flag to write the payload.</param>
	/// <param name="payload">The payload value to write.</param>
	/// <exception cref="ArgumentException">Thrown when the position has more than 1 bit set or no bit set.</exception>
	public void WritePayload(GameKeyFlagType position, byte payload)
	{
		int bitPosition = ValidatePositionHasOnly1Bit(position);

		this.Type |= position;
		this.Payload &= ~(0xFFUL << (bitPosition * 8));
		this.Payload |= (ulong)payload << (bitPosition * 8);
	}
	/// <summary>
	/// Removes the payload from the flag at the specified position. 
	/// This will clear the corresponding bit in <see cref="Type"/> and update the <see cref="Payload"/> accordingly.
	/// </summary>
	/// <param name="position">The position in the flag to remove the payload.</param>
	/// <exception cref="ArgumentException">Thrown when the position has more than 1 bit set or no bit set.</exception>
	public void RemovePayload(GameKeyFlagType position)
	{
		int bitPosition = ValidatePositionHasOnly1Bit(position);

		this.Type &= ~position;
		this.Payload &= ~(0xFFUL << (bitPosition * 8));
	}
	/// <summary>
	/// Reads the payload from the flag at the specified position. 
	/// This will return the value of the payload without modifying the <see cref="Type"/> or <see cref="Payload"/>.
	/// </summary>
	/// <param name="position">The position in the flag to read the payload.</param>
	/// <returns>The value of the payload at the specified position.</returns>
	/// <exception cref="ArgumentException">Thrown when the position has more than 1 bit set, no bit set, or no payload at the position.</exception>
	public byte ReadPayload(GameKeyFlagType position)
	{
		int bitPosition = ValidatePositionHasOnly1Bit(position);
		if ((position & this.Type) == 0) throw new ArgumentException("The position has no payload.", nameof(position));

		return (byte)((this.Payload >> (bitPosition * 8)) & 0xFF);
	}

	/// <inheritdoc/>
	public static GameKeyFlag FromReader(ByteReader reader)
	{
		byte length = reader.ReadByte();
		byte[] rawFlags = reader.ReadBytes(length);
		return new(rawFlags);
	}
	/// <inheritdoc/>
	public void Serialize(ByteWriter writer)
	{
		writer.WriteByte((byte)(this.PayloadCount + sizeof(GameKeyFlagType)));
		writer.WriteUnmanaged(this.Type);

		for (int i = 0; i < 8; i++)
		{
			if (((byte)this.Type & (1 << i)) == 0) continue;

			byte data = (byte)((this.Payload >> (i * 8)) & 0xFF);
			writer.WriteByte(data);
		}
	}
}
