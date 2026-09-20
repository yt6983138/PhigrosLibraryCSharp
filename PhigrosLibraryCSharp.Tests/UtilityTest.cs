namespace PhigrosLibraryCSharp.Tests;

[TestClass]
public class UtilityTest
{
	[TestMethod]
	public void TestEnsureNotNullReturnsValue()
	{
		string testString = "test";
		string result = testString.EnsureNotNull();
		Assert.AreEqual("test", result);
	}

	[TestMethod]
	public void TestEnsureNotNullNullThrows()
	{
		string? nullString = null;
		Assert.Throws<DebugArgumentNullException>(() =>
		{
			nullString.EnsureNotNull();
		});
	}

	[TestMethod]
	public void TestEnsureNotNullWithInfoIncludesInfo()
	{
		string? nullString = null;
		DebugArgumentNullException ex = Assert.Throws<DebugArgumentNullException>(() =>
		{
			nullString.EnsureNotNull("Additional context information");
		});
		Assert.AreEqual("Additional context information", ex.AdditionalMessage);
	}

	[TestMethod]
	public void TestEnsureNotNullValueTypeReturnsValue()
	{
		int testInt = 42;
		int result = testInt.EnsureNotNull();
		Assert.AreEqual(42, result);
	}

	[TestMethod]
	public void TestQuickCopyCreatesIndependentCopy()
	{
		int[] original = [1, 2, 3];
		int[] copy = UtilityExtension.QuickCopy(original);

		copy[0] = 99;
		Assert.AreEqual(1, original[0]);
		Assert.AreEqual(99, copy[0]);
	}

	[TestMethod]
	public void TestQuickCopyEmptyArrayReturnsEmpty()
	{
		int[] original = [];
		int[] copy = UtilityExtension.QuickCopy(original);

		Assert.IsEmpty(copy);
	}

	[TestMethod]
	public void TestQuickCopyStringArrayCopiesCorrectly()
	{
		string[] original = ["hello", "world", "test"];
		string[] copy = UtilityExtension.QuickCopy(original);

		Assert.HasCount(original.Length, copy);
		for (int i = 0; i < original.Length; i++)
		{
			Assert.AreEqual(original[i], copy[i]);
		}
	}

	[TestMethod]
	public void TestToHexEmptyArrayReturnsEmpty()
	{
		byte[] bytes = [];
		string result = bytes.ToHex();
		Assert.AreEqual("", result);
	}

	[TestMethod]
	public void TestToHexMixedBytesReturnsHex()
	{
		byte[] bytes = [0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56];
		string result = bytes.ToHex();
		Assert.AreEqual("abcdef123456", result);
	}


	[TestMethod]
	public void TestHasBit()
	{
		byte value = 0b00010101;
		Assert.IsTrue(value.HasBit(0));
		Assert.IsTrue(value.HasBit(2));
		Assert.IsTrue(value.HasBit(4));
		Assert.IsFalse(value.HasBit(1));
		Assert.IsFalse(value.HasBit(7));
		Assert.IsFalse(value.HasBit(3));
		Assert.IsFalse(value.HasBit(5));
	}

	[TestMethod]
	public void TestHasBitByteInvalidIndexThrows()
	{
		byte value = 5;
		Assert.Throws<ArgumentOutOfRangeException>(() =>
		{
			value.HasBit(-1);
		});
		Assert.Throws<ArgumentOutOfRangeException>(() =>
		{
			value.HasBit(8);
		});
	}

	// note: you need .net 10.0.200+ to not have compiler complaining internal extensions, see roslyn #81180
	[TestMethod]
	public void TestWritePackedBoolsSingleBool()
	{
		using MemoryStream ms = new();
		using BinaryWriter writer = new(ms);

		writer.WritePackedBools(true);

		byte[] result = ms.ToArray();
		Assert.HasCount(1, result);
		Assert.AreEqual(0b00000001, result[0]);
	}

	[TestMethod]
	public void TestWritePackedBoolsMultipleBools()
	{
		using MemoryStream ms = new();
		using BinaryWriter writer = new(ms);

		writer.WritePackedBools(true, false, true, false, false, true, true, false);

		byte[] result = ms.ToArray();
		Assert.HasCount(1, result);
		Assert.AreEqual(0b01100101, result[0]);
	}

	[TestMethod]
	public void TestWritePackedBoolsMoreThan8Throws()
	{
		using MemoryStream ms = new();
		using BinaryWriter writer = new(ms);

		Assert.Throws<ArgumentOutOfRangeException>(() =>
		{
			writer.WritePackedBools(true, true, true, true, true, true, true, true, true);
		});
	}

	[TestMethod]
	public void TestWritePackedBoolsEmptyWritesZero()
	{
		using MemoryStream ms = new();
		using BinaryWriter writer = new(ms);

		writer.WritePackedBools();

		byte[] result = ms.ToArray();
		Assert.HasCount(1, result);
		Assert.AreEqual(0x00, result[0]);
	}

	private enum TestByteEnum : byte
	{
		Value1 = 1,
		Value2 = 2,
		Value255 = 255
	}

	private enum TestIntEnum : int
	{
		ValueNegative = -1,
		Value0 = 0,
		Value100 = 100,
		Value1000 = 1000
	}

	[TestMethod]
	public void TestWriteEnumByteEnum()
	{
		using MemoryStream ms = new();
		using BinaryWriter writer = new(ms);

		writer.WriteEnum(TestByteEnum.Value2);

		byte[] result = ms.ToArray();
		Assert.HasCount(1, result);
		Assert.AreEqual(2, result[0]);
	}

