using PhigrosLibraryCSharp.Cloud.DataStructure;
using PhigrosLibraryCSharp.GameRecords;
using PhigrosLibraryCSharp.GameRecords.Raw;
using System.Text;

namespace PhigrosLibraryCSharp.Extensions;

/// <summary>
/// Extensions providing functionality that can be used to read Phigros records.
/// </summary>
public static class ByteReaderExtensions
{
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
	/// Read all the records.
	/// </summary>
	/// <param name="reader">The reader itself.</param>
	/// <param name="difficulties">The difficulties table of the charts.</param>
	/// <returns>A list of <see cref="CompleteScore"/> containing the user's records.</returns>
	public static List<CompleteScore> ReadAllGameRecord(this ByteReader reader, in IReadOnlyDictionary<string, float[]> difficulties)
	{
		#region old shit
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
		//bool success = false;
		//for (int i = 0; i < 16; i++)
		//{
		//	byte[] datas = reader.Data[(reader.Offset + 1)..(reader.Offset + reader.Data[reader.Offset] + 1)];
		//	if (Encoding.ASCII.GetString(datas).All(x => !char.IsControl(x)))
		//	{
		//		success = true;
		//		break;
		//	}
		//	reader.Jump(1);
		//}
		//if (!success) // fall back manual detection
		//{
		//	reader.JumpTo(0);
		//	int headerLength = reader.Data[0] switch
		//	{
		//		0x9D => 2, // i have no idea what those are
		//		0x7E => 1,
		//		0x2B => 24,
		//		0x66 => 1,
		//		_ => 2
		//	};
		//	reader.Jump(headerLength);
		//}
		#endregion

		short scoreCount = reader.ReadVariedInteger();

		List<CompleteScore> scores = new();
		for (int i = 0; i < scoreCount; i++)
		{
			string id = Encoding.UTF8.GetString(reader.ReadStringBytes())[..^2];
			reader.Jump(3);

			foreach (MoreInfoPartialGameRecord item in reader.ReadRecord())
			{
				if (id.StartsWith("Introduc")) // shits in old version detection
					break;
				scores.Add(new CompleteScore(item, id, difficulties[id][item.LevelType], IntLevelToStringLevel));
			}
		}
		return scores;
	}
	/// <summary>
	/// Read the current data as <see cref="GameUserInfo"/>.
	/// </summary>
	/// <param name="reader">The reader itself.</param>
	/// <returns>User's settings.</returns>
	public static GameUserInfo ReadGameUserInfo(this ByteReader reader)
	{
		reader.Jump(1);
		return new(
			ByteReader.ReadBool(reader.Data[0], 0),
			Encoding.UTF8.GetString(reader.ReadStringBytes()),
			Encoding.UTF8.GetString(reader.ReadStringBytes()),
			Encoding.UTF8.GetString(reader.ReadStringBytes())
			);
	}
	/// <summary>
	/// Read current data as <see cref="GameSettings"/>.
	/// </summary>
	/// <param name="reader">The reader itself.</param>
	/// <returns>User's settings.</returns>
	public static GameSettings ReadGameSettings(this ByteReader reader)
	{
		reader.Jump(1);
		return new(
			ByteReader.ReadBool(reader.Data[0], 0),
			ByteReader.ReadBool(reader.Data[0], 1),
			ByteReader.ReadBool(reader.Data[0], 2),
			ByteReader.ReadBool(reader.Data[0], 3),
			Encoding.UTF8.GetString(reader.ReadStringBytes()),
			reader.ReadFloat(),
			reader.ReadFloat(),
			reader.ReadFloat(),
			reader.ReadFloat(),
			reader.ReadFloat(),
			reader.ReadFloat()
			);
	}
	/// <summary>
	/// Get user's game progress.
	/// </summary>
	/// <param name="reader">The reader itself.</param>
	/// <returns>User's game progress.</returns>
	public static GameProgress ReadGameProgress(this ByteReader reader)
	{
		reader.Jump(1);
		return new(
			ByteReader.ReadBool(reader.Data[0], 0),
			ByteReader.ReadBool(reader.Data[0], 1),
			ByteReader.ReadBool(reader.Data[0], 2),
			ByteReader.ReadBool(reader.Data[0], 3),
			Encoding.UTF8.GetString(reader.ReadStringBytes()),
			reader.ReadVariedInteger(),
			reader.ReadShort(),
			new(reader.ReadVariedInteger(), reader.ReadVariedInteger(), reader.ReadVariedInteger(), reader.ReadVariedInteger(), reader.ReadVariedInteger()),
			reader.ReadByte(),
			reader.ReadByte(),
			reader.ReadByte(),
			reader.ReadByte(),
			reader.ReadByte(),
			ByteReader.ReadBool(reader.Data[^2], 0),
			ByteReader.ReadBool(reader.Data[^2], 1),
			ByteReader.ReadBool(reader.Data[^2], 2),
			reader.Data[^1]);
	}
	internal static List<MoreInfoPartialGameRecord> ReadRecord(this ByteReader reader)
	{
		List<MoreInfoPartialGameRecord> scores = new();
		int readLen = reader.Data[reader.Offset - 3] - 2;
		int endOffset = readLen + reader.Offset;
		byte exists = reader.Data[reader.Offset - 2];
		byte fc = reader.Data[reader.Offset - 1];

		// Console.WriteLine((endOffset - Offset).ToString("X4"));
		// Console.WriteLine(Offset.ToString("X4"));
		// Console.WriteLine(endOffset.ToString("X4"));
		for (byte i = 0; i < 4; i++)
		{
			// Console.WriteLine(BitConverter.ToString(Data[Offset..(Offset + 8)]));
			if (reader.Offset == endOffset) break;
			if (!ByteReader.ReadBool(exists, i) || reader.Offset + 8 > reader.Data.Length)
			{
				// Offset += 8;
				continue;
			}
			PartialGameRecord record = SerialHelper.ByteToStruct<PartialGameRecord>(reader.Data[reader.Offset..(reader.Offset + 8)]);
			if (record.Acc > 100 || record.Acc < 0) Console.WriteLine(BitConverter.ToString(reader.Data[reader.Offset..(reader.Offset + 8)]).Replace('-', ' '));
			scores.Add(
				new MoreInfoPartialGameRecord(
					record,
					ByteReader.ReadBool(fc, i),
					i
				)
			);
			reader.Jump(8);
		}
		reader.JumpTo(endOffset);
		// Offset++;
		return scores;
	}
}
