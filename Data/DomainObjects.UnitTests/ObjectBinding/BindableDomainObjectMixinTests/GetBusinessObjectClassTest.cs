using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain;
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.Data.DomainObjects.UnitTests.ObjectBinding.BindableDomainObjectMixinTests
{
  [TestFixture]
  public class GetBusinessObjectClassTest : ObjectBindingBaseTest
  {
    private BindableSampleDomainObject _bindableObject;
    private BindableDomainObjectMixin _bindableObjectMixin;
    private IBusinessObject _businessObject;

    public override void SetUp ()
    {
      base.SetUp();

      _bindableObject = BindableSampleDomainObject.NewObject();
      _bindableObjectMixin = Mixin.Get<BindableDomainObjectMixin> (_bindableObject);
      _businessObject = _bindableObjectMixin;
    }

    [Test]
    public void FromClass ()
    {
      Assert.That (_bindableObjectMixin.BusinessObjectClass, Is.Not.Null);
      Assert.That (_bindableObjectMixin.BusinessObjectClass.TargetType, Is.SameAs (typeof (BindableSampleDomainObject)));
      Assert.That (_bindableObjectMixin.BusinessObjectClass.BusinessObjectProvider, Is.SameAs (BindableObjectProvider.Current));
    }

    [Test]
    public void FromInterface ()
    {
      Assert.That (_businessObject.BusinessObjectClass, Is.Not.Null);
      Assert.That (_businessObject.BusinessObjectClass, Is.SameAs (_bindableObjectMixin.BusinessObjectClass));
      Assert.That (_businessObject.BusinessObjectClass.BusinessObjectProvider, Is.SameAs (BindableObjectProvider.Current));
    }
  }
}