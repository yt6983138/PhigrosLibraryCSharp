namespace PhigrosLibraryCSharp.PackageExtract;

// i definitely didnt delete the code accidently and had to recover them from dnspy

/// <summary>
/// The level of bundle map filter.
/// </summary>
// Token: 0x0200008F RID: 143
[Flags]
public enum BundleMapFilterLevel
{
	/// <summary>
	/// No track files (TrackFile/...)
	/// </summary>
	// Token: 0x0400047E RID: 1150
	NoTrackFile = 1,
	/// <summary>
	/// No level mods (LevelMod/...)
	/// </summary>
	// Token: 0x0400047F RID: 1151
	NoLevelMod = 2,
	/// <summary>
	/// No detail cover blurs (c8/...)
	/// </summary>
	// Token: 0x04000480 RID: 1152
	NoDetailCoverBlur = 4,
	/// <summary>
	/// No saturn launch music (saturn smth i forgot)
	/// </summary>
	// Token: 0x04000481 RID: 1153
	NoLaunchMusic = 8,
	/// <summary>
	/// No avatars (avatar. ...)
	/// </summary>
	// Token: 0x04000482 RID: 1154
	NoAvatar = 16,
	/// <summary>
	/// No illustrations (Asset/Tracks/.../Illustration.png)
	/// </summary>
	// Token: 0x04000483 RID: 1155
	NoIllustration = 32,
	/// <summary>
	/// No blurry illustrations (Asset/Tracks/.../IllustrationBlur.png)
	/// </summary>
	// Token: 0x04000484 RID: 1156
	NoIllustrationBlur = 64,
	/// <summary>
	/// No low resolution illustrations (Asset/Tracks/.../IllustrationLowRes.png)
	/// </summary>
	// Token: 0x04000485 RID: 1157
	NoIllustrationLowRes = 128,
	/// <summary>
	/// No musics (Asset/Tracks/.../music.wav)
	/// </summary>
	// Token: 0x04000486 RID: 1158
	NoMusic = 256,
	/// <summary>
	/// No EZ charts (Asset/Tracks/.../Chart_EZ.json)
	/// </summary>
	// Token: 0x04000487 RID: 1159
	NoChartEZ = 512,
	/// <summary>
	/// No HD charts (Asset/Tracks/.../Chart_HD.json)
	/// </summary>
	// Token: 0x04000488 RID: 1160
	NoChartHD = 1024,
	/// <summary>
	/// No IN charts (Asset/Tracks/.../Chart_IN.json)
	/// </summary>
	// Token: 0x04000489 RID: 1161
	NoChartIN = 2048,
	/// <summary>
	/// No AT charts (Asset/Tracks/.../Chart_AT.json)
	/// </summary>
	// Token: 0x0400048A RID: 1162
	NoChartAT = 4096,
	/// <summary>
	/// No chapter covers (Asset/Tracks/#ChapterCover/...)
	/// </summary>
	// Token: 0x0400048B RID: 1163
	NoChapterCover = 8192,
	/// <summary>
	/// Combined from: 
	/// NoTrackFile,
	/// NoLevelMod,
	/// NoDetailCoverBlur,
	/// NoLaunchMusic,
	/// NoChapterCover
	/// </summary>
	// Token: 0x0400048C RID: 1164
	NoTrash = 8207,
	/// <summary>
	/// Combined from: 
	/// NoChartEZ,
	/// NoChartHD,
	/// NoChartIN,
	/// NoChartAT
	/// </summary>
	// Token: 0x0400048D RID: 1165
	NoCharts = 7680,
	/// <summary>
	/// Combined from: 
	/// NoIllustration,
	/// NoIllustrationBlur,
	/// NoIllustrationLowRes
	/// </summary>
	// Token: 0x0400048E RID: 1166
	NoIllustrations = 224,
	/// <summary>
	/// Combined from: 
	/// NoIllustrations,
	/// NoCharts,
	/// NoMusic
	/// </summary>
	// Token: 0x0400048F RID: 1167
	NoAssets = 8160
}
