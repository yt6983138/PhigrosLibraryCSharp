namespace PhigrosLibraryCSharp;

/// <summary>
/// Throw when a argument is out of range (where getting max value is possible)
/// </summary>
public class MaxValueArgumentOutOfRangeException : ArgumentOutOfRangeException
{
	internal MaxValueArgumentOutOfRangeException()
		: base()
	{
	}
	internal MaxValueArgumentOutOfRangeException(string? paramName, object? actualValue, string? message = null)
		: base(paramName, actualValue, message)
	{
	}
	internal MaxValueArgumentOutOfRangeException(string? paramName, object? actualValue, object? maxValue)
		: base(paramName, actualValue, null)
	{
		this.MaxValue = maxValue;
	}

	/// <summary>
	/// The max value that the method accepts.
	/// </summary>
	public object? MaxValue { get; }
}
