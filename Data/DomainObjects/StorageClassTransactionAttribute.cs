using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Remotion.Data.DomainObjects
{
  /// <summary>Defines the property as managed in the <see cref="ClientTransaction"/> but not-persisted in the underlying data store.</summary>
  internal sealed class StorageClassTransactionAttribute : StorageClassAttribute
  {
    public StorageClassTransactionAttribute()
        : base (StorageClass.Transaction)
    {
    }
  }
}