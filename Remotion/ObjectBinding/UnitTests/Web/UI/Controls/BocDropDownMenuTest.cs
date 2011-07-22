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
  public class BocDropDownMenuTest : BocTest
  {
    private BocDropDownMenuMock _bocDropDownMenu;
    private TypeWithReference _businessObject;
    private BusinessObjectReferenceDataSource _dataSource;
    private IBusinessObjectReferenceProperty _propertyReferenceValue;

    public BocDropDownMenuTest ()
    {
    }


    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _bocDropDownMenu = new BocDropDownMenuMock();
      _bocDropDownMenu.ID = "BocDropDownMenu";
      NamingContainer.Controls.Add (_bocDropDownMenu);

      _businessObject = TypeWithReference.Create();

      _propertyReferenceValue =
          (IBusinessObjectReferenceProperty) ((IBusinessObject) _businessObject).BusinessObjectClass.GetPropertyDefinition ("ReferenceValue");

      _dataSource = new BusinessObjectReferenceDataSource();
      _dataSource.BusinessObject = (IBusinessObject) _businessObject;
    }


    [Test]
    public void EvaluateWaiConformityDebugLevelUndefined ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
      _bocDropDownMenu.EvaluateWaiConformity();

      Assert.IsFalse (WcagHelperMock.HasWarning);
      Assert.IsFalse (WcagHelperMock.HasError);
    }

    [Test]
    public void EvaluateWaiConformityLevelA ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
      _bocDropDownMenu.EvaluateWaiConformity();

      Assert.IsFalse (WcagHelperMock.HasWarning);
      Assert.IsFalse (WcagHelperMock.HasError);
    }


    [Test]
    public void EvaluateWaiConformityDebugLevelA ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
      _bocDropDownMenu.EvaluateWaiConformity();

      Assert.IsTrue (WcagHelperMock.HasError);
      Assert.AreEqual (1, WcagHelperMock.Priority);
      Assert.AreSame (_bocDropDownMenu, WcagHelperMock.Control);
      Assert.IsNull (WcagHelperMock.Property);
    }


    [Test]
    public void SetValueToObject ()
    {
      IBusinessObject referencedObject = (IBusinessObject) TypeWithReference.Create();
      _bocDropDownMenu.Value = referencedObject;
      Assert.AreEqual (referencedObject, _bocDropDownMenu.Value);
    }

    [Test]
    public void SetValueToNull ()
    {
      _bocDropDownMenu.Value = null;
      Assert.AreEqual (null, _bocDropDownMenu.Value);
    }


    [Test]
    public void HasValue_ValueIsSet_ReturnsTrue ()
    {
      _bocDropDownMenu.Value = (IBusinessObject) TypeWithReference.Create();
      Assert.IsTrue (_bocDropDownMenu.HasValue);
    }

    [Test]
    public void HasValue_ValueIsNull_ReturnsFalse ()
    {
      _bocDropDownMenu.Value = null;
      Assert.IsFalse (_bocDropDownMenu.HasValue);
    }


    [Test]
    public void LoadValueAndInterimTrueWithObject ()
    {
      _businessObject.ReferenceValue = TypeWithReference.Create();
      _bocDropDownMenu.DataSource = _dataSource;
      _bocDropDownMenu.Property = _propertyReferenceValue;
      _bocDropDownMenu.Value = null;

      _bocDropDownMenu.LoadValue (true);
      Assert.AreEqual (_businessObject.ReferenceValue, _bocDropDownMenu.Value);
    }

    [Test]
    public void LoadValueAndInterimTrueWithObjectAndNoProperty ()
    {
      _bocDropDownMenu.DataSource = _dataSource;
      _bocDropDownMenu.Value = null;

      _bocDropDownMenu.LoadValue (true);
      Assert.AreEqual (_businessObject, _bocDropDownMenu.Value);
    }

    [Test]
    public void LoadValueAndInterimTrueWithNull ()
    {
      _businessObject.ReferenceValue = null;
      _bocDropDownMenu.DataSource = _dataSource;
      _bocDropDownMenu.Property = _propertyReferenceValue;
      _bocDropDownMenu.Value = (IBusinessObject) TypeWithReference.Create();

      _bocDropDownMenu.LoadValue (true);
      Assert.AreEqual (_businessObject.ReferenceValue, _bocDropDownMenu.Value);
    }

    [Test]
    public void LoadValueAndInterimFalseWithObject ()
    {
      _businessObject.ReferenceValue = TypeWithReference.Create();
      _bocDropDownMenu.DataSource = _dataSource;
      _bocDropDownMenu.Property = _propertyReferenceValue;
      _bocDropDownMenu.Value = null;

      _bocDropDownMenu.LoadValue (false);
      Assert.AreEqual (_businessObject.ReferenceValue, _bocDropDownMenu.Value);
    }


    [Test]
    public void LoadValueAndInterimFalseWithObjectAndNoProperty ()
    {
      _bocDropDownMenu.DataSource = _dataSource;
      _bocDropDownMenu.Value = null;

      _bocDropDownMenu.LoadValue (false);
      Assert.AreEqual (_businessObject, _bocDropDownMenu.Value);
    }

    [Test]
    public void LoadValueAndInterimFalseWithNull ()
    {
      _businessObject.ReferenceValue = null;
      _bocDropDownMenu.DataSource = _dataSource;
      _bocDropDownMenu.Property = _propertyReferenceValue;
      _bocDropDownMenu.Value = (IBusinessObject) TypeWithReference.Create();

      _bocDropDownMenu.LoadValue (false);
      Assert.AreEqual (_businessObject.ReferenceValue, _bocDropDownMenu.Value);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceNull ()
    {
      var value = (IBusinessObjectWithIdentity) TypeWithReference.Create ();
      _bocDropDownMenu.DataSource = null;
      _bocDropDownMenu.Property = _propertyReferenceValue;
      _bocDropDownMenu.Value = value;

      _bocDropDownMenu.LoadValue (false);
      Assert.AreEqual (value, _bocDropDownMenu.Value);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceBusinessObjectNull ()
    {
      _dataSource.BusinessObject = null;
      _bocDropDownMenu.DataSource = _dataSource;
      _bocDropDownMenu.Property = _propertyReferenceValue;
      _bocDropDownMenu.Value = (IBusinessObjectWithIdentity) TypeWithReference.Create ();

      _bocDropDownMenu.LoadValue (false);
      Assert.AreEqual (null, _bocDropDownMenu.Value);
    }


    [Test]
    public void LoadUnboundValueAndInterimTrueWithObject ()
    {
      IBusinessObject value = (IBusinessObject) TypeWithReference.Create();
      _bocDropDownMenu.Value = null;

      _bocDropDownMenu.LoadUnboundValue (value, true);
      Assert.AreEqual (value, _bocDropDownMenu.Value);
    }

    [Test]
    public void LoadUnboundValueAndInterimTrueWithNull ()
    {
      IBusinessObject value = null;
      _bocDropDownMenu.Value = (IBusinessObject) TypeWithReference.Create();

      _bocDropDownMenu.LoadUnboundValue (value, true);
      Assert.AreEqual (value, _bocDropDownMenu.Value);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithObject ()
    {
      IBusinessObject value = (IBusinessObject) TypeWithReference.Create();
      _bocDropDownMenu.Value = null;

      _bocDropDownMenu.LoadUnboundValue (value, false);
      Assert.AreEqual (value, _bocDropDownMenu.Value);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithNull ()
    {
      IBusinessObject value = null;
      _bocDropDownMenu.Value = (IBusinessObject) TypeWithReference.Create();

      _bocDropDownMenu.LoadUnboundValue (value, false);
      Assert.AreEqual (value, _bocDropDownMenu.Value);
    }
  }
}