// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.Linq.Utilities;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.WxeFunctions;
using Remotion.Web.ExecutionEngine;

namespace Remotion.SecurityManager.Clients.Web.Classes.OrganizationalStructure
{
  public abstract class BaseListControl : BaseControl
  {
    protected abstract IList GetValues ();

    protected abstract FormFunction CreateEditFunction (ITransactionMode transactionMode, ObjectID objectID);

    protected void HandleEditItemClick (BocList sender, BocListItemCommandClickEventArgs e)
    {
      ArgumentUtility.CheckNotNull ("sender", sender);

      if (!Page.IsReturningPostBack)
      {
        var editUserFormFunction = CreateEditFunction (WxeTransactionMode.CreateRootWithAutoCommit, ((DomainObject) e.BusinessObject).ID);
        Page.ExecuteFunction (editUserFormFunction, WxeCallArguments.Default);
      }
      else
      {
        if (!((FormFunction) Page.ReturningFunction).HasUserCancelled)
        {
          CurrentFunction.Transaction.Reset();
          sender.LoadUnboundValue (GetValues(), false);
        }
      }
    }

    protected void HandleNewButtonClick (BocList sender)
    {
      ArgumentUtility.CheckNotNull ("sender", sender);

      if (!Page.IsReturningPostBack)
      {
        var editUserFormFunction = CreateEditFunction (WxeTransactionMode.CreateRootWithAutoCommit, null);
        Page.ExecuteFunction (editUserFormFunction, WxeCallArguments.Default);
      }
      else
      {
        if (!((FormFunction) Page.ReturningFunction).HasUserCancelled)
        {
          CurrentFunction.Transaction.Reset();
          sender.LoadUnboundValue (GetValues(), false);
        }
      }
    }

    protected void ResetListOnTenantChange (BocList list)
    {
      ArgumentUtility.CheckNotNull ("list", list);

      if (HasTenantChanged)
      {
        CurrentFunction.Transaction.Reset ();
        list.LoadUnboundValue (GetValues (), false);
      }
    }
  }
}