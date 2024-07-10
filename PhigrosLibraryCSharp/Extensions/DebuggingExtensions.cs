using System.Runtime.CompilerServices;
using System.Text;

namespace PhigrosLibraryCSharp.Extensions;

#if DEBUG
public
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#else
internal
#endif
	static class DebuggingExtensions
{
#if DEBUG
	public static bool Enable { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#endif

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static T Print<T>(this T obj)
	{
#if DEBUG
		if (Enable)
			Console.WriteLine(obj);
#endif
		return obj;
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static T Print<T>(this T obj, string message)
	{
#if DEBUG
		if (Enable)
			Console.WriteLine(string.Format(message, obj));
#endif
		return obj;
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static byte[] PrintAsUTF8(this byte[] obj)
	{
#if DEBUG
		if (Enable)
			Console.WriteLine(Encoding.UTF8.GetString(obj));
#endif
		return obj;
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static byte[] PrintAsUTF8(this byte[] obj, string message)
	{
#if DEBUG
		if (Enable)
			Console.WriteLine(string.Format(message, Encoding.UTF8.GetString(obj)));
#endif
		return obj;
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static byte[] PrintHex(this byte[] data)
	{
#if DEBUG
		if (Enable)
			Console.WriteLine(BitConverter.ToString(data).Replace('-', ' '));
#endif
		return data;
	}
}
