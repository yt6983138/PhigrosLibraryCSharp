using System.Runtime.InteropServices;
using System.Text;

namespace PhigrosLibraryCSharp.Serialization;

//9D 01 16 47 6C 61 63 69 61 78 69 6F 6E 2E 53 75 6E 73 65 74 52 61 79 2E 30 1A 07 07 40 42 0F 00 00 00 C8 42 6B 39 0F 00 64 7F C7 42 40 42 0F 00 00 00 C8 42 0F 43 72 65 64 69 74 73 2E 46 72 75 6D 73 2E 30 12 0C 00 B7 13 0E 00 D3 F8 BE 42 67 D5 0D 00 76 12 C1 42 0F E5 85 89 2E E5 A7 9C E7
//Glaciaxion.SunsetRay.0@B���ÈBk9�dÇB@B���ÈBCredits.Frums.0�·�Óø¾Bg

// (9D 01)		| (16)				(47 ... 30)		 (1A)							(07)	(07)		(40 ... 42)														|
// header		| id string length	id string		 record Offset (need to +1)		is fc	exists		record start (structure: (int score then float acc) -> repeat)	| repeat (in pipe)
// read header	| read string bytes					|read record																									| read string... 

/// <summary>
/// A reader for Phigros save files.
/// </summary>
public class ByteReader // fuck my brain is going to explode if i keep working on this shit
{
	/// <summary>
	/// Raw decrypted data of the file.
	/// </summary>
	public byte[] Data { get; set; }
	/// <summary>
	/// Current reading offset.
	/// </summary>
	public int Offset { get; private set; }

	/// <summary>
	/// The byte at current offset.
	/// </summary>
	public byte Current => this.Data[this.Offset];

	/// <summary>
	/// The version read from save files, at the very first byte of <i>encrypted</i> file.
	/// </summary>
	public byte ObjectVersion { get; set; }

	/// <summary>
	/// Indicates whether the reader has more data to read or not.
	/// </summary>
	/// <returns><see langword="true"/> if there is more data, otherwise <see langword="false"/>.</returns>
	public bool HasMore => this.Offset != this.Data.Length;

	/// <summary>
	/// Construct the reader with raw data and starting offset.
	/// </summary>
	/// <param name="data">Raw data.</param>
	/// <param name="offset">Starting offset.</param>
	/// <param name="version">Object version, see <see cref="ObjectVersion"/>.</param>
	public ByteReader(byte[] data, int offset = 0, byte version = 0)
	{
		this.ObjectVersion = version;
		this.Offset = offset;
		this.Data = data;
	}

	/// <summary>
	/// Reads packed <see langword="bool"/>s from a <see langword="int"/>.
	/// </summary>
	/// <param name="num">The packed payload to read from.</param>
	/// <param name="index">Index of the <see cref="bool"/> you want to read.</param>
	/// <returns>The <see cref="bool"/> extracted from the packed payload.</returns>
	public static bool ReadBool(int num, int index)
	{
		return (num & (1 << index)) != 0;
	}

	/// <summary>
	/// Reads the data as unmanaged struct. 
	/// With structs that are managed but marshalable, use <see cref="ReadMarshalable{T}"/>
	/// </summary>
	/// <typeparam name="T">Unmanaged struct.</typeparam>
	/// <returns>The struct directly converted from data.</returns>
	public unsafe T ReadUnmanaged<T>() where T : unmanaged
	{
		int offset = this.Offset;
		if (offset + sizeof(T) > this.Data.Length)
			throw new IndexOutOfRangeException("The size of T is too large to be read.");

		fixed (byte* bytePtr = this.Data)
		{
			this.Jump(sizeof(T));
			return *(T*)(bytePtr + offset);
		}
	}
	/// <summary>
	/// Reads the data as marshalable struct.
	/// </summary>
	/// <typeparam name="T">Marshalable struct.</typeparam>
	/// <returns>The struct converted using marshal methods.</returns>
	public unsafe T ReadMarshalable<T>() where T : struct
	{
		int size = Marshal.SizeOf<T>();

		byte[] data = this.Data[this.Offset..(this.Offset + size)];
		fixed (byte* bytePtr = data)
		{
			this.Jump(size);
			return Marshal.PtrToStructure<T>((IntPtr)bytePtr)!;
		}
	}

	/// <summary>
	/// Reads a <see langword="bool"/> from a packed <see langword="byte"/> at current offset, without jumping.
	/// This is used to read packed bools inline so you don't need to cache the byte and use static methods (break api pattern).
	/// </summary>
	/// <param name="offset">The offset to read <see langword="bool"/> at.</param>
	/// <returns>The <see langword="bool"/> extracted from the packed byte.</returns>
	public bool ReadFromPackedBoolNoJump(int offset)
		=> ReadBool(this.Data[this.Offset], offset);
	/// <summary>
	/// Reads a <see langword="bool"/> from a packed <see langword="byte"/> at current offset, then jump to the next byte.
	/// This is used to read packed bools inline so you don't need to cache the byte and use static methods (break api pattern).
	/// </summary>
	/// <param name="offset">The offset to read <see langword="bool"/> at.</param>
	/// <returns>The <see langword="bool"/> extracted from the packed byte.</returns>
	public bool ReadFromPackedBoolThenJump(int offset)
	{
		bool result = this.ReadFromPackedBoolNoJump(offset);
		this.Jump(1);
		return result;
	}

