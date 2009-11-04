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

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{
  [TestFixture]
  public class BocBooleanValueTest : BocTest
  {
    private BocBooleanValueMock _bocBooleanValue;
    private TypeWithBoolean _businessObject;
    private BusinessObjectReferenceDataSource _dataSource;
    private IBusinessObjectBooleanProperty _propertyBooleanValue;
    private IBusinessObjectBooleanProperty _propertyNullableBooleanValue;

    public BocBooleanValueTest ()
    {
    }


    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _bocBooleanValue = new BocBooleanValueMock();
      _bocBooleanValue.ID = "BocBooleanValue";
      NamingContainer.Controls.Add (_bocBooleanValue);

      _businessObject = TypeWithBoolean.Create();

      _propertyBooleanValue =
          (IBusinessObjectBooleanProperty) ((IBusinessObject) _businessObject).BusinessObjectClass.GetPropertyDefinition ("BooleanValue");
      _propertyNullableBooleanValue =
          (IBusinessObjectBooleanProperty) ((IBusinessObject) _businessObject).BusinessObjectClass.GetPropertyDefinition ("NullableBooleanValue");

      _dataSource = new BusinessObjectReferenceDataSource();
      _dataSource.BusinessObject = (IBusinessObject) _businessObject;
    }


    [Test]
    public void EvaluateWaiConformityDebugLevelUndefined ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
      _bocBooleanValue.EvaluateWaiConformity();

      Assert.IsFalse (WcagHelperMock.HasWarning);
      Assert.IsFalse (WcagHelperMock.HasError);
    }

    [Test]
    public void EvaluateWaiConformityLevelA ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
      _bocBooleanValue.EvaluateWaiConformity();

      Assert.IsFalse (WcagHelperMock.HasWarning);
      Assert.IsFalse (WcagHelperMock.HasError);
    }


    [Test]
    public void EvaluateWaiConformityDebugLevelA ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
      _bocBooleanValue.EvaluateWaiConformity();

      Assert.IsTrue (WcagHelperMock.HasError);
      Assert.AreEqual (1, WcagHelperMock.Priority);
      Assert.AreSame (_bocBooleanValue, WcagHelperMock.Control);
      Assert.IsNull (WcagHelperMock.Property);
    }


    [Test]
    public void GetTrackedClientIDsInReadOnlyMode ()
    {
      _bocBooleanValue.ReadOnly = true;
      string[] actual = _bocBooleanValue.GetTrackedClientIDs();
      Assert.IsNotNull (actual);
      Assert.AreEqual (0, actual.Length);
    }

    [Test]
    public void GetTrackedClientIDsInEditMode ()
    {
      _bocBooleanValue.ReadOnly = false;
      string[] actual = _bocBooleanValue.GetTrackedClientIDs();
      Assert.IsNotNull (actual);
      Assert.AreEqual (1, actual.Length);
      Assert.AreEqual (_bocBooleanValue.GetHiddenFieldClientID(), actual[0]);
    }


    [Test]
    public void SetValueToTrue ()
    {
      _bocBooleanValue.IsDirty = false;
      _bocBooleanValue.Value = true;
      Assert.AreEqual (true, _bocBooleanValue.Value);
      Assert.IsTrue (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void SetValueToFalse ()
    {
      _bocBooleanValue.IsDirty = false;
      _bocBooleanValue.Value = false;
      Assert.AreEqual (false, _bocBooleanValue.Value);
      Assert.IsTrue (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void SetValueToNull ()
    {
      _bocBooleanValue.IsDirty = false;
      _bocBooleanValue.Value = null;
      Assert.AreEqual (null, _bocBooleanValue.Value);
      Assert.IsTrue (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void SetValueToNullableBooleanTrue ()
    {
      _bocBooleanValue.IsDirty = false;
      _bocBooleanValue.Value = true;
      Assert.AreEqual (true, _bocBooleanValue.Value);
      Assert.IsTrue (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void SetValueToNullableBooleanFalse ()
    {
      _bocBooleanValue.IsDirty = false;
      _bocBooleanValue.Value = false;
      Assert.AreEqual (false, _bocBooleanValue.Value);
      Assert.IsTrue (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void SetValueToNullableBooleanNull ()
    {
      _bocBooleanValue.IsDirty = false;
      _bocBooleanValue.Value = null;
      Assert.AreEqual (null, _bocBooleanValue.Value);
      Assert.IsTrue (_bocBooleanValue.IsDirty);
    }


    [Test]
    public void IBusinessObjectBoundControl_SetValueToTrue ()
    {
      _bocBooleanValue.IsDirty = false;
      ((IBusinessObjectBoundControl) _bocBooleanValue).Value = true;
      Assert.AreEqual (true, ((IBusinessObjectBoundControl) _bocBooleanValue).Value);
      Assert.IsTrue (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void IBusinessObjectBoundControl_SetValueToFalse ()
    {
      _bocBooleanValue.IsDirty = false;
      ((IBusinessObjectBoundControl) _bocBooleanValue).Value = false;
      Assert.AreEqual (false, ((IBusinessObjectBoundControl) _bocBooleanValue).Value);
      Assert.IsTrue (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void IBusinessObjectBoundControl_SetValueToNull ()
    {
      _bocBooleanValue.IsDirty = false;
      ((IBusinessObjectBoundControl) _bocBooleanValue).Value = null;
      Assert.AreEqual (null, ((IBusinessObjectBoundControl) _bocBooleanValue).Value);
      Assert.IsTrue (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void IBusinessObjectBoundControl_SetValueToNullableBooleanTrue ()
    {
      _bocBooleanValue.IsDirty = false;
      ((IBusinessObjectBoundControl) _bocBooleanValue).Value = true;
      Assert.AreEqual (true, ((IBusinessObjectBoundControl) _bocBooleanValue).Value);
      Assert.IsTrue (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void IBusinessObjectBoundControl_SetValueToNullableBooleanFalse ()
    {
      _bocBooleanValue.IsDirty = false;
      ((IBusinessObjectBoundControl) _bocBooleanValue).Value = false;
      Assert.AreEqual (false, ((IBusinessObjectBoundControl) _bocBooleanValue).Value);
      Assert.IsTrue (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void IBusinessObjectBoundControl_SetValueToNullableBooleanNull ()
    {
      _bocBooleanValue.IsDirty = false;
      ((IBusinessObjectBoundControl) _bocBooleanValue).Value = null;
      Assert.AreEqual (null, ((IBusinessObjectBoundControl) _bocBooleanValue).Value);
      Assert.IsTrue (_bocBooleanValue.IsDirty);
    }


    [Test]
    public void LoadValueAndInterimTrue ()
    {
      _businessObject.BooleanValue = true;
      _bocBooleanValue.DataSource = _dataSource;
      _bocBooleanValue.Property = _propertyBooleanValue;
      _bocBooleanValue.Value = null;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadValue (true);
      Assert.AreEqual (null, _bocBooleanValue.Value);
      Assert.IsTrue (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void LoadValueAndInterimFalseWithValueTrue ()
    {
      _businessObject.BooleanValue = true;
      _bocBooleanValue.DataSource = _dataSource;
      _bocBooleanValue.Property = _propertyBooleanValue;
      _bocBooleanValue.Value = null;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadValue (false);
      Assert.AreEqual (_businessObject.BooleanValue, _bocBooleanValue.Value);
      Assert.IsFalse (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void LoadValueAndInterimFalseWithValueFalse ()
    {
      _businessObject.BooleanValue = false;
      _bocBooleanValue.DataSource = _dataSource;
      _bocBooleanValue.Property = _propertyBooleanValue;
      _bocBooleanValue.Value = null;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadValue (false);
      Assert.AreEqual (_businessObject.BooleanValue, _bocBooleanValue.Value);
      Assert.IsFalse (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void LoadValueAndInterimFalseWithValueNullableBooelanTrue ()
    {
      _businessObject.NullableBooleanValue = true;
      _bocBooleanValue.DataSource = _dataSource;
      _bocBooleanValue.Property = _propertyNullableBooleanValue;
      _bocBooleanValue.Value = null;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadValue (false);
      Assert.AreEqual (_businessObject.NullableBooleanValue, _bocBooleanValue.Value);
      Assert.IsFalse (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void LoadValueAndInterimFalseWithValueNullableBooelanFalse ()
    {
      _businessObject.NullableBooleanValue = false;
      _bocBooleanValue.DataSource = _dataSource;
      _bocBooleanValue.Property = _propertyNullableBooleanValue;
      _bocBooleanValue.Value = null;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadValue (false);
      Assert.AreEqual (_businessObject.NullableBooleanValue, _bocBooleanValue.Value);
      Assert.IsFalse (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void LoadValueAndInterimFalseWithValueNullableBooelanNull ()
    {
      _businessObject.NullableBooleanValue = null;
      _bocBooleanValue.DataSource = _dataSource;
      _bocBooleanValue.Property = _propertyNullableBooleanValue;
      _bocBooleanValue.Value = true;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadValue (false);
      Assert.AreEqual (_businessObject.NullableBooleanValue, _bocBooleanValue.Value);
      Assert.IsFalse (_bocBooleanValue.IsDirty);
    }


    [Test]
    public void LoadUnboundValueAndInterimTrue ()
    {
      bool value = true;
      _bocBooleanValue.Value = null;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadUnboundValue (value, true);
      Assert.AreEqual (null, _bocBooleanValue.Value);
      Assert.IsTrue (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueTrue ()
    {
      bool value = true;
      _bocBooleanValue.Value = null;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadUnboundValue (value, false);
      Assert.AreEqual (value, _bocBooleanValue.Value);
      Assert.IsFalse (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueFalse ()
    {
      bool value = false;
      _bocBooleanValue.Value = null;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadUnboundValue (value, false);
      Assert.AreEqual (value, _bocBooleanValue.Value);
      Assert.IsFalse (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueNull ()
    {
      bool? value = null;
      _bocBooleanValue.Value = true;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadUnboundValue (value, false);
      Assert.AreEqual (value, _bocBooleanValue.Value);
      Assert.IsFalse (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueNullableBooelanTrue ()
    {
      bool? value = true;
      _bocBooleanValue.Value = null;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadUnboundValue (value, false);
      Assert.AreEqual (value, _bocBooleanValue.Value);
      Assert.IsFalse (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueNullableBooelanFalse ()
    {
      bool? value = false;
      _bocBooleanValue.Value = null;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadUnboundValue (value, false);
      Assert.AreEqual (value, _bocBooleanValue.Value);
      Assert.IsFalse (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueNullableBooelanNull ()
    {
      bool? value = null;
      _bocBooleanValue.Value = true;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadUnboundValue (value, false);
      Assert.AreEqual (value, _bocBooleanValue.Value);
      Assert.IsFalse (_bocBooleanValue.IsDirty);
    }
  }
}
