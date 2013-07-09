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
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Configuration;
using Remotion.ObjectBinding.UnitTests.Web.Domain;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{
  [TestFixture]
  public class BocMultilineTextValueTest : BocTest
  {
    private BocMultilineTextValueMock _bocMultilineTextValue;
    private TypeWithString _businessObject;
    private BusinessObjectReferenceDataSource _dataSource;
    private IBusinessObjectStringProperty _propertyStringArray;

    public BocMultilineTextValueTest ()
    {
    }


    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _bocMultilineTextValue = new BocMultilineTextValueMock();
      _bocMultilineTextValue.ID = "BocMultilineTextValue";
      NamingContainer.Controls.Add (_bocMultilineTextValue);

      _businessObject = TypeWithString.Create();

      _propertyStringArray =
          (IBusinessObjectStringProperty) ((IBusinessObject) _businessObject).BusinessObjectClass.GetPropertyDefinition ("StringArray");

      _dataSource = new BusinessObjectReferenceDataSource();
      _dataSource.BusinessObject = (IBusinessObject) _businessObject;
    }


    [Test]
    public void EvaluateWaiConformityDebugLevelUndefined ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
      _bocMultilineTextValue.EvaluateWaiConformity();

      Assert.That (WcagHelperMock.HasWarning, Is.False);
      Assert.That (WcagHelperMock.HasError, Is.False);
    }

    [Test]
    public void EvaluateWaiConformityLevelA ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
      _bocMultilineTextValue.TextBoxStyle.AutoPostBack = true;
      _bocMultilineTextValue.EvaluateWaiConformity();

      Assert.That (WcagHelperMock.HasWarning, Is.False);
      Assert.That (WcagHelperMock.HasError, Is.False);
    }


    [Test]
    public void EvaluateWaiConformityDebugLevelAWithTextBoxStyleAutoPostBackTrue ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
      _bocMultilineTextValue.TextBoxStyle.AutoPostBack = true;
      _bocMultilineTextValue.EvaluateWaiConformity();

      Assert.That (WcagHelperMock.HasWarning, Is.True);
      Assert.That (WcagHelperMock.Priority, Is.EqualTo (1));
      Assert.That (WcagHelperMock.Control, Is.SameAs (_bocMultilineTextValue));
      Assert.That (WcagHelperMock.Property, Is.EqualTo ("TextBoxStyle.AutoPostBack"));
    }

    [Test]
    public void GetTrackedClientIDsInReadOnlyMode ()
    {
      _bocMultilineTextValue.ReadOnly = true;
      string[] actual = _bocMultilineTextValue.GetTrackedClientIDs();
      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.Length, Is.EqualTo (0));
    }

    [Test]
    public void GetTrackedClientIDsInEditMode ()
    {
      _bocMultilineTextValue.ReadOnly = false;
      string[] actual = _bocMultilineTextValue.GetTrackedClientIDs();
      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.Length, Is.EqualTo (1));
      Assert.That (actual[0], Is.EqualTo (((IBocTextValueBase)_bocMultilineTextValue).GetValueName()));
    }

    [Test]
    public void SetValueToString ()
    {
      string[] value = new[] { "Foo", "Bar" };
      _bocMultilineTextValue.IsDirty = false;
      _bocMultilineTextValue.Value = value;
      Assert.That (_bocMultilineTextValue.Value, Is.EqualTo (value));
      Assert.That (_bocMultilineTextValue.IsDirty, Is.True);
    }

    [Test]
    public void SetValueToNull ()
    {
      _bocMultilineTextValue.IsDirty = false;
      _bocMultilineTextValue.Value = null;
      Assert.That (_bocMultilineTextValue.Value, Is.EqualTo (null));
      Assert.That (_bocMultilineTextValue.IsDirty, Is.True);
    }


    [Test]
    public void HasValue_ValueIsSet_ReturnsTrue ()
    {
      _bocMultilineTextValue.Value = new[] { "x" };
      Assert.That (_bocMultilineTextValue.HasValue, Is.True);
    }

    [Test]
    public void HasValue_TextContainsOnlyWhitespace_ReturnsFalse ()
    {
      _bocMultilineTextValue.Text = " \r\n ";
      Assert.That (_bocMultilineTextValue.HasValue, Is.False);
      Assert.That (_bocMultilineTextValue.Value, Is.Null);
    }

    [Test]
    public void HasValue_ValueIsNull_ReturnsFalse ()
    {
      _bocMultilineTextValue.Value = null;
      Assert.That (_bocMultilineTextValue.HasValue, Is.False);
    }


    [Test]
    public void LoadValueAndInterimTrue ()
    {
      _businessObject.StringArray = new[] { "Foo", "Bar" };
      _bocMultilineTextValue.DataSource = _dataSource;
      _bocMultilineTextValue.Property = _propertyStringArray;
      _bocMultilineTextValue.Value = null;
      _bocMultilineTextValue.IsDirty = true;

      _bocMultilineTextValue.LoadValue (true);
      Assert.That (_bocMultilineTextValue.Value, Is.EqualTo (null));
      Assert.That (_bocMultilineTextValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithString ()
    {
      _businessObject.StringArray = new[] { "Foo", "Bar" };
      _bocMultilineTextValue.DataSource = _dataSource;
      _bocMultilineTextValue.Property = _propertyStringArray;
      _bocMultilineTextValue.Value = null;
      _bocMultilineTextValue.IsDirty = true;

      _bocMultilineTextValue.LoadValue (false);
      Assert.That (_bocMultilineTextValue.Value, Is.EqualTo (_businessObject.StringArray));
      Assert.That (_bocMultilineTextValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithNull ()
    {
      _businessObject.StringArray = null;
      _bocMultilineTextValue.DataSource = _dataSource;
      _bocMultilineTextValue.Property = _propertyStringArray;
      _bocMultilineTextValue.Value = new[] { "Foo", "Bar" };
      _bocMultilineTextValue.IsDirty = true;

      _bocMultilineTextValue.LoadValue (false);
      Assert.That (_bocMultilineTextValue.Value, Is.EqualTo (_businessObject.StringArray));
      Assert.That (_bocMultilineTextValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceNull ()
    {
      _bocMultilineTextValue.DataSource = null;
      _bocMultilineTextValue.Property = _propertyStringArray;
      _bocMultilineTextValue.Value = new[] { "Foo", "Bar" };
      _bocMultilineTextValue.IsDirty = true;

      _bocMultilineTextValue.LoadValue (false);
      Assert.That (_bocMultilineTextValue.Value, Is.EqualTo (new[] { "Foo", "Bar" }));
      Assert.That (_bocMultilineTextValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithPropertyNull ()
    {
      _bocMultilineTextValue.DataSource = _dataSource;
      _bocMultilineTextValue.Property = null;
      _bocMultilineTextValue.Value = new[] { "Foo", "Bar" };
      _bocMultilineTextValue.IsDirty = true;

      _bocMultilineTextValue.LoadValue (false);
      Assert.That (_bocMultilineTextValue.Value, Is.EqualTo (new[] { "Foo", "Bar" }));
      Assert.That (_bocMultilineTextValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceBusinessObjectNull ()
    {
      _dataSource.BusinessObject = null;
      _bocMultilineTextValue.DataSource = _dataSource;
      _bocMultilineTextValue.Property = _propertyStringArray;
      _bocMultilineTextValue.Value = new[] { "Foo", "Bar" };
      _bocMultilineTextValue.IsDirty = true;

      _bocMultilineTextValue.LoadValue (false);
      Assert.That (_bocMultilineTextValue.Value, Is.EqualTo (null));
      Assert.That (_bocMultilineTextValue.IsDirty, Is.False);
    }


    [Test]
    public void LoadUnboundValueAndInterimTrue ()
    {
      string[] value = new[] { "Foo", "Bar" };
      _bocMultilineTextValue.Value = null;
      _bocMultilineTextValue.IsDirty = true;

      _bocMultilineTextValue.LoadUnboundValue (value, true);
      Assert.That (_bocMultilineTextValue.Value, Is.EqualTo (null));
      Assert.That (_bocMultilineTextValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithString ()
    {
      string[] value = new[] { "Foo", "Bar" };
      _bocMultilineTextValue.Value = null;
      _bocMultilineTextValue.IsDirty = true;

      _bocMultilineTextValue.LoadUnboundValue (value, false);
      Assert.That (_bocMultilineTextValue.Value, Is.EqualTo (value));
      Assert.That (_bocMultilineTextValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithNull ()
    {
      string[] value = null;
      _bocMultilineTextValue.Value = new[] { "Foo", "Bar" };
      _bocMultilineTextValue.IsDirty = true;

      _bocMultilineTextValue.LoadUnboundValue (value, false);
      Assert.That (_bocMultilineTextValue.Value, Is.EqualTo (value));
      Assert.That (_bocMultilineTextValue.IsDirty, Is.False);
    }

    [Test]
    public void SaveValueAndInterimTrue ()
    {
      _businessObject.StringArray = new[] { "Foo", "Bar" };
      _bocMultilineTextValue.DataSource = _dataSource;
      _bocMultilineTextValue.Property = _propertyStringArray;
      _bocMultilineTextValue.Value = null;
      _bocMultilineTextValue.IsDirty = true;

      _bocMultilineTextValue.SaveValue (true);
      Assert.That (_businessObject.StringArray, Is.EqualTo (new[] { "Foo", "Bar" }));
      Assert.That (_bocMultilineTextValue.IsDirty, Is.True);
    }

    [Test]
    public void SaveValueAndInterimFalse ()
    {
      _businessObject.StringArray = new[] { "Foo", "Bar" };
      _bocMultilineTextValue.DataSource = _dataSource;
      _bocMultilineTextValue.Property = _propertyStringArray;
      _bocMultilineTextValue.Value = null;
      _bocMultilineTextValue.IsDirty = true;

      _bocMultilineTextValue.SaveValue (false);
      Assert.That (_businessObject.StringArray, Is.EqualTo (null));
      Assert.That (_bocMultilineTextValue.IsDirty, Is.False);
    }

    [Test]
    public void SaveValueAndIsDirtyFalse ()
    {
      _businessObject.StringArray = new[] { "Foo", "Bar" };
      _bocMultilineTextValue.DataSource = _dataSource;
      _bocMultilineTextValue.Property = _propertyStringArray;
      _bocMultilineTextValue.Value = null;
      _bocMultilineTextValue.IsDirty = false;

      _bocMultilineTextValue.SaveValue (false);
      Assert.That (_businessObject.StringArray, Is.EqualTo (new[] { "Foo", "Bar" }));
      Assert.That (_bocMultilineTextValue.IsDirty, Is.False);
    }
  }
}