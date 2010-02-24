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
using System.Web.UI.WebControls;
using Remotion.Utilities;
using System.Web;

namespace Remotion.Web.UI.Controls.Rendering.DatePickerButton
{
  public abstract class DatePickerButtonRendererBase : RendererBase<IDatePickerButton>, IDatePickerButtonRenderer
  {
    private const string c_datePickerPopupForm = "DatePickerForm.aspx";

    protected DatePickerButtonRendererBase (HttpContextBase context, IDatePickerButton control)
        : base (context, control)
    {
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

      string cssClass = string.IsNullOrEmpty(Control.CssClass) ? CssClassBase : Control.CssClass;
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
        string imageUrl = GetResolvedImageUrl();
        if (imageUrl == null)
          imageUrl = ImageFileName;

        writer.AddAttribute (HtmlTextWriterAttribute.Src, imageUrl);
        writer.AddAttribute (HtmlTextWriterAttribute.Alt, StringUtility.NullToEmpty (Control.AlternateText));
        writer.RenderBeginTag (HtmlTextWriterTag.Img);
        writer.RenderEndTag();
      }

      writer.RenderEndTag();
    }

    public string GetDatePickerUrl ()
    {
      return ResourceUrlResolver.GetResourceUrl (
          Control.Parent, Context, typeof (DatePickerPage), ResourceType.UI, ResourceTheme, c_datePickerPopupForm);
    }

    public string GetResolvedImageUrl ()
    {
      return ResourceUrlResolver.GetResourceUrl (
          Control, Context, typeof (Controls.DatePickerButton), ResourceType.Image, ResourceTheme, ImageFileName);
    }

    protected abstract bool DetermineClientScriptLevel ();

    public string CssClassBase
    {
      get { return "DatePickerButton"; }
    }

    public string CssClassDisabled
    {
      get { return "disabled"; }
    }

    public string CssClassReadOnly
    {
      get { throw new NotSupportedException (); }
    }

    protected virtual string ImageFileName
    {
      get { return "DatePicker.gif"; }
    }

    private string GetClickScript (bool hasClientScript)
    {
      string script;
      if (hasClientScript && Control.Enabled)
      {
        const string pickerActionButton = "this";

        string pickerActionContainer = "document.getElementById ('" + Control.ContainerControlID.Replace ('$', '_') + "')";
        string pickerActionTarget = "document.getElementById ('" + Control.TargetControlID.Replace ('$', '_') + "')";

        string pickerUrl = "'" + GetDatePickerUrl() + "'";

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

    protected abstract Unit PopUpWidth { get; }
    protected abstract Unit PopUpHeight { get; }
  }
}
