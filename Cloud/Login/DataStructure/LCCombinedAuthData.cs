using Newtonsoft.Json;

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
	/// <param name="profileData">Profile data gotten from <see cref="TapTapHelper.GetProfile(TapTapTokenData.TokenData, bool, int)"/>.</param>
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

	[JsonProperty("kid")]
	public string Kid { get; set; }

	[JsonProperty("access_token")]
	public string Token { get; set; }

	[JsonProperty("token_type")]
	public string TokenType { get; set; }

	[JsonProperty("mac_key")]
	public string MacKey { get; set; }

	[JsonProperty("mac_algorithm")]
	public string MacAlgorithm { get; set; }

	[JsonProperty("openid")]
	public string OpenID { get; set; }

	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("avatar")]
	public string Avatar { get; set; }

	[JsonProperty("unionid")]
	public string UnionID { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	public Dictionary<string, object> ToDictionary()
	{
		Type typeOfThis = typeof(LCCombinedAuthData);
		System.Reflection.PropertyInfo[] properties = typeOfThis.GetProperties();
		Dictionary<string, object> dict = new();
		foreach (System.Reflection.PropertyInfo property in properties)
		{
			dict.Add(property.Name, property.GetValue(this)!);
		}
		return dict;
	}
}
