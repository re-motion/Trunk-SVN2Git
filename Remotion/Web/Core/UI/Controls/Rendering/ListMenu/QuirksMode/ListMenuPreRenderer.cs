// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.Rendering.ListMenu.QuirksMode
{
  /// <summary>
  /// Responsible for registering scripts and the style sheet for <see cref="ListMenu"/> controls in quirks mode.
  /// <seealso cref="IListMenu"/>
  /// </summary>
  public class ListMenuPreRenderer : PreRendererBase<IListMenu>, IListMenuPreRenderer
  {
    public ListMenuPreRenderer (IHttpContext context, IListMenu control)
        : base (context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      string scriptFileKey = typeof (IListMenu).FullName + "_Script";
      string scriptFileUrl = ResourceUrlResolver.GetResourceUrl (Control, GetType (), ResourceType.Html, ResourceTheme.Legacy, "ListMenu.js");
      htmlHeadAppender.RegisterJavaScriptInclude (scriptFileKey, scriptFileUrl);

      string styleSheetKey = typeof (IListMenu).FullName + "_Style";
      string styleSheetUrl = ResourceUrlResolver.GetResourceUrl (Control, GetType (), ResourceType.Html, ResourceTheme.Legacy, "ListMenu.css");
      htmlHeadAppender.RegisterStylesheetLink (styleSheetKey, styleSheetUrl);
    }

    public override void PreRender ()
    {
      if (!Control.HasClientScript)
        return;
      
      WebMenuItem[] groupedListMenuItems = Control.MenuItems.GroupMenuItems (false);

      string key = Control.UniqueID + "_MenuItems";
      if (!Control.Page.ClientScript.IsStartupScriptRegistered (key))
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
            Control.GetSelectionCount);
        Control.Page.ClientScript.RegisterStartupScriptBlock (Control, key, script.ToString ());
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

            if (Control is Control)
            {
              menuItem.Command.RegisterForSynchronousPostBack (
                  (Control) Control, argument, string.Format ("ListMenu '{0}', ListMenuItem '{1}'", Control.ID, menuItem.ItemID));
            }
          }
          else if (menuItem.Command.Type == CommandType.Href)
          {
            href = menuItem.Command.HrefCommand.FormatHref (menuItemIndex.ToString (), menuItem.ItemID);
            if ((Control is Control) && !ControlHelper.IsDesignMode (Control, Context.WrappedInstance))
              href = UrlUtility.GetAbsoluteUrl (((Control)Control).Page, href);
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