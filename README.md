# PhigrosLibraryCSharp
This is a C# implementation of [PhigrosLibrary](https://github.com/7aGiven/PhigrosLibrary), 
allowing you to integrate Phigros login workflow and process your scores nicely.

Now available in [NuGet](https://www.nuget.org/packages/PhigrosLibraryCSharp/)!
# API Usage
AI generated wiki: [![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/yt6983138/PhigrosLibraryCSharp)
## Local save (Reading from PlayerPrefv2.xml)
1. Parse the xml and get the key and value string pair, the string should look like `xaHiFItVgoS6CBFNHTR2%2BA%3D%3D`
2. URL decode it using `System.Net.WebUtility.UrlDecode`, to get the Base64 string (ex. `xaHiFItVgoS6CBFNHTR2+A==`)
3. Decrypt it using static function 
   `LocalSave.DecryptSaveStringNew(string base64EncodedString)`, there is also a model for score (`RawScore`, 
   can be converted into a more dev-friendly format by calling `ToInternalFormat` method)
   - Example: `LocalSave.DecryptSaveStringNew("eSB6QJlXU1vwHjVx7kcpb4/jdk9o5j4Wiatn+jrJ+etI2KFMlPDyH8s7I8zM+qlW")`
   - Decrypted score string pair would be something like `MARENOL.LeaF.0.Record.HD` and `{"s":992580,"a":99.17559051513672,"c":1}`.
   - Note: the PlayerPrefv2.xml also contains other misc data, so you need to pick scores.
4. Once decrypted, you can do anything to the score data. <br/>
Full example code:
```cs
public static SongScore ExampleDecryptLocal(string key, string data)
{
	string decryptedKey = LocalSave.DecryptSaveStringNew(WebUtility.UrlDecode(key));
	string decryptedData = LocalSave.DecryptSaveStringNew(WebUtility.UrlDecode(data));

	string[] splitted = decryptedKey.Split(".");
	string id = string.Join(".", splitted.Take(3));
	return RawScore.FromJson(decryptedData).ToSongScore(id, Enum.Parse<Difficulty>(splitted[^1]));
}

// somewhere else in ur code...
ExampleDecryptLocal(@"Cgw4SttKwRFIjb68TF8z5EC%2FLwVpK8KjKmjcm9T3M78%3D", @"eSB6QJlXU1vwHjVx7kcpb4%2Fjdk9o5j4Wiatn%2BjrJ%2BetI2KFMlPDyH8s7I8zM%2BqlW");
```
## Cloud Save
Common examples can be found [there](/PhigrosLibraryCSharp.Examples/Program.cs).
### TapTap integration
[See this](/PhigrosLibraryCSharp.Examples/Program.cs#L47) for QRCode login example.
There is also a callback login implementation, however it is limited since you can only callback to localhost 
(which means you cannot use this when you are hosting a server like a Discord bot.)
### Fetching scores and other user info
Here is a short example of how to read scores from cloud save:
```cs
Save save = new(/* phigros token */, /* is international or not */);

// if you are using it in wasm project then you need to replace the decrypt function since
// aes doesn't work in browser environment:
// save.Decrypter = async (byte[] key, byte[] iv, byte[] data) => { /* your decrypting function here */ };
var context = await save.GetSaveContextAsync(/* save index, 0 for the latest save */);
(List<CompleteScore> Phi3, List<CompleteScore> Other, double RKS) context.ReadGameRecord().GetSortedListForRks(/* difficulty map */, /* name map */); // all scores
context.ReadSummary() // summary info like username, level, etc.
context.ReadGameUserInfo() // in-game user info
// there's also other functions for reading different data, check the documentation for more info.
```
`difficulties` parameter should be a `IReadOnlyDictionary<ChartConstantKey, float>`, 
the `ChartConstantKey` would be song id and difficulty combined together, and the float is the chart constants.<br/>
Example difficulty value pair: `{ new ChartConstantKey("Credits.Frums.0", Difficulty.IN), 14.0f }`

For more info, please check the common examples. <br/>

# Migrating from v3 to v4
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
