using Newtonsoft.Json;

namespace PhigrosLibraryCSharp.Cloud.Login.DataStructure;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
internal class TapTapTokenErrorResponse
{
	[JsonProperty("data")]
	internal ErrorData Data { get; set; }
	internal class ErrorData
	{
		[JsonProperty("code")]
		public int Code { get; set; }

		[JsonProperty("error")]
		public string Error { get; set; }
	}
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.