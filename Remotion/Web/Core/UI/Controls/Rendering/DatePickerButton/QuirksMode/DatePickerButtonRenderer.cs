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

namespace Remotion.Web.UI.Controls.Rendering.DatePickerButton.QuirksMode
{
  /// <summary>
  /// Responsible for rendering a <see cref="DatePickerButton"/> control in quirks mode.
  /// <seealso cref="IDatePickerButton"/>
  /// </summary>
  public class DatePickerButtonRenderer : RendererBase<IDatePickerButton>, IDatePickerButtonRenderer
  {
    private const string c_datePickerPopupForm = "DatePickerForm.aspx";
    private const int c_defaultDatePickerLengthInPoints = 150;


    public DatePickerButtonRenderer (IHttpContext context, HtmlTextWriter writer, IDatePickerButton control)
        : base (context, writer, control)
    {
    }

    public void PreRender ()
    {
      
    }

    /// <summary>
    /// Renders a click-enabled image that shows a <see cref="DatePickerPage"/> on click, which puts the selected value
    /// into the control specified by <see cref="P:Control.TargetControlID"/>.
    /// </summary>
    public void Render ()
    {
      bool hasClientScript = DetermineClientScriptLevel ();
      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ClientID);
      
      Writer.AddStyleAttribute (HtmlTextWriterStyle.Padding, "0px");
      Writer.AddStyleAttribute ("border", "none");
      Writer.AddStyleAttribute (HtmlTextWriterStyle.BackgroundColor, "transparent");
      // TODO: hyperLink.ApplyStyle (Control.DatePickerButtonStyle);

      bool canScript = (Control.EnableClientScript && Control.IsDesignMode) || hasClientScript;
      if (canScript)
      {
        string script = GetClickScript(hasClientScript);

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
        string imageUrl = GetResolvedImageUrl();
        if (imageUrl == null)
          imageUrl = ImageFileName;

        Writer.AddAttribute (HtmlTextWriterAttribute.Src, imageUrl);
        Writer.AddAttribute (HtmlTextWriterAttribute.Alt, StringUtility.NullToEmpty(Control.AlternateText));
        Writer.AddStyleAttribute (HtmlTextWriterStyle.BorderWidth, "0px");
        Writer.RenderBeginTag (HtmlTextWriterTag.Img);
        Writer.RenderEndTag ();
      }

      Writer.RenderEndTag();
    }

    public string CssClassBase
    {
      get { throw new NotSupportedException (); }
    }

    public string CssClassDisabled
    {
      get { throw new NotSupportedException (); }
    }

    public string CssClassReadOnly
    {
      get { throw new NotSupportedException (); }
    }

    public string GetDatePickerUrl ()
    {
      return ResourceUrlResolver.GetResourceUrl (Control.Parent, Context, typeof (DatePickerPage), ResourceType.UI, c_datePickerPopupForm);
    }

    public string GetResolvedImageUrl ()
    {
      return ResourceUrlResolver.GetResourceUrl (Control, Context, typeof (Controls.DatePickerButton), ResourceType.Image, ImageFileName);
    }

    protected virtual string ImageFileName
    {
      get { return "DatePicker.gif"; }
    }

    private bool DetermineClientScriptLevel ()
    {
      if (Control.IsDesignMode || !Control.EnableClientScript)
        return false;

      bool isVersionGreaterOrEqual55 =
          Context.Request.Browser.MajorVersion >= 6
          || Context.Request.Browser.MajorVersion == 5
             && Context.Request.Browser.MinorVersion >= 0.5;
      bool isInternetExplorer55AndHigher =
          Context.Request.Browser.Browser == "IE" && isVersionGreaterOrEqual55;

      return isInternetExplorer55AndHigher;
    }

    private string GetClickScript (bool hasClientScript)
    {
      string script;
      if (hasClientScript && Control.Enabled)
      {
        const string pickerActionButton = "this";
        
        string pickerActionContainer = "document.getElementById ('" + Control.ContainerControlID.Replace('$', '_') + "')";
        string pickerActionTarget = "document.getElementById ('" + Control.TargetControlID.Replace ('$', '_') + "')";

        string pickerUrl = "'" + GetDatePickerUrl() + "'";

        Unit popUpWidth = Unit.Point (c_defaultDatePickerLengthInPoints);
        string pickerWidth = "'" + popUpWidth + "'";

        Unit popUpHeight = Unit.Point (c_defaultDatePickerLengthInPoints);
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