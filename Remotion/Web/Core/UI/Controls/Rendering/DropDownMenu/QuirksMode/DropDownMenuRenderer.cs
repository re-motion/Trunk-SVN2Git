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

namespace Remotion.Web.UI.Controls.Rendering.DropDownMenu.QuirksMode
{
  /// <summary>
  /// Responsible for rendering a <see cref="DropDownMenu"/> control in quirks mode.
  /// <seealso cref="IDropDownMenu"/>
  /// </summary>
  public class DropDownMenuRenderer : RendererBase<IDropDownMenu>
  {
    private const string c_dropDownIcon = "DropDownMenuArrow.gif";

    public DropDownMenuRenderer (HttpContextBase context, IDropDownMenu control)
        : base(context, control)
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
            Control, Context, typeof (DropDownMenuRenderer), ResourceType.Html, ResourceTheme.Legacy, "DropDownMenu.js");
        htmlHeadAppender.RegisterJavaScriptInclude (key, url);
      }

      key = typeof (DropDownMenuRenderer).FullName + "_Style";
      if (!htmlHeadAppender.IsRegistered (key))
      {
        string styleSheetUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (DropDownMenuRenderer), ResourceType.Html, ResourceTheme.Legacy, "DropDownMenu.css");
        htmlHeadAppender.RegisterStylesheetLink (key, styleSheetUrl, HtmlHeadAppender.Priority.Library);
      }
    }
    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      RegisterMenuItems();

      RegisterEventHandlerScripts();

      AddStandardAttributesToRender (writer);
      writer.AddStyleAttribute (HtmlTextWriterStyle.Display, "inline-block");
      writer.RenderBeginTag (HtmlTextWriterTag.Div);

      //  Menu-Div filling the control's div is required to apply internal css attributes
      //  for position, width and height. This allows the Head and th popup-div to align themselves
      writer.AddStyleAttribute ("position", "relative");
      writer.AddAttribute ("id", Control.MenuHeadClientID);
      writer.RenderBeginTag (HtmlTextWriterTag.Div); // Begin Menu-Div

      RenderHead (writer);

      writer.RenderEndTag (); // End Menu-Div
      writer.RenderEndTag (); // End outer div
    }

    private void RenderHead (HtmlTextWriter writer)
    {
      //  Head-Div is used to group the title and the button, providing a single point of reference
      //  for the popup-div.
      writer.AddStyleAttribute ("position", "relative");
      writer.AddAttribute ("id", Control.ClientID + "_HeadDiv");
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassHead);
      writer.AddAttribute ("OnMouseOver", "DropDownMenu_OnHeadMouseOver (this)");
      writer.AddAttribute ("OnMouseOut", "DropDownMenu_OnHeadMouseOut (this)");
      writer.RenderBeginTag (HtmlTextWriterTag.Div); // Begin Drop Down Head-Div

      writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      writer.AddStyleAttribute ("display", "inline");
      writer.RenderBeginTag (HtmlTextWriterTag.Table); // Begin Drop Down Button table
      writer.RenderBeginTag (HtmlTextWriterTag.Tr);

      RenderHeadTitle (writer);
      RenderHeadButton (writer);

      writer.RenderEndTag();
      writer.RenderEndTag(); // End Drop Down Button table

      writer.RenderEndTag(); // End Drop Down Head-Div
    }

    private void RenderHeadTitle (HtmlTextWriter writer)
    {
      bool hasHeadTitleContents = true;
      if (Control.RenderHeadTitleMethod == null)
      {
        bool hasTitleText = !string.IsNullOrEmpty (Control.TitleText);
        bool hasTitleIcon = Control.TitleIcon != null && !string.IsNullOrEmpty (Control.TitleIcon.Url);
        hasHeadTitleContents = hasTitleText || hasTitleIcon;

        if (hasHeadTitleContents)
        {
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "1%"); //"100%");
          writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassHeadTitle);
          writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td

          if (Control.Enabled)
            writer.RenderBeginTag (HtmlTextWriterTag.A); // Begin title tag
          else
          {
            writer.AddStyleAttribute (HtmlTextWriterStyle.Color, "GrayText");
            writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin title tag
          }
          RenderIcon (writer, Control.TitleIcon);
          writer.Write (Control.TitleText);
          writer.RenderEndTag(); // End title tag

          writer.RenderEndTag(); // End td
        }
      }
      else
        Control.RenderHeadTitleMethod (writer);

      if (hasHeadTitleContents)
      {
        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
        writer.AddStyleAttribute ("padding-right", "0.3em");
        writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td
        writer.RenderEndTag();
      }
    }

    private void RenderIcon (HtmlTextWriter writer, IconInfo icon)
    {
      if (icon == null || string.IsNullOrEmpty (icon.Url))
        return;

      writer.AddAttribute (HtmlTextWriterAttribute.Src, icon.Url);
      if (!icon.Width.IsEmpty && !icon.Height.IsEmpty)
      {
        writer.AddAttribute (HtmlTextWriterAttribute.Width, icon.Width.ToString());
        writer.AddAttribute (HtmlTextWriterAttribute.Height, icon.Height.ToString());
      }
      writer.AddStyleAttribute ("vertical-align", "middle");
      writer.AddStyleAttribute (HtmlTextWriterStyle.BorderStyle, "none");
      writer.AddStyleAttribute ("margin-right", "0.3em");
      writer.RenderBeginTag (HtmlTextWriterTag.Img);
      writer.RenderEndTag();
    }

    private void RenderHeadButton (HtmlTextWriter writer)
    {
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
      writer.AddStyleAttribute ("text-align", "center");
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassHeadButton);
      writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td

      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "1em");
      writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
      writer.RenderBeginTag (HtmlTextWriterTag.A); // Begin anchor

      writer.AddStyleAttribute ("vertical-align", "middle");
      writer.AddStyleAttribute (HtmlTextWriterStyle.BorderStyle, "none");
      string url = ResourceUrlResolver.GetResourceUrl (Control, Context, typeof (IDropDownMenu), ResourceType.Image, ResourceTheme, c_dropDownIcon);
      writer.AddAttribute (HtmlTextWriterAttribute.Src, url);
      writer.AddAttribute (HtmlTextWriterAttribute.Alt, string.Empty);
      writer.RenderBeginTag (HtmlTextWriterTag.Img);
      writer.RenderEndTag(); // End img

      writer.RenderEndTag(); // End anchor

      writer.RenderEndTag(); // End td
    }

    private void RegisterEventHandlerScripts ()
    {
      string key = typeof (IDropDownMenu).FullName + "_Startup";

      if (!Control.Page.ClientScript.IsStartupScriptRegistered (typeof (DropDownMenuClientScriptBehavior), key))
      {
        string styleSheetUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (DropDownMenuRenderer), ResourceType.Html, ResourceTheme.Legacy, "DropDownMenu.css");
        string script = string.Format ("DropDownMenu_InitializeGlobals ('{0}');", styleSheetUrl);
        Control.Page.ClientScript.RegisterStartupScriptBlock (Control, typeof (DropDownMenuClientScriptBehavior), key, script);
      }

      if (Control.Enabled && Control.Visible && Control.Mode == MenuMode.DropDownMenu)
      {
        key = Control.ClientID + "_ClickEventHandlerBindScript";
        if (!Control.Page.ClientScript.IsStartupScriptRegistered (typeof (DropDownMenuClientScriptBehavior), key))
        {
          string elementReference = string.Format ("document.getElementById('{0}')", Control.MenuHeadClientID);
          string menuIDReference = string.Format ("'{0}'", Control.ClientID);
          string script = Control.GetBindOpenEventScript (elementReference, menuIDReference, false);
          Control.Page.ClientScript.RegisterStartupScriptBlock (Control, typeof (DropDownMenuClientScriptBehavior), key, script);
        }
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

    protected virtual string CssClassHead
    {
      get { return "dropDownMenuHead"; }
    }

    protected virtual string CssClassHeadFocus
    {
      get { return "dropDownMenuHeadFocus"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="DropDownMenu"/>'s title. </summary>
    /// <remarks> Class: <c></c> </remarks>
    protected virtual string CssClassHeadTitle
    {
      get { return "dropDownMenuHeadTitle"; }
    }

    protected virtual string CssClassHeadTitleFocus
    {
      get { return "dropDownMenuHeadTitleFocus"; }
    }

    protected virtual string CssClassHeadButton
    {
      get { return "dropDownMenuHeadButton"; }
    }

    protected virtual string CssClassMenuButtonFocus
    {
      get { return "dropDownMenuButtonFocus"; }
    }

    protected virtual string CssClassPopUp
    {
      get { return "dropDownMenuPopUp"; }
    }

    protected virtual string CssClassItem
    {
      get { return "dropDownMenuItem"; }
    }

    protected virtual string CssClassItemFocus
    {
      get { return "dropDownMenuItemFocus"; }
    }

    protected virtual string CssClassItemTextPane
    {
      get { return "dropDownMenuItemTextPane"; }
    }

    protected virtual string CssClassItemIconPane
    {
      get { return "dropDownMenuItemIconPane"; }
    }
  }
}
