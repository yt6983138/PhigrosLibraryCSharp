# 4.3.0
- Made all async apis in `Save`, `SaveContext`, `TapTapHelper`, and `LCHelper` classes cancellable by accepting `CancellationToken` parameters. Users should recompile to prevent `MethodNotFoundException`s.

<RELEASE_NOTE_END>

# 4.2.0
- Made `GameFileMetaData` properties nullable to handle missing data in some save files, as early versions of the sdk does not write those. Users depending on those properties should recompile.

# 4.1.1
- Improved quality of some comments in `Save` and `SaveContext` class
- Removed redundant session token null check in all methods of `Save` class
- Added `RequestHandler` property to `Save` class, allowing you to intercept (and modify) requests before they are sent, and responses before they are processed
- `Save` class now properly dispose `HttpResponseMessage` objects after done with them

# 4.1.0
- Made `SongScore` and `CompleteScore` implement equality related interfaces and override equality related methods.
- Added `CompleteScore.Default` property that returns an empty instance of `CompleteScore`.
- Started writing release note

# 4.0.0
In v4,the entire library's namespace structure has been reorganized, and added *partial* save write-back support (You can serialize but cannot upload).
Nearly every public type has moved, been renamed, or had its signature changed:
 - `PhigrosLibraryCSharp` (for `Save`, `SaveContext`, `LocalSave`) to `PhigrosLibraryCSharp.CloudSave` / `PhigrosLibraryCSharp.LocalSave`
 - `PhigrosLibraryCSharp.GameRecords` to `PhigrosLibraryCSharp.CloudSave`
 - `PhigrosLibraryCSharp.Cloud.Login` to `PhigrosLibraryCSharp.CloudSave.Login`
 - `PhigrosLibraryCSharp.Cloud.RawData` to `PhigrosLibraryCSharp.CloudSave.HttpModels`
## Request/Http model changes
They are renamed due to clarity issues:
 - `RawSave` to `SaveInfo`
 - `RawSaveContainer` to `SaveInfoContainer`
 - `SimplifiedSave` to `SimplifiedSaveInfo`
 - `UserInfo` to `PlayerInfo` (moved to `PhigrosLibraryCSharp.CloudSave.HttpModels`)
Furthermore, their corresponding operation name has also changed:
 - `GetRawSaveFromCloudAsync()` is renamed to `GetSaveInfoFromCloudAsync()`, and now returns `SaveInfoContainer`.
 - `GetSaveRawZipAsync(...)` is renamed to `GetSaveZipAsync(...)`.
 - `GetUserInfoAsync()` is renamed to `GetPlayerInfoAsync()`
 - `GetSaveContextAsync(RawSave)` now accepts `SaveInfo`.

## Save and SaveContext api .changes
`Save` class:
 - The AES delegate `CustomDecrypter Decrypter` is renamed to `AESCipherFunction Decryptor`. 
 - A symmetric `Encryptor` property of the same delegate type has also been added for write-back scenarios.
 - `Save` is now IDisposable, call Dispose() or wrap it in a using block to properly release the underlying `HttpClient`.
 - The `HttpClient Client` property is now public, allowing you to inspect or modify headers and handlers if needed.
`SaveContext` class:
 - You should now construct it using static async factory methods (`FromZipAsync(...)`)
 - `RawEntries` and its corresponding abstraction properties (`RawGameRecord`...) been removed.
 - `DecryptedEntries` now maps to `SaveContext.Entry`, which stores the decrypted data and version of the game file.
 - The property `RawSave OriginalData` is now `SaveInfo OriginalCloudObject`.
 - Save methods have been implemented (`SaveGameRecord`...), and now can be saved back to zip (`SaveToZipAsync(...)`)
## Save file model changes
Score processing:
 - `CompleteScore` now points to another type, original type is renamed to `SongScore`
 - Chart constant and name is now handled in `CompleteScore`, and `RKS` property has moved to this type.
 - `SongScore` is no longer comparable.
 - `SongScore.Id` **has numeric suffix now**, example: originally `Stasis.Maozon`, now `Stasis.Mazon.0`
 - `GameRecord.Summary` has been removed.
 - `GameRecord.Records` now stores `List<SongScore>` instead, use `GetCompleteScores(...)` method to get complete scores.
 - `GameRecord` **now parses legacy scores and no longer remove `Introduction` scores.**
Other models:
 - Added `GameKey` class, for processing game key files.
 - Other model classes are no longer records, their properties are now settable.
 - Many raw byte flags were converted to enum flags to ensure type safety.
