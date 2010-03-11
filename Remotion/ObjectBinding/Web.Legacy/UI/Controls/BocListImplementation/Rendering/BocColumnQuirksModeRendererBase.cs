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
using System.Web.UI;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using System.Web;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Abstract base class for all column renderers. Provides a template for rendering a table cell from an <see cref="IBusinessObject"/>
  /// and a <see cref="BocColumnDefinition"/>. 
  /// </summary>
  /// <typeparam name="TBocColumnDefinition">The column definition class which the deriving class can handle.</typeparam>
  public abstract class BocColumnQuirksModeRendererBase<TBocColumnDefinition> : IBocColumnRenderer
      where TBocColumnDefinition: BocColumnDefinition
  {
    /// <summary>Filename of the image used to indicate an ascending sort order of the column in its title cell.</summary>
    protected const string c_sortAscendingIcon = "SortAscending.gif";

    /// <summary>Filename of the image used to indicate an descending sort order of the column in its title cell.</summary>
    protected const string c_sortDescendingIcon = "SortDescending.gif";

    private const string c_designModeEmptyContents = "#";

    /// <summary>Entity definition for whitespace separating controls, e.g. icons from following text</summary>
    protected const string c_whiteSpace = "&nbsp;";

    /// <summary>Name of the JavaScript function to call when a command control has been clicked.</summary>
    protected const string c_onCommandClickScript = "BocList_OnCommandClick();";

    private readonly TBocColumnDefinition _column;
    private readonly CssClassContainer _cssClasses;
    private readonly int _columnIndex;
    private readonly HttpContextBase _context;
    private readonly IBocList _list;

    /// <summary>
    /// Constructs the renderer, initializing the List, Writer and <see cref="Column"/> properties.
    /// </summary>
    /// <param name="list">The <see cref="BocList"/> containing the data to be rendered.</param>
    /// <param name="context">The <see cref="HttpContextBase"/> that contains the response for which to render the list.</param>
    /// <param name="columnDefinition">The <typeparamref name="TBocColumnDefinition"/> for which cells are rendered.</param>
    /// <param name="cssClasses">The <see cref="CssClassContainer"/> containing the CSS classes to apply to the rendered elements.</param>
    protected BocColumnQuirksModeRendererBase (HttpContextBase context, IBocList list, TBocColumnDefinition columnDefinition, CssClassContainer cssClasses)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("columnDefinition", columnDefinition);
      ArgumentUtility.CheckNotNull ("cssClasses", cssClasses);

      _context = context;
      _list = list;
      _column = columnDefinition;
      _columnIndex = Array.IndexOf (list.GetColumns(), columnDefinition);
      _cssClasses = cssClasses;
    }

    public HttpContextBase Context
    {
      get { return _context; }
    }

    public IBocList List
    {
      get { return _list; }
    }

    public TBocColumnDefinition Column
    {
      get { return _column; }
    }

    public int ColumnIndex
    {
      get { return _columnIndex; }
    }

    public CssClassContainer CssClasses
    {
      get { return _cssClasses; }
    }

    protected ResourceTheme ResourceTheme
    {
      get { return SafeServiceLocator.Current.GetInstance<ResourceTheme> (); }
    }

    public virtual void RenderTitleCell (HtmlTextWriter writer, SortingDirection sortingDirection, int orderIndex)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      string cssClassTitleCell = CssClasses.TitleCell;
      if (!StringUtility.IsNullOrEmpty (Column.CssClass))
        cssClassTitleCell += " " + Column.CssClass;
      writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClassTitleCell);
      writer.RenderBeginTag (HtmlTextWriterTag.Th);

      RenderTitleCellMarkers (writer);
      RenderBeginTagTitleCellSortCommand (writer);
      RenderTitleCellText (writer);
      if (List.IsClientSideSortingEnabled || List.HasSortingKeys)
        RenderTitleCellSortingButton (writer, sortingDirection, orderIndex);
      RenderEndTagTitleCellSortCommand (writer);

      writer.RenderEndTag();
    }

    private void RenderTitleCellMarkers (HtmlTextWriter writer)
    {
      List.EditModeController.RenderTitleCellMarkers (writer, Column, ColumnIndex);
    }

    private void RenderBeginTagTitleCellSortCommand (HtmlTextWriter writer)
    {
      bool hasSortingCommand = List.IsClientSideSortingEnabled
                               && (Column is IBocSortableColumnDefinition && ((IBocSortableColumnDefinition) Column).IsSortable);

      if (hasSortingCommand)
      {
        if (!List.EditModeController.IsRowEditModeActive && !List.EditModeController.IsListEditModeActive && List.HasClientScript)
        {
          string argument = BocList.SortCommandPrefix + ColumnIndex;
          string postBackEvent = List.Page.ClientScript.GetPostBackEventReference (List, argument);
          postBackEvent += "; return false;";
          writer.AddAttribute (HtmlTextWriterAttribute.Onclick, postBackEvent);
          writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
          writer.RenderBeginTag (HtmlTextWriterTag.A);
        }
        else
          writer.RenderBeginTag (HtmlTextWriterTag.Span);
      }
      else
        writer.RenderBeginTag (HtmlTextWriterTag.Span);
    }

    private void RenderTitleCellText (HtmlTextWriter writer)
    {
      if (List.IsDesignMode && Column.ColumnTitleDisplayValue.Length == 0)
        writer.Write (c_designModeEmptyContents);
      else
      {
        string contents = HttpUtility.HtmlEncode (Column.ColumnTitleDisplayValue);
        if (StringUtility.IsNullOrEmpty (contents))
          contents = c_whiteSpace;
        writer.Write (contents);
      }
    }

    private void RenderTitleCellSortingButton (HtmlTextWriter writer, SortingDirection sortingDirection, int orderIndex)
    {
      string imageUrl = GetImageUrl (sortingDirection);

      if (sortingDirection == SortingDirection.None)
        return;

      //  WhiteSpace before icon
      writer.Write (c_whiteSpace);
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.SortingOrder);
      writer.RenderBeginTag (HtmlTextWriterTag.Span);

      Web.UI.Controls.BocList.ResourceIdentifier alternateTextID;
      if (sortingDirection == SortingDirection.Ascending)
        alternateTextID = BocList.ResourceIdentifier.SortAscendingAlternateText;
      else
        alternateTextID = BocList.ResourceIdentifier.SortDescendingAlternateText;

      var icon = new IconInfo (imageUrl);
      icon.AlternateText = List.GetResourceManager().GetString (alternateTextID);
      icon.Render (writer);

      if (List.IsShowSortingOrderEnabled && orderIndex >= 0)
        writer.Write (c_whiteSpace + (orderIndex + 1));
      writer.RenderEndTag();
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
              List, Context, typeof (BocColumnQuirksModeRendererBase<>), ResourceType.Image, c_sortAscendingIcon);
          break;
        }
        case SortingDirection.Descending:
        {
          imageUrl = ResourceUrlResolver.GetResourceUrl (
              List, Context, typeof (BocColumnQuirksModeRendererBase<>), ResourceType.Image, c_sortDescendingIcon);
          break;
        }
        case SortingDirection.None:
        {
          break;
        }
      }
      return imageUrl;
    }

    private void RenderEndTagTitleCellSortCommand (HtmlTextWriter writer)
    {
      writer.RenderEndTag();
    }

    /// <summary>
    /// Renders a table cell for <see cref="Column"/> containing the appropriate data from the <see cref="IBusinessObject"/> contained in
    /// <paramref name="dataRowRenderEventArgs"/>
    /// </summary>
    /// <param name="writer">The <see cref="HtmlTextWriter"/>.</param>
    /// <param name="rowIndex">The zero-based index of the row on the page to be displayed.</param>
    /// <param name="showIcon">Specifies if an object-specific icon will be rendered in the table cell.</param>
    /// <param name="dataRowRenderEventArgs">Specifies row-specific arguments used in rendering the table cell.</param>
    /// <remarks>
    /// This is a template method. Deriving classes must implement <see cref="RenderCellContents"/> to provide the contents of
    /// the table cell (&lt;td&gt;) element.
    /// </remarks>
    public virtual void RenderDataCell (
        HtmlTextWriter writer, 
        int rowIndex,
        bool showIcon,
        BocListDataRowRenderEventArgs dataRowRenderEventArgs)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("dataRowRenderEventArgs", dataRowRenderEventArgs);

      if (!List.IsColumnVisible (Column))
        return;

      string cssClassTableCell = CssClasses.GetDataCell (dataRowRenderEventArgs.IsOddRow);

      if (!StringUtility.IsNullOrEmpty (Column.CssClass))
        cssClassTableCell += " " + Column.CssClass;
      writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClassTableCell);
      writer.RenderBeginTag (HtmlTextWriterTag.Td);

      RenderCellContents (writer, dataRowRenderEventArgs, rowIndex, showIcon);

      writer.RenderEndTag();
    }

    /// <summary>
    /// Renders the contents of the table cell. It is called by <see cref="RenderDataCell"/> and should not be called by other clients.
    /// </summary>
    /// <param name="writer">The <see cref="HtmlTextWriter"/>.</param>
    /// <param name="dataRowRenderEventArgs">The row-specific rendering arguments.</param>
    /// <param name="rowIndex">The zero-based index of the row to render in <see cref="Web.UI.Controls.BocListImplementation.Rendering.BocListRenderer.List"/>.</param>
    /// <param name="showIcon">Specifies if the cell should contain an icon of the current <see cref="IBusinessObject"/>.</param>
    protected abstract void RenderCellContents (HtmlTextWriter writer, BocListDataRowRenderEventArgs dataRowRenderEventArgs, int rowIndex, bool showIcon);
  }
}