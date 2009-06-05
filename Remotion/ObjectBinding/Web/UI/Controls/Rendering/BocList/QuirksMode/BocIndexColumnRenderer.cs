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
  /// <summary>
  /// Responsible for rendering the index column of a <see cref="IBocList"/>.
  /// </summary>
  public class BocIndexColumnRenderer : BocListRendererBase, IBocIndexColumnRenderer
  {
    public BocIndexColumnRenderer (IHttpContext context, HtmlTextWriter writer, IBocList list)
        : base (context, writer, list)
    {
    }

    /// <summary>
    /// Renders the index cell for the data row identified by <paramref name="originalRowIndex"/>.
    /// </summary>
    /// <param name="originalRowIndex">The absolute row index in the original (unsorted) item collection.</param>
    /// <param name="selectorControlID">The ID of the control used for selecting the row. See <see cref="BocSelectorColumnRenderer"/>.</param>
    /// <param name="absoluteRowIndex">The absolute row index (including previous pages) after sorting.</param>
    /// <param name="cssClassTableCell">The CSS class to apply to the cell.</param>
    public void RenderDataCell (int originalRowIndex, string selectorControlID, int absoluteRowIndex, string cssClassTableCell)
    {
      ArgumentUtility.CheckNotNull ("cssClassTableCell", cssClassTableCell);
      if (!List.IsIndexEnabled)
        return;

      string cssClass = cssClassTableCell + " " + List.CssClassDataCellIndex;
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
      Writer.RenderBeginTag (HtmlTextWriterTag.Td);
      if (List.Index == RowIndex.InitialOrder)
        RenderRowIndex (originalRowIndex, selectorControlID);
      else if (List.Index == RowIndex.SortedOrder)
        RenderRowIndex (absoluteRowIndex, selectorControlID);
      Writer.RenderEndTag();
    }

    /// <summary>
    /// Renders the index cell for the title row.
    /// </summary>
    public void RenderTitleCell ()
    {
      if (!List.IsIndexEnabled)
        return;

      string cssClass = List.CssClassTitleCell + " " + List.CssClassTitleCellIndex;
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
      Writer.RenderBeginTag (HtmlTextWriterTag.Th);
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      string indexColumnTitle = List.IndexColumnTitle;
      if (StringUtility.IsNullOrEmpty (List.IndexColumnTitle))
        indexColumnTitle = List.GetResourceManager().GetString (Controls.BocList.ResourceIdentifier.IndexColumnTitle);

      // Do not HTML encode.
      Writer.Write (indexColumnTitle);
      Writer.RenderEndTag();
      Writer.RenderEndTag();
    }

    /// <summary> Renders the zero-based row index normalized to a one-based format
    /// (Optionally as a label for the selector control). </summary>
    private void RenderRowIndex (int index, string selectorControlID)
    {
      bool hasSelectorControl = selectorControlID != null;
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, List.CssClassContent);
      if (hasSelectorControl)
      {
        Writer.AddAttribute (HtmlTextWriterAttribute.For, selectorControlID);
        if (List.HasClientScript)
        {
          const string script = "BocList_OnSelectorControlLabelClick();";
          Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
        }
        Writer.RenderBeginTag (HtmlTextWriterTag.Label);
      }
      else
        Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      int renderedIndex = index + 1;
      if (List.IndexOffset != null)
        renderedIndex += List.IndexOffset.Value;
      Writer.Write (renderedIndex);
      Writer.RenderEndTag();
    }
  }
}