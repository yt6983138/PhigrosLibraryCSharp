namespace PhigrosLibraryCSharp.Cloud.Login.DataStructure;
/// <summary>
/// A completed collection of <see cref="PartialTapTapQRCodeData"/>.
/// </summary>
public class CompleteQRCodeData
{
	internal CompleteQRCodeData(PartialTapTapQRCodeData code, string deviceID)
	{
		this.DeviceID = deviceID;
		this.DeviceCode = code.Data.DeviceCode;
		this.ExpiresInSeconds = code.Data.ExpiresIn;
		this.Url = code.Data.Url;
		this.Interval = code.Data.Interval;
	}
	/// <summary>
	/// Random device GUID.
	/// </summary>
	public string DeviceID { get; init; }
	/// <summary>
	/// [Unknown]
	/// </summary>
	public string DeviceCode { get; init; }
	/// <summary>
	/// QRCode expires in this period.
	/// </summary>
	public int ExpiresInSeconds { get; init; }
	/// <summary>
	/// Login url, user accesses this to login.
	/// </summary>
	public string Url { get; init; }
	/// <summary>
	/// [Unknown]
	/// </summary>
	public int Interval { get; init; }
}
