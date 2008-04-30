using System;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Core.MixedDomains.SampleTypes
{
  [Uses (typeof (MixinAddingPersistentProperties))]
  [Uses (typeof (NullMixin))]
  [DBTable ("MixedDomains_Target")]
  [TestDomain]
  public class TargetClassForPersistentMixin : DomainObject
  {
    public static TargetClassForPersistentMixin NewObject ()
    {
      return NewObject<TargetClassForPersistentMixin> ().With ();
    }

    public static TargetClassForPersistentMixin GetObject (ObjectID id)
    {
      return GetObject<TargetClassForPersistentMixin> (id);
    }
  }
}