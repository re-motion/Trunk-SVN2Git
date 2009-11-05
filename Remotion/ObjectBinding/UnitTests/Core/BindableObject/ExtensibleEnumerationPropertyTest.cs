// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class ExtensibleEnumerationPropertyTest: TestBase
  {
    private BindableObjectProvider _businessObjectProvider;
    private ExtensibleEnumerationProperty _propertyWithResources;
    private IBusinessObjectServiceFactory _serviceFactoryStub;

    public override void SetUp ()
    {
      base.SetUp();

      _serviceFactoryStub = MockRepository.GenerateStub<IBusinessObjectServiceFactory> ();
      _businessObjectProvider = new BindableObjectProvider (BindableObjectMetadataFactory.Create(), _serviceFactoryStub);
      _propertyWithResources = CreateProperty (typeof (ExtensibleEnumWithResources));
    }

    [Test]
    public void CreateEnumerationValueInfo_ValueAndIdentifier ()
    {
      var extensibleEnumInfo = ExtensibleEnumWithResources.Values.Value1().GetValueInfo();
      
      var info = _propertyWithResources.CreateEnumerationValueInfo (extensibleEnumInfo);

      Assert.That (info.Value, Is.SameAs (extensibleEnumInfo.Value));
      Assert.That (info.Identifier, Is.EqualTo (extensibleEnumInfo.Value.ID));
    }

    [Test]
    public void CreateEnumerationValueInfo_DisplayName_WithoutGlobalizationService ()
    {
      var extensibleEnumInfo = ExtensibleEnumWithResources.Values.Value1 ().GetValueInfo ();

      Assert.That (_businessObjectProvider.GetService (typeof (IBindableObjectGlobalizationService)), Is.Null);
      var info = _propertyWithResources.CreateEnumerationValueInfo (extensibleEnumInfo);

      Assert.That (info.DisplayName, Is.EqualTo (extensibleEnumInfo.Value.ToString ()));
    }

    [Test]
    public void CreateEnumerationValueInfo_DisplayName_WithGlobalizationService ()
    {
      var extensibleEnumInfo = ExtensibleEnumWithResources.Values.Value1 ().GetValueInfo ();
      var globalizationServiceMock = MockRepository.GenerateMock<IBindableObjectGlobalizationService> ();
      
      globalizationServiceMock.Expect (mock => mock.GetExtensibleEnumerationValueDisplayName (extensibleEnumInfo.Value)).Return ("DisplayName 1");
      globalizationServiceMock.Replay ();

      _businessObjectProvider.AddService (globalizationServiceMock);

      var info = _propertyWithResources.CreateEnumerationValueInfo (extensibleEnumInfo);

      globalizationServiceMock.VerifyAllExpectations ();
      Assert.That (info.DisplayName, Is.EqualTo ("DisplayName 1"));
    }

    [Test]
    [Ignore ("TODO 1824")]
    public void CreateEnumerationValueInfo_IsEnabled ()
    {
      Assert.Fail ();
    }

    [Test]
    public void GetAllValues ()
    {
      var businessObjectStub = MockRepository.GenerateStub<IBusinessObject> ();

      var valueInfos = _propertyWithResources.GetAllValues (businessObjectStub);

      Assert.That (valueInfos.Length, Is.EqualTo(3));

      Assert.That (valueInfos[0].Value, Is.EqualTo (ExtensibleEnumWithResources.Values.Value1 ()));
      Assert.That (valueInfos[1].Value, Is.EqualTo (ExtensibleEnumWithResources.Values.Value2 ()));
      Assert.That (valueInfos[2].Value, Is.EqualTo (ExtensibleEnumWithResources.Values.ValueWithoutResource ()));
    }

    [Test]
    [Ignore ("TODO 1824")]
    public void GetEnabledValues ()
    {
      Assert.Fail ();
    }

    [Test]
    [Ignore ("TODO 1824")]
    public void GetValueInfoByIdentifier ()
    {
      Assert.Fail ();
    }

    [Test]
    [Ignore ("TODO 1824")]
    public void GetValueInfoByValue ()
    {
      Assert.Fail ();
    }

    private ExtensibleEnumerationProperty CreateProperty (Type propertyType)
    {
      var propertyStub = MockRepository.GenerateStub<IPropertyInformation> ();
      propertyStub.Stub (stub => stub.PropertyType).Return (propertyType);

      var parameters = GetPropertyParameters (propertyStub, _businessObjectProvider);
      return new ExtensibleEnumerationProperty (parameters);
    }
  }
}