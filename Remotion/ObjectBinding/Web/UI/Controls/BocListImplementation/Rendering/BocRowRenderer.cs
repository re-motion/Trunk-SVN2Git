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
using System.Collections.Generic;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering single data rows or the title row of a specific <see cref="BocList"/>.
  /// </summary>
  /// <remarks>This class should not be instantiated directly. It is meant to be used by a <see cref="BocListRenderer"/>.</remarks>
  public class BocRowRenderer : IBocRowRenderer
  {
    /// <summary>Text displayed when control is displayed in desinger and is read-only has no contents.</summary>
    public const string DesignModeDummyColumnTitle = "Column Title {0}";

    /// <summary>Number of columns to show in design mode before actual columns have been defined.</summary>
    public const int DesignModeDummyColumnCount = 3;

    private readonly BocListCssClassDefinition _cssClasses;
    private readonly IBocIndexColumnRenderer _indexColumnRenderer;
    private readonly IBocSelectorColumnRenderer _selectorColumnRenderer;
    
    public BocRowRenderer (BocListCssClassDefinition cssClasses, IBocIndexColumnRenderer indexColumnRenderer, IBocSelectorColumnRenderer selectorColumnRenderer)
    {
      ArgumentUtility.CheckNotNull ("cssClasses", cssClasses);
      
      _cssClasses = cssClasses;
      _indexColumnRenderer = indexColumnRenderer;
      _selectorColumnRenderer = selectorColumnRenderer;
    }

    public BocListCssClassDefinition CssClasses
    {
      get { return _cssClasses; }
    }

    public void RenderTitlesRow (BocListRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Tr);

      GetIndexColumnRenderer ().RenderTitleCell (renderingContext);
      GetSelectorColumnRenderer ().RenderTitleCell (renderingContext);

      var sortingDirections = new Dictionary<int, SortingDirection>();
      var sortingOrder = new List<int>();
      if (renderingContext.Control.IsClientSideSortingEnabled || renderingContext.Control.HasSortingKeys)
      {
        for (int i = 0; i < renderingContext.Control.SortingOrder.Count; i++)
        {
          var currentEntry = (BocListSortingOrderEntry) renderingContext.Control.SortingOrder[i];
          sortingDirections[currentEntry.ColumnIndex] = currentEntry.Direction;
          if (currentEntry.Direction != SortingDirection.None)
            sortingOrder.Add (currentEntry.ColumnIndex);
        }
      }

      RenderTitleCells (renderingContext, sortingDirections, sortingOrder);

      if (ControlHelper.IsDesignMode (renderingContext.Control) && renderingContext.ColumnRenderers.Length == 0)
      {
        for (int i = 0; i < DesignModeDummyColumnCount; i++)
        {
          renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td);
          renderingContext.Writer.Write (String.Format (DesignModeDummyColumnTitle, i + 1));
          renderingContext.Writer.RenderEndTag ();
        }
      }

      renderingContext.Writer.RenderEndTag ();
    }

    private void RenderTitleCells (BocListRenderingContext renderingContext, IDictionary<int, SortingDirection> sortingDirections, IList<int> sortingOrder)
    {
      IBocColumnRenderer[] columnRenderers =renderingContext.ColumnRenderers;
      for (int idxColumns = 0; idxColumns < columnRenderers.Length; idxColumns++)
      {
        IBocColumnRenderer columnRenderer = columnRenderers[idxColumns];
        
        SortingDirection sortingDirection = SortingDirection.None;
        if (sortingDirections.ContainsKey (idxColumns))
          sortingDirection = sortingDirections[idxColumns];

        columnRenderer.RenderTitleCell (renderingContext.Writer, sortingDirection, sortingOrder.IndexOf (idxColumns));
      }
    }

    public void RenderEmptyListDataRow (BocListRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      IBocColumnRenderer[] columnRenderers = renderingContext.ColumnRenderers;
      int columnCount = 0;

      if (renderingContext.Control.IsIndexEnabled)
        columnCount++;

      if (renderingContext.Control.IsSelectionEnabled)
        columnCount++;

      for (int idxColumns = 0; idxColumns < columnRenderers.Length; idxColumns++)
      {
        IBocColumnRenderer columnRenderer = columnRenderers[idxColumns];
        if (renderingContext.Control.IsColumnVisible (columnRenderer.Column))
          columnCount++;
      }

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Colspan, columnCount.ToString());
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td);

      string emptyListMessage;
      if (StringUtility.IsNullOrEmpty (renderingContext.Control.EmptyListMessage))
        emptyListMessage = renderingContext.Control.GetResourceManager().GetString (Controls.BocList.ResourceIdentifier.EmptyListMessage);
      else
        emptyListMessage = renderingContext.Control.EmptyListMessage;
      // Do not HTML encode
      renderingContext.Writer.Write (emptyListMessage);

      renderingContext.Writer.RenderEndTag();
      renderingContext.Writer.RenderEndTag();
    }

    public void RenderDataRow (BocListRenderingContext renderingContext, IBusinessObject businessObject, int rowIndex, int absoluteRowIndex, 
      int originalRowIndex)
    {
      string selectorControlID = renderingContext.Control.GetSelectorControlClientId (rowIndex);
      bool isChecked = (renderingContext.Control.SelectorControlCheckedState.Contains (originalRowIndex));
      bool isOddRow = (rowIndex % 2 == 0); // row index is zero-based here, but one-based in rendering => invert even/odd

      string cssClassTableRow = GetCssClassTableRow (renderingContext, isChecked);
      string cssClassTableCell = CssClasses.GetDataCell (isOddRow);

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClassTableRow);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Tr);

      GetIndexColumnRenderer().RenderDataCell (renderingContext, originalRowIndex, selectorControlID, absoluteRowIndex, cssClassTableCell);
      GetSelectorColumnRenderer().RenderDataCell (renderingContext, originalRowIndex, selectorControlID, isChecked, cssClassTableCell);

      var dataRowRenderEventArgs = new BocListDataRowRenderEventArgs (originalRowIndex, businessObject) { IsOddRow = isOddRow };
      renderingContext.Control.OnDataRowRendering (dataRowRenderEventArgs);

      RenderDataCells (renderingContext, rowIndex, dataRowRenderEventArgs);

      renderingContext.Writer.RenderEndTag();
    }

    private IBocSelectorColumnRenderer GetSelectorColumnRenderer ()
    {
      return _selectorColumnRenderer;
    }

    private IBocIndexColumnRenderer GetIndexColumnRenderer ()
    {
      return _indexColumnRenderer;
    }

    private void RenderDataCells (BocListRenderingContext renderingContext, int rowIndex, BocListDataRowRenderEventArgs dataRowRenderEventArgs)
    {
      bool firstValueColumnRendered = false;
      foreach (IBocColumnRenderer columnRenderer in renderingContext.ColumnRenderers)
      {
        bool showIcon = false;
        if ((!firstValueColumnRendered) && columnRenderer.Column is BocValueColumnDefinition)
        {
          firstValueColumnRendered = true;
          showIcon = renderingContext.Control.EnableIcon;
        }
        
        columnRenderer.RenderDataCell (renderingContext.Writer, rowIndex, showIcon, dataRowRenderEventArgs);
      }
    }

    private string GetCssClassTableRow (BocListRenderingContext renderingContext, bool isChecked)
    {
      string cssClassTableRow;
      if (isChecked && renderingContext.Control.AreDataRowsClickSensitive ())
        cssClassTableRow = CssClasses.DataRowSelected;
      else
        cssClassTableRow = CssClasses.DataRow;
      return cssClassTableRow;
    }
  }
}