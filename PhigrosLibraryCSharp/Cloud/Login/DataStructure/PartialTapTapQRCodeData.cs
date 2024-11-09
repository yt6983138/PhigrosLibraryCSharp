using System.Text.Json.Serialization;

namespace PhigrosLibraryCSharp.Cloud.Login.DataStructure;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
internal class PartialTapTapQRCodeData
{
	[JsonInclude]
	[JsonPropertyName("data")]
	public QRCodeData Data { get; set; }
	internal class QRCodeData
	{
		[JsonInclude]
		[JsonPropertyName("device_code")]
		public string DeviceCode { get; set; }

		[JsonInclude]
		[JsonPropertyName("expires_in")]
		public int ExpiresIn { get; set; }

		[JsonInclude]
		[JsonPropertyName("qrcode_url")]
		public string Url { get; set; }

		[JsonInclude]
		[JsonPropertyName("interval")]
		public int Interval { get; set; }
	}
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
