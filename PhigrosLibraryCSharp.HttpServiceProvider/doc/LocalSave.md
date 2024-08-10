# POST /api/LocalSave/DecryptNew
## Requirements
### Headers
Content-Type: text/plain
### Body
The url encoded base64 string to decrypt. (example: xaHiFItVgoS6CBFNHTR2%2BA%3D%3D)
## Responses
### Success
```
< HTTP/1.1 200 OK
< Content-Length: 12
< Content-Type: text/plain; charset=utf-8
< Date: Sat, 10 Aug 2024 05:26:57 GMT
< Server: Kestrel
<
[Data decrypted, as from example it's 1keyzhanshi2]
```
### Failure
```
< HTTP/1.1 422 Unprocessable Entity
< Content-Type: application/json; charset=utf-8
< Date: Sat, 10 Aug 2024 03:32:58 GMT
< Server: Kestrel
< Transfer-Encoding: chunked
<
{
    "message": "Failed to decrypt data",
    "error": "[message]",
    "code": [Error code],
    "codeName": "[Error code's name]"
}
```