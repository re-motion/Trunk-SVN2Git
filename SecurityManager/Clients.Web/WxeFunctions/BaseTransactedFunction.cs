// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.ExecutionEngine;

namespace Remotion.SecurityManager.Clients.Web.WxeFunctions
{
  [Serializable]
  public abstract class BaseTransactedFunction : WxeFunction
  {
    protected BaseTransactedFunction ()
        : this (WxeTransactionMode.CreateRootWithAutoCommit)
    {
    }

    protected BaseTransactedFunction (ITransactionMode transactionMode, params object[] args)
        : base (transactionMode, args)
    {
      Initialize();
    }

    public ObjectID TenantID
    {
      get { return (Tenant.Current != null) ? Tenant.Current.ID : null; }
    }

    public bool HasUserCancelled
    {
      get { return (ExceptionHandler.Exception != null && ExceptionHandler.Exception.GetType() == typeof (WxeUserCancelException)); }
    }

    protected virtual void Initialize ()
    {
      ExceptionHandler.SetCatchExceptionTypes (typeof (WxeUserCancelException));
    }
  }
}
