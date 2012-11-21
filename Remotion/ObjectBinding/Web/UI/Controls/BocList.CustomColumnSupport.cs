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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public partial class BocList
  {
    private readonly Dictionary<BocCustomColumnDefinition, BocListCustomColumnTuple[]> _customColumnControls =
        new Dictionary<BocCustomColumnDefinition, BocListCustomColumnTuple[]>();

    private readonly PlaceHolder _customColumnsPlaceHolder = new PlaceHolder();

    private void CreateChildControlsForCustomColumns ()
    {
      Controls.Add (_customColumnsPlaceHolder);
    }

    /// <summary> Restores the custom columns from the previous life cycle. </summary>
    private void RestoreCustomColumns ()
    {
      if (! Page.IsPostBack)
        return;
      CreateCustomColumnControls (EnsureColumnsForPreviousLifeCycleGot());
      InitCustomColumns();
      LoadCustomColumns();
    }

    /// <summary> Creates the controls for the custom columns in the <paramref name="columns"/> array. </summary>
    private void CreateCustomColumnControls (BocColumnDefinition[] columns)
    {
      _customColumnControls.Clear();
      _customColumnsPlaceHolder.Controls.Clear();

      if (IsDesignMode)
        return;
      if (!HasValue)
        return;

      EnsureChildControls();

      CalculateCurrentPage (null);

      var controlEnabledCustomColumns = columns
          .Select ((column, index) => new { Column = column as BocCustomColumnDefinition, Index = index })
          .Where (d => d.Column != null)
          .Where (
              d => d.Column.Mode == BocCustomColumnDefinitionMode.ControlsInAllRows
                   || d.Column.Mode == BocCustomColumnDefinitionMode.ControlInEditedRow)
          .ToArray();

       //TODO: Change to Lazy after upgrade to .NET 4.0
      var rows = new DoubleCheckedLockingContainer<SortedRow[]> (() => GetRowsForCurrentPage().ToArray());
      foreach (var customColumnData in controlEnabledCustomColumns)
      {
        var customColumn = customColumnData.Column;
        var placeHolder = new PlaceHolder();

        var customColumnTuples = new List<BocListCustomColumnTuple>();
        foreach (var row in rows.Value)
        {
          bool isEditedRow = _editModeController.IsRowEditModeActive && _editModeController.GetEditableRow (row.ValueRow.Index) != null;
          if (customColumn.Mode == BocCustomColumnDefinitionMode.ControlInEditedRow && !isEditedRow)
            continue;

          var args = new BocCustomCellArguments (this, customColumn);
          Control control = customColumn.CustomCell.CreateControlInternal (args);
          control.ID = ID + "_CustomColumnControl_" + customColumnData.Index + "_" + RowIDProvider.GetControlRowID (row.ValueRow);
          placeHolder.Controls.Add (control);
          customColumnTuples.Add (new BocListCustomColumnTuple (row.ValueRow.BusinessObject, row.ValueRow.Index, control));
        }
        _customColumnsPlaceHolder.Controls.Add (placeHolder);
        _customColumnControls[customColumn] = customColumnTuples.ToArray();
      }
    }

    /// <summary> Invokes the <see cref="BocCustomColumnDefinitionCell.Init"/> method for each custom column. </summary>
    private void InitCustomColumns ()
    {
      foreach (var keyValuePair in _customColumnControls.Where (p => p.Value.Any()))
      {
        var customColumn = keyValuePair.Key;
        var args = new BocCustomCellArguments (this, customColumn);
        customColumn.CustomCell.Init (args);
      }
    }

    /// <summary>
    ///   Invokes the <see cref="BocCustomColumnDefinitionCell.Load"/> method for each cell with a control in the custom columns. 
    /// </summary>
    private void LoadCustomColumns ()
    {
      foreach (var keyValuePair in _customColumnControls)
      {
        var customColumn = keyValuePair.Key;
        var customColumnTuples = keyValuePair.Value;
        foreach (var customColumnTuple in customColumnTuples)
        {
          int originalRowIndex = customColumnTuple.Item2;
          IBusinessObject businessObject = customColumnTuple.Item1;
          Control control = customColumnTuple.Item3;

          var args = new BocCustomCellLoadArguments (this, businessObject, customColumn, originalRowIndex, control);
          customColumn.CustomCell.Load (args);
        }
      }
    }

    private bool ValidateCustomColumns ()
    {
      bool isValid = true;

      foreach (var keyValuePair in _customColumnControls.Where (p => p.Key.Mode == BocCustomColumnDefinitionMode.ControlInEditedRow))
      {
        var customColumn = keyValuePair.Key;
        var customColumnTuples = keyValuePair.Value;
        foreach (var customColumnTuple in customColumnTuples)
        {
          IBusinessObject businessObject = customColumnTuple.Item1;
          Control control = customColumnTuple.Item3;
          var args = new BocCustomCellValidationArguments (this, businessObject, customColumn, control);
          customColumn.CustomCell.Validate (args);
          isValid &= args.IsValid;
        }
      }
      return isValid;
    }

    /// <summary> 
    /// Invokes the <see cref="BocCustomColumnDefinitionCell.PreRender"/> method for each custom column. 
    /// Used by postback-links which must be registered for synchronous postbacks.
    /// </summary>
    private void PreRenderCustomColumns ()
    {
      var columns = EnsureColumnsGot (true);
      foreach (var customColumn in columns.OfType<BocCustomColumnDefinition>())
      {
        var args = new BocCustomCellArguments (this, customColumn);
        customColumn.CustomCell.PreRender (args);
      }
    }
  }
}