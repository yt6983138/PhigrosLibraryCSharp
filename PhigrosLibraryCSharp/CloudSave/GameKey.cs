using PhigrosLibraryCSharp.Serialization;
using System.Text;

namespace PhigrosLibraryCSharp.CloudSave;

/// <summary>
/// The user's game key, which is used to store illustration, avatar, collection unlock state and other various
/// data.
/// </summary>
public class GameKey : IPhigrosCustomSerialization<GameKey>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="GameKey"/> class.
	/// </summary>
	/// <param name="version">The version of the game key.</param>
	/// <param name="keys">A dictionary containing the user's game keys and their corresponding flags.</param>
	/// <param name="lanotaReadKeys">The flag of Lanota read state.</param>
	/// <param name="node2">The second version of the game key node.</param>
	public GameKey(byte version, Dictionary<string, GameKeyFlag> keys, byte lanotaReadKeys, GameKeyNodeVersion2? node2)
	{
		this.Version = version;
		this.Keys = keys;
		this.LanotaReadKeys = lanotaReadKeys;
		this.Node2 = node2;
	}

	/// <summary>
	/// The version of the game key. Latest: 1.
	/// </summary>
	public byte Version { get; set; }

	/// <summary>
	/// A dictionary containing the user's game keys and their corresponding flags.
	/// Known keys: Song's name, Avatar's name (some have number suffix variants merged into a single key), 
	/// Collection's id.
	/// </summary>
	public Dictionary<string, GameKeyFlag> Keys { get; set; }

	/// <summary>
	/// The flag of Lanota read state. Only first 6 bits are used, maps
	/// to collection keys <c>Lanota{0}</c> where {0} is the bit positions. Local key: <c>ReadLanota{0}</c> where {0} is 0 ~ 5.
	/// </summary>
	public byte LanotaReadKeys { get; set; }

	/// <summary>
	/// The second version of the game key node.
	/// </summary>
	public GameKeyNodeVersion2? Node2 { get; set; }

	/// <inheritdoc/>
	public static GameKey FromReader(ByteReader reader)
	{
		short entryCount = reader.ReadVariedInteger();
		Dictionary<string, GameKeyFlag> dict = [];
		for (int i = 0; i < entryCount; i++)
		{
			byte stringLength = reader.ReadByte();
			string key = reader.ReadStringCustomLength(stringLength);
			dict.Add(key, GameKeyFlag.FromReader(reader));
		}

		return new(
			reader.ObjectVersion,
			dict,
			reader.ReadByte(),
			!reader.HasMore ? null : new(
				reader.ReadByte() != 0,
				!reader.HasMore ? null : new(
					reader.ReadByte() != 0,
					reader.ReadByte() != 0)));
	}
	/// <inheritdoc/>
	public void Serialize(ByteWriter writer)
	{
		writer.ObjectVersion = this.Version;

		writer.WriteVariedInteger(checked((short)this.Keys.Count));
		foreach (KeyValuePair<string, GameKeyFlag> item in this.Keys)
		{
			byte[] encoded = Encoding.UTF8.GetBytes(item.Key);

			writer.WriteByte(checked((byte)encoded.Length));
			writer.WriteBytes(encoded);
			item.Value.Serialize(writer);
		}

		writer.WriteByte(this.LanotaReadKeys);

		if (this.Node2 is null) return;
		writer.WriteBool(this.Node2.CamelliaReadKey);

		if (this.Node2.Node3 is null) return;
		writer.WriteBool(this.Node2.Node3.SideStory4BeginReadKey);
		writer.WriteBool(this.Node2.Node3.OldScoreClearedV390);
	}
}

/// <summary>
/// The second revision of the game key file.
/// </summary>
public class GameKeyNodeVersion2
{
	/// <summary>
	/// Initializes a new instance of the <see cref="GameKeyNodeVersion2"/> class.
	/// </summary>
	/// <param name="camelliaReadKey">Whether <c>bassareusEgg</c> collection has been read or not.</param>
	/// <param name="node3">The third version of the game key node.</param>
	public GameKeyNodeVersion2(bool camelliaReadKey, GameKeyNodeVersion3? node3)
	{
		this.CamelliaReadKey = camelliaReadKey;
		this.Node3 = node3;
	}

	/// <summary>
	/// Whether <c>bassareusEgg</c> collection has been read or not. Local key: <c>ReadbassareusEgg</c>
	/// </summary>
	public bool CamelliaReadKey { get; set; }

	/// <summary>
	/// The third version of the game key node.
	/// </summary>
	public GameKeyNodeVersion3? Node3 { get; set; }
}

/// <summary>
/// The third revision of the game key file.
/// </summary>
public class GameKeyNodeVersion3
{
	/// <summary>
	/// Initializes a new instance of the <see cref="GameKeyNodeVersion3"/> class.
	/// </summary>
	/// <param name="sideStory4BeginReadKey">Whether <c>investigatewuxiang</c> collection has been read or not.</param>
	/// <param name="oldScoreClearedV390">Check if the scores have been cleared correctly.</param>
	public GameKeyNodeVersion3(bool sideStory4BeginReadKey, bool oldScoreClearedV390)
	{
		this.SideStory4BeginReadKey = sideStory4BeginReadKey;
		this.OldScoreClearedV390 = oldScoreClearedV390;
	}

	/// <summary>
	/// Whether <c>investigatewuxiang</c> collection has been read or not. Local key: <c>ReadSideStory4Begin</c>
	/// </summary>
	public bool SideStory4BeginReadKey { get; set; }

	/// <summary>
	/// Some of the scores were bugged in version 3.9.0, 
	/// this is used to check if the scores have been cleared correctly.  Local key: <c>IsOldScoreClearedV390</c>
	/// The following score were cleared:
	/// <code>
	/// BonusTime.MegaloPaleWhite.0: EZ, HD
	/// EnginexStartmelodymix.CrossingSound.0: EZ, HD
	/// FULiAUTOSHOOTER.MYUKKE.0: EZ, HD
	/// Glaciaxion.SunsetRay.0: HD
	/// HumaN.SOTUI.0: EZ, HD
	/// Orthodox.tokiwa.0: EZ, HD
	/// PixelRebelz.Normal1zer.0: EZ, HD
	/// PRAW.Bluewind.0: EZ, HD
	/// Wintercube.CtymaxfeatNceS.0: EZ, HD
	/// 混乱Confusion.OnlyMyBlackScore.0: EZ, HD
	/// </code>
	/// </summary>
	public bool OldScoreClearedV390 { get; set; }
}
