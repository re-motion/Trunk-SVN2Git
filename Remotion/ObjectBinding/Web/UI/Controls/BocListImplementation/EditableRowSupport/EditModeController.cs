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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport
{
  [ToolboxItem (false)]
  public class EditModeController : PlaceHolder, IEditModeController
  {
    // types

    // static members and constants

    private const string c_whiteSpace = "&nbsp;";

    // member fields

    private readonly IEditModeHost _editModeHost;

    private bool _isListEditModeActive;
    private string _editedRowID;
    private bool _isEditNewRow;

    private bool _isEditModeRestored;

    private readonly List<EditableRow> _rows = new List<EditableRow>();

    // construction and disposing

    public EditModeController (IEditModeHost editModeHost)
    {
      ArgumentUtility.CheckNotNull ("editModeHost", editModeHost);

      _editModeHost = editModeHost;
    }

    // methods and properties

    public IEditableRow GetEditableRow (int originalRowIndex)
    {
      if (IsRowEditModeActive && (GetEditedRow().Index == originalRowIndex))
        return _rows[0];
      else if (IsListEditModeActive)
        return _rows[originalRowIndex];
      else
        return null;
    }

    public void SwitchRowIntoEditMode (int index, BocColumnDefinition[] oldColumns, BocColumnDefinition[] columns)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("oldColumns", oldColumns);
      ArgumentUtility.CheckNotNullOrItemsNull ("columns", columns);

      if (_editModeHost.Value == null)
      {
        throw new InvalidOperationException (
            string.Format ("Cannot initialize row edit mode: The BocList '{0}' does not have a Value.", _editModeHost.ID));
      }

      if (index < 0)
        throw new ArgumentOutOfRangeException ("index");
      if (index >= _editModeHost.Value.Count)
        throw new ArgumentOutOfRangeException ("index");

      RestoreAndEndEditMode (oldColumns);

      if (_editModeHost.IsReadOnly || IsListEditModeActive || IsRowEditModeActive)
        return;

      _editedRowID = _editModeHost.RowIDProvider.GetItemRowID (new BocListRow (index, (IBusinessObject) _editModeHost.Value[index]));
      CreateEditModeControls (columns);
      LoadValues (false);
    }

    public void SwitchListIntoEditMode (BocColumnDefinition[] oldColumns, BocColumnDefinition[] columns)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("oldColumns", oldColumns);
      ArgumentUtility.CheckNotNullOrItemsNull ("columns", columns);

      if (_editModeHost.Value == null)
      {
        throw new InvalidOperationException (
            string.Format ("Cannot initialize list edit mode: The BocList '{0}' does not have a Value.", _editModeHost.ID));
      }

      RestoreAndEndEditMode (oldColumns);

      if (_editModeHost.IsReadOnly || IsRowEditModeActive || IsListEditModeActive)
        return;

      _isListEditModeActive = true;
      CreateEditModeControls (columns);
      LoadValues (false);
    }

    public bool AddAndEditRow (IBusinessObject businessObject, BocColumnDefinition[] oldColumns, BocColumnDefinition[] columns)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("oldColumns", oldColumns);
      ArgumentUtility.CheckNotNullOrItemsNull ("columns", columns);

      RestoreAndEndEditMode (oldColumns);

      if (_editModeHost.IsReadOnly || IsListEditModeActive || IsRowEditModeActive)
        return false;

      int index = AddRow (businessObject, oldColumns, columns);
      if (index < 0)
        return false;

      SwitchRowIntoEditMode (index, oldColumns, columns);

      if (! IsRowEditModeActive)
      {
        throw new InvalidOperationException (
            string.Format ("BocList '{0}': Could not switch newly added row into edit mode.", _editModeHost.ID));
      }
      _isEditNewRow = true;
      return true;
    }

    private void RestoreAndEndEditMode (BocColumnDefinition[] oldColumns)
    {
      EnsureEditModeRestored (oldColumns);

      if (IsRowEditModeActive)
        EndRowEditMode (true, oldColumns);
      else if (IsListEditModeActive)
        EndListEditMode (true, oldColumns);
    }

    public void EndRowEditMode (bool saveChanges, BocColumnDefinition[] oldColumns)
    {
      if (! IsRowEditModeActive)
        return;

      EnsureEditModeRestored (oldColumns);

      if (! _editModeHost.IsReadOnly)
      {
        var editedRow = GetEditedRow();

        if (saveChanges)
        {
          OnEditableRowChangesSaving (editedRow.Index, editedRow.BusinessObject, _rows[0].GetDataSource(), _rows[0].GetEditControlsAsArray());

          bool isValid = Validate();
          if (! isValid)
            return;

          _editModeHost.IsDirty = IsDirty();

          _rows[0].GetDataSource().SaveValues (false);
          OnEditableRowChangesSaved (editedRow.Index, editedRow.BusinessObject);
        }
        else
        {
          OnEditableRowChangesCanceling (editedRow.Index, editedRow.BusinessObject, _rows[0].GetDataSource(), _rows[0].GetEditControlsAsArray());

          if (_isEditNewRow)
          {
            _editModeHost.RemoveRows (new[] { editedRow.BusinessObject });
            OnEditableRowChangesCanceled (-1, editedRow.BusinessObject);
          }
          else
            OnEditableRowChangesCanceled (editedRow.Index, editedRow.BusinessObject);
        }

        _editModeHost.EndRowEditModeCleanUp (editedRow.Index);
      }

      RemoveEditModeControls();
      _editedRowID = null;
      _isEditNewRow = false;
    }

    public void EndListEditMode (bool saveChanges, BocColumnDefinition[] oldColumns)
    {
      if (! IsListEditModeActive)
        return;

      EnsureEditModeRestored (oldColumns);

      if (! _editModeHost.IsReadOnly)
      {
        IBusinessObject[] values =
            (IBusinessObject[]) ArrayUtility.Convert (_editModeHost.Value, typeof (IBusinessObject));

        if (saveChanges)
        {
          for (int i = 0; i < _rows.Count; i++)
            OnEditableRowChangesSaving (i, values[i], _rows[i].GetDataSource(), _rows[i].GetEditControlsAsArray());

          bool isValid = Validate();
          if (! isValid)
            return;

          _editModeHost.IsDirty = IsDirty();

          for (int i = 0; i < _rows.Count; i++)
            _rows[i].GetDataSource().SaveValues (false);

          for (int i = 0; i < _rows.Count; i++)
            OnEditableRowChangesSaved (i, values[i]);
        }
        else
        {
          for (int i = 0; i < _rows.Count; i++)
            OnEditableRowChangesCanceling (i, values[i], _rows[i].GetDataSource(), _rows[i].GetEditControlsAsArray());

          //if (_isEditNewRow)
          //{
          //  IBusinessObject editedBusinessObject = values[_editableRowIndex.Value];
          //  RemoveRow (_editableRowIndex.Value);
          //  OnRowEditModeCanceled (-1, editedBusinessObject);
          //}
          //else
          //{
          for (int i = 0; i < _rows.Count; i++)
            OnEditableRowChangesCanceled (i, values[i]);
          //}
        }

        _editModeHost.EndListEditModeCleanUp();
      }

      RemoveEditModeControls();
      _isListEditModeActive = false;
    }


    private void CreateEditModeControls (BocColumnDefinition[] columns)
    {
      if (IsRowEditModeActive)
        PopulateEditableRows (new[] { GetEditedRow() }, columns);
      else if (IsListEditModeActive)
        PopulateEditableRows (_editModeHost.Value.Cast<IBusinessObject>().Select ((o, i) => new BocListRow (i, o)), columns);
    }

    private void PopulateEditableRows (IEnumerable<BocListRow> bocListRows, BocColumnDefinition[] columns)
    {
      EnsureChildControls();

      Assertion.IsTrue (_rows.Count == 0, "Populating the editable rows only happens after the last edit mode was ended.");
      Assertion.IsTrue (Controls.Count == 0, "Populating the editable rows only happens after the last edit mode was ended.");

      foreach (var bocListRow in bocListRows)
        AddRowToDataStructure (bocListRow, columns);
    }

    private EditableRow CreateEditableRow (BocListRow bocListRow, BocColumnDefinition[] columns)
    {
      EditableRow row = new EditableRow (_editModeHost);
      row.ID = GetRowID (bocListRow);

      row.DataSourceFactory = _editModeHost.EditModeDataSourceFactory;
      row.ControlFactory = _editModeHost.EditModeControlFactory;

      row.CreateControls (bocListRow.BusinessObject, columns);

      return row;
    }

    private void LoadValues (bool interim)
    {
      for (int i = 0; i < _rows.Count; i++)
        _rows[i].GetDataSource().LoadValues (interim);
    }

    public void EnsureEditModeRestored (BocColumnDefinition[] oldColumns)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("oldColumns", oldColumns);

      if (_isEditModeRestored)
        return;
      _isEditModeRestored = true;

      if (IsRowEditModeActive || IsListEditModeActive)
      {
        if (_editModeHost.Value == null)
        {
          throw new InvalidOperationException (
              string.Format ("Cannot restore edit mode: The BocList '{0}' does not have a Value.", _editModeHost.ID));
        }
        if (IsRowEditModeActive && _editModeHost.RowIDProvider.GetRowFromItemRowID (_editModeHost.Value, _editedRowID) == null)
        {
          throw new InvalidOperationException (
              string.Format (
                  "Cannot restore row edit mode: The Value collection of the BocList '{0}' no longer contains the previously edited row.",
                  _editModeHost.ID));
        }

        CreateEditModeControls (oldColumns);
      }
    }

    private void RemoveEditModeControls ()
    {
      for (int i = _rows.Count - 1; i >= 0; i--)
        RemoveRowFromDataStructure (i);
    }


    public BocListRow[] AddRows (IBusinessObject[] businessObjects, BocColumnDefinition[] oldColumns, BocColumnDefinition[] columns)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("businessObjects", businessObjects);
      ArgumentUtility.CheckNotNullOrItemsNull ("columns", columns);

      var bocListRows = _editModeHost.AddRows (businessObjects);

      if (_editModeHost.Value != null)
      {
        EnsureEditModeRestored (oldColumns);
        if (IsListEditModeActive)
        {
          foreach (var bocListRow in bocListRows.OrderBy (r=>r.Index))
          {
            var newRow = AddRowToDataStructure (bocListRow, columns);
            newRow.GetDataSource().LoadValues (false);
          }
        }
      }

      return bocListRows;
    }

    public int AddRow (IBusinessObject businessObject, BocColumnDefinition[] oldColumns, BocColumnDefinition[] columns)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);
      ArgumentUtility.CheckNotNullOrItemsNull ("columns", columns);

      var bocListRows = AddRows (new[] { businessObject }, oldColumns, columns);

      if (bocListRows.Length == 0)
        return - 1;
      return bocListRows.Single().Index;
    }

    private EditableRow AddRowToDataStructure (BocListRow bocListRow, BocColumnDefinition[] columns)
    {
      EditableRow row = CreateEditableRow (bocListRow, columns);
      Controls.Add (row);
      _rows.Add (row);

      return row;
    }

    public void RemoveRows (IBusinessObject[] businessObjects)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("businessObjects", businessObjects);

      var bocListRows = _editModeHost.RemoveRows (businessObjects);

      if (_editModeHost.Value != null)
      {
        if (IsRowEditModeActive)
        {
          throw new InvalidOperationException (
              string.Format (
                  "Cannot remove rows while the BocList '{0}' is in row edit mode. Call EndEditMode() before removing the rows.",
                  _editModeHost.ID));
        }
        else if (IsListEditModeActive)
        {
          foreach (var row in bocListRows.OrderByDescending (r => r.Index))
            RemoveRowFromDataStructure (row.Index);
        }
      }
    }

    public void RemoveRow (IBusinessObject businessObject)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);

      RemoveRows (new[] { businessObject });
    }

    private void RemoveRowFromDataStructure (int index)
    {
      EditableRow row = _rows[index];
      row.RemoveControls();
      Controls.Remove (row);
      _rows.RemoveAt (index);
    }

    public bool IsRowEditModeActive
    {
      get { return _editedRowID != null; }
    }

    public bool IsListEditModeActive
    {
      get { return _isListEditModeActive; }
    }

    public BocListRow GetEditedRow()
    {
      if (!IsRowEditModeActive)
      {
        throw new InvalidOperationException (
            string.Format ("Cannot retrieve edited row: The BocList '{0}' is not in row edit mode.", _editModeHost.ID));
      }

      if (_editModeHost.Value == null)
      {
        throw new InvalidOperationException (string.Format ("Cannot retrieve edited row: The BocList '{0}' does not have a Value.", _editModeHost.ID));
      }

      var editedRow = _editModeHost.RowIDProvider.GetRowFromItemRowID (_editModeHost.Value, _editedRowID);
      if (editedRow == null)
      {
        throw new InvalidOperationException (
            string.Format (
                "Cannot retrieve edited row: The BocList '{0}' no longer contains the edited row in its Value collection.", _editModeHost.ID));
      }

      return editedRow;
    }

    public BaseValidator[] CreateValidators (IResourceManager resourceManager)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);

      if (! (IsListEditModeActive || IsRowEditModeActive) || ! _editModeHost.EnableEditModeValidator)
        return new BaseValidator[0];

      BaseValidator[] validators = new BaseValidator[1];

      EditModeValidator editModeValidator = new EditModeValidator (this);
      editModeValidator.ID = ID + "_ValidatorEditMode";
      editModeValidator.ControlToValidate = _editModeHost.ID;
      if (StringUtility.IsNullOrEmpty (_editModeHost.ErrorMessage))
      {
        if (IsRowEditModeActive)
          editModeValidator.ErrorMessage = resourceManager.GetString (UI.Controls.BocList.ResourceIdentifier.RowEditModeErrorMessage);
        else if (IsListEditModeActive)
          editModeValidator.ErrorMessage = resourceManager.GetString (UI.Controls.BocList.ResourceIdentifier.ListEditModeErrorMessage);
      }
      else
        editModeValidator.ErrorMessage = _editModeHost.ErrorMessage;
      validators[0] = editModeValidator;

      return validators;
    }

    /// <remarks>
    ///   Validators must be added to the controls collection after LoadPostData is complete.
    ///   If not, invalid validators will know that they are invalid without first calling validate.
    /// </remarks>
    public void EnsureValidatorsRestored ()
    {
      if (IsRowEditModeActive || IsListEditModeActive)
      {
        for (int i = 0; i < _rows.Count; i++)
          _rows[i].EnsureValidatorsRestored();
      }
    }

    public void PrepareValidation ()
    {
      if (IsRowEditModeActive || IsListEditModeActive)
      {
        for (int i = 0; i < _rows.Count; i++)
          _rows[i].PrepareValidation();
      }
    }

    public bool Validate ()
    {
      EnsureValidatorsRestored();

      bool isValid = true;

      if (IsRowEditModeActive || IsListEditModeActive)
      {
        for (int i = 0; i < _rows.Count; i++)
          isValid &= _rows[i].Validate();

        isValid &= _editModeHost.ValidateEditableRows();
      }

      return isValid;
    }


    public void RenderTitleCellMarkers (HtmlTextWriter writer, BocColumnDefinition column, int columnIndex)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("column", column);

      if (_editModeHost.ShowEditModeRequiredMarkers && IsRequired (columnIndex))
      {
        Image requriedFieldMarker = _editModeHost.GetRequiredMarker();
        requriedFieldMarker.RenderControl (writer);
        writer.Write (c_whiteSpace);
      }
    }

    public bool IsRequired (int columnIndex)
    {
      if (IsRowEditModeActive || IsListEditModeActive)
      {
        for (int i = 0; i < _rows.Count; i++)
        {
          if (_rows[i].IsRequired (columnIndex))
            return true;
        }
      }

      return false;
    }


    public bool IsDirty ()
    {
      if (IsRowEditModeActive || IsListEditModeActive)
      {
        for (int i = 0; i < _rows.Count; i++)
        {
          if (_rows[i].IsDirty())
            return true;
        }
      }

      return false;
    }

    public string[] GetTrackedClientIDs ()
    {
      if (IsRowEditModeActive || IsListEditModeActive)
      {
        StringCollection trackedIDs = new StringCollection();
        for (int i = 0; i < _rows.Count; i++)
          trackedIDs.AddRange (_rows[i].GetTrackedClientIDs());

        string[] trackedIDsArray = new string[trackedIDs.Count];
        trackedIDs.CopyTo (trackedIDsArray, 0);
        return trackedIDsArray;
      }
      else
        return new string[0];
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      if (!ControlHelper.IsDesignMode (this))
        Page.RegisterRequiresControlState (this);
    }

    protected override void LoadControlState (object savedState)
    {
      if (savedState == null)
        base.LoadControlState (null);
      else
      {
        object[] values = (object[]) savedState;

        base.LoadControlState (values[0]);
        _isListEditModeActive = (bool) values[1];
        _editedRowID = (string) values[2];
        _isEditNewRow = (bool) values[3];
      }
    }

    protected override object SaveControlState ()
    {
      object[] values = new object[4];

      values[0] = base.SaveControlState();
      values[1] = _isListEditModeActive;
      values[2] = _editedRowID;
      values[3] = _isEditNewRow;

      return values;
    }


    protected virtual void OnEditableRowChangesSaving (
        int index,
        IBusinessObject businessObject,
        IBusinessObjectDataSource dataSource,
        IBusinessObjectBoundEditableWebControl[] controls)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);
      ArgumentUtility.CheckNotNull ("dataSource", dataSource);
      ArgumentUtility.CheckNotNull ("controls", controls);

      _editModeHost.OnEditableRowChangesSaving (index, businessObject, dataSource, controls);
    }

    protected virtual void OnEditableRowChangesSaved (int index, IBusinessObject businessObject)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);

      _editModeHost.OnEditableRowChangesSaved (index, businessObject);
    }

    protected virtual void OnEditableRowChangesCanceling (
        int index,
        IBusinessObject businessObject,
        IBusinessObjectDataSource dataSource,
        IBusinessObjectBoundEditableWebControl[] controls)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);
      ArgumentUtility.CheckNotNull ("dataSource", dataSource);
      ArgumentUtility.CheckNotNull ("controls", controls);

      _editModeHost.OnEditableRowChangesCanceling (index, businessObject, dataSource, controls);
    }

    protected virtual void OnEditableRowChangesCanceled (int index, IBusinessObject businessObject)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);

      _editModeHost.OnEditableRowChangesCanceled (index, businessObject);
    }

    private string GetRowID (BocListRow row)
    {
      return ID + "_Row_" + _editModeHost.RowIDProvider.GetControlRowID (row);
    }

    IPage IControl.Page
    {
      get { return PageWrapper.CastOrCreate (base.Page); }
    }
  }
}