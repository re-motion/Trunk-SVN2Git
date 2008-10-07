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
    private readonly ITransaction _transaction;
    private ITransactionScope _scope;

    public RootTransactionStrategy (
        bool autoCommit, ITransaction transaction, ITransactionStrategy parent, IWxeFunctionExecutionContext executionContext)
        : base (autoCommit, parent, executionContext)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      _transaction = transaction;

      var inParameters = ExecutionContext.GetInParameters();
      RegisterObjects (inParameters);
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
        _scope = _transaction.EnterScope ();
      }
      else
        _transaction.Reset ();
    }

    public override IWxeFunctionExecutionListener CreateExecutionListener (IWxeFunctionExecutionListener innerListener)
    {
      ArgumentUtility.CheckNotNull ("innerListener", innerListener);

      return new TransactionExecutionListener (this, innerListener);
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

      ExecuteAndWrapInnerException (delegate { _scope = _transaction.EnterScope(); }, null);

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
        listener.OnExecutionStop (context);
        if (AutoCommit)
          _transaction.Commit();

        if (Parent != null)
        {
          var outParameters = ExecutionContext.GetOutParameters();
          Parent.RegisterObjects (outParameters);
        }
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
        ExecuteAndWrapInnerException (_transaction.Release, innerException);
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
        listener.OnExecutionPause (context);
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

    public override void OnExecutionFail (WxeContext context, IWxeFunctionExecutionListener listener, Exception exception)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("listener", listener);

      if (_scope == null)
        throw new InvalidOperationException ("OnExecutionFail may not be invoked unless OnExecutionPlay was called first.");

      Exception innerException = null;
      try
      {
        listener.OnExecutionFail (context, exception);
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
        ExecuteAndWrapInnerException (_transaction.Release, innerException);
      }
    }

    public ITransaction Transaction
    {
      get { return _transaction; }
    }

    public ITransactionScope Scope
    {
      get { return _scope; }
    }

    public override TTransaction GetNativeTransaction<TTransaction> ()
    {
      return _transaction.To<TTransaction>();
    }

    public override bool IsNull
    {
      get { return false; }
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