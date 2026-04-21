using System.Runtime.InteropServices;
using System.Text;

namespace PhigrosLibraryCSharp.Serialization;

public class ByteWriter
{
	public Stream BaseStream { get; set; }
	public byte ObjectVersion { get; set; }

	public ByteWriter(Stream stream, byte version)
	{
		this.BaseStream = stream;
		this.ObjectVersion = version;
	}

	/// <summary>
	/// Writes the data as unmanaged struct.
	/// With structs that are managed but marshalable, use <see cref="WriteMarshalable{T}"/>.
	/// </summary>
	/// <typeparam name="T">Unmanaged struct.</typeparam>
	/// <param name="data">The struct to write.</param>
	public unsafe void WriteUnmanaged<T>(T data) where T : unmanaged
	{
		byte* buffer = stackalloc byte[sizeof(T)];
		*(T*)buffer = data;

		this.BaseStream.Write(new ReadOnlySpan<byte>(buffer, sizeof(T)));
	}

	/// <summary>
	/// Writes the data as marshalable struct.
	/// </summary>
	/// <typeparam name="T">Marshalable struct.</typeparam>
	/// <param name="data">The struct to write.</param>
	public unsafe void WriteMarshalable<T>(T data) where T : struct
	{
		int size = Marshal.SizeOf<T>();
		byte* buffer = stackalloc byte[size];

		Marshal.StructureToPtr(data, (IntPtr)buffer, false);
		this.BaseStream.Write(new ReadOnlySpan<byte>(buffer, size));
	}

	public void WriteBool(bool data)
		=> this.WriteByte((byte)(data ? 1 : 0));

	/// <summary>
	/// Writes a byte at current position.
	/// </summary>
	/// <param name="data">The byte to write.</param>
	public void WriteByte(byte data)
		=> this.WriteUnmanaged(data);

	/// <summary>
	/// Writes a sbyte at current position.
	/// </summary>
	/// <param name="data">The sbyte to write.</param>
	public void WriteSignedByte(sbyte data)
		=> this.WriteUnmanaged(data);

	/// <summary>
	/// Writes a short at current position.
	/// </summary>
	/// <param name="data">The short to write.</param>
	public void WriteShort(short data)
		=> this.WriteUnmanaged(data);

	/// <summary>
	/// Writes a ushort at current position.
	/// </summary>
	/// <param name="data">The ushort to write.</param>
	public void WriteUnsignedShort(ushort data)
		=> this.WriteUnmanaged(data);

	/// <summary>
	/// Writes a int at current position.
	/// </summary>
	/// <param name="data">The int to write.</param>
	public void WriteInt(int data)
		=> this.WriteUnmanaged(data);

	/// <summary>
	/// Writes a uint at current position.
	/// </summary>
	/// <param name="data">The uint to write.</param>
	public void WriteUnsignedInt(uint data)
		=> this.WriteUnmanaged(data);

	/// <summary>
	/// Writes a long at current position.
	/// </summary>
	/// <param name="data">The long to write.</param>
	public void WriteLong(long data)
		=> this.WriteUnmanaged(data);

	/// <summary>
	/// Writes a ulong at current position.
	/// </summary>
	/// <param name="data">The ulong to write.</param>
	public void WriteUnsignedLong(ulong data)
		=> this.WriteUnmanaged(data);

	/// <summary>
	/// Writes a float at current position.
	/// </summary>
	/// <param name="data">The float to write.</param>
	public void WriteFloat(float data)
		=> this.WriteUnmanaged(data);

	/// <summary>
	/// Writes a double at current position.
	/// </summary>
	/// <param name="data">The double to write.</param>
	public void WriteDouble(double data)
		=> this.WriteUnmanaged(data);

	/// <summary>
	/// Writes multiple bools at current position, with a byte of space. 
	/// The bools will be packed into a byte, with the first bool at the least significant bit.
	/// Note: This method can only write up to 8 bools, otherwise an exception will be thrown.
	/// </summary>
	/// <param name="bools">Bools to be packed.</param>
	/// <exception cref="ArgumentException">Thrown if there are more than 8 bools provided.</exception>
	public void WritePackedBools(params Span<bool> bools)
	{
		if (bools.Length > 8)
			throw new ArgumentException("Too many bools to be packed into a byte.", nameof(bools));

		byte data = 0;
		for (int i = 0; i < bools.Length; i++)
		{
			data |= (byte)((bools[i] ? 1 : 0) << i);
		}
		this.WriteByte(data);
	}

	/// <summary>
	/// Writes a variable integer at current position.
	/// </summary>
	/// <param name="data"></param>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public void WriteVariedInteger(short data)
	{
		if (data > 0b00111111_11111111)
			throw new ArgumentOutOfRangeException(nameof(data), "Data is too large to be written as a variable integer.");


		if (data > 0b01111111)
		{
			ushort buffer = 0;

			buffer |= (ushort)(data & 0b01111111);
			buffer |= 0b10000000;
			buffer |= (ushort)((data << 1) & 0b01111111_00000000);
			this.WriteUnsignedShort(buffer);
		}
		else
		{
			this.WriteByte((byte)data);
		}
	}

	/// <summary>
	/// Writes a string at current position. 
	/// Note: this method does not write the length of the string, you have to write it by yourself if needed.
	/// </summary>
	/// <param name="str"></param>
	/// <param name="encoding"></param>
	public void WriteStringBytes(string str, Encoding? encoding = null)
	{
		byte[] data = (encoding ?? Encoding.UTF8).GetBytes(str);
		this.BaseStream.Write(data);
	}
	/// <summary>
	/// Writes a string at current position. The length of the string will be written as a variable integer.
	/// </summary>
	/// <param name="str"></param>
	/// <param name="encoding"></param>
	public void WriteString(string str, Encoding? encoding = null)
	{
		byte[] data = (encoding ?? Encoding.UTF8).GetBytes(str);
		this.WriteVariedInteger((short)data.Length);

		this.BaseStream.Write(data);
	}

	public void WritePhigrosSerializableObject<T>(T obj) where T : IPhigrosCustomSerialization<T>
	{
		obj.Serialize(this);
	}
}
