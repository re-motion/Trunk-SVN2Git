using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.ReferencePropertyTests
{
  [TestFixture]
  public class SearchAvailableObjects : TestBase
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
    public void Search_WithSearchSupported ()
    {
      IBusinessObject stubBusinessObject = _mockRepository.Stub<IBusinessObject>();
      ISearchServiceOnType mockService = _mockRepository.CreateMock<ISearchServiceOnType>();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromType", typeof (ClassWithSearchServiceTypeAttribute));
      IBusinessObject[] expected = new IBusinessObject[0];

      using (_mockRepository.Ordered())
      {
        Expect.Call (mockService.SupportsIdentity (property)).Return (true);
        Expect.Call (mockService.Search (stubBusinessObject, property, "*")).Return (expected);
      }
      _mockRepository.ReplayAll();

      _businessObjectProvider.AddService (typeof (ISearchServiceOnType), mockService);
      IBusinessObject[] actual = property.SearchAvailableObjects (stubBusinessObject, true, "*");

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException),
        ExpectedMessage =
        "Searching is not supported for reference property 'SearchServiceFromType' of business object class "
        + "'Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain.ClassWithSearchServiceTypeAttribute, Remotion.ObjectBinding.UnitTests'.")]
    public void Search_WithSearchNotSupported ()
    {
      IBusinessObject businessObject = (IBusinessObject) ObjectFactory.Create<ClassWithSearchServiceTypeAttribute>().With();
      ISearchServiceOnType mockService = _mockRepository.CreateMock<ISearchServiceOnType>();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromType", typeof (ClassWithSearchServiceTypeAttribute));

      Expect.Call (mockService.SupportsIdentity (property)).Return (false);
      _mockRepository.ReplayAll();

      _businessObjectProvider.AddService (typeof (ISearchServiceOnType), mockService);
      try
      {
        property.SearchAvailableObjects (businessObject, true, "*");
      }
      finally
      {
        _mockRepository.VerifyAll();
      }
    }

    private ReferenceProperty CreateProperty (string propertyName, Type propertyType)
    {
      return new ReferenceProperty (
        GetPropertyParameters (GetPropertyInfo (typeof (ClassWithBusinessObjectProperties), propertyName), _businessObjectProvider),  
        TypeFactory.GetConcreteType (propertyType));
    }
  }
}