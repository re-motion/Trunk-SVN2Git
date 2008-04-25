using Remotion.Data.DomainObjects;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>The storage class options available in the persistence framework.</summary>
  public enum StorageClass
  {
    /// <summary>The property is persistet into the data store.</summary>
    Persistent,
    /// <summary>The property is managed by the <see cref="ClientTransaction"/>, but not persistet.</summary>
    Transaction,
    /// <summary>The property is ignored by the persistence framework.</summary>
    None
  }
}