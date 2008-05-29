using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Core
{
  [TestFixture]
  public class BusinessObjectProviderTest : TestBase
  {
    private IBusinessObjectProvider _provider;
    private MockRepository _mockRepository;
    private IBusinessObjectServiceFactory _serviceFactoryStub;

    public override void SetUp ()
    {
      base.SetUp();

      _serviceFactoryStub = MockRepository.GenerateStub<IBusinessObjectServiceFactory>();
      _provider = new StubBusinessObjectProvider (_serviceFactoryStub);
      _mockRepository = new MockRepository();

      BusinessObjectProvider.SetProvider<StubBusinessObjectProviderAttribute> (null);
      BusinessObjectProvider.SetProvider<BindableObjectProviderAttribute> (null);
    }

    public override void TearDown ()
    {
      base.TearDown();

      BusinessObjectProvider.SetProvider<StubBusinessObjectProviderAttribute> (null);
      BusinessObjectProvider.SetProvider<BindableObjectProviderAttribute> (null);
    }

    [Test]
    public void Instantiate ()
    {
      Assert.That (((BusinessObjectProvider) _provider).ServiceFactory, Is.SameAs (_serviceFactoryStub));
    }

    [Test]
    public void GetProvider ()
    {
      IBusinessObjectProvider provider = BusinessObjectProvider.GetProvider (typeof (StubBusinessObjectProviderAttribute));
      Assert.That (provider, Is.TypeOf (typeof (StubBusinessObjectProvider)));
    }

    [Test]
    public void GetProvider_WithMixin ()
    {
      using (MixinConfiguration.BuildNew().ForClass (typeof (StubBusinessObjectProvider)).AddMixin<MixinStub>().EnterScope())
      {
        IBusinessObjectProvider provider = BusinessObjectProvider.GetProvider (typeof (StubBusinessObjectProviderAttribute));
        Assert.That (provider, Is.InstanceOfType (typeof (IMixinTarget)));
      }
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
      BusinessObjectProvider.SetProvider (typeof (StubBusinessObjectProviderAttribute), new StubBusinessObjectProvider (_serviceFactoryStub));
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
        ExpectedMessage = 
        "The provider is not compatible with the provider-type required by the businessObjectProviderAttributeType's instantiation."
        + "\r\nParameter name: provider")]
    public void SetProvider_WithMismatchedTypes ()
    {
      BusinessObjectProvider.SetProvider (typeof (BindableObjectProviderAttribute), _provider);
    }

    [Test]
    public void AddAndGetService ()
    {
      IBusinessObjectService expectedService = _mockRepository.Stub<IBusinessObjectService>();
      Assert.That (_provider.GetService (expectedService.GetType()), Is.Null);

      ((BusinessObjectProvider) _provider).AddService (expectedService.GetType(), expectedService);

      Assert.That (_provider.GetService (expectedService.GetType()), Is.SameAs (expectedService));
    }

    [Test]
    public void AddService_Twice ()
    {
      IBusinessObjectService expectedService = _mockRepository.Stub<IBusinessObjectService>();
      Assert.That (_provider.GetService (expectedService.GetType()), Is.Null);

      ((BusinessObjectProvider) _provider).AddService (expectedService.GetType(), _mockRepository.Stub<IBusinessObjectService>());
      ((BusinessObjectProvider) _provider).AddService (expectedService.GetType(), expectedService);

      Assert.That (_provider.GetService (expectedService.GetType()), Is.SameAs (expectedService));
    }

    [Test]
    public void AddService_WithGeneric ()
    {
      StubBusinessObjectService expectedService = new StubBusinessObjectService();
      Assert.That (_provider.GetService (expectedService.GetType ()), Is.Null);

      ((BusinessObjectProvider) _provider).AddService<IBusinessObjectService> (new StubBusinessObjectService());
      ((BusinessObjectProvider) _provider).AddService (expectedService);

      Assert.That (_provider.GetService (typeof (IBusinessObjectService)), Is.InstanceOfType (typeof (StubBusinessObjectService)));
      Assert.That (_provider.GetService (expectedService.GetType ()), Is.SameAs (expectedService));
      Assert.That (_provider.GetService (expectedService.GetType ()), Is.Not.SameAs (_provider.GetService (typeof (IBusinessObjectService))));
    }

    [Test]
    public void GetService_FromGeneric ()
    {
      ((BusinessObjectProvider) _provider).AddService (typeof (IBusinessObjectService), _mockRepository.Stub<IBusinessObjectService>());

      Assert.That (
          ((BusinessObjectProvider) _provider).GetService<IBusinessObjectService>(),
          Is.SameAs (_provider.GetService (typeof (IBusinessObjectService))));
    }

    [Test]
    public void GetService_FromServiceFactory ()
    {
      MockRepository mockRepository = new MockRepository();
      IBusinessObjectServiceFactory serviceFactoryMock = mockRepository.CreateMock<IBusinessObjectServiceFactory>();
      IBusinessObjectStringFormatterService serviceStub = MockRepository.GenerateStub<IBusinessObjectStringFormatterService>();
      BusinessObjectProvider provider = new StubBusinessObjectProvider (serviceFactoryMock);

      Expect.Call (serviceFactoryMock.CreateService (typeof (IBusinessObjectStringFormatterService))).Return (serviceStub);

      mockRepository.ReplayAll();

      IBusinessObjectService actual = provider.GetService (typeof (IBusinessObjectStringFormatterService));
      IBusinessObjectService actual2 = provider.GetService (typeof (IBusinessObjectStringFormatterService));

      mockRepository.VerifyAll();

      Assert.That (actual, Is.SameAs (serviceStub));
      Assert.That (actual, Is.SameAs (actual2));
    }

    [Test]
    public void GetService_FromExplictValue ()
    {
      MockRepository mockRepository = new MockRepository();
      IBusinessObjectServiceFactory serviceFactoryMock = mockRepository.CreateMock<IBusinessObjectServiceFactory>();
      IBusinessObjectStringFormatterService serviceStub = MockRepository.GenerateStub<IBusinessObjectStringFormatterService>();
      BusinessObjectProvider provider = new StubBusinessObjectProvider (serviceFactoryMock);

      provider.AddService (typeof (IBusinessObjectStringFormatterService), serviceStub);

      mockRepository.ReplayAll();

      IBusinessObjectService actual = provider.GetService (typeof (IBusinessObjectStringFormatterService));
      IBusinessObjectService actual2 = provider.GetService (typeof (IBusinessObjectStringFormatterService));

      mockRepository.VerifyAll();

      Assert.That (actual, Is.SameAs (serviceStub));
      Assert.That (actual, Is.SameAs (actual2));
    }
  }
}