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
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.QuirksMode
{
  /// <summary>
  /// Abstract base class for all column renderers. Provides a template for rendering a table cell from an <see cref="IBusinessObject"/>
  /// and a <see cref="BocColumnDefinition"/>. 
  /// </summary>
  /// <typeparam name="TBocColumnDefinition">The column definition class which the deriving class can handle.</typeparam>
  public abstract class BocColumnRendererBase<TBocColumnDefinition> : BocListRendererBase, IBocColumnRenderer<TBocColumnDefinition>
      where TBocColumnDefinition: BocColumnDefinition
  {
    /// <summary>Filename of the image used to indicate an ascending sort order of the column in its title cell.</summary>
    protected const string c_sortAscendingIcon = "SortAscending.gif";

    /// <summary>Filename of the image used to indicate an descending sort order of the column in its title cell.</summary>
    protected const string c_sortDescendingIcon = "SortDescending.gif";

    private const string c_designModeEmptyContents = "#";

    private readonly TBocColumnDefinition _column;
    private readonly int _columnIndex;

    /// <summary>
    /// Constructs the renderer, initializing the List, Writer and <see cref="Column"/> properties.
    /// </summary>
    /// <param name="list">The <see cref="BocList"/> containing the data to be rendered.</param>
    /// <param name="context">The <see cref="IHttpContext"/> that contains the response for which to render the list.</param>
    /// <param name="writer">The <see cref="HtmlTextWriter"/> to render the cells to.</param>
    /// <param name="columnDefinition">The <typeparamref name="TBocColumnDefinition"/> for which cells are rendered.</param>
    /// <param name="cssClasses">The <see cref="CssClassContainer"/> containing the CSS classes to apply to the rendered elements.</param>
    protected BocColumnRendererBase (
        IHttpContext context, HtmlTextWriter writer, IBocList list, TBocColumnDefinition columnDefinition, CssClassContainer cssClasses)
        : base (context, writer, list, cssClasses)
    {
      ArgumentUtility.CheckNotNull ("columnDefinition", columnDefinition);

      _column = columnDefinition;
      _columnIndex = Array.IndexOf (List.GetColumns(), columnDefinition);
    }

    BocColumnDefinition IBocColumnRenderer.Column
    {
      get { return Column; }
    }

    public TBocColumnDefinition Column
    {
      get { return _column; }
    }

    public int ColumnIndex
    {
      get { return _columnIndex; }
    }

    public virtual void RenderTitleCell (SortingDirection sortingDirection, int orderIndex)
    {
      string cssClassTitleCell = CssClasses.TitleCell;
      if (!StringUtility.IsNullOrEmpty (Column.CssClass))
        cssClassTitleCell += " " + Column.CssClass;
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClassTitleCell);
      Writer.RenderBeginTag (HtmlTextWriterTag.Th);

      RenderTitleCellMarkers();
      RenderBeginTagTitleCellSortCommand();
      RenderTitleCellText();
      if (List.IsClientSideSortingEnabled || List.HasSortingKeys)
        RenderTitleCellSortingButton (sortingDirection, orderIndex);
      RenderEndTagTitleCellSortCommand();

      Writer.RenderEndTag();
    }

    private void RenderTitleCellMarkers ()
    {
      List.EditModeController.RenderTitleCellMarkers (Writer, Column, ColumnIndex);
    }

    private void RenderBeginTagTitleCellSortCommand ()
    {
      bool hasSortingCommand = List.IsClientSideSortingEnabled
                               && (Column is IBocSortableColumnDefinition && ((IBocSortableColumnDefinition) Column).IsSortable);

      if (hasSortingCommand)
      {
        if (!List.EditModeController.IsRowEditModeActive && !List.EditModeController.IsListEditModeActive && List.HasClientScript)
        {
          string argument = Controls.BocList.SortCommandPrefix + ColumnIndex;
          string postBackEvent = List.Page.ClientScript.GetPostBackEventReference (List, argument);
          postBackEvent += "; return false;";
          Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, postBackEvent);
          Writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
          Writer.RenderBeginTag (HtmlTextWriterTag.A);
        }
        else
          Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      }
      else
        Writer.RenderBeginTag (HtmlTextWriterTag.Span);
    }

    private void RenderTitleCellText ()
    {
      if (List.IsDesignMode && Column.ColumnTitleDisplayValue.Length == 0)
        Writer.Write (c_designModeEmptyContents);
      else
      {
        string contents = HtmlUtility.HtmlEncode (Column.ColumnTitleDisplayValue);
        if (StringUtility.IsNullOrEmpty (contents))
          contents = c_whiteSpace;
        Writer.Write (contents);
      }
    }

    private void RenderTitleCellSortingButton (SortingDirection sortingDirection, int orderIndex)
    {
      string imageUrl = GetImageUrl (sortingDirection);

      if (sortingDirection == SortingDirection.None)
        return;

      //  WhiteSpace before icon
      Writer.Write (c_whiteSpace);
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.SortingOrder);
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      Controls.BocList.ResourceIdentifier? alternateTextID;
      if (sortingDirection == SortingDirection.Ascending)
        alternateTextID = Controls.BocList.ResourceIdentifier.SortAscendingAlternateText;
      else
        alternateTextID = Controls.BocList.ResourceIdentifier.SortDescendingAlternateText;
      RenderIcon (new IconInfo (imageUrl), alternateTextID);

      if (List.IsShowSortingOrderEnabled && orderIndex >= 0)
        Writer.Write (c_whiteSpace + (orderIndex + 1));
      Writer.RenderEndTag();
    }

    private string GetImageUrl (SortingDirection sortingDirection)
    {
      string imageUrl = string.Empty;
      //  Button Asc -> Button Desc -> No Button
      switch (sortingDirection)
      {
        case SortingDirection.Ascending:
        {
          imageUrl = ResourceUrlResolver.GetResourceUrl (
              List, Context, typeof (Controls.BocList), ResourceType.Image, c_sortAscendingIcon);
          break;
        }
        case SortingDirection.Descending:
        {
          imageUrl = ResourceUrlResolver.GetResourceUrl (
              List, Context, typeof (Controls.BocList), ResourceType.Image, c_sortDescendingIcon);
          break;
        }
        case SortingDirection.None:
        {
          break;
        }
      }
      return imageUrl;
    }

    private void RenderEndTagTitleCellSortCommand ()
    {
      Writer.RenderEndTag();
    }

    /// <summary>
    /// Renders a table cell for <see cref="Column"/> containing the appropriate data from the <see cref="IBusinessObject"/> contained in
    /// <paramref name="dataRowRenderEventArgs"/>
    /// </summary>
    /// <param name="rowIndex">The zero-based index of the row on the page to be displayed.</param>
    /// <param name="showIcon">Specifies if an object-specific icon will be rendered in the table cell.</param>
    /// <param name="dataRowRenderEventArgs">Specifies row-specific arguments used in rendering the table cell.</param>
    /// <remarks>
    /// This is a template method. Deriving classes must implement <see cref="RenderCellContents"/> to provide the contents of
    /// the table cell (&lt;td&gt;) element.
    /// </remarks>
    public virtual void RenderDataCell (
        int rowIndex,
        bool showIcon,
        BocListDataRowRenderEventArgs dataRowRenderEventArgs)
    {
      ArgumentUtility.CheckNotNull ("dataRowRenderEventArgs", dataRowRenderEventArgs);

      if (!List.IsColumnVisible (Column))
        return;

      string cssClassTableCell = GetCssClassTableCell (dataRowRenderEventArgs.IsOddRow);

      if (!StringUtility.IsNullOrEmpty (Column.CssClass))
        cssClassTableCell += " " + Column.CssClass;
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClassTableCell);
      Writer.RenderBeginTag (HtmlTextWriterTag.Td);

      RenderCellContents (dataRowRenderEventArgs, rowIndex, showIcon);

      Writer.RenderEndTag();
    }

    /// <summary>
    /// Renders the contents of the table cell. It is called by <see cref="RenderDataCell"/> and should not be called by other clients.
    /// </summary>
    /// <param name="dataRowRenderEventArgs">The row-specific rendering arguments.</param>
    /// <param name="rowIndex">The zero-based index of the row to render in <see cref="BocListRendererBase.List"/>.</param>
    /// <param name="showIcon">Specifies if the cell should contain an icon of the current <see cref="IBusinessObject"/>.</param>
    protected abstract void RenderCellContents (BocListDataRowRenderEventArgs dataRowRenderEventArgs, int rowIndex, bool showIcon);
  }
}