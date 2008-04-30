using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Remotion.Data.DomainObjects.UnitTests.Core.MixedDomains.SampleTypes
{
  public interface IMixinAddingPeristentProperties
  {
    int PersistentProperty { get; set; }

    [StorageClass (StorageClass.Persistent)]
    int ExtraPersistentProperty { get; set; }

    [StorageClassNone]
    int NonPersistentProperty { get; set; }
  }
}