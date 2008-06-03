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
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public abstract class BocTreeViewMenuItemProvider : WebTreeViewMenuItemProvider
  {
    private BocTreeView _ownerControl;

    public BocTreeViewMenuItemProvider ()
    {
    }

    public override void OnMenuItemEventCommandClick (WebMenuItem menuItem, WebTreeNode node)
    {
      if (menuItem != null && menuItem.Command != null)
      {
        if (menuItem is BocMenuItem)
          ((BocMenuItemCommand) menuItem.Command).OnClick ((BocMenuItem) menuItem);
        else
          base.OnMenuItemEventCommandClick (menuItem, node);
      }
    }

    public override void OnMenuItemWxeFunctionCommandClick (WebMenuItem menuItem, WebTreeNode node)
    {
      if (menuItem != null && menuItem.Command != null)
      {
        if (menuItem is BocMenuItem)
        {
          BocMenuItemCommand command = (BocMenuItemCommand) menuItem.Command;
          IBusinessObject businessObject = null;
          if (node is BusinessObjectTreeNode)
            businessObject = ((BusinessObjectTreeNode) node).BusinessObject;

          Page page = node.TreeView.Page;
          if (page is IWxePage)
            command.ExecuteWxeFunction ((IWxePage) page, businessObject);
          //else
          //  command.ExecuteWxeFunction (Page, businessObject);
        }
        else
        {
          base.OnMenuItemWxeFunctionCommandClick (menuItem, node);
        }
      }
    }

    public BocTreeView OwnerControl
    {
      get { return _ownerControl; }
      set { _ownerControl = value; }
    }
  }
}
