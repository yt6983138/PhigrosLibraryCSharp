using PhigrosLibraryCSharp.GameRecords;

namespace PhigrosLibraryCSharp.Cloud.DataStructure;

/// <summary>
/// A container containing save records.
/// </summary>
public class GameSave
{
	/// <summary>
	/// The player song records.
	/// </summary>
	public required List<CompleteScore> Records { get; set; }
	/// <summary>
	/// The creation date of the save.
	/// </summary>
	public required DateTime CreationDate { get; set; }
	/// <summary>
	/// The creation date of the save.
	/// </summary>
	public required DateTime ModificationTime { get; set; }
	/// <summary>
	/// [Unknown]
	/// </summary>
	public string Summary { get; set; } = ""; // unused, unknown

	/// <summary>
	/// Sorts the records and returns the phis and the RKS.
	/// Note: this is for game version > 3.11.0
	/// </summary>
	/// <returns>A tuple containing the sorted list of scores and the RKS.</returns>
	public (List<CompleteScore> Phis, List<CompleteScore> OtherScores, double Rks) GetSortedListForRks()
	{
		List<CompleteScore> sorted = new(this.Records);
		sorted.Sort();

		List<CompleteScore> phi3 = sorted
			.Where(x => x.Status == ScoreStatus.Phi)
			.Take(3)
			.ToList();

		while (phi3.Count < 3)
		{
			phi3.Add(CompleteScore.Empty);
		}

		double rks = phi3.Sum(x => x.Rks / 30d);
		rks += sorted.Take(27).Sum(x => x.Rks / 30d);

		return (phi3, sorted, rks);
	}
	/// <summary>
	/// Sorts the records, merges the phi scores, and returns the sorted list and the RKS.
	/// Note: this is for game version > 3.11.0
	/// </summary>
	/// <returns>A tuple containing the sorted list of scores and the RKS.</returns>
	public (List<CompleteScore> Scores, double Rks) GetSortedListForRksMerged()
	{
		(List<CompleteScore> phis, List<CompleteScore> scores, double rks) = this.GetSortedListForRks();
		scores.InsertRange(0, phis);
		return (scores, rks);
	}
}