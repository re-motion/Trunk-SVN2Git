using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using NUnit.Framework;
using Rhino.Mocks;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Design.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.ObjectBinding.UnitTests.Core.Design.BindableObject
{
  [TestFixture]
  public class BindableObjectTypeFinderTest
  {
    private MockRepository _mockRepository;
    private IServiceProvider _serviceProvider;
    private ITypeDiscoveryService _typeDiscoveryService;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _serviceProvider = _mockRepository.CreateMock<IServiceProvider> ();
      _typeDiscoveryService = _mockRepository.CreateMock<ITypeDiscoveryService> ();
    }

    [Test]
    public void GetTypes_WithTypeDiscoveryService_IncludeGac ()
    {
      Expect.Call (_serviceProvider.GetService (typeof (ITypeDiscoveryService))).Return (_typeDiscoveryService);
      Expect.Call (_typeDiscoveryService.GetTypes (typeof (object), false))
          .Return (
          new object[]
              {
                  typeof (ClassWithAllDataTypes),
                  typeof (ClassWithValueType<>),
                  typeof (SimpleValueType),
                  typeof (SimpleReferenceType),
                  typeof (ClassWithIdentity)
              });

      _mockRepository.ReplayAll ();

      BindableObjectTypeFinder finder = new BindableObjectTypeFinder (_serviceProvider);
      List<Type> types = finder.GetTypes (true);

      Assert.That (types, Is.EquivalentTo (new Type[] { typeof (ClassWithAllDataTypes), typeof (ClassWithValueType<>), typeof (ClassWithIdentity) }));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetTypes_WithTypeDiscoveryService_NotIncludeGac ()
    {
      Expect.Call (_serviceProvider.GetService (typeof (ITypeDiscoveryService))).Return (_typeDiscoveryService);
      Expect.Call (_typeDiscoveryService.GetTypes (typeof (object), true))
          .Return (
          new object[]
              {
                  typeof (ClassWithAllDataTypes),
                  typeof (ClassWithValueType<>),
                  typeof (SimpleValueType),
                  typeof (SimpleReferenceType)
              });

      _mockRepository.ReplayAll ();

      BindableObjectTypeFinder finder = new BindableObjectTypeFinder (_serviceProvider);
      List<Type> types = finder.GetTypes (false);

      Assert.That (types, Is.EquivalentTo (new Type[] { typeof (ClassWithAllDataTypes), typeof (ClassWithValueType<>) }));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetTypes_WithTypeDiscoveryService_IgnoresActiveMixinConfiguration ()
    {
      using (MixinConfiguration.BuildNew ().EnterScope ())
      {
        Expect.Call (_serviceProvider.GetService (typeof (ITypeDiscoveryService))).Return (_typeDiscoveryService);
        Expect.Call (_typeDiscoveryService.GetTypes (typeof (object), true))
            .Return (
            new object[]
                {
                    typeof (ClassWithAllDataTypes),
                    typeof (ClassWithValueType<>),
                    typeof (SimpleValueType),
                    typeof (SimpleReferenceType)
                });

        _mockRepository.ReplayAll();

        BindableObjectTypeFinder finder = new BindableObjectTypeFinder (_serviceProvider);
        List<Type> types = finder.GetTypes (false);

        Assert.That (types, Is.EquivalentTo (new Type[] {typeof (ClassWithAllDataTypes), typeof (ClassWithValueType<>)}));

        _mockRepository.VerifyAll();
      }
    }

    [Test]
    public void GetTypes_WithTypeDiscoveryService_GetsTypeInheritingMixinFromBase ()
    {
      Expect.Call (_serviceProvider.GetService (typeof (ITypeDiscoveryService))).Return (_typeDiscoveryService);
      Expect.Call (_typeDiscoveryService.GetTypes (typeof (object), true))
          .Return (
          new object[]
              {
                  typeof (DerivedBusinessObjectClassWithoutAttribute)
              });

      _mockRepository.ReplayAll ();

      BindableObjectTypeFinder finder = new BindableObjectTypeFinder (_serviceProvider);
      List<Type> types = finder.GetTypes (false);

      Assert.That (types, Is.EquivalentTo (new Type[] { typeof (DerivedBusinessObjectClassWithoutAttribute) }));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetTypes_WithoutTypeDiscoveryService ()
    {
      Expect.Call (_serviceProvider.GetService (typeof (ITypeDiscoveryService))).Return (null);

      _mockRepository.ReplayAll ();

      BindableObjectTypeFinder finder = new BindableObjectTypeFinder (_serviceProvider);
      List<Type> types = finder.GetTypes (false);

      Assert.That (types, Is.Empty);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetMixinConfiguration_IncludeGac ()
    {
      Expect.Call (_serviceProvider.GetService (typeof (ITypeDiscoveryService))).Return (_typeDiscoveryService);
      Expect.Call (_typeDiscoveryService.GetTypes (typeof (object), false))
          .Return (
          new object[]
              {
                  typeof (DerivedBusinessObjectClassWithoutAttribute),
                  typeof (SimpleBusinessObjectClass),
                  typeof (ClassWithIdentity)
              });

      _mockRepository.ReplayAll ();

      BindableObjectTypeFinder finder = new BindableObjectTypeFinder (_serviceProvider);
      MixinConfiguration configuration = finder.GetMixinConfiguration (true);
      Assert.That (configuration.ClassContexts.Count, Is.EqualTo (3));
      Assert.That (configuration.ClassContexts.ContainsExact (typeof (BaseBusinessObjectClass)));
      Assert.That (configuration.ClassContexts.ContainsExact (typeof (DerivedBusinessObjectClassWithoutAttribute)), Is.False);
      Assert.That (configuration.ClassContexts.ContainsExact (typeof (SimpleBusinessObjectClass)));
      Assert.That (configuration.ClassContexts.ContainsExact (typeof (ClassWithIdentity)));

      Assert.That (configuration.ClassContexts.GetExact (typeof (BaseBusinessObjectClass)).Mixins.ContainsKey (typeof (BindableObjectMixin)));
      Assert.That (configuration.ClassContexts.GetExact (typeof (ClassWithIdentity)).Mixins.ContainsKey (typeof (BindableObjectWithIdentityMixin)));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetMixinConfiguration_NotIncludeGac ()
    {
      Expect.Call (_serviceProvider.GetService (typeof (ITypeDiscoveryService))).Return (_typeDiscoveryService);
      Expect.Call (_typeDiscoveryService.GetTypes (typeof (object), true)).Return (new object[0]);

      _mockRepository.ReplayAll ();

      BindableObjectTypeFinder finder = new BindableObjectTypeFinder (_serviceProvider);
      finder.GetMixinConfiguration (false);

      _mockRepository.VerifyAll ();
    }
  }
}