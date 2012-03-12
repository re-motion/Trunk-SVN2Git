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
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Remotion.ObjectBinding.UnitTests.Web.Domain;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{
  [TestFixture]
  public class BocListTest : BocTest
  {
    private BocListMock _bocList;
    private TypeWithReference _businessObject;
    private BusinessObjectReferenceDataSource _dataSource;
    private IBusinessObjectReferenceProperty _propertyReferenceList;

    public BocListTest ()
    {
    }

    [TestFixtureSetUp]
    public void TestFixtureSetUp ()
    {
      ServiceLocator.SetLocatorProvider (() => new StubServiceLocator());
    }

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      Invoker.InitRecursive();

      _bocList = new BocListMock();
      _bocList.ID = "BocList";
      NamingContainer.Controls.Add (_bocList);

      _businessObject = TypeWithReference.Create();

      _propertyReferenceList =
          (IBusinessObjectReferenceProperty) ((IBusinessObject) _businessObject).BusinessObjectClass.GetPropertyDefinition ("ReferenceList");

      _dataSource = new BusinessObjectReferenceDataSource();
      _dataSource.BusinessObject = (IBusinessObject) _businessObject;
    }


    [Test]
    public void GetTrackedClientIDsInReadOnlyMode ()
    {
      _bocList.ReadOnly = true;
      string[] actual = _bocList.GetTrackedClientIDs();
      Assert.IsNotNull (actual);
      Assert.AreEqual (0, actual.Length);
    }

    [Test]
    public void GetTrackedClientIDsInEditModeWithoutRowEditModeActive ()
    {
      _bocList.ReadOnly = false;
      Assert.IsFalse (_bocList.IsRowEditModeActive);
      string[] actual = _bocList.GetTrackedClientIDs();
      Assert.IsNotNull (actual);
      Assert.AreEqual (0, actual.Length);
    }

    [Test]
    public void SetValueToList ()
    {
      IBusinessObject[] list = new[] { (IBusinessObject) TypeWithString.Create() };
      _bocList.IsDirty = false;
      _bocList.Value = list;
      Assert.AreEqual (list, _bocList.Value);
      Assert.IsTrue (_bocList.IsDirty);
    }

    [Test]
    public void SetValueToNull ()
    {
      _bocList.IsDirty = false;
      _bocList.Value = null;
      Assert.AreEqual (null, _bocList.Value);
      Assert.IsTrue (_bocList.IsDirty);
    }


    [Test]
    public void HasValue_ValueIsSet_ReturnsTrue ()
    {
      _bocList.Value = new IBusinessObjectWithIdentity[1];
      Assert.IsTrue (_bocList.HasValue);
    }

    [Test]
    public void HasValue_ValueIsEmpty_ReturnsFalse ()
    {
      _bocList.Value = new IBusinessObjectWithIdentity[0];
      Assert.IsFalse(_bocList.HasValue);
    }

    [Test]
    public void HasValue_ValueIsNull_ReturnsFalse ()
    {
      _bocList.Value = null;
      Assert.IsFalse (_bocList.HasValue);
    }


    [Test]
    public void LoadValueAndInterimTrueWithListAndDirty ()
    {
      _businessObject.ReferenceList = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      _bocList.Value = null;
      _bocList.IsDirty = true;

      _bocList.LoadValue (true);
      Assert.AreSame (_businessObject.ReferenceList, _bocList.Value);
      Assert.IsTrue (_bocList.IsDirty);
    }

    [Test]
    public void LoadValueAndInterimTrueWithNullAndDirty ()
    {
      _businessObject.ReferenceList = null;
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      _bocList.Value = new TypeWithReference[0];
      _bocList.IsDirty = true;

      _bocList.LoadValue (true);
      Assert.AreSame (_businessObject.ReferenceList, _bocList.Value);
      Assert.IsTrue (_bocList.IsDirty);
    }

    [Test]
    public void LoadValueAndInterimTrueWithListAndNotDirty ()
    {
      _businessObject.ReferenceList = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      _bocList.Value = null;
      _bocList.IsDirty = false;

      _bocList.LoadValue (true);
      Assert.AreSame (_businessObject.ReferenceList, _bocList.Value);
      Assert.IsFalse (_bocList.IsDirty);
    }

    [Test]
    public void LoadValueAndInterimTrueWithNullAndNotDirty ()
    {
      _businessObject.ReferenceList = null;
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      _bocList.Value = new TypeWithReference[0];
      _bocList.IsDirty = false;

      _bocList.LoadValue (true);
      Assert.AreSame (_businessObject.ReferenceList, _bocList.Value);
      Assert.IsFalse (_bocList.IsDirty);
    }

    [Test]
    public void LoadValueAndInterimFalseWithListAndDirty ()
    {
      _businessObject.ReferenceList = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      _bocList.Value = null;
      _bocList.IsDirty = true;

      _bocList.LoadValue (false);
      Assert.AreSame (_businessObject.ReferenceList, _bocList.Value);
      Assert.IsFalse (_bocList.IsDirty);
    }

    [Test]
    public void LoadValueAndInterimFalseWithNullAndDirty ()
    {
      _businessObject.ReferenceList = null;
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      _bocList.Value = new TypeWithReference[0];
      _bocList.IsDirty = true;

      _bocList.LoadValue (false);
      Assert.AreSame (_businessObject.ReferenceList, _bocList.Value);
      Assert.IsFalse (_bocList.IsDirty);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceNull ()
    {
      TypeWithReference[] value = new[] { TypeWithReference.Create (), TypeWithReference.Create () };
      _bocList.DataSource = null;
      _bocList.Property = _propertyReferenceList;
      _bocList.Value = value;
      _bocList.IsDirty = true;

      _bocList.LoadValue (false);
      Assert.AreEqual (value, _bocList.Value);
      Assert.IsTrue (_bocList.IsDirty);
    }

    [Test]
    public void LoadValueAndInterimFalseWithPropertyNull ()
    {
      TypeWithReference[] value = new[] { TypeWithReference.Create (), TypeWithReference.Create () };
      _bocList.DataSource = _dataSource;
      _bocList.Property = null;
      _bocList.Value = value;
      _bocList.IsDirty = true;

      _bocList.LoadValue (false);
      Assert.AreEqual (value, _bocList.Value);
      Assert.IsTrue (_bocList.IsDirty);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceBusinessObjectNull ()
    {
      _dataSource.BusinessObject = null;
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      _bocList.Value = new[] { TypeWithReference.Create (), TypeWithReference.Create () };
      _bocList.IsDirty = true;

      _bocList.LoadValue (false);
      Assert.AreEqual (null, _bocList.Value);
      Assert.IsFalse (_bocList.IsDirty);
    }

    [Test]
    public void LoadValueAndInterimTrueWithDataSourceBusinessObjectNull ()
    {
      _dataSource.BusinessObject = null;
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      _bocList.Value = new[] { TypeWithReference.Create (), TypeWithReference.Create () };
      _bocList.IsDirty = true;

      _bocList.LoadValue (true);
      Assert.AreEqual (null, _bocList.Value);
      Assert.IsTrue (_bocList.IsDirty);
    }


    [Test]
    public void LoadUnboundValueAndInterimTrueWithListAndDirty ()
    {
      TypeWithReference[] value = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocList.DataSource = _dataSource;
      _bocList.Value = null;
      _bocList.IsDirty = true;

      _bocList.LoadUnboundValue (value, true);
      Assert.AreSame (value, _bocList.Value);
      Assert.IsTrue (_bocList.IsDirty);
    }

    [Test]
    public void LoadUnboundValueAndInterimTrueWithNullAndDirty ()
    {
      TypeWithReference[] value = null;
      _bocList.DataSource = _dataSource;
      _bocList.Value = new TypeWithReference[0];
      _bocList.IsDirty = true;

      _bocList.LoadUnboundValue (value, true);
      Assert.AreSame (value, _bocList.Value);
      Assert.IsTrue (_bocList.IsDirty);
    }

    [Test]
    public void LoadUnboundValueAndInterimTrueWithListAndNotDirty ()
    {
      TypeWithReference[] value = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocList.DataSource = _dataSource;
      _bocList.Value = null;
      _bocList.IsDirty = false;

      _bocList.LoadUnboundValue (value, true);
      Assert.AreSame (value, _bocList.Value);
      Assert.IsFalse (_bocList.IsDirty);
    }

    [Test]
    public void LoadUnboundValueAndInterimTrueWithNullAndNotDirty ()
    {
      TypeWithReference[] value = null;
      _bocList.DataSource = _dataSource;
      _bocList.Value = new TypeWithReference[0];
      _bocList.IsDirty = false;

      _bocList.LoadUnboundValue (value, true);
      Assert.AreSame (value, _bocList.Value);
      Assert.IsFalse (_bocList.IsDirty);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithListAndDirty ()
    {
      TypeWithReference[] value = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocList.DataSource = _dataSource;
      _bocList.Value = null;
      _bocList.IsDirty = true;

      _bocList.LoadUnboundValue (value, false);
      Assert.AreSame (value, _bocList.Value);
      Assert.IsFalse (_bocList.IsDirty);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithNullAndDirty ()
    {
      TypeWithReference[] value = null;
      _bocList.DataSource = _dataSource;
      _bocList.Value = new TypeWithReference[0];
      _bocList.IsDirty = true;

      _bocList.LoadUnboundValue (value, false);
      Assert.AreSame (value, _bocList.Value);
      Assert.IsFalse (_bocList.IsDirty);
    }

    [Test]
    public void SaveValueAndInterimTrue ()
    {
      _businessObject.ReferenceList = new[] { TypeWithReference.Create() };
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      TypeWithReference[] newValue = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocList.Value = newValue;
      _bocList.IsDirty = true;

      _bocList.SaveValue (true);
      Assert.AreEqual (newValue, _businessObject.ReferenceList);
      Assert.IsTrue (_bocList.IsDirty);
    }

    [Test]
    public void SaveValueAndInterimFalse ()
    {
      _businessObject.ReferenceList = new[] { TypeWithReference.Create() };
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      TypeWithReference[] newValue = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocList.Value = newValue;
      _bocList.IsDirty = true;

      _bocList.SaveValue (false);
      Assert.AreEqual (newValue, _businessObject.ReferenceList);
      Assert.IsFalse (_bocList.IsDirty);
    }

    [Test]
    public void SaveValueAndIsDirtyFalse ()
    {
      var oldValue = new[] { TypeWithReference.Create() };
      _businessObject.ReferenceList = oldValue;
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      _bocList.Value = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocList.IsDirty = false;

      _bocList.SaveValue (false);
      Assert.AreEqual (oldValue, _businessObject.ReferenceList);
      Assert.IsFalse (_bocList.IsDirty);
    }

    [Test]
    public void SaveValueAndIsRowEditModeActiveTrueAndInterimTrue ()
    {
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      _bocList.Value = new[] { TypeWithReference.Create() };
      _bocList.SwitchRowIntoEditMode (0);
      Assert.IsTrue (_bocList.IsRowEditModeActive);

      _bocList.SaveValue (true);
      Assert.IsTrue (_bocList.IsRowEditModeActive);
    }

    [Test]
    public void SaveValueAndIsRowEditModeActiveTrueAndInterimFalse ()
    {
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      _bocList.Value = new[] { TypeWithReference.Create() };
      _bocList.SwitchRowIntoEditMode (0);
      Assert.IsTrue (_bocList.IsRowEditModeActive);

      _bocList.SaveValue (false);
      Assert.IsFalse (_bocList.IsRowEditModeActive);
    }

    [Test]
    public void SaveValueAndIsListEditModeActiveTrueAndInterimTrue ()
    {
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      _bocList.Value = new[] { TypeWithReference.Create() };
      _bocList.SwitchListIntoEditMode();
      Assert.IsTrue (_bocList.IsListEditModeActive);

      _bocList.SaveValue (true);
      Assert.IsTrue (_bocList.IsListEditModeActive);
    }

    [Test]
    public void SaveValueAndIsListEditModeActiveTrueAndInterimFalse ()
    {
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      _bocList.Value = new[] { TypeWithReference.Create() };
      _bocList.SwitchListIntoEditMode();
      Assert.IsTrue (_bocList.IsListEditModeActive);

      _bocList.SaveValue (false);
      Assert.IsFalse (_bocList.IsListEditModeActive);
    }
  }
}