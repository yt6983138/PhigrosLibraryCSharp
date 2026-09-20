using PhigrosLibraryCSharp.CloudSave;

namespace PhigrosLibraryCSharp.Tests;

[TestClass]
public class ScoreTest
{
	private static CompleteScore CreateCompleteScore(string id, Difficulty diff, int score, float acc, bool isFc = false)
	{
		SongScore s = new(score, acc, id, isFc, diff);
		return new CompleteScore(s, DataModelTest._testConstantMap, DataModelTest._testNameMap);
	}

	[TestMethod]
	[DataRow(100.0, 15.0, 15.0)]
	[DataRow(100.0, 10.0, 10.0)]
	[DataRow(100.0, 5.0, 5.0)]
	[DataRow(99.0, 15.0, 14.3407)]
	[DataRow(90.0, 15.0, 9.0741)]
	[DataRow(70.0, 15.0, 1.6667)]
	[DataRow(69.0, 15.0, 0.0)]
	[DataRow(0.0, 15.0, 0.0)]
	[DataRow(55.0, 15.0, 0.0)]
	[DataRow(54.9, 15.0, 0.0)]
	public void TestCompleteScoreRksCalculation(double accuracy, double chartConstant, double expectedRks)
	{
		SongScore score = new(960000, (float)accuracy, "SongA.0", false, Difficulty.IN);
		Dictionary<ChartConstantKey, float> constantMap = new()
		{
			{ new("SongA.0", Difficulty.IN), (float)chartConstant },
		};
		CompleteScore complete = new(score, constantMap, DataModelTest._testNameMap);

		Assert.AreEqual(expectedRks, complete.Rks, 0.0002);
	}

	[TestMethod]
	public void TestCompleteScoreRksAccuracies()
	{
		CompleteScore on0 = CreateCompleteScore("SongA.0", Difficulty.IN, 0, 0f);
		CompleteScore on70 = CreateCompleteScore("SongA.0", Difficulty.IN, 700000, 70f);
		CompleteScore less70 = CreateCompleteScore("SongA.0", Difficulty.IN, 699999, 69.99f);
		CompleteScore on100 = CreateCompleteScore("SongA.0", Difficulty.IN, 1000000, 100f);

		Assert.AreEqual(0.0, on0.Rks, 0.0001);
		Assert.AreEqual(1.0 / 9.0 * 15.0, on70.Rks, 0.0001);
		Assert.AreEqual(0.0, less70.Rks, 0.0001);
		Assert.AreEqual(15.0, on100.Rks, 0.0001);
	}

	[TestMethod]
	public void TestCompleteScoreNameOrDefaultFound()
	{
		CompleteScore score = CreateCompleteScore("SongA.0", Difficulty.EZ, 950000, 95f);

		Assert.AreEqual("Alpha", score.NameOrDefault);
	}

	[TestMethod]
	public void TestCompleteScoreNames()
	{
		CompleteScore unknown = CreateCompleteScore("Unknown.0", Difficulty.EZ, 950000, 95f);
		CompleteScore known = CreateCompleteScore("SongA.0", Difficulty.EZ, 950000, 95f);

		Assert.AreEqual("Unknown.0", unknown.NameOrDefault);
		Assert.AreEqual("Alpha", known.NameOrDefault);
	}

	[TestMethod]
	public void TestCompleteScoreNameThrowsWhenNotFound()
	{
		CompleteScore score = CreateCompleteScore("Unknown.0", Difficulty.EZ, 950000, 95f);
		Assert.Throws<KeyNotFoundException>(() => { _ = score.Name; });
	}

	[TestMethod]
	public void TestCompleteScoreChartConstant()
	{
		CompleteScore score = CreateCompleteScore("SongA.0", Difficulty.IN, 950000, 95f);
		Assert.AreEqual(15f, score.ChartConstant);
	}

	[TestMethod]
	public void TestCompleteScoreChartConstantThrowsWhenNotFound()
	{
		CompleteScore score = CreateCompleteScore("Unknown.0", Difficulty.EZ, 950000, 95f);
		Assert.Throws<KeyNotFoundException>(() => { _ = score.ChartConstant; });
	}

	[TestMethod]
	public void TestCompleteScoreCompare()
	{
		// CompareTo does descending sort: other.Rks.CompareTo(this.Rks)
		// higher rks -> smaller CompareTo result (sorts first)
		CompleteScore high = CreateCompleteScore("SongA.0", Difficulty.IN, 1000000, 100f);
		CompleteScore low = CreateCompleteScore("SongA.0", Difficulty.IN, 900000, 90f);
		CompleteScore equalHigh = CreateCompleteScore("SongA.0", Difficulty.IN, 1000000, 100f);

		Assert.IsLessThan(0, high.CompareTo(low));
		Assert.IsGreaterThan(0, low.CompareTo(high));
		Assert.AreEqual(0, high.CompareTo(equalHigh));
	}

