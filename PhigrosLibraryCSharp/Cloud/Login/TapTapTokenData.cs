using System.Text.Json.Serialization;

namespace PhigrosLibraryCSharp.Cloud.Login;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
/// <summary>
/// Token data gotten from <see cref="TapTapHelper.CheckQRCodeResult(CompleteQRCodeData, bool)"/>.
/// </summary>
public class TapTapTokenData
{
	/// <summary>
	/// The data presenting user's token.
	/// </summary>
	[JsonPropertyName("data")]
	public TokenData Data { get; set; }

	/// <summary>
	/// The data presenting user's token.
	/// </summary>
	public class TokenData
	{
		/// <summary>
		/// The user's kid data. [Unknown]
		/// </summary>
		[JsonPropertyName("kid")]
		public string Kid { get; set; }

		/// <summary>
		/// The user's token.
		/// </summary>
		[JsonPropertyName("access_token")]
		public string Token { get; set; }
		[JsonPropertyName("token")]
		private string TokenAlias { set => this.Token = value; }

		/// <summary>
		/// The user's token type.
		/// </summary>
		[JsonPropertyName("token_type")]
		public string TokenType { get; set; }
		[JsonPropertyName("tokenType")]
		private string TokenTypeAlias { set => this.TokenType = value; }

		/// <summary>
		/// The user's mac key. [Unknown]
		/// </summary>
		[JsonPropertyName("mac_key")]
		public string MacKey { get; set; }
		[JsonPropertyName("macKey")]
		private string MacKeyAlias { set => this.MacKey = value; }

		/// <summary>
		/// The user's mac algorithm. [Unknown]
		/// </summary>
		[JsonPropertyName("mac_algorithm")]
		public string MacAlgorithm { get; set; }
		[JsonPropertyName("macAlgorithm")]
		private string MacAlgorithmAlias { set => this.MacAlgorithm = value; }

		/// <summary>
		/// The application permission scope.
		/// </summary>
		[JsonPropertyName("scope")]
		public string Scope { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	}
}
