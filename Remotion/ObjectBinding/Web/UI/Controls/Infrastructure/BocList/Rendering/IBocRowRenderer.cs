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
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode;

namespace Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering
{
  public interface IBocRowRenderer
  {
    /// <summary> Renders the table row containing the column titles and sorting buttons. </summary>
    /// <remarks> Title format: &lt;span&gt;label button &lt;span&gt;sort order&lt;/span&gt;&lt;/span&gt; </remarks>
    void RenderTitlesRow ();

    /// <summary>
    /// Renders a row containing the empty list message in <see cref="BocListRendererBase.List"/>'s 
    /// <see cref="Remotion.ObjectBinding.Web.UI.Controls.BocList.EmptyListMessage"/> property.
    /// </summary>
    /// <remarks>
    /// If the property is not set, a default message will be loaded from the resource file, using 
    /// <see cref="Remotion.ObjectBinding.Web.UI.Controls.BocList.ResourceIdentifier.EmptyListMessage"/> as key. 
    /// </remarks>
    void RenderEmptyListDataRow ();

    /// <summary>Renders a table row containing the data of <paramref name="businessObject"/>. </summary>
    /// <param name="businessObject"> The <see cref="IBusinessObject"/> whose data will be rendered. </param>
    /// <param name="rowIndex"> The row number in the current view. </param>
    /// <param name="absoluteRowIndex"> The position of <paramref name="businessObject"/> in the list of values. </param>
    /// <param name="originalRowIndex"> The position of <paramref name="businessObject"/> in the list of values before sorting. </param>
    void RenderDataRow (
        IBusinessObject businessObject,
        int rowIndex,
        int absoluteRowIndex,
        int originalRowIndex);

    /// <summary>The <see cref="BocList"/> containing the data to render.</summary>
    IBocList List { get; }

    /// <summary>The <see cref="HtmlTextWriter"/> that is used to render the table cells.</summary>
    HtmlTextWriter Writer { get; }
  }
}