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
public static CompleteScore ExampleDecryptLocal(string key, string data, float chartConstant) 
{
	string decryptedKey = LocalSave.DecryptSaveStringNew(WebUtility.UrlDecode(key));
	string decryptedData = LocalSave.DecryptSaveStringNew(WebUtility.UrlDecode(data));

	string[] splitted = decryptedKey.Split(".");
		string id = $"{splitted[0]}.{splitted[1]}";
		return
			JsonConvert.DeserializeObject<RawScore>(decryptedData)
				.ToInternalFormat(
					chartConstant,
					id,
					splitted[^1]
				);
}

// somewhere else in ur code...
ExampleDecryptLocal(@"Cgw4SttKwRFIjb68TF8z5EC%2FLwVpK8KjKmjcm9T3M78%3D", @"eSB6QJlXU1vwHjVx7kcpb4%2Fjdk9o5j4Wiatn%2BjrJ%2BetI2KFMlPDyH8s7I8zM%2BqlW", 11.4f);
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
Save save = new(/* phigros token */);

// if you are using it in wasm project then you need to replace the decrypt function since
// aes doesn't work in browser environment:
// save.Decrypter = async (byte[] key, byte[] iv, byte[] data) => { /* your decrypting function here */ };
var context = await save.GetSaveContextAsync(/* save index, 0 for the latest save */);
context.ReadGameRecord(/* difficulties */).Records // all scores
context.ReadSummary() // summary info like username, level, etc.
context.ReadGameUserInfo() // in-game user info
// there's also other functions for reading different data, check the documentation for more info.

// as a function:
public static async Task<(Summary Summary, GameRecord Record)> GetLatestSave(string token, Dictionary<string, float[]> difficulties) 
{
	var helper = new Save(token);
	var context = await helper.GetSaveContextAsync(0);
	return (context.ReadSummary(), context.ReadGameRecord(difficulties));
}
// somewhere else in ur code...
await GetLatestSave("mysupersecrettoken1145141");
```
`difficulties` parameter should be a `IReadOnlyDictionary<string, float[]>`, 
the string would be song id, and float array for chart constants.<br/>
The float array index should map to difficulty, from 0 to 3: 
EZ, HD, IN, then AT (fill value by 0 if AT difficulty doesn't exist). <br/>
Example difficulty value pair: `{ "Credits.Frums", [1.0f, 8.5f, 14.0f, 0f] }`

For more info, please check the common examples. <br/>
