using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Mvc;
using PhigrosLibraryCSharp.Cloud.DataStructure;
using PhigrosLibraryCSharp.HttpServiceProvider.Dependency;

namespace PhigrosLibraryCSharp.HttpServiceProvider.Controllers;
public class CloudSaveController : Controller
{
	public static Dictionary<string, Save> TokenSaveCache { get; set; } = new();

	private ILogger<CloudSaveController> _logger;
	private PhigrosData _phigrosData;

	public CloudSaveController(ILogger<CloudSaveController> logger, PhigrosData data)
	{
		this._logger = logger;
		this._phigrosData = data;
	}

	private async Task<Save> GetSaveOrAdd(string token)
	{
		if (TokenSaveCache.TryGetValue(token, out Save? save))
			return save;

		Save newSave = new(token); // will throw if invalid token
		await newSave.GetUserInfoAsync();
		this._logger.LogInformation("Adding token {token} to cache", token);
		TokenSaveCache.Add(token, newSave);
		return newSave;
	}
	private async Task<(IActionResult?, Save?)> GetSaveAndHandleError(string token)
	{
		Save save;
		try
		{
			save = await this.GetSaveOrAdd(token);
		}
		catch (ArgumentException argEx)
		{
			return (this.Error(this._logger, argEx.Message, code: ErrorCode.PhigrosTokenError), null);
		}
		catch (HttpRequestException argEx)
		{
			return (this.Error(this._logger, argEx.Message, code: ErrorCode.PhigrosTokenError), null);
		}
		catch (Exception ex)
		{
			return (this.Error(this._logger, ex.Message, code: ErrorCode.PhigrosLibraryInternalError), null);
		}

		return (null, save);
	}

	[HttpPost]
	[Route("api/[controller]/GetSaveIndexes")]
	public async Task<IActionResult> GetSaveIndexes()
	{
		(IActionResult? action, Save? save) = await this.GetSaveAndHandleError(await this.Request.ReadBodyAsString());
		if (save is null)
			return action!;

		List<object> timeList = new();
		try
		{
			Cloud.DataStructure.Raw.RawSaveContainer container = await save.GetRawSaveFromCloudAsync();
			int index = 0;
			foreach (Cloud.DataStructure.Raw.RawSave item in container.results)
			{
				timeList.Add(new { ModificationTime = item.modifiedAt.iso, Index = index });
				index++;
			}
		}
		catch (Exception ex)
		{
			return this.Error(this._logger, ex.Message, code: ErrorCode.PhigrosLibraryInternalError);
		}

		this._logger.LogInformation("{ip} requested save index successfully.", this.Request.HttpContext.GetIP());
		return this.Json(timeList);
	}

	[HttpPost]
	[Route("api/[controller]/GetGameUserInfo/{id=0}")]
	public async Task<IActionResult> GetGameUserInfo(int id)
	{
		(IActionResult? action, Save? save) = await this.GetSaveAndHandleError(await this.Request.ReadBodyAsString());
		if (save is null)
			return action!;

		GameUserInfo userInfo;
		try
		{
			userInfo = await save.GetGameUserInfoAsync(id);
		}
		catch (ArgumentOutOfRangeException ex)
		{
			return this.Error(this._logger, ex.Message, code: ErrorCode.ArgumentOutOfRange);
		}
		catch (Exception ex)
		{
			return this.Error(this._logger, ex.Message, code: ErrorCode.PhigrosLibraryInternalError);
		}

		this._logger.LogInformation("{ip} requested game user info successfully.", this.Request.HttpContext.GetIP());
		return this.Json(userInfo);
	}

	[HttpPost]
	[Route("api/[controller]/GetGameSettings/{id=0}")]
	public async Task<IActionResult> GetGameSettings(int id)
	{
		(IActionResult? action, Save? save) = await this.GetSaveAndHandleError(await this.Request.ReadBodyAsString());
		if (save is null)
			return action!;

		GameSettings data;
		try
		{
			data = await save.GetGameSettingsAsync(id);
		}
		catch (ArgumentOutOfRangeException ex)
		{
			return this.Error(this._logger, ex.Message, code: ErrorCode.ArgumentOutOfRange);
		}
		catch (Exception ex)
		{
			return this.Error(this._logger, ex.Message, code: ErrorCode.PhigrosLibraryInternalError);
		}

		this._logger.LogInformation("{ip} requested game settings successfully.", this.Request.HttpContext.GetIP());
		return this.Json(data);
	}

