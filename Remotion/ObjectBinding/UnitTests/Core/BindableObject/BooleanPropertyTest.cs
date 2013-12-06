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
using Remotion.ExtensibleEnums.Globalization;
using Remotion.Globalization;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Remotion.Reflection;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class BooleanPropertyTest : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;
    private IBusinessObjectClass _businessObjectClass;

    private MockRepository _mockRepository;

    public override void SetUp ()
    {
      base.SetUp();

      _businessObjectProvider = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory ();
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithValueType<bool>), _businessObjectProvider, BindableObjectMetadataFactory.Create ());
      _businessObjectClass = classReflector.GetMetadata();

      _mockRepository = new MockRepository();
    }

    [Test]
    public void GetDefaultValue_Scalar ()
    {
      IBusinessObjectBooleanProperty property = CreateProperty ("Scalar");

      Assert.That (property.GetDefaultValue (_businessObjectClass), Is.False);
    }

    [Test]
    public void GetDefaultValue_NullableScalar ()
    {
      IBusinessObjectBooleanProperty property = CreateProperty ("NullableScalar");

      Assert.That (property.GetDefaultValue (_businessObjectClass), Is.Null);
    }

    [Test]
    public void GetDefaultValue_Array ()
    {
      IBusinessObjectBooleanProperty property = new BooleanProperty (
          new PropertyBase.Parameters (
              _businessObjectProvider,
              GetPropertyInfo (typeof (ClassWithValueType<bool>), "Array"),
              typeof (bool),
              typeof (bool),
              new ListInfo (typeof (bool[]), typeof (bool)),
              false,
              false,
              new BindableObjectDefaultValueStrategy ()));

      Assert.That (property.GetDefaultValue (_businessObjectClass), Is.False);
    }

    [Test]
    public void GetDefaultValue_NullableArray ()
    {
      IBusinessObjectBooleanProperty property = new BooleanProperty (
          new PropertyBase.Parameters (
              _businessObjectProvider,
              GetPropertyInfo (typeof (ClassWithValueType<bool>), "NullableArray"),
              typeof (bool),
              typeof (bool),
              new ListInfo (typeof (bool?[]), typeof (bool?)),
              false,
              false,
              new BindableObjectDefaultValueStrategy ()));

      Assert.That (property.GetDefaultValue (_businessObjectClass), Is.Null);
    }

    [Test]
    public void GetDisplayName_WithGlobalizationSerivce ()
    {
      IBusinessObjectBooleanProperty property = CreateProperty ("Scalar");
      var mockCompoundGlobalizationService = _mockRepository.StrictMock<ICompoundGlobalizationService>();
      _businessObjectProvider.AddService (
          typeof (BindableObjectGlobalizationService),
          new BindableObjectGlobalizationService (
              mockCompoundGlobalizationService,
              MockRepository.GenerateStub<IMemberInformationGlobalizationService>(),
              MockRepository.GenerateStub<IEnumerationGlobalizationService>(),
              MockRepository.GenerateStub<IExtensibleEnumerationGlobalizationService>()));

      var resourceIdentifierType = typeof (BindableObjectGlobalizationService).GetNestedType ("ResourceIdentifier", BindingFlags.NonPublic);

      var mockResourceManager = _mockRepository.StrictMock<IResourceManager>();
      Expect.Call (
          mockCompoundGlobalizationService.GetResourceManager (TypeAdapter.Create (resourceIdentifierType)))
          .Return (mockResourceManager);

      Expect.Call (
          mockResourceManager.TryGetString (
              Arg.Is ("Remotion.ObjectBinding.BindableObject.BindableObjectGlobalizationService.True"),
              out Arg<string>.Out ("MockTrue").Dummy))
          .Return (true);

      _mockRepository.ReplayAll();

      string actual = property.GetDisplayName (true);

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.EqualTo ("MockTrue"));
    }

    [Test]
    public void GetDisplayName_WithoutGlobalizationSerivce ()
    {
      IBusinessObjectBooleanProperty property = CreateProperty (
          "Scalar",
          new BindableObjectProvider (BindableObjectMetadataFactory.Create(), MockRepository.GenerateStub<IBusinessObjectServiceFactory>()));

      Assert.That (property.GetDisplayName (true), Is.EqualTo ("True"));
      Assert.That (property.GetDisplayName (false), Is.EqualTo ("False"));
    }


    [Test]
    public void IBusinessObjectEnumerationProperty_GetAllValues ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty ("NullableScalar");
      BooleanEnumerationValueInfo[] expected = new []
          {
              new BooleanEnumerationValueInfo (true, (IBusinessObjectBooleanProperty) property),
              new BooleanEnumerationValueInfo (false, (IBusinessObjectBooleanProperty) property)
          };

      CheckEnumerationValueInfos (expected, property.GetAllValues (null));
    }

    [Test]
    public void IBusinessObjectEnumerationProperty_GetEnabledValues ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty ("NullableScalar");
      BooleanEnumerationValueInfo[] expected = new []
          {
              new BooleanEnumerationValueInfo (true, (IBusinessObjectBooleanProperty) property),
              new BooleanEnumerationValueInfo (false, (IBusinessObjectBooleanProperty) property)
          };

      CheckEnumerationValueInfos (expected, property.GetEnabledValues (null));
    }

    [Test]
    public void IBusinessObjectEnumerationProperty_GetValueInfoByValue_WithTrue ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty ("Scalar");

      CheckEnumerationValueInfo (
          new BooleanEnumerationValueInfo (true, (IBusinessObjectBooleanProperty) property),
          property.GetValueInfoByValue (true, null));
    }

    [Test]
    public void IBusinessObjectEnumerationProperty_GetValueInfoByValue_WithFalse ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty ("Scalar");

      CheckEnumerationValueInfo (
          new BooleanEnumerationValueInfo (false, (IBusinessObjectBooleanProperty) property),
          property.GetValueInfoByValue (false, null));
    }

    [Test]
    public void IBusinessObjectEnumerationProperty_GetValueInfoByValue_WithNull ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty ("Scalar");

      Assert.That (property.GetValueInfoByValue (null, null), Is.Null);
    }

    [Test]
    public void IBusinessObjectEnumerationProperty_GetValueInfoByIdentifier_WithTrue ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty ("Scalar");

      CheckEnumerationValueInfo (
          new BooleanEnumerationValueInfo (true, (IBusinessObjectBooleanProperty) property),
          property.GetValueInfoByIdentifier ("True", null));
    }

    [Test]
    public void IBusinessObjectEnumerationProperty_GetValueInfoByIdentifier_WithFalse ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty ("Scalar");

      CheckEnumerationValueInfo (
          new BooleanEnumerationValueInfo (false, (IBusinessObjectBooleanProperty) property),
          property.GetValueInfoByIdentifier ("False", null));
    }

    [Test]
    public void IBusinessObjectEnumerationProperty_GetValueInfoByIdentifier_WithNull ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty ("Scalar");

      Assert.That (property.GetValueInfoByIdentifier (null, null), Is.Null);
    }

    [Test]
    public void IBusinessObjectEnumerationProperty_GetValueInfoByIdentifier_WithEmptyString ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty ("Scalar");

      Assert.That (property.GetValueInfoByIdentifier (string.Empty, null), Is.Null);
    }


    private BooleanProperty CreateProperty (string propertyName)
    {
      return CreateProperty (propertyName, _businessObjectProvider);
    }

    private BooleanProperty CreateProperty (string propertyName, BindableObjectProvider provider)
    {
      return new BooleanProperty (GetPropertyParameters (GetPropertyInfo (typeof (ClassWithValueType<bool>), propertyName), provider));
    }

    private void CheckEnumerationValueInfos (BooleanEnumerationValueInfo[] expected, IEnumerationValueInfo[] actual)
    {
      ArgumentUtility.CheckNotNull ("expected", expected);

      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.Length, Is.EqualTo (expected.Length));
      for (int i = 0; i < expected.Length; i++)
        CheckEnumerationValueInfo (expected[i], actual[i]);
    }

    private void CheckEnumerationValueInfo (BooleanEnumerationValueInfo expected, IEnumerationValueInfo actual)
    {
      ArgumentUtility.CheckNotNull ("expected", expected);

      Assert.That (actual, Is.InstanceOf (expected.GetType()));
      Assert.That (actual.Value, Is.EqualTo (expected.Value));
      Assert.That (actual.Identifier, Is.EqualTo (expected.Identifier));
      Assert.That (actual.IsEnabled, Is.EqualTo (expected.IsEnabled));
      Assert.That (actual.DisplayName, Is.EqualTo (expected.DisplayName));
    }
  }
}
