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
using Remotion.Web.Infrastructure;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.Rendering.DropDownMenu.QuirksMode
{
  public class DropDownMenuPreRenderer : PreRendererBase<IDropDownMenu>, IDropDownMenuPreRenderer
  {
    public DropDownMenuPreRenderer (IHttpContext context, IDropDownMenu control)
        : base(context, control)
    {
    }

    public override void PreRender ()
    {
      //  Startup script initalizing the global values of the script.
      string key = typeof (IDropDownMenu).FullName + "_Startup";
      if (!Control.Page.ClientScript.IsStartupScriptRegistered (key))
      {
        string styleSheetUrl = ResourceUrlResolver.GetResourceUrl (Control, Context, typeof (IDropDownMenu), ResourceType.Html, "DropDownMenu.css");
        string script = string.Format ("DropDownMenu_InitializeGlobals ('{0}');", styleSheetUrl);
        Control.Page.ClientScript.RegisterStartupScriptBlock (Control, key, script);
      }

      key = Control.UniqueID;
      if (Control.Enabled
          && !Control.Page.ClientScript.IsStartupScriptRegistered (key))
      {
        StringBuilder script = new StringBuilder ();
        script.Append ("DropDownMenu_AddMenuInfo (\r\n\t");
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
        Control.Page.ClientScript.RegisterStartupScriptBlock (Control, key, script.ToString ());
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

            if( Control is Control )
            menuItem.Command.RegisterForSynchronousPostBack (
                ((Control)Control), argument, string.Format ("DropDownMenu '{0}', MenuItem '{1}'", Control.ID, menuItem.ItemID));
          }
          else if (menuItem.Command.Type == CommandType.Href)
          {
            href = menuItem.Command.HrefCommand.FormatHref (menuItemIndex.ToString (), menuItem.ItemID);
            if (Control.IsDesignMode)
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
  }
}