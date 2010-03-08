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
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.StandardMode.Factories;
using Remotion.Utilities;
using System.Web;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.StandardMode
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
    /// This class should not be instantiated directly by clients. Instead, a <see cref="BocListRendererFactory"/> should use a
    /// <see cref="BocRowRenderer"/> to obtain instances of this class.
    /// </remarks>
    public BocSimpleColumnRenderer (HttpContextBase context, IBocList list, BocSimpleColumnDefinition columnDefinition, CssClassContainer cssClasses)
        : base (context, list, columnDefinition, cssClasses)
    {
    }

    /// <summary>
    /// Renders either the string value of the <paramref name="businessObject"/> or the edit mode controls, 
    /// depending on <paramref name="showEditModeControl"/>
    /// </summary>
    /// <param name="writer">The <see cref="HtmlTextWriter"/>. </param>
    /// <param name="businessObject">The <see cref="IBusinessObject"/> for the current row.</param>
    /// <param name="showEditModeControl">Specifies if the edit controls will be rendered (<see langword="true"/>) or
    /// a string representation of <paramref name="businessObject"/> will be displayed (<see langword="false"/>).</param>
    /// <param name="editableRow">The <see cref="EditableRow"/> object used to actually render the edit row controls.
    /// May be <see langword="null"/> if <paramref name="showEditModeControl"/> is <see langword="false"/>.</param>
    protected override void RenderCellText (HtmlTextWriter writer, IBusinessObject businessObject, bool showEditModeControl, IEditableRow editableRow)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);

      if (showEditModeControl)
        RenderEditModeControl (writer, businessObject, editableRow);
      else
        RenderValueColumnCellText (writer, Column.GetStringValue (businessObject));
    }

    /// <summary>
    /// Renders the icon of the <see cref="IBusinessObject"/> determined by the column's property path.
    /// <seealso cref="BocSimpleColumnDefinition.GetPropertyPath"/>
    /// <seealso cref="BocSimpleColumnDefinition.GetDynamicPropertyPath"/>
    /// <seealso cref="BocSimpleColumnDefinition.IsDynamic"/>
    /// </summary>
    /// <param name="writer">The <see cref="HtmlTextWriter"/>.</param>
    /// <param name="businessObject">The <see cref="IBusinessObject"/> that acts as a starting point for the property path.</param>
    protected override void RenderOtherIcons (HtmlTextWriter writer, IBusinessObject businessObject)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);

      if (Column.EnableIcon)
      {
        IBusinessObjectPropertyPath propertyPath;
        if (Column.IsDynamic)
          propertyPath = Column.GetDynamicPropertyPath (businessObject.BusinessObjectClass);
        else
          propertyPath = Column.GetPropertyPath();

        IBusinessObject value = propertyPath.GetValue (businessObject, false, true) as IBusinessObject;
        if (value != null)
          RenderCellIcon (writer, value);
      }
    }

    private void RenderEditModeControl (HtmlTextWriter writer, IBusinessObject businessObject, IEditableRow editableRow)
    {
      EditModeValidator editModeValidator = null;
      for (int i = 0; i < List.Validators.Count; i++)
      {
        BaseValidator validator = (BaseValidator) List.Validators[i];
        if (validator is EditModeValidator)
          editModeValidator = (EditModeValidator) validator;
      }

      if (List.HasClientScript)
        writer.AddAttribute (HtmlTextWriterAttribute.Onclick, c_onCommandClickScript);
      writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin span

      editableRow.RenderSimpleColumnCellEditModeControl (
          writer,
          Column,
          businessObject,
          ColumnIndex,
          editModeValidator,
          List.EditModeController.ShowEditModeValidationMarkers,
          List.EditModeController.DisableEditModeValidationMessages);

      writer.RenderEndTag(); // End span
    }
  }
}
