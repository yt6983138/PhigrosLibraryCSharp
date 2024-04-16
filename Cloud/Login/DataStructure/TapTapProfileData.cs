using Newtonsoft.Json;

namespace PhigrosLibraryCSharp.Cloud.Login.DataStructure;

/// <summary>
/// Profile data gotten from <see cref="TapTapHelper.GetProfile(PhigrosLibraryCSharp.Cloud.Login.DataStructure.TapTapTokenData.TokenData, bool, int)"/>.
/// </summary>
public class TapTapProfileData
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	/// <summary>
	/// The data presenting user profile.
	/// </summary>
	[JsonProperty("data")]
	public ProfileData Data { get; set; }
	/// <summary>
	/// The data presenting user profile.
	/// </summary>
	public class ProfileData
	{
		/// <summary>
		/// The user's open id.
		/// </summary>
		[JsonProperty("openid")]
		public string OpenId { get; set; }

		/// <summary>
		/// The user's union id.
		/// </summary>
		[JsonProperty("unionid")]
		public string UnionId { get; set; }

		/// <summary>
		/// The user's name.
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; set; }

		/// <summary>
		/// The user's gender.
		/// </summary>
		[JsonProperty("gender")]
		public string Gender { get; set; }

		/// <summary>
		/// The user's avatar.
		/// </summary>
		[JsonProperty("avatar")]
		public string Avatar { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	}
}
