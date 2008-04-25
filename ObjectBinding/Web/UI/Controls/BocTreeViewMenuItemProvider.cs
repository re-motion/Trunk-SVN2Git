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