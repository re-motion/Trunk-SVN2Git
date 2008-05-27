using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Core
{
  [TestFixture]
  public class BusinessObjectProviderTest : TestBase
  {
    private IBusinessObjectProvider _provider;
    private MockRepository _mockRepository;

    public override void SetUp ()
    {
      base.SetUp ();

      _provider = new StubBusinessObjectProvider ();
      _mockRepository = new MockRepository ();
    }

    [Test]
    public void GetProvider ()
    {
      IBusinessObjectProvider provider = BusinessObjectProvider.GetProvider (typeof (StubBusinessObjectProviderAttribute));
      Assert.That (provider, Is.TypeOf (typeof (StubBusinessObjectProvider)));

      Assert.That (provider.GetService (typeof (IBindableObjectGlobalizationService)), Is.Null);
      Assert.That (provider.GetService (typeof (IBusinessObjectStringFormatterService)), Is.TypeOf (typeof (StubBusinessObjectProvider.StringFormatterService)));
    }

    [Test]
    public void GetProvider_WithDifferentAttributesResultingInDifferentProviders ()
    {
      IBusinessObjectProvider provider = BusinessObjectProvider.GetProvider (typeof (StubBusinessObjectProviderAttribute));
      Assert.That (provider, Is.TypeOf (typeof (StubBusinessObjectProvider)));
      Assert.That (provider, Is.Not.SameAs (BusinessObjectProvider.GetProvider (typeof (Stub2BusinessObjectProviderAttribute))));
      Assert.That (provider, Is.Not.SameAs (BusinessObjectProvider.GetProvider (typeof (DerivedStubBusinessObjectProviderAttribute))));
    }

    [Test]
    public void GetProvider_SameTwice ()
    {
      Assert.That (
        BusinessObjectProvider.GetProvider (typeof (StubBusinessObjectProviderAttribute)),
        Is.SameAs (BusinessObjectProvider.GetProvider (typeof (StubBusinessObjectProviderAttribute))));
    }

    [Test]
    public void GetProvider_FromGeneric ()
    {
      Assert.That (
        BusinessObjectProvider.GetProvider<StubBusinessObjectProviderAttribute>(),
        Is.SameAs (BusinessObjectProvider.GetProvider (typeof (StubBusinessObjectProviderAttribute))));
    }

    [Test]
    public void SetProvider ()
    {
      BusinessObjectProvider.SetProvider (typeof (StubBusinessObjectProviderAttribute), _provider);
      Assert.That (BusinessObjectProvider.GetProvider (typeof (StubBusinessObjectProviderAttribute)), Is.SameAs (_provider));
    }

    [Test]
    public void SetProvider_WithGeneric ()
    {
      BusinessObjectProvider.SetProvider<StubBusinessObjectProviderAttribute> (_provider);
      Assert.That (BusinessObjectProvider.GetProvider (typeof (StubBusinessObjectProviderAttribute)), Is.SameAs (_provider));
    }

    [Test]
    public void SetProvider_Twice ()
    {
      BusinessObjectProvider.SetProvider (typeof (StubBusinessObjectProviderAttribute), new StubBusinessObjectProvider ());
      BusinessObjectProvider.SetProvider (typeof (StubBusinessObjectProviderAttribute), _provider);
      Assert.That (BusinessObjectProvider.GetProvider (typeof (StubBusinessObjectProviderAttribute)), Is.SameAs (_provider));
    }

    [Test]
    public void SetProvider_Null ()
    {
      BusinessObjectProvider.SetProvider (typeof (StubBusinessObjectProviderAttribute), _provider);
      BusinessObjectProvider.SetProvider (typeof (StubBusinessObjectProviderAttribute), null);
      Assert.That (BusinessObjectProvider.GetProvider (typeof (StubBusinessObjectProviderAttribute)), Is.Not.SameAs (_provider));
      Assert.That (BusinessObjectProvider.GetProvider (typeof (StubBusinessObjectProviderAttribute)), Is.TypeOf (typeof (StubBusinessObjectProvider)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The provider is not compatible with the provider-type required by the businessObjectProviderAttributeType's instantiation.\r\nParameter name: provider")]
    public void SetProvider_WithMismatchedTypes ()
    {
      BusinessObjectProvider.SetProvider (typeof (BindableObjectProviderAttribute), _provider);
    }

    [Test]
    public void AddAndGetService ()
    {
      IBusinessObjectService expectedService = _mockRepository.Stub<IBusinessObjectService> ();
      Assert.That (_provider.GetService (expectedService.GetType ()), Is.Null);

      ((BusinessObjectProvider)_provider).AddService (expectedService.GetType (), expectedService);

      Assert.That (_provider.GetService (expectedService.GetType ()), Is.SameAs (expectedService));
    }

    [Test]
    public void GetServiceFromGeneric ()
    {
      ((BusinessObjectProvider) _provider).AddService (typeof (IBusinessObjectService), _mockRepository.Stub<IBusinessObjectService> ());

      Assert.That (
        ((BusinessObjectProvider) _provider).GetService<IBusinessObjectService> (), 
        Is.SameAs (_provider.GetService (typeof (IBusinessObjectService))));
    }
  }
}