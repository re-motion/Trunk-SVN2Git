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
using Remotion.Web.UI.Controls.ListMenuImplementation.Rendering;
using Remotion.Web.Utilities;

namespace Remotion.Web.Legacy.UI.Controls
{
  /// <summary>
  /// Responsible for registering scripts and the style sheet for <see cref="ListMenu"/> controls in quirks mode.
  /// <seealso cref="IListMenu"/>
  /// </summary>
  public class ListMenuQuirksModeRenderer : QuirksModeRendererBase<IListMenu>, IListMenuRenderer
  {
    protected const string c_whiteSpace = "&nbsp;";

    public ListMenuQuirksModeRenderer ()
        : base (null, null)
    {
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender, IControl control, HttpContextBase context)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      // Do not call base implementation
      //base.RegisterHtmlHeadContents

      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude ();

      string scriptFileKey = typeof (ListMenuQuirksModeRenderer).FullName + "_Script";
      string scriptFileUrl = ResourceUrlResolver.GetResourceUrl (
          control, typeof (ListMenuQuirksModeRenderer), ResourceType.Html, "ListMenu.js");
      htmlHeadAppender.RegisterJavaScriptInclude (scriptFileKey, scriptFileUrl);

      string styleSheetKey = typeof (ListMenuQuirksModeRenderer).FullName + "_Style";
      string styleSheetUrl = ResourceUrlResolver.GetResourceUrl (
          control, typeof (ListMenuQuirksModeRenderer), ResourceType.Html, "ListMenu.css");
      htmlHeadAppender.RegisterStylesheetLink (styleSheetKey, styleSheetUrl, HtmlHeadAppender.Priority.Library);
    }

    public void Render (ListMenuRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      RegisterMenuItems (renderingContext);

      WebMenuItem[] groupedListMenuItems = renderingContext.Control.MenuItems.GroupMenuItems (false);

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Id, renderingContext.Control.ClientID);
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Border, "0");
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Table);
      bool isFirstItem = true;
      for (int idxItems = 0; idxItems < groupedListMenuItems.Length; idxItems++)
      {
        WebMenuItem currentItem = groupedListMenuItems[idxItems];
        if (!currentItem.EvaluateVisible ())
          continue;

        bool isLastItem = (idxItems == (groupedListMenuItems.Length - 1));
        bool isFirstCategoryItem = (isFirstItem || (groupedListMenuItems[idxItems - 1].Category != currentItem.Category));
        bool isLastCategoryItem = (isLastItem || (groupedListMenuItems[idxItems + 1].Category != currentItem.Category));
        bool hasAlwaysLineBreaks = (renderingContext.Control.LineBreaks == ListMenuLineBreaks.All);
        bool hasCategoryLineBreaks = (renderingContext.Control.LineBreaks == ListMenuLineBreaks.BetweenGroups);

        if (hasAlwaysLineBreaks || (hasCategoryLineBreaks && isFirstCategoryItem) || isFirstItem)
        {
          renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Tr);
          renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
          renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, "listMenuRow");
          renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td);
        }
        RenderListMenuItem (renderingContext, currentItem, renderingContext.Control.ClientID, renderingContext.Control.MenuItems.IndexOf (currentItem));
        if (hasAlwaysLineBreaks || (hasCategoryLineBreaks && isLastCategoryItem) || isLastItem)
        {
          renderingContext.Writer.RenderEndTag ();
          renderingContext.Writer.RenderEndTag ();
        }

        if (isFirstItem)
          isFirstItem = false;
      }
      renderingContext.Writer.RenderEndTag ();
    }

    private void RenderListMenuItem (ListMenuRenderingContext renderingContext, WebMenuItem menuItem, string menuID, int index)
    {
      bool showIcon = menuItem.Style == WebMenuItemStyle.Icon || menuItem.Style == WebMenuItemStyle.IconAndText;
      bool showText = menuItem.Style == WebMenuItemStyle.Text || menuItem.Style == WebMenuItemStyle.IconAndText;

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Id, menuID + "_" + index);
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, "listMenuItem");
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      if (!menuItem.IsDisabled)
      {
        menuItem.Command.RenderBegin (
            renderingContext.Writer, renderingContext.Control.Page.ClientScript.GetPostBackClientHyperlink (renderingContext.Control, index.ToString ()), new[] { index.ToString () }, "", null);
      }
      else
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.A);

      if (showIcon && menuItem.Icon.HasRenderingInformation)
      {
        menuItem.Icon.Render (renderingContext.Writer, renderingContext.Control);
        if (showText)
          renderingContext.Writer.Write (c_whiteSpace);
      }
      if (showText)
        renderingContext.Writer.Write (menuItem.Text); // Do not HTML encode.
      renderingContext.Writer.RenderEndTag ();
      renderingContext.Writer.RenderEndTag ();
    }

    private void RegisterMenuItems (ListMenuRenderingContext renderingContext)
    {
      if (!renderingContext.Control.HasClientScript)
        return;

      WebMenuItem[] groupedListMenuItems = renderingContext.Control.MenuItems.GroupMenuItems (false);

      string key = renderingContext.Control.UniqueID + "_MenuItems";
      if (!renderingContext.Control.Page.ClientScript.IsStartupScriptRegistered (typeof (ListMenuQuirksModeRenderer), key))
      {
        StringBuilder script = new StringBuilder ();
        script.AppendFormat ("ListMenu_AddMenuInfo (document.getElementById ('{0}'), \r\n\t", renderingContext.Control.ClientID);
        script.AppendFormat ("new ListMenu_MenuInfo ('{0}', new Array (\r\n", renderingContext.Control.ClientID);
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
          AppendListMenuItem (renderingContext, script, currentItem);
        }
        script.Append (" )"); // Close Array
        script.Append (" )"); // Close new MenuInfo
        script.Append (" );\r\n"); // Close AddMenuInfo

        script.AppendFormat (
            "ListMenu_Update ( document.getElementById ('{0}'), {1} );",
            renderingContext.Control.ClientID,
            string.IsNullOrEmpty (renderingContext.Control.GetSelectionCount) ? "null" : renderingContext.Control.GetSelectionCount);
        renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock (renderingContext.Control, typeof (ListMenuQuirksModeRenderer), key, script.ToString ());
      }
    }

    private void AppendListMenuItem (ListMenuRenderingContext renderingContext, StringBuilder stringBuilder, WebMenuItem menuItem)
    {
      int menuItemIndex = renderingContext.Control.MenuItems.IndexOf (menuItem);
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
            href = renderingContext.Control.Page.ClientScript.GetPostBackClientHyperlink (renderingContext.Control, argument) + ";";
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
      string icon = "null";
      if (showIcon && menuItem.Icon.HasRenderingInformation)
        icon = "'" + renderingContext.Control.ResolveClientUrl (menuItem.Icon.Url) + "'";
      string disabledIcon = "null";
      if (showIcon && menuItem.DisabledIcon.HasRenderingInformation)
        disabledIcon = "'" + renderingContext.Control.ResolveClientUrl (menuItem.DisabledIcon.Url) + "'";
      string text = showText ? "'" + menuItem.Text + "'" : "null";

      bool isDisabled = !renderingContext.Control.Enabled
                        || !menuItem.EvaluateEnabled ()
                        || !isCommandEnabled;
      stringBuilder.AppendFormat (
          "\t\tnew ListMenuItemInfo ('{0}', '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8})",
          renderingContext.Control.ClientID + "_" + menuItemIndex,
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