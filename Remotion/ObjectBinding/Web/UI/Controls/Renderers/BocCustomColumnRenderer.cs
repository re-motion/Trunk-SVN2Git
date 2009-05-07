using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Renderers
{
  /// <summary>
  /// Responsible for rendering table cells of <see cref="BocCustomColumnDefinition"/> columns.
  /// </summary>
  public class BocCustomColumnRenderer : BocColumnRenderer<BocCustomColumnDefinition>
  {
    /// <summary>
    /// Contructs a renderer bound to a <see cref="BocList"/> to render, an <see cref="HtmlTextWriter"/> to render to, and a
    /// <see cref="BocCustomColumnDefinition"/> column for which to render cells.
    /// </summary>
    /// <remarks>
    /// This class should not be instantiated directly by clients. Instead, a <see cref="BocRowRenderer"/> should use a
    /// <see cref="BocListRendererFactory"/> to obtain instances of this class.
    /// </remarks>
    public BocCustomColumnRenderer (BocList list, HtmlTextWriter writer, BocCustomColumnDefinition column)
        : base(list, writer, column)
    {
    }

    /// <summary>
    /// Renders a custom column cell either directly or by wrapping the contained controls, depending on <see cref="BocCustomColumnDefinition.Mode"/>
    /// and the current row state.
    /// </summary>
    /// <remarks>
    /// If the <see cref="BocCustomColumnDefinition.Mode"/> property of <see cref="BocColumnRenderer{TBocColumnDefinition}.Column"/> indicates that
    /// the custom cell does not contain any controls (<see cref="BocCustomColumnDefinitionMode.NoControls"/> or 
    /// <see cref="BocCustomColumnDefinitionMode.ControlInEditedRow"/> when the current row is not being edited),
    /// a <see cref="BocCustomCellRenderArguments"/> object is created and passed to the custom cell's 
    /// <see cref="BocCustomColumnDefinitionCell.RenderInternal"/> method.
    /// Otherwise, a click wrapper is rendered around the child control obtained from
    /// <see cref="BocListBaseRenderer.List"/>'s <see cref="BocList.CustomColumns"/> property.
    /// </remarks>
    protected override void RenderCellContents (
      BocListDataRowRenderEventArgs dataRowRenderEventArgs, 
      int rowIndex, 
      bool isEditedRow,
      bool showIcon)
    {
      if (List.CustomColumns == null)
        return;

      int originalRowIndex = dataRowRenderEventArgs.ListIndex;
      IBusinessObject businessObject = dataRowRenderEventArgs.BusinessObject;

      if (Column.Mode == BocCustomColumnDefinitionMode.NoControls
          || (Column.Mode == BocCustomColumnDefinitionMode.ControlInEditedRow && !isEditedRow))
      {
        RenderCustomCellDirectly(businessObject, ColumnIndex, originalRowIndex);
      }
      else
      {
        RenderCustomCellInnerControls(rowIndex);
      }
    }

    private void RenderCustomCellInnerControls (int rowIndex)
    {
      Triplet[] customColumnTriplets = (Triplet[]) List.CustomColumns[Column];
      Triplet customColumnTriplet = customColumnTriplets[rowIndex];
      if (customColumnTriplet == null)
      {
        Writer.Write (c_whiteSpace);
        return;
      }

      RenderClickWrapperBeginTag ();

      Control control = (Control) customColumnTriplet.Third;
      
      ApplyStyleDefaults(control);
      control.RenderControl (Writer);

      RenderClickWrapperEndTag ();
    }

    private void RenderClickWrapperBeginTag ()
    {
      string onClick = List.HasClientScript ? c_onCommandClickScript : string.Empty;
      Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, onClick);
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);
    }

    private void RenderClickWrapperEndTag ()
    {
      Writer.RenderEndTag();
    }

    private static void ApplyStyleDefaults (Control control)
    {
      bool isControlWidthEmpty;
      CssStyleCollection controlStyle = GetControlStyle (control, out isControlWidthEmpty);
      if (controlStyle != null)
      {
        if (StringUtility.IsNullOrEmpty (controlStyle["width"]) && isControlWidthEmpty)
          controlStyle["width"] = "100%";
        if (StringUtility.IsNullOrEmpty (controlStyle["vertical-align"]))
          controlStyle["vertical-align"] = "middle";
      }
    }

    private static CssStyleCollection GetControlStyle (Control control, out bool isControlWidthEmpty)
    {
      CssStyleCollection controlStyle = null;
      isControlWidthEmpty = true;
      if (control is WebControl)
      {
        controlStyle = ((WebControl) control).Style;
        isControlWidthEmpty = ((WebControl) control).Width.IsEmpty;
      }
      else if (control is System.Web.UI.HtmlControls.HtmlControl)
      {
        controlStyle = ((System.Web.UI.HtmlControls.HtmlControl) control).Style;
      }
      return controlStyle;
    }

    private void RenderCustomCellDirectly (IBusinessObject businessObject, int columnIndex, int originalRowIndex)
    {
      string onClick = List.HasClientScript ? c_onCommandClickScript : string.Empty;
      BocCustomCellRenderArguments arguments = new BocCustomCellRenderArguments (
          List, businessObject, Column, columnIndex, originalRowIndex, onClick);
      Column.CustomCell.RenderInternal (Writer, arguments);
    }
  }
}
