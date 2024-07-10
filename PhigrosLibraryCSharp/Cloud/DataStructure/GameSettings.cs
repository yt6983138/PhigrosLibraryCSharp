namespace PhigrosLibraryCSharp.Cloud.DataStructure;
/// <summary>
/// The user's settings in game.
/// </summary>
/// <param name="ChordSupport">[Unknown]</param>
/// <param name="FcApIndicatorOn"><see langword="true"/> if the user has "FC/AP Indicator" on, otherwise <see langword="false"/>.</param>
/// <param name="EnableHitSound"><see langword="true"/> if the user has hitsounds on, otherwise <see langword="false"/>.</param>
/// <param name="LowResolutionModeOn"><see langword="true"/> if the user has "Low Resolution Mode" on, otherwise <see langword="false"/>.</param>
/// <param name="DeviceName">User's phone/tablet device name.</param>
/// <param name="BackgroundBrightness">The player's background brightness, 0 ~ 1.</param>
/// <param name="MusicVolume">The player's music volume, 0 ~ 1.</param>
/// <param name="EffectVolume">The player's effect volume, 0 ~ 1.</param>
/// <param name="HitSoundVolume">The player's hitsound volume, 0 ~ 1.</param>
/// <param name="SoundOffset">The player's sound offset, in seconds.</param>
/// <param name="NoteScale">The player's note scale.</param>
public record class GameSettings(
	bool ChordSupport,
	bool FcApIndicatorOn,
	bool EnableHitSound,
	bool LowResolutionModeOn,
	string DeviceName,
	float BackgroundBrightness,
	float MusicVolume,
	float EffectVolume,
	float HitSoundVolume,
	float SoundOffset,
	float NoteScale);