	[TestMethod]
	public void TestCompleteScoreEquality()
	{
		CompleteScore a = CreateCompleteScore("SongA.0", Difficulty.EZ, 950000, 95f);
		CompleteScore b = CreateCompleteScore("SongA.0", Difficulty.EZ, 950000, 95f);
		CompleteScore c = CreateCompleteScore("SongA.0", Difficulty.EZ, 950000, 96f);
		CompleteScore d = CreateCompleteScore("SongA.0", Difficulty.EZ, 960000, 95f);
		CompleteScore e = CreateCompleteScore("SongA.0", Difficulty.HD, 950000, 95f);
		CompleteScore f = CreateCompleteScore("SongB.0", Difficulty.EZ, 950000, 95f);

		Assert.IsTrue(a.Equals(a));
#pragma warning disable CS1718 // Comparison made to same variable
		Assert.IsTrue(a == a);
		Assert.IsFalse(a != a);
#pragma warning restore CS1718 // Comparison made to same variable

		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(a == b);
		Assert.IsFalse(a != b);

		Assert.IsFalse(a.Equals(c));
		Assert.IsFalse(a.Equals(d));
		Assert.IsFalse(a.Equals(e));
		Assert.IsFalse(a.Equals(f));

		Assert.IsFalse(a.Equals(null));
		Assert.IsFalse(a.Equals("not score"));
	}

	[TestMethod]
	public void TestCompleteScoreGetHashCode()
	{
		CompleteScore a = CreateCompleteScore("SongA.0", Difficulty.EZ, 950000, 95f);
		CompleteScore b = CreateCompleteScore("SongA.0", Difficulty.EZ, 950000, 95f);
		CompleteScore c = CreateCompleteScore("SongA.0", Difficulty.EZ, 950000, 94f);

		Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
		Assert.AreNotEqual(a.GetHashCode(), c.GetHashCode());
	}

	[TestMethod]
	public void TestCompleteScoreDefault()
	{
		CompleteScore score = CompleteScore.Default;

		Assert.AreEqual(0.0, score.Rks, 0.0001);
		Assert.AreEqual("", score.NameOrDefault);
		Assert.AreEqual(0f, score.ChartConstant);
	}

	[TestMethod]
	public void TestCompleteScoreSortDescendingByRks()
	{
		List<CompleteScore> scores =
		[
			CreateCompleteScore("SongA.0", Difficulty.IN, 800000, 80f),  // low rks
			CreateCompleteScore("SongA.0", Difficulty.IN, 1000000, 100f), // high rks
			CreateCompleteScore("SongA.0", Difficulty.IN, 950000, 95f),  // medium rks
		];

		scores.Sort();

		Assert.AreEqual(15.0, scores[0].Rks, 0.0001);  // highest first
		Assert.IsGreaterThanOrEqualTo(scores[1].Rks, scores[0].Rks);
		Assert.IsGreaterThanOrEqualTo(scores[2].Rks, scores[1].Rks);
	}

	[TestMethod]
	public void TestSongScoreDefaultValues()
	{
		SongScore score = SongScore.Default;

		Assert.AreEqual(0, score.Score);
		Assert.AreEqual(0f, score.Accuracy);
		Assert.AreEqual("", score.Id);
		Assert.AreEqual(Difficulty.EZ, score.Difficulty);
		Assert.AreEqual(ScoreStatus.False, score.Status);
	}
	[TestMethod]
	public void TestSongScoreDefaultReturnsNewInstance()
	{
		SongScore first = SongScore.Default;
		SongScore second = SongScore.Default;

		Assert.AreNotSame(first, second);

		first.Score = 999;
		Assert.AreEqual(0, second.Score);
	}

	[TestMethod]
	public void TestSongScoreConstructorWithStatus()
	{
		SongScore score = new(950000, 95f, "Song.0", Difficulty.IN, ScoreStatus.Vu);

		Assert.AreEqual(950000, score.Score);
		Assert.AreEqual(95f, score.Accuracy);
		Assert.AreEqual("Song.0", score.Id);
		Assert.AreEqual(Difficulty.IN, score.Difficulty);
		Assert.AreEqual(ScoreStatus.S, score.Status);
	}

	[TestMethod]
	public void TestSongScoreConstructorWithIsFcTrue()
	{
		SongScore score = new(950000, 95f, "Song.0", true, Difficulty.IN);

		Assert.AreEqual(ScoreStatus.Fc, score.Status);
	}

	[TestMethod]
	public void TestSongScoreConstructorWithIsFcFalse()
	{
		SongScore score = new(950000, 95f, "Song.0", false, Difficulty.IN);

		Assert.AreEqual(ScoreStatus.S, score.Status);
	}

