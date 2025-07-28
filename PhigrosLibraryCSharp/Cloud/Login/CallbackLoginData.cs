namespace PhigrosLibraryCSharp.Cloud.Login;

/// <summary>
/// Data used to store login information for callback login.
/// </summary>
public class CallbackLoginData
{
	/// <summary>
	/// TapTap code verifier used for OAuth 2.0 flow.
	/// </summary>
	public string CodeVerifier { get; init; }
	/// <summary>
	/// URL that will be redirected to when the login is complete. See <see cref="TapTapHelper.HandleCallbackLogin(CallbackLoginData, string, bool)"/>.
	/// </summary>
	public string RedirectUrl { get; init; }
	/// <summary>
	/// State used to maintain state between the request and the callback.
	/// </summary>
	public string State { get; init; }
	/// <summary>
	/// [Unknown]
	/// </summary>
	public string CodeChallenge { get; init; }
	/// <summary>
	/// Scope of the OAuth 2.0 request, usually "public_profile".
	/// </summary>
	public string Scope { get; init; }
	/// <summary>
	/// Url for user to begin the login process.
	/// </summary>
	public string BeginUrl { get; init; }

	internal CallbackLoginData(string codeVerifier, string redirectUrl, string state, string codeChallenge, string scope, string beginUrl)
	{
		this.CodeVerifier = codeVerifier;
		this.RedirectUrl = redirectUrl;
		this.State = state;
		this.CodeChallenge = codeChallenge;
		this.Scope = scope;
		this.BeginUrl = beginUrl;
	}

	/// <summary>
	/// For serialization purposes only
	/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	public CallbackLoginData() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}
