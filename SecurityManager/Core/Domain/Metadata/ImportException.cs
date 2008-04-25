using System;
using System.Runtime.Serialization;

namespace Remotion.SecurityManager.Domain.Metadata
{
  [Serializable]
  public class ImportException : Exception
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public ImportException () : this ("A metadata import exception occurred.") { }
    public ImportException (string message) : base (message) { }
    public ImportException (string message, Exception inner) : base (message, inner) { }
    protected ImportException (SerializationInfo info, StreamingContext context) : base (info, context) { }

    // methods and properties

  }
}
