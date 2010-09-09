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

    public DropDownMenuRenderer (HttpContextBase context, IDropDownMenu control, IResourceUrlFactory resourceUrlFactory)
      : base (context, control, resourceUrlFactory)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude();
      htmlHeadAppender.RegisterJQueryIFrameShimJavaScriptInclude ();

      string scriptKey = typeof (DropDownMenuRenderer).FullName + "_Script";
      var scriptUrl = ResourceUrlFactory.CreateResourceUrl (typeof (DropDownMenuRenderer), ResourceType.Html, "DropDownMenu.js");
      htmlHeadAppender.RegisterJavaScriptInclude (scriptKey, scriptUrl);

      string styleSheetKey = typeof (DropDownMenuRenderer).FullName + "_Style";
      var styleSheetUrl = ResourceUrlFactory.CreateThemedResourceUrl (typeof (DropDownMenuRenderer), ResourceType.Html, "DropDownMenu.css");
      htmlHeadAppender.RegisterStylesheetLink (styleSheetKey, styleSheetUrl, HtmlHeadAppender.Priority.Library);
    }

    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      Render (new DropDownMenuRenderingContext (Context, writer, Control));
    }

    public void Render (DropDownMenuRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      RegisterMenuItems (renderingContext);

      RegisterEventHandlerScripts (renderingContext);

      AddAttributesToRender (renderingContext);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      RenderTitle (renderingContext);

      renderingContext.Writer.RenderEndTag ();
    }

    private void RenderTitle (DropDownMenuRenderingContext renderingContext)
    {
      string cssClass = CssClassHead;
      if (!renderingContext.Control.Enabled)
        cssClass += " " + CssClassDisabled;
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      if (renderingContext.Control.RenderHeadTitleMethod != null)
        renderingContext.Control.RenderHeadTitleMethod (renderingContext.Writer);
      else
        RenderDefaultTitle (renderingContext);

      RenderDropdownButton (renderingContext);

      renderingContext.Writer.RenderEndTag ();
    }

    private void RenderDefaultTitle (DropDownMenuRenderingContext renderingContext)
    {
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, "return false;");
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.A);

      if (renderingContext.Control.TitleIcon != null && !string.IsNullOrEmpty (renderingContext.Control.TitleIcon.Url))
        renderingContext.Control.TitleIcon.Render (renderingContext.Writer, renderingContext.Control);

      if (!string.IsNullOrEmpty (renderingContext.Control.TitleText))
      {
        renderingContext.Writer.Write (renderingContext.Control.TitleText);
        renderingContext.Writer.Write (c_whiteSpace);
      }
      renderingContext.Writer.RenderEndTag ();
    }

    private void RenderDropdownButton (DropDownMenuRenderingContext renderingContext)
    {
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassDropDownButton);
      var imageUrl = ResourceUrlFactory.CreateThemedResourceUrl (
          typeof (DropDownMenuRenderer), ResourceType.Image, renderingContext.Control.Enabled ? c_dropDownIcon : c_dropDownIconDisabled);

      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.BackgroundImage, string.Format ("url({0})", imageUrl.GetUrl ()));
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      IconInfo.Spacer.Render (renderingContext.Writer, renderingContext.Control);

      renderingContext.Writer.RenderEndTag ();
    }

    protected string CssClassDropDownButton
    {
      get { return "DropDownMenuButton"; }
    }

    private void AddAttributesToRender (DropDownMenuRenderingContext renderingContext)
    {
      AddStandardAttributesToRender (renderingContext);
      if (string.IsNullOrEmpty (renderingContext.Control.CssClass) && string.IsNullOrEmpty (renderingContext.Control.Attributes["class"]))
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassBase);

      if (renderingContext.Control.ControlStyle.Width.IsEmpty)
      {
        if (!renderingContext.Control.Width.IsEmpty)
          renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, renderingContext.Control.Width.ToString ());
      }
    }

    private void RegisterEventHandlerScripts (DropDownMenuRenderingContext renderingContext)
    {
      if (!renderingContext.Control.Enabled)
        return;

      string key = renderingContext.Control.ClientID + "_KeyDownEventHandlerBindScript";
      string getSelectionCount = (string.IsNullOrEmpty (renderingContext.Control.GetSelectionCount) ? "null" : renderingContext.Control.GetSelectionCount);
      string script = string.Format (
          "$('#{0}').keydown( function(event){{ DropDownMenu_OnKeyDown(event, document.getElementById('{0}'), {1}); }} );",
          renderingContext.Control.ClientID,
          getSelectionCount);

      renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock (renderingContext.Control, typeof (ClientScriptBehavior), key, script);

      if (renderingContext.Control.Enabled && renderingContext.Control.Visible && renderingContext.Control.Mode == MenuMode.DropDownMenu)
      {
        key = renderingContext.Control.ClientID + "_ClickEventHandlerBindScript";
        string elementReference = string.Format ("$('#{0}')", renderingContext.Control.ClientID);
        string menuIDReference = string.Format ("'{0}'", renderingContext.Control.ClientID);
        script = renderingContext.Control.GetBindOpenEventScript (elementReference, menuIDReference, false);
        renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock (renderingContext.Control, typeof (ClientScriptBehavior), key, script);
      }
    }

    private void RegisterMenuItems (DropDownMenuRenderingContext renderingContext)
    {
      string key = renderingContext.Control.UniqueID;
      if (renderingContext.Control.Enabled && !renderingContext.Control.Page.ClientScript.IsStartupScriptRegistered (typeof (DropDownMenuRenderer), key))
      {
        StringBuilder script = new StringBuilder ();
        script.Append ("DropDownMenu_AddMenuInfo" + " (\r\n\t");
        script.AppendFormat ("new DropDownMenu_MenuInfo ('{0}', new Array (\r\n", renderingContext.Control.ClientID);
        bool isFirstItem = true;

        WebMenuItem[] menuItems;
        if (renderingContext.Control.EnableGrouping)
          menuItems = renderingContext.Control.MenuItems.GroupMenuItems (true);
        else
          menuItems = renderingContext.Control.MenuItems.ToArray ();

        string category = null;
        bool isCategoryVisible = false;
        for (int i = 0; i < menuItems.Length; i++)
        {
          WebMenuItem menuItem = menuItems[i];
          if (renderingContext.Control.EnableGrouping && category != menuItem.Category)
          {
            category = menuItem.Category;
            isCategoryVisible = false;
          }
          if (!menuItem.EvaluateVisible ())
            continue;
          if (renderingContext.Control.EnableGrouping && menuItem.IsSeparator && !isCategoryVisible)
            continue;
          if (renderingContext.Control.EnableGrouping)
            isCategoryVisible = true;
          if (isFirstItem)
            isFirstItem = false;
          else
            script.AppendFormat (",\r\n");
          AppendMenuItem (renderingContext, script, menuItem, renderingContext.Control.MenuItems.IndexOf (menuItem));
        }
        script.Append (" )"); // Close Array
        script.Append (" )"); // Close new MenuInfo
        script.Append (" );"); // Close AddMenuInfo
        renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock (renderingContext.Control, typeof (DropDownMenuRenderer), key, script.ToString ());
      }
    }

    private void AppendMenuItem (DropDownMenuRenderingContext renderingContext, StringBuilder stringBuilder, WebMenuItem menuItem, int menuItemIndex)
    {
      string href = "null";
      string target = "null";

      bool isCommandEnabled = true;
      if (menuItem.Command != null)
      {
        bool isActive = menuItem.Command.Show == CommandShow.Always
                        || renderingContext.Control.IsReadOnly && menuItem.Command.Show == CommandShow.ReadOnly
                        || !renderingContext.Control.IsReadOnly && menuItem.Command.Show == CommandShow.EditMode;

        isCommandEnabled = isActive && menuItem.Command.Type != CommandType.None;
        if (isCommandEnabled)
        {
          bool isPostBackCommand = menuItem.Command.Type == CommandType.Event
                                   || menuItem.Command.Type == CommandType.WxeFunction;
          if (isPostBackCommand)
          {
            // Clientside script creates an anchor with href="#" and onclick=function
            string argument = menuItemIndex.ToString ();
            href = renderingContext.Control.Page.ClientScript.GetPostBackClientHyperlink (Control, argument);
            href = ScriptUtility.EscapeClientScript (href);
            href = "'" + href + "'";
          }
          else if (menuItem.Command.Type == CommandType.Href)
          {
            href = menuItem.Command.HrefCommand.FormatHref (menuItemIndex.ToString (), menuItem.ItemID);
            href = "'" + renderingContext.Control.ResolveClientUrl (href) + "'";
            target = "'" + menuItem.Command.HrefCommand.Target + "'";
          }
        }
      }

      bool showIcon = menuItem.Style == WebMenuItemStyle.Icon || menuItem.Style == WebMenuItemStyle.IconAndText;
      bool showText = menuItem.Style == WebMenuItemStyle.Text || menuItem.Style == WebMenuItemStyle.IconAndText;

      string icon = GetIconUrl (renderingContext, menuItem, showIcon);
      string disabledIcon = GetDisabledIconUrl (renderingContext, menuItem, showIcon);
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

    protected virtual string GetIconUrl (DropDownMenuRenderingContext renderingContext, WebMenuItem menuItem, bool showIcon)
    {
      string icon = "null";

      if (showIcon && menuItem.Icon.HasRenderingInformation)
      {
        string url = menuItem.Icon.Url;
        icon = "'" + renderingContext.Control.ResolveClientUrl (url) + "'";
      }
      return icon;
    }

    protected virtual string GetDisabledIconUrl (DropDownMenuRenderingContext renderingContext, WebMenuItem menuItem, bool showIcon)
    {
      string disabledIcon = "null";
      if (showIcon && menuItem.DisabledIcon.HasRenderingInformation)
      {
        string url = menuItem.DisabledIcon.Url;
        disabledIcon = "'" + renderingContext.Control.ResolveClientUrl (url) + "'";
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