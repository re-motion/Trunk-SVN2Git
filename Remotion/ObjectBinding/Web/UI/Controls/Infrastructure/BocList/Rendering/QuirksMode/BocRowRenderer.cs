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
using System.Collections.Generic;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode
{
  /// <summary>
  /// Responsible for rendering single data rows or the title row of a specific <see cref="BocList"/>.
  /// </summary>
  /// <remarks>This class should not be instantiated directly. It is meant to be used by a <see cref="BocListRenderer"/>.</remarks>
  public class BocRowRenderer : BocListRendererBase
  {
    private const int c_titleRowIndex = -1;

    /// <summary>Text displayed when control is displayed in desinger and is read-only has no contents.</summary>
    private const string c_designModeDummyColumnTitle = "Column Title {0}";

    private readonly BocListRendererFactory _columnRendererFactory;

    public BocRowRenderer (Controls.BocList list, HtmlTextWriter writer, BocListRendererFactory columnRendererFactory)
        : base (list, writer)
    {
      _columnRendererFactory = columnRendererFactory;
    }

    public BocListRendererFactory ColumnRendererFactory
    {
      get { return _columnRendererFactory; }
    }

    /// <summary>Fetches a column renderer from the factory the first time it is called with a specific column argument,
    /// returns the cached renderer on subsequent calls with the same column.</summary>
    private IBocColumnRenderer GetColumnRenderer (BocColumnDefinition column)
    {
      return column.GetRenderer (List, Writer);
    }

    /// <summary> Renders the table row containing the column titles and sorting buttons. </summary>
    /// <remarks> Title format: &lt;span&gt;label button &lt;span&gt;sort order&lt;/span&gt;&lt;/span&gt; </remarks>
    public void RenderTitlesRow ()
    {
      Writer.RenderBeginTag (HtmlTextWriterTag.Tr);

      RenderIndexTitle();
      RenderSelectionTitle();

      var sortingDirections = new Dictionary<int, SortingDirection>();
      var sortingOrder = new List<int>();
      if (List.IsClientSideSortingEnabled || List.HasSortingKeys)
      {
        for (int i = 0; i < List.SortingOrder.Count; i++)
        {
          var currentEntry = (BocListSortingOrderEntry) List.SortingOrder[i];
          sortingDirections[currentEntry.ColumnIndex] = currentEntry.Direction;
          if (currentEntry.Direction != SortingDirection.None)
            sortingOrder.Add (currentEntry.ColumnIndex);
        }
      }

      RenderTitleCells (sortingDirections, sortingOrder);

      if (ControlHelper.IsDesignMode ((Control) List) && List.EnsureColumnsGot().Length == 0)
      {
        for (int i = 0; i < c_designModeDummyColumnCount; i++)
        {
          Writer.RenderBeginTag (HtmlTextWriterTag.Td);
          Writer.Write (string.Format (c_designModeDummyColumnTitle, i + 1));
          Writer.RenderEndTag();
        }
      }

      Writer.RenderEndTag();
    }

    private void RenderTitleCells (IDictionary<int, SortingDirection> sortingDirections, IList<int> sortingOrder)
    {
      BocColumnDefinition[] renderColumns = List.EnsureColumnsGot();
      for (int idxColumns = 0; idxColumns < renderColumns.Length; idxColumns++)
      {
        BocColumnDefinition column = renderColumns[idxColumns];
        if (!List.IsColumnVisible (column))
          continue;

        IBocColumnRenderer renderer = GetColumnRenderer (column);
        SortingDirection sortingDirection = SortingDirection.None;
        if (sortingDirections.ContainsKey (idxColumns))
          sortingDirection = sortingDirections[idxColumns];

        renderer.RenderTitleCell (sortingDirection, sortingOrder.IndexOf (idxColumns));
      }
    }

    private void RenderSelectionTitle ()
    {
      if (!List.IsSelectionEnabled)
        return;

      Writer.AddAttribute (HtmlTextWriterAttribute.Class, List.CssClassTitleCell);
      Writer.RenderBeginTag (HtmlTextWriterTag.Th);
      if (List.Selection == RowSelection.Multiple)
      {
        string selectorControlName = List.ID + c_titleRowSelectorControlIDSuffix;
        bool isChecked = (List.SelectorControlCheckedState[c_titleRowIndex] != null);
        RenderSelectorControl (selectorControlName, c_titleRowIndex.ToString(), isChecked, true);
      }
      else
        Writer.Write (c_whiteSpace);
      Writer.RenderEndTag();
    }

    private void RenderIndexTitle ()
    {
      if (!List.IsIndexEnabled)
        return;

      string cssClass = List.CssClassTitleCell + " " + List.CssClassTitleCellIndex;
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
      Writer.RenderBeginTag (HtmlTextWriterTag.Th);
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      string indexColumnTitle;
      if (StringUtility.IsNullOrEmpty (List.IndexColumnTitle))
        indexColumnTitle = List.GetResourceManager().GetString (Controls.BocList.ResourceIdentifier.IndexColumnTitle);
      else
        indexColumnTitle = List.IndexColumnTitle;
      // Do not HTML encode.
      Writer.Write (indexColumnTitle);
      Writer.RenderEndTag();
      Writer.RenderEndTag();
    }

    /// <summary>
    /// Renders a row containing the empty list message in <see cref="BocListRendererBase.List"/>'s <see cref="BocList.EmptyListMessage"/> protperty.
    /// </summary>
    /// <remarks>
    /// If the property is not set, a default message will be loaded from the resource file, using 
    /// <see cref="BocList.ResourceIdentifier.EmptyListMessage"/> as key. 
    /// </remarks>
    public void RenderEmptyListDataRow ()
    {
      BocColumnDefinition[] renderColumns = List.EnsureColumnsGot();
      int columnCount = 0;

      if (List.IsIndexEnabled)
        columnCount++;

      if (List.IsSelectionEnabled)
        columnCount++;

      for (int idxColumns = 0; idxColumns < renderColumns.Length; idxColumns++)
      {
        BocColumnDefinition column = renderColumns[idxColumns];
        if (List.IsColumnVisible (column))
          columnCount++;
      }

      Writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      Writer.AddAttribute (HtmlTextWriterAttribute.Colspan, columnCount.ToString());
      Writer.RenderBeginTag (HtmlTextWriterTag.Td);

      string emptyListMessage;
      if (StringUtility.IsNullOrEmpty (List.EmptyListMessage))
        emptyListMessage = List.GetResourceManager().GetString (Controls.BocList.ResourceIdentifier.EmptyListMessage);
      else
        emptyListMessage = List.EmptyListMessage;
      // Do not HTML encode
      Writer.Write (emptyListMessage);

      Writer.RenderEndTag();
      Writer.RenderEndTag();
    }

    /// <summary>Renders a table row containing the data of <paramref name="businessObject"/>. </summary>
    /// <param name="businessObject"> The <see cref="IBusinessObject"/> whose data will be rendered. </param>
    /// <param name="rowIndex"> The row number in the current view. </param>
    /// <param name="absoluteRowIndex"> The position of <paramref name="businessObject"/> in the list of values. </param>
    /// <param name="originalRowIndex"> The position of <paramref name="businessObject"/> in the list of values before sorting. </param>
    /// <param name="isOddRow"> Whether the data row is rendered in an odd or an even table row. </param>
    public void RenderDataRow (
        IBusinessObject businessObject,
        int rowIndex,
        int absoluteRowIndex,
        int originalRowIndex,
        bool isOddRow)
    {
      string selectorControlID = List.ClientID + c_dataRowSelectorControlIDSuffix + rowIndex;
      bool isChecked = (List.SelectorControlCheckedState[originalRowIndex] != null);

      string cssClassTableRow = GetCssClassTableRow (isChecked);
      string cssClassTableCell = GetCssClassTableCell (isOddRow);

      AddRowOnClickScript (selectorControlID);
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClassTableRow);
      Writer.RenderBeginTag (HtmlTextWriterTag.Tr);

      RenderIndexCell (originalRowIndex, selectorControlID, cssClassTableCell, absoluteRowIndex);
      RenderSelectorCell (originalRowIndex, selectorControlID, cssClassTableCell, isChecked);

      var dataRowRenderEventArgs = new BocListDataRowRenderEventArgs (originalRowIndex, businessObject);
      List.OnDataRowRendering (dataRowRenderEventArgs);

      RenderDataCells (rowIndex, cssClassTableCell, dataRowRenderEventArgs);

      Writer.RenderEndTag();
    }

    private void RenderDataCells (int rowIndex, string cssClassTableCell, BocListDataRowRenderEventArgs dataRowRenderEventArgs)
    {
      bool firstValueColumnRendered = false;
      foreach (BocColumnDefinition column in List.EnsureColumnsGot())
      {
        bool showIcon = false;
        if ((!firstValueColumnRendered) && column is BocValueColumnDefinition)
        {
          firstValueColumnRendered = true;
          showIcon = List.EnableIcon;
        }

        IBocColumnRenderer columnRenderer = GetColumnRenderer (column);

        columnRenderer.RenderDataCell (
            rowIndex,
            showIcon,
            cssClassTableCell,
            dataRowRenderEventArgs);
      }
    }

    private void RenderSelectorCell (int originalRowIndex, string selectorControlID, string cssClassTableCell, bool isChecked)
    {
      if (List.IsSelectionEnabled)
      {
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClassTableCell);
        Writer.RenderBeginTag (HtmlTextWriterTag.Td);
        RenderSelectorControl (selectorControlID, originalRowIndex.ToString(), isChecked, false);
        Writer.RenderEndTag();
      }
    }

    private void RenderIndexCell (int originalRowIndex, string selectorControlID, string cssClassTableCell, int absoluteRowIndex)
    {
      if (List.IsIndexEnabled)
      {
        string cssClass = cssClassTableCell + " " + List.CssClassDataCellIndex;
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
        Writer.RenderBeginTag (HtmlTextWriterTag.Td);
        if (List.Index == RowIndex.InitialOrder)
          RenderRowIndex (originalRowIndex, selectorControlID);
        else if (List.Index == RowIndex.SortedOrder)
          RenderRowIndex (absoluteRowIndex, selectorControlID);
        Writer.RenderEndTag();
      }
    }

    private void AddRowOnClickScript (string selectorControlID)
    {
      if (List.IsSelectionEnabled && ! List.IsRowEditModeActive)
      {
        if (List.AreDataRowsClickSensitive())
        {
          string script = "BocList_OnRowClick ("
                          + "document.getElementById ('" + List.ClientID + "'), "
                          + "this, "
                          + "document.getElementById ('" + selectorControlID + "'));";
          Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
        }
      }
    }

    private string GetCssClassTableCell (bool isOddRow)
    {
      string cssClassTableCell;
      if (isOddRow)
        cssClassTableCell = List.CssClassDataCellOdd;
      else
        cssClassTableCell = List.CssClassDataCellEven;
      return cssClassTableCell;
    }

    private string GetCssClassTableRow (bool isChecked)
    {
      string cssClassTableRow;
      if (isChecked && List.AreDataRowsClickSensitive())
        cssClassTableRow = List.CssClassDataRowSelected;
      else
        cssClassTableRow = List.CssClassDataRow;
      return cssClassTableRow;
    }


    /// <summary> Renders the zero-based row index normalized to a one-based format
    /// (Optionally as a label for the selector control). </summary>
    private void RenderRowIndex (int index, string selectorControlID)
    {
      bool hasSelectorControl = selectorControlID != null;
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, List.CssClassContent);
      if (hasSelectorControl)
      {
        Writer.AddAttribute (HtmlTextWriterAttribute.For, selectorControlID);
        if (List.HasClientScript)
        {
          const string script = "BocList_OnSelectorControlLabelClick();";
          Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
        }
        Writer.RenderBeginTag (HtmlTextWriterTag.Label);
      }
      else
        Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      int renderedIndex = index + 1;
      if (List.IndexOffset != null)
        renderedIndex += List.IndexOffset.Value;
      Writer.Write (renderedIndex);
      Writer.RenderEndTag();
    }
  }
}