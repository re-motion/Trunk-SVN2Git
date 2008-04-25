using System;
using System.Runtime.Serialization;

namespace Remotion.Data.DomainObjects.Persistence
{
[Serializable]
public class PersistenceException : DomainObjectException 
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public PersistenceException () : this ("A persistence exception occurred.") {}
  public PersistenceException (string message) : base (message) {}
  public PersistenceException (string message, Exception inner) : base (message, inner) {}
  protected PersistenceException (SerializationInfo info, StreamingContext context) : base (info, context) {}

  // methods and properties

}
}