	[TestMethod]
	public void TestHasMoreAtBeginningReturnsTrue()
	{
		byte[] data = [1, 2, 3, 4, 5];
		using BinaryReader reader = BinaryReader.FromArray(data, out MemoryStream ms);

		Assert.IsTrue(reader.HasMore);

		ms.Dispose();
	}

	[TestMethod]
	public void TestHasMoreAtEndReturnsFalse()
	{
		byte[] data = [1, 2, 3];
		using BinaryReader reader = BinaryReader.FromArray(data, out MemoryStream ms);

		reader.ReadByte();
		reader.ReadByte();
		reader.ReadByte();
		Assert.IsFalse(reader.HasMore);

		ms.Dispose();
	}

	[TestMethod]
	public void TestFromArrayCreatesReader()
	{
		byte[] data = [1, 2, 3, 4, 5];
		using BinaryReader reader = BinaryReader.FromArray(data, out MemoryStream ms);

		Assert.IsNotNull(reader);
		Assert.IsNotNull(ms);
		Assert.AreEqual(5, ms.Length);

		ms.Dispose();
	}

	[TestMethod]
	public void TestFromArrayReadsData()
	{
		byte[] data = [10, 20, 30];
		using BinaryReader reader = BinaryReader.FromArray(data, out MemoryStream ms);

		Assert.AreEqual(10, reader.ReadByte());
		Assert.AreEqual(20, reader.ReadByte());
		Assert.AreEqual(30, reader.ReadByte());

		ms.Dispose();
	}

	[TestMethod]
	public void TestReadFromPackedBoolNoJumpReadsBit()
	{
		byte[] data = [0b00000101]; // bits 0 and 2 are set
		using BinaryReader reader = BinaryReader.FromArray(data, out MemoryStream ms);

		Assert.IsTrue(reader.ReadFromPackedBoolNoJump(0));
		Assert.IsFalse(reader.ReadFromPackedBoolNoJump(1));
		Assert.IsTrue(reader.ReadFromPackedBoolNoJump(2));

		ms.Dispose();
	}

	[TestMethod]
	public void TestReadFromPackedBoolNoJumpDoesNotAdvancePosition()
	{
		byte[] data = [0b00000101, 0xFF];
		using BinaryReader reader = BinaryReader.FromArray(data, out MemoryStream ms);

		long initialPosition = reader.BaseStream.Position;
		reader.ReadFromPackedBoolNoJump(0);
		Assert.AreEqual(initialPosition, reader.BaseStream.Position);

		ms.Dispose();
	}

	[TestMethod]
	public void TestReadFromPackedBoolThenJumpReadsBit()
	{
		byte[] data = [0b00000101];
		using BinaryReader reader = BinaryReader.FromArray(data, out MemoryStream ms);

		Assert.IsTrue(reader.ReadFromPackedBoolThenJump(0));

		ms.Dispose();
	}

	[TestMethod]
	public void TestReadFromPackedBoolThenJumpAdvancesPosition()
	{
		byte[] data = [0b00000101, 0xFF];
		using BinaryReader reader = BinaryReader.FromArray(data, out MemoryStream ms);

		long initialPosition = reader.BaseStream.Position;
		reader.ReadFromPackedBoolThenJump(0);
		Assert.AreEqual(initialPosition + 1, reader.BaseStream.Position);

		ms.Dispose();
	}

	[TestMethod]
	public void TestReadFromPackedBoolThenJumpInvalidIndexThrows()
	{
		byte[] data = [0xFF];
		using BinaryReader reader = BinaryReader.FromArray(data, out MemoryStream ms);

		Assert.Throws<ArgumentOutOfRangeException>(() =>
		{
			reader.ReadFromPackedBoolThenJump(-1);
		});
		Assert.Throws<ArgumentOutOfRangeException>(() =>
		{
			reader.ReadFromPackedBoolThenJump(8);
		});


		ms.Dispose();
	}

	[TestMethod]
	public void TestReadFromPackedBoolThenJumpAllBits()
	{
		byte[] data = [0b10101010]; // alternating bits
		using BinaryReader reader = BinaryReader.FromArray(data, out MemoryStream ms);

		Assert.IsFalse(reader.ReadFromPackedBoolNoJump(0));
		Assert.IsTrue(reader.ReadFromPackedBoolNoJump(1));
		Assert.IsFalse(reader.ReadFromPackedBoolNoJump(2));
		Assert.IsTrue(reader.ReadFromPackedBoolNoJump(3));
		Assert.IsFalse(reader.ReadFromPackedBoolNoJump(4));
		Assert.IsTrue(reader.ReadFromPackedBoolNoJump(5));
		Assert.IsFalse(reader.ReadFromPackedBoolNoJump(6));
		Assert.IsTrue(reader.ReadFromPackedBoolThenJump(7));

		ms.Dispose();
	}

	[TestMethod]
	public void TestReadEnumByteEnumReadsValue()
	{
		byte[] data = [2];
		using BinaryReader reader = BinaryReader.FromArray(data, out MemoryStream ms);

		TestByteEnum result = reader.ReadEnum<TestByteEnum>();
		Assert.AreEqual(TestByteEnum.Value2, result);

		ms.Dispose();
	}

	[TestMethod]
	public void TestReadEnumIntEnumReadsValue()
	{
		byte[] data = BitConverter.GetBytes(100);
		using BinaryReader reader = BinaryReader.FromArray(data, out MemoryStream ms);

		TestIntEnum result = reader.ReadEnum<TestIntEnum>();
		Assert.AreEqual(TestIntEnum.Value100, result);

		ms.Dispose();
	}
}
