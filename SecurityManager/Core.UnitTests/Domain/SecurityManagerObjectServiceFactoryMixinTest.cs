using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain;
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
    private IBusinessObjectProviderWithIdentity _provider;

    [SetUp]
    public void SetUp ()
    {
      _serviceFactory = BindableObjectServiceFactory.Create();
      _serviceMixin = Mixin.Get<SecurityManagerObjectServiceFactoryMixin> (_serviceFactory);
      _provider = MockRepository.GenerateStub<IBusinessObjectProviderWithIdentity>();
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
          _serviceFactory.CreateService (_provider, typeof (UserPropertiesSearchService)),
          Is.InstanceOfType (typeof (UserPropertiesSearchService)));
    }

    [Test]
    public void GetService_FromGroupPropertiesSearchService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_provider, typeof (GroupPropertiesSearchService)),
          Is.InstanceOfType (typeof (GroupPropertiesSearchService)));
    }

    [Test]
    public void GetService_FromRolePropertiesSearchService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_provider, typeof (RolePropertiesSearchService)),
          Is.InstanceOfType (typeof (RolePropertiesSearchService)));
    }

    [Test]
    public void GetService_FromIBindableObjectGlobalizationService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_provider, typeof (IBindableObjectGlobalizationService)),
          Is.InstanceOfType (typeof (BindableObjectGlobalizationService)));
    }

    [Test]
    public void GetService_FromIGetObjectService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_provider, typeof (IGetObjectService)),
          Is.InstanceOfType (typeof (BindableDomainObjectGetObjectService)));
    }

    [Test]
    public void GetService_FromUnknownService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_provider, typeof (IStubService)),
          Is.Null);
    }
  }
}