	/// <summary>
	/// Reads a <see langword="byte"/> at current offset, and jump.
	/// </summary>
	/// <returns>The byte at current position.</returns>
	public byte ReadByte()
		=> this.ReadUnmanaged<byte>();
	/// <summary>
	/// Reads a <see langword="sbyte"/> at current offset, and jump.
	/// </summary>
	/// <returns>The sbyte at current position.</returns>
	public sbyte ReadSignedByte()
		=> this.ReadUnmanaged<sbyte>();
	/// <summary>
	/// Reads a <see langword="short"/> at current offset, and jump.
	/// </summary>
	/// <returns>The short at current position.</returns>
	public short ReadShort()
		=> this.ReadUnmanaged<short>();
	/// <summary>
	/// Reads a <see langword="ushort"/> at current offset, and jump.
	/// </summary>
	/// <returns>The ushort at current position.</returns>
	public ushort ReadUnsignedShort()
		=> this.ReadUnmanaged<ushort>();
	/// <summary>
	/// Reads a <see langword="int"/> at current offset, and jump.
	/// </summary>
	/// <returns>The int at current position.</returns>
	public int ReadInt()
		=> this.ReadUnmanaged<int>();
	/// <summary>
	/// Reads a <see langword="uint"/> at current offset, and jump.
	/// </summary>
	/// <returns>The uint at current position.</returns>
	public uint ReadUnsignedInt()
		=> this.ReadUnmanaged<uint>();
	/// <summary>
	/// Reads a <see langword="long"/> at current offset, and jump.
	/// </summary>
	/// <returns>The long at current position.</returns>
	public long ReadLong()
		=> this.ReadUnmanaged<long>();
	/// <summary>
	/// Reads a <see langword="ulong"/> at current offset, and jump.
	/// </summary>
	/// <returns>The ulong at current position.</returns>
	public ulong ReadUnsignedLong()
		=> this.ReadUnmanaged<ulong>();
	/// <summary>
	/// Reads a <see langword="float"/> at current offset, and jump.
	/// </summary>
	/// <returns>The float at current position.</returns>
	public float ReadFloat()
		=> this.ReadUnmanaged<float>();
	/// <summary>
	/// Reads a <see langword="double"/> at current offset, and jump.
	/// </summary>
	/// <returns>The float at current position.</returns>
	public double ReadDouble()
		=> this.ReadUnmanaged<double>();
	/// <summary>
	/// Reads a variable length integer at current offset, and jump. 
	/// Essentially a LEB128 encoded integer, however only 2 bytes are decoded.
	/// (I have no idea why everyone just call it VarInt and not use the proper encoding name)
	/// </summary>
	/// <returns>The LEB128 integer at current position.</returns>
	public short ReadVariedInteger()
	{
		if (this.Data[this.Offset] > 127)
		{
			if (this.Offset + 2 >= this.Data.Length)
				throw new IndexOutOfRangeException("Varied integer requires another byte, but has already reached EOF.");

			this.Offset += 2;
			return (short)((0b01111111 & this.Data[this.Offset - 2])
				^ (this.Data[this.Offset - 1] << 7));
		}
		else
		{
			return this.ReadByte();
		}
	}
	/// <summary>
	/// Read a byte array with given length at current offset, and jump.
	/// </summary>
	/// <param name="length">Length of the bytes to read.</param>
	/// <returns><see langword="byte[]"/> read from the current position.</returns>
	public byte[] ReadBytes(int length)
	{
		byte[] data = this.Data[this.Offset..(this.Offset + length)];
		this.Offset += length;
		return data;
	}
	/// <summary>
	/// Reads the string at current offset, and jump. Length is read as a varied integer before the string.
	/// </summary>
	/// <returns>Raw string bytes read at current position.</returns>
	public byte[] ReadStringBytes()
	{
		short length = this.ReadVariedInteger();
		byte[] data = this.ReadBytes(length);
		return data;
	}
	/// <summary>
	/// Read the string at current offset, and jump. The string is decoded with the given encoding, if not supplied UTF8 is used.
	/// </summary>
	/// <param name="encoding">The encoding used to read string, if not supplied UTF8 is used.</param>
	/// <returns>Decoded string read at current position.</returns>
	public string ReadString(Encoding? encoding = null)
		=> (encoding ?? Encoding.UTF8).GetString(this.ReadStringBytes());
	/// <summary>
	/// Reads a string of a specified length from the current offset and advances the offset.
	/// </summary>
	/// <param name="length">The number of bytes to read for the string.</param>
	/// <param name="encoding">The encoding used to decode the string. If not provided, UTF8 is used by default.</param>
	/// <returns>The decoded string from the specified length of bytes.</returns>
	public string ReadStringCustomLength(int length, Encoding? encoding = null)
	{
		byte[] data = this.ReadBytes(length);
		return (encoding ?? Encoding.UTF8).GetString(data);
	}
	/// <summary>
	/// Jump by offset.
	/// </summary>
	/// <param name="offset">The offset relative to current position you want to jump.</param>
	public void Jump(int offset)
	{
		this.Offset += offset;
	}
	/// <summary>
	/// Jump to offset.
	/// </summary>
	/// <param name="offset">The offset you want to jump to.</param>
	public void JumpTo(int offset)
	{
		this.Offset = offset;
	}

	/// <summary>
	/// Read a custom serialized object that implements <see cref="IPhigrosCustomSerialization{T}"/>. 
	/// This is basically same as calling <see cref="IPhigrosCustomSerialization{T}.FromReader"/> directly, 
	/// but it fits the api pattern of this class and is more convenient to use.
	/// </summary>
	/// <typeparam name="T">The type of Phigros serializable object to read.</typeparam>
	/// <returns>The deserialized Phigros object.</returns>
	public T ReadPhigrosSerializedObject<T>() where T : IPhigrosCustomSerialization<T>
	{
		return T.FromReader(this);
	}
}
