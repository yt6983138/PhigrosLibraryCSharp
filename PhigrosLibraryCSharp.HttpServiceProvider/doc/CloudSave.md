# POST /api/CloudSave/GetSaveIndexes
## Requirements
### Headers
Content-Type: text/plain
### Body
User's phigros token.
## Responses
### Success
```
< HTTP/1.1 200 OK
< Content-Type: application/json; charset=utf-8
< Date: Sat, 10 Aug 2024 13:22:37 GMT
< Server: Kestrel
< Transfer-Encoding: chunked
<
[
    {
        "modificationTime": "2024-06-07T07:29:56.697Z",
        "index": 0
    },
    ...
]
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
    "message": "Error processing request",
    "error": "[message]",
    "code": [Error code],
    "codeName": "[Error code's name]"
}
```
# POST /api/CloudSave/GetGameUserInfo/[id\]
Id is the id got from POST /api/CloudSave/GetSaveIndexes. 0 is always latest and the default value.
## Requirements
### Headers
Content-Type: text/plain
### Body
User's phigros token.
## Responses
### Success
```
< HTTP/1.1 200 OK
< Content-Type: application/json; charset=utf-8
< Date: Sat, 10 Aug 2024 13:22:37 GMT
< Server: Kestrel
< Transfer-Encoding: chunked
<
{
    "showUserId": true,
    "intro": "amongus sus\n\n69420",
    "avatarId": "\u3082\u307A\u3082\u307A1",
    "backgroundId": "Aleph-0"
}
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
    "message": "Error processing request",
    "error": "[message]",
    "code": [Error code],
    "codeName": "[Error code's name]"
}
```
# POST /api/CloudSave/GetGameSettings/[id\]
Id is the id got from POST /api/CloudSave/GetSaveIndexes. 0 is always latest and the default value.
## Requirements
### Headers
Content-Type: text/plain
### Body
User's phigros token.
## Responses
### Success
```
< HTTP/1.1 200 OK
< Content-Type: application/json; charset=utf-8
< Date: Sat, 10 Aug 2024 13:30:54 GMT
< Server: Kestrel
< Transfer-Encoding: chunked
<
{
    "chordSupport": true,
    "fcApIndicatorOn": true,
    "enableHitSound": false,
    "lowResolutionModeOn": false,
    "deviceName": "[Redacted]",
    "backgroundBrightness": 0.5454732,
    "musicVolume": 0.3261604,
    "effectVolume": 0.2909569,
    "hitSoundVolume": 1,
    "soundOffset": 0.045,
    "noteScale": 1
}
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
    "message": "Error processing request",
    "error": "[message]",
    "code": [Error code],
    "codeName": "[Error code's name]"
}
```
# POST /api/CloudSave/GetGameProgress/[id\]
Id is the id got from POST /api/CloudSave/GetSaveIndexes. 0 is always latest and the default value.
## Requirements
### Headers
Content-Type: text/plain
### Body
User's phigros token.
## Responses
### Success
```
< HTTP/1.1 200 OK
< Content-Type: application/json; charset=utf-8
< Date: Sat, 10 Aug 2024 13:31:55 GMT
< Server: Kestrel
< Transfer-Encoding: chunked
<
{
    "isFirstRun": true,
    "legacyChapterFinished": true,
    "alreadyShowCollectionTip": true,
    "alreadyShowAutoUnlockINTip": true,
    "completed": "3.0",
    "songUpdateInfo": 4,
    "challengeModeRank": 444,
    "money": {
        "kiB": 61,
        "miB": 180,
        "giB": 1,
        "tiB": 0,
        "piB": 0
    },
    "unlockFlagOfSpasmodic": 15,
    "unlockFlagOfIgallta": 15,
    "unlockFlagOfRrharil": 14,
    "flagOfSongRecordKey": 127,
    "randomVersionUnlocked": 1,
    "chapter8UnlockBegin": true,
    "chapter8UnlockSecondPhase": true,
    "chapter8Passed": true,
    "chapter8SongUnlockFlag": 63
}
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
    "message": "Error processing request",
    "error": "[message]",
    "code": [Error code],
    "codeName": "[Error code's name]"
}
```
# POST /api/CloudSave/GetSaveAndSummary/[id\]
Id is the id got from POST /api/CloudSave/GetSaveIndexes. 0 is always latest and the default value.
## Requirements
### Headers
Content-Type: text/plain
### Body
User's phigros token.
## Responses
### Success
```
< HTTP/1.1 200 OK
< Content-Type: application/json; charset=utf-8
< Date: Sat, 10 Aug 2024 13:32:39 GMT
< Server: Kestrel
< Transfer-Encoding: chunked
<
{
    "summary": {
        "saveVersion": 5,
        "gameVersion": 104,
        "challengeCode": 444,
        "avatar": "\u3082\u307A\u3082\u307A1",
        "clears": [
            7,
            1,
            0,
            111,
            27,
            0,
            190,
            64,
            16,
            23,
            4,
            0
        ]
    },
    "gameSave": {
        "creationDate": "2023-04-29T13:16:47.811Z",
        "modificationTime": "2024-06-07T07:29:59.251Z",
        "records": [
            {
                "id": "Glaciaxion.SunsetRay",
                "name": "Glaciaxion",
                "difficulty": "HD",
                "chartConstant": 6.5,
                "score": 958301,
                "acc": 99.59329986572266,
                "rksGiven": 6.383039779724785,
                "stat": "S" // can be the following: Bugged (should never happen), Phi, Fc, Vu, S, A, B, C, False.
            },
            // ...
        ]
    }
}
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
    "message": "Error processing request",
    "error": "[message]",
    "code": [Error code],
    "codeName": "[Error code's name]"
}
```
# POST /api/CloudSave/GetDecryptedZip/[id\]
Id is the id got from POST /api/CloudSave/GetSaveIndexes. 0 is always latest and the default value.
## Requirements
### Headers
Content-Type: text/plain
### Body
User's phigros token.
## Responses
### Success
```
< HTTP/1.1 200 OK
< Content-Length: 10369
< Content-Type: application/zip
< Date: Sat, 10 Aug 2024 13:34:23 GMT
< Server: Kestrel
<
[Zip binary output]
```
The zip contains following files: user, settings, gameProgress, gameRecord, and gameKey.
### Failure
```
< HTTP/1.1 422 Unprocessable Entity
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