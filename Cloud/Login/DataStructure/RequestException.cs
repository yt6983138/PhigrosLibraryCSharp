using System.Net;

namespace PhigrosLibraryCSharp.Cloud.Login.DataStructure;
/// <summary>
/// An exception that will be thrown to user if server sent unknown response.
/// </summary>
public class RequestException : Exception
{
	internal enum FailingType
	{
		Pending,
		Denied,
		Waiting,
		None,
		Unknown
	}

	internal FailingType Failing { get; set; }
	internal HttpStatusCode HttpStatus { get; init; }
	internal RequestException(string message, HttpStatusCode code, FailingType type)
		: base(message)
	{
		this.Failing = type;
		this.HttpStatus = code;
	}

	/// <summary>
	/// Returns a <see cref="string"/> form of this instance.
	/// </summary>
	/// <returns>A <see cref="string"/> representing this instance.</returns>
	public override string ToString()
	{
		return $"{this.Failing}, {this.HttpStatus}: {this.Message}";
	}
}
