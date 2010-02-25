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
using Remotion.Utilities;
using System.Web;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.Rendering.DropDownMenu
{
  /// <summary>
  /// Responsible for registering scripts and the style sheet for <see cref="DropDownMenu"/> controls.
  /// <seealso cref="IDropDownMenu"/>
  /// </summary>
  public abstract class DropDownMenuPreRendererBase : IDropDownMenuPreRenderer
  {
    private readonly HttpContextBase _context;
    private readonly IDropDownMenu _control;
    
    protected DropDownMenuPreRendererBase (HttpContextBase context, IDropDownMenu control)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("control", control);

      _control = control;
      _context = context;
    }

    /// <summary>Gets the <see cref="HttpContextBase"/> that contains the response for which this renderer generates output.</summary>
    public HttpContextBase Context
    {
      get { return _context; }
    }

    /// <summary>Gets the control that will be rendered.</summary>
    public IDropDownMenu Control
    {
      get { return _control; }
    }

    public void PreRender ()
    {
      string key = Control.UniqueID;
      if (Control.Enabled && !Control.Page.ClientScript.IsStartupScriptRegistered (typeof (DropDownMenuPreRendererBase), key))
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
        Control.Page.ClientScript.RegisterStartupScriptBlock (Control, typeof (DropDownMenuPreRendererBase), key, script.ToString ());
      }
    }

    public abstract bool GetBrowserCapableOfScripting ();

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
            if( Control is Control )
              href = UrlUtility.GetAbsoluteUrl (((Control)Control).Page, href);
            href = "'" + href + "'";
            target = "'" + menuItem.Command.HrefCommand.Target + "'";
          }
        }
      }

      bool showIcon = menuItem.Style == WebMenuItemStyle.Icon || menuItem.Style == WebMenuItemStyle.IconAndText;
      bool showText = menuItem.Style == WebMenuItemStyle.Text || menuItem.Style == WebMenuItemStyle.IconAndText;
      
      string icon = GetIconUrl(menuItem, showIcon);
      string disabledIcon = GetDisabledIconUrl(menuItem, showIcon);
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
        string url =  menuItem.Icon.Url;
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
  }
}
