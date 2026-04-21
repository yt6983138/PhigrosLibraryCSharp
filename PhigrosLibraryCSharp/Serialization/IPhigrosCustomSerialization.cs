namespace PhigrosLibraryCSharp.Serialization;
public interface IPhigrosCustomSerialization<TSelf>
{
	static abstract TSelf FromReader(ByteReader reader);
	void Serialize(ByteWriter writer);
}
