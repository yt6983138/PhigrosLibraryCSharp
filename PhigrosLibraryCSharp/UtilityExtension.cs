using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace PhigrosLibraryCSharp;
internal static class UtilityExtension
{
	[return: NotNull]
	internal static T EnsureNotNull<T>(this T obj)
	{
		if (obj == null) throw new ArgumentNullException(nameof(obj));
		return obj;
	}
	internal static T[] QuickCopy<T>(T[] array)
	{
		T[] values = new T[array.Length];
		array.CopyTo(values, 0);
		return values;
	}
	internal static string ToHex(this byte[] bytes)
	{
		StringBuilder sb = new();
		for (int i = 0; i < bytes.Length; i++)
		{
			sb.Append(bytes[i].ToString("x2"));
		}
		return sb.ToString();
	}
}
