using System.Text;

namespace PhigrosLibraryCSharp.PackageExtract;
internal static class Extensions
{
	internal static byte[] AsBase64AndDecode(this string data)
		=> Convert.FromBase64String(data);
	internal static string ToBase64(this byte[] data)
		=> Convert.ToBase64String(data);
	internal static string AsUtf8(this byte[] array)
		=> Encoding.UTF8.GetString(array);
	internal static string AsUtf16(this byte[] array)
		=> Encoding.Unicode.GetString(array);
	internal static string AsAscii(this byte[] array)
		=> Encoding.ASCII.GetString(array);
}
