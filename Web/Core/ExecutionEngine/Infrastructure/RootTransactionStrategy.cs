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
    private readonly IWxeFunctionExecutionContext _executionContext;
    private ITransactionScope _scope;

    public RootTransactionStrategy (
        bool autoCommit, ITransaction transaction, IWxeFunctionExecutionContext executionContext, IWxeFunctionExecutionListener innerListener)
        : base (autoCommit, innerListener)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      ArgumentUtility.CheckNotNull ("executionContext", executionContext);

      _transaction = transaction;
      _executionContext = executionContext;

      var inParameters = _executionContext.GetInParameters();
      RegisterObjects (inParameters);
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
        
        var outParameters = _executionContext.GetOutParameters();
        _executionContext.ParentTransactionStrategy.RegisterObjects (outParameters);
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
        ExecuteAndWrapInnerException (_transaction.Release, innerException);
      }
    }

    public override ITransaction Transaction
    {
      get { return _transaction; }
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
        _scope = _transaction.EnterScope();
      }
      else
        _transaction.Reset();
    }

    public override bool IsNull
    {
      get { return false; }
    }

    public ITransactionScope Scope
    {
      get { return _scope; }
    }

    public sealed override void RegisterObjects (IEnumerable objects)
    {
      _transaction.RegisterObjects (FlattenList (objects));
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