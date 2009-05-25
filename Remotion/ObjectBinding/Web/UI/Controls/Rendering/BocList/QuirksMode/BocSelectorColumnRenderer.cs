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
using Remotion.Utilities;
using Remotion.Web.Infrastructure;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.QuirksMode
{
  public class BocSelectorColumnRenderer : BocListRendererBase, IBocSelectorColumnRenderer
  {
    private const int c_titleRowIndex = -1;

    public BocSelectorColumnRenderer (IHttpContext context, HtmlTextWriter writer, IBocList list)
        : base (context, writer, list)
    {
    }

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

    public void RenderTitleCell ()
    {
      if (!List.IsSelectionEnabled)
        return;

      Writer.AddAttribute (HtmlTextWriterAttribute.Class, List.CssClassTitleCell);
      Writer.RenderBeginTag (HtmlTextWriterTag.Th);
      if (List.Selection == RowSelection.Multiple)
      {
        string selectorControlName = List.ID + c_titleRowSelectorControlIDSuffix;
        bool isChecked = (List.SelectorControlCheckedState.Contains (c_titleRowIndex));
        RenderSelectorControl (selectorControlName, c_titleRowIndex.ToString(), isChecked, true);
      }
      else
        Writer.Write (c_whiteSpace);
      Writer.RenderEndTag();
    }
  }
}