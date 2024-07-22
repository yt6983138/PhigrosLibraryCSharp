using PhigrosLibraryCSharp.Cloud.Login.DataStructure;
using System.Runtime.InteropServices;

namespace PhigrosLibraryCSharp.UnmanagedWrapper.Structures;
public unsafe struct TokenData
{
	public IntPtr Kid;
	public IntPtr Token;
	public IntPtr TokenType;
	public IntPtr MacKey;
	public IntPtr MacAlgorithm;
	public IntPtr Scope;

	public TapTapTokenData ToLibraryType()
	{
		return new()
		{
			Data = new()
			{
				Kid = Marshal.PtrToStringUTF8(this.Kid)!,
				Token = Marshal.PtrToStringUTF8(this.Token)!,
				TokenType = Marshal.PtrToStringUTF8(this.TokenType)!,
				MacKey = Marshal.PtrToStringUTF8(this.MacKey)!,
				MacAlgorithm = Marshal.PtrToStringUTF8(this.MacAlgorithm)!,
				Scope = Marshal.PtrToStringUTF8(this.Scope)!
			}
		};
	}

	public static void FromLibrary(TapTapTokenData data, TokenData* ptr)
	{
		ptr->Kid = data.Data.Kid.ToHGlobalUTF8NullTerminated();
		ptr->Token = data.Data.Token.ToHGlobalUTF8NullTerminated();
		ptr->TokenType = data.Data.TokenType.ToHGlobalUTF8NullTerminated();
		ptr->MacKey = data.Data.MacKey.ToHGlobalUTF8NullTerminated();
		ptr->MacAlgorithm = data.Data.MacAlgorithm.ToHGlobalUTF8NullTerminated();
		ptr->Scope = data.Data.Scope.ToHGlobalUTF8NullTerminated();
	}
}
