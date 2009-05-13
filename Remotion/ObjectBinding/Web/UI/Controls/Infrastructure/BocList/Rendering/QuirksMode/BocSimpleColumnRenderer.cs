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
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode.Factories;

namespace Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode
{
  /// <summary>
  /// Responsible for rendering table cells of <see cref="BocSimpleColumnDefinition"/> columns.
  /// </summary>
  public class BocSimpleColumnRenderer : BocValueColumnRendererBase<BocSimpleColumnDefinition>
  {
    /// <summary>
    /// Contructs a renderer bound to a <see cref="BocList"/> to render, an <see cref="HtmlTextWriter"/> to render to, and a
    /// <see cref="BocSimpleColumnDefinition"/> column for which to render cells.
    /// </summary>
    /// <remarks>
    /// This class should not be instantiated directly by clients. Instead, a <see cref="BocRowRenderer"/> should use a
    /// <see cref="BocListRendererFactory"/> to obtain instances of this class.
    /// </remarks>
    public BocSimpleColumnRenderer (HtmlTextWriter writer, Controls.BocList list, BocSimpleColumnDefinition columnDefinition)
        : base (writer, list, columnDefinition)
    {
    }

    /// <summary>
    /// Renders either the string value of the <paramref name="businessObject"/> or the edit mode controls, 
    /// depending on <paramref name="showEditModeControl"/>
    /// </summary>
    /// <param name="businessObject">The <see cref="IBusinessObject"/> for the current row.</param>
    /// <param name="showEditModeControl">Specifies if the edit controls will be rendered (<see langword="true"/>) or
    /// a string representation of <paramref name="businessObject"/> will be displayed (<see langword="false"/>).</param>
    /// <param name="editableRow">The <see cref="EditableRow"/> object used to actually render the edit row controls.
    /// May be <see langword="null"/> if <paramref name="showEditModeControl"/> is <see langword="false"/>.</param>
    protected override void RenderCellText (IBusinessObject businessObject, bool showEditModeControl, EditableRow editableRow)
    {
      if (showEditModeControl)
        RenderEditModeControl (businessObject, editableRow);
      else
        RenderValueColumnCellText (Column.GetStringValue (businessObject));
    }

    /// <summary>
    /// Renders the icon of the <see cref="IBusinessObject"/> determined by the column's property path.
    /// <seealso cref="BocSimpleColumnDefinition.GetPropertyPath"/>
    /// <seealso cref="BocSimpleColumnDefinition.GetDynamicPropertyPath"/>
    /// <seealso cref="BocSimpleColumnDefinition.IsDynamic"/>
    /// </summary>
    /// <param name="businessObject">The <see cref="IBusinessObject"/> that acts as a starting point for the property path.</param>
    protected override void RenderOtherIcons (IBusinessObject businessObject)
    {
      if (Column.EnableIcon)
      {
        IBusinessObjectPropertyPath propertyPath;
        if (Column.IsDynamic)
          propertyPath = Column.GetDynamicPropertyPath (businessObject.BusinessObjectClass);
        else
          propertyPath = Column.GetPropertyPath();

        IBusinessObject value = propertyPath.GetValue (businessObject, false, true) as IBusinessObject;
        if (value != null)
          RenderCellIcon (value);
      }
    }

    private void RenderEditModeControl (
        IBusinessObject businessObject,
        EditableRow editableRow)
    {
      EditModeValidator editModeValidator = null;
      for (int i = 0; i < List.Validators.Count; i++)
      {
        BaseValidator validator = (BaseValidator) List.Validators[i];
        if (validator is EditModeValidator)
          editModeValidator = (EditModeValidator) validator;
      }

      if (List.HasClientScript)
        Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, c_onCommandClickScript);
      Writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin span

      editableRow.RenderSimpleColumnCellEditModeControl (
          Writer,
          Column,
          businessObject,
          ColumnIndex,
          editModeValidator,
          List.EditModeController.ShowEditModeValidationMarkers,
          List.EditModeController.DisableEditModeValidationMessages);

      Writer.RenderEndTag(); // End span
    }
  }
}