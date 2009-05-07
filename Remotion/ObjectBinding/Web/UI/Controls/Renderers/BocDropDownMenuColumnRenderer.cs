using System;
using System.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.Renderers
{
  /// <summary>
  /// Responsible for rendering cells of <see cref="BocDropDownMenuColumnDefinition"/> columns.
  /// </summary>
  public class BocDropDownMenuColumnRenderer : BocColumnRenderer<BocDropDownMenuColumnDefinition>
  {
    /// <summary>
    /// Contructs a renderer bound to a <see cref="BocList"/> to render, an <see cref="HtmlTextWriter"/> to render to, and a
    /// <see cref="BocDropDownMenuColumnDefinition"/> column for which to render cells.
    /// </summary>
    /// <remarks>
    /// This class should not be instantiated directly by clients. Instead, a <see cref="BocRowRenderer"/> should use a
    /// <see cref="BocListRendererFactory"/> to obtain instances of this class.
    /// </remarks>
    public BocDropDownMenuColumnRenderer (BocList list, HtmlTextWriter writer, BocDropDownMenuColumnDefinition column)
        : base(list, writer, column)
    {
    }

    /// <summary>
    /// Renders a <see cref="DropDownMenu"/> with the options for the current row.
    /// <seealso cref="BocColumnRenderer{TBocColumnDefinition}.RenderCellContents"/>
    /// </summary>
    /// <remarks>
    /// The menu title is generated from the <see cref="DropDownMenu.TitleText"/> and <see cref="DropDownMenu.TitleText"/> properties of
    /// the column definition in <see cref="BocColumnRenderer{TBocColumnDefinition}.Column"/>, and populated with the menu items in
    /// the <see cref="BocList.RowMenus"/> property of <see cref="BocListBaseRenderer.List"/>.
    /// </remarks>
    protected override void RenderCellContents (
      BocListDataRowRenderEventArgs dataRowRenderEventArgs, 
      int rowIndex, 
      bool isEditedRow, 
      bool showIcon)
    {
      if (List.RowMenus == null || List.RowMenus.Length < rowIndex || List.RowMenus[rowIndex] == null)
      {
        Writer.Write (c_whiteSpace);
        return;
      }

      DropDownMenu dropDownMenu = (DropDownMenu) List.RowMenus[rowIndex].Third;
      if (dropDownMenu.MenuItems.Count == 0)
      {
        Writer.Write (c_whiteSpace);
        return;
      }

      if (List.HasClientScript)
        Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, c_onCommandClickScript);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div); // Begin div

      dropDownMenu.Enabled = !List.IsRowEditModeActive;

      dropDownMenu.TitleText = Column.MenuTitleText;
      dropDownMenu.TitleIcon = Column.MenuTitleIcon;
      dropDownMenu.RenderControl (Writer);

      Writer.RenderEndTag (); // End div
    }
  }
}
