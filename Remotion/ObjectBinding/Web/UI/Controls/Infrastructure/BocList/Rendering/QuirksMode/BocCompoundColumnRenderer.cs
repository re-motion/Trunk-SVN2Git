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
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode.Factories;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode
{
  /// <summary>
  /// Responsible for rendering table cells of <see cref="BocCompoundColumnDefinition"/> columns.
  /// </summary>
  public class BocCompoundColumnRenderer : BocValueColumnRendererBase<BocCompoundColumnDefinition>
  {
    /// <summary>
    /// Contructs a renderer bound to a <see cref="BocList"/> to render, an <see cref="HtmlTextWriter"/> to render to, and a
    /// <see cref="BocCompoundColumnDefinition"/> column for which to render cells.
    /// </summary>
    /// <remarks>
    /// This class should not be instantiated directly by clients. Instead, a <see cref="BocRowRenderer"/> should use a
    /// <see cref="BocListRendererFactory"/> to obtain instances of this class.
    /// </remarks>
    public BocCompoundColumnRenderer (HtmlTextWriter writer, IBocList list, BocCompoundColumnDefinition columnDefinition)
        : base (writer, list, columnDefinition)
    {
    }

    /// <summary>
    /// Renders a string representation of the property of <paramref name="businessObject"/> that is shown in the column.
    /// </summary>
    /// <param name="businessObject">The <see cref="IBusinessObject"/> whose property will be rendered.</param>
    /// <param name="showEditModeControl">Prevents rendering if <see langword="true"/>.</param>
    /// <param name="editableRow">Ignored.</param>
    protected override void RenderCellText (IBusinessObject businessObject, bool showEditModeControl, EditableRow editableRow)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);

      string valueColumnText = null;
      if (!showEditModeControl)
        valueColumnText = Column.GetStringValue (businessObject);

      RenderValueColumnCellText (valueColumnText);
    }
  }
}