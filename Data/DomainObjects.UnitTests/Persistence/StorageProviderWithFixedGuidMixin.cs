using System;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence
{
  public class StorageProviderWithFixedGuidMixin : IStorageProviderWithFixedGuid
  {
    private Guid _fixedGuid = Guid.NewGuid ();

    [OverrideTarget]
    public ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      return new ObjectID (classDefinition, FixedGuid);
    }

    public Guid FixedGuid
    {
      get { return _fixedGuid; }
      set { _fixedGuid = value; }
    }
  }

  public interface IStorageProviderWithFixedGuid
  {
    Guid FixedGuid { get; set; }
  }
}