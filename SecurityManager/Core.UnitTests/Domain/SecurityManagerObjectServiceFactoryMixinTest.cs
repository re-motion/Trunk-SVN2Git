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
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain
{
  [TestFixture]
  public class SecurityManagerObjectServiceFactoryMixinTest
  {
    private interface IStubService : IBusinessObjectService
    {}

    private IBusinessObjectServiceFactory _serviceFactory;
    private SecurityManagerObjectServiceFactoryMixin _serviceMixin;
    private IBusinessObjectProviderWithIdentity _bindableDomainObjectProvider;
    private IBusinessObjectProviderWithIdentity _bindableObjectProvider;
    private MockRepository _mockRepository;

    [SetUp]
    public void SetUp ()
    {
      _serviceFactory = BindableObjectServiceFactory.Create();
      _serviceMixin = Mixin.Get<SecurityManagerObjectServiceFactoryMixin> (_serviceFactory);
      _mockRepository = new MockRepository ();
      _bindableDomainObjectProvider = _mockRepository.Stub<IBusinessObjectProviderWithIdentity> ();
      _bindableObjectProvider = _mockRepository.Stub<IBusinessObjectProviderWithIdentity> ();
      SetupResult.For (_bindableDomainObjectProvider.ProviderAttribute).Return (new BindableDomainObjectProviderAttribute ());
      SetupResult.For (_bindableObjectProvider.ProviderAttribute).Return (new BindableObjectProviderAttribute ());
      _mockRepository.ReplayAll ();
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (_serviceMixin, Is.Not.Null);
      Assert.That (_serviceMixin, Is.InstanceOfType (typeof (IBusinessObjectServiceFactory)));
      Assert.That (Mixin.Get<BindableDomainObjectServiceFactoryMixin> (_serviceFactory), Is.Not.Null);
    }

    [Test]
    public void GetService_FromUserPropertiesSearchService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_bindableObjectProvider, typeof (UserPropertiesSearchService)),
          Is.InstanceOfType (typeof (UserPropertiesSearchService)));
    }

    [Test]
    public void GetService_FromGroupPropertiesSearchService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_bindableObjectProvider, typeof (GroupPropertiesSearchService)),
          Is.InstanceOfType (typeof (GroupPropertiesSearchService)));
    }

    [Test]
    public void GetService_FromRolePropertiesSearchService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_bindableObjectProvider, typeof (RolePropertiesSearchService)),
          Is.InstanceOfType (typeof (RolePropertiesSearchService)));
    }

    [Test]
    public void GetService_FromAccessControlEntryPropertiesSearchService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_bindableObjectProvider, typeof (AccessControlEntryPropertiesSearchService)),
          Is.InstanceOfType (typeof (AccessControlEntryPropertiesSearchService)));
    }

    [Test]
    public void GetService_FromIBindableObjectGlobalizationService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_bindableObjectProvider, typeof (IBindableObjectGlobalizationService)),
          Is.InstanceOfType (typeof (BindableObjectGlobalizationService)));
    }

    [Test]
    public void GetService_FromIGetObjectService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_bindableDomainObjectProvider, typeof (IGetObjectService)),
          Is.InstanceOfType (typeof (BindableDomainObjectGetObjectService)));
    }

    [Test]
    public void GetService_FromUnknownService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_bindableObjectProvider, typeof (IStubService)),
          Is.Null);
    }
  }
}
