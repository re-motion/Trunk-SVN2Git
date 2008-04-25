using System;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes
{
  [Uses (typeof (StubStoragePersistentMixin))]
  [Uses (typeof (NullMixin))]
  [DBTable]
  [StorageProviderStub]
  public class StubStorageTargetClassForPersistentMixin : DomainObject
  {
    public static StubStorageTargetClassForPersistentMixin NewObject ()
    {
      return DomainObject.NewObject<StubStorageTargetClassForPersistentMixin> ().With ();
    }

    public static StubStorageTargetClassForPersistentMixin GetObject (ObjectID id)
    {
      return DomainObject.GetObject<StubStorageTargetClassForPersistentMixin> (id);
    }
  }
}