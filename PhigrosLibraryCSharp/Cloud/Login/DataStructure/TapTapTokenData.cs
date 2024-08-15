using Newtonsoft.Json;

namespace PhigrosLibraryCSharp.Cloud.Login.DataStructure;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
/// <summary>
/// Token data gotten from <see cref="TapTapHelper.CheckQRCodeResult(PhigrosLibraryCSharp.Cloud.Login.DataStructure.CompleteQRCodeData)"/>.
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
		[JsonProperty("token")]
		private string TokenAlias { set => this.Token = value; }

		/// <summary>
		/// The user's token type.
		/// </summary>
		[JsonProperty("token_type")]
		public string TokenType { get; set; }
		[JsonProperty("tokenType")]
		private string TokenTypeAlias { set => this.TokenType = value; }

		/// <summary>
		/// The user's mac key. [Unknown]
		/// </summary>
		[JsonProperty("mac_key")]
		public string MacKey { get; set; }
		[JsonProperty("macKey")]
		private string MacKeyAlias { set => this.MacKey = value; }

		/// <summary>
		/// The user's mac algorithm. [Unknown]
		/// </summary>
		[JsonProperty("mac_algorithm")]
		public string MacAlgorithm { get; set; }
		[JsonProperty("macAlgorithm")]
		private string MacAlgorithmAlias { set => this.MacAlgorithm = value; }

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
