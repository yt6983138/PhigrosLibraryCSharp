using PhigrosLibraryCSharp.Serialization;

namespace PhigrosLibraryCSharp.CloudSave;

/// <summary>
/// The user's settings in game.
/// </summary>
public class GameSettings : IPhigrosCustomSerialization<GameSettings>
{
	/// <summary>
	/// Gets or sets the version of the settings file. Latest: 1.
	/// </summary>
	public byte Version { get; set; }

	/// <summary>
	/// Gets or sets [Unknown].
	/// </summary>
	public bool ChordSupport { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the user has "FC/AP Indicator" on.
	/// </summary>
	public bool FcApIndicatorOn { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the user has hitsounds on.
	/// </summary>
	public bool EnableHitSound { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the user has "Low Resolution Mode" on.
	/// </summary>
	public bool LowResolutionModeOn { get; set; }

	/// <summary>
	/// Gets or sets the user's phone/tablet device name.
	/// </summary>
	public string DeviceName { get; set; }

	/// <summary>
	/// Gets or sets the player's background brightness, 0 ~ 1.
	/// </summary>
	public float BackgroundBrightness { get; set; }

	/// <summary>
	/// Gets or sets the player's music volume, 0 ~ 1.
	/// </summary>
	public float MusicVolume { get; set; }

	/// <summary>
	/// Gets or sets the player's effect volume, 0 ~ 1.
	/// </summary>
	public float EffectVolume { get; set; }

	/// <summary>
	/// Gets or sets the player's hitsound volume, 0 ~ 1.
	/// </summary>
	public float HitSoundVolume { get; set; }

	/// <summary>
	/// Gets or sets the player's sound offset, in seconds.
	/// </summary>
	public float SoundOffset { get; set; }

	/// <summary>
	/// Gets or sets the player's note scale.
	/// </summary>
	public float NoteScale { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="GameSettings"/> class.
	/// </summary>
	/// <param name="version">Version of the settings file.</param>
	/// <param name="chordSupport">[Unknown]</param>
	/// <param name="fcApIndicatorOn"><see langword="true"/> if the user has "FC/AP Indicator" on, otherwise <see langword="false"/>.</param>
	/// <param name="enableHitSound"><see langword="true"/> if the user has hitsounds on, otherwise <see langword="false"/>.</param>
	/// <param name="lowResolutionModeOn"><see langword="true"/> if the user has "Low Resolution Mode" on, otherwise <see langword="false"/>.</param>
	/// <param name="deviceName">User's phone/tablet device name.</param>
	/// <param name="backgroundBrightness">The player's background brightness, 0 ~ 1.</param>
	/// <param name="musicVolume">The player's music volume, 0 ~ 1.</param>
	/// <param name="effectVolume">The player's effect volume, 0 ~ 1.</param>
	/// <param name="hitSoundVolume">The player's hitsound volume, 0 ~ 1.</param>
	/// <param name="soundOffset">The player's sound offset, in seconds.</param>
	/// <param name="noteScale">The player's note scale.</param>
	public GameSettings(
		byte version,
		bool chordSupport,
		bool fcApIndicatorOn,
		bool enableHitSound,
		bool lowResolutionModeOn,
		string deviceName,
		float backgroundBrightness,
		float musicVolume,
		float effectVolume,
		float hitSoundVolume,
		float soundOffset,
		float noteScale)
	{
		this.Version = version;
		this.ChordSupport = chordSupport;
		this.FcApIndicatorOn = fcApIndicatorOn;
		this.EnableHitSound = enableHitSound;
		this.LowResolutionModeOn = lowResolutionModeOn;
		this.DeviceName = deviceName;
		this.BackgroundBrightness = backgroundBrightness;
		this.MusicVolume = musicVolume;
		this.EffectVolume = effectVolume;
		this.HitSoundVolume = hitSoundVolume;
		this.SoundOffset = soundOffset;
		this.NoteScale = noteScale;
	}

	public static GameSettings FromReader(ByteReader reader)
	{
		return new(
			reader.ObjectVersion,
			reader.ReadFromPackedBoolNoJump(0),
			reader.ReadFromPackedBoolNoJump(1),
			reader.ReadFromPackedBoolNoJump(2),
			reader.ReadFromPackedBoolThenJump(3),
			reader.ReadString(),
			reader.ReadFloat(),
			reader.ReadFloat(),
			reader.ReadFloat(),
			reader.ReadFloat(),
			reader.ReadFloat(),
			reader.ReadFloat());
	}
	public void Serialize(ByteWriter writer)
	{
		writer.ObjectVersion = this.Version;

		writer.WritePackedBools(this.ChordSupport, this.FcApIndicatorOn, this.EnableHitSound, this.LowResolutionModeOn);
		writer.WriteString(this.DeviceName);
		writer.WriteFloat(this.BackgroundBrightness);
		writer.WriteFloat(this.MusicVolume);
		writer.WriteFloat(this.EffectVolume);
		writer.WriteFloat(this.HitSoundVolume);
		writer.WriteFloat(this.SoundOffset);
		writer.WriteFloat(this.NoteScale);
	}
}
