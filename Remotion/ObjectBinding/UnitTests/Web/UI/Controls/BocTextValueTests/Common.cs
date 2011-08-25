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
using Remotion.ObjectBinding.UnitTests.Web.Domain;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocTextValueTests
{
  [TestFixture]
  public class Common : BocTest
  {
    private BocTextValueMock _bocTextValue;
    private TypeWithString _businessObject;
    private BusinessObjectReferenceDataSource _dataSource;
    private IBusinessObjectStringProperty _propertyStringValue;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _bocTextValue = new BocTextValueMock();
      _bocTextValue.ID = "BocTextValue";
      NamingContainer.Controls.Add (_bocTextValue);

      _businessObject = TypeWithString.Create();

      _propertyStringValue =
          (IBusinessObjectStringProperty) ((IBusinessObject) _businessObject).BusinessObjectClass.GetPropertyDefinition ("StringValue");

      _dataSource = new BusinessObjectReferenceDataSource();
      _dataSource.BusinessObject = (IBusinessObject) _businessObject;
    }


    [Test]
    public void EvaluateWaiConformityDebugLevelUndefined ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
      _bocTextValue.EvaluateWaiConformity();

      Assert.IsFalse (WcagHelperMock.HasWarning);
      Assert.IsFalse (WcagHelperMock.HasError);
    }

    [Test]
    public void EvaluateWaiConformityLevelA ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
      _bocTextValue.TextBoxStyle.AutoPostBack = true;
      _bocTextValue.EvaluateWaiConformity();

      Assert.IsFalse (WcagHelperMock.HasWarning);
      Assert.IsFalse (WcagHelperMock.HasError);
    }


    [Test]
    public void EvaluateWaiConformityDebugLevelAWithTextBoxStyleAutoPostBackTrue ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
      _bocTextValue.TextBoxStyle.AutoPostBack = true;
      _bocTextValue.EvaluateWaiConformity();

      Assert.IsTrue (WcagHelperMock.HasWarning);
      Assert.AreEqual (1, WcagHelperMock.Priority);
      Assert.AreSame (_bocTextValue, WcagHelperMock.Control);
      Assert.AreEqual ("TextBoxStyle.AutoPostBack", WcagHelperMock.Property);
    }

    [Test]
    public void GetTrackedClientIDsInReadOnlyMode ()
    {
      _bocTextValue.ReadOnly = true;
      string[] actual = _bocTextValue.GetTrackedClientIDs();
      Assert.IsNotNull (actual);
      Assert.AreEqual (0, actual.Length);
    }

    [Test]
    public void GetTrackedClientIDsInEditMode ()
    {
      _bocTextValue.ReadOnly = false;
      string[] actual = _bocTextValue.GetTrackedClientIDs();
      Assert.IsNotNull (actual);
      Assert.AreEqual (1, actual.Length);
      Assert.AreEqual (_bocTextValue.GetTextBoxClientID(), actual[0]);
    }


    [Test]
    public void SetValueToString ()
    {
      string value = "Foo Bar";
      _bocTextValue.IsDirty = false;
      _bocTextValue.Value = value;
      Assert.AreEqual (value, _bocTextValue.Value);
      Assert.IsTrue (_bocTextValue.IsDirty);
    }

    [Test]
    public void SetValueToNull ()
    {
      _bocTextValue.IsDirty = false;
      _bocTextValue.Value = null;
      Assert.AreEqual (null, _bocTextValue.Value);
      Assert.IsTrue (_bocTextValue.IsDirty);
    }


    [Test]
    public void HasValue_ValueIsSet_ReturnsTrue ()
    {
      _bocTextValue.Value = "x";
      Assert.IsTrue (_bocTextValue.HasValue);
    }

    [Test]
    public void HasValue_TextContainsInvaliData_ReturnsTrue ()
    {
      _bocTextValue.ValueType = BocTextValueType.Date;
      _bocTextValue.Text = "x";
      Assert.IsTrue (_bocTextValue.HasValue);
    }

    [Test]
    public void HasValue_TextContainsOnlyWhitespace_ReturnsFalse ()
    {
      _bocTextValue.ValueType = BocTextValueType.Date;
      _bocTextValue.Text = "  ";
      Assert.IsFalse (_bocTextValue.HasValue);
      Assert.IsNull (_bocTextValue.Value);
    }

    [Test]
    public void HasValue_ValueIsNull_ReturnsFalse ()
    {
      _bocTextValue.Value = null;
      Assert.IsFalse (_bocTextValue.HasValue);
    }


    [Test]
    public void LoadValueAndInterimTrue ()
    {
      _businessObject.StringValue = "Foo Bar";
      _bocTextValue.DataSource = _dataSource;
      _bocTextValue.Property = _propertyStringValue;
      _bocTextValue.Value = null;
      _bocTextValue.IsDirty = true;

      _bocTextValue.LoadValue (true);
      Assert.AreEqual (null, _bocTextValue.Value);
      Assert.IsTrue (_bocTextValue.IsDirty);
    }

    [Test]
    public void LoadValueAndInterimFalseWithString ()
    {
      _businessObject.StringValue = "Foo Bar";
      _bocTextValue.DataSource = _dataSource;
      _bocTextValue.Property = _propertyStringValue;
      _bocTextValue.Value = null;
      _bocTextValue.IsDirty = true;

      _bocTextValue.LoadValue (false);
      Assert.AreEqual (_businessObject.StringValue, _bocTextValue.Value);
      Assert.IsFalse (_bocTextValue.IsDirty);
    }

    [Test]
    public void LoadValueAndInterimFalseWithNull ()
    {
      _businessObject.StringValue = null;
      _bocTextValue.DataSource = _dataSource;
      _bocTextValue.Property = _propertyStringValue;
      _bocTextValue.Value = "Foo Bar";
      _bocTextValue.IsDirty = true;

      _bocTextValue.LoadValue (false);
      Assert.AreEqual (_businessObject.StringValue, _bocTextValue.Value);
      Assert.IsFalse (_bocTextValue.IsDirty);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceNull ()
    {
      _bocTextValue.DataSource = null;
      _bocTextValue.Property = _propertyStringValue;
      _bocTextValue.Value = "Foo Bar";
      _bocTextValue.IsDirty = true;

      _bocTextValue.LoadValue (false);
      Assert.AreEqual ("Foo Bar", _bocTextValue.Value);
      Assert.IsTrue (_bocTextValue.IsDirty);
    }

    [Test]
    public void LoadValueAndInterimFalseWithPropertyNull ()
    {
      _bocTextValue.DataSource = _dataSource;
      _bocTextValue.Property = null;
      _bocTextValue.Value = "Foo Bar";
      _bocTextValue.IsDirty = true;

      _bocTextValue.LoadValue (false);
      Assert.AreEqual ("Foo Bar", _bocTextValue.Value);
      Assert.IsTrue (_bocTextValue.IsDirty);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceBusinessObjectNull ()
    {
      _dataSource.BusinessObject = null;
      _bocTextValue.DataSource = _dataSource;
      _bocTextValue.Property = _propertyStringValue;
      _bocTextValue.Value = "Foo Bar";
      _bocTextValue.IsDirty = true;

      _bocTextValue.LoadValue (false);
      Assert.AreEqual (null, _bocTextValue.Value);
      Assert.IsFalse (_bocTextValue.IsDirty);
    }


    [Test]
    public void LoadUnboundValueAndInterimTrue ()
    {
      string value = "Foo Bar";
      _bocTextValue.Value = null;
      _bocTextValue.IsDirty = true;

      _bocTextValue.LoadUnboundValue (value, true);
      Assert.AreEqual (null, _bocTextValue.Value);
      Assert.IsTrue (_bocTextValue.IsDirty);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithString ()
    {
      string value = "Foo Bar";
      _bocTextValue.Value = null;
      _bocTextValue.IsDirty = true;

      _bocTextValue.LoadUnboundValue (value, false);
      Assert.AreEqual (value, _bocTextValue.Value);
      Assert.IsFalse (_bocTextValue.IsDirty);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithNull ()
    {
      string value = null;
      _bocTextValue.Value = "Foo Bar";
      _bocTextValue.IsDirty = true;

      _bocTextValue.LoadUnboundValue (value, false);
      Assert.AreEqual (value, _bocTextValue.Value);
      Assert.IsFalse (_bocTextValue.IsDirty);
    }

    [Test]
    public void SaveValueAndInterimTrue ()
    {
      _businessObject.StringValue = "Foo Bar";
      _bocTextValue.DataSource = _dataSource;
      _bocTextValue.Property = _propertyStringValue;
      _bocTextValue.Value = null;
      _bocTextValue.IsDirty = true;

      _bocTextValue.SaveValue (true);
      Assert.AreEqual ("Foo Bar", _businessObject.StringValue);
      Assert.IsTrue (_bocTextValue.IsDirty);
    }

    [Test]
    public void SaveValueAndInterimFalse ()
    {
      _businessObject.StringValue = "Foo Bar";
      _bocTextValue.DataSource = _dataSource;
      _bocTextValue.Property = _propertyStringValue;
      _bocTextValue.Value = null;
      _bocTextValue.IsDirty = true;

      _bocTextValue.SaveValue (false);
      Assert.AreEqual (null, _businessObject.StringValue);
      Assert.IsFalse (_bocTextValue.IsDirty);
    }

    [Test]
    public void SaveValueAndIsDirtyFalse ()
    {
      _businessObject.StringValue = "Foo Bar";
      _bocTextValue.DataSource = _dataSource;
      _bocTextValue.Property = _propertyStringValue;
      _bocTextValue.Value = null;
      _bocTextValue.IsDirty = false;

      _bocTextValue.SaveValue (false);
      Assert.AreEqual ("Foo Bar", _businessObject.StringValue);
      Assert.IsFalse (_bocTextValue.IsDirty);
    }
  }
}
