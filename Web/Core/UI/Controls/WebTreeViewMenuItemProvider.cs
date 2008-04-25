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