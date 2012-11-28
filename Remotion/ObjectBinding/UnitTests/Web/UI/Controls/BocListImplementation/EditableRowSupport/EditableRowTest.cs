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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.BindableObject;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.ObjectBinding.UnitTests.Web.Domain;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocListImplementation.EditableRowSupport
{
  [TestFixture]
  public class EditableRowTest : BocTest
  {
    private FakeEditModeHost _editModeHost;

    private EditableRow _editableRow;

    private IBusinessObject _value01;

    private BindableObjectClass _typeWithAllDataTypesClass;

    private IBusinessObjectPropertyPath _typeWithAllDataTypesStringValuePath;
    private IBusinessObjectPropertyPath _typeWithAllDataTypesInt32ValuePath;

    private BocSimpleColumnDefinition _typeWithAllDataTypesStringValueSimpleColumn;
    private BocSimpleColumnDefinition _typeWithAllDataTypesStringValueSimpleColumnAsDynamic;
    private BocSimpleColumnDefinition _typeWithAllDataTypesInt32ValueSimpleColumn;

    private BocCompoundColumnDefinition _typeWithAllDataTypesStringValueFirstValueCompoundColumn;
    private BocCustomColumnDefinition _typeWithAllDataTypesStringValueCustomColumn;
    private BocCommandColumnDefinition _commandColumn;
    private BocRowEditModeColumnDefinition _rowEditModeColumn;
    private BocDropDownMenuColumnDefinition _dropDownMenuColumn;

    [TestFixtureSetUp]
    public void TestFixtureSetUp ()
    {
      ServiceLocator.SetLocatorProvider (() => new StubServiceLocator ());
    }

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _editModeHost = new FakeEditModeHost();
      _editModeHost.ID = "BocList";
      _editModeHost.RowIDProvider = new FakeRowIDProvider();
      _editModeHost.EditModeControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();
      _editModeHost.EditModeDataSourceFactory = new EditableRowDataSourceFactory();

      _editableRow = new EditableRow (_editModeHost);
      _editableRow.ID = "Row";
      NamingContainer.Controls.Add (_editableRow);

      _value01 = (IBusinessObject) TypeWithAllDataTypes.Create ("A", 1);

      _typeWithAllDataTypesClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof (TypeWithAllDataTypes));

      _typeWithAllDataTypesStringValuePath = BusinessObjectPropertyPath.ParseStatic (_typeWithAllDataTypesClass, "String");
      _typeWithAllDataTypesInt32ValuePath = BusinessObjectPropertyPath.ParseStatic (_typeWithAllDataTypesClass, "Int32");

      _typeWithAllDataTypesStringValueSimpleColumn = new BocSimpleColumnDefinition();
      _typeWithAllDataTypesStringValueSimpleColumn.SetPropertyPath (_typeWithAllDataTypesStringValuePath);

      _typeWithAllDataTypesStringValueSimpleColumnAsDynamic = new BocSimpleColumnDefinition ();
      _typeWithAllDataTypesStringValueSimpleColumnAsDynamic.PropertyPathIdentifier = "StringValue";
      _typeWithAllDataTypesStringValueSimpleColumnAsDynamic.IsDynamic = true;

      _typeWithAllDataTypesInt32ValueSimpleColumn = new BocSimpleColumnDefinition();
      _typeWithAllDataTypesInt32ValueSimpleColumn.SetPropertyPath (_typeWithAllDataTypesInt32ValuePath);

      _typeWithAllDataTypesStringValueFirstValueCompoundColumn = new BocCompoundColumnDefinition();
      _typeWithAllDataTypesStringValueFirstValueCompoundColumn.PropertyPathBindings.Add (
          new PropertyPathBinding (_typeWithAllDataTypesStringValuePath));
      _typeWithAllDataTypesStringValueFirstValueCompoundColumn.PropertyPathBindings.Add (
          new PropertyPathBinding (_typeWithAllDataTypesStringValuePath));
      _typeWithAllDataTypesStringValueFirstValueCompoundColumn.FormatString = "{0}, {1}";

      _typeWithAllDataTypesStringValueCustomColumn = new BocCustomColumnDefinition();
      _typeWithAllDataTypesStringValueCustomColumn.SetPropertyPath (_typeWithAllDataTypesStringValuePath);
      _typeWithAllDataTypesStringValueCustomColumn.IsSortable = true;

      _commandColumn = new BocCommandColumnDefinition();
      _rowEditModeColumn = new BocRowEditModeColumnDefinition();
      _dropDownMenuColumn = new BocDropDownMenuColumnDefinition();
    }


    [Test]
    public void Initialize ()
    {
      Assert.IsNull (_editableRow.DataSourceFactory);
      Assert.IsNull (_editableRow.ControlFactory);
    }


    [Test]
    public void CreateControlsWithEmptyColumns ()
    {
      Invoker.InitRecursive();

      Assert.IsFalse (_editableRow.HasEditControls());
      Assert.IsFalse (_editableRow.HasValidators());

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      _editableRow.CreateControls (_value01, new BocColumnDefinition[0]);

      Assert.IsTrue (_editableRow.HasEditControls());
      Assert.IsTrue (_editableRow.HasValidators());

      Assert.IsNotNull (_editableRow.DataSourceFactory);
      Assert.IsNotNull (_editableRow.ControlFactory);

      IBusinessObjectReferenceDataSource dataSource = _editableRow.GetDataSource();
      Assert.IsNotNull (dataSource);
      Assert.AreSame (_value01, dataSource.BusinessObject);
    }

    [Test]
    public void CreateControlsWithColumns ()
    {
      Invoker.InitRecursive();

      Assert.IsFalse (_editableRow.HasEditControls());
      Assert.IsFalse (_editableRow.HasValidators());
      Assert.IsFalse (_editableRow.HasEditControl (0));

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[8];
      columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
      columns[1] = _typeWithAllDataTypesStringValueFirstValueCompoundColumn;
      columns[2] = _typeWithAllDataTypesStringValueCustomColumn;
      columns[3] = _commandColumn;
      columns[4] = _rowEditModeColumn;
      columns[5] = _dropDownMenuColumn;
      columns[6] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[7] = _typeWithAllDataTypesStringValueSimpleColumnAsDynamic;

      _editableRow.CreateControls (_value01, columns);

      Assert.IsTrue (_editableRow.HasEditControls());
      Assert.IsTrue (_editableRow.HasValidators());

      Assert.IsNotNull (_editableRow.DataSourceFactory);
      Assert.IsNotNull (_editableRow.ControlFactory);

      IBusinessObjectReferenceDataSource dataSource = _editableRow.GetDataSource();
      Assert.IsNotNull (dataSource);
      Assert.AreSame (_value01, dataSource.BusinessObject);

      Assert.IsTrue (_editableRow.HasEditControl (0));
      Assert.IsFalse (_editableRow.HasEditControl (1));
      Assert.IsFalse (_editableRow.HasEditControl (2));
      Assert.IsFalse (_editableRow.HasEditControl (3));
      Assert.IsFalse (_editableRow.HasEditControl (4));
      Assert.IsFalse (_editableRow.HasEditControl (5));
      Assert.IsTrue (_editableRow.HasEditControl (6));
      Assert.IsFalse (_editableRow.HasEditControl (7));

      IBusinessObjectBoundEditableWebControl textBoxFirstValue = _editableRow.GetEditControl (0);
      Assert.IsTrue (textBoxFirstValue is BocTextValue);
      Assert.AreSame (dataSource, textBoxFirstValue.DataSource);
      Assert.AreSame (_typeWithAllDataTypesStringValuePath.Properties.Last(), textBoxFirstValue.Property);

      IBusinessObjectBoundEditableWebControl textBoxSecondValue = _editableRow.GetEditControl (6);
      Assert.IsTrue (textBoxSecondValue is BocTextValue);
      Assert.AreSame (dataSource, textBoxSecondValue.DataSource);
      Assert.AreSame (_typeWithAllDataTypesInt32ValuePath.Properties.Last(), textBoxSecondValue.Property);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "BocList 'BocList': DataSourceFactory has not been set prior to invoking CreateControls().")]
    public void CreateControlsDataSourceFactoryNull ()
    {
      Invoker.InitRecursive();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();
      _editableRow.CreateControls (_value01, new BocColumnDefinition[0]);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "BocList 'BocList': ControlFactory has not been set prior to invoking CreateControls().")]
    public void CreateControlsControlFactoryNull ()
    {
      Invoker.InitRecursive();
      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.CreateControls (_value01, new BocColumnDefinition[0]);
    }

    [Test]
    public void EnsureValidators ()
    {
      Invoker.InitRecursive();

      Assert.IsFalse (_editableRow.HasValidators());

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[7];
      columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
      columns[1] = _typeWithAllDataTypesStringValueFirstValueCompoundColumn;
      columns[2] = _typeWithAllDataTypesStringValueCustomColumn;
      columns[3] = _commandColumn;
      columns[4] = _rowEditModeColumn;
      columns[5] = _dropDownMenuColumn;
      columns[6] = _typeWithAllDataTypesInt32ValueSimpleColumn;

      _editableRow.CreateControls (_value01, columns);
      _editableRow.EnsureValidatorsRestored();

      Assert.IsTrue (_editableRow.HasValidators());

      Assert.IsTrue (_editableRow.HasValidators (0));
      Assert.IsFalse (_editableRow.HasValidators (1));
      Assert.IsFalse (_editableRow.HasValidators (2));
      Assert.IsFalse (_editableRow.HasValidators (3));
      Assert.IsFalse (_editableRow.HasValidators (4));
      Assert.IsFalse (_editableRow.HasValidators (5));
      Assert.IsTrue (_editableRow.HasValidators (6));

      ControlCollection validators0 = _editableRow.GetValidators (0);
      Assert.IsNotNull (validators0);
      Assert.AreEqual (0, validators0.Count);

      ControlCollection validators6 = _editableRow.GetValidators (6);
      Assert.IsNotNull (validators6);
      Assert.AreEqual (2, validators6.Count);
      Assert.IsTrue (validators6[0] is RequiredFieldValidator);
      Assert.IsTrue (validators6[1] is NumericValidator);
    }

    [Test]
    public void EnsureValidatorsWithoutCreateControls ()
    {
      Invoker.InitRecursive();

      Assert.IsFalse (_editableRow.HasValidators());

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      _editableRow.EnsureValidatorsRestored();

      Assert.IsFalse (_editableRow.HasValidators());

      BocColumnDefinition[] columns = new BocColumnDefinition[7];
      columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
      columns[1] = _typeWithAllDataTypesStringValueFirstValueCompoundColumn;
      columns[2] = _typeWithAllDataTypesStringValueCustomColumn;
      columns[3] = _commandColumn;
      columns[4] = _rowEditModeColumn;
      columns[5] = _dropDownMenuColumn;
      columns[6] = _typeWithAllDataTypesInt32ValueSimpleColumn;

      _editableRow.CreateControls (_value01, columns);
      _editableRow.EnsureValidatorsRestored();

      Assert.IsTrue (_editableRow.HasValidators());

      Assert.IsTrue (_editableRow.HasValidators (0));
      Assert.IsFalse (_editableRow.HasValidators (1));
      Assert.IsFalse (_editableRow.HasValidators (2));
      Assert.IsFalse (_editableRow.HasValidators (3));
      Assert.IsFalse (_editableRow.HasValidators (4));
      Assert.IsFalse (_editableRow.HasValidators (5));
      Assert.IsTrue (_editableRow.HasValidators (6));

      ControlCollection validators0 = _editableRow.GetValidators (0);
      Assert.IsNotNull (validators0);
      Assert.AreEqual (0, validators0.Count);

      ControlCollection validators6 = _editableRow.GetValidators (6);
      Assert.IsNotNull (validators6);
      Assert.AreEqual (2, validators6.Count);
      Assert.IsTrue (validators6[0] is RequiredFieldValidator);
      Assert.IsTrue (validators6[1] is NumericValidator);
    }

    [Test]
    public void ControlInit ()
    {
      Assert.IsFalse (_editableRow.HasControls());
      Assert.IsFalse (_editableRow.HasEditControls());
      Assert.IsFalse (_editableRow.HasValidators());

      Invoker.InitRecursive();

      Assert.IsFalse (_editableRow.HasControls());
      Assert.IsFalse (_editableRow.HasEditControls());
      Assert.IsFalse (_editableRow.HasValidators());
    }

    [Test]
    public void ControlLoad ()
    {
      Assert.IsFalse (_editableRow.HasControls());
      Assert.IsFalse (_editableRow.HasEditControls());
      Assert.IsFalse (_editableRow.HasValidators());

      Invoker.LoadRecursive();

      Assert.IsFalse (_editableRow.HasControls());
      Assert.IsFalse (_editableRow.HasEditControls());
      Assert.IsFalse (_editableRow.HasValidators());
    }

    [Test]
    public void LoadValue ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
      columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;

      _editableRow.CreateControls (_value01, columns);

      IBusinessObjectReferenceDataSource dataSource = _editableRow.GetDataSource();
      dataSource.LoadValues (false);

      BocTextValue textBoxStringValue = (BocTextValue) _editableRow.GetEditControl (0);
      BocTextValue textBoxInt32Value = (BocTextValue) _editableRow.GetEditControl (1);

      Assert.AreEqual ("A", textBoxStringValue.Value);
      Assert.AreEqual (1, textBoxInt32Value.Value);
    }

    [Test]
    public void SaveValue ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
      columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;

      _editableRow.CreateControls (_value01, columns);

      IBusinessObjectReferenceDataSource dataSource = _editableRow.GetDataSource();
      dataSource.LoadValues (false);

      BocTextValue textBoxStringValue = (BocTextValue) _editableRow.GetEditControl (0);
      BocTextValue textBoxInt32Value = (BocTextValue) _editableRow.GetEditControl (1);

      Assert.AreEqual ("A", textBoxStringValue.Value);
      Assert.AreEqual (1, textBoxInt32Value.Value);

      textBoxStringValue.Value = "New Value A";
      textBoxInt32Value.Value = "100";

      dataSource.SaveValues (false);

      Assert.AreEqual ("New Value A", ((TypeWithAllDataTypes) _value01).String);
      Assert.AreEqual (100, ((TypeWithAllDataTypes) _value01).Int32);
    }


    [Test]
    public void HasEditControl ()
    {
      Invoker.InitRecursive();

      Assert.IsFalse (_editableRow.HasEditControls());
      Assert.IsFalse (_editableRow.HasEditControl (0));
      Assert.IsFalse (_editableRow.HasEditControl (1));

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[1] = _commandColumn;

      _editableRow.CreateControls (_value01, columns);

      Assert.IsTrue (_editableRow.HasEditControls());
      Assert.IsTrue (_editableRow.HasEditControl (0));
      Assert.IsFalse (_editableRow.HasEditControl (1));
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException))]
    public void HasEditControlWithNegativeIndex ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[1] = _commandColumn;

      _editableRow.CreateControls (_value01, columns);

      _editableRow.HasEditControl (-1);
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException))]
    public void HasEditControlWithIndexOutOfPositiveRange ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[1] = _commandColumn;

      _editableRow.CreateControls (_value01, columns);

      _editableRow.HasEditControl (3);
    }


    [Test]
    public void GetEditControl ()
    {
      Invoker.InitRecursive();

      Assert.IsFalse (_editableRow.HasEditControls());
      Assert.IsFalse (_editableRow.HasEditControl (0));
      Assert.IsFalse (_editableRow.HasEditControl (0));

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[1] = _commandColumn;

      _editableRow.CreateControls (_value01, columns);

      Assert.IsTrue (_editableRow.HasEditControls());
      Assert.IsTrue (_editableRow.HasEditControl (0));
      Assert.IsFalse (_editableRow.HasEditControl (1));

      IBusinessObjectBoundEditableWebControl control = _editableRow.GetEditControl (0);
      Assert.IsNotNull (control);
      Assert.IsTrue (control is BocTextValue);

      Assert.IsNull (_editableRow.GetEditControl (1));
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException))]
    public void GetEditControlWithNegativeIndex ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[1] = _commandColumn;

      _editableRow.CreateControls (_value01, columns);

      _editableRow.HasEditControl (-1);
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException))]
    public void GetEditControlWithIndexOutOfPositiveRange ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[1] = _commandColumn;

      _editableRow.CreateControls (_value01, columns);

      _editableRow.HasEditControl (3);
    }


    [Test]
    public void HasValidators ()
    {
      Invoker.InitRecursive();

      Assert.IsFalse (_editableRow.HasValidators());
      Assert.IsFalse (_editableRow.HasValidators (0));
      Assert.IsFalse (_editableRow.HasValidators (1));

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[1] = _commandColumn;

      _editableRow.CreateControls (_value01, columns);

      Assert.IsTrue (_editableRow.HasValidators());
      Assert.IsTrue (_editableRow.HasValidators (0));
      Assert.IsFalse (_editableRow.HasValidators (1));

      _editableRow.EnsureValidatorsRestored();

      Assert.IsTrue (_editableRow.HasValidators());
      Assert.IsTrue (_editableRow.HasValidators (0));
      Assert.IsFalse (_editableRow.HasValidators (1));
    }

    [Test]
    public void HasValidatorsWithoutCreateControls ()
    {
      Invoker.InitRecursive();

      Assert.IsFalse (_editableRow.HasValidators());
      Assert.IsFalse (_editableRow.HasValidators (0));
      Assert.IsFalse (_editableRow.HasValidators (1));

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      _editableRow.EnsureValidatorsRestored();

      Assert.IsFalse (_editableRow.HasValidators());
      Assert.IsFalse (_editableRow.HasValidators (0));
      Assert.IsFalse (_editableRow.HasValidators (1));

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[1] = _commandColumn;

      _editableRow.CreateControls (_value01, columns);

      Assert.IsTrue (_editableRow.HasValidators());
      Assert.IsTrue (_editableRow.HasValidators (0));
      Assert.IsFalse (_editableRow.HasValidators (1));

      _editableRow.EnsureValidatorsRestored();

      Assert.IsTrue (_editableRow.HasValidators());
      Assert.IsTrue (_editableRow.HasValidators (0));
      Assert.IsFalse (_editableRow.HasValidators (1));
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException))]
    public void HasValidatorsWithNegativeIndex ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[1] = _commandColumn;

      _editableRow.CreateControls (_value01, columns);
      _editableRow.EnsureValidatorsRestored();

      _editableRow.HasValidators (-1);
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException))]
    public void HasValidatorsWithIndexOutOfPositiveRange ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[1] = _commandColumn;

      _editableRow.CreateControls (_value01, columns);
      _editableRow.EnsureValidatorsRestored();

      _editableRow.HasValidators (3);
    }


    [Test]
    public void GetValidators ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[1] = _commandColumn;

      _editableRow.CreateControls (_value01, columns);
      _editableRow.EnsureValidatorsRestored();

      Assert.IsTrue (_editableRow.HasValidators());
      Assert.IsTrue (_editableRow.HasValidators (0));
      Assert.IsFalse (_editableRow.HasValidators (1));

      ControlCollection validators = _editableRow.GetValidators (0);
      Assert.IsNotNull (validators);
      Assert.AreEqual (2, validators.Count);
      Assert.IsTrue (validators[0] is RequiredFieldValidator);
      Assert.IsTrue (validators[1] is NumericValidator);

      Assert.IsNull (_editableRow.GetValidators (1));
    }

    [Test]
    public void GetValidatorsWithoutCreateControls ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      _editableRow.EnsureValidatorsRestored();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[1] = _commandColumn;

      _editableRow.CreateControls (_value01, columns);
      _editableRow.EnsureValidatorsRestored();

      Assert.IsTrue (_editableRow.HasValidators());
      Assert.IsTrue (_editableRow.HasValidators (0));
      Assert.IsFalse (_editableRow.HasValidators (1));

      ControlCollection validators = _editableRow.GetValidators (0);
      Assert.IsNotNull (validators);
      Assert.AreEqual (2, validators.Count);
      Assert.IsTrue (validators[0] is RequiredFieldValidator);
      Assert.IsTrue (validators[1] is NumericValidator);

      Assert.IsNull (_editableRow.GetValidators (1));
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException))]
    public void GetValidatorsWithNegativeIndex ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[1] = _commandColumn;

      _editableRow.CreateControls (_value01, columns);
      _editableRow.EnsureValidatorsRestored();

      _editableRow.GetValidators (-1);
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException))]
    public void GetValidatorsWithIndexOutOfPositiveRange ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[1] = _commandColumn;

      _editableRow.CreateControls (_value01, columns);
      _editableRow.EnsureValidatorsRestored();

      _editableRow.GetValidators (3);
    }


    [Test]
    public void IsRequired ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[3];
      columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
      columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[2] = _commandColumn;

      _editableRow.CreateControls (_value01, columns);

      Assert.IsFalse (_editableRow.IsRequired (0));
      Assert.IsTrue (_editableRow.IsRequired (1));
      Assert.IsFalse (_editableRow.IsRequired (2));
    }


    [Test]
    public void IsDirty ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[3];
      columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
      columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[2] = _commandColumn;

      _editableRow.CreateControls (_value01, columns);

      IBusinessObjectReferenceDataSource dataSource = _editableRow.GetDataSource();
      dataSource.LoadValues (false);

      Assert.IsFalse (_editableRow.IsDirty());

      BocTextValue textBoxStringValue = (BocTextValue) _editableRow.GetEditControl (0);
      textBoxStringValue.Value = "a";

      Assert.IsTrue (_editableRow.IsDirty());
    }

    [Test]
    public void GetTrackedIDs ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[3];
      columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
      columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[2] = _commandColumn;

      _editableRow.CreateControls (_value01, columns);

      IBusinessObjectReferenceDataSource dataSource = _editableRow.GetDataSource();
      dataSource.LoadValues (false);

      string id = "NamingContainer_Row_{0}_Boc_TextBox";
      string[] trackedIDs = new string[2];
      trackedIDs[0] = string.Format (id, 0);
      trackedIDs[1] = string.Format (id, 1);

      Assert.AreEqual (trackedIDs, _editableRow.GetTrackedClientIDs());
    }


    [Test]
    public void ValidateWithValidValues ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[3];
      columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
      columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[2] = _commandColumn;

      _editableRow.CreateControls (_value01, columns);
      _editableRow.EnsureValidatorsRestored();

      IBusinessObjectReferenceDataSource dataSource = _editableRow.GetDataSource();
      dataSource.LoadValues (false);

      SetValues (_editableRow, "A", "300");

      Assert.IsTrue (_editableRow.Validate());
    }

    [Test]
    public void ValidateWithInvalidValues ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[3];
      columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
      columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[2] = _commandColumn;

      _editableRow.CreateControls (_value01, columns);
      _editableRow.EnsureValidatorsRestored();

      IBusinessObjectReferenceDataSource dataSource = _editableRow.GetDataSource();
      dataSource.LoadValues (false);

      SetValues (_editableRow, "A", "");

      Assert.IsFalse (_editableRow.Validate());
    }


    [Test]
    public void PrepareValidation ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[3];
      columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
      columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[2] = _commandColumn;

      _editableRow.CreateControls (_value01, columns);
      _editableRow.EnsureValidatorsRestored();

      IBusinessObjectReferenceDataSource dataSource = _editableRow.GetDataSource();
      dataSource.LoadValues (false);

      BocTextValue stringValueField = (BocTextValue) _editableRow.GetEditControl (0);
      BocTextValue int32ValueField = (BocTextValue) _editableRow.GetEditControl (1);

      _editableRow.PrepareValidation();

      Assert.AreEqual (stringValueField.Text, stringValueField.Text);
      Assert.AreEqual (int32ValueField.Text, int32ValueField.Text);
    }


    private void SetValues (EditableRow row, string stringValue, string int32Value)
    {
      BocTextValue stringValueField = (BocTextValue) row.GetEditControl (0);
      stringValueField.Text = stringValue;

      BocTextValue int32ValueField = (BocTextValue) row.GetEditControl (1);
      int32ValueField.Text = int32Value;
    }
  }
}