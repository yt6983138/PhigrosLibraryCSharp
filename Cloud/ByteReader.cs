using PhigrosLibraryCSharp.Cloud.DataStructure;
using System.Runtime.InteropServices;
using System.Text;

namespace PhigrosLibraryCSharp;

//9D 01 16 47 6C 61 63 69 61 78 69 6F 6E 2E 53 75 6E 73 65 74 52 61 79 2E 30 1A 07 07 40 42 0F 00 00 00 C8 42 6B 39 0F 00 64 7F C7 42 40 42 0F 00 00 00 C8 42 0F 43 72 65 64 69 74 73 2E 46 72 75 6D 73 2E 30 12 0C 00 B7 13 0E 00 D3 F8 BE 42 67 D5 0D 00 76 12 C1 42 0F E5 85 89 2E E5 A7 9C E7
//Glaciaxion.SunsetRay.0@B���ÈBk9�dÇB@B���ÈBCredits.Frums.0�·�Óø¾Bg

// (9D 01)		| (16)				(47 ... 30)		 (1A)							(07)	(07)		(40 ... 42)														|
// header		| id string length	id string		 record Offset (need to +1)		is fc	unknown		record start (structure: (int score then float acc) -> repeat)	| repeat (in pipe)
// read header	| read string bytes					|read record																								| read string... 

[StructLayout(LayoutKind.Sequential)]
internal struct PartialGameRecord
{
	//[FieldOffset(0)]
	internal int Score;
	//[FieldOffset(4)]
	internal float Acc;
}
internal struct MoreInfoPartialGameRecord
{
	internal int Score;
	internal float Acc;
	internal bool IsFc;
	internal int LevelType;

