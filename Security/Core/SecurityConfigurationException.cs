using System;
using System.Runtime.Serialization;

namespace Remotion.Security
{
  [Serializable]
  public class SecurityConfigurationException : SecurityException
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public SecurityConfigurationException () : this ("An error in the security configuration occurred.") {}
    public SecurityConfigurationException (string message) : base (message) {}
    public SecurityConfigurationException (string message, Exception inner) : base (message, inner) {}
    protected SecurityConfigurationException (SerializationInfo info, StreamingContext context) : base (info, context) { }

    // methods and properties
  }
}
