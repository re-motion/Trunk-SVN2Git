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
using Remotion.Utilities;
using System.Web;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.StandardMode
{
  /// <summary>
  /// Responsible for rendering the cells containing the row selector controls.
  /// </summary>
  public class BocSelectorColumnRenderer : BocListRendererBase, IBocSelectorColumnRenderer
  {
    private const int c_titleRowIndex = -1;

    public BocSelectorColumnRenderer (HttpContextBase context, HtmlTextWriter writer, IBocList list, CssClassContainer cssClasses)
        : base (context, writer, list, cssClasses)
    {
    }

    /// <summary>
    /// Renders a cell containing the selector control specified by <see cref="IBocList.Selection"/> for the row
    /// identified by <paramref name="originalRowIndex"/>
    /// </summary>
    /// <param name="originalRowIndex">The absollute index of the row in the original (unsorted) collection.</param>
    /// <param name="selectorControlID">The ID to apply to the selector control.</param>
    /// <param name="isChecked">Indicates whether the row is selected.</param>
    /// <param name="cssClassTableCell">The CSS class to apply to the cell.</param>
    public void RenderDataCell (int originalRowIndex, string selectorControlID, bool isChecked, string cssClassTableCell)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("selectorControlID", selectorControlID);
      ArgumentUtility.CheckNotNullOrEmpty ("cssClassTableCell", cssClassTableCell);

      if (!List.IsSelectionEnabled)
        return;

      Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClassTableCell);
      Writer.RenderBeginTag (HtmlTextWriterTag.Td);
      RenderSelectorControl (selectorControlID, originalRowIndex.ToString(), isChecked, false);
      Writer.RenderEndTag();
    }

    /// <summary>
    /// Renders the cell for the title row.
    /// </summary>
    public void RenderTitleCell ()
    {
      if (!List.IsSelectionEnabled)
        return;

      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.TitleCell);
      Writer.RenderBeginTag (HtmlTextWriterTag.Th);
      if (List.Selection == RowSelection.Multiple)
      {
        string selectorControlName = List.GetSelectAllControlClientID();
        bool isChecked = (List.SelectorControlCheckedState.Contains (c_titleRowIndex));
        RenderSelectorControl (selectorControlName, c_titleRowIndex.ToString(), isChecked, true);
      }
      else
        Writer.Write (c_whiteSpace);
      Writer.RenderEndTag();
    }
  }
}
