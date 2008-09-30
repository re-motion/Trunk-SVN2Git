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
    private readonly ITransaction _transaction;
    private ITransactionScope _scope;

    public RootTransactionStrategy (bool autoCommit, IWxeFunctionExecutionListener innerListener, ITransactionScopeManager scopeManager)
        : base (autoCommit, innerListener)
    {
      ArgumentUtility.CheckNotNull ("_scopeManager", scopeManager);

      _scopeManager = scopeManager;
      _transaction = scopeManager.CreateRootTransaction();
    }

    public override void OnExecutionPlay (WxeContext context)
    {
      if (_scope != null)
      {
        throw new InvalidOperationException (
            "OnExecutionPlay may not be invoked twice without calling OnExecutionStop, OnExecutionPause, or OnExecutionFail in-between.");
      }

      ExecuteAndWrapInnerException (delegate { _scope = _transaction.EnterScope(); }, null);

      InnerListener.OnExecutionPlay (context);
    }

    public override void OnExecutionStop (WxeContext context)
    {
      if (_scope == null)
        throw new InvalidOperationException ("OnExecutionStop may not be invoked unless OnExecutionPlay was called first.");

      Exception innerException = null;
      try
      {
        InnerListener.OnExecutionStop (context);
        if (AutoCommit)
          _transaction.Commit();
      }
      catch (Exception e)
      {
        innerException = e;
        throw;
      }
      finally
      {
        ExecuteAndWrapInnerException (_scope.Leave, innerException);
        ExecuteAndWrapInnerException (_transaction.Release, innerException);
        _scope = null;
      }
    }

    public override void OnExecutionPause (WxeContext context)
    {
      if (_scope == null)
        throw new InvalidOperationException ("OnExecutionPause may not be invoked unless OnExecutionPlay was called first.");

      Exception innerException = null;
      try
      {
        InnerListener.OnExecutionPause (context);
      }
      catch (Exception e)
      {
        innerException = e;
        throw;
      }
      finally
      {
        ExecuteAndWrapInnerException (_scope.Leave, innerException);
        _scope = null;
      }
    }

    public override void OnExecutionFail (WxeContext context, Exception exception)
    {
      if (_scope == null)
        throw new InvalidOperationException ("OnExecutionFail may not be invoked unless OnExecutionPlay was called first.");

      Exception innerException = null;
      try
      {
        InnerListener.OnExecutionFail (context, exception);
      }
      catch (Exception e)
      {
        innerException = e;
        throw;
      }
      finally
      {
        ExecuteAndWrapInnerException (_scope.Leave, innerException);
        ExecuteAndWrapInnerException (_transaction.Release, innerException);
        _scope = null;
      }
    }

    public override ITransaction Transaction
    {
      get { return _transaction; }
    }

    public override void Commit ()
    {
      throw new NotImplementedException();
    }

    public override void Rollback ()
    {
      throw new NotImplementedException();
    }

    public override void Reset ()
    {
      throw new NotImplementedException();
    }

    public override bool IsNull
    {
      get { return false; }
    }

    public ITransactionScopeManager ScopeManager
    {
      get { return _scopeManager; }
    }

    public ITransactionScope Scope
    {
      get { return _scope; }
    }

    private void ExecuteAndWrapInnerException (Action action, Exception existingInnerException)
    {
      try
      {
        action();
      }
      catch (Exception e)
      {
        if (existingInnerException == null)
          throw new WxeFatalExecutionException (e, null);
        else
          throw new WxeFatalExecutionException (existingInnerException, e);
      }
    }
  }
}