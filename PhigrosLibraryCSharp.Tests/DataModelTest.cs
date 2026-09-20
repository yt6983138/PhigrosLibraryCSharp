using PhigrosLibraryCSharp.CloudSave;

namespace PhigrosLibraryCSharp.Tests;

[TestClass]
public class DataModelTest
{
	internal static readonly Dictionary<ChartConstantKey, float> _testConstantMap = new()
	{
		{ new("SongA.0", Difficulty.EZ), 10f },
		{ new("SongA.0", Difficulty.HD), 12f },
		{ new("SongA.0", Difficulty.IN), 15f },
		{ new("SongB.0", Difficulty.EZ), 5f },
		{ new("SongC.0", Difficulty.AT), 16.5f },
		{ new("", Difficulty.EZ), 0f },
	};
	internal static readonly Dictionary<string, string> _testNameMap = new()
	{
		{ "SongA.0", "Alpha" },
		{ "SongB.0", "Beta" },
		{ "SongC.0", "Gamma" },
		{ "", "DefaultName" },
	};

	[TestMethod]
	public void TestMoneyZero()
	{
		Money zero = Money.Zero;

		Assert.AreEqual(0, zero.KiB);
		Assert.AreEqual(0, zero.MiB);
		Assert.AreEqual(0, zero.GiB);
		Assert.AreEqual(0, zero.TiB);
		Assert.AreEqual(0, zero.PiB);

		Money second = Money.Zero;
		Assert.AreNotSame(zero, second);

		zero.KiB = 42;
		Assert.AreEqual(0, second.KiB);
	}

	[TestMethod]
	public void TestMoneyEquality()
	{
		Money first = new(1, 2, 3, 4, 5);

		Assert.AreEqual(1, first.KiB);
		Assert.AreEqual(2, first.MiB);
		Assert.AreEqual(3, first.GiB);
		Assert.AreEqual(4, first.TiB);
		Assert.AreEqual(5, first.PiB);

		Money second = new(1, 2, 3, 4, 5);
		Assert.IsTrue(first.Equals(second));
		Assert.IsTrue(first == second);
		Assert.IsFalse(first != second);

		Money different = new(6, 9, 4, 2, 0);
		Assert.IsFalse(first.Equals(different));
		Assert.IsFalse(first == different);
		Assert.IsTrue(first != different);

		Money? notExist = null;
		Assert.IsFalse(first.Equals(notExist));
		Assert.IsFalse(first == notExist);
		Assert.IsTrue(first != notExist);
		Assert.IsFalse(notExist == first);
		Assert.IsTrue(notExist != first);

		Money? notExist2 = null;
		Assert.IsTrue(notExist == notExist2);
		Assert.IsFalse(notExist != notExist2);

		object? notMoney = "not a money";
		Assert.IsFalse(first.Equals(notMoney));

		// object overload, different to the above ones
		object isMoney = second;
		Assert.IsTrue(first.Equals(isMoney));
	}

	[TestMethod]
	public void TestMoneyComparation()
	{
		Money first = new(0, 0, 0, 0, 0);
		Money identicalFirst = Money.Zero;

		Assert.IsGreaterThan(0, first.CompareTo(null));

		Assert.AreEqual(0, first.CompareTo(identicalFirst));

		Money second = new(1, 2, 3, 4, 5);
		Assert.IsLessThan(0, first.CompareTo(second));
		Assert.IsGreaterThan(0, second.CompareTo(first));
	}

	[TestMethod]
	public void TestMoneyCompareToDifference()
	{
		Money largeP = new(0, 0, 0, 0, 2);
		Money smallP = new(0, 0, 0, 0, 1);
		Money largeT = new(0, 0, 0, 100, 0);
		Money smallT = new(0, 0, 0, 50, 0);
		Money largeG = new(0, 0, 680, 0, 0);
		Money smallG = new(0, 0, 650, 0, 0);
		Money largeM = new(0, 999, 0, 0, 0);
		Money smallM = new(0, 998, 0, 0, 0);
		Money largeK = new(2048, 0, 0, 0, 0);
		Money smallK = new(1069, 0, 0, 0, 0);

		Assert.IsLessThan(0, smallP.CompareTo(largeP));
		Assert.IsGreaterThan(0, largeP.CompareTo(smallP));

		Assert.IsLessThan(0, smallT.CompareTo(largeT));
		Assert.IsGreaterThan(0, largeT.CompareTo(smallT));

		Assert.IsLessThan(0, smallG.CompareTo(largeG));
		Assert.IsGreaterThan(0, largeG.CompareTo(smallG));

		Assert.IsLessThan(0, smallM.CompareTo(largeM));
		Assert.IsGreaterThan(0, largeM.CompareTo(smallM));

		Assert.IsLessThan(0, smallK.CompareTo(largeK));
		Assert.IsGreaterThan(0, largeK.CompareTo(smallK));

		// test compare on bigger unit first
		Assert.IsLessThan(0, largeT.CompareTo(smallP));
		Assert.IsLessThan(0, largeG.CompareTo(smallT));
		Assert.IsLessThan(0, largeM.CompareTo(smallG));
		Assert.IsLessThan(0, largeK.CompareTo(smallM));
	}

