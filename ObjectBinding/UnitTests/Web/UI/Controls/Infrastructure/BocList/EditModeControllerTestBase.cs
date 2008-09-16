/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Specialized;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList;
using Remotion.ObjectBinding.UnitTests.Web.Domain;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Infrastructure.BocList
{
  public class EditModeControllerTestBase : BocTest
  {
    private StringCollection _actualEvents;

    private Remotion.ObjectBinding.Web.UI.Controls.BocList _bocList;
    private EditModeController _controller;
    private ControlInvoker _controllerInvoker;

    private IBusinessObject[] _values;
    private IBusinessObject[] _newValues;

    private BindableObjectClass _class;

    private IBusinessObjectPropertyPath _stringValuePath;
    private IBusinessObjectPropertyPath _int32ValuePath;

    private BocColumnDefinition[] _columns;

    private BocSimpleColumnDefinition _stringValueSimpleColumn;
    private BocSimpleColumnDefinition _int32ValueSimpleColumn;

    public override void SetUp ()
    {
      base.SetUp();

      _actualEvents = new StringCollection();

      _values = new IBusinessObject[5];
      _values[0] = (IBusinessObject) TypeWithAllDataTypes.Create ("A", 1);
      _values[1] = (IBusinessObject) TypeWithAllDataTypes.Create ("B", 2);
      _values[2] = (IBusinessObject) TypeWithAllDataTypes.Create ("C", 3);
      _values[3] = (IBusinessObject) TypeWithAllDataTypes.Create ("D", 4);
      _values[4] = (IBusinessObject) TypeWithAllDataTypes.Create ("E", 5);

      _newValues = new IBusinessObject[2];
      _newValues[0] = (IBusinessObject) TypeWithAllDataTypes.Create ("F", 6);
      _newValues[1] = (IBusinessObject) TypeWithAllDataTypes.Create ("G", 7);

      _class = BindableObjectProvider.GetBindableObjectClass(typeof (TypeWithAllDataTypes));

      _stringValuePath = BusinessObjectPropertyPath.Parse (_class, "String");
      _int32ValuePath = BusinessObjectPropertyPath.Parse (_class, "Int32");

      _stringValueSimpleColumn = new BocSimpleColumnDefinition();
      _stringValueSimpleColumn.SetPropertyPath (_stringValuePath);

      _int32ValueSimpleColumn = new BocSimpleColumnDefinition();
      _int32ValueSimpleColumn.SetPropertyPath (_int32ValuePath);

      _columns = new BocColumnDefinition[2];
      _columns[0] = _stringValueSimpleColumn;
      _columns[1] = _int32ValueSimpleColumn;

      _bocList = new Remotion.ObjectBinding.Web.UI.Controls.BocList();
      _bocList.ID = "BocList";
      NamingContainer.Controls.Add (_bocList);

      _controller = new EditModeController (_bocList);
      _controller.ID = "Controller";
      NamingContainer.Controls.Add (_controller);

      _controllerInvoker = new ControlInvoker (_controller);

      _bocList.EditableRowChangesCanceled += new BocListItemEventHandler (Boclist_EditableRowChangesCanceled);
      _bocList.EditableRowChangesCanceling += new BocListEditableRowChangesEventHandler (Boclist_EditableRowChangesCanceling);
      _bocList.EditableRowChangesSaved += new BocListItemEventHandler (Boclist_EditableRowChangesSaved);
      _bocList.EditableRowChangesSaving += new BocListEditableRowChangesEventHandler (Boclist_EditableRowChangesSaving);

      _bocList.FixedColumns.AddRange (_columns);
      _bocList.LoadUnboundValue (_values, false);
    }

    protected StringCollection ActualEvents
    {
      get { return _actualEvents; }
    }

    protected Remotion.ObjectBinding.Web.UI.Controls.BocList BocList
    {
      get { return _bocList; }
    }

    protected EditModeController Controller
    {
      get { return _controller; }
    }

    public ControlInvoker ControllerInvoker
    {
      get { return _controllerInvoker; }
    }

    protected IBusinessObject[] Values
    {
      get { return _values; }
    }

    protected IBusinessObject[] NewValues
    {
      get { return _newValues; }
    }

    protected BocColumnDefinition[] Columns
    {
      get { return _columns; }
    }

    protected void SetValues (EditableRow row, string stringValue, string int32Value)
    {
      ArgumentUtility.CheckNotNull ("row", row);

      BocTextValue stringValueField = (BocTextValue) row.GetEditControl (0);
      stringValueField.TextBox.Text = stringValue;
      stringValueField.Text = stringValue;

      BocTextValue int32ValueField = (BocTextValue) row.GetEditControl (1);
      int32ValueField.TextBox.Text = int32Value;
      int32ValueField.Text = int32Value;
    }

    protected void CheckValues (IBusinessObject value, string stringValue, int int32Value)
    {
      TypeWithAllDataTypes typeWithAllDataTypes = ArgumentUtility.CheckNotNullAndType<TypeWithAllDataTypes> ("value", value);

      Assert.AreEqual (stringValue, typeWithAllDataTypes.String);
      Assert.AreEqual (int32Value, typeWithAllDataTypes.Int32);
    }

    protected void CheckEvents (StringCollection expected, StringCollection actual)
    {
      CollectionAssert.AreEqual (expected, actual);
    }

    protected string FormatChangesCanceledEventMessage (int index, IBusinessObject businessObject)
    {
      return FormatEventMessage ("ChangesCanceled", index, businessObject);
    }

    protected string FormatChangesCancelingEventMessage (int index, IBusinessObject businessObject)
    {
      return FormatEventMessage ("ChangesCanceling", index, businessObject);
    }

    protected string FormatChangesSavedEventMessage (int index, IBusinessObject businessObject)
    {
      return FormatEventMessage ("ChangesSaved", index, businessObject);
    }

    protected string FormatChangesSavingEventMessage (int index, IBusinessObject businessObject)
    {
      return FormatEventMessage ("ChangesSaving", index, businessObject);
    }

    private string FormatEventMessage (string eventName, int index, IBusinessObject businessObject)
    {
      return string.Format ("{0}: {1}, {2}", eventName, index, businessObject.ToString());
    }

    private void Boclist_EditableRowChangesCanceled (object sender, BocListItemEventArgs e)
    {
      _actualEvents.Add (FormatChangesCanceledEventMessage (e.ListIndex, e.BusinessObject));
    }

    private void Boclist_EditableRowChangesCanceling (object sender, BocListEditableRowChangesEventArgs e)
    {
      _actualEvents.Add (FormatChangesCancelingEventMessage (e.ListIndex, e.BusinessObject));
    }

    private void Boclist_EditableRowChangesSaved (object sender, BocListItemEventArgs e)
    {
      _actualEvents.Add (FormatChangesSavedEventMessage (e.ListIndex, e.BusinessObject));
    }

    private void Boclist_EditableRowChangesSaving (object sender, BocListEditableRowChangesEventArgs e)
    {
      _actualEvents.Add (FormatChangesSavingEventMessage (e.ListIndex, e.BusinessObject));
    }

    protected object CreateControlState (
        object baseControlState,
        bool isListEditModeActive,
        int? editableRowIndex,
        bool isEditNewRow,
        EditableRowIDProvider rowIDProvider)
    {
      object[] values = new object[5];

      values[0] = baseControlState;
      values[1] = isListEditModeActive;
      values[2] = editableRowIndex;
      values[3] = isEditNewRow;
      values[4] = rowIDProvider;

      return values;
    }
  }
}
