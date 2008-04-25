using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes
{
  [NonIntroduced (typeof (IDomainObjectMixin))]
  public class HookedDomainObjectMixin : Mixin<Order>, IDomainObjectMixin
  {
    public bool OnLoadedCalled = false;
    public int OnLoadedCount = 0;
    public LoadMode OnLoadedLoadMode;
    public bool OnCreatedCalled = false;

    public void OnDomainObjectLoaded (LoadMode loadMode)
    {
      OnLoadedCalled = true;
      OnLoadedLoadMode = loadMode;
      ++OnLoadedCount;
      Assert.IsNotNull (This.ID);
      ++This.OrderNumber;
      Assert.IsNotNull (This.OrderItems);
    }

    public void OnDomainObjectCreated ()
    {
      OnCreatedCalled = true;
      Assert.IsNotNull (This.ID);
      This.OrderNumber += 2;
      Assert.IsNotNull (This.OrderItems);
    }
  }
}