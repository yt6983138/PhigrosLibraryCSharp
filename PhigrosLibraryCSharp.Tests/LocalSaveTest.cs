using PhigrosLibraryCSharp.CloudSave;
using PhigrosLibraryCSharp.LocalSave;

namespace PhigrosLibraryCSharp.Tests;

[TestClass]
public class LocalSaveTest
{
	[TestMethod]
	[DataRow(1000000, 100.0, true)]
	[DataRow(114514, 98.0, false)]
	public void TestRawScoreSerialization(int score, double accuracy, bool isFc)
	{
		string json = $$"""
			{
				"s": {{score}},
				"a": {{accuracy}},
				"c": {{(isFc ? 1 : 0)}}
			}
			""";
		RawScore rawScore = RawScore.FromJson(json);
		Assert.AreEqual(rawScore.Score, score);
		Assert.AreEqual(rawScore.Accuracy, accuracy);
		Assert.AreEqual(rawScore.Status, isFc ? ScoreStatus.Fc : ScoreStatus.NotFc);
	}

	[TestMethod]
	[DataRow(1000000, 100.0, true, Difficulty.AT, "Stasis.Maozon.0")]
	[DataRow(114514, 98.0, false, Difficulty.IN, "Some.Other.0")]
	public void TestRawScoreToSongScore(int score, double accuracy, bool isFc, Difficulty diff, string id)
	{
		RawScore rawScore = new()
		{
			Score = score,
			Accuracy = (float)accuracy,
			Status = isFc ? ScoreStatus.Fc : ScoreStatus.NotFc
		};
		SongScore songScore = rawScore.ToSongScore(id, diff);
		Assert.AreEqual(songScore.Score, rawScore.Score);
		Assert.AreEqual(songScore.Accuracy, rawScore.Accuracy);
		Assert.AreEqual(songScore.Id, id);
		Assert.AreEqual(songScore.Difficulty, diff);
		Assert.AreEqual(songScore.Status, ScoreHelper.ParseStatus(rawScore));
	}

	[TestMethod]
	[DataRow("xaHiFItVgoS6CBFNHTR2+A==", "1keyzhanshi2")]
	[DataRow("eSB6QJlXU1vwHjVx7kcpb4/jdk9o5j4Wiatn+jrJ+etI2KFMlPDyH8s7I8zM+qlW", "{\"s\":992580,\"a\":99.17559051513672,\"c\":1}")]
	public void TestLocalSaveDecrypt(string base64, string expected)
	{
		Assert.AreEqual(expected, LocalSave.LocalSave.DecryptLocalSaveStringNew(base64));
	}

	[TestMethod]
	[DataRow(1000000, 100.0, true, ScoreStatus.Phi)]
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
	public void TestScoreStatusParsing(int score, double accuracy, bool isFc, ScoreStatus expected)
	{
		Assert.AreEqual(expected, ScoreHelper.ParseStatus(score, accuracy, isFc));
	}
}
