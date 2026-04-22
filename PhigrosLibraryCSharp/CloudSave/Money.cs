using PhigrosLibraryCSharp.Serialization;

namespace PhigrosLibraryCSharp.CloudSave;

/// <summary>
/// The Phigros currency.
/// </summary>
public class Money : IPhigrosCustomSerialization<Money>
{
	/// <summary>KiB count.</summary>
	public short KiB { get; set; }

	/// <summary>MiB count.</summary>
	public short MiB { get; set; }

	/// <summary>GiB count.</summary>
	public short GiB { get; set; }

	/// <summary>TiB count.</summary>
	public short TiB { get; set; }

	/// <summary>PiB count.</summary>
	public short PiB { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="Money"/> class.
	/// </summary>
	/// <param name="kiB">KiB count.</param>
	/// <param name="miB">MiB count.</param>
	/// <param name="giB">GiB count.</param>
	/// <param name="tiB">TiB count.</param>
	/// <param name="piB">PiB count.</param>
	public Money(short kiB, short miB, short giB, short tiB, short piB)
	{
		this.KiB = kiB;
		this.MiB = miB;
		this.GiB = giB;
		this.TiB = tiB;
		this.PiB = piB;
	}

	/// <inheritdoc/>
	public override string ToString()
	{
		return (this.KiB, this.MiB, this.GiB, this.TiB, this.PiB) switch
		{
			(_, _, _, _, > 0) => $"{this.PiB} PiB, {this.TiB} TiB, {this.GiB} GiB, {this.MiB} MiB, {this.KiB} KiB",
			(_, _, _, > 0, _) => $"{this.TiB} TiB, {this.GiB} GiB, {this.MiB} MiB, {this.KiB} KiB",
			(_, _, > 0, _, _) => $"{this.GiB} GiB, {this.MiB} MiB, {this.KiB} KiB",
			(_, > 0, _, _, _) => $"{this.MiB} MiB, {this.KiB} KiB",
			(_, _, _, _, _) => $"{this.KiB} KiB"
		};
	}

	/// <inheritdoc/>
	public static Money FromReader(ByteReader reader)
	{
		return new(
			reader.ReadVariedInteger(),
			reader.ReadVariedInteger(),
			reader.ReadVariedInteger(),
			reader.ReadVariedInteger(),
			reader.ReadVariedInteger());
	}
	/// <inheritdoc/>
	public void Serialize(ByteWriter writer)
	{
		writer.WriteVariedInteger(this.KiB);
		writer.WriteVariedInteger(this.MiB);
		writer.WriteVariedInteger(this.GiB);
		writer.WriteVariedInteger(this.TiB);
		writer.WriteVariedInteger(this.PiB);
	}
}
