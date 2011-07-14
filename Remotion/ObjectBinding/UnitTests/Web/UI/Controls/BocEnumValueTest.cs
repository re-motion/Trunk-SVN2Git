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
using Remotion.Development.Web.UnitTesting.Configuration;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.UnitTests.Web.Domain;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{

[TestFixture]
public class BocEnumValueTest: BocTest
{
  private BocEnumValueMock _bocEnumValue;
  private TypeWithEnum _businessObject;
  private BusinessObjectReferenceDataSource _dataSource;
  private IBusinessObjectEnumerationProperty _propertyEnumValue;

  public BocEnumValueTest()
  {
  }

  
  [SetUp]
  public override void SetUp()
  {
    base.SetUp();
    _bocEnumValue = new BocEnumValueMock();
    _bocEnumValue.ID = "BocEnumValue";
    NamingContainer.Controls.Add (_bocEnumValue);

    _businessObject = TypeWithEnum.Create();

    _propertyEnumValue = (IBusinessObjectEnumerationProperty) ((IBusinessObject) _businessObject).BusinessObjectClass.GetPropertyDefinition ("EnumValue");
    
    _dataSource = new BusinessObjectReferenceDataSource();
    _dataSource.BusinessObject = (IBusinessObject) _businessObject;
  }


  [Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _bocEnumValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }

  [Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocEnumValue.ListControlStyle.AutoPostBack = true;
    _bocEnumValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }


  [Test]
  public void EvaluateWaiConformityDebugLevelAWithListControlStyleAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocEnumValue.ListControlStyle.AutoPostBack = true;
    _bocEnumValue.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasWarning);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocEnumValue, WcagHelperMock.Control);
    Assert.AreEqual ("ListControlStyle.AutoPostBack", WcagHelperMock.Property);
  }

  [Test]
  public void GetTrackedClientIDsInReadOnlyMode()
  {
    _bocEnumValue.ReadOnly = true;
    string[] actual = _bocEnumValue.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (0, actual.Length);
  }

  [Test]
  public void GetTrackedClientIDsInEditModeAsDropDownList()
  {
    _bocEnumValue.ReadOnly = false;
    _bocEnumValue.ListControlStyle.ControlType = ListControlType.DropDownList;
    string[] actual = _bocEnumValue.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (1, actual.Length);
    Assert.AreEqual (_bocEnumValue.GetListControlClientID(), actual[0]);
  }

  [Test]
  public void GetTrackedClientIDsInEditModeAsListBox()
  {
    _bocEnumValue.ReadOnly = false;
    _bocEnumValue.ListControlStyle.ControlType = ListControlType.ListBox;
    string[] actual = _bocEnumValue.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (1, actual.Length);
    Assert.AreEqual (_bocEnumValue.GetListControlClientID(), actual[0]);
  }

  [Test]
  public void GetTrackedClientIDsInEditModeAsRadioButtonList()
  {
    _bocEnumValue.ReadOnly = false;
    _bocEnumValue.ListControlStyle.ControlType = ListControlType.RadioButtonList;
    Assert.IsNotNull (_propertyEnumValue, "Could not find property 'EnumValue'.");
    Assert.IsTrue (
        typeof (IBusinessObjectEnumerationProperty).IsAssignableFrom (_propertyEnumValue.GetType()), 
        "Property 'EnumValue' of invalid type.");
    _bocEnumValue.Property = _propertyEnumValue;

    string[] actual = _bocEnumValue.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (3, actual.Length);
    Assert.AreEqual (_bocEnumValue.GetListControlClientID() + "_0", actual[0]);
    Assert.AreEqual (_bocEnumValue.GetListControlClientID() + "_1", actual[1]);
    Assert.AreEqual (_bocEnumValue.GetListControlClientID() + "_2", actual[2]);
  }


  [Test]
  public void SetValueToEnum()
  {
    _bocEnumValue.Property = _propertyEnumValue;
    _bocEnumValue.IsDirty = false;
    _bocEnumValue.Value = TestEnum.Second;
    Assert.AreEqual (TestEnum.Second, _bocEnumValue.Value);
    Assert.IsTrue (_bocEnumValue.IsDirty);
  }
    
  [Test]
  public void SetValueToNull()
  {
    _bocEnumValue.Property = _propertyEnumValue;
    _bocEnumValue.IsDirty = false;
    _bocEnumValue.Value = null;
    Assert.AreEqual (null, _bocEnumValue.Value);
    Assert.IsTrue (_bocEnumValue.IsDirty);
  }
  

  [Test]
  public void HasValue_ValueIsSet_ReturnsTrue ()
  {
    _bocEnumValue.Property = _propertyEnumValue;
    _bocEnumValue.Value = TestEnum.Second;
    Assert.IsTrue (_bocEnumValue.HasValue);
  }

  [Test]
  public void HasValue_ValueIsNull_ReturnsFalse ()
  {
    _bocEnumValue.Property = _propertyEnumValue;
    _bocEnumValue.Value = null;
    Assert.IsFalse (_bocEnumValue.HasValue);
  }


  [Test]
  public void LoadValueAndInterimTrue()
  {
    _businessObject.EnumValue = TestEnum.Second;
    _bocEnumValue.DataSource = _dataSource;
    _bocEnumValue.Property = _propertyEnumValue;
    _bocEnumValue.Value = null;
    _bocEnumValue.IsDirty = true;

    _bocEnumValue.LoadValue (true);
    Assert.AreEqual (null, _bocEnumValue.Value);
    Assert.IsTrue (_bocEnumValue.IsDirty);
  }

  [Test]
  public void LoadValueAndInterimFalseWithEnum()
  {
    _businessObject.EnumValue = TestEnum.Second;
    _bocEnumValue.DataSource = _dataSource;
    _bocEnumValue.Property = _propertyEnumValue;
    _bocEnumValue.Value = null;
    _bocEnumValue.IsDirty = true;

    _bocEnumValue.LoadValue (false);
    Assert.AreEqual (_businessObject.EnumValue, _bocEnumValue.Value);
    Assert.IsFalse (_bocEnumValue.IsDirty);
  }


  [Test]
  public void LoadUnboundValueAndInterimTrue()
  {
    TestEnum value = TestEnum.Second;
    _bocEnumValue.Property = _propertyEnumValue;
    _bocEnumValue.Value = null;
    _bocEnumValue.IsDirty = true;

    _bocEnumValue.LoadUnboundValue (value, true);
    Assert.AreEqual (null, _bocEnumValue.Value);
    Assert.IsTrue (_bocEnumValue.IsDirty);
  }

  [Test]
  public void LoadUnboundValueAndInterimFalseWithEnum()
  {
    TestEnum value = TestEnum.Second;
    _bocEnumValue.Property = _propertyEnumValue;
    _bocEnumValue.Value = null;
    _bocEnumValue.IsDirty = true;

    _bocEnumValue.LoadUnboundValue (value, false);
    Assert.AreEqual (value, _bocEnumValue.Value);
    Assert.IsFalse (_bocEnumValue.IsDirty);
  }

  [Test]
  public void LoadUnboundValueAndInterimFalseWithNull()
  {
    TestEnum? value = null;
    _bocEnumValue.Property = _propertyEnumValue;
    _bocEnumValue.Value = TestEnum.Second;
    _bocEnumValue.IsDirty = true;

    _bocEnumValue.LoadUnboundValue (value, false);
    Assert.AreEqual (value, _bocEnumValue.Value);
    Assert.IsFalse (_bocEnumValue.IsDirty);
  }
}

}
