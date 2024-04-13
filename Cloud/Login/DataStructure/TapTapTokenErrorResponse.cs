using Newtonsoft.Json;

namespace PhigrosLibraryCSharp.Cloud.Login.DataStructure;
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