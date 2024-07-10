using System.Runtime.CompilerServices;

namespace PhigrosLibraryCSharp.PackageExtract;

// i 100% didnt delete the code accidently and had to recover them from dnspy

// Token: 0x0200008E RID: 142
internal static class ParserHelper
{
	// Token: 0x06000371 RID: 881 RVA: 0x00016EEC File Offset: 0x000150EC
	internal static int ReadUnityInt(this ByteReader reader)
	{
		reader.Jump(4);
		return reader.Data[reader.Offset - 4] ^ reader.Data[reader.Offset - 3] << 8 ^ reader.Data[reader.Offset - 2] << 16;
	}

	// Token: 0x06000372 RID: 882 RVA: 0x00016F3C File Offset: 0x0001513C
	internal static byte[] ReadUnityStringBytes(this ByteReader reader)
	{
		int length = reader.ReadInt();
		byte[] data = RuntimeHelpers.GetSubArray(reader.Data, new Range(reader.Offset, reader.Offset + length));
		reader.Jump(length);
		return data;
	}

	// Token: 0x06000373 RID: 883 RVA: 0x00016F8C File Offset: 0x0001518C
	internal static byte[] ReadEntryByteArray(this ByteReader reader, int position)
	{
		int calculatedOffset = 4 + 28 * position;
		return RuntimeHelpers.GetSubArray(reader.Data, new Range(calculatedOffset, calculatedOffset + 28));
	}
}
