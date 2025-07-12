using System.Text.Json.Serialization;

namespace PhigrosLibraryCSharp.Cloud.Login.DataStructure;
/// <summary>
/// A combined collection of <see cref="TapTapProfileData.ProfileData"/> and <see cref="TapTapTokenData.TokenData"/>.
/// </summary>
public class LCCombinedAuthData
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	internal LCCombinedAuthData() { }
	/// <summary>
	/// Creates a combined collection of <see cref="TapTapProfileData.ProfileData"/> and <see cref="TapTapTokenData.TokenData"/>.
	/// </summary>
	/// <param name="profileData">Profile data gotten from <see cref="TapTapHelper.GetProfile(TapTapTokenData.TokenData, int, bool)"/>.</param>
	/// <param name="tokenData">Token data gotten from <see cref="TapTapHelper.CheckQRCodeResult(CompleteQRCodeData, bool)"/>.</param>
	public LCCombinedAuthData(TapTapProfileData.ProfileData profileData, TapTapTokenData.TokenData tokenData)
	{
		this.Kid = tokenData.Kid;
		this.Token = tokenData.Token;
		this.TokenType = tokenData.TokenType;
		this.MacKey = tokenData.MacKey;
		this.MacAlgorithm = tokenData.MacAlgorithm;
		this.OpenID = profileData.OpenId;
		this.Name = profileData.Name;
		this.Avatar = profileData.Avatar;
		this.UnionID = profileData.UnionId;
	}

	/// <summary>
	/// The user's kid data. [Unknown]
	/// </summary>
	[JsonPropertyName("kid")]
	public string Kid { get; set; }

	/// <summary>
	/// The user's TapTap access token.
	/// </summary>
	[JsonPropertyName("access_token")]
	public string Token { get; set; }

	/// <summary>
	/// The type of user's TapTapToken.
	/// </summary>
	[JsonPropertyName("token_type")]
	public string TokenType { get; set; }

	/// <summary>
	/// The user's mac key. [Unknown]
	/// </summary>
	[JsonPropertyName("mac_key")]
	public string MacKey { get; set; }

	/// <summary>
	/// The user's mac algorithm. [Unknown]
	/// </summary>
	[JsonPropertyName("mac_algorithm")]
	public string MacAlgorithm { get; set; }

	/// <summary>
	/// The user's open id.
	/// </summary>
	[JsonPropertyName("openid")]
	public string OpenID { get; set; }

	/// <summary>
	/// The user's TapTap name.
	/// </summary>
	[JsonPropertyName("name")]
	public string Name { get; set; }

	/// <summary>
	/// The user's TapTap avatar.
	/// </summary>
	[JsonPropertyName("avatar")]
	public string Avatar { get; set; }

	/// <summary>
	/// The user's union id. 
	/// </summary>
	[JsonPropertyName("unionid")]
	public string UnionID { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	internal Dictionary<string, object> ToDictionary()
	{
		Type typeOfThis = typeof(LCCombinedAuthData);
		System.Reflection.PropertyInfo[] properties = typeOfThis.GetProperties();
		Dictionary<string, object> dict = [];
		foreach (System.Reflection.PropertyInfo property in properties)
		{
			dict.Add(property.Name, property.GetValue(this)!);
		}
		return dict;
	}
}
