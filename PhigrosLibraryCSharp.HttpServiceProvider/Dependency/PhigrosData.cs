using Microsoft.Extensions.Options;

namespace PhigrosLibraryCSharp.HttpServiceProvider.Dependency;

public class PhigrosData
{
	public Dictionary<string, string> NameMap { get; set; } = new();
	public Dictionary<string, float[]> DifficultyMap { get; set; } = new();

	private ILogger<PhigrosData> _logger;
	private Config _config;

	public PhigrosData(ILogger<PhigrosData> logger, IOptions<Config> config)
	{
		this._logger = logger;
		this._config = config.Value;

		bool noFile = false;
		if (!File.Exists(this._config.DifficultyMapLocation))
		{
			this._logger.LogCritical("Seems difficulty.tsv(or csv) does not exist. " +
				"Place them at {pos}, which is specified in appsettings.json.",
				this._config.DifficultyMapLocation);
			noFile = true;
		}
		if (!File.Exists(this._config.NameIdMapLocation))
		{
			this._logger.LogCritical("Seems info.tsv(or csv) does not exist. " +
				"Place them at {pos}, which is specified in appsettings.json.",
				this._config.NameIdMapLocation);
			noFile = true;
		}
		if (noFile)
			throw new FileNotFoundException();

		string[] csvFile = File.ReadAllLines(this._config.DifficultyMapLocation);
		char separatorDiff = this._config.DifficultyMapLocation.EndsWith(".tsv", StringComparison.InvariantCultureIgnoreCase) ? '\t' : ',';
		foreach (string line in csvFile)
		{
			try
			{
				float[] diffcultys = new float[4];
				string[] splitted = line.Split(separatorDiff);
				for (byte i = 0; i < splitted.Length; i++)
				{
					if (i > 4 || i == 0) { continue; }
					if (!float.TryParse(splitted[i], out diffcultys[i - 1]))
						this._logger.LogWarning("Error processing {data}", splitted[i]);
				}
				this.DifficultyMap.Add(splitted[0], diffcultys);
			}
			catch (Exception ex)
			{
				this._logger.LogError("Error while reading difficulties csv");
				this._logger.LogError(ex.ToString());
			}
		}

		string[] csvFile2 = File.ReadAllLines(this._config.NameIdMapLocation);
		char separatorName = this._config.NameIdMapLocation.EndsWith(".tsv", StringComparison.InvariantCultureIgnoreCase) ? '\t' : '\\';
		foreach (string line in csvFile2)
		{
			try
			{
				string[] splitted = line.Split(separatorName);
				this.NameMap.Add(splitted[0], splitted[1]);
			}
			catch (Exception ex)
			{
				this._logger.LogError("Error while reading names csv");
				this._logger.LogError(ex.ToString());
			}
		}
	}
}
