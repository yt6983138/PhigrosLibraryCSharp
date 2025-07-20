using System.Runtime.InteropServices;

namespace PhigrosLibraryCSharp;

internal static class SerialHelper
{
	internal static byte[] StructToBytes<T>(T structure) where T : struct
	{
		int size = Marshal.SizeOf(structure);
		byte[] output = new byte[size];

		IntPtr pointer = IntPtr.Zero;
		try
		{
			pointer = Marshal.AllocHGlobal(size);
			Marshal.StructureToPtr(structure, pointer, true);
			Marshal.Copy(pointer, output, 0, size);
		}
		finally
		{
			Marshal.FreeHGlobal(pointer);
		}
		return output;
	}
	internal static T ByteToStruct<T>(byte[] bytes) where T : struct
	{
		T str = new();

		int size = Marshal.SizeOf(str);
		IntPtr pointer = IntPtr.Zero;
		try
		{
			pointer = Marshal.AllocHGlobal(size);
			Marshal.Copy(bytes, 0, pointer, size);
			str = (T)Marshal.PtrToStructure(pointer, typeof(T))!;
		}
		finally
		{
			Marshal.FreeHGlobal(pointer);
		}
		return str;
	}
}
