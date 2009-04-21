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
