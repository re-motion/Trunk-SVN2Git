using System;
using System.Runtime.Serialization;

namespace Remotion.Data.DomainObjects
{
/// <summary>
/// BaseClass for exceptions that are related to the configuraton of the persistence framework.
/// </summary>
[Serializable]
public class ConfigurationException : DomainObjectException
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public ConfigurationException () : this ("A configuration exception occurred.") {}
  public ConfigurationException (string message) : base (message) {}
  public ConfigurationException (string message, Exception inner) : base (message, inner) {}
  protected ConfigurationException (SerializationInfo info, StreamingContext context) : base (info, context) {}

  // methods and properties

}
}
