using System.Text;

namespace PhigrosLibraryCSharp.AccountCreator;
internal static class MiscExtensions
{
	internal static long AsMillisecondsSinceUTC(this DateTime now)
		=> (long)now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
	internal static byte[] ToUtf8Bytes(this string str)
		=> Encoding.UTF8.GetBytes(str);
	internal static string ToHexString<T>(this T val) where T : struct
	{
		byte[] bytes = SerialHelper.StructToBytes(val);
		return BitConverter.ToString(bytes).Replace("-", "").ToLower();
	}
}
