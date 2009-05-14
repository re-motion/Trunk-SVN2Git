using System;
using System.Web.UI;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode
{
  public class BocIndexColumnRenderer : BocListRendererBase, IBocIndexColumnRenderer
  {
    public BocIndexColumnRenderer (HtmlTextWriter writer, Controls.BocList list)
        : base(writer, list)
    {
    }

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
      Writer.RenderEndTag ();
    }

    public void RenderTitleCell ()
    {
      if (!List.IsIndexEnabled)
        return;

      string cssClass = List.CssClassTitleCell + " " + List.CssClassTitleCellIndex;
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
      Writer.RenderBeginTag (HtmlTextWriterTag.Th);
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      string indexColumnTitle;
      if (StringUtility.IsNullOrEmpty (List.IndexColumnTitle))
        indexColumnTitle = List.GetResourceManager ().GetString (Controls.BocList.ResourceIdentifier.IndexColumnTitle);
      else
        indexColumnTitle = List.IndexColumnTitle;
      // Do not HTML encode.
      Writer.Write (indexColumnTitle);
      Writer.RenderEndTag ();
      Writer.RenderEndTag ();
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
      Writer.RenderEndTag ();
    }
  }
}
