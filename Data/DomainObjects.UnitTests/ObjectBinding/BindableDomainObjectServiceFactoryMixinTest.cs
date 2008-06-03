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
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.ObjectBinding
{
  [TestFixture]
  public class BindableDomainObjectServiceFactoryMixinTest
  {
    private interface IStubService : IBusinessObjectService
    {}

    private IBusinessObjectServiceFactory _serviceFactory;
    private BindableDomainObjectServiceFactoryMixin _serviceMixin;
    private IBusinessObjectProviderWithIdentity _bindableDomainObjectProvider;
    private IBusinessObjectProviderWithIdentity _bindableObjectProvider;
    private MockRepository _mockRepository;

    [SetUp]
    public void SetUp ()
    {
      _serviceFactory = BindableObjectServiceFactory.Create();
      _serviceMixin = Mixin.Get<BindableDomainObjectServiceFactoryMixin>(_serviceFactory);

      _mockRepository = new MockRepository ();
      _bindableDomainObjectProvider = _mockRepository.Stub<IBusinessObjectProviderWithIdentity> ();
      _bindableObjectProvider = _mockRepository.Stub<IBusinessObjectProviderWithIdentity> ();
      SetupResult.For (_bindableDomainObjectProvider.ProviderAttribute).Return (new BindableDomainObjectProviderAttribute());
      SetupResult.For (_bindableObjectProvider.ProviderAttribute).Return (new BindableObjectProviderAttribute ());
      _mockRepository.ReplayAll();
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (_serviceMixin, Is.Not.Null);
      Assert.That (_serviceMixin, Is.InstanceOfType (typeof (IBusinessObjectServiceFactory)));
    }

    [Test]
    public void GetService_FromIBindableObjectGlobalizationService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_bindableDomainObjectProvider, typeof (IBindableObjectGlobalizationService)),
          Is.InstanceOfType (typeof (BindableObjectGlobalizationService)));
    }

    [Test]
    public void GetService_FromIBusinessObjectStringFormatterService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_bindableDomainObjectProvider, typeof (IBusinessObjectStringFormatterService)),
          Is.InstanceOfType (typeof (BusinessObjectStringFormatterService)));
    }

    [Test]
    public void GetService_FromIGetObjectService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_bindableDomainObjectProvider, typeof (IGetObjectService)),
          Is.InstanceOfType (typeof (BindableDomainObjectGetObjectService)));
    }

    [Test]
    public void GetService_FromISearchAvailableObjectsService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_bindableDomainObjectProvider, typeof (ISearchAvailableObjectsService)),
          Is.InstanceOfType (typeof (BindableDomainObjectSearchService)));
    }

    [Test]
    public void GetService_FromIGetObjectServiceWithBindableObjectProvider ()
    {
      Assert.That (
          _serviceFactory.CreateService (_bindableObjectProvider, typeof (IGetObjectService)),
          Is.Null);
    }

    [Test]
    public void GetService_FromISearchAvailableObjectsServiceWithBindableObjectProvider ()
    {
      Assert.That (
          _serviceFactory.CreateService (_bindableObjectProvider, typeof (ISearchAvailableObjectsService)),
          Is.Null);
    }

    [Test]
    public void GetService_FromUnknownService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_bindableDomainObjectProvider, typeof (IStubService)),
          Is.Null);
    }
  }
}
