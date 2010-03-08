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

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering the index column of a <see cref="IBocList"/>.
  /// </summary>
  public class BocIndexColumnRenderer : IBocIndexColumnRenderer
  {
    private readonly HttpContextBase _context;
    private readonly IBocList _list;
    private readonly CssClassContainer _cssClasses;

    public BocIndexColumnRenderer (HttpContextBase context, IBocList list, CssClassContainer cssClasses)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("cssClasses", cssClasses);

      _context = context;
      _list = list;
      _cssClasses = cssClasses;
    }

    public HttpContextBase Context
    {
      get { return _context; }
    }

    public IBocList List
    {
      get { return _list; }
    }

    public CssClassContainer CssClasses
    {
      get { return _cssClasses; }
    }

    public void RenderDataCell (HtmlTextWriter writer, int originalRowIndex, string selectorControlID, int absoluteRowIndex, string cssClassTableCell)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("cssClassTableCell", cssClassTableCell);

      if (!List.IsIndexEnabled)
        return;

      string cssClass = cssClassTableCell + " " + CssClasses.DataCellIndex;
      writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
      writer.RenderBeginTag (HtmlTextWriterTag.Td);
      if (List.Index == RowIndex.InitialOrder)
        RenderRowIndex (writer, originalRowIndex, selectorControlID);
      else if (List.Index == RowIndex.SortedOrder)
        RenderRowIndex (writer, absoluteRowIndex, selectorControlID);
      writer.RenderEndTag();
    }

    public void RenderTitleCell (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      if (!List.IsIndexEnabled)
        return;

      string cssClass = CssClasses.TitleCell + " " + CssClasses.TitleCellIndex;
      writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
      writer.RenderBeginTag (HtmlTextWriterTag.Th);
      writer.RenderBeginTag (HtmlTextWriterTag.Span);
      string indexColumnTitle = List.IndexColumnTitle;
      if (StringUtility.IsNullOrEmpty (List.IndexColumnTitle))
        indexColumnTitle = List.GetResourceManager().GetString (Controls.BocList.ResourceIdentifier.IndexColumnTitle);

      // Do not HTML encode.
      writer.Write (indexColumnTitle);
      writer.RenderEndTag();
      writer.RenderEndTag();
    }

    /// <summary> Renders the zero-based row index normalized to a one-based format
    /// (Optionally as a label for the selector control). </summary>
    private void RenderRowIndex (HtmlTextWriter writer, int index, string selectorControlID)
    {
      bool hasSelectorControl = selectorControlID != null;
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.Content);
      if (hasSelectorControl)
      {
        writer.AddAttribute (HtmlTextWriterAttribute.For, selectorControlID);
        if (List.HasClientScript)
        {
          const string script = "BocList_OnSelectorControlLabelClick();";
          writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
        }
        writer.RenderBeginTag (HtmlTextWriterTag.Label);
      }
      else
        writer.RenderBeginTag (HtmlTextWriterTag.Span);
      int renderedIndex = index + 1;
      if (List.IndexOffset != null)
        renderedIndex += List.IndexOffset.Value;
      writer.Write (renderedIndex);
      writer.RenderEndTag();
    }


  }
}