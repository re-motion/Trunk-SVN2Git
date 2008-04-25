using System;
using System.Runtime.Serialization;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  [Serializable]
  public class AccessControlException : Exception
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public AccessControlException () : this ("An access control exception occurred.") { }
    public AccessControlException (string message) : base (message) { }
    public AccessControlException (string message, Exception inner) : base (message, inner) { }
    protected AccessControlException (SerializationInfo info, StreamingContext context) : base (info, context) { }

    // methods and properties

  }
}
