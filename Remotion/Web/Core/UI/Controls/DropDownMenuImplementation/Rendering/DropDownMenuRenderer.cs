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
using System.Text;
using System.Web.UI;
using System.Web;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.DropDownMenuImplementation.Rendering
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

      RegisterMenuItems();

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

      Control.Page.ClientScript.RegisterStartupScriptBlock (Control, typeof (ClientScriptBehavior), key, script);

      if (Control.Enabled && Control.Visible && Control.Mode == MenuMode.DropDownMenu)
      {
        key = Control.ClientID + "_ClickEventHandlerBindScript";
        string elementReference = string.Format ("$('#{0}')", Control.ClientID);
        string menuIDReference = string.Format ("'{0}'", Control.ClientID);
        script = Control.GetBindOpenEventScript (elementReference, menuIDReference, false);
        Control.Page.ClientScript.RegisterStartupScriptBlock (Control, typeof (ClientScriptBehavior), key, script);
      }
    }

    private void RegisterMenuItems ()
    {
      string key = Control.UniqueID;
      if (Control.Enabled && !Control.Page.ClientScript.IsStartupScriptRegistered (typeof (DropDownMenuRenderer), key))
      {
        StringBuilder script = new StringBuilder ();
        script.Append ("DropDownMenu_AddMenuInfo" + " (\r\n\t");
        script.AppendFormat ("new DropDownMenu_MenuInfo ('{0}', new Array (\r\n", Control.ClientID);
        bool isFirstItem = true;

        WebMenuItem[] menuItems;
        if (Control.EnableGrouping)
          menuItems = Control.MenuItems.GroupMenuItems (true);
        else
          menuItems = Control.MenuItems.ToArray ();

        string category = null;
        bool isCategoryVisible = false;
        for (int i = 0; i < menuItems.Length; i++)
        {
          WebMenuItem menuItem = menuItems[i];
          if (Control.EnableGrouping && category != menuItem.Category)
          {
            category = menuItem.Category;
            isCategoryVisible = false;
          }
          if (!menuItem.EvaluateVisible ())
            continue;
          if (Control.EnableGrouping && menuItem.IsSeparator && !isCategoryVisible)
            continue;
          if (Control.EnableGrouping)
            isCategoryVisible = true;
          if (isFirstItem)
            isFirstItem = false;
          else
            script.AppendFormat (",\r\n");
          AppendMenuItem (script, menuItem, Control.MenuItems.IndexOf (menuItem));
        }
        script.Append (" )"); // Close Array
        script.Append (" )"); // Close new MenuInfo
        script.Append (" );"); // Close AddMenuInfo
        Control.Page.ClientScript.RegisterStartupScriptBlock (Control, typeof (DropDownMenuRenderer), key, script.ToString ());
      }
    }

    private void AppendMenuItem (StringBuilder stringBuilder, WebMenuItem menuItem, int menuItemIndex)
    {
      string href = "null";
      string target = "null";

      bool isCommandEnabled = true;
      if (menuItem.Command != null)
      {
        bool isActive = menuItem.Command.Show == CommandShow.Always
                        || Control.IsReadOnly && menuItem.Command.Show == CommandShow.ReadOnly
                        || !Control.IsReadOnly && menuItem.Command.Show == CommandShow.EditMode;

        isCommandEnabled = isActive && menuItem.Command.Type != CommandType.None;
        if (isCommandEnabled)
        {
          bool isPostBackCommand = menuItem.Command.Type == CommandType.Event
                                   || menuItem.Command.Type == CommandType.WxeFunction;
          if (isPostBackCommand)
          {
            // Clientside script creates an anchor with href="#" and onclick=function
            string argument = menuItemIndex.ToString ();
            href = Control.Page.ClientScript.GetPostBackClientHyperlink (Control, argument);
            href = ScriptUtility.EscapeClientScript (href);
            href = "'" + href + "'";
          }
          else if (menuItem.Command.Type == CommandType.Href)
          {
            href = menuItem.Command.HrefCommand.FormatHref (menuItemIndex.ToString (), menuItem.ItemID);
            if (Control is Control)
              href = UrlUtility.GetAbsoluteUrl (((Control) Control).Page, href);
            href = "'" + href + "'";
            target = "'" + menuItem.Command.HrefCommand.Target + "'";
          }
        }
      }

      bool showIcon = menuItem.Style == WebMenuItemStyle.Icon || menuItem.Style == WebMenuItemStyle.IconAndText;
      bool showText = menuItem.Style == WebMenuItemStyle.Text || menuItem.Style == WebMenuItemStyle.IconAndText;

      string icon = GetIconUrl (menuItem, showIcon);
      string disabledIcon = GetDisabledIconUrl (menuItem, showIcon);
      string text = showText ? "'" + menuItem.Text + "'" : "null";

      bool isDisabled = !menuItem.EvaluateEnabled () || !isCommandEnabled;
      stringBuilder.AppendFormat (
          "\t\tnew DropDownMenu_ItemInfo ('{0}', '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8})",
          menuItemIndex,
          menuItem.Category,
          text,
          icon,
          disabledIcon,
          (int) menuItem.RequiredSelection,
          isDisabled ? "true" : "false",
          href,
          target);
    }

    protected virtual string GetIconUrl (WebMenuItem menuItem, bool showIcon)
    {
      string icon = "null";

      if (showIcon && menuItem.Icon.HasRenderingInformation)
      {
        string url = menuItem.Icon.Url;
        icon = "'" + UrlUtility.ResolveUrl (url) + "'";
      }
      return icon;
    }

    protected virtual string GetDisabledIconUrl (WebMenuItem menuItem, bool showIcon)
    {
      string disabledIcon = "null";
      if (showIcon && menuItem.DisabledIcon.HasRenderingInformation)
      {
        string url = menuItem.DisabledIcon.Url;
        disabledIcon = "'" + UrlUtility.ResolveUrl (url) + "'";
      }
      return disabledIcon;
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