using System;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;

namespace Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode
{
  public class BocSelectorColumnRenderer : BocListRendererBase, IBocSelectorColumnRenderer
  {
    private const int c_titleRowIndex = -1;

    public BocSelectorColumnRenderer (IHttpContext context, HtmlTextWriter writer, IBocList list)
        : base(context, writer, list)
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
        bool isChecked = (List.SelectorControlCheckedState.Contains(c_titleRowIndex));
        RenderSelectorControl (selectorControlName, c_titleRowIndex.ToString (), isChecked, true);
      }
      else
        Writer.Write (c_whiteSpace);
      Writer.RenderEndTag ();
    }
  }
}
