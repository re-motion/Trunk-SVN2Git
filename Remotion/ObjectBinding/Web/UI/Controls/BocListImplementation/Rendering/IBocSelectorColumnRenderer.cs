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

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList
{
  public interface IBocSelectorColumnRenderer
  {
    /// <summary>
    /// Renders the cell for the title row.
    /// </summary>
    void RenderTitleCell (HtmlTextWriter writer);

    /// <summary>
    /// Renders a cell containing the selector control specified by <see cref="IBocList.Selection"/> for the row
    /// identified by <paramref name="originalRowIndex"/>
    /// </summary>
    /// <param name="writer">The <see cref="HtmlTextWriter"/>.</param>
    /// <param name="originalRowIndex">The absollute index of the row in the original (unsorted) collection.</param>
    /// <param name="selectorControlID">The ID to apply to the selector control.</param>
    /// <param name="isChecked">Indicates whether the row is selected.</param>
    /// <param name="cssClassTableCell">The CSS class to apply to the cell.</param>
    void RenderDataCell (HtmlTextWriter writer, int originalRowIndex, string selectorControlID, bool isChecked, string cssClassTableCell);
  }
}
