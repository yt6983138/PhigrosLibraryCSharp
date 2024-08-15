#pragma comment(lib, "PhigrosLibraryCSharp.UnmanagedWrapper.lib")

typedef long AsyncHandle;

enum ReturnType
{
	Error = -1,
	Other = 0,
	Ok = 1
};

struct Qrcode
{
	char* Url;
	char* DeviceId;
	char* DeviceCode;
	int ExpiresInSeconds;
};

struct TapTapToken
{
	char* Kid;
	char* Token;
	char* TokenType;
	char* MacKey;
	char* MacAlgorithm;
	char* Scope;
};

#pragma region Get Qrcode
// async
extern AsyncHandle GetQrcodeAsync(void);
extern ReturnType CheckGetQrcodeHandle(AsyncHandle handle, Qrcode* qrcode);
// sync
extern ReturnType GetQrcode(Qrcode* qrcode);

extern void FreeQrcode(Qrcode* qrcode);
#pragma endregion

#pragma region Check Qrcode
// async
extern AsyncHandle CheckQrcodeAsync(Qrcode* qrcode);
extern ReturnType CheckCheckQrcodeHandle(AsyncHandle handle, TapTapToken* out);
// sync
extern ReturnType CheckQrcode(Qrcode* qrcode, TapTapToken* out);

extern void FreeTokenData(TapTapToken* token);
#pragma endregion

