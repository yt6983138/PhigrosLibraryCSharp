namespace PhigrosLibraryCSharp.Cloud.DataStructure;

/// <summary>
/// A struct presenting player's nickname, username...
/// </summary>
public struct UserInfo
{
	/// <summary>
	/// The player's nickname.
	/// </summary>
	public string NickName { get; set; }
	/// <summary>
	/// The player's username.
	/// </summary>
	public string UserName { get; set; }
	/// <summary>
	/// The player's creation time.
	/// </summary>
	public DateTime CreationTime { get; set; }
	/// <summary>
	/// The last <see cref="UserInfo"/> modification time.
	/// </summary>
	public DateTime ModificationTime { get; set; }
}