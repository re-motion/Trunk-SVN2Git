using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Renderers
{
  /// <summary>
  /// Abstract base class for all column renderers. Provides a template for rendering a table cell from an <see cref="IBusinessObject"/>
  /// and a <see cref="BocColumnDefinition"/>. 
  /// </summary>
  /// <typeparam name="TBocColumnDefinition">The column definition class which the deriving class can handle.</typeparam>
  public abstract class BocColumnRenderer<TBocColumnDefinition> : BocListBaseRenderer, IBocColumnRenderer
    where TBocColumnDefinition : BocColumnDefinition
  {
    /// <summary>Filename of the image used to indicate an ascending sort order of the column in its title cell.</summary>
    protected const string c_sortAscendingIcon = "SortAscending.gif";
    /// <summary>Filename of the image used to indicate an descending sort order of the column in its title cell.</summary>
    protected const string c_sortDescendingIcon = "SortDescending.gif";
    /// <summary> Prefix applied to the post back argument of the sort buttons. </summary>
    protected const string c_sortCommandPrefix = "Sort=";

    private const string c_designModeEmptyContents = "#";

    private readonly TBocColumnDefinition _column;
    private readonly int _columnIndex;

    /// <summary>
    /// Constructs the renderer, initializing the <see cref="BocListBaseRenderer.List"/>, <see cref="BocListBaseRenderer.Writer"/> and 
    /// <see cref="Column"/> properties.
    /// </summary>
    /// <param name="list">The <see cref="BocList"/> containing the data to be rendered.</param>
    /// <param name="writer">The <see cref="HtmlTextWriter"/> to render the cells to.</param>
    /// <param name="column">The <typeparamref name="TBocColumnDefinition"/> for which cells are rendered.</param>
    protected BocColumnRenderer (BocList list, HtmlTextWriter writer, TBocColumnDefinition column) : base(list, writer)
    {
      _column = column;
      List<BocColumnDefinition> columnsInBocList = new List<BocColumnDefinition> (List.EnsureColumnsGot ());
      _columnIndex = columnsInBocList.IndexOf (column);
    }

    BocColumnDefinition IBocColumnRenderer.Column
    {
      get { return Column; }
    }

    HtmlTextWriter IBocColumnRenderer.Writer
    {
      get { return Writer; }
    }

    BocList IBocColumnRenderer.List
    {
      get { return List; }
    }

    /// <see cref="IBocColumnRenderer.Column"/>
    public TBocColumnDefinition Column
    {
      get { return _column; }
    }

    /// <see cref="IBocColumnRenderer.ColumnIndex"/>
    public int ColumnIndex
    {
      get { return _columnIndex; }
    }

    /// <see cref="IBocColumnRenderer.RenderTitleCell"/>
    public void RenderTitleCell (SortingDirection sortingDirection, int orderIndex)
    {
      string cssClassTitleCell = List.CssClassTitleCell;
      if (!StringUtility.IsNullOrEmpty (Column.CssClass))
        cssClassTitleCell += " " + Column.CssClass;
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClassTitleCell);
      Writer.RenderBeginTag (HtmlTextWriterTag.Th);

      RenderTitleCellMarkers ();
      RenderBeginTagTitleCellSortCommand ();
      RenderTitleCellText ();
      if (List.IsClientSideSortingEnabled || List.HasSortingKeys)
        RenderTitleCellSortingButton (sortingDirection, orderIndex);
      RenderEndTagTitleCellSortCommand ();

      Writer.RenderEndTag ();
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
        if (!List.IsRowEditModeActive && !List.IsListEditModeActive && List.HasClientScript)
        {
          string argument = c_sortCommandPrefix + ColumnIndex;
          string postBackEvent = List.Page.ClientScript.GetPostBackEventReference (List, argument);
          postBackEvent += "; return false;";
          Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, postBackEvent);
          Writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
          Writer.RenderBeginTag (HtmlTextWriterTag.A);
        }
        else
        {
          Writer.RenderBeginTag (HtmlTextWriterTag.Span);
        }
      }
      else
      {
        Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      }
    }

    private void RenderTitleCellText ()
    {
      if (ControlHelper.IsDesignMode ((Control) List) && Column.ColumnTitleDisplayValue.Length == 0)
      {
        Writer.Write (c_designModeEmptyContents);
      }
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
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, List.CssClassSortingOrder);
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      BocList.ResourceIdentifier? alternateTextID;
      if (sortingDirection == SortingDirection.Ascending)
        alternateTextID = BocList.ResourceIdentifier.SortAscendingAlternateText;
      else
        alternateTextID = BocList.ResourceIdentifier.SortDescendingAlternateText;
      RenderIcon (new IconInfo (imageUrl), alternateTextID);

      if (List.IsShowSortingOrderEnabled && orderIndex >= 0)
      {
        Writer.Write (c_whiteSpace + (orderIndex + 1));
      }
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
              List, HttpContext.Current, typeof (BocList), ResourceType.Image, c_sortAscendingIcon);
          break;
        }
        case SortingDirection.Descending:
        {
          imageUrl = ResourceUrlResolver.GetResourceUrl (
              List, HttpContext.Current, typeof (BocList), ResourceType.Image, c_sortDescendingIcon);
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
      Writer.RenderEndTag ();
    }

    /// <summary>
    /// Renders a table cell for <see cref="Column"/> containing the appropriate data from the <see cref="IBusinessObject"/> contained in
    /// <paramref name="dataRowRenderEventArgs"/>
    /// </summary>
    /// <param name="rowIndex">The zero-based index of the row on the page to be displayed.</param>
    /// <param name="showIcon">Specifies if an object-specific icon will be rendered in the table cell.</param>
    /// <param name="cssClassTableCell">Specifies the CSS class to be applied to the table cell.</param>
    /// <param name="dataRowRenderEventArgs">Specifies row-specific arguments used in rendering the table cell.</param>
    /// <remarks>
    /// This is a template method. Deriving classes must implement <see cref="RenderCellContents"/> to provide the contents of
    /// the table cell (&lt;td&gt;) element.
    /// </remarks>
    public void RenderDataCell (
      int rowIndex,
      bool showIcon, 
      string cssClassTableCell,
      BocListDataRowRenderEventArgs dataRowRenderEventArgs)
    {
      if (!List.IsColumnVisible (Column))
        return;

      if (!StringUtility.IsNullOrEmpty (Column.CssClass))
        cssClassTableCell += " " + Column.CssClass;
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClassTableCell);
      Writer.RenderBeginTag (HtmlTextWriterTag.Td);

      int originalRowIndex = dataRowRenderEventArgs.ListIndex;
      bool isEditedRow = List.IsRowEditModeActive && List.EditableRowIndex.Value == originalRowIndex;
      RenderCellContents (dataRowRenderEventArgs, rowIndex, isEditedRow, showIcon);
      
      Writer.RenderEndTag ();
    }

    /// <summary>
    /// Renders the contents of the table cell. It is called by <see cref="RenderDataCell"/> and should not be called by other clients.
    /// </summary>
    /// <param name="dataRowRenderEventArgs">The row-specific rendering arguments.</param>
    /// <param name="rowIndex">The zero-based index of the row to render in <see cref="BocListBaseRenderer.List"/>.</param>
    /// <param name="isEditedRow">Specifies if the row to render is currently being edited.</param>
    /// <param name="showIcon">Specifies if the cell should contain an icon of the current <see cref="IBusinessObject"/>.</param>
    protected abstract void RenderCellContents (
      BocListDataRowRenderEventArgs dataRowRenderEventArgs,
      int rowIndex, bool isEditedRow,
      bool showIcon);
    
  }
}
