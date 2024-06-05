using System.Runtime.CompilerServices;
using System.Text;

namespace PhigrosLibraryCSharp;
internal static class DebuggingExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static T Print<T>(this T obj)
	{
#if DEBUG
		Console.WriteLine(obj);
#endif
		return obj;
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static T Print<T>(this T obj, string message)
	{
#if DEBUG
		Console.WriteLine(string.Format(message, obj));
#endif
		return obj;
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static byte[] PrintAsUTF8(this byte[] obj)
	{
#if DEBUG
		Console.WriteLine(Encoding.UTF8.GetString(obj));
#endif
		return obj;
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static byte[] PrintAsUTF8(this byte[] obj, string message)
	{
#if DEBUG
		Console.WriteLine(string.Format(message, Encoding.UTF8.GetString(obj)));
#endif
		return obj;
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static byte[] PrintHex(this byte[] data)
	{
#if DEBUG
		Console.WriteLine(BitConverter.ToString(data).Replace('-', ' '));
#endif
		return data;
	}
}
