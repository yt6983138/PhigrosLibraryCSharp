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

extern AsyncHandle GetQrcodeAsync(void);
extern ReturnType CheckGetQrcodeHandle(AsyncHandle, Qrcode*);
extern ReturnType GetQrcode(Qrcode*);
extern void FreeQrcode(Qrcode*);

extern AsyncHandle CheckQrcodeAsync(Qrcode*);
extern ReturnType CheckCheckQrcodeHandle(AsyncHandle, TapTapToken*);
extern ReturnType CheckQrcode(Qrcode*, TapTapToken*);
extern void FreeTokenData(TapTapToken*);
