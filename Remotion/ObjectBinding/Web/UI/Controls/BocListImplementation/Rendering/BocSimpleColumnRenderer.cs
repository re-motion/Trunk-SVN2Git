// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.Utilities;
using Remotion.Web;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering table cells of <see cref="BocSimpleColumnDefinition"/> columns.
  /// </summary>
  public class BocSimpleColumnRenderer : BocValueColumnRendererBase<BocSimpleColumnDefinition>, IBocSimpleColumnRenderer
  {
    /// <summary>
    /// Contructs a renderer bound to a <see cref="BocList"/> to render, an <see cref="HtmlTextWriter"/> to render to, and a
    /// <see cref="BocSimpleColumnDefinition"/> column for which to render cells.
    /// </summary>
    /// <remarks>
    /// This class should not be instantiated directly by clients. Instead, a <see cref="BocRowRenderer"/> should use a
    /// factory to obtain instances of this class.
    /// </remarks>
    public BocSimpleColumnRenderer (
        IResourceUrlFactory resourceUrlFactory,
        BocListCssClassDefinition cssClasses)
        : base (resourceUrlFactory, cssClasses)
    {
    }

    /// <summary>
    /// Renders the edit mode control.
    /// </summary>
    /// <param name="renderingContext">The <see cref="BocColumnRenderingContext{BocColumnDefinition}"/>.</param>
    /// <param name="businessObject">The <see cref="IBusinessObject"/> whose property will be rendered.</param>
    /// <param name="editableRow">The <see cref="EditableRow"/> object used to actually render the edit row controls.</param>
    protected override void RenderCellDataForEditMode (
        BocColumnRenderingContext<BocSimpleColumnDefinition> renderingContext, IBusinessObject businessObject, IEditableRow editableRow)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);
      ArgumentUtility.CheckNotNull ("editableRow", editableRow);

      RenderEditModeControl (renderingContext, businessObject, editableRow);
    }

    /// <summary>
    /// Renders the icon of the <see cref="IBusinessObject"/> determined by the column's property path.
    /// <seealso cref="BocSimpleColumnDefinition.GetPropertyPath"/>
    /// <seealso cref="BocSimpleColumnDefinition.GetDynamicPropertyPath"/>
    /// <seealso cref="BocSimpleColumnDefinition.IsDynamic"/>
    /// </summary>
    /// <param name="renderingContext">The <see cref="BocColumnRenderingContext{BocColumnDefinition}"/>.</param>
    /// <param name="businessObject">The <see cref="IBusinessObject"/> that acts as a starting point for the property path.</param>
    protected override void RenderOtherIcons (BocColumnRenderingContext<BocSimpleColumnDefinition> renderingContext, IBusinessObject businessObject)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);

      if (renderingContext.ColumnDefinition.EnableIcon)
      {
        IBusinessObjectPropertyPath propertyPath;
        if (renderingContext.ColumnDefinition.IsDynamic)
          propertyPath = renderingContext.ColumnDefinition.GetDynamicPropertyPath (businessObject.BusinessObjectClass);
        else
          propertyPath = renderingContext.ColumnDefinition.GetPropertyPath();

        IBusinessObject value = propertyPath.GetValue (businessObject, false, true) as IBusinessObject;
        if (value != null)
          RenderCellIcon (renderingContext, value);
      }
    }

    private void RenderEditModeControl (
        BocColumnRenderingContext<BocSimpleColumnDefinition> renderingContext, IBusinessObject businessObject, IEditableRow editableRow)
    {
      EditModeValidator editModeValidator = null;
      for (int i = 0; i < renderingContext.Control.Validators.Count; i++)
      {
        BaseValidator validator = (BaseValidator) renderingContext.Control.Validators[i];
        if (validator is EditModeValidator)
          editModeValidator = (EditModeValidator) validator;
      }

      if (renderingContext.Control.HasClientScript)
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, c_onCommandClickScript);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin span

      editableRow.RenderSimpleColumnCellEditModeControl (
          renderingContext.Writer,
          renderingContext.ColumnDefinition,
          businessObject,
          renderingContext.ColumnIndex,
          editModeValidator,
          renderingContext.Control.EditModeController.ShowEditModeValidationMarkers,
          renderingContext.Control.EditModeController.DisableEditModeValidationMessages);

      renderingContext.Writer.RenderEndTag(); // End span
    }
  }
}