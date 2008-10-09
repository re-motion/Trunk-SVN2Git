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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.AccessControl;
using Remotion.SecurityManager.Clients.Web.WxeFunctions;
using Remotion.SecurityManager.Clients.Web.WxeFunctions.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;

namespace Remotion.SecurityManager.Clients.Web.UI.AccessControl
{
  [WebMultiLingualResources (typeof (SecurableClassDefinitionListFormResources))]
  public partial class SecurableClassDefinitionListForm : BasePage
  {

    protected new SecurableClassDefinitionListFormFunction CurrentFunction
    {
      get { return (SecurableClassDefinitionListFormFunction) base.CurrentFunction; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      LoadTree (IsPostBack, false);
      if (!IsPostBack)
        ExpandTreeNodes (SecurableClassDefinitionTree.Nodes);
    }

    protected override void OnPreRenderComplete (EventArgs e)
    {
      HtmlHeadAppender.Current.SetTitle (SecurableClassDefinitionListFormResources.Title);
      base.OnPreRenderComplete (e);
    }

    private void LoadTree (bool interim, bool refreshTreeNodes)
    {
      SecurableClassDefinitionTree.LoadUnboundValue (SecurableClassDefinition.FindAllBaseClasses (), interim);
      if (refreshTreeNodes)
        SecurableClassDefinitionTree.RefreshTreeNodes ();
    }

    private void ExpandTreeNodes (WebTreeNodeCollection webTreeNodeCollection)
    {
      foreach (WebTreeNode treeNode in webTreeNodeCollection)
      {
        treeNode.EvaluateExpand ();
        ExpandTreeNodes (treeNode.Children);
      }
    }

    protected void SecurableClassDefinitionTree_Click (object sender, BocTreeNodeClickEventArgs e)
    {
      if (!IsReturningPostBack)
      {
        SecurableClassDefinition classDefinition = (SecurableClassDefinition) e.BusinessObjectTreeNode.BusinessObject;
        EditPermissionsFormFunction function = new EditPermissionsFormFunction (WxeTransactionMode.CreateRootWithAutoCommit , classDefinition.ID);
        var options = new WxeCallOptionsExternal (
            "_blank", "width=1000, height=700, resizable=yes, menubar=no, toolbar=no, location=no, status=no", true);
        try
        {
          ExecuteFunction (function, new WxeCallArguments ((Control) sender, options));
        }
        catch (WxeCallExternalException)
        {
        }
      }
      else
      {
        LoadTree (false, true);
      }
    }
  }
}
