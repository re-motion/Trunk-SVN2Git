using System;
using System.Runtime.Serialization;

namespace Remotion.Data.DomainObjects.UnitTests.EventReceiver
{
  [Serializable]
  public class EventReceiverCancelException : ApplicationException
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public EventReceiverCancelException () : this ("An EventReceiver cancelled the operation.") { }
    public EventReceiverCancelException (string message) : base (message) { }
    public EventReceiverCancelException (string message, Exception inner) : base (message, inner) { }
    protected EventReceiverCancelException (SerializationInfo info, StreamingContext context) : base (info, context) { }

    // methods and properties

  }
}
