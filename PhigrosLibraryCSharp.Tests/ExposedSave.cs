using PhigrosLibraryCSharp.CloudSave;

namespace PhigrosLibraryCSharp.Tests;
public class ExposedSave : Save
{
	public ExposedSave(string sessionToken, bool isInternational) : base(sessionToken, isInternational)
	{
	}

	public string? CachedUserId { get => this._userObjectId; set => this._userObjectId = value; }
	public HttpRequestMessage CreateDefaultRequest(HttpMethod method, string url)
		=> this.CreateDefaultMessage(method, url);
}
