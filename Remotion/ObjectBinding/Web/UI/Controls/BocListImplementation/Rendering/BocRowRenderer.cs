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
using Microsoft.Practices.ServiceLocation;
using Remotion.Collections;
using Remotion.Utilities;
using System.Web;
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

    private readonly HttpContextBase _context;
    private readonly IBocList _list;
    private readonly CssClassContainer _cssClasses;
    private readonly IServiceLocator _serviceLocator;
    private readonly IBocIndexColumnRenderer _indexColumnRenderer;
    private readonly IBocSelectorColumnRenderer _selectorColumnRenderer;
    private readonly ICache<BocColumnDefinition, IBocColumnRenderer> _columnRendererCache = new Cache<BocColumnDefinition, IBocColumnRenderer>();

    public BocRowRenderer (HttpContextBase context, IBocList list, CssClassContainer cssClasses, IServiceLocator serviceLocator)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("cssClasses", cssClasses);
      ArgumentUtility.CheckNotNull ("serviceLocator", serviceLocator);

      _context = context;
      _list = list;
      _cssClasses = cssClasses;
      _serviceLocator = serviceLocator;
      _indexColumnRenderer = ServiceLocator.GetInstance<IBocIndexColumnRendererFactory>().CreateRenderer (Context, List);
      _selectorColumnRenderer = ServiceLocator.GetInstance<IBocSelectorColumnRendererFactory>().CreateRenderer (Context, List);
    }

    public HttpContextBase Context
    {
      get { return _context; }
    }

    public IBocList List
    {
      get { return _list; }
    }

    public CssClassContainer CssClasses
    {
      get { return _cssClasses; }
    }

    public IServiceLocator ServiceLocator
    {
      get { return _serviceLocator; }
    }

    public void RenderTitlesRow (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      writer.RenderBeginTag (HtmlTextWriterTag.Tr);

      GetIndexColumnRenderer().RenderTitleCell (writer);
      GetSelectorColumnRenderer().RenderTitleCell (writer);

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

      RenderTitleCells (writer, sortingDirections, sortingOrder);

      if (ControlHelper.IsDesignMode (List) && List.GetColumns().Length == 0)
      {
        for (int i = 0; i < DesignModeDummyColumnCount; i++)
        {
          writer.RenderBeginTag (HtmlTextWriterTag.Td);
          writer.Write (String.Format (DesignModeDummyColumnTitle, i + 1));
          writer.RenderEndTag();
        }
      }

      writer.RenderEndTag();
    }

    private void RenderTitleCells (HtmlTextWriter writer, IDictionary<int, SortingDirection> sortingDirections, IList<int> sortingOrder)
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

        renderer.RenderTitleCell (writer, sortingDirection, sortingOrder.IndexOf (idxColumns));
      }
    }

    public void RenderEmptyListDataRow (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

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

      writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      writer.AddAttribute (HtmlTextWriterAttribute.Colspan, columnCount.ToString());
      writer.RenderBeginTag (HtmlTextWriterTag.Td);

      string emptyListMessage;
      if (StringUtility.IsNullOrEmpty (List.EmptyListMessage))
        emptyListMessage = List.GetResourceManager().GetString (Controls.BocList.ResourceIdentifier.EmptyListMessage);
      else
        emptyListMessage = List.EmptyListMessage;
      // Do not HTML encode
      writer.Write (emptyListMessage);

      writer.RenderEndTag();
      writer.RenderEndTag();
    }

    public void RenderDataRow (HtmlTextWriter writer,IBusinessObject businessObject,int rowIndex,int absoluteRowIndex,int originalRowIndex)
    {
      string selectorControlID = List.GetSelectorControlClientId (rowIndex);
      bool isChecked = (List.SelectorControlCheckedState.Contains (originalRowIndex));
      bool isOddRow = (rowIndex % 2 == 0); // row index is zero-based here, but one-based in rendering => invert even/odd

      string cssClassTableRow = GetCssClassTableRow (isChecked);
      string cssClassTableCell = CssClasses.GetDataCell (isOddRow);

      writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClassTableRow);
      writer.RenderBeginTag (HtmlTextWriterTag.Tr);

      GetIndexColumnRenderer().RenderDataCell (writer, originalRowIndex, selectorControlID, absoluteRowIndex, cssClassTableCell);
      GetSelectorColumnRenderer().RenderDataCell (writer, originalRowIndex, selectorControlID, isChecked, cssClassTableCell);

      var dataRowRenderEventArgs = new BocListDataRowRenderEventArgs (originalRowIndex, businessObject) { IsOddRow = isOddRow };
      List.OnDataRowRendering (dataRowRenderEventArgs);

      RenderDataCells (writer, rowIndex, dataRowRenderEventArgs);

      writer.RenderEndTag();
    }

    private IBocSelectorColumnRenderer GetSelectorColumnRenderer ()
    {
      return _selectorColumnRenderer;
    }

    private IBocIndexColumnRenderer GetIndexColumnRenderer ()
    {
      return _indexColumnRenderer;
    }

    /// <summary>Fetches a column renderer from the factory the first time it is called with a specific column argument,
    /// returns the cached renderer on subsequent calls with the same column.</summary>
    private IBocColumnRenderer GetColumnRenderer (BocColumnDefinition column)
    {
      return _columnRendererCache.GetOrCreateValue (column, key => key.GetRenderer (ServiceLocator, Context, List));
    }

    private void RenderDataCells (HtmlTextWriter writer, int rowIndex, BocListDataRowRenderEventArgs dataRowRenderEventArgs)
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

        columnRenderer.RenderDataCell (writer, rowIndex, showIcon, dataRowRenderEventArgs);
      }
    }

    private string GetCssClassTableRow (bool isChecked)
    {
      string cssClassTableRow;
      if (isChecked && List.AreDataRowsClickSensitive())
        cssClassTableRow = CssClasses.DataRowSelected;
      else
        cssClassTableRow = CssClasses.DataRow;
      return cssClassTableRow;
    }
  }
}