namespace PhigrosLibraryCSharp.GameRecords;

/// <summary>
/// The user's game key, which is used to store illustration, avatar, collection unlock state and other various
/// data.
/// </summary>
/// <param name="Keys">A dictionary containing the user's game keys and their corresponding flags.
/// Known keys: Song's name, Avatar's name (some have number suffix variants merged into a single key), 
/// Collection's id.</param>
/// <param name="LanotaReadKeys">The flag of Lanota read state. Only first 6 bits are used, maps
/// to collection keys <c>Lanota{0}</c> where {0} is the bit positions. Local key: <c>ReadLanota{0}</c> where {0} is 0 ~ 5.</param>
/// <param name="Node2">The second version of the game key node.</param>
public record class GameKey(
	Dictionary<string, GameKeyFlag> Keys,
	byte LanotaReadKeys,
	GameKeyNodeVersion2? Node2);
/// <summary>
/// The second revision of the game key file.
/// </summary>
/// <param name="CamelliaReadKey">Whether <c>bassareusEgg</c> collection has been read or not. Local key: <c>ReadbassareusEgg</c></param>
/// <param name="Node3">The third version of the game key node.</param>
public record class GameKeyNodeVersion2(bool CamelliaReadKey, GameKeyNodeVersion3? Node3);
/// <summary>
/// The third revision of the game key file.
/// </summary>
/// <param name="SideStory4BeginReadKey">Whether <c>investigatewuxiang</c> collection has been read or not. Local key: <c>ReadSideStory4Begin</c></param>
/// <param name="OldScoreClearedV390">Some of the scores were bugged in version 3.9.0, 
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
/// </code></param>
public record class GameKeyNodeVersion3(bool SideStory4BeginReadKey, bool OldScoreClearedV390);
