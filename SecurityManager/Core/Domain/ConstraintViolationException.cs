using System;
using System.Runtime.Serialization;

namespace Remotion.SecurityManager.Domain
{
  [Serializable]
  public class ConstraintViolationException : SecurityManagerException
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public ConstraintViolationException () : this ("A constraint has been violated.") {}
    public ConstraintViolationException (string message) : base (message) {}
    public ConstraintViolationException (string message, Exception inner) : base (message, inner) {}
    protected ConstraintViolationException (SerializationInfo info, StreamingContext context) : base (info, context) { }

    // methods and properties
  }
}
