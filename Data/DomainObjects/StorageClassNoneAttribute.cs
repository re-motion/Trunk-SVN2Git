using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Remotion.Data.DomainObjects
{
  /// <summary>Defines the property as not managed by the persistence framework.</summary>
  public sealed class StorageClassNoneAttribute : StorageClassAttribute
  {
    public StorageClassNoneAttribute()
        : base (StorageClass.None)
    {
    }
  }
}