	[TestMethod]
	[DataRow(1000000, 100.0, false, ScoreStatus.Phi)]
	[DataRow(999999, 100.0, false, ScoreStatus.Bugged)]
	[DataRow(969696, 98.0, true, ScoreStatus.Fc)]
	[DataRow(999999, 98.0, false, ScoreStatus.Vu)]
	[DataRow(960000, 98.0, false, ScoreStatus.Vu)]
	[DataRow(959999, 98.0, false, ScoreStatus.S)]
	[DataRow(920000, 98.0, false, ScoreStatus.S)]
	[DataRow(919999, 98.0, false, ScoreStatus.A)]
	[DataRow(880000, 98.0, false, ScoreStatus.A)]
	[DataRow(879999, 98.0, false, ScoreStatus.B)]
	[DataRow(820000, 98.0, false, ScoreStatus.B)]
	[DataRow(819999, 98.0, false, ScoreStatus.C)]
	[DataRow(700000, 98.0, false, ScoreStatus.C)]
	[DataRow(699999, 98.0, false, ScoreStatus.False)]
	[DataRow(0, 98.0, false, ScoreStatus.False)]
	[DataRow(-1, 98.0, false, ScoreStatus.Bugged)]
	public void TestSongScoreStatusPropertyLogic(int score, double accuracy, bool isFc, ScoreStatus expected)
	{
		SongScore s = new(score, (float)accuracy, "Song.0", isFc, Difficulty.IN);

		Assert.AreEqual(expected, s.Status);
	}

	[TestMethod]
	public void TestSongScoreStatusSetter()
	{
		SongScore score = new(950000, 95f, "Song.0", false, Difficulty.IN)
		{
			Status = ScoreStatus.Fc,
		};
		Assert.AreEqual(ScoreStatus.Fc, score.Status);

		score.Score = 1000000;
		score.Accuracy = 100f;
		score.Status = ScoreStatus.Phi;
		Assert.AreEqual(ScoreStatus.Phi, score.Status);

		// cant be phi since score and acc != full, silently ignored
		score.Score = 999999;
		score.Accuracy = 99f;
		score.Status = ScoreStatus.Phi;
		Assert.AreEqual(ScoreStatus.Vu, score.Status);

		// removes fc
		score.Status = ScoreStatus.Vu;
		Assert.AreEqual(ScoreStatus.Vu, score.Status);

		// fall back to parse score/acc directly
		score.Score = 930000;
		score.Accuracy = 98f;
		score.Status = ScoreStatus.False;
		Assert.AreEqual(ScoreStatus.S, score.Status);
	}

	[TestMethod]
	public void TestSongScoreEquality()
	{
		SongScore a = new(950000, 95f, "Song.0", true, Difficulty.IN);
		SongScore b = new(950000, 95f, "Song.0", true, Difficulty.IN);
		SongScore c = new(960000, 95f, "Song.0", true, Difficulty.IN);
		SongScore d = new(950000, 96f, "Song.0", true, Difficulty.IN);
		SongScore e = new(950000, 95f, "Other.0", true, Difficulty.IN);
		SongScore f = new(950000, 95f, "Song.0", true, Difficulty.HD);
		SongScore g = new(950000, 95f, "Song.0", false, Difficulty.IN);

		Assert.IsTrue(a.Equals(a));
#pragma warning disable CS1718 // Comparison made to same variable
		Assert.IsTrue(a == a);
		Assert.IsFalse(a != a);
#pragma warning restore CS1718 // Comparison made to same variable

		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(a == b);
		Assert.IsFalse(a != b);

		Assert.IsFalse(a.Equals(c));
		Assert.IsFalse(a == c);
		Assert.IsTrue(a != c);

		Assert.IsFalse(a.Equals(d));
		Assert.IsFalse(a.Equals(e));
		Assert.IsFalse(a.Equals(f));
		Assert.IsFalse(a.Equals(g));

		Assert.IsFalse(a.Equals(null));
		Assert.IsFalse(a == null);
		Assert.IsFalse(null == a);
		Assert.IsTrue(a != null);
		Assert.IsTrue(null != a);

		SongScore? n1 = null;
		SongScore? n2 = null;
		Assert.IsTrue(n1 == n2);
		Assert.IsFalse(n1 != n2);

		Assert.IsFalse(a.Equals("not a score"));

		object obj = new SongScore(950000, 95f, "Song.0", true, Difficulty.IN);
		Assert.IsTrue(a.Equals(obj));
	}

	[TestMethod]
	public void TestSongScoreGetHashCodeSameForEqual()
	{
		SongScore a = new(950000, 95f, "Song.0", true, Difficulty.IN);
		SongScore b = new(950000, 95f, "Song.0", true, Difficulty.IN);
		SongScore c = new(960000, 95f, "Song.0", true, Difficulty.IN);

		Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
		Assert.AreNotEqual(a.GetHashCode(), c.GetHashCode());
	}
}
