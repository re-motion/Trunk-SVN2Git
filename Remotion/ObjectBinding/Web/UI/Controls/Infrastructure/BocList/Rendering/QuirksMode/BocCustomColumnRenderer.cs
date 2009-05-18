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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode.Factories;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode
{
  /// <summary>
  /// Responsible for rendering table cells of <see cref="BocCustomColumnDefinition"/> columns.
  /// </summary>
  public class BocCustomColumnRenderer : BocColumnRendererBase<BocCustomColumnDefinition>
  {
    /// <summary>
    /// Contructs a renderer bound to a <see cref="Remotion.ObjectBinding.Web.UI.Controls.BocList"/> to render, 
    /// an <see cref="HtmlTextWriter"/> to render to, and a <see cref="BocCustomColumnDefinition"/> column for which to render cells.
    /// </summary>
    /// <remarks>
    /// This class should not be instantiated directly by clients. Instead, a <see cref="BocRowRenderer"/> should use a
    /// <see cref="BocListRendererFactory"/> to obtain instances of this class.
    /// </remarks>
    public BocCustomColumnRenderer (IHttpContext context, HtmlTextWriter writer, IBocList list, BocCustomColumnDefinition column)
        : base (context, writer, list, column)
    {
    }

    /// <summary>
    /// Renders a custom column cell either directly or by wrapping the contained controls, depending on <see cref="BocCustomColumnDefinition.Mode"/>
    /// and the current row state.
    /// </summary>
    /// <remarks>
    /// If the <see cref="BocCustomColumnDefinition.Mode"/> property of <see cref="BocColumnRendererBase{TBocColumnDefinition}.Column"/> indicates that
    /// the custom cell does not contain any controls (<see cref="BocCustomColumnDefinitionMode.NoControls"/> or 
    /// <see cref="BocCustomColumnDefinitionMode.ControlInEditedRow"/> when the current row is not being edited),
    /// a <see cref="BocCustomCellRenderArguments"/> object is created and passed to the custom cell's 
    /// <see cref="BocCustomColumnDefinitionCell.RenderInternal"/> method.
    /// Otherwise, a click wrapper is rendered around the child control obtained from
    /// <see cref="BocListRendererBase.List"/>'s <see cref="IBocList.CustomColumns"/> property.
    /// </remarks>
    protected override void RenderCellContents (
        BocListDataRowRenderEventArgs dataRowRenderEventArgs,
        int rowIndex,
        bool isEditedRow,
        bool showIcon)
    {
      ArgumentUtility.CheckNotNull ("dataRowRenderEventArgs", dataRowRenderEventArgs);
      if (List.CustomColumns == null)
        return;

      int originalRowIndex = dataRowRenderEventArgs.ListIndex;
      IBusinessObject businessObject = dataRowRenderEventArgs.BusinessObject;

      if (Column.Mode == BocCustomColumnDefinitionMode.NoControls
          || (Column.Mode == BocCustomColumnDefinitionMode.ControlInEditedRow && !isEditedRow))
        RenderCustomCellDirectly (businessObject, ColumnIndex, originalRowIndex);
      else
        RenderCustomCellInnerControls (rowIndex);
    }

    private void RenderCustomCellInnerControls (int rowIndex)
    {
      BocListCustomColumnTriplet[] customColumnTriplets = List.CustomColumns[Column];
      BocListCustomColumnTriplet customColumnTriplet = customColumnTriplets[rowIndex];
      if (customColumnTriplet == null)
      {
        Writer.Write (c_whiteSpace);
        return;
      }

      RenderClickWrapperBeginTag();

      Control control = customColumnTriplet.Third;
      if (control != null)
      {
        ApplyStyleDefaults (control);
        control.RenderControl (Writer);
      }

      RenderClickWrapperEndTag();
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

    private void ApplyStyleDefaults (Control control)
    {
      bool isControlWidthEmpty;
      CssStyleCollection controlStyle = GetControlStyle (control, out isControlWidthEmpty);
      if (controlStyle == null)
        return;

      if (StringUtility.IsNullOrEmpty (controlStyle["width"]) && isControlWidthEmpty)
        controlStyle["width"] = "100%";
      if (StringUtility.IsNullOrEmpty (controlStyle["vertical-align"]))
        controlStyle["vertical-align"] = "middle";
    }

    private CssStyleCollection GetControlStyle (Control control, out bool isControlWidthEmpty)
    {
      CssStyleCollection controlStyle = null;
      isControlWidthEmpty = true;
      if (control is WebControl)
      {
        controlStyle = ((WebControl) control).Style;
        isControlWidthEmpty = ((WebControl) control).Width.IsEmpty;
      }
      else if (control is HtmlControl)
        controlStyle = ((HtmlControl) control).Style;
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