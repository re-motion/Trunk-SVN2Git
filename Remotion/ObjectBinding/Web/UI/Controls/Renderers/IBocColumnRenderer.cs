using System;
using System.Web.UI;

namespace Remotion.ObjectBinding.Web.UI.Controls.Renderers
{
  /// <summary>
  /// Interface for classes able to render table cells from a column definition derived from <see cref="BocColumnDefinition"/>.
  /// </summary>
  public interface IBocColumnRenderer
  {
    /// <summary>
    /// Renders a table header cell for <see cref="Column"/> including title and sorting controls.
    /// </summary>
    /// <param name="sortingDirection">Specifies if rows are sorted by this column's data, and if so in which direction.</param>
    /// <param name="orderIndex">The zero-based index of the column in a virtual sorted list containing all columns by which data is sorted.</param>
    void RenderTitleCell (SortingDirection sortingDirection, int orderIndex);

    /// <summary>
    /// Renders a table cell for <see cref="Column"/> containing the appropriate data from the <see cref="IBusinessObject"/> contained in
    /// <paramref name="dataRowRenderEventArgs"/>
    /// </summary>
    /// <param name="rowIndex">The zero-based index of the row on the page to be displayed.</param>
    /// <param name="showIcon">Specifies if an object-specific icon will be rendered in the table cell.</param>
    /// <param name="cssClassTableCell">Specifies the CSS class to be applied to the table cell.</param>
    /// <param name="dataRowRenderEventArgs">Specifies row-specific arguments used in rendering the table cell.</param>
    void RenderDataCell (
        int rowIndex,
        bool showIcon,
        string cssClassTableCell,
        BocListDataRowRenderEventArgs dataRowRenderEventArgs);

    /// <summary>The <see cref="BocList"/> containing the data to render.</summary>
    BocList List { get; }

    /// <summary>The <see cref="HtmlTextWriter"/> that is used to render the table cells.</summary>
    HtmlTextWriter Writer { get; }

    /// <summary>The column definition from which table cells are rendered.</summary>
    BocColumnDefinition Column { get; }

    /// <summary>The zero-based index of <see cref="Column"/> in <see cref="List"/>.</summary>
    int ColumnIndex { get; }
  }
}
