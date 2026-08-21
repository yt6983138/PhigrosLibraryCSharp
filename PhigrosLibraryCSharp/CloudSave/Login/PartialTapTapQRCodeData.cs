using System.Text.Json.Serialization;

namespace PhigrosLibraryCSharp.CloudSave.Login;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
/// <summary>
/// A partial model for TapTap QR code data, containing only the necessary fields for login.
/// </summary>
public class PartialTapTapQRCodeData
{
	/// <summary>
	/// The core data of the QR code response.
	/// </summary>
	[JsonInclude]
	[JsonPropertyName("data")]
	public QRCodeData Data { get; set; }
	/// <summary>
	/// The core data of the QR code response.
	/// </summary>
	public class QRCodeData
	{
		/// <inheritdoc cref="CompleteQRCodeData.DeviceCode"/>/>
		[JsonInclude]
		[JsonPropertyName("device_code")]
		public string DeviceCode { get; set; }

		/// <inheritdoc cref="CompleteQRCodeData.ExpiresInSeconds"/>/>
		[JsonInclude]
		[JsonPropertyName("expires_in")]
		public int ExpiresIn { get; set; }

		/// <inheritdoc cref="CompleteQRCodeData.Url"/>/>
		[JsonInclude]
		[JsonPropertyName("qrcode_url")]
		public string Url { get; set; }

		/// <inheritdoc cref="CompleteQRCodeData.Interval"/>/>
		[JsonInclude]
		[JsonPropertyName("interval")]
		public int Interval { get; set; }
	}
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
