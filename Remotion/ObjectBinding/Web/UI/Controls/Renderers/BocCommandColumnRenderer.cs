using System;
using System.Web.UI;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Renderers
{
  /// <summary>
  /// Responsible for rendering cells of <see cref="BocCommandColumnDefinition"/> columns.
  /// </summary>
  public class BocCommandColumnRenderer : BocCommandEnabledColumnRenderer<BocCommandColumnDefinition>
  {
    /// <summary>
    /// Contructs a renderer bound to a <see cref="BocList"/> to render, an <see cref="HtmlTextWriter"/> to render to, and a
    /// <see cref="BocCommandColumnDefinition"/> column for which to render cells.
    /// </summary>
    /// <remarks>
    /// This class should not be instantiated directly by clients. Instead, a <see cref="BocRowRenderer"/> should use a
    /// <see cref="BocListRendererFactory"/> to obtain instances of this class.
    /// </remarks>
    public BocCommandColumnRenderer (BocList list, HtmlTextWriter writer, BocCommandColumnDefinition column)
        : base(list, writer, column)
    {
    }

    /// <summary>
    /// Renders a command control with an icon, text, or both.
    /// </summary>
    /// <remarks>
    /// A <see cref="BocCommandColumnDefinition"/> can contain both an object icon and a command icon. The former is rendered according to
    /// <paramref name="showIcon"/>, the latter if the column defintion's <see cref="BocCommandColumnDefinition.Icon"/> property contains
    /// an URL. Furthermore, the command text in <see cref="BocCommandColumnDefinition.Text"/> is rendered after any icons.
    /// </remarks>
    protected override void RenderCellContents (
      BocListDataRowRenderEventArgs dataRowRenderEventArgs,
      int rowIndex, 
      bool isEditedRow, 
      bool showIcon)
    {
      int originalRowIndex = dataRowRenderEventArgs.ListIndex;
      IBusinessObject businessObject = dataRowRenderEventArgs.BusinessObject;

      EditableRow editableRow = GetEditableRow (isEditedRow, originalRowIndex);
      
      bool hasEditModeControl = editableRow != null && editableRow.HasEditControl (ColumnIndex);

      bool isCommandEnabled = RenderBeginTag(originalRowIndex, businessObject);

      RenderCellIcon(businessObject, hasEditModeControl, showIcon);
      RenderCellCommand();

      RenderEndTag (isCommandEnabled);
    }

    private void RenderCellCommand ()
    {
      if (Column.Icon.HasRenderingInformation)
        Column.Icon.Render (Writer);

      if (!StringUtility.IsNullOrEmpty (Column.Text))
        Writer.Write (Column.Text); // Do not HTML encode
    }

    private void RenderCellIcon (IBusinessObject businessObject, bool hasEditModeControl, bool showIcon)
    {
      if (!hasEditModeControl && showIcon)
      {
        RenderCellIcon (businessObject);
      }
    }

    private bool RenderBeginTag (int originalRowIndex, IBusinessObject businessObject)
    {
      bool isCommandEnabled = RenderBeginTagDataCellCommand (businessObject, originalRowIndex);
      if (!isCommandEnabled)
      {
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, List.CssClassContent);
        Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      }
      return isCommandEnabled;
    }

    protected void RenderEndTag (bool isCommandEnabled)
    {
      if (isCommandEnabled)
        RenderEndTagDataCellCommand ();
      else
        Writer.RenderEndTag ();
    }
  }
}
