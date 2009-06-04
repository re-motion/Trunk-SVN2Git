// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Development.Web.UnitTesting.Configuration;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.UnitTests.Web.Domain;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{
  [TestFixture]
  public class BocDateTimeValueTest: BocTest
  {
    private BocDateTimeValueMock _bocDateTimeValue;
    private TypeWithDateTime _businessObject;
    private BusinessObjectReferenceDataSource _dataSource;
    private IBusinessObjectDateTimeProperty _propertyDateTimeValue;
    private IBusinessObjectDateTimeProperty _propertyNullableDateTimeValue;

    public BocDateTimeValueTest()
    {
    }


    [SetUp]
    public override void SetUp()
    {
      base.SetUp();
      _bocDateTimeValue = new BocDateTimeValueMock();
      _bocDateTimeValue.ID = "BocDateTimeValue";
      NamingContainer.Controls.Add (_bocDateTimeValue);

      _businessObject = TypeWithDateTime.Create();

      _propertyDateTimeValue = (IBusinessObjectDateTimeProperty) ((IBusinessObject) _businessObject).BusinessObjectClass.GetPropertyDefinition ("DateTimeValue");
      _propertyNullableDateTimeValue = (IBusinessObjectDateTimeProperty) ((IBusinessObject) _businessObject).BusinessObjectClass.GetPropertyDefinition ("NullableDateTimeValue");

      _dataSource = new BusinessObjectReferenceDataSource();
      _dataSource.BusinessObject = (IBusinessObject) _businessObject;
    }


    [Test]
    public void EvaluateWaiConformityDebugLevelUndefined()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
      _bocDateTimeValue.EvaluateWaiConformity();

      Assert.IsFalse (WcagHelperMock.HasWarning);
      Assert.IsFalse (WcagHelperMock.HasError);
    }

    [Test]
    public void EvaluateWaiConformityLevelA()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
      _bocDateTimeValue.DateTextBoxStyle.AutoPostBack = true;
      _bocDateTimeValue.EvaluateWaiConformity();

      Assert.IsFalse (WcagHelperMock.HasWarning);
      Assert.IsFalse (WcagHelperMock.HasError);
    }

    [Test]
    public void EvaluateWaiConformityDebugLevelDoubleAWithTimeTextBoxActive()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelDoubleA();
      _bocDateTimeValue.ValueType = BocDateTimeValueType.DateTime;
      _bocDateTimeValue.EvaluateWaiConformity();

      Assert.IsTrue (WcagHelperMock.HasError);
      Assert.AreEqual (2, WcagHelperMock.Priority);
      Assert.AreSame (_bocDateTimeValue, WcagHelperMock.Control);
      Assert.AreEqual ("ActualValueType", WcagHelperMock.Property);
    }

    [Test]
    public void EvaluateWaiConformityDebugLevelAWithDateTimeTextBoxStyleAutoPostBackTrue()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
      _bocDateTimeValue.DateTimeTextBoxStyle.AutoPostBack = true;
      _bocDateTimeValue.EvaluateWaiConformity();

      Assert.IsTrue (WcagHelperMock.HasWarning);
      Assert.AreEqual (1, WcagHelperMock.Priority);
      Assert.AreSame (_bocDateTimeValue, WcagHelperMock.Control);
      Assert.AreEqual ("DateTimeTextBoxStyle.AutoPostBack", WcagHelperMock.Property);
    }


    [Test]
    public void EvaluateWaiConformityDebugLevelAWithDateTextBoxStyleAutoPostBackTrue()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
      _bocDateTimeValue.DateTextBoxStyle.AutoPostBack = true;
      _bocDateTimeValue.EvaluateWaiConformity();

      Assert.IsTrue (WcagHelperMock.HasWarning);
      Assert.AreEqual (1, WcagHelperMock.Priority);
      Assert.AreSame (_bocDateTimeValue, WcagHelperMock.Control);
      Assert.AreEqual ("DateTextBoxStyle.AutoPostBack", WcagHelperMock.Property);
    }

    [Test]
    public void EvaluateWaiConformityDebugLevelAWithTimeTextBoxStyleAutoPostBackTrue()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
      _bocDateTimeValue.TimeTextBoxStyle.AutoPostBack = true;
      _bocDateTimeValue.EvaluateWaiConformity();

      Assert.IsTrue (WcagHelperMock.HasWarning);
      Assert.AreEqual (1, WcagHelperMock.Priority);
      Assert.AreSame (_bocDateTimeValue, WcagHelperMock.Control);
      Assert.AreEqual ("TimeTextBoxStyle.AutoPostBack", WcagHelperMock.Property);
    }

    [Test]
    public void GetTrackedClientIDsInReadOnlyMode()
    {
      _bocDateTimeValue.ReadOnly = true;
      string[] actual = _bocDateTimeValue.GetTrackedClientIDs();
      Assert.IsNotNull (actual);
      Assert.AreEqual (0, actual.Length);
    }

    [Test]
    public void GetTrackedClientIDsInEditModeAndValueTypeIsDateTime()
    {
      _bocDateTimeValue.ReadOnly = false;
      _bocDateTimeValue.ValueType = BocDateTimeValueType.DateTime;
      string[] actual = _bocDateTimeValue.GetTrackedClientIDs();
      Assert.IsNotNull (actual);
      Assert.AreEqual (2, actual.Length);
      Assert.AreEqual (_bocDateTimeValue.GetDateTextboxClientId(), actual[0]);
      Assert.AreEqual (_bocDateTimeValue.GetTimeTextboxClientId(), actual[1]);
    }

    [Test]
    public void GetTrackedClientIDsInEditModeAndValueTypeIsDate()
    {
      _bocDateTimeValue.ReadOnly = false;
      _bocDateTimeValue.ValueType = BocDateTimeValueType.Date;
      string[] actual = _bocDateTimeValue.GetTrackedClientIDs();
      Assert.IsNotNull (actual);
      Assert.AreEqual (1, actual.Length);
      Assert.AreEqual (_bocDateTimeValue.GetDateTextboxClientId(), actual[0]);
    }

    [Test]
    public void GetTrackedClientIDsInEditModeAndValueTypeIsUndefined()
    {
      _bocDateTimeValue.ReadOnly = false;
      _bocDateTimeValue.ValueType = BocDateTimeValueType.Undefined;
      string[] actual = _bocDateTimeValue.GetTrackedClientIDs();
      Assert.IsNotNull (actual);
      Assert.AreEqual (2, actual.Length);
      Assert.AreEqual (_bocDateTimeValue.GetDateTextboxClientId(), actual[0]);
      Assert.AreEqual (_bocDateTimeValue.GetTimeTextboxClientId(), actual[1]);
    }


    [Test]
    public void SetValueToDateTime()
    {
      DateTime dateTime = new DateTime (2006, 1, 1, 1, 1, 1);
      _bocDateTimeValue.IsDirty = false;
      _bocDateTimeValue.Value = dateTime;
      Assert.AreEqual (dateTime, _bocDateTimeValue.Value);
      Assert.IsTrue (_bocDateTimeValue.IsDirty);
    }

    [Test]
    public void SetValueToNull()
    {
      _bocDateTimeValue.IsDirty = false;
      _bocDateTimeValue.Value = null;
      Assert.AreEqual (null, _bocDateTimeValue.Value);
      Assert.IsTrue (_bocDateTimeValue.IsDirty);
    }

    [Test]
    public void SetValueToNullableDateTime()
    {
      DateTime? dateTime = new DateTime (2006, 1, 1, 1, 1, 1);
      _bocDateTimeValue.IsDirty = false;
      _bocDateTimeValue.Value = dateTime;
      Assert.AreEqual (dateTime, _bocDateTimeValue.Value);
      Assert.IsTrue (_bocDateTimeValue.IsDirty);
    }

    [Test]
    public void SetValueToNullableDateTimeNull()
    {
      _bocDateTimeValue.IsDirty = false;
      _bocDateTimeValue.Value = null;
      Assert.AreEqual (null, _bocDateTimeValue.Value);
      Assert.IsTrue (_bocDateTimeValue.IsDirty);
    }


    [Test]
    public void IBusinessObjectBoundControl_SetValueToDateTime ()
    {
      DateTime dateTime = new DateTime (2006, 1, 1, 1, 1, 1);
      _bocDateTimeValue.IsDirty = false;
      ((IBusinessObjectBoundControl) _bocDateTimeValue).Value = dateTime;
      Assert.AreEqual (dateTime, ((IBusinessObjectBoundControl) _bocDateTimeValue).Value);
      Assert.IsTrue (_bocDateTimeValue.IsDirty);
    }

    [Test]
    public void IBusinessObjectBoundControl_SetValueToNull ()
    {
      _bocDateTimeValue.IsDirty = false;
      ((IBusinessObjectBoundControl) _bocDateTimeValue).Value = null;
      Assert.AreEqual (null, ((IBusinessObjectBoundControl) _bocDateTimeValue).Value);
      Assert.IsTrue (_bocDateTimeValue.IsDirty);
    }

    [Test]
    public void IBusinessObjectBoundControl_SetValueToNullableDateTime ()
    {
      DateTime? dateTime = new DateTime (2006, 1, 1, 1, 1, 1);
      _bocDateTimeValue.IsDirty = false;
      ((IBusinessObjectBoundControl) _bocDateTimeValue).Value = dateTime;
      Assert.AreEqual (dateTime, ((IBusinessObjectBoundControl) _bocDateTimeValue).Value);
      Assert.IsTrue (_bocDateTimeValue.IsDirty);
    }

    [Test]
    public void IBusinessObjectBoundControl_SetValueToNullableDateTimeNull ()
    {
      _bocDateTimeValue.IsDirty = false;
      ((IBusinessObjectBoundControl) _bocDateTimeValue).Value = null;
      Assert.AreEqual (null, ((IBusinessObjectBoundControl) _bocDateTimeValue).Value);
      Assert.IsTrue (_bocDateTimeValue.IsDirty);
    }


    [Test]
    public void LoadValueAndInterimTrue()
    {
      _businessObject.DateTimeValue = new DateTime (2006, 1, 1, 1, 1, 1);
      _bocDateTimeValue.DataSource = _dataSource;
      _bocDateTimeValue.Property = _propertyDateTimeValue;
      _bocDateTimeValue.Value = null;
      _bocDateTimeValue.IsDirty = true;

      _bocDateTimeValue.LoadValue (true);
      Assert.AreEqual (null, _bocDateTimeValue.Value);
      Assert.IsTrue (_bocDateTimeValue.IsDirty);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDateTime()
    {
      _businessObject.DateTimeValue = new DateTime (2006, 1, 1, 1, 1, 1);
      _bocDateTimeValue.DataSource = _dataSource;
      _bocDateTimeValue.Property = _propertyDateTimeValue;
      _bocDateTimeValue.Value = null;
      _bocDateTimeValue.IsDirty = true;

      _bocDateTimeValue.LoadValue (false);
      Assert.AreEqual (_businessObject.DateTimeValue, _bocDateTimeValue.Value);
      Assert.IsFalse (_bocDateTimeValue.IsDirty);
    }

    [Test]
    public void LoadValueAndInterimFalseWithValueNullableDateTime()
    {
      _businessObject.NullableDateTimeValue = new DateTime (2006, 1, 1, 1, 1, 1);
      _bocDateTimeValue.DataSource = _dataSource;
      _bocDateTimeValue.Property = _propertyNullableDateTimeValue;
      _bocDateTimeValue.Value = null;
      _bocDateTimeValue.IsDirty = true;

      _bocDateTimeValue.LoadValue (false);
      Assert.AreEqual (_businessObject.NullableDateTimeValue, _bocDateTimeValue.Value);
      Assert.IsFalse (_bocDateTimeValue.IsDirty);
    }

    [Test]
    public void LoadValueAndInterimFalseWithValueNullableDateTimeNull()
    {
      _businessObject.NullableDateTimeValue = null;
      _bocDateTimeValue.DataSource = _dataSource;
      _bocDateTimeValue.Property = _propertyNullableDateTimeValue;
      _bocDateTimeValue.Value = DateTime.Now;
      _bocDateTimeValue.IsDirty = true;

      _bocDateTimeValue.LoadValue (false);
      Assert.AreEqual (_businessObject.NullableDateTimeValue, _bocDateTimeValue.Value);
      Assert.IsFalse (_bocDateTimeValue.IsDirty);
    }


    [Test]
    public void LoadUnboundValueAndInterimTrue()
    {
      DateTime value = new DateTime (2006, 1, 1, 1, 1, 1);
      _bocDateTimeValue.Value = null;
      _bocDateTimeValue.IsDirty = true;

      _bocDateTimeValue.LoadUnboundValue (value, true);
      Assert.AreEqual (null, _bocDateTimeValue.Value);
      Assert.IsTrue (_bocDateTimeValue.IsDirty);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithDateTime()
    {
      DateTime value = new DateTime (2006, 1, 1, 1, 1, 1);
      _bocDateTimeValue.Value = null;
      _bocDateTimeValue.IsDirty = true;

      _bocDateTimeValue.LoadUnboundValue (value, false);
      Assert.AreEqual (value, _bocDateTimeValue.Value);
      Assert.IsFalse (_bocDateTimeValue.IsDirty);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueNull()
    {
      DateTime? value = null;
      _bocDateTimeValue.Value = DateTime.Now;
      _bocDateTimeValue.IsDirty = true;

      _bocDateTimeValue.LoadUnboundValue (value, false);
      Assert.AreEqual (value, _bocDateTimeValue.Value);
      Assert.IsFalse (_bocDateTimeValue.IsDirty);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueNullableDateTime()
    {
      DateTime? value = new DateTime (2006, 1, 1, 1, 1, 1);
      _bocDateTimeValue.Value = null;
      _bocDateTimeValue.IsDirty = true;

      _bocDateTimeValue.LoadUnboundValue (value, false);
      Assert.AreEqual (value, _bocDateTimeValue.Value);
      Assert.IsFalse (_bocDateTimeValue.IsDirty);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueNullableDateTimeNull()
    {
      DateTime? value = null;
      _bocDateTimeValue.Value = DateTime.Now;
      _bocDateTimeValue.IsDirty = true;

      _bocDateTimeValue.LoadUnboundValue (value, false);
      Assert.AreEqual (value, _bocDateTimeValue.Value);
      Assert.IsFalse (_bocDateTimeValue.IsDirty);
    }
  }
}