	internal MoreInfoPartialGameRecord(PartialGameRecord data, bool isfc, int levelType)
	{
		this.Score = data.Score;
		this.Acc = data.Acc;
		this.IsFc = isfc;
		this.LevelType = levelType;
	}
}

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
	/// A map converting index to readable difficulty.
	/// </summary>
	public static IReadOnlyDictionary<int, string> IntLevelToStringLevel { get; } = new Dictionary<int, string>()
	{
		{ 0, "EZ" },
		{ 1, "HD" },
		{ 2, "IN" },
		{ 3, "AT" }
	};
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
	/// Reads float at current offset, and jump.
	/// </summary>
	/// <returns>The float at current position.</returns>
	public float ReadFloat()
	{
		this.Jump(4);
		return SerialHelper.ByteToStruct<float>(this.Data[(this.Offset - 4)..this.Offset]);
	}
	/// <summary>
	/// Reads byte at current offset, and jump.
	/// </summary>
	/// <returns>The byte at current position.</returns>
	public byte ReadByte()
	{
		this.Jump(1);
		return this.Data[this.Offset - 1];
	}
	/// <summary>
	/// Reads short/byte at current offset, and jump.
	/// </summary>
	/// <returns>The short/byte at current position.</returns>
	public short ReadVariedInt()
	{
		if ((this.Data[this.Offset.Print("varied int at offset {0}")] > 127).Print("is larger than 127: {0}"))
		{
			this.Offset += 2;
			return (short)((0b01111111 & this.Data[this.Offset - 2].Print("first: {0}")) ^ (this.Data[this.Offset - 1].Print("second: {0}") << 7));
		}
		else return this.Data[this.Offset++].Print("single: {0}");
	}
	/// <summary>
	/// Reads short at current offset, and jump.
	/// </summary>
	/// <returns>The short at current position.</returns>
	public short ReadShort()
	{
		this.Jump(2);
		return SerialHelper.ByteToStruct<short>(this.Data[(this.Offset - 2)..this.Offset]);
	}
	/// <summary>
	/// Reads the header.
	/// </summary>
	/// <param name="size">The size of the header.</param>
	public void ReadHeader(int size)
	{
		this.Offset += size;
	}
	/// <summary>
	/// Reads the string at current offset, and jump.
	/// </summary>
	/// <returns>The read decoded string.</returns>
	public byte[] ReadStringBytes()
	{
		short length = this.ReadVariedInt().Print("ReadStringBytes length {0}, data:");
		byte[] data = this.Data[this.Offset..(this.Offset + length)].PrintHex();
		this.Offset += length.Print("added offset {0}");
		return data.PrintAsUTF8();
	}
	internal List<MoreInfoPartialGameRecord> ReadRecord()
	{
		List<MoreInfoPartialGameRecord> scores = new();
		int readLen = this.Data[this.Offset - 3] - 2;
		int endOffset = readLen + this.Offset;
		byte exists = this.Data[this.Offset - 2];
		byte fc = this.Data[this.Offset - 1];

		// Console.WriteLine((endOffset - Offset).ToString("X4"));
		// Console.WriteLine(Offset.ToString("X4"));
		// Console.WriteLine(endOffset.ToString("X4"));
		for (byte i = 0; i < 4; i++)
		{
			// Console.WriteLine(BitConverter.ToString(Data[Offset..(Offset + 8)]));
			if (this.Offset == endOffset) break;
			if (!ReadBool(exists, i) || this.Offset + 8 > this.Data.Length)
			{
				// Offset += 8;
				continue;
			}
			PartialGameRecord record = SerialHelper.ByteToStruct<PartialGameRecord>(this.Data[this.Offset..(this.Offset + 8)]);
			if (record.Acc > 100 || record.Acc < 0) Console.WriteLine(BitConverter.ToString(this.Data[this.Offset..(this.Offset + 8)]).Replace('-', ' '));
			scores.Add(
				new MoreInfoPartialGameRecord(
					record,
					ReadBool(fc, i),
					i
				)
			);
			this.Offset += 8;
		}
		this.Offset = endOffset;
		// Offset++;
		return scores;
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
	/// Read all the records.
	/// </summary>
	/// <param name="difficulties">The difficulties table of the charts.</param>
	/// <returns>A list of <see cref="InternalScoreFormat"/> containing the user's records.</returns>
	public List<InternalScoreFormat> ReadAllGameRecord(in IReadOnlyDictionary<string, float[]> difficulties)
	{
		// auto detection
		// int glaciaxionLocation = first64.IndexOf("Glaciaxion");
		// int nonMelodicLocation = first64.IndexOf("NonMelodic");
		// if (glaciaxionLocation != -1)
		// {
		// 	headerLength = glaciaxionLocation - 1;
		// }
		// else if (nonMelodicLocation != -1) // old version compatibility
		// {
		// 	headerLength = nonMelodicLocation - 1;
		// }
		bool success = false;
		for (int i = 0; i < 16; i++)
		{
			byte[] datas = this.Data[(this.Offset + 1)..(this.Offset + this.Data[this.Offset] + 1)];
			if (Encoding.ASCII.GetString(datas).All(x => !char.IsControl(x)))
			{
				success = true;
				break;
			}
			this.Offset++;
		}
		if (!success) // fall back manual detection
		{
			this.Offset = 0;
			int headerLength = this.Data[0] switch
			{
				0x9D => 2, // i have no idea what those are
				0x7E => 1,
				0x2B => 24,
				0x66 => 1,
				_ => 2
			};
			this.ReadHeader(headerLength);
		}

		List<InternalScoreFormat> scores = new();
		while (this.Offset < this.Data.Length)
		{
			string id = Encoding.UTF8.GetString(this.ReadStringBytes())[..^2];
			this.Jump(3);

			foreach (MoreInfoPartialGameRecord item in this.ReadRecord())
			{
				scores.Add(new InternalScoreFormat(item, id, difficulties[id][item.LevelType], IntLevelToStringLevel));
			}
		}
		return scores;
	}
	/// <summary>
	/// Read the current data as <see cref="GameUserInfo"/>.
	/// </summary>
	/// <returns>User's settings.</returns>
	public GameUserInfo ReadGameUserInfo()
	{
		this.Jump(1);
		return new(
			ReadBool(this.Data[0], 0),
			Encoding.UTF8.GetString(this.ReadStringBytes()),
			Encoding.UTF8.GetString(this.ReadStringBytes()),
			Encoding.UTF8.GetString(this.ReadStringBytes())
			);
	}
	/// <summary>
	/// Read current data as <see cref="GameSettings"/>.
	/// </summary>
	/// <returns>User's settings.</returns>
	public GameSettings ReadGameSettings()
	{
		this.Jump(1);
		return new(
			ReadBool(this.Data[0], 0),
			ReadBool(this.Data[0], 1),
			ReadBool(this.Data[0], 2),
			ReadBool(this.Data[0], 3),
			Encoding.UTF8.GetString(this.ReadStringBytes()),
			this.ReadFloat(),
			this.ReadFloat(),
			this.ReadFloat(),
			this.ReadFloat(),
			this.ReadFloat(),
			this.ReadFloat()
			);
	}
	/// <summary>
	/// Get user's game progress.
	/// </summary>
	/// <returns>User's game progress.</returns>
	public GameProgress ReadGameProgress()
	{
		this.Jump(1);
		return new(
			ReadBool(this.Data[0], 0),
			ReadBool(this.Data[0], 1),
			ReadBool(this.Data[0], 2),
			ReadBool(this.Data[0], 3),
			Encoding.UTF8.GetString(this.ReadStringBytes()),
			this.ReadVariedInt(),
			this.ReadShort(),
			new(this.ReadVariedInt(), this.ReadVariedInt(), this.ReadVariedInt(), this.ReadVariedInt(), this.ReadVariedInt()),
			this.ReadByte(),
			this.ReadByte(),
			this.ReadByte(),
			this.ReadByte(),
			this.ReadByte(),
			ReadBool(this.Data[^2], 0),
			ReadBool(this.Data[^2], 1),
			ReadBool(this.Data[^2], 2),
			this.Data[^1]);
	}
}
