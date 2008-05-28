using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class BusinessObjectServiceFactoryTest
  {
    private IBusinessObjectServiceFactory _serviceFactory;

    [SetUp]
    public void SetUp ()
    {
      _serviceFactory = new BusinessObjectServiceFactory ();
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
      Assert.That (_serviceFactory.CreateService (typeof (IGetObjectService)), Is.Null);
    }

    [Test]
    public void GetService_FromISearchAvailableObjectsService ()
    {
      Assert.That (_serviceFactory.CreateService (typeof (ISearchAvailableObjectsService)), Is.Null);
    }
  }
}