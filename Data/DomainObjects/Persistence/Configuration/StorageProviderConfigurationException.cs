using System;
using System.Runtime.Serialization;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
[Serializable]
public class StorageProviderConfigurationException : ConfigurationException
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public StorageProviderConfigurationException () : this ("A storage provider exception occurred.") {}

  public StorageProviderConfigurationException (string message) : base (message) {}
  public StorageProviderConfigurationException (string message, Exception inner) : base (message, inner) {}
  protected StorageProviderConfigurationException (SerializationInfo info, StreamingContext context) : base (info, context) {}

  // methods and properties

}
}