	[HttpPost]
	[Route("api/[controller]/GetGameProgress/{id=0}")]
	public async Task<IActionResult> GetGameProgress(int id)
	{
		(IActionResult? action, Save? save) = await this.GetSaveAndHandleError(await this.Request.ReadBodyAsString());
		if (save is null)
			return action!;

		GameProgress data;
		try
		{
			data = await save.GetGameProgressAsync(id);
		}
		catch (ArgumentOutOfRangeException ex)
		{
			return this.Error(this._logger, ex.Message, code: ErrorCode.ArgumentOutOfRange);
		}
		catch (Exception ex)
		{
			return this.Error(this._logger, ex.Message, code: ErrorCode.PhigrosLibraryInternalError);
		}

		this._logger.LogInformation("{ip} requested game progress successfully.", this.Request.HttpContext.GetIP());
		return this.Json(data);
	}

	[HttpPost]
	[Route("api/[controller]/GetDecryptedZip/{id=0}")]
	public async Task<IActionResult> GetDecryptedZip(int id)
	{
		(IActionResult? action, Save? save) = await this.GetSaveAndHandleError(await this.Request.ReadBodyAsString());
		if (save is null)
			return action!;

		using MemoryStream newStream = new();
		using ZipOutputStream newZip = new(newStream);
		try
		{
			byte[] d = await save.GetSaveRawZipAsync((await save.GetRawSaveFromCloudAsync()).GetParsedSaves()[id]);
			using ZipFile rawZip = new(new MemoryStream(d));

			foreach (ZipEntry entry in rawZip)
			{
				byte[] raw = new byte[entry.Size];
				using Stream entryStream = rawZip.GetInputStream(entry);
				entryStream.Read(raw);
				byte[] decrypted = await save.Decrypt(raw[1..]);
				ZipEntry newEntry = new(entry.Name)
				{
					Size = decrypted.Length
				};
				newZip.PutNextEntry(newEntry);
				newZip.Write(decrypted);
			}
			
			newZip.Close();
		}
		catch (ArgumentOutOfRangeException ex)
		{
			return this.Error(this._logger, ex.Message, code: ErrorCode.ArgumentOutOfRange);
		}
		catch (Exception ex)
		{
			return this.Error(this._logger, ex.Message, code: ErrorCode.PhigrosLibraryInternalError);
		}

		this._logger.LogInformation("{ip} requested decrypted zip successfully.", this.Request.HttpContext.GetIP());
		return this.File(newStream.ToArray(), "application/zip");
	}

	[HttpPost]
	[Route("api/[controller]/GetSaveAndSummary/{id=0}")]
	public async Task<IActionResult> GetSaveAndSummary(int id)
	{
		(IActionResult? action, Save? save) = await this.GetSaveAndHandleError(await this.Request.ReadBodyAsString());
		if (save is null)
			return action!;

		Summary summary;
		GameSave gameSave;

		object converted;
		try
		{
			(summary, gameSave) = await save.GetGameSaveAsync(this._phigrosData.DifficultyMap, id);

			converted = new
			{
				Summary = summary,
				GameSave = new
				{
					gameSave.CreationDate,
					gameSave.ModificationTime,
					Records = gameSave.Records.Select(r => r.Export(this._phigrosData.NameMap[r.Name]))
				}
			};
		}
		catch (ArgumentOutOfRangeException ex)
		{
			return this.Error(this._logger, ex.Message, code: ErrorCode.ArgumentOutOfRange);
		}
		catch (Exception ex)
		{
			return this.Error(this._logger, ex.Message, code: ErrorCode.PhigrosLibraryInternalError);
		}

		this._logger.LogInformation("{ip} requested summary and save successfully.", this.Request.HttpContext.GetIP());
		return this.Json(converted);
	}
}
