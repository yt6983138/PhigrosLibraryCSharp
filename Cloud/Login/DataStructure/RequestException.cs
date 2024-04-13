using System.Net;

namespace PhigrosLibraryCSharp.Cloud.Login.DataStructure;
internal class RequestException : Exception
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

	public override string ToString()
	{
		return $"{this.Failing}, {this.HttpStatus}: {this.Message}";
	}
}
