using System;
using System.Runtime.Serialization;

namespace Remotion.Data.DomainObjects.Persistence
{
[Serializable]
public class ConverterException : PersistenceException
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public ConverterException () : this ("A converter exception occurred.") {}
  public ConverterException (string message) : base (message) {}
  public ConverterException (string message, Exception inner) : base (message, inner) {}
  protected ConverterException (SerializationInfo info, StreamingContext context) : base (info, context) {}

  // methods and properties

}
}
