using System;
using System.Runtime.Serialization;

namespace Remotion.SecurityManager.Domain.Metadata
{
  [Serializable]
  public class MetadataException : Exception
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public MetadataException () : this ("A metadata exception occurred.") { }
    public MetadataException (string message) : base (message) { }
    public MetadataException (string message, Exception inner) : base (message, inner) { }
    protected MetadataException (SerializationInfo info, StreamingContext context) : base (info, context) { }

    // methods and properties

  }
}
