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
using System.Collections;
using System.Collections.Generic;
using Remotion.Data;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  // TODO: Doc
  [Serializable]
  public abstract class ScopedTransactionStrategyBase : TransactionStrategyBase
  {
    private readonly ITransaction _transaction;
    [NonSerialized]
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
      _child = NullTransactionStrategy.Null;

      var inParameters = ExecutionContext.GetInParameters();
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

    public IWxeFunctionExecutionContext ExecutionContext
    {
      get { return _executionContext; }
    }

    public override void Commit ()
    {
      _transaction.Commit();
    }

    public override void Rollback ()
    {
      _transaction.Rollback();
    }

    public override void Reset ()
    {
      if (_scope != null)
      {
        _scope.Leave();
        _transaction.Reset();
        EnterScope();
      }
      else
        _transaction.Reset();
    }

    public override sealed TransactionStrategyBase CreateChildTransactionStrategy (bool autoCommit, IWxeFunctionExecutionContext executionContext, WxeContext wxeContext)
    {
      ArgumentUtility.CheckNotNull ("executionContext", executionContext);
      ArgumentUtility.CheckNotNull ("wxeContext", wxeContext);

      if (!_child.IsNull)
      {
        throw new InvalidOperationException (
            "The transaction strategy already has an active child transaction strategy. "
            + "This child transaction strategy must first be unregistered before invoking CreateChildTransactionStrategy again.");
      }

      var childTransactionStrategy = new ChildTransactionStrategy (autoCommit, Transaction.CreateChild(), this, executionContext);
      _child = childTransactionStrategy;
      if (_scope != null)
        _child.OnExecutionPlay (wxeContext, NullExecutionListener.Null);

      return childTransactionStrategy;
    }

    public override void UnregisterChildTransactionStrategy (TransactionStrategyBase childTransactionStrategy)
    {
      ArgumentUtility.CheckNotNull ("childTransactionStrategy", childTransactionStrategy);

      if (_child != childTransactionStrategy)
      {
        throw new InvalidOperationException (
            "Unregistering a child transaction strategy that is different from the presently registered strategy is not supported.");
      }

      _child = NullTransactionStrategy.Null;
    }

    public override sealed void RegisterObjects (IEnumerable objects)
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

      Child.OnExecutionPlay (context, listener);
    }

    public override void OnExecutionStop (WxeContext context, IWxeFunctionExecutionListener listener)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("listener", listener);

      if (_scope == null)
        throw new InvalidOperationException ("OnExecutionStop may not be invoked unless OnExecutionPlay was called first.");

      Child.OnExecutionStop (context, listener);

      if (AutoCommit)
        CommitTransaction();

      var outParameters = ExecutionContext.GetOutParameters();
      OuterTransactionStrategy.RegisterObjects (outParameters);

      LeaveScopeAndReleaseTransaction (null);
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
        Child.OnExecutionPause (context, listener);
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
        Child.OnExecutionFail (context, listener, exception);
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
      var scope = _transaction.EnterScope();
      Assertion.IsNotNull (scope);
      _scope = scope;
    }

    protected virtual void CommitTransaction ()
    {
      _transaction.Commit();
    }

    protected virtual void ReleaseTransaction ()
    {
      _transaction.Release();
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
