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
using System.Collections.Generic;
using System.Web.UI;
using Remotion.Utilities;
using System.Web;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering the index column of a <see cref="IBocList"/>.
  /// </summary>
  public class BocIndexColumnRenderer : IBocIndexColumnRenderer
  {
    private readonly BocListCssClassDefinition _cssClasses;

    public BocIndexColumnRenderer (BocListCssClassDefinition cssClasses)
    {
      ArgumentUtility.CheckNotNull ("cssClasses", cssClasses);

      _cssClasses = cssClasses;
    }

    public BocListCssClassDefinition CssClasses
    {
      get { return _cssClasses; }
    }

    public void RenderDataCell (BocListRenderingContext renderingContext, int originalRowIndex, string selectorControlID, int absoluteRowIndex, string cssClassTableCell)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNull ("cssClassTableCell", cssClassTableCell);

      if (!renderingContext.Control.IsIndexEnabled)
        return;

      string cssClass = cssClassTableCell + " " + CssClasses.DataCellIndex;
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td);
      if (renderingContext.Control.Index == RowIndex.InitialOrder)
        RenderRowIndex (renderingContext, originalRowIndex, selectorControlID);
      else if (renderingContext.Control.Index == RowIndex.SortedOrder)
        RenderRowIndex (renderingContext, absoluteRowIndex, selectorControlID);
      renderingContext.Writer.RenderEndTag();
    }

    public void RenderTitleCell (BocListRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      if (!renderingContext.Control.IsIndexEnabled)
        return;

      string cssClass = CssClasses.TitleCell + " " + CssClasses.TitleCellIndex;
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Th);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      string indexColumnTitle = renderingContext.Control.IndexColumnTitle;
      if (StringUtility.IsNullOrEmpty (renderingContext.Control.IndexColumnTitle))
        indexColumnTitle = renderingContext.Control.GetResourceManager().GetString (Controls.BocList.ResourceIdentifier.IndexColumnTitle);

      // Do not HTML encode.
      renderingContext.Writer.Write (indexColumnTitle);
      renderingContext.Writer.RenderEndTag();
      renderingContext.Writer.RenderEndTag();
    }

    /// <summary> Renders the zero-based row index normalized to a one-based format
    /// (Optionally as a label for the selector control). </summary>
    private void RenderRowIndex (BocListRenderingContext renderingContext, int index, string selectorControlID)
    {
      bool hasSelectorControl = selectorControlID != null;
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.Content);
      if (hasSelectorControl)
      {
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.For, selectorControlID);
        if (renderingContext.Control.HasClientScript)
        {
          const string script = "BocList_OnSelectorControlLabelClick();";
          renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
        }
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Label);
      }
      else
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      int renderedIndex = index + 1;
      if (renderingContext.Control.IndexOffset != null)
        renderedIndex += renderingContext.Control.IndexOffset.Value;
      renderingContext.Writer.Write (renderedIndex);
      renderingContext.Writer.RenderEndTag();
    }


  }
}