	[TestMethod]
	public void TestMoneyToString()
	{
		Money a = new(5, 3, 0, 0, 0);
		Money b = new(5, 3, 2, 0, 0);
		Money c = new(5, 3, 2, 4, 0);
		Money d = new(5, 3, 2, 4, 1);
		Money e = new(0, 0, 0, 0, 1);
		Money f = new(5, 0, 0, 1, 0);

		Assert.AreEqual("3 MiB, 5 KiB", a.ToString());
		Assert.AreEqual("2 GiB, 3 MiB, 5 KiB", b.ToString());
		Assert.AreEqual("4 TiB, 2 GiB, 3 MiB, 5 KiB", c.ToString());
		Assert.AreEqual("1 PiB, 4 TiB, 2 GiB, 3 MiB, 5 KiB", d.ToString());
		Assert.AreEqual("1 PiB, 0 TiB, 0 GiB, 0 MiB, 0 KiB", e.ToString());
		Assert.AreEqual("1 TiB, 0 GiB, 0 MiB, 5 KiB", f.ToString());
		Assert.AreEqual("0 KiB", Money.Zero.ToString());
	}

	[TestMethod]
	public void TestMoneyGetHashCode()
	{
		Money a = new(1, 2, 3, 4, 5);
		Money b = new(1, 2, 3, 4, 5);
		Money c = new(5, 4, 3, 2, 1);

		Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
		Assert.AreNotEqual(a.GetHashCode(), c.GetHashCode());
	}

	[TestMethod]
	public void TestMoneySerialize()
	{
		Money original = new(16382, 370, 209570, 126, 67108864);
		byte[] expected = [0xfe, 0x7f, 0xf2, 0x02, 0xa2, 0xe5, 0x0c, 0x7e, 0x80, 0x80, 0x80, 0x20];

		using MemoryStream ms = new();
		using (BinaryWriter writer = new(ms))
		{
			original.Serialize(writer, out byte version);
			Assert.AreEqual(byte.MaxValue, version);
		}

		Assert.AreSequenceEqual(expected, ms.ToArray());
	}

	[TestMethod]
	public void TestMoneyDeserialize()
	{
		byte[] bytes = [0xfe, 0x7f, 0xf2, 0x02, 0xa2, 0xe5, 0x0c, 0x7e, 0x80, 0x80, 0x80, 0x20];
		using BinaryReader reader = BinaryReader.FromArray(bytes, out _);
		Money deserialized = Money.FromReader(reader, 0);
		Assert.AreEqual(new(16382, 370, 209570, 126, 67108864), deserialized);
	}

	[TestMethod]
	public void TestChallengeConstructorUshort()
	{
		Challenge challenge = new((ushort)548);
		Challenge challenge2 = new(-100); // short

		Assert.AreEqual((short)548, challenge.RawCode);
		Assert.AreEqual((short)-100, challenge2.RawCode);
	}

	[TestMethod]
	[DataRow(0, ChallengeRank.WhiteOrNone, 0)]
	[DataRow(21, ChallengeRank.WhiteOrNone, 21)]
	[DataRow(100, ChallengeRank.Green, 0)]
	[DataRow(167, ChallengeRank.Green, 67)]
	[DataRow(200, ChallengeRank.Blue, 0)]
	[DataRow(269, ChallengeRank.Blue, 69)]
	[DataRow(300, ChallengeRank.Red, 0)]
	[DataRow(325, ChallengeRank.Red, 325)]
	[DataRow(400, ChallengeRank.Gold, 0)]
	[DataRow(499, ChallengeRank.Gold, 99)]
	[DataRow(500, ChallengeRank.Rainbow, 0)]
	[DataRow(547, ChallengeRank.Rainbow, 47)]
	public void TestChallengeGet(int rawCode, ChallengeRank expectedRank, int expectedLevel)
	{
		Challenge challenge = new((short)rawCode);

		Assert.AreEqual(expectedRank, challenge.Rank);
	}

