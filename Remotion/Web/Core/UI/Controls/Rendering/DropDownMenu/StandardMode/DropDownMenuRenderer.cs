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
using System.Web;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls.Rendering.DropDownMenu.StandardMode
{
  /// <summary>
  /// Responsible for rendering <see cref="DropDownMenu"/> controls in standard mode.
  /// <seealso cref="IDropDownMenu"/>
  /// </summary>
  public class DropDownMenuRenderer : RendererBase<IDropDownMenu>
  {
    private const string c_whiteSpace = "&nbsp;";
    private const string c_dropDownIcon = "DropDownMenuArrow.gif";
    private const string c_dropDownIconDisabled = "DropDownMenuArrow_disabled.gif";

    public DropDownMenuRenderer (HttpContextBase context, IDropDownMenu control)
        : base (context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude (Control.Page);

      string key = typeof (DropDownMenuRenderer).FullName + "_Script";
      if (!htmlHeadAppender.IsRegistered (key))
      {
        string url = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (DropDownMenuRenderer), ResourceType.Html, "DropDownMenu.js");
        htmlHeadAppender.RegisterJavaScriptInclude (key, url);
      }

      key = typeof (DropDownMenuRenderer).FullName + "_Style";
      if (!htmlHeadAppender.IsRegistered (key))
      {
        string styleSheetUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (DropDownMenuRenderer), ResourceType.Html, ResourceTheme, "DropDownMenu.css");
        htmlHeadAppender.RegisterStylesheetLink (key, styleSheetUrl, HtmlHeadAppender.Priority.Library);
      }
    }

    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      RegisterEventHandlerScripts();

      AddAttributesToRender (writer);
      writer.RenderBeginTag (HtmlTextWriterTag.Span);

      RenderTitle (writer);

      writer.RenderEndTag();
    }

    private void RenderTitle (HtmlTextWriter writer)
    {
      string cssClass = CssClassHead;
      if (!Control.Enabled)
        cssClass += " " + CssClassDisabled;
      writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
      writer.RenderBeginTag (HtmlTextWriterTag.Span);

      if (Control.RenderHeadTitleMethod != null)
        Control.RenderHeadTitleMethod (writer);
      else
        RenderDefaultTitle (writer);

      RenderDropdownButton (writer);

      writer.RenderEndTag ();
    }

    private void RenderDefaultTitle (HtmlTextWriter writer)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
      writer.AddAttribute (HtmlTextWriterAttribute.Onclick, "return false;");
      writer.RenderBeginTag (HtmlTextWriterTag.A);

      if (Control.TitleIcon != null && !string.IsNullOrEmpty (Control.TitleIcon.Url))
        Control.TitleIcon.Render (writer);

      if (!string.IsNullOrEmpty (Control.TitleText))
      {
        writer.Write (Control.TitleText);
        writer.Write (c_whiteSpace);
      }
      writer.RenderEndTag ();
    }

    private void RenderDropdownButton (HtmlTextWriter writer)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassDropDownButton);
      string imageUrl = ResourceUrlResolver.GetResourceUrl (
          Control, typeof (IDropDownMenu), ResourceType.Image, ResourceTheme, Control.Enabled ? c_dropDownIcon : c_dropDownIconDisabled);

      writer.AddStyleAttribute (HtmlTextWriterStyle.BackgroundImage, string.Format("url({0})", imageUrl));
      writer.RenderBeginTag (HtmlTextWriterTag.Span);

      IconInfo.Spacer.Render (writer);
      
      writer.RenderEndTag();
    }

    protected string CssClassDropDownButton
    {
      get { return "DropDownMenuButton"; }
    }

    private void AddAttributesToRender (HtmlTextWriter writer)
    {
      AddStandardAttributesToRender (writer);
      if (string.IsNullOrEmpty (Control.CssClass) && string.IsNullOrEmpty (Control.Attributes["class"]))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassBase);

      if (Control.ControlStyle.Width.IsEmpty)
      {
        string width = "auto";
        if (!Control.Width.IsEmpty)
          width = Control.Width.ToString();

        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, width);
      }
    }

    private void RegisterEventHandlerScripts ()
    {
      if (!Control.Enabled)
        return;

      string key = Control.ClientID + "_KeyDownEventHandlerBindScript";
      string getSelectionCount = (string.IsNullOrEmpty (Control.GetSelectionCount) ? "null" : Control.GetSelectionCount);
      string script = string.Format (
          "$('#{0}').keydown( function(event){{ DropDownMenu_OnKeyDown(event, document.getElementById('{0}'), {1}); }} );",
          Control.ClientID,
          getSelectionCount);

      Control.Page.ClientScript.RegisterStartupScriptBlock (Control, typeof (DropDownMenuPreRenderer), key, script);

      if (Control.Enabled && Control.Visible && Control.Mode == MenuMode.DropDownMenu)
      {
        key = Control.ClientID + "_ClickEventHandlerBindScript";
        string elementReference = string.Format ("$('#{0}')", Control.ClientID);
        string menuIDReference = string.Format ("'{0}'", Control.ClientID);
        script = Control.GetBindOpenEventScript (elementReference, menuIDReference, false);
        Control.Page.ClientScript.RegisterStartupScriptBlock (Control, typeof (DropDownMenuPreRenderer), key, script);
      }
    }

    protected string CssClassBase
    {
      get { return "DropDownMenuContainer"; }
    }

    protected string CssClassHead
    {
      get { return "DropDownMenuSelect"; }
    }

    protected string CssClassList
    {
      get { return "DropDownMenuOptions"; }
    }

    protected string CssClassDisabled
    {
      get { return "disabled"; }
    }
  }
}
