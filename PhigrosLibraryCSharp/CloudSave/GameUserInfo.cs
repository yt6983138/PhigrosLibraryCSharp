using PhigrosLibraryCSharp.Serialization;

namespace PhigrosLibraryCSharp.CloudSave;

/// <summary>
/// The user's info in game.
/// </summary>
public class GameUserInfo : IPhigrosCustomSerialization<GameUserInfo>
{
	/// <summary>
	/// Gets or sets the version of user info file. Latest: 1.
	/// </summary>
	public byte Version { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the user has the user name expanded.
	/// </summary>
	public bool ShowUserId { get; set; }

	/// <summary>
	/// Gets or sets the user's intro.
	/// </summary>
	public string Intro { get; set; }

	/// <summary>
	/// Gets or sets the user's avatar id.
	/// </summary>
	public string AvatarId { get; set; }

	/// <summary>
	/// Gets or sets the user's background id.
	/// </summary>
	public string BackgroundId { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="GameUserInfo"/> class.
	/// </summary>
	/// <param name="version">The version of user info file.</param>
	/// <param name="showUserId"><see langword="true"/> if user has the user name expanded, otherwise <see langword="false"/>.</param>
	/// <param name="intro">User's intro.</param>
	/// <param name="avatarId">User's avatar id.</param>
	/// <param name="backgroundId">User's background id.</param>
	public GameUserInfo(byte version, bool showUserId, string intro, string avatarId, string backgroundId)
	{
		this.Version = version;
		this.ShowUserId = showUserId;
		this.Intro = intro;
		this.AvatarId = avatarId;
		this.BackgroundId = backgroundId;
	}

	/// <inheritdoc/>
	public static GameUserInfo FromReader(ByteReader reader)
	{
		//string tmp;
		return new(
			reader.ObjectVersion,
			reader.ReadByte() != 0,
			reader.ReadString(),
			//string.IsNullOrWhiteSpace(tmp = reader.ReadString()) ? "Introduction" : tmp,
			reader.ReadString(),
			reader.ReadString());
	}
	/// <inheritdoc/>
	public void Serialize(ByteWriter writer)
	{
		writer.ObjectVersion = this.Version;

		writer.WriteByte((byte)(this.ShowUserId ? 1 : 0));
		writer.WriteString(this.Intro);
		writer.WriteString(this.AvatarId);
		writer.WriteString(this.BackgroundId);
	}
}
