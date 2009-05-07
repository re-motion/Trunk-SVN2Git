using System;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.Renderers
{
  /// <summary>
  /// Responsible for rendering cells of <see cref="BocRowEditModeColumnDefinition"/> columns.
  /// </summary>
  public class BocRowEditModeColumnRenderer : BocColumnRenderer<BocRowEditModeColumnDefinition>
  {
    private const string c_eventRowEditModePrefix = "RowEditMode=";

    /// <summary>
    /// Contructs a renderer bound to a <see cref="BocList"/> to render, an <see cref="HtmlTextWriter"/> to render to, and a
    /// <see cref="BocRowEditModeColumnDefinition"/> column for which to render cells.
    /// </summary>
    /// <remarks>
    /// This class should not be instantiated directly by clients. Instead, a <see cref="BocRowRenderer"/> should use a
    /// <see cref="BocListRendererFactory"/> to obtain instances of this class.
    /// </remarks>
    public BocRowEditModeColumnRenderer (BocList list, HtmlTextWriter writer, BocRowEditModeColumnDefinition column)
        : base(list, writer, column)
    {
    }

    /// <summary>
    /// Renders the cell contents depending on the <paramref name="isEditedRow"/> argument and <paramref name="dataRowRenderEventArgs"/>'s
    /// <see cref="BocListDataRowRenderEventArgs.IsEditableRow"/> property.
    /// <seealso cref="BocColumnRenderer{TBocColumnDefinition}.RenderCellContents"/>
    /// </summary>
    /// <remarks>
    /// If the current row is being edited, "Save" and "Cancel" controls are rendered; if the row can be edited, an "Edit" control is rendered;
    /// if the row cannot be edited, an empty cell is rendered.
    /// Since the "Save", "Cancel" and "Edit" controls are structurally identical, their actual rendering is done by <see cref="RenderCommandControl"/>
    /// </remarks>
    protected override void RenderCellContents (
      BocListDataRowRenderEventArgs dataRowRenderEventArgs,
      int rowIndex,
      bool isEditedRow, 
      bool showIcon)
    {
      bool isEditableRow = dataRowRenderEventArgs.IsEditableRow;
      int originalRowIndex = dataRowRenderEventArgs.ListIndex;

      if (isEditedRow)
      {
        RenderEditedRowCellContents(originalRowIndex);
      }
      else
      {
        if (isEditableRow)
        {
          RenderEditableRowCellContents(originalRowIndex);
        }
        else
        {
          Writer.Write (c_whiteSpace);
        }
      }
    }

    private void RenderEditableRowCellContents (int originalRowIndex)
    {
      RenderCommandControl (
          originalRowIndex,
          BocList.RowEditModeCommand.Edit,
          BocList.ResourceIdentifier.RowEditModeEditAlternateText,
          Column.EditIcon,
          Column.EditText);
    }

    private void RenderEditedRowCellContents (int originalRowIndex)
    {
      RenderCommandControl (
          originalRowIndex,
          BocList.RowEditModeCommand.Save,
          BocList.ResourceIdentifier.RowEditModeSaveAlternateText,
          Column.SaveIcon, Column.SaveText);

      Writer.Write (" ");

      RenderCommandControl (
          originalRowIndex,
          BocList.RowEditModeCommand.Cancel,
          BocList.ResourceIdentifier.RowEditModeCancelAlternateText,
          Column.CancelIcon, Column.CancelText);
    }

    /// <summary>
    /// Renders a command control as link with an icon, a text or both.
    /// </summary>
    /// <param name="originalRowIndex">The zero-based index of the current row in <see cref="BocListBaseRenderer.List"/></param>
    /// <param name="command">The <see cref="BocList.RowEditModeCommand"/> that is issued when the control is clicked.</param>
    /// <param name="alternateText">The <see cref="BocList.ResourceIdentifier"/> specifying which resource to load as alternate text to the icon.</param>
    /// <param name="icon">The icon to render; must not be <see langword="null"/>. To skip the icon, set <see cref="IconInfo.Url"/> to null.</param>
    /// <param name="text">The text to render after the icon. May be <see langword="null"/>, in which case no text is rendered.</param>
    protected virtual void RenderCommandControl (
      int originalRowIndex, 
      BocList.RowEditModeCommand command, 
      BocList.ResourceIdentifier alternateText, 
      IconInfo icon, 
      string text)
    {
      ArgumentUtility.CheckNotNull ("icon", icon);

      if (!List.IsReadOnly && List.HasClientScript)
      {
        string argument = c_eventRowEditModePrefix + originalRowIndex + "," + command;
        string postBackEvent = List.Page.ClientScript.GetPostBackEventReference (List, argument) + ";";
        Writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
        Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, postBackEvent + c_onCommandClickScript);
      }
      Writer.RenderBeginTag (HtmlTextWriterTag.A);

      bool hasIcon = icon.HasRenderingInformation;
      bool hasText = !StringUtility.IsNullOrEmpty (text);

      if (hasIcon && hasText)
      {
        RenderIcon (icon, null);
        Writer.Write (c_whiteSpace);
      }
      else if (hasIcon)
      {
        RenderIcon (icon, alternateText);
      }
      if (hasText)
        Writer.Write (text); // Do not HTML encode.

      Writer.RenderEndTag ();
    }
  }
}
