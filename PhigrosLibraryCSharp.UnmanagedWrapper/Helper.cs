using System.Runtime.InteropServices;
using System.Text;

namespace PhigrosLibraryCSharp.UnmanagedWrapper;
internal static unsafe class Helper
{
	internal static IntPtr ToHGlobalUTF8NullTerminated(this string data)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(data);
		nint hGlobalPtr = Marshal.AllocHGlobal(bytes.Length + 1);

		fixed (byte* ptr = new Span<byte>(hGlobalPtr.ToPointer(), bytes.Length + 1))
		{
			int i;
			for (i = 0; i < bytes.Length; i++)
			{
				ptr[i] = bytes[i];
			}
			ptr[i] = 0x00; // add null terminator
		}

		return hGlobalPtr;
	}
}
