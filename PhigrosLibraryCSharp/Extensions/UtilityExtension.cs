using System.Diagnostics.CodeAnalysis;

namespace PhigrosLibraryCSharp.Extensions;
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
}
