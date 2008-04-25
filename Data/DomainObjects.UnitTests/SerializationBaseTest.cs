using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Remotion.Data.DomainObjects.UnitTests
{
  public class SerializationBaseTest : ClientTransactionBaseTest
  {
    protected object SerializeAndDeserialize (object graph)
    {
      using (MemoryStream stream = new MemoryStream ())
      {
        Serialize (stream, graph);
        return Deserialize (stream);
      }
    }

    protected void Serialize (Stream stream, object graph)
    {
      BinaryFormatter serializationFormatter = new BinaryFormatter ();
      serializationFormatter.Serialize (stream, graph);
    }

    protected object Deserialize (Stream stream)
    {
      stream.Position = 0;

      BinaryFormatter deserializationFormatter = new BinaryFormatter ();
      return deserializationFormatter.Deserialize (stream);
    }
  }
}
