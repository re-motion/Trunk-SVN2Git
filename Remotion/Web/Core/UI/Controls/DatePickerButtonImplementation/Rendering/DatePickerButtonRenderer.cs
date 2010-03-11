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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls.DatePickerButtonImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering a <see cref="DatePickerButton"/> control in standard mode.
  /// <seealso cref="IDatePickerButton"/>
  /// </summary>
  public class DatePickerButtonRenderer : RendererBase<IDatePickerButton>
  {
    public DatePickerButtonRenderer (HttpContextBase context, IDatePickerButton control)
        : base (context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      string scriptFileKey = typeof (DatePickerButtonRenderer).FullName + "_Script";
      if (!htmlHeadAppender.IsRegistered (scriptFileKey))
      {
        string scriptUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (DatePickerButtonRenderer), ResourceType.Html, "DatePicker.js");
        htmlHeadAppender.RegisterJavaScriptInclude (scriptFileKey, scriptUrl);
      }

      string styleFileKey = typeof (DatePickerButtonRenderer).FullName + "_Style";
      if (!htmlHeadAppender.IsRegistered (styleFileKey))
      {
        string styleUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (DatePickerButtonRenderer), ResourceType.Html, ResourceTheme, "DatePicker.css");
        htmlHeadAppender.RegisterStylesheetLink (styleFileKey, styleUrl, HtmlHeadAppender.Priority.Library);
      }
    }


    private const string c_datePickerPopupForm = "DatePickerForm.aspx";
    private const string c_datePickerIcon = "DatePicker.gif";

    /// <summary>
    /// Renders a click-enabled image that shows a <see cref="DatePickerPage"/> on click, which puts the selected value
    /// into the control specified by <see cref="P:Control.TargetControlID"/>.
    /// </summary>
    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ClientID);

      string cssClass = string.IsNullOrEmpty (Control.CssClass) ? CssClassBase : Control.CssClass;
      if (!Control.Enabled)
        cssClass += " " + CssClassDisabled;
      writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);

      // TODO: hyperLink.ApplyStyle (Control.DatePickerButtonStyle);

      string script = GetClickScript (true);

      writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
      writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");

      if (!Control.Enabled)
        writer.AddAttribute (HtmlTextWriterAttribute.Disabled, "disabled");

      writer.RenderBeginTag (HtmlTextWriterTag.A);

      string imageUrl = GetResolvedImageUrl();

      writer.AddAttribute (HtmlTextWriterAttribute.Src, imageUrl);
      writer.AddAttribute (HtmlTextWriterAttribute.Alt, StringUtility.NullToEmpty (Control.AlternateText));
      writer.RenderBeginTag (HtmlTextWriterTag.Img);
      writer.RenderEndTag();

      writer.RenderEndTag();
    }

    public string GetDatePickerUrl ()
    {
      return ResourceUrlResolver.GetResourceUrl (
          Control.Parent, Context, typeof (DatePickerPageRenderer), ResourceType.UI, ResourceTheme, c_datePickerPopupForm);
    }

    public string GetResolvedImageUrl ()
    {
      return ResourceUrlResolver.GetResourceUrl (
          Control, Context, typeof (DatePickerButtonRenderer), ResourceType.Image, ResourceTheme, c_datePickerIcon);
    }

    private string GetClickScript (bool hasClientScript)
    {
      string script;
      if (hasClientScript && Control.Enabled)
      {
        const string pickerActionButton = "this";

        string pickerActionContainer = "document.getElementById ('" + Control.ContainerControlID.Replace ('$', '_') + "')";
        string pickerActionTarget = "document.getElementById ('" + Control.TargetControlID.Replace ('$', '_') + "')";

        string pickerUrl = "'" + GetDatePickerUrl () + "'";

        Unit popUpWidth = PopUpWidth;
        string pickerWidth = "'" + popUpWidth + "'";

        Unit popUpHeight = PopUpHeight;
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

    public string CssClassBase
    {
      get { return "DatePickerButton"; }
    }

    public string CssClassDisabled
    {
      get { return "disabled"; }
    }

    protected Unit PopUpWidth
    {
      get { return new Unit (14, UnitType.Em); }
    }

    protected Unit PopUpHeight
    {
      get { return new Unit (16, UnitType.Em); }
    }
  }
}