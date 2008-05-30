using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.BindableObject;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class BindableObjectServiceFactoryTest
  {
    private IBusinessObjectServiceFactory _serviceFactory;
    private IBusinessObjectProviderWithIdentity _provider;

    [SetUp]
    public void SetUp ()
    {
      _serviceFactory = BindableObjectServiceFactory.Create();
      _provider = MockRepository.GenerateStub<IBusinessObjectProviderWithIdentity>();
    }

    [Test]
    public void GetService_FromIBindableObjectGlobalizationService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_provider, typeof (IBindableObjectGlobalizationService)),
          Is.InstanceOfType (typeof (BindableObjectGlobalizationService)));
    }

    [Test]
    public void GetService_FromIBusinessObjectStringFormatterService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_provider, typeof (IBusinessObjectStringFormatterService)),
          Is.InstanceOfType (typeof (BusinessObjectStringFormatterService)));
    }

    [Test]
    public void GetService_FromIGetObjectService ()
    {
      Assert.That (_serviceFactory.CreateService (_provider, typeof (IGetObjectService)), Is.Null);
    }

    [Test]
    public void GetService_FromISearchAvailableObjectsService ()
    {
      Assert.That (_serviceFactory.CreateService (_provider, typeof (ISearchAvailableObjectsService)), Is.Null);
    }
  }
}