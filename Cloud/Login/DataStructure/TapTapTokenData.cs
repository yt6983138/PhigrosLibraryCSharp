using Newtonsoft.Json;

namespace PhigrosLibraryCSharp.Cloud.Login.DataStructure;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
/// <summary>
/// Token data gotten from <see cref="TapTapHelper.CheckQRCodeResult(PhigrosLibraryCSharp.Cloud.Login.DataStructure.CompleteQRCodeData, bool)"/>.
/// </summary>
public class TapTapTokenData
{
	[JsonProperty("data")]
	public TokenData Data { get; set; }

	public class TokenData
	{
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

		[JsonProperty("scope")]
		public string Scope { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

		public Dictionary<string, object> ToDictionary()
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
