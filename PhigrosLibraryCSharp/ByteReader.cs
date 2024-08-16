using PhigrosLibraryCSharp.Extensions;
using System.Runtime.InteropServices;

namespace PhigrosLibraryCSharp;

//9D 01 16 47 6C 61 63 69 61 78 69 6F 6E 2E 53 75 6E 73 65 74 52 61 79 2E 30 1A 07 07 40 42 0F 00 00 00 C8 42 6B 39 0F 00 64 7F C7 42 40 42 0F 00 00 00 C8 42 0F 43 72 65 64 69 74 73 2E 46 72 75 6D 73 2E 30 12 0C 00 B7 13 0E 00 D3 F8 BE 42 67 D5 0D 00 76 12 C1 42 0F E5 85 89 2E E5 A7 9C E7
//Glaciaxion.SunsetRay.0@B���ÈBk9�dÇB@B���ÈBCredits.Frums.0�·�Óø¾Bg

// (9D 01)		| (16)				(47 ... 30)		 (1A)							(07)	(07)		(40 ... 42)														|
// header		| id string length	id string		 record Offset (need to +1)		is fc	unknown		record start (structure: (int score then float acc) -> repeat)	| repeat (in pipe)
// read header	| read string bytes					|read record																								| read string... 

/// <summary>
/// A class can be used to read gameRecord file.
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
	/// Construct the reader with raw data and starting offset.
	/// </summary>
	/// <param name="data">Raw data.</param>
	/// <param name="offset">Starting offset.</param>
	public ByteReader(byte[] data, int offset = 0)
	{
		this.Offset = offset;
		this.Data = data;
	}

	/// <summary>
	/// Phigros decided to put 4 or more <see cref="bool"/>s inside a <see cref="byte"/>, use this to get the bool from index.
	/// </summary>
	/// <param name="num">The data.</param>
	/// <param name="index">Index of the <see cref="bool"/> you want to read.</param>
	/// <returns>The <see cref="bool"/> gotten from num.</returns>
	public static bool ReadBool(int num, int index)
	{
		return (num & (1 << index)) != 0;
	}

	/// <summary>
	/// Reads the data as unmanaged struct. 
	/// With structs that are managed but marshalable, use <see cref="ReadMarshalable{T}"/>
	/// </summary>
	/// <typeparam name="T">Unmanaged struct</typeparam>
	/// <returns>The struct directly converted from data</returns>
	public unsafe T ReadUnmanaged<T>() where T : unmanaged
	{
		T dat = default;

		byte* ptr = (byte*)&dat;
		for (int i = 0; i < sizeof(T); i++)
		{
			*ptr = this.Data[this.Offset++];
			ptr++;
		}
		return dat;
	}
	/// <summary>
	/// Reads the data as marshalable struct.
	/// </summary>
	/// <typeparam name="T">Marshalable struct.</typeparam>
	/// <returns>The struct directly converted from data</returns>
	public T ReadMarshalable<T>() where T : struct
	{
		int size = Marshal.SizeOf<T>();
		this.Jump(size);
		return SerialHelper.ByteToStruct<T>(this.Data[(this.Offset - size)..this.Offset]);
	}
	/// <summary>
	/// Reads byte at current offset, and jump.
	/// </summary>
	/// <returns>The byte at current position.</returns>
	public byte ReadByte()
		=> this.ReadUnmanaged<byte>();
	/// <summary>
	/// Reads sbyte at current offset, and jump.
	/// </summary>
	/// <returns>The sbyte at current position.</returns>
	public sbyte ReadSignedByte()
		=> this.ReadUnmanaged<sbyte>();
	/// <summary>
	/// Reads short at current offset, and jump.
	/// </summary>
	/// <returns>The short at current position.</returns>
	public short ReadShort()
		=> this.ReadUnmanaged<short>();
	/// <summary>
	/// Reads ushort at current offset, and jump.
	/// </summary>
	/// <returns>The ushort at current position.</returns>
	public ushort ReadUnsignedShort()
		=> this.ReadUnmanaged<ushort>();
	/// <summary>
	/// Reads a int at current offset, and jump.
	/// </summary>
	/// <returns>The int at current position.</returns>
	public int ReadInt()
		=> this.ReadUnmanaged<int>();
	/// <summary>
	/// Reads a uint at current offset, and jump.
	/// </summary>
	/// <returns>The uint at current position.</returns>
	public uint ReadUnsignedInt()
		=> this.ReadUnmanaged<uint>();
	/// <summary>
	/// Reads a long at current offset, and jump.
	/// </summary>
	/// <returns>The long at current position.</returns>
	public long ReadLong()
		=> this.ReadUnmanaged<long>();
	/// <summary>
	/// Reads a ulong at current offset, and jump.
	/// </summary>
	/// <returns>The ulong at current position.</returns>
	public ulong ReadUnsignedLong()
		=> this.ReadUnmanaged<ulong>();
	/// <summary>
	/// Reads a float at current offset, and jump.
	/// </summary>
	/// <returns>The float at current position.</returns>
	public float ReadFloat()
		=> this.ReadUnmanaged<float>();
	/// <summary>
	/// Reads a double at current offset, and jump.
	/// </summary>
	/// <returns>The float at current position.</returns>
	public double ReadDouble()
		=> this.ReadUnmanaged<double>();
	/// <summary>
	/// Reads short/byte at current offset, and jump.
	/// </summary>
	/// <returns>The short/byte at current position.</returns>
	public short ReadVariedInteger()
	{
		if ((this.Data[this.Offset.Print("varied int at offset {0}")] > 127).Print("is larger than 127: {0}"))
		{
			this.Offset += 2;
			return (short)((0b01111111 & this.Data[this.Offset - 2].Print("first: {0}"))
				^ (this.Data[this.Offset - 1].Print("second: {0}") << 7));
		}
		else return this.Data[this.Offset++].Print("single: {0}");
	}
	/// <summary>
	/// Reads the string at current offset, and jump.
	/// </summary>
	/// <returns>The read decoded string.</returns>
	public byte[] ReadStringBytes()
	{
		short length = this.ReadVariedInteger().Print("ReadStringBytes length {0}, data:");
		byte[] data = this.Data[this.Offset..(this.Offset + length)].PrintHex();
		this.Offset += length.Print("added offset {0}");
		return data.PrintAsUTF8();
	}
	/// <summary>
	/// Jump with offset.
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
}
