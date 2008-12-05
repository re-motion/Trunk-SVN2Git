// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
//
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Remotion.Utilities;
using Rhino.Mocks;

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
      BusinessObjectProvider.SetProvider<BindableObjectProviderAttribute> (_bindableObjectProvider);
      BusinessObjectProvider.SetProvider<BindableObjectWithIdentityProviderAttribute> (_bindableObjectProvider);
      _mockRepository = new MockRepository ();
    }

    [Test]
    public void Initialize ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Scalar");
      PropertyBase propertyBase =
          new StubPropertyBase (
              new PropertyBase.Parameters (
                  _bindableObjectProvider,
                  IPropertyInformation,
                  IPropertyInformation.PropertyType,
                  null,
                  true,
                  true));

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
              _bindableObjectProvider,
              GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Array"),
              typeof (SimpleReferenceType),
              expected,
              false,
              false));

      Assert.That (property.IsList, Is.True);
      Assert.That (property.ListInfo, Is.SameAs (expected));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot access ListInfo for non-list properties.\r\nProperty: Scalar")]
    public void GetListInfo_WithNonListProperty ()
    {
      PropertyBase property = new StubPropertyBase (
          new PropertyBase.Parameters (
              _bindableObjectProvider,
              GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Scalar"),
              typeof (SimpleReferenceType),
              null,
              false,
              false));

      Assert.That (property.IsList, Is.False);
      Dev.Null = property.ListInfo;
    }


    [Test]
    public void ConvertFromNativePropertyType ()
    {
      PropertyBase property = new StubPropertyBase (
          new PropertyBase.Parameters (
              _bindableObjectProvider,
              GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Scalar"),
              typeof (SimpleReferenceType),
              null,
              false,
              false));
      SimpleReferenceType expected = new SimpleReferenceType();

      Assert.That (property.ConvertFromNativePropertyType (expected), Is.SameAs (expected));
    }

    [Test]
    public void ConvertToNativePropertyType_Scalar ()
    {
      PropertyBase property = new StubPropertyBase (
          new PropertyBase.Parameters (
              _bindableObjectProvider,
              GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Scalar"),
              typeof (SimpleReferenceType),
              null,
              false,
              false));
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
      IBindableObjectGlobalizationService mockGlobalizationService = _mockRepository.StrictMock<IBindableObjectGlobalizationService>();
      _bindableObjectProvider.AddService (typeof (IBindableObjectGlobalizationService), mockGlobalizationService);

      Expect.Call (mockGlobalizationService.GetPropertyDisplayName (IPropertyInformation)).Return ("MockString");
      _mockRepository.ReplayAll();

      string actual = property.DisplayName;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.EqualTo ("MockString"));
    }

    [Test]
    public void GetDisplayName_WithoutGlobalizationSerivce ()
    {
      PropertyBase property = new StubPropertyBase (
          new PropertyBase.Parameters (
              _bindableObjectProvider,
              GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String"),
              typeof (string),
              null,
              false,
              false));

      Assert.That (property.DisplayName, Is.EqualTo ("String"));
    }

    [Test]
    public void SetAndGetReflectedClass ()
    {
      Type type = typeof (SimpleBusinessObjectClass);
      ArgumentUtility.CheckNotNull ("type", type);
      BindableObjectClass bindableObjectClass = BindableObjectProvider.GetBindableObjectClass (type);
      PropertyBase property = new StubPropertyBase (
          new PropertyBase.Parameters (
              _bindableObjectProvider,
              GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String"),
              typeof (string),
              null,
              false,
              false));

      property.SetReflectedClass (bindableObjectClass);

      Assert.That (property.ReflectedClass, Is.SameAs (bindableObjectClass));
      Assert.That (((IBusinessObjectProperty)property).ReflectedClass, Is.SameAs (bindableObjectClass));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage =
        "The BusinessObjectProvider of property 'String' does not match the BusinessObjectProvider of class "
        + "'Remotion.ObjectBinding.UnitTests.Core.TestDomain.SimpleBusinessObjectClass, Remotion.ObjectBinding.UnitTests'."
        + "\r\nParameter name: reflectedClass")]
    public void SetReflectedClass_FromDifferentProviders ()
    {
      BindableObjectProvider provider = new BindableObjectProvider();
      Type type = typeof (SimpleBusinessObjectClass);
      ArgumentUtility.CheckNotNull ("type", type);
      BindableObjectClass bindableObjectClass = BindableObjectProvider.GetBindableObjectClass (type);

      PropertyBase property = new StubPropertyBase (
          new PropertyBase.Parameters (
              provider,
              GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String"),
              typeof (string),
              null,
              false,
              false));

      property.SetReflectedClass (bindableObjectClass);

      Assert.That (property.ReflectedClass, Is.SameAs (bindableObjectClass));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage =
        "The ReflectedClass of a property cannot be changed after it was assigned."
        + "\r\nClass 'Remotion.ObjectBinding.UnitTests.Core.TestDomain.SimpleBusinessObjectClass, Remotion.ObjectBinding.UnitTests'"
        + "\r\nProperty 'String'")]
    public void SetReflectedClass_Twice ()
    {
      Type type = typeof (SimpleBusinessObjectClass);
      ArgumentUtility.CheckNotNull ("type", type);
      BindableObjectClass bindableObjectClass = BindableObjectProvider.GetBindableObjectClass (type);

      PropertyBase property = new StubPropertyBase (
          new PropertyBase.Parameters (
              _bindableObjectProvider,
              GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String"),
              typeof (string),
              null,
              false,
              false));

      property.SetReflectedClass (bindableObjectClass);
      Type type1 = typeof (ClassWithIdentity);
      ArgumentUtility.CheckNotNull ("type", type1);
      property.SetReflectedClass (BindableObjectProvider.GetBindableObjectClass (type1));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage =
        "Accessing the ReflectedClass of a property is invalid until the property has been associated with a class.\r\nProperty 'String'")]
    public void GetReflectedClass_WithoutBusinessObjectClass ()
    {
      PropertyBase property = new StubPropertyBase (
          new PropertyBase.Parameters (
              _bindableObjectProvider,
              GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String"),
              typeof (string),
              null,
              false,
              false));

      Dev.Null = property.ReflectedClass;
    }
  }
}
