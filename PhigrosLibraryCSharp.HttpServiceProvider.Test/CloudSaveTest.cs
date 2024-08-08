using System.IO.Compression;
using System.Text;

namespace PhigrosLibraryCSharp.HttpServiceProvider.Test;

[TestClass]
public class CloudSaveTest
{
	public const string JsonType = "application/json";
	public const string ZipType = "application/zip";
	public const string TextType = "text/plain";

	public static short Port { get; set; } = 5015;
	public static string Host { get; set; } = "127.0.0.1";
	public HttpClient HttpClient { get; set; } = new();

	public string Template { get; set; } = $"http://{Host}:{Port}/api/CloudSave";
	public string Token => Secret.PhigrosToken;

	public string EndPointUserInfo => $"{this.Template}/GetGameUserInfo";
	public string EndPointSaveIndexes => $"{this.Template}/GetSaveIndexes";
	public string EndPointGameSettings => $"{this.Template}/GetGameSettings";
	public string EndPointGameProgress => $"{this.Template}/GetGameProgress";
	public string EndPointDecryptedZip => $"{this.Template}/GetDecryptedZip";
	public string EndPointSaveAndSummary => $"{this.Template}/GetSaveAndSummary";

	[TestMethod]
	public async Task TestUserInfo()
	{
		Console.WriteLine(await this.HttpClient.PostWithStringAsContentAsync(this.EndPointUserInfo, this.Token, TextType));
	}
	[TestMethod]
	public async Task TestSaveIndexes()
	{
		Console.WriteLine(await this.HttpClient.PostWithStringAsContentAsync(this.EndPointSaveIndexes, this.Token, TextType));
	}
	[TestMethod]
	public async Task TestGameSettings()
	{
		Console.WriteLine(await this.HttpClient.PostWithStringAsContentAsync(this.EndPointGameSettings, this.Token, TextType));
	}
	[TestMethod]
	public async Task TestGameProgress()
	{
		Console.WriteLine(await this.HttpClient.PostWithStringAsContentAsync(this.EndPointGameProgress, this.Token, TextType));
	}
	[TestMethod]
	public async Task TestSaveAndSummary()
	{
		Console.WriteLine(await this.HttpClient.PostWithStringAsContentAsync(this.EndPointSaveAndSummary, this.Token, TextType));
	}
	[TestMethod]
	public async Task TestDecryptedZip()
	{
		HttpContent content = (await this.HttpClient.PostAsync(
			this.EndPointDecryptedZip, new StringContent(this.Token, Encoding.UTF8, ZipType))).Content;

		Stream raw = content.ReadAsStream();
		using ZipArchive zip = new(raw, ZipArchiveMode.Read);
		IEnumerable<string> filenames = zip.Entries.Select(x => x.Name);
		Console.WriteLine($"Zip contains: {string.Join(", ", filenames)}");
		Assert.IsTrue(filenames.Contains("gameProgress"));
		Assert.IsTrue(filenames.Contains("gameKey"));
		Assert.IsTrue(filenames.Contains("gameRecord"));
		Assert.IsTrue(filenames.Contains("settings"));
		Assert.IsTrue(filenames.Contains("user"));
	}
}
