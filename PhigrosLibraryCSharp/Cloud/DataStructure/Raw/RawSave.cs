namespace PhigrosLibraryCSharp.Cloud.DataStructure.Raw;

/// <summary>
/// A container containing <see cref="RawSave"/>s.
/// </summary>
public struct RawSaveContainer
{
	/// <summary>
	/// Player's <see cref="RawSave"/>s.
	/// </summary>
	public List<RawSave> results;

	/// <summary>
	/// Get <see cref="SimplifiedSave"/>s by parsing them.
	/// </summary>
	/// <returns>A <see cref="List{T}"/> of <see cref="SimplifiedSave"/>s containing parsed saves.</returns>
	public readonly List<SimplifiedSave> GetParsedSaves()
	{
		List<SimplifiedSave> saves = new();
		foreach (RawSave item in this.results) saves.Add(item.ToParsed());
		return saves;
	}
}
/// <summary>
/// Raw save directly converted from cloud object.
/// </summary>
public class RawSave
{
	// yep i cant remember those too
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	public DateTime createdAt;
	public GameFile gameFile;
	public RawSaveTime modifiedAt;
	public string name = "";
	public string objectId = "";
	public string summary = "";
	public DateTime updatedAt;
	public RawUserInfo user;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

	/// <summary>
	/// Convert to <see cref="SimplifiedSave"/>.
	/// </summary>
	/// <returns>A converted <see cref="SimplifiedSave"/>.</returns>
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
// sorry i cant remember what those are anymore
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
