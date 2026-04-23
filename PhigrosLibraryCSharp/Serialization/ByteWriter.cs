using System.Runtime.InteropServices;
using System.Text;

namespace PhigrosLibraryCSharp.Serialization;

/// <summary>
/// A writer for Phigros save files.
/// </summary>
public class ByteWriter
{
	/// <summary>
	/// Gets or sets the underlying stream being written to.
	/// </summary>
	public Stream BaseStream { get; set; }

	/// <summary>
	/// Gets or sets the version of the object being serialized.
	/// </summary>
	public byte ObjectVersion { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="ByteWriter"/> class.
	/// </summary>
	/// <param name="stream">The stream to write to.</param>
	/// <param name="version">The version of the object structure.</param>
	public ByteWriter(Stream stream, byte version = 0)
	{
		this.BaseStream = stream;
		this.ObjectVersion = version;
	}

	/// <summary>
	/// Writes the data as unmanaged struct.
	/// With structs that are managed but marshalable, use <see cref="WriteMarshalable{T}"/>.
	/// </summary>
	/// <typeparam name="T">Unmanaged struct type.</typeparam>
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
	/// <typeparam name="T">Marshalable struct type.</typeparam>
	/// <param name="data">The struct to write.</param>
	public unsafe void WriteMarshalable<T>(T data) where T : struct
	{
		int size = Marshal.SizeOf<T>();
		byte* buffer = stackalloc byte[size];

		Marshal.StructureToPtr(data, (IntPtr)buffer, false);
		this.BaseStream.Write(new ReadOnlySpan<byte>(buffer, size));
	}

	/// <summary>
	/// Writes a boolean value as a single byte.
	/// </summary>
	/// <param name="data">The boolean value to write.</param>
	public void WriteBool(bool data)
		=> this.WriteByte((byte)(data ? 1 : 0));

	/// <summary>
	/// Writes a byte at the current position.
	/// </summary>
	/// <param name="data">The byte to write.</param>
	public void WriteByte(byte data)
		=> this.WriteUnmanaged(data);

	/// <summary>
	/// Writes a signed byte at the current position.
	/// </summary>
	/// <param name="data">The sbyte to write.</param>
	public void WriteSignedByte(sbyte data)
		=> this.WriteUnmanaged(data);

	/// <summary>
	/// Writes a 16-bit signed integer at the current position.
	/// </summary>
	/// <param name="data">The short to write.</param>
	public void WriteShort(short data)
		=> this.WriteUnmanaged(data);

	/// <summary>
	/// Writes a 16-bit unsigned integer at the current position.
	/// </summary>
	/// <param name="data">The ushort to write.</param>
	public void WriteUnsignedShort(ushort data)
		=> this.WriteUnmanaged(data);

	/// <summary>
	/// Writes a 32-bit signed integer at the current position.
	/// </summary>
	/// <param name="data">The int to write.</param>
	public void WriteInt(int data)
		=> this.WriteUnmanaged(data);

	/// <summary>
	/// Writes a 32-bit unsigned integer at the current position.
	/// </summary>
	/// <param name="data">The uint to write.</param>
	public void WriteUnsignedInt(uint data)
		=> this.WriteUnmanaged(data);

	/// <summary>
	/// Writes a 64-bit signed integer at the current position.
	/// </summary>
	/// <param name="data">The long to write.</param>
	public void WriteLong(long data)
		=> this.WriteUnmanaged(data);

	/// <summary>
	/// Writes a 64-bit unsigned integer at the current position.
	/// </summary>
	/// <param name="data">The ulong to write.</param>
	public void WriteUnsignedLong(ulong data)
		=> this.WriteUnmanaged(data);

	/// <summary>
	/// Writes a 32-bit floating-point value at the current position.
	/// </summary>
	/// <param name="data">The float to write.</param>
	public void WriteFloat(float data)
		=> this.WriteUnmanaged(data);

	/// <summary>
	/// Writes a 64-bit floating-point value at the current position.
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
	/// Writes a variable-length integer at the current position.
	/// Essentially a LEB128 encoded integer, however only 2 bytes are encoded.
	/// </summary>
	/// <param name="data">The integer to write.</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if data exceeds the 14-bit storage capacity.</exception>
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
	/// Writes a string's raw bytes at the current position. 
	/// Note: this method does not write the length of the string; it must be written manually if needed.
	/// </summary>
	/// <param name="str">The string to write.</param>
	/// <param name="encoding">The encoding to use. Defaults to UTF-8.</param>
	public void WriteStringBytes(string str, Encoding? encoding = null)
	{
		byte[] data = (encoding ?? Encoding.UTF8).GetBytes(str);
		this.BaseStream.Write(data);
	}

	/// <summary>
	/// Writes a string at the current position. The length of the string is written 
	/// using <see cref="WriteVariedInteger(short)"/> before the content.
	/// </summary>
	/// <param name="str">The string to write.</param>
	/// <param name="encoding">The encoding to use. Defaults to UTF-8.</param>
	public void WriteString(string str, Encoding? encoding = null)
	{
		byte[] data = (encoding ?? Encoding.UTF8).GetBytes(str);
		this.WriteVariedInteger((short)data.Length);

		this.BaseStream.Write(data);
	}

	/// <summary>
	/// Writes a byte array to the stream.
	/// </summary>
	/// <param name="data">The byte array to write.</param>
	public void WriteBytes(byte[] data)
		=> this.BaseStream.Write(data);

	/// <summary>
	/// Writes a read-only span of bytes to the stream.
	/// </summary>
	/// <param name="data">The byte span to write.</param>
	public void WriteBytes(ReadOnlySpan<byte> data)
		=> this.BaseStream.Write(data);

	/// <summary>
	/// Serializes a custom Phigros object using its own serialization logic.
	/// This is basically same as calling <see cref="IPhigrosCustomSerialization{T}.FromReader"/> directly, 
	/// but it fits the api pattern of this class and is more convenient to use.
	/// </summary>
	/// <typeparam name="T">The type of the object implementing <see cref="IPhigrosCustomSerialization{T}"/>.</typeparam>
	/// <param name="obj">The object to serialize.</param>
	public void WritePhigrosSerializableObject<T>(T obj) where T : IPhigrosCustomSerialization<T>
	{
		obj.Serialize(this);
	}
}
