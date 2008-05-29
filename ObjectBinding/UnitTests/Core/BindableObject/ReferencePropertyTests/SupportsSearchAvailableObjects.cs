using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.ReferencePropertyTests
{
  [TestFixture]
  public class SupportsSearchAvailableObjects : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;
    private MockRepository _mockRepository;

    public override void SetUp ()
    {
      base.SetUp();

      _businessObjectProvider = new BindableObjectProvider();
      _mockRepository = new MockRepository();
    }

    [Test]
    public void SearchServiceFromType_AndRequiresIdentity ()
    {
      ISearchServiceOnProperty mockService = _mockRepository.CreateMock<ISearchServiceOnProperty>();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromPropertyWithIdentity");

      Expect.Call (mockService.SupportsIdentity (property)).Return (true);
      _mockRepository.ReplayAll();

      _businessObjectProvider.AddService (mockService);
      bool actual = property.SupportsSearchAvailableObjects;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void SearchServiceFromType_AndNotRequiresIdentity ()
    {
      ISearchServiceOnType mockService = _mockRepository.CreateMock<ISearchServiceOnType>();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromType");

      _mockRepository.ReplayAll();

      _businessObjectProvider.AddService (mockService);
      bool actual = property.SupportsSearchAvailableObjects;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void SearchServiceFromPropertyType ()
    {
      ISearchServiceOnProperty mockService = _mockRepository.CreateMock<ISearchServiceOnProperty>();
      ISearchServiceOnType stubSearchServiceOnType = _mockRepository.CreateMock<ISearchServiceOnType>();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromPropertyWithIdentity");

      Expect.Call (mockService.SupportsIdentity (property)).Return (true);
      _mockRepository.ReplayAll();

      _businessObjectProvider.AddService (stubSearchServiceOnType);
      _businessObjectProvider.AddService (mockService);
      bool actual = property.SupportsSearchAvailableObjects;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void UnknownSearchService ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromType");

      Assert.That (property.SupportsSearchAvailableObjects, Is.False);
    }

    [Test]
    public void WithoutSearchServiceAttribute_AndDefaultSearchService ()
    {
      ISearchAvailableObjectsService mockAvailableObjectsService = _mockRepository.CreateMock<ISearchAvailableObjectsService>();
      IBusinessObjectClassService mockBusinessObjectClassService = _mockRepository.CreateMock<IBusinessObjectClassService>();
      IBusinessObjectClassWithIdentity mockBusinessObjectClassWithIdentity = _mockRepository.CreateMock<IBusinessObjectClassWithIdentity>();
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing ("NoSearchServiceWithIdentity");

      Expect.Call (mockBusinessObjectClassService.GetBusinessObjectClass (typeof (ClassWithIdentityFromOtherBusinessObjectImplementation)))
        .Return (mockBusinessObjectClassWithIdentity);
      Expect.Call (mockAvailableObjectsService.SupportsIdentity (property)).Return (true);
      _mockRepository.ReplayAll();

      _businessObjectProvider.AddService (mockAvailableObjectsService);
      _businessObjectProvider.AddService (mockBusinessObjectClassService);
      bool actual = property.SupportsSearchAvailableObjects;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void WithoutSearchServiceAttribute_AndNoDefaultSearchService ()
    {
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing ("NoSearchService");

      Assert.That (property.SupportsSearchAvailableObjects, Is.False);
    }

    private ReferenceProperty CreateProperty (string propertyName)
    {
      PropertyBase.Parameters propertyParameters = GetPropertyParameters (propertyName);
      return new ReferenceProperty (propertyParameters, TypeFactory.GetConcreteType (propertyParameters.UnderlyingType));
    }

    private ReferenceProperty CreatePropertyWithoutMixing (string propertyName)
    {
      PropertyBase.Parameters propertyParameters = GetPropertyParameters (propertyName);
      return new ReferenceProperty (propertyParameters, propertyParameters.UnderlyingType);
    }

    private PropertyBase.Parameters GetPropertyParameters (string propertyName)
    {
      return GetPropertyParameters (GetPropertyInfo (typeof (ClassWithBusinessObjectProperties), propertyName), _businessObjectProvider);
    }
  }
}