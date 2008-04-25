using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.Data.DomainObjects.UnitTests.ObjectBinding.BindableDomainObjectMixinTests
{
  [TestFixture]
  public class IdentityTest : ObjectBindingBaseTest
  {
    [Test]
    public void BindableDomainObjectsHaveIdentity ()
    {
      BindableSampleDomainObject domainObject = BindableSampleDomainObject.NewObject ();
      Assert.IsTrue (domainObject is IBusinessObjectWithIdentity);
    }

    [Test]
    public void BindableDomainObjectClassesHaveIdentity ()
    {
      BindableSampleDomainObject domainObject = BindableSampleDomainObject.NewObject ();
      Assert.IsTrue (((IBusinessObjectWithIdentity)domainObject).BusinessObjectClass is IBusinessObjectClassWithIdentity);
    }
    
    [Test]
    public void UniqueIdentifier ()
    {
      BindableSampleDomainObject domainObject = BindableSampleDomainObject.NewObject ();
      Assert.AreEqual (domainObject.ID.ToString (), ((IBusinessObjectWithIdentity) domainObject).UniqueIdentifier);
    }

    [Test]
    public void GetFromUniqueIdentifier ()
    {
      BindableObjectProvider.Current.AddService (typeof (BindableDomainObjectGetObjectService), new BindableDomainObjectGetObjectService());
      BindableSampleDomainObject original = BindableSampleDomainObject.NewObject ();
      BindableObjectClassWithIdentity boClass =
          (BindableObjectClassWithIdentity) BindableObjectProvider.Current.GetBindableObjectClass (typeof (BindableSampleDomainObject));
      Assert.AreSame (original, boClass.GetObject (original.ID.ToString ()));
    }
  }
}
