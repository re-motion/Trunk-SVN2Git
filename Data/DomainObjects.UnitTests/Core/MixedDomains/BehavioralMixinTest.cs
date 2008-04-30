using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.Core.MixedDomains.SampleTypes;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Core.MixedDomains
{
  [TestFixture]
  public class BehavioralMixinTest : ClientTransactionBaseTest
  {
    [Test]
    public void NewDomainObjectsCanBeMixed ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (Order)).Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        Order order = Order.NewObject ();
        Assert.IsNotNull (Mixin.Get<NullMixin> (order));
      }
    }

    [Test]
    public void LoadedDomainObjectsCanBeMixed ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (Order)).Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        Order order = Order.GetObject (DomainObjectIDs.Order1);
        Assert.IsNotNull (Mixin.Get<NullMixin> (order));
      }
    }

    [Test]
    public void MixinCanAddInterface ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (Order)).Clear().AddMixins (typeof (MixinAddingInterface)).EnterScope())
      {
        Order order = Order.GetObject (DomainObjectIDs.Order1);
        Assert.IsTrue (order is IInterfaceAddedByMixin);
        Assert.AreEqual ("Hello, my ID is " + DomainObjectIDs.Order1, ((IInterfaceAddedByMixin) order).GetGreetings ());
      }
    }

    [Test]
    public void MixinCanOverrideVirtualPropertiesAndMethods ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (DOWithVirtualPropertiesAndMethods)).Clear().AddMixins (typeof (MixinOverridingPropertiesAndMethods)).EnterScope())
      {
        DOWithVirtualPropertiesAndMethods instance = (DOWithVirtualPropertiesAndMethods) RepositoryAccessor.NewObject (typeof (DOWithVirtualPropertiesAndMethods)).With();
        instance.Property = "Text";
        Assert.AreEqual ("Text-MixinSetter-MixinGetter", instance.Property);
        Assert.AreEqual ("Something-MixinMethod", instance.GetSomething ());
      }
    }

    [DBTable]
    [TestDomain]
    [Uses (typeof (NullMixin))]
    public class NestedDomainObject : DomainObject
    {
      public static NestedDomainObject NewObject ()
      {
        return NewObject<NestedDomainObject> ().With ();
      }
    }

    [Test]
    public void NestedDomainObjectDomainObjectsCanBeMixed ()
    {
      DomainObject domainObject = NestedDomainObject.NewObject ();
      Assert.IsNotNull (Mixin.Get<NullMixin> (domainObject));
    }
  }
}