using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.ReferencePropertyTests
{
  [TestFixture]
  public class SupportsSearchAvailableObjects : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;
    private MockRepository _mockRepository;

    public override void SetUp ()
    {
      base.SetUp ();

      _businessObjectProvider = new BindableObjectProvider ();
      _mockRepository = new MockRepository();
    }

    [Test]
    public void SearchServiceFromType_AndRequiresIdentity ()
    {
      ISearchServiceOnType mockService = _mockRepository.CreateMock<ISearchServiceOnType>();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromType", typeof (ClassWithSearchServiceTypeAttribute));

      Expect.Call (mockService.SupportsIdentity (property)).Return (true);
      _mockRepository.ReplayAll();

      _businessObjectProvider.AddService (typeof (ISearchServiceOnType), mockService);
      bool actual = property.SupportsSearchAvailableObjects (true);

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void SearchServiceFromType_AndNotRequiresIdentity ()
    {
      ISearchServiceOnType mockService = _mockRepository.CreateMock<ISearchServiceOnType>();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromType", typeof (ClassWithSearchServiceTypeAttribute));

      _mockRepository.ReplayAll();

      _businessObjectProvider.AddService (typeof (ISearchServiceOnType), mockService);
      bool actual = property.SupportsSearchAvailableObjects (false);

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void SearchServiceFromPropertyType ()
    {
      ISearchServiceOnProperty mockService = _mockRepository.CreateMock<ISearchServiceOnProperty>();
      ISearchServiceOnType stubSearchServiceOnType = _mockRepository.CreateMock<ISearchServiceOnType>();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromProperty", typeof (ClassWithSearchServiceTypeAttribute));

      Expect.Call (mockService.SupportsIdentity (property)).Return (true);
      _mockRepository.ReplayAll();

      _businessObjectProvider.AddService (typeof (ISearchServiceOnType), stubSearchServiceOnType);
      _businessObjectProvider.AddService (typeof (ISearchServiceOnProperty), mockService);
      bool actual = property.SupportsSearchAvailableObjects (true);

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void UnknownSearchService ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromType", typeof (ClassWithSearchServiceTypeAttribute));

      Assert.That (property.SupportsSearchAvailableObjects (false), Is.False);
    }

    [Test]
    public void WithoutSearchServiceAttribute_AndDefaultSearchService ()
    {
      ISearchAvailableObjectsService mockAvailableObjectsService = _mockRepository.CreateMock<ISearchAvailableObjectsService>();
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing ("NoSearchService", typeof (ClassWithOtherBusinessObjectImplementation));

      Expect.Call (mockAvailableObjectsService.SupportsIdentity (property)).Return (true);
      _mockRepository.ReplayAll();

      _businessObjectProvider.AddService (typeof (ISearchAvailableObjectsService), mockAvailableObjectsService);
      bool actual = property.SupportsSearchAvailableObjects (true);

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void WithoutSearchServiceAttribute_AndNoDefaultSearchService ()
    {
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing ("NoSearchService", typeof (ClassWithOtherBusinessObjectImplementation));

      Assert.That (property.SupportsSearchAvailableObjects (false), Is.False);
    }

    private ReferenceProperty CreateProperty (string propertyName, Type propertyType)
    {
      return new ReferenceProperty (
        GetPropertyParameters (GetPropertyInfo (typeof (ClassWithBusinessObjectProperties), propertyName), _businessObjectProvider),
          TypeFactory.GetConcreteType (propertyType));
    }

    private ReferenceProperty CreatePropertyWithoutMixing (string propertyName, Type propertyType)
    {
      return new ReferenceProperty (
          GetPropertyParameters (GetPropertyInfo (typeof (ClassWithBusinessObjectProperties), propertyName), _businessObjectProvider),
          propertyType);
    }
  }
}