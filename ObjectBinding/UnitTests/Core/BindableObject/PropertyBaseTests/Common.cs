using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.PropertyBaseTests
{
  [TestFixture]
  public class Common : TestBase
  {
    private BindableObjectProvider _bindableObjectProvider;
    private MockRepository _mockRepository;

    public override void SetUp ()
    {
      base.SetUp();

      _bindableObjectProvider = new BindableObjectProvider();
      _mockRepository = new MockRepository();
    }

    [Test]
    public void Initialize ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Scalar");
      PropertyBase propertyBase = new StubPropertyBase (new PropertyBase.Parameters (_bindableObjectProvider, IPropertyInformation, IPropertyInformation.PropertyType,
          null, true, true));

      Assert.That (propertyBase.PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (propertyBase.PropertyType, Is.SameAs (IPropertyInformation.PropertyType));
      Assert.That (propertyBase.IsRequired, Is.True);
      Assert.That (propertyBase.IsReadOnly (null), Is.True);
      Assert.That (propertyBase.BusinessObjectProvider, Is.SameAs (_bindableObjectProvider));
      Assert.That (((IBusinessObjectProperty) propertyBase).BusinessObjectProvider, Is.SameAs (_bindableObjectProvider));
    }

    [Test]
    public void GetListInfo ()
    {
      IListInfo expected = new ListInfo (typeof (SimpleReferenceType[]), typeof (SimpleReferenceType));
      PropertyBase property = new StubPropertyBase (
          new PropertyBase.Parameters (
              _bindableObjectProvider, GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Array"), typeof (SimpleReferenceType),
              expected, false, false));

      Assert.That (property.IsList, Is.True);
      Assert.That (property.ListInfo, Is.SameAs (expected));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot access ListInfo for non-list properties.\r\nProperty: Scalar")]
    public void GetListInfo_WithNonListProperty ()
    {
      PropertyBase property = new StubPropertyBase (
          new PropertyBase.Parameters (
              _bindableObjectProvider, GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Scalar"), typeof (SimpleReferenceType),
              null, false, false));

      Assert.That (property.IsList, Is.False);
      Dev.Null = property.ListInfo;
    }


    [Test]
    public void ConvertFromNativePropertyType ()
    {
      PropertyBase property = new StubPropertyBase (
          new PropertyBase.Parameters (
              _bindableObjectProvider, GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Scalar"), typeof (SimpleReferenceType),
              null, false, false));
      SimpleReferenceType expected = new SimpleReferenceType();

      Assert.That (property.ConvertFromNativePropertyType (expected), Is.SameAs (expected));
    }

    [Test]
    public void ConvertToNativePropertyType_Scalar ()
    {
      PropertyBase property = new StubPropertyBase (
          new PropertyBase.Parameters (
              _bindableObjectProvider, GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Scalar"), typeof (SimpleReferenceType),
              null, false, false));
      SimpleReferenceType expected = new SimpleReferenceType();

      Assert.That (property.ConvertToNativePropertyType (expected), Is.SameAs (expected));
    }

    [Test]
    public void GetDisplayName_WithGlobalizationSerivce ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String");
      PropertyBase property = new StubPropertyBase (
          new PropertyBase.Parameters (
              _bindableObjectProvider, IPropertyInformation, typeof (string), null, false, false));
      IBindableObjectGlobalizationService mockGlobalizationService = _mockRepository.CreateMock<IBindableObjectGlobalizationService> ();
      _bindableObjectProvider.AddService (typeof (IBindableObjectGlobalizationService), mockGlobalizationService);

      Expect.Call (mockGlobalizationService.GetPropertyDisplayName (IPropertyInformation)).Return ("MockString");
      _mockRepository.ReplayAll ();

      string actual = property.DisplayName;

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.EqualTo ("MockString"));
    }

    [Test]
    public void GetDisplayName_WithoutGlobalizationSerivce ()
    {
      PropertyBase property = new StubPropertyBase (
          new PropertyBase.Parameters (
              _bindableObjectProvider, GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String"), typeof (string),
              null, false, false));

      Assert.That (property.DisplayName, Is.EqualTo ("String"));
    }
  }
}