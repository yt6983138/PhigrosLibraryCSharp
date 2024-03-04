namespace PhigrosLibraryCSharp;

public struct RawSaveContainer
{
	public List<RawSave> results;

	public List<SimplifiedSave> GetParsedSaves()
	{
		List<SimplifiedSave> saves = new();
		foreach (var item in this.results) saves.Add(item.ToParsed());
		return saves;
	}
}
public class RawSave
{
	public DateTime createdAt;
	public GameFile gameFile;
	public RawSaveTime modifiedAt;
	public string name = "";
	public string objectId = "";
	public string summary = "";
	public DateTime updatedAt;
	public RawUserInfo user;

	public SimplifiedSave ToParsed()
	{
		return new SimplifiedSave()
		{
			GameSave = new PhiCloudObj()
			{
				Url = this.gameFile.url
			},
			CreationDate = this.createdAt,
			ModificationTime = this.updatedAt,
			Summary = this.summary
		};
	}
}
public struct GameFile
{
	public string __type;
	public string bucket;
	public DateTime createdAt;
	public string key;
	public GameFileMetaData metaData;
	public string mime_type;
	public string name;
	public string objectId;
	public string provider;
	public DateTime updatedAt;
	public string url;
}
public struct GameFileMetaData
{
	public string _checksum;
	public string prefix;
	public int size;
}
public struct RawSaveTime
{
	public string __type;
	public DateTime iso;
}
public struct RawUserInfo
{
	public string __type;
	public string className;
	public string objectId;
}
