using Newtonsoft.Json;

namespace PhigrosLibraryCSharp.Cloud.Login.DataStructure;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
internal class PartialTapTapQRCodeData
{
	[JsonProperty("data")]
	internal QRCodeData Data { get; set; }
	internal class QRCodeData
	{
		[JsonProperty("device_code")]
		public string DeviceCode { get; set; }

		[JsonProperty("expires_in")]
		public int ExpiresIn { get; set; }

		[JsonProperty("qrcode_url")]
		public string Url { get; set; }

		[JsonProperty("interval")]
		public int Interval { get; set; }
	}
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
