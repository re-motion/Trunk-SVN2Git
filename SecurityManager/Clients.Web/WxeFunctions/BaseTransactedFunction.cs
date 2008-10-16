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