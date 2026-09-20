using System.Numerics;

namespace PhigrosLibraryCSharp.CloudSave;

/// <summary>
/// The Phigros currency.
/// </summary>
public class Money : IPhigrosCustomSerialization<Money>, IEquatable<Money>, IEqualityOperators<Money, Money, bool>, IComparable<Money>
{
	/// <summary>
	/// Default value of <see cref="Money"/>, all counts are 0. Returns a new instance every time.
	/// </summary>
	public static Money Zero => new(0, 0, 0, 0, 0);

	/// <summary>KiB count.</summary>
	public int KiB { get; set; }

	/// <summary>MiB count.</summary>
	public int MiB { get; set; }

	/// <summary>GiB count.</summary>
	public int GiB { get; set; }

	/// <summary>TiB count.</summary>
	public int TiB { get; set; }

	/// <summary>PiB count.</summary>
	public int PiB { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="Money"/> class.
	/// </summary>
	/// <param name="kiB">KiB count.</param>
	/// <param name="miB">MiB count.</param>
	/// <param name="giB">GiB count.</param>
	/// <param name="tiB">TiB count.</param>
	/// <param name="piB">PiB count.</param>
	public Money(int kiB, int miB, int giB, int tiB, int piB)
	{
		this.KiB = kiB;
		this.MiB = miB;
		this.GiB = giB;
		this.TiB = tiB;
		this.PiB = piB;
	}

	/// <inheritdoc/>
	public override bool Equals(object? obj)
	{
		if (obj is not Money other)
			return false;

		return this.Equals(other);
	}
	/// <inheritdoc/>
	public override int GetHashCode()
	{
		return HashCode.Combine(this.KiB, this.MiB, this.GiB, this.TiB, this.PiB);
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
	public static Money FromReader(BinaryReader reader, byte objectVersion)
	{
		return new(
			reader.Read7BitEncodedInt(),
			reader.Read7BitEncodedInt(),
			reader.Read7BitEncodedInt(),
			reader.Read7BitEncodedInt(),
			reader.Read7BitEncodedInt());
	}
	/// <inheritdoc/>
	public void Serialize(BinaryWriter writer, out byte objectVersion)
	{
		objectVersion = byte.MaxValue;

		writer.Write7BitEncodedInt(this.KiB);
		writer.Write7BitEncodedInt(this.MiB);
		writer.Write7BitEncodedInt(this.GiB);
		writer.Write7BitEncodedInt(this.TiB);
		writer.Write7BitEncodedInt(this.PiB);
	}

	/// <inheritdoc/>
	public virtual bool Equals(Money? other)
	{
		if (other is null) return false;

		return this.KiB == other.KiB
			&& this.MiB == other.MiB
			&& this.GiB == other.GiB
			&& this.TiB == other.TiB
			&& this.PiB == other.PiB;
	}
	/// <inheritdoc/>
	public int CompareTo(Money? other)
	{
		if (other is null) return 1;

		if (this.PiB != other.PiB) return this.PiB.CompareTo(other.PiB);
		if (this.TiB != other.TiB) return this.TiB.CompareTo(other.TiB);
		if (this.GiB != other.GiB) return this.GiB.CompareTo(other.GiB);
		if (this.MiB != other.MiB) return this.MiB.CompareTo(other.MiB);

		return this.KiB.CompareTo(other.KiB);
	}

	/// <inheritdoc/>
	public static bool operator ==(Money? left, Money? right)
	{
		if (left is null) return right is null;

		return left.Equals(right);
	}
	/// <inheritdoc/>
	public static bool operator !=(Money? left, Money? right)
	{
		return !(left == right);
	}
	/// <inheritdoc/>
	public static bool operator <(Money? left, Money? right)
	{
		if (left is null || right is null) return false;
		return left.CompareTo(right) < 0;
	}
	/// <inheritdoc/>
	public static bool operator >(Money? left, Money? right)
	{
		if (left is null || right is null) return false;
		return left.CompareTo(right) > 0;
	}
	/// <inheritdoc/>
	public static bool operator <=(Money? left, Money? right)
	{
		if (left is null) return right is null;
		if (right is null) return false;

		return left.CompareTo(right) <= 0;
	}
	/// <inheritdoc/>
	public static bool operator >=(Money? left, Money? right)
	{
		if (left is null) return right is null;
		if (right is null) return false;

		return left.CompareTo(right) >= 0;
	}
}
