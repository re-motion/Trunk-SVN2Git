using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.Data.DomainObjects.UnitTests.ObjectBinding
{
  [TestFixture]
  public class BindableDomainObjectServiceFactoryTest
  {
    private interface IStubService : IBusinessObjectService
    {}

    private IBusinessObjectServiceFactory _serviceFactory;

    [SetUp]
    public void SetUp ()
    {
      _serviceFactory = new BindableDomainObjectServiceFactory();
    }

    [Test]
    public void GetService_FromIBindableObjectGlobalizationService ()
    {
      Assert.That (
          _serviceFactory.CreateService (typeof (IBindableObjectGlobalizationService)),
          Is.InstanceOfType (typeof (BindableObjectGlobalizationService)));
    }

    [Test]
    public void GetService_FromIBusinessObjectStringFormatterService ()
    {
      Assert.That (
          _serviceFactory.CreateService (typeof (IBusinessObjectStringFormatterService)),
          Is.InstanceOfType (typeof (BusinessObjectStringFormatterService)));
    }

    [Test]
    public void GetService_FromIGetObjectService ()
    {
      Assert.That (
          _serviceFactory.CreateService (typeof (IGetObjectService)),
          Is.InstanceOfType (typeof (BindableDomainObjectGetObjectService)));
    }

    [Test]
    public void GetService_FromISearchAvailableObjectsService ()
    {
      Assert.That (
          _serviceFactory.CreateService (typeof (ISearchAvailableObjectsService)),
          Is.InstanceOfType (typeof (BindableDomainObjectSearchService)));
    }

    [Test]
    public void GetService_FromUnknownService ()
    {
      Assert.That (
          _serviceFactory.CreateService (typeof (IStubService)),
          Is.Null);
    }
  }
}