using System;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Core.MixedDomains.SampleTypes
{
  [Uses (typeof (StubStoragePersistentMixin))]
  [Uses (typeof (NullMixin))]
  [DBTable]
  [StorageProviderStub]
  public class StubStorageTargetClassForPersistentMixin : DomainObject
  {
    public static StubStorageTargetClassForPersistentMixin NewObject ()
    {
      return NewObject<StubStorageTargetClassForPersistentMixin> ().With ();
    }

    public static StubStorageTargetClassForPersistentMixin GetObject (ObjectID id)
    {
      return GetObject<StubStorageTargetClassForPersistentMixin> (id);
    }
  }
}