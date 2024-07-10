#warning This project is work in progress and you should not use it by now

using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace PhigrosLibraryCSharp.PackageExtract;

// i deleted the code accidently and had to recover them from dnspy
// also i will prob refactor this at sometime

/// <summary>
/// A helper class to extract song assets.
/// </summary>
// Token: 0x02000090 RID: 144
public class SongAssetExtractor
{
	/// <summary>
	/// A helper method that can be used to filter out useless bundles.
	/// </summary>
	/// <param name="data">The dictionary containing the datas</param>
	/// <param name="level">The filter level</param>
	/// <returns>Filtered map</returns>
	/// <exception cref="T:System.InvalidOperationException">When user passed a invalid level</exception>
	// Token: 0x06000374 RID: 884 RVA: 0x00016FC8 File Offset: 0x000151C8
	public static Dictionary<string, string> FilterAddressableBundleMap(IDictionary<string, string> data, BundleMapFilterLevel level)
	{
		IEnumerable<KeyValuePair<string, string>> processing = data;
		foreach (BundleMapFilterLevel lvl in allLevels)
		{
			bool flag = (lvl & level) != lvl;
			if (!flag)
			{
				BundleMapFilterLevel bundleMapFilterLevel = lvl;
				BundleMapFilterLevel bundleMapFilterLevel2 = bundleMapFilterLevel;
				if (bundleMapFilterLevel2 <= BundleMapFilterLevel.NoIllustrationLowRes)
				{
					if (bundleMapFilterLevel2 <= BundleMapFilterLevel.NoAvatar)
					{
						switch (bundleMapFilterLevel2)
						{
							case BundleMapFilterLevel.NoTrackFile:
								processing = processing.Where((x) => !x.Value.StartsWith("TrackFile/"));
								break;
							case BundleMapFilterLevel.NoLevelMod:
								processing = processing.Where((x) => !x.Value.StartsWith("LevelMod/"));
								break;
							case BundleMapFilterLevel.NoTrackFile | BundleMapFilterLevel.NoLevelMod:
								goto IL_034E;
							case BundleMapFilterLevel.NoDetailCoverBlur:
								processing = processing.Where((x) => !x.Value.StartsWith("image."));
								break;
							default:
								if (bundleMapFilterLevel2 != BundleMapFilterLevel.NoLaunchMusic)
								{
									if (bundleMapFilterLevel2 != BundleMapFilterLevel.NoAvatar)
										goto IL_034E;
									processing = processing.Where((x) => !x.Value.StartsWith("avatar."));
								}
								else
								{
									processing = processing.Where((x) => !x.Value.StartsWith("music."));
								}
								break;
						}
					}
					else if (bundleMapFilterLevel2 != BundleMapFilterLevel.NoIllustration)
					{
						if (bundleMapFilterLevel2 != BundleMapFilterLevel.NoIllustrationBlur)
						{
							if (bundleMapFilterLevel2 != BundleMapFilterLevel.NoIllustrationLowRes)
								goto IL_034E;
							processing = processing.Where((x) => !x.Value.EndsWith("IllustrationLowRes.png"));
						}
						else
						{
							processing = processing.Where((x) => !x.Value.EndsWith("IllustrationBlur.png"));
						}
					}
					else
					{
						processing = processing.Where((x) => !x.Value.EndsWith("Illustration.png"));
					}
				}
				else if (bundleMapFilterLevel2 <= BundleMapFilterLevel.NoChartHD)
				{
					if (bundleMapFilterLevel2 != BundleMapFilterLevel.NoMusic)
					{
						if (bundleMapFilterLevel2 != BundleMapFilterLevel.NoChartEZ)
						{
							if (bundleMapFilterLevel2 != BundleMapFilterLevel.NoChartHD)
								goto IL_034E;
							processing = processing.Where((x) => !x.Value.EndsWith("Chart_HD.json"));
						}
						else
						{
							processing = processing.Where((x) => !x.Value.EndsWith("Chart_EZ.json"));
						}
					}
					else
					{
						processing = processing.Where((x) => !x.Value.EndsWith("music.wav"));
					}
				}
				else if (bundleMapFilterLevel2 != BundleMapFilterLevel.NoChartIN)
				{
					if (bundleMapFilterLevel2 != BundleMapFilterLevel.NoChartAT)
					{
						if (bundleMapFilterLevel2 != BundleMapFilterLevel.NoChapterCover)
							goto IL_034E;
						processing = processing.Where((x) => !x.Value.Contains("#ChapterCover"));
					}
					else
					{
						processing = processing.Where((x) => !x.Value.EndsWith("Chart_AT.json"));
					}
				}
				else
				{
					processing = processing.Where((x) => !x.Value.EndsWith("Chart_IN.json"));
				}
				goto IL_037C;
			IL_034E:
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new(14, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Invalid level ");
				defaultInterpolatedStringHandler.AppendFormatted(level);
				throw new InvalidOperationException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
		IL_037C:;
		}
		return processing.ToDictionary((x) => x.Key, (x) => x.Value);
	}

	/// <summary>
	/// The obb file of Phigros.
	/// </summary>
	// Token: 0x1700006C RID: 108
	// (get) Token: 0x06000375 RID: 885 RVA: 0x000173A8 File Offset: 0x000155A8
	// (set) Token: 0x06000376 RID: 886 RVA: 0x000173B0 File Offset: 0x000155B0
	public ZipFile ObbFile { get; init; }

	/// <summary>
	/// The catalog data extracted from obb.
	/// </summary>
	// Token: 0x1700006D RID: 109
	// (get) Token: 0x06000377 RID: 887 RVA: 0x000173B9 File Offset: 0x000155B9
	// (set) Token: 0x06000378 RID: 888 RVA: 0x000173C1 File Offset: 0x000155C1
	public IReadOnlyDictionary<string, string> Catalog { get; init; }

	/// <summary>
	/// Decoded bucket data.
	/// </summary>
	// Token: 0x1700006E RID: 110
	// (get) Token: 0x06000379 RID: 889 RVA: 0x000173CA File Offset: 0x000155CA
	// (set) Token: 0x0600037A RID: 890 RVA: 0x000173D2 File Offset: 0x000155D2
	public byte[] BucketData { get; init; }

	/// <summary>
	/// Decoded key data.
	/// </summary>
	// Token: 0x1700006F RID: 111
	// (get) Token: 0x0600037B RID: 891 RVA: 0x000173DB File Offset: 0x000155DB
	// (set) Token: 0x0600037C RID: 892 RVA: 0x000173E3 File Offset: 0x000155E3
	public byte[] KeyData { get; init; }

	/// <summary>
	/// Decoded entry data.
	/// </summary>
	// Token: 0x17000070 RID: 112
	// (get) Token: 0x0600037D RID: 893 RVA: 0x000173EC File Offset: 0x000155EC
	// (set) Token: 0x0600037E RID: 894 RVA: 0x000173F4 File Offset: 0x000155F4
	public byte[] EntryData { get; init; }

	/// <summary>
	/// Initializes a new instance of <see cref="T:PhigrosLibraryCSharp.PackageExtractor.SongAssetExtractor" />.
	/// </summary>
	/// <param name="rawObbOrApk">The obb containing the addressable bundles. 
	/// You can also pass a apk file when the apk is from TapTap. 
	/// (Basically pass *.apk or *.obb that is larger than 1 GiB)
	/// </param>
	// Token: 0x0600037F RID: 895 RVA: 0x00017400 File Offset: 0x00015600
	public SongAssetExtractor(Stream rawObbOrApk)
	{
		this.ObbFile = new ZipFile(rawObbOrApk);
		this.Catalog = (from x in JsonConvert.DeserializeObject<Dictionary<string, object>>(
			new StreamReader(
				this.ObbFile.GetInputStream(
					this.ObbFile.GetEntry("assets/aa/catalog.json"))).ReadToEnd())
						where x.Value is string
						select x).ToDictionary((x) => x.Key, (x) => (string)x.Value);
		this.BucketData = this.Catalog["m_BucketDataString"].AsBase64AndDecode();
		this.EntryData = this.Catalog["m_EntryDataString"].AsBase64AndDecode();
		this.KeyData = this.Catalog["m_KeyDataString"].AsBase64AndDecode();
	}

	/// <summary>
	/// Extract addressable bundle map from the obb.
	/// </summary>
	/// <returns>The addressable bundle map</returns>
	/// <exception cref="T:System.IO.InvalidDataException">When something went wrong while parsing data</exception>
	// Token: 0x06000380 RID: 896 RVA: 0x00017510 File Offset: 0x00015710
	public Dictionary<string, string> ExtractAddressableBundleMap()
	{
		ByteReader bucketReader = new(this.BucketData, 0);
		ByteReader keyReader = new(this.KeyData, 0);
		ByteReader entryReader = new(this.EntryData, 0);
		int dataCount = bucketReader.ReadUnityInt();
		List<ValueTuple<string, int>> rawData = new();
		int count = 0;
		while (count < dataCount)
		{
			int keyPos = bucketReader.ReadUnityInt();
			keyReader.JumpTo(keyPos);
			byte keyValueType = keyReader.ReadByte();
			string stringData;
			switch (keyValueType)
			{
				case 0:
					stringData = keyReader.ReadUnityStringBytes().AsUtf8();
					goto IL_00FC;
				case 1:
					stringData = keyReader.ReadUnityStringBytes().AsUtf16();
					goto IL_00FC;
				case 2:
				case 3:
					goto IL_00B3;
				case 4:
					{
						int discardCount = bucketReader.ReadUnityInt();
						bucketReader.Jump(discardCount * 4);
						break;
					}
				default:
					goto IL_00B3;
			}
		IL_01D0:
			count++;
			continue;
		IL_00FC:
			int entryLength = bucketReader.ReadUnityInt();
			int? entryValue = null;
			for (int entry = 0; entry < entryLength; entry++)
			{
				int entryPos = bucketReader.ReadUnityInt();
				byte[] array = entryReader.ReadEntryByteArray(entryPos);
				entryValue = new int?(array[8] ^ array[9] << 8);
			}
			bool flag = entryValue == null;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
			if (flag)
			{
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Invalid entry length ");
				defaultInterpolatedStringHandler.AppendFormatted(entryLength);
				defaultInterpolatedStringHandler.AppendLiteral(" at offset ");
				defaultInterpolatedStringHandler.AppendFormatted(bucketReader.Offset);
				throw new InvalidDataException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			bool flag2 = entryValue.GetValueOrDefault() == 65535;
			if (flag2)
				goto IL_01D0;
			rawData.Add(new ValueTuple<string, int>(stringData, entryValue!.Value));
			goto IL_01D0;
		IL_00B3:
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 2);
			defaultInterpolatedStringHandler.AppendLiteral("Invalid type ");
			defaultInterpolatedStringHandler.AppendFormatted(keyValueType);
			defaultInterpolatedStringHandler.AppendLiteral(" at position ");
			defaultInterpolatedStringHandler.AppendFormatted(keyReader.Offset);
			throw new InvalidDataException(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		Dictionary<string, string> data = new();
		bool flag3 = rawData.Count % 2 != 0;
		if (flag3)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new(19, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Invalid data count ");
			defaultInterpolatedStringHandler.AppendFormatted(rawData.Count);
			throw new InvalidDataException(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		for (int i = 0; i < rawData.Count; i += 2)
		{
			data.Add(rawData[i + 1].Item1, rawData[i].Item1);
		}
		return data;
	}

	// Token: 0x04000490 RID: 1168
	private static readonly BundleMapFilterLevel[] allLevels = new BundleMapFilterLevel[]
	{
		BundleMapFilterLevel.NoTrackFile,
		BundleMapFilterLevel.NoLevelMod,
		BundleMapFilterLevel.NoDetailCoverBlur,
		BundleMapFilterLevel.NoLaunchMusic,
		BundleMapFilterLevel.NoAvatar,
		BundleMapFilterLevel.NoIllustration,
		BundleMapFilterLevel.NoIllustrationBlur,
		BundleMapFilterLevel.NoIllustrationLowRes,
		BundleMapFilterLevel.NoMusic,
		BundleMapFilterLevel.NoChartEZ,
		BundleMapFilterLevel.NoChartHD,
		BundleMapFilterLevel.NoChartIN,
		BundleMapFilterLevel.NoChartAT,
		BundleMapFilterLevel.NoChapterCover
	};
}
