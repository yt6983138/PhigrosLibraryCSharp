namespace PhigrosLibraryCSharp.Serialization;

/// <summary>
/// Interface for Phigros custom serialization, which is used for some complex objects that 
/// cannot be serialized/deserialized by <see cref="ByteReader"/> and <see cref="ByteWriter"/> directly.
/// </summary>
/// <typeparam name="TSelf">The type to serialize to and deserialize from. Usually the class type itself.</typeparam>
public interface IPhigrosCustomSerialization<TSelf>
{
	/// <summary>
	/// Constructs an object of type <typeparamref name="TSelf"/> from the given <see cref="ByteReader"/>.
	/// The reader is expected to be at the correct position for reading the object, and should be at the end of the object after reading.
	/// </summary>
	/// <param name="reader">A <see cref="ByteReader"/> to read data from.</param>
	/// <returns>A constructed instance of <typeparamref name="TSelf"/>.</returns>
	static abstract TSelf FromReader(ByteReader reader);
	/// <summary>
	/// Serializes the current object to the given <see cref="ByteWriter"/>. 
	/// The writer is expected to be at the correct position for writing the object, and should be at the end of the object after writing.
	/// 
	/// Note: This may or may not write <see cref="ByteWriter.ObjectVersion"/> to the writer, depending on the implementation.
	/// </summary>
	/// <param name="writer">A <see cref="ByteWriter"/> to write data to.</param>
	void Serialize(ByteWriter writer);
}
