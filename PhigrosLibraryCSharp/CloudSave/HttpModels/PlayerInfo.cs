namespace PhigrosLibraryCSharp.CloudSave.HttpModels;

/// <summary>
/// A struct presenting player's information, 
/// which is used in some APIs to present the player who uploaded a save.
/// </summary>
public class PlayerInfo
{
	/// <summary>
	/// The player's nickname. (The name shown in game)
	/// </summary>
	public string NickName { get; set; } = "";
	/// <summary>
	/// The player's username.
	/// </summary>
	public string UserName { get; set; } = "";
	/// <summary>
	/// The player's creation time.
	/// </summary>
	public DateTime CreationTime { get; set; }
	/// <summary>
	/// The last <see cref="PlayerInfo"/> modification time.
	/// </summary>
	public DateTime ModificationTime { get; set; }
	/// <summary>
	/// The player's object ID. Can be used to filter saves uploaded by a specific player.
	/// </summary>
	public string ObjectId { get; set; } = "";
}