using Newtonsoft.Json;

namespace PhigrosLibraryCSharp.Cloud.Login.DataStructure;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
/// <summary>
/// Token data gotten from <see cref="TapTapHelper.CheckQRCodeResult(PhigrosLibraryCSharp.Cloud.Login.DataStructure.CompleteQRCodeData, bool)"/>.
/// </summary>
public class TapTapTokenData
{
	/// <summary>
	/// The data presenting user's token.
	/// </summary>
	[JsonProperty("data")]
	public TokenData Data { get; set; }

	/// <summary>
	/// The data presenting user's token.
	/// </summary>
	public class TokenData
	{
		/// <summary>
		/// The user's kid data. [Unknown]
		/// </summary>
		[JsonProperty("kid")]
		public string Kid { get; set; }

		/// <summary>
		/// The user's token.
		/// </summary>
		[JsonProperty("access_token")]
		public string Token { get; set; }

		/// <summary>
		/// The user's token type.
		/// </summary>
		[JsonProperty("token_type")]
		public string TokenType { get; set; }

		/// <summary>
		/// The user's mac key. [Unknown]
		/// </summary>
		[JsonProperty("mac_key")]
		public string MacKey { get; set; }

		/// <summary>
		/// The user's mac algorithm. [Unknown]
		/// </summary>
		[JsonProperty("mac_algorithm")]
		public string MacAlgorithm { get; set; }

		/// <summary>
		/// The application permission scope.
		/// </summary>
		[JsonProperty("scope")]
		public string Scope { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

		internal Dictionary<string, object> ToDictionary()
		{
			Type typeOfThis = typeof(TokenData);
			System.Reflection.PropertyInfo[] properties = typeOfThis.GetProperties();
			Dictionary<string, object> dict = new();
			foreach (System.Reflection.PropertyInfo property in properties)
			{
				dict.Add(property.Name, property.GetValue(this)!);
			}
			return dict;
		}
	}
}
