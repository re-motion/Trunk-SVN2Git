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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.Factories;
using Remotion.Utilities;
using System.Web;

namespace Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering cells of <see cref="BocCommandColumnDefinition"/> columns.
  /// </summary>
  public class BocCommandColumnQuirksModeRenderer : BocCommandEnabledColumnQuirksModeRendererBase<BocCommandColumnDefinition>
  {
    /// <summary>
    /// Contructs a renderer bound to a <see cref="BocList"/> to render, an <see cref="HtmlTextWriter"/> to render to, and a
    /// <see cref="BocCommandColumnDefinition"/> column for which to render cells.
    /// </summary>
    /// <remarks>
    /// This class should not be instantiated directly by clients. Instead, a <see cref="BocRowRenderer"/> should use a
    /// <see cref="BocListRendererFactory"/> to obtain instances of this class.
    /// </remarks>
    public BocCommandColumnQuirksModeRenderer (HttpContextBase context, IBocList list, BocCommandColumnDefinition columnDefinition, CssClassContainer cssClasses)
        : base (context, list, columnDefinition, cssClasses)
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
    protected override void RenderCellContents (HtmlTextWriter writer, BocListDataRowRenderEventArgs dataRowRenderEventArgs, int rowIndex, bool showIcon)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("dataRowRenderEventArgs", dataRowRenderEventArgs);

      int originalRowIndex = dataRowRenderEventArgs.ListIndex;
      IBusinessObject businessObject = dataRowRenderEventArgs.BusinessObject;

      IEditableRow editableRow = List.EditModeController.GetEditableRow (originalRowIndex);

      bool hasEditModeControl = editableRow != null && editableRow.HasEditControl (ColumnIndex);

      bool isCommandEnabled = RenderBeginTag (writer, originalRowIndex, businessObject);

      RenderCellIcon (writer, businessObject, hasEditModeControl, showIcon);
      RenderCellCommand (writer);

      RenderEndTag (writer, isCommandEnabled);
    }

    private void RenderCellCommand (HtmlTextWriter writer)
    {
      if (Column.Icon.HasRenderingInformation)
        Column.Icon.Render (writer);

      if (!StringUtility.IsNullOrEmpty (Column.Text))
        writer.Write (Column.Text); // Do not HTML encode
    }

    private void RenderCellIcon (HtmlTextWriter writer, IBusinessObject businessObject, bool hasEditModeControl, bool showIcon)
    {
      if (!hasEditModeControl && showIcon)
        RenderCellIcon (writer, businessObject);
    }

    private bool RenderBeginTag (HtmlTextWriter writer, int originalRowIndex, IBusinessObject businessObject)
    {
      bool isCommandEnabled = RenderBeginTagDataCellCommand (writer, businessObject, originalRowIndex);
      if (!isCommandEnabled)
      {
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.Content);
        writer.RenderBeginTag (HtmlTextWriterTag.Span);
      }
      return isCommandEnabled;
    }

    protected void RenderEndTag (HtmlTextWriter writer, bool isCommandEnabled)
    {
      if (isCommandEnabled)
        RenderEndTagDataCellCommand (writer);
      else
        writer.RenderEndTag();
    }
  }
}