	[TestMethod]
	[DataRow(0, ChallengeRank.WhiteOrNone, 0)]
	[DataRow(21, ChallengeRank.WhiteOrNone, 21)]
	[DataRow(100, ChallengeRank.Green, 0)]
	[DataRow(167, ChallengeRank.Green, 67)]
	[DataRow(200, ChallengeRank.Blue, 0)]
	[DataRow(269, ChallengeRank.Blue, 69)]
	[DataRow(300, ChallengeRank.Red, 0)]
	[DataRow(325, ChallengeRank.Red, 25)]
	[DataRow(400, ChallengeRank.Gold, 0)]
	[DataRow(499, ChallengeRank.Gold, 99)]
	[DataRow(500, ChallengeRank.Rainbow, 0)]
	[DataRow(547, ChallengeRank.Rainbow, 47)]
	public void TestChallengeSet(int expectedRawCode, ChallengeRank rank, int level)
	{
		Challenge challenge = new((short)0)
		{
			Rank = rank,
			Level = (byte)level
		};

		Assert.AreEqual((short)expectedRawCode, challenge.RawCode);
	}

	[TestMethod]
	public void TestChallengeRankSetterPreservesLevel()
	{
		Challenge challenge = new((short)323)
		{
			Rank = ChallengeRank.Rainbow
		};

		Assert.AreEqual(ChallengeRank.Rainbow, challenge.Rank);
		Assert.AreEqual((byte)23, challenge.Level);
		Assert.AreEqual((short)523, challenge.RawCode);
	}

	[TestMethod]
	public void TestChallengeLevelSetterPreservesRank()
	{
		Challenge challenge = new((short)323)
		{
			Level = 50
		};

		Assert.AreEqual(ChallengeRank.Red, challenge.Rank);
		Assert.AreEqual((byte)50, challenge.Level);
		Assert.AreEqual((short)350, challenge.RawCode);
	}

	[TestMethod]
	public void TestChallengeHasEverDone()
	{
		Challenge challenge = new(0);
		Challenge challenge2 = new(100);

		Assert.IsFalse(challenge.HasEverDone);
		Assert.IsTrue(challenge2.HasEverDone);
	}

	[TestMethod]
	[DataRow(0, (GameKeyFlagType)0)]
	[DataRow(1, GameKeyFlagType.HasReadCollectionPieceCount)]
	[DataRow(1, GameKeyFlagType.HasUnlockedIllustration)]
	[DataRow(2, GameKeyFlagType.HasUnlockedAvatar | GameKeyFlagType.HasUnlockedCollectionPieceCount)]
	[DataRow(2, GameKeyFlagType.HasUnlockedAvatar | GameKeyFlagType.HasUnlockedIllustration)]
	[DataRow(3, GameKeyFlagType.HasUnlockedAvatar | GameKeyFlagType.HasUnlockedIllustration | GameKeyFlagType.HasUnlockedSingle)]
	public void TestGameKeyFlagPayloadCount(int expected, GameKeyFlagType type)
	{
		GameKeyFlag flag = new()
		{
			Type = type,
		};

		Assert.AreEqual(expected, flag.PayloadCount);
	}
	[TestMethod]
	public void TestGameKeyFlagPayloadCountUpperBitsNotIgnored()
	{
		// upper/unused bits should not be ignored so developers can use them for their own purposes
		GameKeyFlag flag = new()
		{
			Type = (GameKeyFlagType)0b11100000,
		};

		Assert.AreEqual(3, flag.PayloadCount);
	}

	[TestMethod]
	public void TestGameKeyFlagWritePayloadSetsTypeAndPayload()
	{
		GameKeyFlag flag = new();

		flag.WritePayload(GameKeyFlagType.HasUnlockedSingle, 42);

		Assert.IsTrue(flag.Type.HasFlag(GameKeyFlagType.HasUnlockedSingle));
		Assert.AreEqual(42, flag.ReadPayload(GameKeyFlagType.HasUnlockedSingle));
	}

	[TestMethod]
	public void TestGameKeyFlagWritePayloadMultiplePositions()
	{
		GameKeyFlag flag = new();

		flag.WritePayload(GameKeyFlagType.HasUnlockedSingle, 10);
		flag.WritePayload(GameKeyFlagType.HasUnlockedIllustration, 20);
		flag.WritePayload(GameKeyFlagType.HasUnlockedAvatar, 30);

		Assert.AreEqual(10, flag.ReadPayload(GameKeyFlagType.HasUnlockedSingle));
		Assert.AreEqual(20, flag.ReadPayload(GameKeyFlagType.HasUnlockedIllustration));
		Assert.AreEqual(30, flag.ReadPayload(GameKeyFlagType.HasUnlockedAvatar));
		Assert.AreEqual(3, flag.PayloadCount);
	}

	[TestMethod]
	public void TestGameKeyFlagWritePayloadOverwritesExisting()
	{
		GameKeyFlag flag = new();

		flag.WritePayload(GameKeyFlagType.HasUnlockedSingle, 10);
		flag.WritePayload(GameKeyFlagType.HasUnlockedSingle, 99);

		Assert.AreEqual(99, flag.ReadPayload(GameKeyFlagType.HasUnlockedSingle));
		Assert.AreEqual(1, flag.PayloadCount);
	}

