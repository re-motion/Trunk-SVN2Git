using System;
using NUnit.Framework;
using Rhino.Mocks;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain;
using Remotion.Security;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.PropertyBaseTests
{
  [TestFixture]
  public class SecurityTest : TestBase
  {
    private MockRepository _mocks;
    private BindableObjectProvider _businessObjectProvider;
    private IObjectSecurityAdapter _mockObjectSecurityAdapter;
    private IBusinessObjectProperty _securableProperty;
    private IBusinessObjectProperty _securableExplicitInterfaceProperty;
    private IBusinessObjectProperty _nonSecurableProperty;
    private IBusinessObjectProperty _nonSecurablePropertyReadOnly;
    private IBusinessObject _securableObject;
    private IBusinessObject _nonSecurableObject;

    public override void SetUp ()
    {
      base.SetUp ();
      
      _mocks = new MockRepository ();
      _businessObjectProvider = new BindableObjectProvider();
      _mockObjectSecurityAdapter = _mocks.CreateMock<IObjectSecurityAdapter>();

      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IObjectSecurityAdapter), _mockObjectSecurityAdapter);

      _securableObject = (IBusinessObject) ObjectFactory.Create<SecurableClassWithReferenceType<SimpleReferenceType>>()
                                               .With (_mocks.CreateMock<IObjectSecurityStrategy>());

      _nonSecurableObject = (IBusinessObject) ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>>().With();

      _securableProperty = new StubPropertyBase (
          new PropertyBase.Parameters (
              _businessObjectProvider, GetPropertyInfo (typeof (SecurableClassWithReferenceType<SimpleReferenceType>), "Scalar"),
              typeof (SimpleReferenceType), null, false, false));

      _securableExplicitInterfaceProperty = new StubPropertyBase (
          new PropertyBase.Parameters (
              _businessObjectProvider, GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>),
              "Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain.IInterfaceWithReferenceType<T>.ExplicitInterfaceScalar"),
              typeof (SimpleReferenceType), null, false, false));
      
      _nonSecurablePropertyReadOnly = new StubPropertyBase (
          new PropertyBase.Parameters (
              _businessObjectProvider, GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "ReadOnlyScalar"),
              typeof (SimpleReferenceType), null,  false, true));
      
      _nonSecurableProperty = new StubPropertyBase (
          new PropertyBase.Parameters (
              _businessObjectProvider, GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Scalar"),
              typeof (SimpleReferenceType), null, false, false));
    }

    public override void TearDown ()
    {
      base.TearDown();
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IObjectSecurityAdapter), null);
    }

    [Test]
    public void IsAccessibleWithoutObjectSecurityProvider ()
    {
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IObjectSecurityAdapter), null);
      _mocks.ReplayAll();

      bool isAccessible = _securableProperty.IsAccessible (_securableObject.BusinessObjectClass, _securableObject);

      _mocks.VerifyAll();
      Assert.IsTrue (isAccessible);
    }

    [Test]
    public void IsAccessible ()
    {
      ExpectHasAccessOnGetAccessor (true);
      _mocks.ReplayAll();

      bool isAccessible = _securableProperty.IsAccessible (_securableObject.BusinessObjectClass, _securableObject);

      _mocks.VerifyAll();
      Assert.IsTrue (isAccessible);
    }

    [Test]
    public void IsNotAccessible ()
    {
      ExpectHasAccessOnGetAccessor (false);
      _mocks.ReplayAll();

      bool isAccessible = _securableProperty.IsAccessible (_securableObject.BusinessObjectClass, _securableObject);

      _mocks.VerifyAll();
      Assert.IsFalse (isAccessible);
    }

    [Test]
    public void IsAccessibleOnExplicitInterfaceProperty ()
    {
      Expect.Call (
          _mockObjectSecurityAdapter.HasAccessOnGetAccessor (
              (ISecurableObject) _securableObject, ((StubPropertyBase) _securableExplicitInterfaceProperty).PropertyInfo.Name)).Return (true);

      _mocks.ReplayAll ();

      bool isAccessible = _securableExplicitInterfaceProperty.IsAccessible (_securableObject.BusinessObjectClass, _securableObject);

      _mocks.VerifyAll ();
      Assert.IsTrue (isAccessible);
    }

    [Test]
    public void IsAccessibleForNonSecurableType ()
    {
      _mocks.ReplayAll();

      bool isAccessible = _nonSecurableProperty.IsAccessible (_nonSecurableObject.BusinessObjectClass, _nonSecurableObject);

      _mocks.VerifyAll();
      Assert.IsTrue (isAccessible);
    }

    [Test]
    public void IsNotReadOnlyWithoutObjectSecurityProvider ()
    {
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IObjectSecurityAdapter), null);
      _mocks.ReplayAll();

      bool isReadOnly = _securableProperty.IsReadOnly (_securableObject);

      _mocks.VerifyAll();
      Assert.IsFalse (isReadOnly);
    }

    [Test]
    public void IsReadOnly ()
    {
      ExpectHasAccessOnSetAccessor (false);
      _mocks.ReplayAll();

      bool isReadOnly = _securableProperty.IsReadOnly (_securableObject);

      _mocks.VerifyAll();
      Assert.IsTrue (isReadOnly);
    }

    [Test]
    public void IsReadOnlyOnExplicitInterfaceProperty ()
    {
      Expect.Call (
          _mockObjectSecurityAdapter.HasAccessOnSetAccessor (
              (ISecurableObject) _securableObject, ((StubPropertyBase) _securableExplicitInterfaceProperty).PropertyInfo.Name)).Return (false);

      _mocks.ReplayAll ();

      bool isReadOnly = _securableExplicitInterfaceProperty.IsReadOnly (_securableObject);

      _mocks.VerifyAll ();
      Assert.IsTrue (isReadOnly);
    }

    [Test]
    public void IsNotReadOnly ()
    {
      ExpectHasAccessOnSetAccessor (true);
      _mocks.ReplayAll();

      bool isReadOnly = _securableProperty.IsReadOnly (_securableObject);

      _mocks.VerifyAll();
      Assert.IsFalse (isReadOnly);
    }

    [Test]
    public void IsNotReadOnlyForNonSecurableType ()
    {
      _mocks.ReplayAll();

      bool isReadOnly = _nonSecurableProperty.IsReadOnly (_nonSecurableObject);

      _mocks.VerifyAll();
      Assert.IsFalse (isReadOnly);
    }

    [Test]
    public void IsReadOnlyForNonSecurableType ()
    {
      _mocks.ReplayAll();

      bool isReadOnly = _nonSecurablePropertyReadOnly.IsReadOnly (_nonSecurableObject);

      _mocks.VerifyAll();
      Assert.IsTrue (isReadOnly);
    }

    private void ExpectHasAccessOnGetAccessor (bool returnValue)
    {
      Expect.Call (
          _mockObjectSecurityAdapter.HasAccessOnGetAccessor (
              (ISecurableObject) _securableObject, ((StubPropertyBase) _securableProperty).PropertyInfo.Name)).Return (returnValue);
    }

    private void ExpectHasAccessOnSetAccessor (bool returnValue)
    {
      Expect.Call (
          _mockObjectSecurityAdapter.HasAccessOnSetAccessor (
              (ISecurableObject) _securableObject, ((StubPropertyBase) _securableProperty).PropertyInfo.Name)).Return (returnValue);
    }
  }
}