using PhigrosLibraryCSharp.CloudSave;

namespace PhigrosLibraryCSharp.Tests;

[TestClass]
public class GameRecordTest
{
	[TestMethod]
	public void TestGameRecordGetCompleteScores()
	{
		List<SongScore> scores =
		[
			new(800000, 97f, "SongA.0", false, Difficulty.IN), // low rks
			new(1000000, 100f, "SongA.0", true, Difficulty.IN), // high rks (phi)
			new(970000, 97f, "SongB.0", true, Difficulty.EZ), // super low rks
		];
		GameRecord record = new(scores, 1);

		List<CompleteScore> complete = record.GetCompleteScores(DataModelTest._testConstantMap, DataModelTest._testNameMap).ToList();
		(List<CompleteScore> phis, List<CompleteScore> sorted, double rks) = record.GetSortedListForRks(DataModelTest._testConstantMap, DataModelTest._testNameMap);
		(_, double rks2) = record.GetSortedListForRksMerged(DataModelTest._testConstantMap, DataModelTest._testNameMap);

		Assert.HasCount(3, complete);
		Assert.AreEqual("Alpha", complete[0].NameOrDefault);
		Assert.AreEqual("Alpha", complete[1].NameOrDefault);
		Assert.AreEqual("Beta", complete[2].NameOrDefault);

		Assert.IsGreaterThanOrEqualTo(sorted[1].Rks, sorted[0].Rks);
		Assert.IsGreaterThanOrEqualTo(sorted[2].Rks, sorted[1].Rks);
		Assert.AreEqual(15.0, sorted[0].Rks, 0.0001); // the phi score
		Assert.HasCount(1, phis);
		Assert.AreEqual(ScoreStatus.Phi, phis[0].Score.Status);

		Assert.AreEqual(rks, rks2);
	}

	[TestMethod]
	public void TestGameRecordGetCompleteScoresEmpty()
	{
		GameRecord record = new([], 1);

		List<CompleteScore> complete = record.GetCompleteScores(DataModelTest._testConstantMap, DataModelTest._testNameMap).ToList();
		(List<CompleteScore> phis, List<CompleteScore> sorted, double rks) = record.GetSortedListForRks(DataModelTest._testConstantMap, DataModelTest._testNameMap);

		Assert.IsEmpty(complete);
		Assert.AreEqual(0.0, rks);
		Assert.IsEmpty(phis);
		Assert.IsEmpty(sorted);
	}

	[TestMethod]
	public void TestGameRecordGetSortedListForRksMoreThan3Phis()
	{
		List<SongScore> scores =
		[
			new(1000000, 100f, "SongA.0", true, Difficulty.IN),   // rks 15
			new(1000000, 100f, "SongB.0", true, Difficulty.EZ),     // rks 5
			new(1000000, 100f, "SongC.0", true, Difficulty.AT),    // rks 16.5
			new(1000000, 100f, "SongA.0", true, Difficulty.EZ),    // rks 10
		];
		GameRecord record = new(scores, 1);

		(List<CompleteScore> phis, List<CompleteScore> sorted, double rks) = record.GetSortedListForRks(DataModelTest._testConstantMap, DataModelTest._testNameMap);

		Assert.AreEqual(phis.Concat(sorted).Sum(x => x.Rks / 30f), rks, 0.0001);
		Assert.HasCount(3, phis);
	}

	[TestMethod]
	public void TestGameRecordGetSortedListForRksNoPhis()
	{
		List<SongScore> scores =
		[
			new(950000, 95f, "SongA.0", false, Difficulty.IN),  // rks = (40/45)^2 * 15 = 11.851...
		];
		GameRecord record = new(scores, 1);

		(List<CompleteScore> phis, List<CompleteScore> sorted, double rks) = record.GetSortedListForRks(DataModelTest._testConstantMap, DataModelTest._testNameMap);

		Assert.IsEmpty(phis);
		Assert.AreEqual(sorted[0].Rks / 30.0, rks, 0.0001);
	}

	[TestMethod]
	public void TestGameRecordGetSortedListForRksMergedPadsPhis()
	{
		List<SongScore> scores =
		[
			new(1000000, 100f, "SongA.0", true, Difficulty.IN),  // phi, rks 15
		];
		GameRecord record = new(scores, 1);

		(List<CompleteScore> merged, double rks) = record.GetSortedListForRksMerged(DataModelTest._testConstantMap, DataModelTest._testNameMap);

		Assert.IsGreaterThanOrEqualTo(4, merged.Count);
		Assert.AreEqual(15.0, merged[0].Rks, 0.0001);
		Assert.AreEqual(0.0, merged[1].Rks, 0.0001);
		Assert.AreEqual(0.0, merged[2].Rks, 0.0001);
		Assert.AreEqual(15.0, merged[3].Rks, 0.0001);
	}

	[TestMethod]
	public void TestGameRecordGetSortedListForRksMergedNoPhis()
	{
		List<SongScore> scores =
		[
			new(950000, 95f, "SongA.0", false, Difficulty.IN),
		];
		GameRecord record = new(scores, 1);

		(List<CompleteScore> merged, double rks) = record.GetSortedListForRksMerged(DataModelTest._testConstantMap, DataModelTest._testNameMap);

		Assert.HasCount(4, merged);
		Assert.AreEqual(0.0, merged[0].Rks, 0.0001);
		Assert.AreEqual(0.0, merged[1].Rks, 0.0001);
		Assert.AreEqual(0.0, merged[2].Rks, 0.0001);
	}
}
