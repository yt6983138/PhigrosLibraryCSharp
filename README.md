# PhigrosLibraryCSharp
This is a C# implementation of [PhigrosLibrary](https://github.com/7aGiven/PhigrosLibrary).

# API Usage
## Local save (...PlayerPrefv2.xml)
1. You parse the xml and get the key string and value string ex. `xaHiFItVgoS6CBFNHTR2%2BA%3D%3D`
2. Convert decode it using `System.Net.WebUtility.UrlDecode`, then get string like this (`xaHiFItVgoS6CBFNHTR2+A==`)
3. Decrypt it by static method `LocalSave.DecryptSaveStringNew(string base64EncodedString)` ex. `LocalSave.DecryptSaveStringNew(eSB6QJlXU1vwHjVx7kcpb4/jdk9o5j4Wiatn+jrJ+etI2KFMlPDyH8s7I8zM+qlW)`
4. After it returns data (ex. `MARENOL.LeaF.0.Record.HD` and `{"s":992580,"a":99.17559051513672,"c":1}`) convert it from json to ScoreFormat struct and do ``theStruct.ToInternalFormat()``.
5. do whatever you want to the data
Full example code:
```cs
public static CompleteScore ExampleDecryptLocal(string key, string data, float chartConstant) 
{
	string decryptedKey = LocalSave.DecryptSaveStringNew(System.Net.WebUtility.UrlDecode(key));
	string decryptedData = LocalSave.DecryptSaveStringNew(System.Net.WebUtility.UrlDecode(data));

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
### TapTap integration
[See this method](/PhigrosLibraryCSharp.Examples/Program.cs#L47) for examples on getting in a better way.
### Cloud save
Most examples can be found [there](/PhigrosLibraryCSharp.Examples/Program.cs). <br/>
1. First new a `SaveHelper` object ex. `var helper = new SaveHelper()` (note: if you are using this library in wasm project you need to set Decrypter property ex. `new SaveHelper() { Decrypter = async (byte[] key, byte[] iv, byte[] data) => { /* your decrypting function here */ } }` cuz aes doesnt work in wasm)
```cs
Save save = new(/* phigros token */);
// if you are using it in wasm project then do this:
// save.Decrypter = async (byte[] key, byte[] iv, byte[] data) => { /* your decrypting function here */ };
var context = await save.GetSaveContextAsync(/* save index, 0 for latest */);
context.ReadGameRecord(/* difficulties */).Records // all scores
```
Fore more info, go to [there](/PhigrosLibraryCSharp.Examples/Program.cs)
### Parameter explanation:
`GetSaveContextAsync(int index)` <br/>
Get save context, with save context you can read most thing in the save. <br/>
`index`: The index of the save (0 for latest) <br/>
`ReadGameRecord(IReadOnlyDictionary<string, float[]> difficulties)` <br/>
difficulties is a `IReadOnlyDictionary<string, float[]>` (just pass in a `Dictionary<string, float[]>`), the string is the ID, and the float is the chart constant. <br/>
Elements of the float array: <br/>
0: EZ cc (short name of chart constant)<br/>
1: HD cc <br/>
2: IN cc <br/>
3: AT cc <br/>
### Example code
```cs
public static async Task<(Summary Summary, GameRecord Record)> GetLatestSave(string token, Dictionary<string, float[]> difficulties) 
{
	var helper = new Save(token);
	var context = await helper.GetSaveContextAsync(0);
	return (context.ReadSummary(), context.ReadGameRecord(difficulties));
}
// somewhere else in ur code...
await GetLatestSave("mysupersecrettoken1145141");
```