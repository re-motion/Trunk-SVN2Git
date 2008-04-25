using System;
using System.Runtime.Serialization;

namespace Remotion.Security
{
  [Serializable]
  public class SecurityException : Exception
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public SecurityException () : this ("An error occurred in the security system.") {}
    public SecurityException (string message) : base (message) {}
    public SecurityException (string message, Exception inner) : base (message, inner) {}
    protected SecurityException (SerializationInfo info, StreamingContext context) : base (info, context) { }

    // methods and properties
  }
}
