using PhigrosLibraryCSharp.Cloud.Login;
using PhigrosLibraryCSharp.Cloud.Login.DataStructure;
using PhigrosLibraryCSharp.UnmanagedWrapper.Structures;
using System.Runtime.InteropServices;

namespace PhigrosLibraryCSharp.UnmanagedWrapper;
public unsafe static class LoginWrapper
{
	private static Dictionary<nint, Task<TapTapTokenData?>> _asyncCheckRequests = new();
	private static Dictionary<nint, Task<CompleteQRCodeData>> _asyncQrcodeRequests = new();
	private static nint _asyncCount = 0;

	#region Get QrCodes
	/// <summary>
	/// returns async handle
	/// </summary>
	/// <returns></returns>
	[UnmanagedCallersOnly(EntryPoint = nameof(GetQrcodeAsync))]
	public static AsyncHandle GetQrcodeAsync()
	{
		_asyncCount++;
		_asyncQrcodeRequests[_asyncCount] = TapTapHelper.RequestLoginQrCode();
		return (AsyncHandle)_asyncCount;
	}
	[UnmanagedCallersOnly(EntryPoint = nameof(CheckGetQrcodeHandle))]
	public static ReturnType CheckGetQrcodeHandle(AsyncHandle asyncHandle, Qrcode* data)
	{
		if (!_asyncQrcodeRequests.TryGetValue(asyncHandle, out Task<CompleteQRCodeData>? task))
		{
			return ReturnType.Error;
		}
		task.ContinueWith(_ => { }).Wait();
		if (task.IsFaulted)
		{
			_asyncQrcodeRequests.Remove(asyncHandle);
			return ReturnType.Error;
		}

		CompleteQRCodeData result = task.GetAwaiter().GetResult();
		Qrcode.FromLibrary(result, data);

		_asyncQrcodeRequests.Remove(asyncHandle);
		return ReturnType.Ok;
	}
	[UnmanagedCallersOnly(EntryPoint = nameof(GetQrcode))]
	public static ReturnType GetQrcode(Qrcode* data)
	{
		Task<CompleteQRCodeData?> task = TapTapHelper.RequestLoginQrCode().ContinueWith(t => t.IsFaulted ? null : t.Result);
		task.Wait();
		if (task.Result is null) return ReturnType.Error;

		CompleteQRCodeData result = task.Result;
		Qrcode.FromLibrary(result, data);

		return ReturnType.Ok;
	}
	[UnmanagedCallersOnly(EntryPoint = nameof(FreeQrcode))]
	public static void FreeQrcode(Qrcode* data)
	{
		Marshal.FreeHGlobal(data->DeviceCode);
		Marshal.FreeHGlobal(data->DeviceId);
		Marshal.FreeHGlobal(data->Url);
	}
	#endregion

	#region Check QrCode
	[UnmanagedCallersOnly(EntryPoint = nameof(CheckQrcodeAsync))]
	public static AsyncHandle CheckQrcodeAsync(Qrcode* data)
	{
		_asyncCount++;
		_asyncCheckRequests[_asyncCount] = TapTapHelper.CheckQRCodeResult(data->ToLibraryType());
		return (AsyncHandle)_asyncCount;
	}
	[UnmanagedCallersOnly(EntryPoint = nameof(CheckCheckQrcodeHandle))]
	public static ReturnType CheckCheckQrcodeHandle(AsyncHandle asyncHandle, TokenData* data)
	{
		if (!_asyncCheckRequests.TryGetValue(asyncHandle, out Task<TapTapTokenData?>? task))
		{
			return ReturnType.Error;
		}
		task.ContinueWith(_ => { }).Wait();
		if (task.IsFaulted)
		{
			_asyncQrcodeRequests.Remove(asyncHandle);
			return ReturnType.Error;
		}

		TapTapTokenData? result = task.GetAwaiter().GetResult();
		if (result is null)
		{
			_asyncQrcodeRequests.Remove(asyncHandle);
			return ReturnType.Other;
		}
		TokenData.FromLibrary(result, data);

		_asyncQrcodeRequests.Remove(asyncHandle);
		return ReturnType.Ok;
	}
	[UnmanagedCallersOnly(EntryPoint = nameof(CheckQrcode))]
	public static ReturnType CheckQrcode(Qrcode* data, TokenData* token)
	{
		Task<TapTapTokenData?> task = TapTapHelper.CheckQRCodeResult(data->ToLibraryType());
		task.ContinueWith(_ => { }).Wait();
		if (task.IsFaulted) return ReturnType.Error;
		if (task.Result is null) return ReturnType.Other;

		TapTapTokenData result = task.Result;
		TokenData.FromLibrary(result, token);

		return ReturnType.Ok;
	}
	[UnmanagedCallersOnly(EntryPoint = nameof(FreeTokenData))]
	public static void FreeTokenData(TokenData* token)
	{
		Marshal.FreeHGlobal(token->Kid);
		Marshal.FreeHGlobal(token->Token);
		Marshal.FreeHGlobal(token->TokenType);
		Marshal.FreeHGlobal(token->MacKey);
		Marshal.FreeHGlobal(token->MacAlgorithm);
		Marshal.FreeHGlobal(token->Scope);
	}
	#endregion
}
