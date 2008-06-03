/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class BindableObjectProviderTest : TestBase
  {
    private BindableObjectProvider _provider;
    private IMetadataFactory _metadataFactoryStub;
    private IBusinessObjectServiceFactory _serviceFactoryStub;

    public override void SetUp ()
    {
      base.SetUp();

      _provider = new BindableObjectProvider();
      _metadataFactoryStub = MockRepository.GenerateStub<IMetadataFactory>();
      _serviceFactoryStub = MockRepository.GenerateStub<IBusinessObjectServiceFactory>();
    }

    [Test]
    public void GetProviderForBindableObjectType ()
    {
      BindableObjectProvider provider = BindableObjectProvider.GetProviderForBindableObjectType (typeof (SimpleBusinessObjectClass));

      Assert.That (provider, Is.Not.Null);
      Assert.That (provider, Is.SameAs (BusinessObjectProvider.GetProvider (typeof (BindableObjectProviderAttribute))));
    }

    [Test]
    public void GetProviderForBindableObjectType_WithIdentityType ()
    {
      BindableObjectProvider provider = BindableObjectProvider.GetProviderForBindableObjectType (typeof (ClassWithIdentity));

      Assert.That (provider, Is.Not.Null);
      Assert.That (provider, Is.SameAs (BusinessObjectProvider.GetProvider (typeof (BindableObjectWithIdentityProviderAttribute))));
      Assert.That (provider, Is.Not.SameAs (BusinessObjectProvider.GetProvider (typeof (BindableObjectProviderAttribute))));
    }

    [Test]
    [Ignore ("TODO: Implement once AttributeUtility has been extended.")]
    public void GetProviderForBindableObjectType_WithAttributeFromTypeOverridingAttributeFromMixin ()
    {
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage =
        "The type 'Remotion.ObjectBinding.UnitTests.Core.TestDomain.ManualBusinessObject' does not have the "
        + "'Remotion.ObjectBinding.BusinessObjectProviderAttribute' applied.\r\nParameter name: type")]
    public void GetProviderForBindableObjectType_WithMissingAttributeType ()
    {
      BindableObjectProvider.GetProviderForBindableObjectType (typeof (ManualBusinessObject));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage =
        "The business object provider associated with the type 'Remotion.ObjectBinding.UnitTests.Core.TestDomain.StubBusinessObject' "
        + "is not of type 'Remotion.ObjectBinding.BindableObject.BindableObjectProvider'.\r\nParameter name: type")]
    public void GetProviderForBindableObjectType_WithInvalidProviderType ()
    {
      BindableObjectProvider.GetProviderForBindableObjectType (typeof (StubBusinessObject));
    }

    [Test]
    public void GetBindableObjectClass ()
    {
      MockRepository mockRepository = new MockRepository();
      IMetadataFactory metadataFactoryMock = mockRepository.CreateMock<IMetadataFactory>();
      IClassReflector classReflectorMock = mockRepository.CreateMock<IClassReflector>();

      BindableObjectProvider provider = new BindableObjectProvider (metadataFactoryMock, _serviceFactoryStub);
      Type targetType = typeof (SimpleBusinessObjectClass);
      Type concreteType = Mixins.TypeUtility.GetConcreteMixedType (targetType);
      BindableObjectClass expectedBindableObjectClass = new BindableObjectClass (concreteType, provider);

      Expect.Call (metadataFactoryMock.CreateClassReflector (targetType, provider)).Return (classReflectorMock);
      Expect.Call (classReflectorMock.GetMetadata()).Return (expectedBindableObjectClass);

      mockRepository.ReplayAll();

      BindableObjectClass actual = provider.GetBindableObjectClass (targetType);

      mockRepository.VerifyAll();

      Assert.That (actual, Is.SameAs (expectedBindableObjectClass));
    }

    [Test]
    public void GetBindableObjectClass_SameTwice ()
    {
      Assert.That (
          _provider.GetBindableObjectClass (typeof (SimpleBusinessObjectClass)),
          Is.SameAs (_provider.GetBindableObjectClass (typeof (SimpleBusinessObjectClass))));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage =
        "Type 'Remotion.ObjectBinding.UnitTests.Core.TestDomain.SimpleReferenceType' does not implement the "
        + "'Remotion.ObjectBinding.IBusinessObject' interface via the 'Remotion.ObjectBinding.BindableObject.BindableObjectMixinBase`1'.\r\n"
        + "Parameter name: concreteType")]
    public void GetBindableObjectClass_WithTypeNotUsingBindableObjectMixin ()
    {
      _provider.GetBindableObjectClass (typeof (SimpleReferenceType));
    }

    [Test]
    public void GetBindableObjectClassFromProvider ()
    {
      BindableObjectClass actual = BindableObjectProvider.GetBindableObjectClassFromProvider (typeof (SimpleBusinessObjectClass));

      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.TargetType, Is.SameAs (typeof (SimpleBusinessObjectClass)));
      Assert.That (
          actual,
          Is.SameAs (
              BindableObjectProvider.GetProviderForBindableObjectType (typeof (SimpleBusinessObjectClass)).GetBindableObjectClass (
                  typeof (SimpleBusinessObjectClass))));
    }

    [Test]
    public void GetMetadataFactory_WithDefaultFactory ()
    {
      Assert.IsInstanceOfType (typeof (BindableObjectMetadataFactory), _provider.MetadataFactory);
    }

    [Test]
    public void GetMetadataFactoryForType_WithCustomMetadataFactory ()
    {
      BindableObjectProvider provider = new BindableObjectProvider (_metadataFactoryStub, _serviceFactoryStub);
      Assert.AreSame (_metadataFactoryStub, provider.MetadataFactory);
    }

    [Test]
    public void GetServiceFactory_WithDefaultFactory ()
    {
      Assert.That (_provider.ServiceFactory, Is.InstanceOfType (typeof (BindableObjectServiceFactory)));
    }

    [Test]
    public void GetServiceFactory_WithMixin ()
    {
      using (MixinConfiguration.BuildNew ().ForClass (typeof (BindableObjectServiceFactory)).AddMixin<MixinStub> ().EnterScope ())
      {
        BindableObjectProvider provider = new BindableObjectProvider ();
        Assert.That (provider.ServiceFactory, Is.InstanceOfType (typeof (BindableObjectServiceFactory)));
        Assert.That (provider.ServiceFactory, Is.InstanceOfType (typeof (IMixinTarget)));
      }
    }

    [Test]
    public void GetServiceFactoryForType_WithCustomServiceFactory ()
    {
      BindableObjectProvider provider = new BindableObjectProvider (_metadataFactoryStub, _serviceFactoryStub);
      Assert.AreSame (_serviceFactoryStub, provider.ServiceFactory);
    }
  }
}
