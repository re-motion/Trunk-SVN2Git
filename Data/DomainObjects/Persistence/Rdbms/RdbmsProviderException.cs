using System;
using System.Runtime.Serialization;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
[Serializable]
public class RdbmsProviderException : StorageProviderException
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public RdbmsProviderException () : this ("An RDBMS exception has occurred.") {}
  public RdbmsProviderException (string message) : base (message) {}
  public RdbmsProviderException (string message, Exception inner) : base (message, inner) {}
  protected RdbmsProviderException (SerializationInfo info, StreamingContext context) : base (info, context) {}

  // methods and properties

}
}
