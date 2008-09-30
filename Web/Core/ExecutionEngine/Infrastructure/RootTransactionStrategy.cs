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
using Remotion.Data;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  //TODO: Doc
  public class RootTransactionStrategy : TransactionStrategyBase
  {
    private readonly ITransactionScopeManager _scopeManager;

    public RootTransactionStrategy (bool autoCommit, IWxeFunctionExecutionListener innerListener, ITransactionScopeManager scopeManager)
        : base (autoCommit, innerListener)
    {
      ArgumentUtility.CheckNotNull ("_scopeManager", scopeManager);
      _scopeManager = scopeManager;
    }

    public override void OnExecutionPlay (WxeContext context)
    {
      throw new System.NotImplementedException();
    }

    public override void OnExecutionStop (WxeContext context)
    {
      throw new System.NotImplementedException();
    }

    public override void OnExecutionPause (WxeContext context)
    {
      throw new System.NotImplementedException();
    }

    public override void OnExecutionFail (WxeContext context, Exception exception)
    {
      throw new System.NotImplementedException();
    }

    public override ITransaction Transaction
    {
      get { throw new System.NotImplementedException(); }
    }

    public override void Commit ()
    {
      throw new System.NotImplementedException();
    }

    public override void Rollback ()
    {
      throw new System.NotImplementedException();
    }

    public override void Reset ()
    {
      throw new System.NotImplementedException ();
    }

    public override bool IsNull
    {
      get { return false; }
    }

    public ITransactionScopeManager ScopeManager
    {
      get { return _scopeManager; }
    }
  }
}