namespace PhigrosLibraryCSharp.UnmanagedWrapper.Structures;
public struct AsyncHandle
{
	public nint Handle;

	public static implicit operator nint(AsyncHandle handle) => handle.Handle;
	public static explicit operator AsyncHandle(nint data) => new() { Handle = data };
	public override readonly int GetHashCode()
	{
		return this.Handle.GetHashCode();
	}
	public override readonly string ToString()
	{
		return this.Handle.ToString();
	}
}
