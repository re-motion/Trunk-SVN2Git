/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Sample
{
  public class TestBocTreeViewMenuItemProvider : BocTreeViewMenuItemProvider
  {
    public TestBocTreeViewMenuItemProvider ()
    {
    }

    public override WebMenuItem[] InitalizeMenuItems (WebTreeNode node)
    {
      ArgumentUtility.CheckNotNull ("node", node);

      WebMenuItem eventMenuItem = new WebMenuItem();
      eventMenuItem.Text = "Event";
      eventMenuItem.Command.Type = CommandType.Event;

      WebMenuItem wxeMenuItem = new WebMenuItem();
      wxeMenuItem.Text = "WXE";
      wxeMenuItem.Command.Type = CommandType.WxeFunction;
      wxeMenuItem.Command.WxeFunctionCommand.TypeName = TypeUtility.GetPartialAssemblyQualifiedName (typeof (TestWxeFunction));

      WebMenuItem[] menuItems = new WebMenuItem[] {eventMenuItem, wxeMenuItem};
      return menuItems;
    }

    public override void PreRenderMenuItems (WebTreeNode node, WebMenuItemCollection menuItems)
    {
      base.PreRenderMenuItems (node, menuItems);
    }

    public override void OnMenuItemEventCommandClick (WebMenuItem menuItem, WebTreeNode node)
    {
      base.OnMenuItemEventCommandClick (menuItem, node);
    }

    public override void OnMenuItemWxeFunctionCommandClick (WebMenuItem menuItem, WebTreeNode node)
    {
      base.OnMenuItemWxeFunctionCommandClick (menuItem, node);
    }
  }
}
