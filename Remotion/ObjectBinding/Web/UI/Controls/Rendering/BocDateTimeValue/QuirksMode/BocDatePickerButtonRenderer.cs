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
using System.Web.UI.WebControls;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocDateTimeValue.QuirksMode
{
  public class BocDatePickerButtonRenderer : RendererBase<IBocDatePickerButton>, IRenderer
  {
    private const int c_defaultDatePickerLengthInPoints = 150;

    public BocDatePickerButtonRenderer (IHttpContext context, HtmlTextWriter writer, IBocDatePickerButton control)
        : base (context, writer, control)
    {
    }

    public void Render ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ClientID);
      
      Writer.AddStyleAttribute (HtmlTextWriterStyle.Padding, "0px");
      Writer.AddStyleAttribute ("border", "none");
      Writer.AddStyleAttribute (HtmlTextWriterStyle.BackgroundColor, "transparent");
      // TODO: hyperLink.ApplyStyle (Control.DatePickerButtonStyle);

      bool canScript = (Control.EnableClientScript && Control.IsDesignMode) || Control.HasClientScript;
      if (canScript)
      {
        string script = GetClickScript();

        Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
        Writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
      }
      if (!Control.Enabled)
      {
        Writer.AddAttribute (HtmlTextWriterAttribute.Disabled, "disabled");
      }

      Writer.RenderBeginTag (HtmlTextWriterTag.A);

      if (canScript)
      {
        string imageUrl = Control.GetResolvedImageUrl();
        if (imageUrl == null)
          imageUrl = Control.ImageFileName;

        Writer.AddAttribute (HtmlTextWriterAttribute.Src, imageUrl);
        Writer.AddAttribute (HtmlTextWriterAttribute.Alt, StringUtility.NullToEmpty(Control.AlternateText));
        Writer.AddStyleAttribute (HtmlTextWriterStyle.BorderWidth, "0px");
        Writer.RenderBeginTag (HtmlTextWriterTag.Img);
        Writer.RenderEndTag ();
      }

      Writer.RenderEndTag();
    }

    private string GetClickScript ()
    {
      string script;
      if (Control.HasClientScript && Control.Enabled)
      {
        string pickerActionButton = "this";
        
        string pickerActionContainer = "document.getElementById ('" + Control.ContainerControlId.Replace('$', '_') + "')";
        string pickerActionTarget = "document.getElementById ('" + Control.TargetControlId.Replace ('$', '_') + "')";

        string pickerUrl = "'" + Control.GetDatePickerUrl() + "'";

        Unit popUpWidth = Control.DatePickerPopupWidth;
        if (popUpWidth.IsEmpty)
          popUpWidth = Unit.Point (c_defaultDatePickerLengthInPoints);
        string pickerWidth = "'" + popUpWidth + "'";

        Unit popUpHeight = Control.DatePickerPopupHeight;
        if (popUpHeight.IsEmpty)
          popUpHeight = Unit.Point (c_defaultDatePickerLengthInPoints);
        string pickerHeight = "'" + popUpHeight + "'";

        script = "DatePicker_ShowDatePicker("
                 + pickerActionButton + ", "
                 + pickerActionContainer + ", "
                 + pickerActionTarget + ", "
                 + pickerUrl + ", "
                 + pickerWidth + ", "
                 + pickerHeight + ");"
                 + "return false;";
      }
      else
        script = "return false;";
      return script;
    }
  }
}