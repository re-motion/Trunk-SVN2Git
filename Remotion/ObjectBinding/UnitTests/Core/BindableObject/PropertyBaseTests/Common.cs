// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.PropertyBaseTests
{
  [TestFixture]
  public class Common : TestBase
  {
    private BindableObjectProvider _bindableObjectProvider;
    private MockRepository _mockRepository;
    private BindableObjectClass _bindableObjectClass;

    public override void SetUp ()
    {
      base.SetUp();

      _bindableObjectProvider = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();
      BusinessObjectProvider.SetProvider<BindableObjectProviderAttribute> (_bindableObjectProvider);
      BusinessObjectProvider.SetProvider<BindableObjectWithIdentityProviderAttribute> (_bindableObjectProvider);
      _mockRepository = new MockRepository();
      _bindableObjectClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (SimpleBusinessObjectClass));
    }

    [Test]
    public void Initialize ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Scalar");
      PropertyBase propertyBase =
          new StubPropertyBase (
              new PropertyBase.Parameters (
                  _bindableObjectProvider,
                  propertyInfo,
                  propertyInfo.PropertyType,
                  propertyInfo.PropertyType,
                  null,
                  true,
                  true,
                  new BindableObjectDefaultValueStrategy()));

      Assert.That (propertyBase.PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (propertyBase.PropertyType, Is.SameAs (propertyInfo.PropertyType));
      Assert.That (propertyBase.IsRequired, Is.True);
      Assert.That (propertyBase.IsReadOnly (null), Is.True);
      Assert.That (propertyBase.BusinessObjectProvider, Is.SameAs (_bindableObjectProvider));
      Assert.That (((IBusinessObjectProperty) propertyBase).BusinessObjectProvider, Is.SameAs (_bindableObjectProvider));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Indexed properties are not supported.")]
    public void Initialize_IndexedProperty ()
    {
      IPropertyInformation propertyInfo =
          PropertyInfoAdapter.Create (typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("Item", new[] { typeof (int) }));
      new StubPropertyBase (
          new PropertyBase.Parameters (
              _bindableObjectProvider,
              propertyInfo,
              propertyInfo.PropertyType,
              propertyInfo.PropertyType,
              null,
              true,
              true,
              new BindableObjectDefaultValueStrategy()));
    }

    [Test]
    public void GetValue ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Scalar");
      PropertyBase propertyBase =
          new StubPropertyBase (
              new PropertyBase.Parameters (
                  _bindableObjectProvider,
                  propertyInfo,
                  propertyInfo.PropertyType,
                  propertyInfo.PropertyType,
                  null,
                  true,
                  true,
                  new BindableObjectDefaultValueStrategy()));

      var instance = ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>> (ParamList.Empty);

      var value = new SimpleReferenceType();
      instance.Scalar = value;

      Assert.That (propertyBase.GetValue (((IBusinessObject) instance)), Is.SameAs (value));
    }

    [Test]
    public void GetValue_WithPrivatAccessor ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "PrivateProperty");
      PropertyBase propertyBase =
          new StubPropertyBase (
              new PropertyBase.Parameters (
                  _bindableObjectProvider,
                  propertyInfo,
                  propertyInfo.PropertyType,
                  propertyInfo.PropertyType,
                  null,
                  true,
                  true,
                  new BindableObjectDefaultValueStrategy()));

      var instance = ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>> (ParamList.Empty);

      var value = new SimpleReferenceType();
      PrivateInvoke.SetNonPublicProperty (instance, "PrivateProperty", value);

      Assert.That (propertyBase.GetValue (((IBusinessObject) instance)), Is.SameAs (value));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Property has no getter.")]
    public void GetValue_NoGetter ()
    {
      var propertyInfo = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInfo.Stub (stub => stub.PropertyType).Return (typeof (bool));
      propertyInfo.Stub (stub => stub.GetIndexParameters()).Return (new ParameterInfo[0]);
      propertyInfo.Stub (stub => stub.GetSetMethod (true)).Return (null);
      PropertyBase propertyBase =
          new StubPropertyBase (
              new PropertyBase.Parameters (
                  _bindableObjectProvider,
                  propertyInfo,
                  propertyInfo.PropertyType,
                  propertyInfo.PropertyType,
                  null,
                  true,
                  true,
                  new BindableObjectDefaultValueStrategy()));

      var instance = ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>> (ParamList.Empty);

      propertyBase.GetValue (((IBusinessObject) instance));
    }

    [Test]
    public void SetValue ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Scalar");
      PropertyBase propertyBase =
          new StubPropertyBase (
              new PropertyBase.Parameters (
                  _bindableObjectProvider,
                  propertyInfo,
                  propertyInfo.PropertyType,
                  propertyInfo.PropertyType,
                  null,
                  true,
                  true,
                  new BindableObjectDefaultValueStrategy()));

      var instance = ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>> (ParamList.Empty);

      var value = new SimpleReferenceType();
      propertyBase.SetValue ((IBusinessObject) instance, value);

      Assert.That (instance.Scalar, Is.SameAs (value));
    }

    [Test]
    public void SetValue_PrivateAccessor ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "PrivateProperty");
      PropertyBase propertyBase =
          new StubPropertyBase (
              new PropertyBase.Parameters (
                  _bindableObjectProvider,
                  propertyInfo,
                  propertyInfo.PropertyType,
                  propertyInfo.PropertyType,
                  null,
                  true,
                  true,
                  new BindableObjectDefaultValueStrategy()));

      var instance = ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>> (ParamList.Empty);

      var value = new SimpleReferenceType();
      propertyBase.SetValue ((IBusinessObject) instance, value);

      Assert.That (PrivateInvoke.GetNonPublicProperty (instance, "PrivateProperty"), Is.SameAs (value));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Property has no setter.")]
    public void GetValue_NoSetter ()
    {
      var propertyInfo = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInfo.Stub (stub => stub.PropertyType).Return (typeof (bool));
      propertyInfo.Stub (stub => stub.GetIndexParameters()).Return (new ParameterInfo[0]);
      propertyInfo.Stub (stub => stub.GetSetMethod (true)).Return (null);
      PropertyBase propertyBase =
          new StubPropertyBase (
              new PropertyBase.Parameters (
                  _bindableObjectProvider,
                  propertyInfo,
                  propertyInfo.PropertyType,
                  propertyInfo.PropertyType,
                  null,
                  true,
                  true,
                  new BindableObjectDefaultValueStrategy()));

      var instance = ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>> (ParamList.Empty);

      propertyBase.SetValue (((IBusinessObject) instance), new object());
    }

    [Test]
    public void GetDefaultValueStrategy ()
    {
      var businessObject = MockRepository.GenerateStub<IBusinessObject>();
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Scalar");
      PropertyBase propertyBase =
          new StubPropertyBase (
              new PropertyBase.Parameters (
                  _bindableObjectProvider,
                  propertyInfo,
                  propertyInfo.PropertyType,
                  propertyInfo.PropertyType,
                  null,
                  true,
                  true,
                  new BindableObjectDefaultValueStrategy()));

      Assert.That (propertyBase.IsDefaultValue (businessObject), Is.False);
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
              typeof (SimpleReferenceType),
              expected,
              false,
              false,
              new BindableObjectDefaultValueStrategy()));

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
              typeof (SimpleReferenceType),
              null,
              false,
              false,
              new BindableObjectDefaultValueStrategy()));

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
              typeof (SimpleReferenceType),
              null,
              false,
              false,
              new BindableObjectDefaultValueStrategy()));
      var expected = new SimpleReferenceType();

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
              typeof (SimpleReferenceType),
              null,
              false,
              false,
              new BindableObjectDefaultValueStrategy()));
      var expected = new SimpleReferenceType();

      Assert.That (property.ConvertToNativePropertyType (expected), Is.SameAs (expected));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "The reflected class for the property 'SimpleBusinessObjectClass.String' is not set.")]
    public void GetDisplayName_ReflectedClassNotSet ()
    {
      var propertyInfo = GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String");
      var property = new StubPropertyBase (
          new PropertyBase.Parameters (
              _bindableObjectProvider,
              propertyInfo,
              typeof (string),
              typeof (string),
              null,
              false,
              false,
              new BindableObjectDefaultValueStrategy()));
      var mockGlobalizationService = _mockRepository.StrictMock<IBindableObjectGlobalizationService>();
      _bindableObjectProvider.AddService (typeof (IBindableObjectGlobalizationService), mockGlobalizationService);

      var displayName = property.DisplayName;
    }

    [Test]
    public void GetDisplayName_WithGlobalizationSerivce ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String");
      PropertyBase property = new StubPropertyBase (
          new PropertyBase.Parameters (
              _bindableObjectProvider,
              propertyInfo,
              typeof (string),
              typeof (string),
              null,
              false,
              false,
              new BindableObjectDefaultValueStrategy()));
      property.SetReflectedClass (_bindableObjectClass);

      var mockGlobalizationService = _mockRepository.StrictMock<IBindableObjectGlobalizationService>();
      _bindableObjectProvider.AddService (typeof (IBindableObjectGlobalizationService), mockGlobalizationService);

      Expect.Call (mockGlobalizationService.GetPropertyDisplayName (propertyInfo, TypeAdapter.Create (_bindableObjectClass.TargetType)))
          .Return ("MockString");
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
              typeof (string),
              null,
              false,
              false,
              new BindableObjectDefaultValueStrategy()));

      Assert.That (property.DisplayName, Is.EqualTo ("String"));
    }

    [Test]
    public void SetAndGetReflectedClass ()
    {
      PropertyBase property = new StubPropertyBase (
          new PropertyBase.Parameters (
              _bindableObjectProvider,
              GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String"),
              typeof (string),
              typeof (string),
              null,
              false,
              false,
              new BindableObjectDefaultValueStrategy()));

      property.SetReflectedClass (_bindableObjectClass);

      Assert.That (property.ReflectedClass, Is.SameAs (_bindableObjectClass));
      Assert.That (((IBusinessObjectProperty) property).ReflectedClass, Is.SameAs (_bindableObjectClass));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage =
            "The BusinessObjectProvider of property 'String' does not match the BusinessObjectProvider of class "
            + "'Remotion.ObjectBinding.UnitTests.Core.TestDomain.SimpleBusinessObjectClass, Remotion.ObjectBinding.UnitTests'."
            + "\r\nParameter name: reflectedClass")]
    public void SetReflectedClass_FromDifferentProviders ()
    {
      var provider = new BindableObjectProvider();
      BindableObjectClass bindableObjectClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (SimpleBusinessObjectClass));

      PropertyBase property = new StubPropertyBase (
          new PropertyBase.Parameters (
              provider,
              GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String"),
              typeof (string),
              typeof (string),
              null,
              false,
              false,
              new BindableObjectDefaultValueStrategy()));

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
      BindableObjectClass bindableObjectClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (SimpleBusinessObjectClass));

      PropertyBase property = new StubPropertyBase (
          new PropertyBase.Parameters (
              _bindableObjectProvider,
              GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String"),
              typeof (string),
              typeof (string),
              null,
              false,
              false,
              new BindableObjectDefaultValueStrategy()));

      property.SetReflectedClass (bindableObjectClass);
      property.SetReflectedClass (BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (ClassWithIdentity)));
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
              typeof (string),
              null,
              false,
              false,
              new BindableObjectDefaultValueStrategy()));

      Dev.Null = property.ReflectedClass;
    }
  }
}