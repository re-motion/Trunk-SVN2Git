using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain;
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.Security;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.ObjectBinding.BindableDomainObjectMixinTests
{
  [TestFixture]
  public class GetDisplayNameTest : ObjectBindingBaseTest
  {
    private MockRepository _mockRepository;
    private IObjectSecurityAdapter _mockObjectSecurityAdapter;

    public override void SetUp ()
    {
      base.SetUp();

      _mockRepository = new MockRepository();
      _mockObjectSecurityAdapter = _mockRepository.CreateMock<IObjectSecurityAdapter>();
      AdapterRegistry.Instance.SetAdapter (typeof (IObjectSecurityAdapter), _mockObjectSecurityAdapter);
    }

    public override void TearDown ()
    {
      base.TearDown();
      AdapterRegistry.Instance.SetAdapter (typeof (IObjectSecurityAdapter), null);
    }

    [Test]
    public void DisplayName ()
    {
      BindableDomainObjectMixin bindableObjectMixin = Mixin.Get<BindableDomainObjectMixin> (BindableSampleDomainObject.NewObject ());

      Assert.That (
          ((IBusinessObject) bindableObjectMixin).DisplayName,
          Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain.BindableSampleDomainObject, Remotion.Data.DomainObjects.UnitTests"));
    }

    [Test]
    public void OverriddenDisplayName ()
    {
      IBusinessObject businessObject = (IBusinessObject) BindableDomainObjectWithOverriddenDisplayName.NewObject();

      Assert.That (
          businessObject.DisplayName,
          Is.EqualTo ("TheDisplayName"));
    }

    [Test]
    public void DisplayNameSafe_WithOverriddenDisplayNameAndAccessGranted ()
    {
      IObjectSecurityStrategy stubSecurityStrategy = _mockRepository.Stub<IObjectSecurityStrategy>();
      ISecurableObject securableObject = SecurableBindableDomainObjectWithOverriddenDisplayName.NewObject (stubSecurityStrategy);
      BindableDomainObjectMixin bindableObjectMixin = Mixin.Get<BindableDomainObjectMixin> (securableObject);
      Expect.Call (_mockObjectSecurityAdapter.HasAccessOnGetAccessor (securableObject, "DisplayName")).Return (true);
      _mockRepository.ReplayAll();

      string actual = ((IBusinessObject) bindableObjectMixin).DisplayNameSafe;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.EqualTo ("TheDisplayName"));
    }

    [Test]
    public void DisplayNameSafe_WithOverriddenDisplayNameAndWithAccessDenied ()
    {
      IObjectSecurityStrategy stubSecurityStrategy = _mockRepository.Stub<IObjectSecurityStrategy>();
      ISecurableObject securableObject = SecurableBindableDomainObjectWithOverriddenDisplayName.NewObject (stubSecurityStrategy);
      BindableDomainObjectMixin bindableObjectMixin = Mixin.Get<BindableDomainObjectMixin> (securableObject);
      Expect.Call (_mockObjectSecurityAdapter.HasAccessOnGetAccessor (securableObject, "DisplayName")).Return (false);
      _mockRepository.ReplayAll();

      string actual = ((IBusinessObject) bindableObjectMixin).DisplayNameSafe;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.EqualTo ("×"));
    }
  }
}