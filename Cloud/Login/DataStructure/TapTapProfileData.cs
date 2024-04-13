using Newtonsoft.Json;

namespace PhigrosLibraryCSharp.Cloud.Login.DataStructure;

/// <summary>
/// Profile data gotten from <see cref="TapTapHelper.GetProfile(PhigrosLibraryCSharp.Cloud.Login.DataStructure.TapTapTokenData.TokenData, bool, int)"/>.
/// </summary>
public class TapTapProfileData
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	[JsonProperty("data")]
	public ProfileData Data { get; set; }
	public class ProfileData
	{
		[JsonProperty("openid")]
		public string OpenId { get; set; }

		[JsonProperty("unionid")]
		public string UnionId { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("gender")]
		public string Gender { get; set; }

		[JsonProperty("avatar")]
		public string Avatar { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	}
}
