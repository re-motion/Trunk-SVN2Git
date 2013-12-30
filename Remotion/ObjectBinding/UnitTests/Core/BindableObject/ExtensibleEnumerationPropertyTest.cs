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
using Remotion.ExtensibleEnums;
using Remotion.Globalization;
using Remotion.Globalization.ExtensibleEnums;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class ExtensibleEnumerationPropertyTest: TestBase
  {
    private BindableObjectProvider _businessObjectProvider;
    private ExtensibleEnumerationProperty _propertyWithResources;
    private ExtensibleEnumerationProperty _propertyWithFilteredType;

    public override void SetUp ()
    {
      base.SetUp();

      _businessObjectProvider = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();

      _propertyWithResources = CreateProperty (typeof (ExtensibleEnumWithResources));
      _propertyWithFilteredType = CreateProperty (typeof (ExtensibleEnumWithFilter));
    }

    [Test]
    public void CreateEnumerationValueInfo_ValueAndIdentifier ()
    {
      var extensibleEnumInfo = ExtensibleEnumWithResources.Values.Value1().GetValueInfo();
      
      var info = _propertyWithResources.CreateEnumerationValueInfo (extensibleEnumInfo, null);

      Assert.That (info.Value, Is.SameAs (extensibleEnumInfo.Value));
      Assert.That (info.Identifier, Is.EqualTo (extensibleEnumInfo.Value.ID));
    }

    [Test]
    public void CreateEnumerationValueInfo_DisplayName_WithoutGlobalizationService ()
    {
      var extensibleEnumInfo = ExtensibleEnumWithResources.Values.Value1 ().GetValueInfo ();

      Assert.That (_businessObjectProvider.GetService (typeof (BindableObjectGlobalizationService)), Is.Null);
      var info = _propertyWithResources.CreateEnumerationValueInfo (extensibleEnumInfo, null);

      Assert.That (info.DisplayName, Is.EqualTo (extensibleEnumInfo.Value.ToString ()));
    }

    [Test]
    public void CreateEnumerationValueInfo_DisplayName_WithGlobalizationService ()
    {
      var extensibleEnumInfo = ExtensibleEnumWithResources.Values.Value1 ().GetValueInfo ();
      var mockExtensibleEnumerationGlobalizationService = MockRepository.GenerateMock<IExtensibleEnumerationGlobalizationService> ();

      _businessObjectProvider.AddService (
          typeof (BindableObjectGlobalizationService),
          new BindableObjectGlobalizationService (
              MockRepository.GenerateStub<ICompoundGlobalizationService>(),
              MockRepository.GenerateStub<IMemberInformationGlobalizationService>(),
              MockRepository.GenerateStub<IEnumerationGlobalizationService>(),
              mockExtensibleEnumerationGlobalizationService));

      mockExtensibleEnumerationGlobalizationService
          .Expect (mock => mock.TryGetExtensibleEnumerationValueDisplayName (Arg.Is(extensibleEnumInfo.Value), out Arg<string>.Out("DisplayName 1").Dummy))
          .Return (true);
      mockExtensibleEnumerationGlobalizationService.Replay ();

      var info = _propertyWithResources.CreateEnumerationValueInfo (extensibleEnumInfo, null);

      mockExtensibleEnumerationGlobalizationService.VerifyAllExpectations ();
      Assert.That (info.DisplayName, Is.EqualTo ("DisplayName 1"));
    }

    [Test]
    public void CreateEnumerationValueInfo_IsEnabled_True ()
    {
      var extensibleEnumInfo = ExtensibleEnumWithResources.Values.Value1 ().GetValueInfo ();
      var info = _propertyWithResources.CreateEnumerationValueInfo (extensibleEnumInfo, null);

      Assert.That (info.IsEnabled, Is.True);
    }

    [Test]
    public void CreateEnumerationValueInfo_IsEnabled_False_ViaFilter ()
    {
      var extensibleEnumInfo = ExtensibleEnumWithResources.Values.Value1 ().GetValueInfo ();
      var businessObjectStub = MockRepository.GenerateStub<IBusinessObject> ();

      var filterMock = new MockRepository().StrictMock<IEnumerationValueFilter> ();

      var property = CreatePropertyWithSpecificFilter (filterMock);

      // the filter must be called exactly as specified
      filterMock
          .Expect (mock => mock.IsEnabled (
              Arg<IEnumerationValueInfo>.Matches (
                  i => i.Value.Equals (extensibleEnumInfo.Value)
                       && i.Identifier == extensibleEnumInfo.Value.ID
                       && i.DisplayName == null
                       && i.IsEnabled),
              Arg.Is (businessObjectStub),
              Arg.Is (property)))
          .Return (false);
      filterMock.Replay();
      
      var info = property.CreateEnumerationValueInfo (extensibleEnumInfo, businessObjectStub);

      filterMock.VerifyAllExpectations ();

      Assert.That (info.IsEnabled, Is.False);
    }

    [Test]
    public void IsEnabled_IntegrationTest ()
    {
      Assert.That (IsEnabled (_propertyWithFilteredType, ExtensibleEnumWithFilter.Values.Value1 ()),  Is.False);
      Assert.That (IsEnabled (_propertyWithFilteredType, ExtensibleEnumWithFilter.Values.Value2 ()), Is.True);
      Assert.That (IsEnabled (_propertyWithFilteredType, ExtensibleEnumWithFilter.Values.Value3 ()), Is.False);
      Assert.That (IsEnabled (_propertyWithFilteredType, ExtensibleEnumWithFilter.Values.Value4 ()), Is.True);

      var propertyInfoStub = MockRepository.GenerateStub<IPropertyInformation> ();
      propertyInfoStub.Stub (stub => stub.PropertyType).Return (typeof (ExtensibleEnumWithResources));
      propertyInfoStub.Stub (stub => stub.GetIndexParameters ()).Return (new ParameterInfo[0]);
      propertyInfoStub
          .Stub (stub => stub.GetCustomAttribute<DisableExtensibleEnumValuesAttribute> (true))
          .Return (new DisableExtensibleEnumValuesAttribute (ExtensibleEnumWithResources.Values.Value1().ID));

      var propertyWithFilter = new ExtensibleEnumerationProperty (GetPropertyParameters (propertyInfoStub, _businessObjectProvider));
      Assert.That (IsEnabled (propertyWithFilter, ExtensibleEnumWithResources.Values.Value1 ()), Is.False);
      Assert.That (IsEnabled (propertyWithFilter, ExtensibleEnumWithResources.Values.Value2 ()), Is.True);
    }

    [Test]
    public void GetAllValues ()
    {
      var valueInfos = _propertyWithResources.GetAllValues (null);

      Assert.That (valueInfos.Length, Is.EqualTo(3));

      Assert.That (valueInfos[0].Value, Is.EqualTo (ExtensibleEnumWithResources.Values.Value1 ()));
      Assert.That (valueInfos[1].Value, Is.EqualTo (ExtensibleEnumWithResources.Values.Value2 ()));
      Assert.That (valueInfos[2].Value, Is.EqualTo (ExtensibleEnumWithResources.Values.ValueWithoutResource ()));
    }

    [Test]
    public void GetEnabledValues ()
    {
      var valueInfos = _propertyWithFilteredType.GetEnabledValues (null);

      Assert.That (valueInfos.Length, Is.EqualTo (2));

      Assert.That (valueInfos[0].Value, Is.EqualTo (ExtensibleEnumWithFilter.Values.Value2 ()));
      Assert.That (valueInfos[1].Value, Is.EqualTo (ExtensibleEnumWithFilter.Values.Value4 ()));
    }

    [Test]
    public void GetValueInfoByIdentifier ()
    {
      var info = _propertyWithResources.GetValueInfoByIdentifier (ExtensibleEnumWithResources.Values.Value1().ID, null);
      Assert.That (info.Value, Is.EqualTo (ExtensibleEnumWithResources.Values.Value1()));
    }

    [Test]
    public void GetValueInfoByIdentifier_NullOrEmpty ()
    {
      Assert.That (_propertyWithResources.GetValueInfoByIdentifier ("", null), Is.Null);
      Assert.That (_propertyWithResources.GetValueInfoByIdentifier (null, null), Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The identifier '?' does not identify a defined value for type " 
        + "'Remotion.ObjectBinding.UnitTests.Core.TestDomain.ExtensibleEnumWithResources'.\r\nParameter name: identifier")]
    public void GetValueInfoByIdentifier_InvalidID ()
    {
      _propertyWithResources.GetValueInfoByIdentifier ("?", null);
    }

    [Test]
    public void GetValueInfoByValue ()
    {
      var info = _propertyWithResources.GetValueInfoByValue (ExtensibleEnumWithResources.Values.Value1 (), null);
      Assert.That (info.Value, Is.EqualTo (ExtensibleEnumWithResources.Values.Value1()));
    }

    [Test]
    public void GetValueInfoByValue_Null ()
    {
      var info = _propertyWithResources.GetValueInfoByValue (null, null);
      Assert.That (info, Is.Null);
    }

    [Test]
    public void GetValueInfoByValue_UndefinedValue ()
    {
      var info = _propertyWithResources.GetValueInfoByValue (new ExtensibleEnumWithResources (MethodBase.GetCurrentMethod()), null);
      Assert.That (info, Is.Null);
    }

    [Test]
    public void GetValueInfoByValue_InvalidType ()
    {
      var info = _propertyWithResources.GetValueInfoByValue ("?", null);
      Assert.That (info, Is.Null);
    }

    private ExtensibleEnumerationProperty CreateProperty (Type propertyType)
    {
      var propertyStub = MockRepository.GenerateStub<IPropertyInformation> ();
      propertyStub.Stub (stub => stub.PropertyType).Return (propertyType);
      propertyStub.Stub (stub => stub.GetIndexParameters ()).Return (new ParameterInfo[0]);
      
      var parameters = GetPropertyParameters (propertyStub, _businessObjectProvider);
      return new ExtensibleEnumerationProperty (parameters);
    }

    private ExtensibleEnumerationProperty CreatePropertyWithSpecificFilter (IEnumerationValueFilter filterMock)
    {
      var attribute = new DisableExtensibleEnumValuesAttribute ("x");
      PrivateInvoke.SetNonPublicField (attribute, "_filter", filterMock);

      var propertyInfoStub = MockRepository.GenerateStub<IPropertyInformation> ();
      propertyInfoStub.Stub (stub => stub.PropertyType).Return (typeof (ExtensibleEnumWithResources));
      propertyInfoStub.Stub (stub => stub.GetIndexParameters ()).Return (new ParameterInfo[0]);
      propertyInfoStub
          .Stub (stub => stub.GetCustomAttribute<DisableExtensibleEnumValuesAttribute> (true))
          .Return (attribute);

      return new ExtensibleEnumerationProperty (GetPropertyParameters (propertyInfoStub, _businessObjectProvider));
    }
    
    private bool IsEnabled (ExtensibleEnumerationProperty propertyWithFilteredType, IExtensibleEnum value)
    {
      return propertyWithFilteredType.CreateEnumerationValueInfo (value.GetValueInfo (), null).IsEnabled;
    }
  }
}