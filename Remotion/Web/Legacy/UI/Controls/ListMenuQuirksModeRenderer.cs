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
using System.Web;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.ListMenuImplementation;
using Remotion.Web.Utilities;

namespace Remotion.Web.Legacy.UI.Controls
{
  /// <summary>
  /// Responsible for registering scripts and the style sheet for <see cref="ListMenu"/> controls in quirks mode.
  /// <seealso cref="IListMenu"/>
  /// </summary>
  public class ListMenuQuirksModeRenderer : QuirksModeRendererBase<IListMenu>
  {
    protected const string c_whiteSpace = "&nbsp;";

    public ListMenuQuirksModeRenderer (HttpContextBase context, IListMenu control)
        : base (context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      // Do not call base implementation
      //base.RegisterHtmlHeadContents

      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude ();

      string scriptFileKey = typeof (ListMenuQuirksModeRenderer).FullName + "_Script";
      string scriptFileUrl = ResourceUrlResolver.GetResourceUrl (
          Control, typeof (ListMenuQuirksModeRenderer), ResourceType.Html, "ListMenu.js");
      htmlHeadAppender.RegisterJavaScriptInclude (scriptFileKey, scriptFileUrl);

      string styleSheetKey = typeof (ListMenuQuirksModeRenderer).FullName + "_Style";
      string styleSheetUrl = ResourceUrlResolver.GetResourceUrl (
          Control, typeof (ListMenuQuirksModeRenderer), ResourceType.Html, "ListMenu.css");
      htmlHeadAppender.RegisterStylesheetLink (styleSheetKey, styleSheetUrl, HtmlHeadAppender.Priority.Library);
    }

    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      RegisterMenuItems ();

      WebMenuItem[] groupedListMenuItems = Control.MenuItems.GroupMenuItems (false);

      writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ClientID);
      writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      writer.AddAttribute (HtmlTextWriterAttribute.Border, "0");
      writer.RenderBeginTag (HtmlTextWriterTag.Table);
      bool isFirstItem = true;
      for (int idxItems = 0; idxItems < groupedListMenuItems.Length; idxItems++)
      {
        WebMenuItem currentItem = groupedListMenuItems[idxItems];
        if (!currentItem.EvaluateVisible ())
          continue;

        bool isLastItem = (idxItems == (groupedListMenuItems.Length - 1));
        bool isFirstCategoryItem = (isFirstItem || (groupedListMenuItems[idxItems - 1].Category != currentItem.Category));
        bool isLastCategoryItem = (isLastItem || (groupedListMenuItems[idxItems + 1].Category != currentItem.Category));
        bool hasAlwaysLineBreaks = (Control.LineBreaks == ListMenuLineBreaks.All);
        bool hasCategoryLineBreaks = (Control.LineBreaks == ListMenuLineBreaks.BetweenGroups);

        if (hasAlwaysLineBreaks || (hasCategoryLineBreaks && isFirstCategoryItem) || isFirstItem)
        {
          writer.RenderBeginTag (HtmlTextWriterTag.Tr);
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
          writer.AddAttribute (HtmlTextWriterAttribute.Class, "listMenuRow");
          writer.RenderBeginTag (HtmlTextWriterTag.Td);
        }
        RenderListMenuItem (writer, currentItem, Control.ClientID, Control.MenuItems.IndexOf (currentItem));
        if (hasAlwaysLineBreaks || (hasCategoryLineBreaks && isLastCategoryItem) || isLastItem)
        {
          writer.RenderEndTag ();
          writer.RenderEndTag ();
        }

        if (isFirstItem)
          isFirstItem = false;
      }
      writer.RenderEndTag ();
    }

    private void RenderListMenuItem (HtmlTextWriter writer, WebMenuItem menuItem, string menuID, int index)
    {
      bool showIcon = menuItem.Style == WebMenuItemStyle.Icon || menuItem.Style == WebMenuItemStyle.IconAndText;
      bool showText = menuItem.Style == WebMenuItemStyle.Text || menuItem.Style == WebMenuItemStyle.IconAndText;

      writer.AddAttribute (HtmlTextWriterAttribute.Id, menuID + "_" + index);
      writer.AddAttribute (HtmlTextWriterAttribute.Class, "listMenuItem");
      writer.RenderBeginTag (HtmlTextWriterTag.Span);
      if (!menuItem.IsDisabled)
      {
        menuItem.Command.RenderBegin (
            writer, Control.Page.ClientScript.GetPostBackClientHyperlink (Control, index.ToString ()), new[] { index.ToString () }, "", null);
      }
      else
        writer.RenderBeginTag (HtmlTextWriterTag.A);

      if (showIcon && menuItem.Icon.HasRenderingInformation)
      {
        writer.AddAttribute (HtmlTextWriterAttribute.Src, UrlUtility.ResolveUrl (menuItem.Icon.Url));
        writer.AddAttribute (HtmlTextWriterAttribute.Alt, StringUtility.NullToEmpty (menuItem.Icon.AlternateText));
        writer.AddStyleAttribute ("vertical-align", "middle");
        writer.AddStyleAttribute (HtmlTextWriterStyle.BorderStyle, "none");
        writer.RenderBeginTag (HtmlTextWriterTag.Img);
        writer.RenderEndTag ();
        if (showText)
          writer.Write (c_whiteSpace);
      }
      if (showText)
        writer.Write (menuItem.Text); // Do not HTML encode.
      writer.RenderEndTag ();
      writer.RenderEndTag ();
    }

    private void RegisterMenuItems ()
    {
      if (!Control.HasClientScript)
        return;

      WebMenuItem[] groupedListMenuItems = Control.MenuItems.GroupMenuItems (false);

      string key = Control.UniqueID + "_MenuItems";
      if (!Control.Page.ClientScript.IsStartupScriptRegistered (typeof (ListMenuQuirksModeRenderer), key))
      {
        StringBuilder script = new StringBuilder ();
        script.AppendFormat ("ListMenu_AddMenuInfo (document.getElementById ('{0}'), \r\n\t", Control.ClientID);
        script.AppendFormat ("new ListMenu_MenuInfo ('{0}', new Array (\r\n", Control.ClientID);
        bool isFirstItemInGroup = true;

        for (int idxItems = 0; idxItems < groupedListMenuItems.Length; idxItems++)
        {
          WebMenuItem currentItem = groupedListMenuItems[idxItems];
          if (!currentItem.EvaluateVisible ())
            continue;

          if (isFirstItemInGroup)
            isFirstItemInGroup = false;
          else
            script.AppendFormat (",\r\n");
          AppendListMenuItem (script, currentItem);
        }
        script.Append (" )"); // Close Array
        script.Append (" )"); // Close new MenuInfo
        script.Append (" );\r\n"); // Close AddMenuInfo

        script.AppendFormat (
            "ListMenu_Update ( document.getElementById ('{0}'), {1} );",
            Control.ClientID,
            string.IsNullOrEmpty (Control.GetSelectionCount) ? "null" : Control.GetSelectionCount);
        Control.Page.ClientScript.RegisterStartupScriptBlock (Control, typeof (ListMenuQuirksModeRenderer), key, script.ToString ());
      }
    }

    private void AppendListMenuItem (StringBuilder stringBuilder, WebMenuItem menuItem)
    {
      int menuItemIndex = Control.MenuItems.IndexOf (menuItem);
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
            href = Control.Page.ClientScript.GetPostBackClientHyperlink (Control, argument) + ";";
            href = ScriptUtility.EscapeClientScript (href);
            href = "'" + href + "'";
          }
          else if (menuItem.Command.Type == CommandType.Href)
          {
            href = menuItem.Command.HrefCommand.FormatHref (menuItemIndex.ToString (), menuItem.ItemID);
            if ((Control is Control) && !ControlHelper.IsDesignMode (Control))
              href = UrlUtility.GetAbsoluteUrl (((Control) Control).Page, href);
            href = "'" + href + "'";
            target = "'" + menuItem.Command.HrefCommand.Target + "'";
          }
        }
      }

      bool showIcon = menuItem.Style == WebMenuItemStyle.Icon || menuItem.Style == WebMenuItemStyle.IconAndText;
      bool showText = menuItem.Style == WebMenuItemStyle.Text || menuItem.Style == WebMenuItemStyle.IconAndText;
      string icon = "null";
      if (showIcon && menuItem.Icon.HasRenderingInformation)
        icon = "'" + UrlUtility.ResolveUrl (menuItem.Icon.Url) + "'";
      string disabledIcon = "null";
      if (showIcon && menuItem.DisabledIcon.HasRenderingInformation)
        disabledIcon = "'" + UrlUtility.ResolveUrl (menuItem.DisabledIcon.Url) + "'";
      string text = showText ? "'" + menuItem.Text + "'" : "null";

      bool isDisabled = !Control.Enabled
                        || !menuItem.EvaluateEnabled ()
                        || !isCommandEnabled;
      stringBuilder.AppendFormat (
          "\t\tnew ListMenuItemInfo ('{0}', '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8})",
          Control.ClientID + "_" + menuItemIndex,
          menuItem.Category,
          text,
          icon,
          disabledIcon,
          (int) menuItem.RequiredSelection,
          isDisabled ? "true" : "false",
          href,
          target);
    }
  }
}