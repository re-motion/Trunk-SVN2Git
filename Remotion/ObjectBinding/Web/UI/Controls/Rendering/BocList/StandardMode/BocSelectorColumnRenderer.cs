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

    public override void Render (HtmlTextWriter writer)
    {
      throw new NotImplementedException();
    }

    public void RenderDataCell (HtmlTextWriter writer, int originalRowIndex, string selectorControlID, bool isChecked, string cssClassTableCell)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNullOrEmpty ("selectorControlID", selectorControlID);
      ArgumentUtility.CheckNotNullOrEmpty ("cssClassTableCell", cssClassTableCell);

      if (!List.IsSelectionEnabled)
        return;

      writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClassTableCell);
      writer.RenderBeginTag (HtmlTextWriterTag.Td);
      RenderSelectorControl (writer, selectorControlID, originalRowIndex.ToString(), isChecked, false);
      writer.RenderEndTag();
    }

    public void RenderTitleCell (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      if (!List.IsSelectionEnabled)
        return;

      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.TitleCell);
      writer.RenderBeginTag (HtmlTextWriterTag.Th);
      if (List.Selection == RowSelection.Multiple)
      {
        string selectorControlName = List.GetSelectAllControlClientID();
        bool isChecked = (List.SelectorControlCheckedState.Contains (c_titleRowIndex));
        RenderSelectorControl (writer, selectorControlName, c_titleRowIndex.ToString(), isChecked, true);
      }
      else
        writer.Write (c_whiteSpace);
      writer.RenderEndTag();
    }
  }
}
