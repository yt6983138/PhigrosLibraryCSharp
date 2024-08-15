Typically you'd do this in following order:
1. GET /api/LoginQrCode/GetNewQrCode, save the response and give the login url to the user.
2. do POST /api/LoginQrCode/CheckQRCode every [interval\] (specified in step one response), and once success continue step 3.
3. POST /LoginQrCode/GetPhigrosToken, and get token.

# GET /api/LoginQrCode/GetNewQrCode
## Requirements
No headers/body are needed.

## Responses
In any scenario:
```
< HTTP/1.1 200 OK
< Content-Type: application/json; charset=utf-8
< Date: Sat, 10 Aug 2024 03:07:47 GMT
< Server: Kestrel
< Transfer-Encoding: chunked
<
{
    "deviceID": "80492fe21eab44ebb7ce27649748bfa6",
    "deviceCode": "bdK9qxspDeBrbM2OUu4M0EBloEVR9App6Xh9buXD",
    "expiresInSeconds": 300,
    "url": "https://accounts.taptap.cn/device?qrcode=1\u0026user_code=lyhtp",
    "interval": 2
}
```
Response body from this request is needed.

# POST /api/LoginQrCode/CheckQRCode
## Requirements
### Headers
Content-Type: application/json
### Body
Response body from `GET /api/LoginQrCode/GetNewQrCode`.
### Other
Once you request new qrcode, you should do this request every [interval\] seconds, until code expires (expires in [expiresInSeconds\] seconds after requested).
## Responses
### When user done login
```
< HTTP/1.1 200 OK
< Content-Type: application/json; charset=utf-8
< Date: Sat, 10 Aug 2024 03:12:21 GMT
< Server: Kestrel
< Transfer-Encoding: chunked
<
{
    "data": {
        "kid": "[Redacted]",
        "token": "[Redacted]",
        "tokenType": "mac",
        "macKey": "[Redacted]",
        "macAlgorithm": "hmac-sha-1",
        "scope": "public_profile"
    }
}
```
Response body from this request is needed.

### When user have not done login
```
< HTTP/1.1 200 OK
< Content-Type: application/json; charset=utf-8
< Date: Sat, 10 Aug 2024 03:11:08 GMT
< Server: Kestrel
< Transfer-Encoding: chunked
<
{
    "message": "Login progress not done",
    "error": "",
    "code": 1,
    "codeName": "LoginProcessNotDone"
}
```
### When expired/invalid code was sent to server
```
< HTTP/1.1 500 Internal Server Error
< Content-Type: application/json; charset=utf-8
< Date: Sat, 10 Aug 2024 03:04:20 GMT
< Server: Kestrel
< Transfer-Encoding: chunked
<
{
    "message": "Error processing request",
    "error": "{\"data\":{\"code\":-1,\"msg\":\"请求错误\",\"error\":\"invalid_grant_code\",\"error_description\":\"oauth2.tapapis.com.INVALID_GRANT_CODE: InvalidArgument: the provided access grant is invalid, expired, or revoked\"},\"now\":1723259060,\"success\":false}",
    "code": 11,
    "codeName": "LoginOtherError"
}
```
# POST /api/LoginQrCode/GetTapTapProfile
## Requirements
### Headers
Content-Type: application/json
### Body
Response body from `POST /api/LoginQrCode/CheckQRCode`.
## Responses
### When correct data was sent
```
< HTTP/1.1 200 OK
< Content-Type: application/json; charset=utf-8
< Date: Sat, 10 Aug 2024 03:30:25 GMT
< Server: Kestrel
< Transfer-Encoding: chunked
<
{
    "data": {
        "openId": "[Redacted]",
        "unionId": "[Redacted]",
        "name": "static-void2",
        "gender": "",
        "avatar": "https://img3.tapimg.com/default_avatars/384aa197eceba6322c9af740d008e65e.jpg?imageMogr2/auto-orient/strip/thumbnail/!270x270r/gravity/Center/crop/270x270/format/jpg/interlace/1/quality/80"
    }
}
```
### When something is wrong
```
< HTTP/1.1 500 Internal Server Error
< Content-Type: application/json; charset=utf-8
< Date: Sat, 10 Aug 2024 03:32:58 GMT
< Server: Kestrel
< Transfer-Encoding: chunked
<
{
    "message": "Error processing request",
    "error": "[message]",
    "code": [Error code],
    "codeName": "[Error code's name]"
}
```
# POST /api/LoginQrCode/GetPhigrosToken
## Requirements
### Headers
Content-Type: application/json
### Body
Response body from `POST /api/LoginQrCode/CheckQRCode`.
## Responses
### When correct data was sent
```
< HTTP/1.1 200 OK
< Content-Length: 25
< Content-Type: text/plain; charset=utf-8
< Date: Sat, 10 Aug 2024 03:35:45 GMT
< Server: Kestrel
<
f5knk52e8o[Redacted]s7zx
```
### When something is wrong
```
< HTTP/1.1 500 Internal Server Error
< Content-Type: application/json; charset=utf-8
< Date: Sat, 10 Aug 2024 03:32:58 GMT
< Server: Kestrel
< Transfer-Encoding: chunked
<
{
    "message": "Error processing request",
    "error": "[message]",
    "code": [Error code],
    "codeName": "[Error code's name]"
}
```