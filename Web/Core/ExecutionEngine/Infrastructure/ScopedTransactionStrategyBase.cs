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
  // TODO: Doc
  public abstract class ScopedTransactionStrategyBase : TransactionStrategyBase
  {
    private readonly ITransaction _transaction;
    private ITransactionScope _scope;
    private readonly bool _autoCommit;
    private readonly IWxeFunctionExecutionContext _executionContext;
    private readonly TransactionStrategyBase _outerTransactionStrategy;
    private TransactionStrategyBase _child;

    protected ScopedTransactionStrategyBase (
        bool autoCommit, ITransaction transaction, TransactionStrategyBase outerTransactionStrategy, IWxeFunctionExecutionContext executionContext)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      ArgumentUtility.CheckNotNull ("outerTransactionStrategy", outerTransactionStrategy);
      ArgumentUtility.CheckNotNull ("executionContext", executionContext);

      _autoCommit = autoCommit;
      _transaction = transaction;
      _outerTransactionStrategy = outerTransactionStrategy;
      _executionContext = executionContext;

      var inParameters = ExecutionContext.GetInParameters ();
      RegisterObjects (inParameters);
    }

    public ITransaction Transaction
    {
      get { return _transaction; }
    }

    public ITransactionScope Scope
    {
      get { return _scope; }
    }

    public override bool IsNull
    {
      get { return false; }
    }

    public bool AutoCommit
    {
      get { return _autoCommit; }
    }

    public override TransactionStrategyBase OuterTransactionStrategy
    {
      get { return _outerTransactionStrategy; }
    }

    public TransactionStrategyBase Child
    {
      get { return _child; }
    }

    public void SetChild (TransactionStrategyBase child)
    {
      _child = child;
    }

    public IWxeFunctionExecutionContext ExecutionContext
    {
      get { return _executionContext; }
    }

    public override void Commit ()
    {
      _transaction.Commit ();
    }

    public override void Rollback ()
    {
      _transaction.Rollback ();
    }

    public override void Reset ()
    {
      if (_scope != null)
      {
        _scope.Leave ();
        _transaction.Reset ();
        EnterScope();
      }
      else
        _transaction.Reset ();
    }

    public sealed override void RegisterObjects (IEnumerable objects)
    {
      ArgumentUtility.CheckNotNull ("objects", objects);

      _transaction.RegisterObjects (FlattenList (objects));
    }

    public override void OnExecutionPlay (WxeContext context, IWxeFunctionExecutionListener listener)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("listener", listener);
      
      if (_scope != null)
      {
        throw new InvalidOperationException (
            "OnExecutionPlay may not be invoked twice without calling OnExecutionStop, OnExecutionPause, or OnExecutionFail in-between.");
      }

      ExecuteAndWrapInnerException (EnterScope, null);

      if (Child != null)
        Child.OnExecutionPlay (context, listener);
      else
        listener.OnExecutionPlay (context);
    }

    public override void OnExecutionStop (WxeContext context, IWxeFunctionExecutionListener listener)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("listener", listener);
      
      if (_scope == null)
        throw new InvalidOperationException ("OnExecutionStop may not be invoked unless OnExecutionPlay was called first.");

      Exception innerException = null;
      try
      {
        if (Child != null)
          Child.OnExecutionStop (context, listener);
        else
          listener.OnExecutionStop (context);

        if (AutoCommit)
          CommitTransaction();

        var outParameters = ExecutionContext.GetOutParameters ();
        OuterTransactionStrategy.RegisterObjects (outParameters);
      }
      catch (Exception e)
      {
        innerException = e;
        throw;
      }
      finally
      {
        LeaveScopeAndReleaseTransaction (innerException);
      }
    }

    public override void OnExecutionPause (WxeContext context, IWxeFunctionExecutionListener listener)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("listener", listener);
      
      if (_scope == null)
        throw new InvalidOperationException ("OnExecutionPause may not be invoked unless OnExecutionPlay was called first.");

      Exception innerException = null;
      try
      {
        if (Child != null)
          Child.OnExecutionPause (context, listener);
        else
          listener.OnExecutionPause (context);
      }
      catch (Exception e)
      {
        innerException = e;
        throw;
      }
      finally
      {
        LeaveScope (innerException);
      }
    }

    public override void OnExecutionFail (WxeContext context, IWxeFunctionExecutionListener listener, Exception exception)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("listener", listener);

      if (_scope == null)
        throw new InvalidOperationException ("OnExecutionFail may not be invoked unless OnExecutionPlay was called first.");

      Exception innerException = null;
      try
      {
        if (Child != null)
          Child.OnExecutionFail (context, listener, exception);
        else
          listener.OnExecutionFail (context, exception);
      }
      catch (Exception e)
      {
        innerException = e;
        throw;
      }
      finally
      {
        LeaveScopeAndReleaseTransaction (innerException);
      }
    }

    public override TTransaction GetNativeTransaction<TTransaction> ()
    {
      return _transaction.To<TTransaction>();
    }

    private void EnterScope ()
    {
      _scope = _transaction.EnterScope ();
    }

    protected virtual void CommitTransaction ()
    {
      _transaction.Commit ();
    }

    protected virtual void ReleaseTransaction ()
    {
      _transaction.Release ();
    }

    private void LeaveScope (Exception innerException)
    {
      bool isFatalExecutionException = innerException != null && innerException is WxeFatalExecutionException;
      if (!isFatalExecutionException)
      {
        ExecuteAndWrapInnerException (_scope.Leave, innerException);
        _scope = null;
      }
    }

    private void LeaveScopeAndReleaseTransaction (Exception innerException)
    {
      bool isFatalExecutionException = innerException != null && innerException is WxeFatalExecutionException;
      if (!isFatalExecutionException)
      {
        ExecuteAndWrapInnerException (_scope.Leave, innerException);
        _scope = null;
        ExecuteAndWrapInnerException (ReleaseTransaction, innerException);
      }
    }

    private IEnumerable<object> FlattenList (IEnumerable objects)
    {
      var list = new List<object>();
      foreach (var obj in objects)
      {
        if (obj is IEnumerable)
          list.AddRange (FlattenList ((IEnumerable) obj));
        else if (obj != null)
          list.Add (obj);
      }

      return list;
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