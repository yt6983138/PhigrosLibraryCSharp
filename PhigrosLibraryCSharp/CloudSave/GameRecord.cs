using PhigrosLibraryCSharp.Serialization;

namespace PhigrosLibraryCSharp.CloudSave;

/// <summary>
/// A container containing save records.
/// </summary>
public class GameRecord : IPhigrosCustomSerialization<GameRecord>
{
	private record struct RawRecord(int Score, float Acc, bool IsFc, int Difficulty);

	/// <summary>
	/// Version of the gameRecord file. Latest: 1.
	/// </summary>
	public byte Version { get; set; }
	/// <summary>
	/// The player song records.
	/// </summary>
	public List<SongScore> Records { get; set; }

	public GameRecord(List<SongScore> records, byte version)
	{
		this.Records = records;
		this.Version = version;
	}

	public IEnumerable<CompleteScore> GetCompleteScores(IReadOnlyDictionary<ChartConstantKey, float> constantMap, IReadOnlyDictionary<string, string> nameMap)
	{
		return this.Records.Select(x => new CompleteScore(x, constantMap, nameMap));
	}
	/// <summary>
	/// Sorts the records and returns the phis and the RKS.
	/// Note: this is for game version > 3.11.0
	/// </summary>
	/// <returns>A tuple containing the sorted list of scores and the RKS.</returns>
	public (List<CompleteScore> Phis, List<CompleteScore> OtherScores, double Rks) GetSortedListForRks(IReadOnlyDictionary<ChartConstantKey, float> constantMap, IReadOnlyDictionary<string, string> nameMap)
	{
		List<CompleteScore> sorted = this.GetCompleteScores(constantMap, nameMap).ToList();
		sorted.Sort();

		List<CompleteScore> phi3 = sorted
			.Where(x => x.Score.Status == ScoreStatus.Phi)
			.Take(3)
			.ToList();

		double rks = phi3.Sum(x => x.Rks / 30d);
		rks += sorted.Take(27).Sum(x => x.Rks / 30d);

		return (phi3, sorted, rks);
	}
	/// <summary>
	/// Sorts the records, merges the phi scores, and returns the sorted list and the RKS.
	/// Note: this is for game version > 3.11.0
	/// </summary>
	/// <returns>A tuple containing the sorted list of scores and the RKS.</returns>
	public (List<CompleteScore> Scores, double Rks) GetSortedListForRksMerged(IReadOnlyDictionary<ChartConstantKey, float> constantMap, IReadOnlyDictionary<string, string> nameMap)
	{
		(List<CompleteScore>? phis, List<CompleteScore>? scores, double rks) = this.GetSortedListForRks(constantMap, nameMap);
		scores.InsertRange(0, phis);
		return (scores, rks);
	}

	private static List<RawRecord> ReadEntry(ByteReader reader)
	{
		// The record structure is as follows:
		// byte: record length in bytes (excluding this byte)
		// byte: difficulty exist flag (bit 0-3 for difficulty 0-3)
		// byte: full combo flag (bit 0-3 for difficulty 0-3)
		// for each difficulty:
		//   int: score
		//   float: accuracy

		List<RawRecord> scores = [];
		byte recordLength = reader.ReadByte();
		int endOffset = recordLength + reader.Offset;

		byte difficultyExistFlag = reader.ReadByte();
		byte fullComboFlag = reader.ReadByte();

		for (byte i = 0; i < 4; i++)
		{
			if (reader.Offset == endOffset) break;
			if (!ByteReader.ReadBool(difficultyExistFlag, i) || reader.Offset + 8 > reader.Data.Length)
				continue;

			int score = reader.ReadInt();
			float acc = reader.ReadFloat();

			if (acc > 100 || acc < 0)
				throw new InvalidDataException($"Invalid accuracy value {acc}, score {score} found");

			scores.Add(new(score, acc, ByteReader.ReadBool(fullComboFlag, i), i));
		}
		reader.JumpTo(endOffset);
		return scores;
	}
	public static GameRecord FromReader(ByteReader reader)
	{
		List<SongScore> scores = [];

		short scoreCount = reader.ReadVariedInteger();
		for (int i = 0; i < scoreCount; i++)
		{
			string id = reader.ReadString();

			foreach (RawRecord item in ReadEntry(reader))
			{
				scores.Add(new(item.Score, item.Acc, id, item.IsFc, (Difficulty)item.Difficulty));
			}
		}

		return new(scores, reader.ObjectVersion);
	}
	public void Serialize(ByteWriter writer)
	{
		writer.WriteVariedInteger((short)this.Records.Count);
		foreach (IGrouping<string, SongScore> group in this.Records.GroupBy(x => x.Id))
		{
			writer.WriteString(group.Key);
			writer.WriteByte((byte)((sizeof(byte) * 2) + ((sizeof(int) + sizeof(float)) * group.Count())));

			byte difficultyExistFlag = 0;
			byte fullComboFlag = 0;
			foreach (SongScore score in group)
			{
				difficultyExistFlag |= (byte)(1 << (int)score.Difficulty);
				if (score._isFc)
					fullComboFlag |= (byte)(1 << (int)score.Difficulty);
			}
			writer.WriteByte(difficultyExistFlag);
			writer.WriteByte(fullComboFlag);
			foreach (SongScore score in group)
			{
				writer.WriteInt(score.Score);
				writer.WriteFloat(score.Accuracy);
			}
		}
	}
}