using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList
{

[ToolboxItem (false)]
public class EditModeController : PlaceHolder
{
  // types

  // static members and constants

  private const string c_whiteSpace = "&nbsp;";

  // member fields

  private Controls.BocList _ownerControl;

  private bool _isListEditModeActive;
  private int? _editableRowIndex = null;
  private bool _isEditNewRow = false;
  
  private bool _isEditModeRestored = false;
  
  internal EditableRow[] _rows;
  private EditableRowIDProvider _rowIDProvider;

  private bool _enableEditModeValidator = true;
  private bool _showEditModeRequiredMarkers = true;
  private bool _showEditModeValidationMarkers = false;
  private bool _disableEditModeValidationMessages = false;

  // construction and disposing

  public EditModeController (Controls.BocList ownerControl)
  {
    ArgumentUtility.CheckNotNull ("ownerControl", ownerControl);

    _ownerControl = ownerControl;
  }

  // methods and properties

  public Controls.BocList OwnerControl
  {
    get { return _ownerControl; }
  }

  public void SwitchRowIntoEditMode (int index, BocColumnDefinition[] oldColumns, BocColumnDefinition[] columns)
  {
    ArgumentUtility.CheckNotNullOrItemsNull ("oldColumns", oldColumns);
    ArgumentUtility.CheckNotNullOrItemsNull ("columns", columns);

    if (_ownerControl.Value == null)
    {
      throw new InvalidOperationException (string.Format (
          "Cannot initialize row edit mode: The BocList '{0}' does not have a Value.", _ownerControl.ID));
    }

    if (index < 0) throw new ArgumentOutOfRangeException ("index");
    if (index >= _ownerControl.Value.Count) throw new ArgumentOutOfRangeException ("index");

    RestoreAndEndEditMode (oldColumns);
    
    if (_ownerControl.IsReadOnly || IsListEditModeActive || IsRowEditModeActive)
      return;

    _editableRowIndex = index;
    _rowIDProvider = new EditableRowIDProvider (ID + "_Row{0}");
    CreateEditModeControls (columns);
    LoadValues (false);
  }

  public void SwitchListIntoEditMode (BocColumnDefinition[] oldColumns, BocColumnDefinition[] columns)
  {
    ArgumentUtility.CheckNotNullOrItemsNull ("oldColumns", oldColumns);
    ArgumentUtility.CheckNotNullOrItemsNull ("columns", columns);

    if (_ownerControl.Value == null)
    {
      throw new InvalidOperationException (string.Format (
          "Cannot initialize list edit mode: The BocList '{0}' does not have a Value.", _ownerControl.ID));
    }

    RestoreAndEndEditMode (oldColumns);

    if (_ownerControl.IsReadOnly || IsRowEditModeActive || IsListEditModeActive)
      return;

    _isListEditModeActive = true;
    _rowIDProvider = new EditableRowIDProvider (ID + "_Row{0}");
    CreateEditModeControls (columns);
    LoadValues (false);
  }
  
  public bool AddAndEditRow (IBusinessObject businessObject, BocColumnDefinition[] oldColumns, BocColumnDefinition[] columns)
  {
    ArgumentUtility.CheckNotNullOrItemsNull ("oldColumns", oldColumns);
    ArgumentUtility.CheckNotNullOrItemsNull ("columns", columns);

    RestoreAndEndEditMode (oldColumns);

    if (_ownerControl.IsReadOnly || IsListEditModeActive || IsRowEditModeActive)
      return false;

    int index = AddRow (businessObject, oldColumns, columns);
    if (index < 0)
      return false;

    SwitchRowIntoEditMode (index, oldColumns, columns);
    
    if (! IsRowEditModeActive)
    {
      throw new InvalidOperationException (string.Format (
          "BocList '{0}': Could not switch newly added row into edit mode.", 
          OwnerControl.ID));
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

    if (! _ownerControl.IsReadOnly)
    {
      int index = _editableRowIndex.Value;
      IBusinessObject value = (IBusinessObject) _ownerControl.Value[index];

      if (saveChanges)
      {
        OnEditableRowChangesSaving (index, value, _rows[0].GetDataSource(), _rows[0].GetEditControlsAsArray());
        
        bool isValid = Validate();
        if (! isValid)
          return;

        _ownerControl.IsDirty = IsDirty();

        _rows[0].GetDataSource().SaveValues (false);
        OnEditableRowChangesSaved (index, value);
      }
      else
      {
        OnEditableRowChangesCanceling (index, value, _rows[0].GetDataSource(), _rows[0].GetEditControlsAsArray());
        
        if (_isEditNewRow)
        {
          _ownerControl.RemoveRowInternal (value);
          OnEditableRowChangesCanceled (-1, value);
        }
        else
        {
          OnEditableRowChangesCanceled (index, value);
        }
      }

      _ownerControl.EndRowEditModeCleanUp (_editableRowIndex.Value);
    }

    RemoveEditModeControls();
    _editableRowIndex = null;
    _isEditNewRow = false;
    _rowIDProvider = null;
  }

  public void EndListEditMode (bool saveChanges, BocColumnDefinition[] oldColumns)
  {
    if (! IsListEditModeActive)
      return;

    EnsureEditModeRestored (oldColumns);

    if (! _ownerControl.IsReadOnly)
    {
      IBusinessObject[] values = 
          (IBusinessObject[]) ArrayUtility.Convert (_ownerControl.Value, typeof (IBusinessObject));

      if (saveChanges)
      {
        for (int i = 0; i < _rows.Length; i++)
          OnEditableRowChangesSaving (i, values[i], _rows[i].GetDataSource(), _rows[i].GetEditControlsAsArray());

        bool isValid = Validate();
        if (! isValid)
          return;

        _ownerControl.IsDirty = IsDirty();

        for (int i = 0; i < _rows.Length; i++)
          _rows[i].GetDataSource().SaveValues (false);

        for (int i = 0; i < _rows.Length; i++)
          OnEditableRowChangesSaved (i, values[i]);
      }
      else
      {
        for (int i = 0; i < _rows.Length; i++)
          OnEditableRowChangesCanceling (i, values[i], _rows[i].GetDataSource(), _rows[i].GetEditControlsAsArray());

        //if (_isEditNewRow)
        //{
        //  IBusinessObject editedBusinessObject = values[_editableRowIndex.Value];
        //  RemoveRow (_editableRowIndex.Value);
        //  OnRowEditModeCanceled (-1, editedBusinessObject);
        //}
        //else
        //{
        for (int i = 0; i < _rows.Length; i++)
          OnEditableRowChangesCanceled (i, values[i]);
        //}
      }

      _ownerControl.EndListEditModeCleanUp();
    }

    RemoveEditModeControls();
    _isListEditModeActive = false;
    _rowIDProvider = null;
  }


  private void CreateEditModeControls (BocColumnDefinition[] columns)
  {
    if (IsRowEditModeActive)
    {
      IBusinessObject value = (IBusinessObject) _ownerControl.Value[_editableRowIndex.Value];
      PopulateEditableRows (new IBusinessObject[] {value}, columns);
    }
    else if (IsListEditModeActive)
    {
      PopulateEditableRows (_ownerControl.Value, columns);
    }
  }

  private void PopulateEditableRows (IList values, BocColumnDefinition[] columns)
  {
    EnsureChildControls();

    _rows = new EditableRow[values.Count];
    Controls.Clear();

    for (int i = 0; i < values.Count; i++)
    {
      EditableRow row = CreateEditableRow ((IBusinessObject) values[i], columns);

      _rows[i] = row;
      Controls.Add (row);
    }
  }

  private EditableRow CreateEditableRow (IBusinessObject value, BocColumnDefinition[] columns)
  {
    EditableRow row = new EditableRow (_ownerControl);
    row.ID = _rowIDProvider.GetNextID();

    row.DataSourceFactory = _ownerControl.EditModeDataSourceFactory;
    row.ControlFactory = _ownerControl.EditModeControlFactory;

    row.CreateControls (value, columns);

    return row;
  }

  private void LoadValues (bool interim)
  {
    for (int i = 0; i < _rows.Length; i++)
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
      if (_ownerControl.Value == null)
      {
        throw new InvalidOperationException (string.Format (
            "Cannot restore edit mode: The BocList '{0}' does not have a Value.", _ownerControl.ID));
      }
      if (IsRowEditModeActive && _editableRowIndex.Value >= _ownerControl.Value.Count)
      {
        throw new InvalidOperationException (string.Format ("Cannot restore row edit mode: "
            + "The Value collection of the BocList '{0}' no longer contains the previously edited row.", 
            _ownerControl.ID));
      }

      _rowIDProvider.Reset();

      CreateEditModeControls (oldColumns);
    }
  }

  private void RemoveEditModeControls()
  {
    for (int i = 0; i < _rows.Length; i++)
      _rows[i].RemoveControls();
  }


  public void AddRows (IBusinessObject[] businessObjects, BocColumnDefinition[] oldColumns, BocColumnDefinition[] columns)
  {
    ArgumentUtility.CheckNotNullOrItemsNull ("businessObjects", businessObjects);
    ArgumentUtility.CheckNotNullOrItemsNull ("columns", columns);

    _ownerControl.AddRowsInternal (businessObjects);

    if (_ownerControl.Value != null)
    {
      EnsureEditModeRestored (oldColumns);
      if (IsListEditModeActive)
      {
        int startIndex = _ownerControl.Value.Count - businessObjects.Length;
        ArrayList newRows = new ArrayList (businessObjects.Length);
        for (int i = startIndex; i < _ownerControl.Value.Count; i++)
        {
          EditableRow newRow = CreateEditableRow ((IBusinessObject) _ownerControl.Value[i], columns);
          newRow.GetDataSource().LoadValues (false);
          Controls.Add (newRow);
          newRows.Add (newRow);
        }
        _rows = (EditableRow[]) ListUtility.AddRange (_rows, newRows, (CreateListMethod) null, true, true);
      }
    }
  }

  public int AddRow (IBusinessObject businessObject, BocColumnDefinition[] oldColumns, BocColumnDefinition[] columns)
  {
    ArgumentUtility.CheckNotNull ("businessObject", businessObject);
    ArgumentUtility.CheckNotNullOrItemsNull ("columns", columns);
  
    int index = _ownerControl.AddRowInternal (businessObject);

    if (index != -1)
    {
      EnsureEditModeRestored (oldColumns);
      if (IsListEditModeActive)
      {
        EditableRow newRow = CreateEditableRow (businessObject, columns);
        newRow.GetDataSource().LoadValues (false);
        Controls.Add (newRow);
        _rows = (EditableRow[]) ListUtility.AddRange (_rows, newRow, (CreateListMethod) null, true, true);
      }
    }

    return index;
  }

  public void RemoveRows (IBusinessObject[] businessObjects)
  {
    ArgumentUtility.CheckNotNullOrItemsNull ("businessObjects", businessObjects);

    if (_ownerControl.Value != null)
    {
      if (IsRowEditModeActive)
      {
        throw new InvalidOperationException (string.Format (
            "Cannot remove rows while the BocList '{0}' is in row edit mode. "
            + "Call EndEditMode() before removing the rows.",
            _ownerControl.ID));
      }
      else if (IsListEditModeActive)
      {
        int[] indices = Remotion.Utilities.ListUtility.IndicesOf (_ownerControl.Value, businessObjects, false);
        ArrayList rows = new ArrayList (indices.Length);
        foreach (int index in indices)
        {
          EditableRow row = _rows[index];
          Controls.Remove (row);
          row.RemoveControls();
          _rowIDProvider.ExcludeID (row.ID);
          rows.Add (row);
        }
        _rows = (EditableRow[]) ListUtility.Remove (_rows, rows, (CreateListMethod) null, true);
      }
    }

    _ownerControl.RemoveRowsInternal (businessObjects);
  }

  public void RemoveRow (IBusinessObject businessObject)
  {
    ArgumentUtility.CheckNotNull ("businessObject", businessObject);
    
    if (_ownerControl.Value != null)
    {
      if (IsRowEditModeActive)
      {
        throw new InvalidOperationException (string.Format (
          "Cannot remove a row while the BocList '{0}' is in row edit mode. Call EndEditMode() before removing the row.",
          _ownerControl.ID));
      }  
      else if (IsListEditModeActive)
      {
        int index = Remotion.Utilities.ListUtility.IndexOf (_ownerControl.Value, businessObject);
        if (index != -1)
        {
          EditableRow row = _rows[index];
          Controls.Remove (row);
          row.RemoveControls();
          _rowIDProvider.ExcludeID (row.ID);
          _rows = (EditableRow[]) ListUtility.Remove (_rows, row, (CreateListMethod) null, true);
        }
      }
    }

    _ownerControl.RemoveRowInternal (businessObject);
  }


  public bool IsRowEditModeActive
  {
    get { return _editableRowIndex != null; } 
  }

  public bool IsListEditModeActive
  {
    get { return _isListEditModeActive; } 
  }

  public int? EditableRowIndex
  {
    get { return _editableRowIndex; }
  }


  public BaseValidator[] CreateValidators (IResourceManager resourceManager)
  {
    ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);

    if (! (IsListEditModeActive || IsRowEditModeActive) || ! _enableEditModeValidator)
      return new BaseValidator[0];

    BaseValidator[] validators = new BaseValidator[1];

    EditModeValidator editModeValidator = new EditModeValidator (_ownerControl);
    editModeValidator.ID = ID + "_ValidatorEditMode";
    editModeValidator.ControlToValidate = _ownerControl.ID;
    if (StringUtility.IsNullOrEmpty (_ownerControl.ErrorMessage))
    {
      if (IsRowEditModeActive)
      {
        editModeValidator.ErrorMessage = 
            resourceManager.GetString (UI.Controls.BocList.ResourceIdentifier.RowEditModeErrorMessage);
      }
      else if (IsListEditModeActive)
      {
        editModeValidator.ErrorMessage = 
            resourceManager.GetString (UI.Controls.BocList.ResourceIdentifier.ListEditModeErrorMessage);
     }
    }
    else
    {
      editModeValidator.ErrorMessage = _ownerControl.ErrorMessage;
    }
    validators[0] = editModeValidator;

    return validators;
  }

  /// <remarks>
  ///   Validators must be added to the controls collection after LoadPostData is complete.
  ///   If not, invalid validators will know that they are invalid without first calling validate.
  /// </remarks>
  public void EnsureValidatorsRestored()
  {
    if (IsRowEditModeActive || IsListEditModeActive)
    {
      for (int i = 0; i < _rows.Length; i++)
        _rows[i].EnsureValidatorsRestored();
    }
  }

  public void PrepareValidation ()
  {
    if (IsRowEditModeActive || IsListEditModeActive)
    {
      for (int i = 0; i < _rows.Length; i++)
        _rows[i].PrepareValidation();
    }
  }

  public bool Validate()
  {
    EnsureValidatorsRestored();

    bool isValid = true;
    
    if (IsRowEditModeActive || IsListEditModeActive)
    {
      for (int i = 0; i < _rows.Length; i++)
        isValid &= _rows[i].Validate();
    
      isValid &= _ownerControl.ValidateEditableRowsInternal();
    }

    return isValid;
  }

  
  public void RenderTitleCellMarkers (HtmlTextWriter writer, BocColumnDefinition column, int columnIndex)
  {
    ArgumentUtility.CheckNotNull ("writer", writer);
    ArgumentUtility.CheckNotNull ("column", column);
    
    if (_showEditModeRequiredMarkers && IsRequired (columnIndex))
    {
      Image requriedFieldMarker = _ownerControl.GetRequiredMarker();
      requriedFieldMarker.RenderControl (writer);
      writer.Write (c_whiteSpace);
    }
  }

  public bool IsRequired (int columnIndex)
  {
    if (IsRowEditModeActive || IsListEditModeActive)
    {
      for (int i = 0; i < _rows.Length; i++)
      {
        if (_rows[i].IsRequired (columnIndex))
          return true;
      }
    }

    return false;
  }

  
  public bool IsDirty()
  {
    if (IsRowEditModeActive || IsListEditModeActive)
    {
      for (int i = 0; i < _rows.Length; i++)
      {
        if (_rows[i].IsDirty())
          return true;
      }
    }

    return false;
  }

  public string[] GetTrackedClientIDs()
  {
    if (IsRowEditModeActive || IsListEditModeActive)
    {
      StringCollection trackedIDs = new StringCollection();
      for (int i = 0; i < _rows.Length; i++)
        trackedIDs.AddRange (_rows[i].GetTrackedClientIDs());

      string[] trackedIDsArray = new string[trackedIDs.Count];
      trackedIDs.CopyTo (trackedIDsArray, 0);
      return trackedIDsArray;
    }
    else
    {
      return new string[0];
    }
  }

  protected override void OnInit (EventArgs e)
  {
    base.OnInit (e);

    if (!ControlHelper.IsDesignMode (this))
    {
      Page.RegisterRequiresControlState (this);
    }
  }

  protected override void LoadControlState (object savedState)
  {
    if (savedState == null)
    {
      base.LoadControlState (null);
    }
    else
    {
      object[] values = (object[]) savedState;

      base.LoadControlState (values[0]);
      _isListEditModeActive = (bool) values[1];
      _editableRowIndex = (int?) values[2];
      _isEditNewRow = (bool) values[3];
      _rowIDProvider = (EditableRowIDProvider) values[4];
    }
  }

  protected override object SaveControlState()
  {
    object[] values = new object[5];

    values[0] = base.SaveControlState ();
    values[1] = _isListEditModeActive;
    values[2] = _editableRowIndex;
    values[3] = _isEditNewRow;
    values[4] = _rowIDProvider;

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

    _ownerControl.OnEditableRowChangesSaving (index, businessObject, dataSource, controls);
  }

  protected virtual void OnEditableRowChangesSaved (int index, IBusinessObject businessObject)
  {
    ArgumentUtility.CheckNotNull ("businessObject", businessObject);

    _ownerControl.OnEditableRowChangesSaved (index, businessObject);
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

    _ownerControl.OnEditableRowChangesCanceling (index, businessObject, dataSource, controls);
  }

  protected virtual void OnEditableRowChangesCanceled (int index, IBusinessObject businessObject)
  {
    ArgumentUtility.CheckNotNull ("businessObject", businessObject);

    _ownerControl.OnEditableRowChangesCanceled (index, businessObject);
  }


  public bool ShowEditModeRequiredMarkers
  {
    get { return _showEditModeRequiredMarkers; }
    set { _showEditModeRequiredMarkers = value; }
  }

  public bool ShowEditModeValidationMarkers
  {
    get { return _showEditModeValidationMarkers; }
    set { _showEditModeValidationMarkers = value; }
  }

  public bool DisableEditModeValidationMessages
  {
    get { return _disableEditModeValidationMessages; }
    set { _disableEditModeValidationMessages = value; }
  }

  public bool EnableEditModeValidator
  {
    get { return _enableEditModeValidator; }
    set { _enableEditModeValidator = value; }
  }
}

}
