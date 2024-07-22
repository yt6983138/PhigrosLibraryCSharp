namespace PhigrosLibraryCSharp.HttpServiceProvider.Test;

[TestClass]
public class LocalSaveTest
{
	public const string JsonType = "application/json";

	public static short Port { get; set; } = 5015;
	public static string Host { get; set; } = "127.0.0.1";
	public HttpClient HttpClient { get; set; } = new();

	public string Template { get; set; } = $"http://{Host}:{Port}/api/LocalSave";

	public string EndPointDecrypt => $"{this.Template}/DecryptNew";

	[TestMethod]
	public async Task TestDecrypt()
	{
		Console.WriteLine(await this.HttpClient.PostWithStringAsContentAsync(this.EndPointDecrypt, "xaHiFItVgoS6CBFNHTR2%2BA%3D%3D", JsonType));
	}
}
