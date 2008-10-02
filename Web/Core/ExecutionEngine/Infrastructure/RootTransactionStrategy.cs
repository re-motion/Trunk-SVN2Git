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
using System.Collections;
using System.Collections.Generic;
using Remotion.Data;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  //TODO: Doc
  public class RootTransactionStrategy : TransactionStrategyBase
  {
    private readonly ITransactionManager _transactionManager;
    private ITransactionScope _scope;

    public RootTransactionStrategy (bool autoCommit, IWxeFunctionExecutionListener innerListener, ITransactionManager transactionManager, IWxeFunctionExecutionContext _executionContext)
        : base (autoCommit, innerListener)
    {
      ArgumentUtility.CheckNotNull ("transactionManager", transactionManager);

      _transactionManager = transactionManager;
      InitializeTransaction ();
    }

    public override void OnExecutionPlay (WxeContext context)
    {
      if (_scope != null)
      {
        throw new InvalidOperationException (
            "OnExecutionPlay may not be invoked twice without calling OnExecutionStop, OnExecutionPause, or OnExecutionFail in-between.");
      }

      ExecuteAndWrapInnerException (delegate { _scope = _transactionManager.EnterScope(); }, null);

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
          _transactionManager.Transaction.Commit ();
        // outParameters = function.Variables.GetOutParameters();
        // function.ParentFunction.Transaction.RegisterObjects (outParameters);
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
        ExecuteAndWrapInnerException (_transactionManager.ReleaseTransaction, innerException);
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
        _scope = null;
        ExecuteAndWrapInnerException (_transactionManager.ReleaseTransaction, innerException);
      }
    }

    public override ITransaction Transaction
    {
      get { return _transactionManager.Transaction; }
    }

    public override void Commit ()
    {
      _transactionManager.Transaction.Commit ();
    }

    public override void Rollback ()
    {
      _transactionManager.Transaction.Rollback ();
    }

    public override void Reset ()
    {
      if (_scope != null)
      {
        _scope.Leave();
        _transactionManager.ReleaseTransaction ();
        InitializeTransaction();
        _scope = _transactionManager.EnterScope ();
      }
      else
      {
        _transactionManager.ReleaseTransaction ();
        InitializeTransaction();
      }
    }

    public override bool IsNull
    {
      get { return false; }
    }

    public ITransactionScope Scope
    {
      get { return _scope; }
    }

    private void InitializeTransaction ()
    {
      _transactionManager.InitializeTransaction();
      // inParameters = function.Variables.GetInParameters();
      // RegisterObjects (inParameters);
    }

    // after RegisterObjects finished, all newly registered objects should be loaded
    // transaction event handlers and 
    // if called from Reset: collection event handlers should be copied, transaction event handlers should be copied

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