	[TestMethod]
	public void TestGameKeyFlagWritePayloadClearsOldByte()
	{
		GameKeyFlag flag = new();

		flag.WritePayload(GameKeyFlagType.HasUnlockedSingle, 0xFF);
		flag.WritePayload(GameKeyFlagType.HasUnlockedSingle, 0x01);

		Assert.AreEqual(1, flag.ReadPayload(GameKeyFlagType.HasUnlockedSingle));
	}

	[TestMethod]
	public void TestGameKeyFlagRemovePayloadClearsType()
	{
		GameKeyFlag flag = new();

		flag.WritePayload(GameKeyFlagType.HasUnlockedSingle, 42);
		flag.RemovePayload(GameKeyFlagType.HasUnlockedSingle);

		Assert.IsFalse(flag.Type.HasFlag(GameKeyFlagType.HasUnlockedSingle));
		Assert.AreEqual(0, flag.PayloadCount);
	}

	[TestMethod]
	public void TestGameKeyFlagRemovePayloadClearsByte()
	{
		GameKeyFlag flag = new();

		flag.WritePayload(GameKeyFlagType.HasUnlockedSingle, 42);
		flag.WritePayload(GameKeyFlagType.HasUnlockedIllustration, 50);
		flag.RemovePayload(GameKeyFlagType.HasUnlockedSingle);

		// The byte for Single should be cleared, but Illustration remains
		Assert.AreEqual(50, flag.ReadPayload(GameKeyFlagType.HasUnlockedIllustration));
		Assert.AreEqual(1, flag.PayloadCount);
	}

	[TestMethod]
	public void TestGameKeyFlagReadPayloadThrows()
	{
		GameKeyFlag flag = new()
		{
			Type = GameKeyFlagType.HasUnlockedIllustration
		};

		Assert.Throws<ArgumentException>(() => flag.ReadPayload(GameKeyFlagType.HasUnlockedSingle));
		Assert.Throws<ArgumentException>(() => flag.ReadPayload(GameKeyFlagType.HasUnlockedIllustration | GameKeyFlagType.HasUnlockedSingle));
		Assert.Throws<ArgumentException>(() => flag.ReadPayload(0));
	}

	[TestMethod]
	public void TestGameKeyFlagRemovePayloadThrows()
	{
		GameKeyFlag flag = new();

		Assert.Throws<ArgumentException>(() => flag.RemovePayload(GameKeyFlagType.HasUnlockedSingle | GameKeyFlagType.HasUnlockedIllustration));
		Assert.Throws<ArgumentException>(() => flag.RemovePayload(0));
	}

	[TestMethod]
	public void TestGameKeyFlagWritePayloadThrows()
	{
		GameKeyFlag flag = new();

		Assert.Throws<ArgumentException>(() => flag.WritePayload(GameKeyFlagType.HasUnlockedSingle | GameKeyFlagType.HasUnlockedIllustration, 42));
		Assert.Throws<ArgumentException>(() => flag.WritePayload(0, 42));
	}

	[TestMethod]
	public void TestGameKeyFlagConstructorPackedFlagAndData()
	{
		byte packedFlag = (byte)(GameKeyFlagType.HasUnlockedSingle | GameKeyFlagType.HasUnlockedAvatar);
		byte[] data = [50, 69];

		GameKeyFlag flag = new(packedFlag, data);

		Assert.AreEqual(50, flag.ReadPayload(GameKeyFlagType.HasUnlockedSingle));
		Assert.AreEqual(69, flag.ReadPayload(GameKeyFlagType.HasUnlockedAvatar));
		Assert.AreEqual(2, flag.PayloadCount);
	}

	[TestMethod]
	public void TestGameKeyFlagConstructorByteArray()
	{
		byte[] data = [(byte)GameKeyFlagType.HasUnlockedSingle, 62];

		GameKeyFlag flag = new(data);

		Assert.AreEqual(62, flag.ReadPayload(GameKeyFlagType.HasUnlockedSingle));
	}

	[TestMethod]
	public void TestGameKeyFlagConstructorByteArrayMultiplePayloads()
	{
		byte type = (byte)(GameKeyFlagType.HasUnlockedSingle | GameKeyFlagType.HasUnlockedIllustration);
		byte[] data = [type, 100, 200];

		GameKeyFlag flag = new(data);

		Assert.AreEqual(100, flag.ReadPayload(GameKeyFlagType.HasUnlockedSingle));
		Assert.AreEqual(200, flag.ReadPayload(GameKeyFlagType.HasUnlockedIllustration));
		Assert.AreEqual(2, flag.PayloadCount);
	}
}
