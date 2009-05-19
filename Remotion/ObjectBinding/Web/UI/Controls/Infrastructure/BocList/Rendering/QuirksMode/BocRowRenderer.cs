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
using Microsoft.Practices.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode
{
  /// <summary>
  /// Responsible for rendering single data rows or the title row of a specific <see cref="BocList"/>.
  /// </summary>
  /// <remarks>This class should not be instantiated directly. It is meant to be used by a <see cref="BocListRenderer"/>.</remarks>
  public class BocRowRenderer : BocListRendererBase, IBocRowRenderer
  {
    /// <summary>Text displayed when control is displayed in desinger and is read-only has no contents.</summary>
    private const string c_designModeDummyColumnTitle = "Column Title {0}";

    private readonly IServiceLocator _serviceLocator;

    public BocRowRenderer (IHttpContext context, HtmlTextWriter writer, IBocList list, IServiceLocator serviceLocator)
        : base (context, writer, list)
    {
      _serviceLocator = serviceLocator;
    }

    public IServiceLocator ServiceLocator
    {
      get { return _serviceLocator; }
    }

    /// <summary>Fetches a column renderer from the factory the first time it is called with a specific column argument,
    /// returns the cached renderer on subsequent calls with the same column.</summary>
    private IBocColumnRenderer GetColumnRenderer (BocColumnDefinition column)
    {
      return column.GetRenderer (ServiceLocator, Context, Writer, List);
    }

    /// <summary> Renders the table row containing the column titles and sorting buttons. </summary>
    /// <remarks> Title format: &lt;span&gt;label button &lt;span&gt;sort order&lt;/span&gt;&lt;/span&gt; </remarks>
    public void RenderTitlesRow ()
    {
      Writer.RenderBeginTag (HtmlTextWriterTag.Tr);

      GetIndexColumnRenderer().RenderTitleCell();
      GetSelectorColumnRenderer().RenderTitleCell();

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

      if (ControlHelper.IsDesignMode (List) && List.GetColumns().Length == 0)
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
      BocColumnDefinition[] renderColumns = List.GetColumns();
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

    /// <summary>
    /// Renders a row containing the empty list message in <see cref="BocListRendererBase.List"/>'s 
    /// <see cref="Remotion.ObjectBinding.Web.UI.Controls.BocList.EmptyListMessage"/> protperty.
    /// </summary>
    /// <remarks>
    /// If the property is not set, a default message will be loaded from the resource file, using 
    /// <see cref="Remotion.ObjectBinding.Web.UI.Controls.BocList.ResourceIdentifier.EmptyListMessage"/> as key. 
    /// </remarks>
    public void RenderEmptyListDataRow ()
    {
      BocColumnDefinition[] renderColumns = List.GetColumns();
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
    public void RenderDataRow (
        IBusinessObject businessObject,
        int rowIndex,
        int absoluteRowIndex,
        int originalRowIndex)
    {
      string selectorControlID = List.ClientID + c_dataRowSelectorControlIDSuffix + rowIndex;
      bool isChecked = (List.SelectorControlCheckedState[originalRowIndex] != null);
      bool isOddRow = (rowIndex % 2 == 1);

      string cssClassTableRow = GetCssClassTableRow (isChecked);
      string cssClassTableCell = GetCssClassTableCell (isOddRow);

      AddRowOnClickScript (selectorControlID);
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClassTableRow);
      Writer.RenderBeginTag (HtmlTextWriterTag.Tr);

      GetIndexColumnRenderer().RenderDataCell (originalRowIndex, selectorControlID, absoluteRowIndex, cssClassTableCell);
      GetSelectorColumnRenderer().RenderDataCell (originalRowIndex, selectorControlID, isChecked, cssClassTableCell);

      var dataRowRenderEventArgs = new BocListDataRowRenderEventArgs (originalRowIndex, businessObject);
      List.OnDataRowRendering (dataRowRenderEventArgs);

      RenderDataCells (rowIndex, dataRowRenderEventArgs);

      Writer.RenderEndTag();
    }

    private IBocSelectorColumnRenderer GetSelectorColumnRenderer ()
    {
      return ServiceLocator.GetInstance<IBocSelectorColumnRendererFactory>().CreateRenderer (Context, Writer, List);
    }

    private IBocIndexColumnRenderer GetIndexColumnRenderer ()
    {
      return ServiceLocator.GetInstance<IBocIndexColumnRendererFactory>().CreateRenderer (Context, Writer, List);
    }

    private void RenderDataCells (int rowIndex, BocListDataRowRenderEventArgs dataRowRenderEventArgs)
    {
      bool firstValueColumnRendered = false;
      foreach (BocColumnDefinition column in List.GetColumns())
      {
        bool showIcon = false;
        if ((!firstValueColumnRendered) && column is BocValueColumnDefinition)
        {
          firstValueColumnRendered = true;
          showIcon = List.EnableIcon;
        }

        IBocColumnRenderer columnRenderer = GetColumnRenderer (column);

        columnRenderer.RenderDataCell (rowIndex, showIcon, dataRowRenderEventArgs);
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

    private string GetCssClassTableRow (bool isChecked)
    {
      string cssClassTableRow;
      if (isChecked && List.AreDataRowsClickSensitive())
        cssClassTableRow = List.CssClassDataRowSelected;
      else
        cssClassTableRow = List.CssClassDataRow;
      return cssClassTableRow;
    }
  }
}