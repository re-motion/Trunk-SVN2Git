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

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  public interface IBocRowRenderer
  {
    /// <summary> Renders the table row containing the column titles and sorting buttons. </summary>
    /// <remarks> Title format: &lt;span&gt;label button &lt;span&gt;sort order&lt;/span&gt;&lt;/span&gt; </remarks>
    void RenderTitlesRow (HtmlTextWriter writer);

    /// <summary>
    /// Renders a row containing the empty list message in <see cref="BocListRenderer.List"/>'s 
    /// <see cref="Remotion.ObjectBinding.Web.UI.Controls.BocList.EmptyListMessage"/> property.
    /// </summary>
    /// <remarks>
    /// If the property is not set, a default message will be loaded from the resource file, using 
    /// <see cref="Remotion.ObjectBinding.Web.UI.Controls.BocList.ResourceIdentifier.EmptyListMessage"/> as key. 
    /// </remarks>
    void RenderEmptyListDataRow (HtmlTextWriter writer);

    /// <summary>Renders a table row containing the data of <paramref name="businessObject"/>. </summary>
    /// <param name="writer">The <see cref="HtmlTextWriter"/>.</param>
    /// <param name="businessObject"> The <see cref="IBusinessObject"/> whose data will be rendered. </param>
    /// <param name="rowIndex"> The row number in the current view. </param>
    /// <param name="absoluteRowIndex"> The position of <paramref name="businessObject"/> in the list of values. </param>
    /// <param name="originalRowIndex"> The position of <paramref name="businessObject"/> in the list of values before sorting. </param>
    void RenderDataRow (HtmlTextWriter writer, IBusinessObject businessObject, int rowIndex, int absoluteRowIndex, int originalRowIndex);
  }
}