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
using System.Web.UI;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UI.Controls
{
  public abstract class WebTreeViewMenuItemProvider
  {
    public WebTreeViewMenuItemProvider ()
    {
    }

    public abstract WebMenuItem[] InitalizeMenuItems (WebTreeNode node);

    public virtual void PreRenderMenuItems (WebTreeNode node, WebMenuItemCollection menuItems)
    {
    }

    public virtual void OnMenuItemEventCommandClick (WebMenuItem menuItem, WebTreeNode node)
    {
      if (menuItem != null && menuItem.Command != null)
        menuItem.Command.OnClick();
    }

    public virtual void OnMenuItemWxeFunctionCommandClick (WebMenuItem menuItem, WebTreeNode node)
    {
      if (menuItem != null && menuItem.Command != null)
      {
        Command command = menuItem.Command;
        Page page = node.TreeView.Page;
        if (page is IWxePage)
          command.ExecuteWxeFunction ((IWxePage) page, null);
        //else
        //  command.ExecuteWxeFunction (Page, null, new NameValueCollection (0));
      }
    }
  }
}
