using PhigrosLibraryCSharp.Serialization;

namespace PhigrosLibraryCSharp.CloudSave;

public enum GameKeyFlagType : byte
{
	HasReadCollectionPieceCount = 1 << 0,
	HasUnlockedSingle = 1 << 1,
	HasUnlockedCollectionPieceCount = 1 << 2,
	HasUnlockedIllustration = 1 << 3,
	HasUnlockedAvatar = 1 << 4
}

/// <summary>
/// The flag of the game key, which is used to store various state of the game.
/// Multiple illustrations/avatars/collections can share same key thus they have to be stored in a single flag.
/// </summary>
public struct GameKeyFlag : IPhigrosCustomSerialization<GameKeyFlag>
{
	public GameKeyFlagType Type { get; set; }
	public ulong Payload { get; set; }

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
	/// <param name="data">The raw flag data to initialize the <see cref="GameKeyFlag"/>.</param>
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

	public void WritePayload(GameKeyFlagType position, byte payload)
	{
		int bitPosition = ValidatePositionHasOnly1Bit(position);

		this.Type |= position;
		this.Payload &= ~(0xFFUL << (bitPosition * 8));
		this.Payload |= (ulong)payload << (bitPosition * 8);
	}
	public void RemovePayload(GameKeyFlagType position)
	{
		int bitPosition = ValidatePositionHasOnly1Bit(position);

		this.Type &= ~position;
		this.Payload &= ~(0xFFUL << (bitPosition * 8));
	}
	public byte ReadPayload(GameKeyFlagType position)
	{
		int bitPosition = ValidatePositionHasOnly1Bit(position);
		if ((position & this.Type) == 0) throw new ArgumentException("The position has no payload.", nameof(position));

		return (byte)((this.Payload >> (bitPosition * 8)) & 0xFF);
	}

	public static GameKeyFlag FromReader(ByteReader reader)
	{
		byte length = reader.ReadByte();
		byte[] rawFlags = reader.ReadBytes(length);
		return new(rawFlags);
	}
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
