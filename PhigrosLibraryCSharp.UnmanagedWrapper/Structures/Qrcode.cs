using PhigrosLibraryCSharp.Cloud.Login.DataStructure;
using System.Runtime.InteropServices;

namespace PhigrosLibraryCSharp.UnmanagedWrapper.Structures;
public unsafe struct Qrcode
{
	public IntPtr Url;
	public IntPtr DeviceId;
	public IntPtr DeviceCode;
	public int ExpiresInSeconds;

	public CompleteQRCodeData ToLibraryType()
	{
		return new()
		{
			Url = Marshal.PtrToStringUTF8(this.Url)!,
			DeviceID = Marshal.PtrToStringUTF8(this.DeviceId)!,
			DeviceCode = Marshal.PtrToStringUTF8(this.DeviceCode)!,
			ExpiresInSeconds = this.ExpiresInSeconds
		};
	}
	public static void FromLibrary(CompleteQRCodeData data, Qrcode* ptr)
	{
		ptr->Url = data.Url.ToHGlobalUTF8NullTerminated();
		ptr->DeviceId = data.DeviceID.ToHGlobalUTF8NullTerminated();
		ptr->DeviceCode = data.DeviceCode.ToHGlobalUTF8NullTerminated();
		ptr->ExpiresInSeconds = data.ExpiresInSeconds;
	}
}
