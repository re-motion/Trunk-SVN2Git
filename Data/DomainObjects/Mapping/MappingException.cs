using System;
using System.Runtime.Serialization;

namespace Remotion.Data.DomainObjects.Mapping
{
[Serializable]
public class MappingException : ConfigurationException
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public MappingException () : this ("A mapping exception occurred.") {}
  public MappingException (string message) : base (message) {}
  public MappingException (string message, Exception inner) : base (message, inner) {}
  protected MappingException (SerializationInfo info, StreamingContext context) : base (info, context) {}

  // methods and properties

}
}
