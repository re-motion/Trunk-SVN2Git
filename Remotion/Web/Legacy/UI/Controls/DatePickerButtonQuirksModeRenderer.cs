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
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.DatePickerButtonImplementation;

namespace Remotion.Web.Legacy.UI.Controls
{
  /// <summary>
  /// Responsible for rendering a <see cref="DatePickerButton"/> control in quirks mode.
  /// <seealso cref="IDatePickerButton"/>
  /// </summary>
  public class DatePickerButtonQuirksModeRenderer : RendererBase<IDatePickerButton>
  {
    private const int c_defaultDatePickerLengthInPoints = 150;
    private const string c_datePickerPopupForm = "DatePickerForm.aspx";
    private const string c_datePickerIcon = "DatePicker.gif";
    private readonly IClientScriptBehavior _clientScriptBehavior;

    public DatePickerButtonQuirksModeRenderer (HttpContextBase context, IDatePickerButton control, IClientScriptBehavior clientScriptBehavior)
        : base (context, control)
    {
      ArgumentUtility.CheckNotNull ("clientScriptBehavior", clientScriptBehavior);

      _clientScriptBehavior = clientScriptBehavior;
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      string scriptFileKey = typeof (DatePickerButtonQuirksModeRenderer).FullName + "_Script";
      if (!htmlHeadAppender.IsRegistered (scriptFileKey))
      {
        string scriptUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (DatePickerButtonQuirksModeRenderer), ResourceType.Html, "DatePicker.js");
        htmlHeadAppender.RegisterJavaScriptInclude (scriptFileKey, scriptUrl);
      }

      string styleFileKey = typeof (DatePickerButtonQuirksModeRenderer).FullName + "_Style";
      if (!htmlHeadAppender.IsRegistered (styleFileKey))
      {
        string styleUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (DatePickerButtonQuirksModeRenderer), ResourceType.Html, "DatePicker.css");
        htmlHeadAppender.RegisterStylesheetLink (styleFileKey, styleUrl, HtmlHeadAppender.Priority.Library);
      }
    }

    /// <summary>
    /// Renders a click-enabled image that shows a <see cref="DatePickerPage"/> on click, which puts the selected value
    /// into the control specified by <see cref="P:Control.TargetControlID"/>.
    /// </summary>
    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      bool hasClientScript = DetermineClientScriptLevel ();
      writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ClientID);

      string cssClass = string.IsNullOrEmpty (Control.CssClass) ? CssClassBase : Control.CssClass;
      if (!Control.Enabled)
        cssClass += " " + CssClassDisabled;
      writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);

      // TODO: hyperLink.ApplyStyle (Control.DatePickerButtonStyle);

      bool canScript = (Control.EnableClientScript && Control.IsDesignMode) || hasClientScript;
      if (canScript)
      {
        string script = GetClickScript (hasClientScript);

        writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
        writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
      }
      if (!Control.Enabled)
        writer.AddAttribute (HtmlTextWriterAttribute.Disabled, "disabled");

      writer.RenderBeginTag (HtmlTextWriterTag.A);

      if (canScript)
      {
        string imageUrl = GetResolvedImageUrl ();

        writer.AddAttribute (HtmlTextWriterAttribute.Src, imageUrl);
        writer.AddAttribute (HtmlTextWriterAttribute.Alt, StringUtility.NullToEmpty (Control.AlternateText));
        writer.RenderBeginTag (HtmlTextWriterTag.Img);
        writer.RenderEndTag ();
      }

      writer.RenderEndTag ();
    }

    public string GetDatePickerUrl ()
    {
      return ResourceUrlResolver.GetResourceUrl (
          Control.Parent, Context, typeof (DatePickerPageQuirksModeRenderer), ResourceType.UI, c_datePickerPopupForm);
    }

    public string GetResolvedImageUrl ()
    {
      return ResourceUrlResolver.GetResourceUrl (
          Control, Context, typeof (DatePickerButtonQuirksModeRenderer), ResourceType.Image, c_datePickerIcon);
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

    protected bool DetermineClientScriptLevel ()
    {
      if (!Control.EnableClientScript)
        return false;

      return _clientScriptBehavior.IsBrowserCapableOfScripting;
    }

    protected Unit PopUpWidth
    {
      get { return Unit.Point (c_defaultDatePickerLengthInPoints); }
    }

    protected Unit PopUpHeight
    {
      get { return Unit.Point (c_defaultDatePickerLengthInPoints); }
    }

    public string CssClassBase
    {
      get { return "DatePickerButton"; }
    }

    public string CssClassDisabled
    {
      get { return "disabled"; }
    }
  }
}