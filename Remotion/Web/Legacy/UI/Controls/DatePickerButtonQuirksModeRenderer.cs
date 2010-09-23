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
using Remotion.Web.UI.Controls.DatePickerButtonImplementation.Rendering;

namespace Remotion.Web.Legacy.UI.Controls
{
  /// <summary>
  /// Responsible for rendering a <see cref="DatePickerButton"/> control in quirks mode.
  /// <seealso cref="IDatePickerButton"/>
  /// </summary>
  public class DatePickerButtonQuirksModeRenderer : QuirksModeRendererBase<IDatePickerButton>
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

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender, IControl control)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      string scriptFileKey = typeof (DatePickerButtonQuirksModeRenderer).FullName + "_Script";
      if (!htmlHeadAppender.IsRegistered (scriptFileKey))
      {
        string scriptUrl = ResourceUrlResolver.GetResourceUrl (
            control, Context, typeof (DatePickerButtonQuirksModeRenderer), ResourceType.Html, "DatePicker.js");
        htmlHeadAppender.RegisterJavaScriptInclude (scriptFileKey, scriptUrl);
      }

      string styleFileKey = typeof (DatePickerButtonQuirksModeRenderer).FullName + "_Style";
      if (!htmlHeadAppender.IsRegistered (styleFileKey))
      {
        string styleUrl = ResourceUrlResolver.GetResourceUrl (
            control, Context, typeof (DatePickerButtonQuirksModeRenderer), ResourceType.Html, "DatePicker.css");
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

      Render (new DatePickerButtonRenderingContext (Context, writer, Control));
    }

    public void Render (DatePickerButtonRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      bool hasClientScript = DetermineClientScriptLevel (renderingContext);
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Id, renderingContext.Control.ClientID);

      string cssClass = string.IsNullOrEmpty (renderingContext.Control.CssClass) ? CssClassBase : renderingContext.Control.CssClass;
      if (!renderingContext.Control.Enabled)
        cssClass += " " + CssClassDisabled;
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);

      // TODO: hyperLink.ApplyStyle (Control.DatePickerButtonStyle);

      bool canScript = (renderingContext.Control.EnableClientScript && renderingContext.Control.IsDesignMode) || hasClientScript;
      if (canScript)
      {
        string script = GetClickScript (renderingContext, hasClientScript);

        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
      }
      if (!renderingContext.Control.Enabled)
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Disabled, "disabled");

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.A);

      if (canScript)
      {
        string imageUrl = GetResolvedImageUrl (renderingContext);

        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Src, imageUrl);
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Alt, StringUtility.NullToEmpty (renderingContext.Control.AlternateText));
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Img);
        renderingContext.Writer.RenderEndTag ();
      }

      renderingContext.Writer.RenderEndTag ();
    }

    public string GetDatePickerUrl (DatePickerButtonRenderingContext renderingContext)
    {
      return ResourceUrlResolver.GetResourceUrl (
          renderingContext.Control.Parent, renderingContext.HttpContext, typeof (DatePickerPageQuirksModeRenderer), ResourceType.UI, c_datePickerPopupForm);
    }

    public string GetResolvedImageUrl (DatePickerButtonRenderingContext renderingContext)
    {
      return ResourceUrlResolver.GetResourceUrl (
          renderingContext.Control, renderingContext.HttpContext, typeof (DatePickerButtonQuirksModeRenderer), ResourceType.Image, c_datePickerIcon);
    }

    private string GetClickScript (DatePickerButtonRenderingContext renderingContext, bool hasClientScript)
    {
      string script;
      if (hasClientScript && renderingContext.Control.Enabled)
      {
        const string pickerActionButton = "this";

        string pickerActionContainer = "document.getElementById ('" + renderingContext.Control.ContainerControlID.Replace ('$', '_') + "')";
        string pickerActionTarget = "document.getElementById ('" + renderingContext.Control.TargetControlID.Replace ('$', '_') + "')";

        string pickerUrl = "'" + GetDatePickerUrl (renderingContext) + "'";

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

    protected bool DetermineClientScriptLevel (DatePickerButtonRenderingContext renderingContext)
    {
      if (!renderingContext.Control.EnableClientScript)
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