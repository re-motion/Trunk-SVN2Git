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
using Remotion.Globalization;
using Remotion.Globalization.ExtensibleEnums;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.Reflection;
using Remotion.TypePipe;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.PropertyBaseTests
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
      PropertyBase propertyBase = new StubPropertyBase (
          CreateParameters (
              propertyInfo: propertyInfo,
              underlyingType: propertyInfo.PropertyType,
              concreteType: propertyInfo.PropertyType,
              listInfo: null,
              isRequired: true,
              isReadOnly: true));

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
          CreateParameters (
              propertyInfo: propertyInfo,
              underlyingType: propertyInfo.PropertyType,
              concreteType: propertyInfo.PropertyType,
              listInfo: null,
              isRequired: true,
              isReadOnly: true));
    }

    [Test]
    public void GetValue ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Scalar");
      PropertyBase propertyBase = new StubPropertyBase (
          CreateParameters (
              propertyInfo: propertyInfo,
              underlyingType: propertyInfo.PropertyType,
              concreteType: propertyInfo.PropertyType,
              listInfo: null,
              isRequired: true,
              isReadOnly: true));

      var instance = ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>> (ParamList.Empty);

      var value = new SimpleReferenceType();
      instance.Scalar = value;

      Assert.That (propertyBase.GetValue (((IBusinessObject) instance)), Is.SameAs (value));
    }

    [Test]
    public void GetValue_WithPrivatAccessor ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "PrivateProperty");
      PropertyBase propertyBase = new StubPropertyBase (
          CreateParameters (
              propertyInfo: propertyInfo,
              underlyingType: propertyInfo.PropertyType,
              concreteType: propertyInfo.PropertyType,
              listInfo: null,
              isRequired: true,
              isReadOnly: true));

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
      PropertyBase propertyBase = new StubPropertyBase (
          CreateParameters (
              propertyInfo: propertyInfo,
              underlyingType: propertyInfo.PropertyType,
              concreteType: propertyInfo.PropertyType,
              listInfo: null,
              isRequired: true,
              isReadOnly: true));

      var instance = ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>> (ParamList.Empty);

      propertyBase.GetValue (((IBusinessObject) instance));
    }

    [Test]
    public void SetValue ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Scalar");
      PropertyBase propertyBase = new StubPropertyBase (
          CreateParameters (
              propertyInfo: propertyInfo,
              underlyingType: propertyInfo.PropertyType,
              concreteType: propertyInfo.PropertyType,
              listInfo: null,
              isRequired: true,
              isReadOnly: true));

      var instance = ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>> (ParamList.Empty);

      var value = new SimpleReferenceType();
      propertyBase.SetValue ((IBusinessObject) instance, value);

      Assert.That (instance.Scalar, Is.SameAs (value));
    }

    [Test]
    public void SetValue_PrivateAccessor ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "PrivateProperty");
      PropertyBase propertyBase = new StubPropertyBase (
          CreateParameters (
              propertyInfo: propertyInfo,
              underlyingType: propertyInfo.PropertyType,
              concreteType: propertyInfo.PropertyType,
              listInfo: null,
              isRequired: true,
              isReadOnly: true));

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
      PropertyBase propertyBase = new StubPropertyBase (
          CreateParameters (
              propertyInfo: propertyInfo,
              underlyingType: propertyInfo.PropertyType,
              concreteType: propertyInfo.PropertyType,
              listInfo: null,
              isRequired: true,
              isReadOnly: true));

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
              CreateParameters (
                  propertyInfo: propertyInfo,
                  underlyingType: propertyInfo.PropertyType,
                  concreteType: propertyInfo.PropertyType,
                  listInfo: null,
                  isRequired: true,
                  isReadOnly: true));

      Assert.That (propertyBase.IsDefaultValue (businessObject), Is.False);
    }

    [Test]
    public void GetListInfo ()
    {
      IListInfo expected = new ListInfo (typeof (SimpleReferenceType[]), typeof (SimpleReferenceType));
      PropertyBase property = new StubPropertyBase (
          CreateParameters (
              propertyInfo: GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Array"),
              underlyingType: typeof (SimpleReferenceType),
              concreteType: typeof (SimpleReferenceType),
              listInfo: expected,
              isRequired: false,
              isReadOnly: false));

      Assert.That (property.IsList, Is.True);
      Assert.That (property.ListInfo, Is.SameAs (expected));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot access ListInfo for non-list properties.\r\nProperty: Scalar")]
    public void GetListInfo_WithNonListProperty ()
    {
      PropertyBase property = new StubPropertyBase (
          CreateParameters (
              propertyInfo: GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Scalar"),
              underlyingType: typeof (SimpleReferenceType),
              concreteType: typeof (SimpleReferenceType),
              listInfo: null,
              isRequired: false,
              isReadOnly: false));

      Assert.That (property.IsList, Is.False);
      Dev.Null = property.ListInfo;
    }


    [Test]
    public void ConvertFromNativePropertyType ()
    {
      PropertyBase property = new StubPropertyBase (
          CreateParameters (
              propertyInfo: GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Scalar"),
              underlyingType: typeof (SimpleReferenceType),
              concreteType: typeof (SimpleReferenceType),
              listInfo: null,
              isRequired: false,
              isReadOnly: false));
      var expected = new SimpleReferenceType();

      Assert.That (property.ConvertFromNativePropertyType (expected), Is.SameAs (expected));
    }

    [Test]
    public void ConvertToNativePropertyType_Scalar ()
    {
      PropertyBase property = new StubPropertyBase (
          CreateParameters (
              propertyInfo: GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Scalar"),
              underlyingType: typeof (SimpleReferenceType),
              concreteType: typeof (SimpleReferenceType),
              listInfo: null,
              isRequired: false,
              isReadOnly: false));
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
          CreateParameters (
              propertyInfo: propertyInfo,
              underlyingType: typeof (string),
              concreteType: typeof (string),
              listInfo: null,
              isRequired: false,
              isReadOnly: false));
      _bindableObjectProvider.AddService (
          typeof (BindableObjectGlobalizationService),
          new BindableObjectGlobalizationService (
              MockRepository.GenerateStub<IGlobalizationService>(),
              MockRepository.GenerateStub<IMemberInformationGlobalizationService>(),
              MockRepository.GenerateStub<IEnumerationGlobalizationService>(),
              MockRepository.GenerateStub<IExtensibleEnumGlobalizationService>()));

      Dev.Null = property.DisplayName;
    }

    [Test]
    public void GetDisplayName_WithGlobalizationSerivce ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String");
      PropertyBase property = new StubPropertyBase (
          CreateParameters (
              propertyInfo: propertyInfo,
              underlyingType: typeof (string),
              concreteType: typeof (string),
              listInfo: null,
              isRequired: false,
              isReadOnly: false));
      property.SetReflectedClass (_bindableObjectClass);

      var mockMemberInformationGlobalizationService = _mockRepository.StrictMock<IMemberInformationGlobalizationService>();
      _bindableObjectProvider.AddService (
          typeof (BindableObjectGlobalizationService),
          new BindableObjectGlobalizationService (
              MockRepository.GenerateStub<IGlobalizationService>(),
              mockMemberInformationGlobalizationService,
              MockRepository.GenerateStub<IEnumerationGlobalizationService>(),
              MockRepository.GenerateStub<IExtensibleEnumGlobalizationService>()));

      Expect.Call (
          mockMemberInformationGlobalizationService.TryGetPropertyDisplayName (
              Arg.Is (propertyInfo),
              Arg<ITypeInformation>.Matches (c => c.ConvertToRuntimeType() == _bindableObjectClass.TargetType),
              out Arg<string>.Out ("MockString").Dummy))
          .Return (true);
      _mockRepository.ReplayAll();

      string actual = property.DisplayName;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.EqualTo ("MockString"));
    }

    [Test]
    public void GetDisplayName_WithoutGlobalizationSerivce ()
    {
      PropertyBase property = new StubPropertyBase (
          CreateParameters (
              propertyInfo: GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String"),
              underlyingType: typeof (string),
              concreteType: typeof (string),
              listInfo: null,
              isRequired: false,
              isReadOnly: false));

      Assert.That (property.DisplayName, Is.EqualTo ("String"));
    }

    [Test]
    public void SetAndGetReflectedClass ()
    {
      PropertyBase property = new StubPropertyBase (
          CreateParameters (
              propertyInfo: GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String"),
              underlyingType: typeof (string),
              concreteType: typeof (string),
              listInfo: null,
              isRequired: false,
              isReadOnly: false));

      property.SetReflectedClass (_bindableObjectClass);

      Assert.That (property.ReflectedClass, Is.SameAs (_bindableObjectClass));
      Assert.That (((IBusinessObjectProperty) property).ReflectedClass, Is.SameAs (_bindableObjectClass));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage =
            "The BusinessObjectProvider of property 'String' does not match the BusinessObjectProvider of class "
            + "'Remotion.ObjectBinding.UnitTests.TestDomain.SimpleBusinessObjectClass, Remotion.ObjectBinding.UnitTests'."
            + "\r\nParameter name: reflectedClass")]
    public void SetReflectedClass_FromDifferentProviders ()
    {
      var provider = new BindableObjectProvider();
      BindableObjectClass bindableObjectClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (SimpleBusinessObjectClass));

      PropertyBase property = new StubPropertyBase (
          CreateParameters (
              businessObjectProvider: provider,
              propertyInfo: GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String"),
              underlyingType: typeof (string),
              concreteType: typeof (string),
              listInfo: null,
              isRequired: false,
              isReadOnly: false));

      property.SetReflectedClass (bindableObjectClass);

      Assert.That (property.ReflectedClass, Is.SameAs (bindableObjectClass));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage =
            "The ReflectedClass of a property cannot be changed after it was assigned."
            + "\r\nClass 'Remotion.ObjectBinding.UnitTests.TestDomain.SimpleBusinessObjectClass, Remotion.ObjectBinding.UnitTests'"
            + "\r\nProperty 'String'")]
    public void SetReflectedClass_Twice ()
    {
      BindableObjectClass bindableObjectClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (SimpleBusinessObjectClass));

      PropertyBase property = new StubPropertyBase (
          CreateParameters (
              propertyInfo: GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String"),
              underlyingType: typeof (string),
              concreteType: typeof (string),
              listInfo: null,
              isRequired: false,
              isReadOnly: false));

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
          CreateParameters (
              propertyInfo: GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String"),
              underlyingType: typeof (string),
              concreteType: typeof (string),
              listInfo: null,
              isRequired: false,
              isReadOnly: false));

      Dev.Null = property.ReflectedClass;
    }

    private new PropertyBase.Parameters CreateParameters (
        BindableObjectProvider businessObjectProvider = null,
        IPropertyInformation propertyInfo = null,
        Type underlyingType = null,
        Type concreteType = null,
        IListInfo listInfo = null,
        bool isRequired = false,
        bool isReadOnly = false)
    {
      return base.CreateParameters (
          businessObjectProvider ?? _bindableObjectProvider,
          propertyInfo,
          underlyingType,
          concreteType,
          listInfo,
          isRequired,
          isReadOnly);
    }

  }
}