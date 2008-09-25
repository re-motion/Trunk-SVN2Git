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
using System.Threading;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  //Documentation until coding is completed.
  internal class TxStrategyDoc
  {
    interface IWxeTransactionStrategyPublicApi
    {
      // Commit, Rollback, Reset
      // ITransaction MyTransaction { get; }
      bool AutoCommit { get; set; }
    }

    interface IWxeExecutionListener // create tx in ctor
    {
      void OnExecutionPlay (); // make transaction current; must only throw WxeFatalTransactionException
      void OnExecutionStop (); // autocommit, restore previous, release; can throw WxeFatalTransactionException or user transactions from Commit
      void OnExecutionFail (); // restore previous, release; must only throw WxeFatalTransactionException
      void OnExecutionPause (); // restore previous; must only throw WxeFatalTransactionException
    }

    interface IWxeTransactionStrategy : IWxeTransactionStrategyPublicApi, IWxeExecutionListener { }

    #region Transaction strategies
    class NullTransactionStrategy : IWxeTransactionStrategy {
      private readonly IWxeExecutionListener _nextListener;

      public NullTransactionStrategy (IWxeExecutionListener nextListener)
      {
        _nextListener = nextListener;
      }

      public void OnExecutionPlay ()
      {
        _nextListener.OnExecutionPlay();
      }

      public void OnExecutionStop ()
      {
        _nextListener.OnExecutionStop ();
      }

      public void OnExecutionFail ()
      {
        _nextListener.OnExecutionFail ();
      }

      public void OnExecutionPause ()
      {
        _nextListener.OnExecutionPause ();
      }
    }

    class RootTransactionStrategy : IWxeTransactionStrategy {
      private readonly IWxeExecutionListener _nextListener;

      public RootTransactionStrategy (IWxeExecutionListener nextListener)
      {
        _nextListener = nextListener;
      }

      public void OnExecutionPlay ()
      {
        // ensure transaction created
        // make transaction current
        _nextListener.OnExecutionPlay (); // not executed on fatal exception
      }

      public void OnExecutionStop ()
      {
        try
        {
          _nextListener.OnExecutionStop ();
        }
        finally
        {
          // if one of these throws an exception and _nextListener also threw, throw fatal exception and wrap both
          // consider using lambdas for exception handling rather than try-catches

          // autocommit
          // make previous transaction current
          // release transaction
        }
      }

      public void OnExecutionFail ()
      {
        try
        {
          _nextListener.OnExecutionFail ();
        }
        finally
        {
          // if one of these throws an exception and _nextListener also threw, throw fatal exception and wrap both
          // consider using lambdas for exception handling rather than try-catches

          // make previous transaction current
          // release transaction
        }
      }

      public void OnExecutionPause ()
      {
        try
        {
          _nextListener.OnExecutionPause ();
        }
        finally
        {
          // if one of these throws an exception and _nextListener also threw, throw fatal exception and wrap both
          // consider using lambdas for exception handling rather than try-catches

          // make previous transaction current
        }
      }
    }
    
    class ChildTransactionStrategy : RootTransactionStrategy {
      public ChildTransactionStrategy (IWxeExecutionListener nextListener)
          : base(nextListener)
      {
      }
    }

    #endregion 

    abstract class WxeTransactionMode
    {
      public static readonly WxeTransactionMode Null = new NullWxeTransactionMode ();
      public static readonly WxeTransactionMode CreateRoot = new CreateRootWxeTransactionMode ();
      public static readonly WxeTransactionMode CreateChildIfParent = new CreateChildIfParentWxeTransactionMode ();
      public abstract IWxeTransactionStrategy GetStrategy (WxeFunction function, IWxeExecutionListener executionListener);
    }

    #region Transaction Mode
    class NullWxeTransactionMode : WxeTransactionMode
    {
      public override IWxeTransactionStrategy GetStrategy (WxeFunction function, IWxeExecutionListener executionListener)
      {
        return new NullTransactionStrategy (executionListener);
      }
    }

    class CreateRootWxeTransactionMode : WxeTransactionMode
    {
      public override IWxeTransactionStrategy GetStrategy (WxeFunction function, IWxeExecutionListener executionListener)
      {
        return new RootTransactionStrategy (executionListener);
      }
    }

    class CreateChildIfParentWxeTransactionMode : WxeTransactionMode
    {
      public override IWxeTransactionStrategy GetStrategy (WxeFunction function, IWxeExecutionListener executionListener)
      {
        // search function parents for existing transaction
        // if found, return new ChildTransactionStrategy (parent := existingTransaction);
        // else, return new RootTransactionStrategy();
        throw new System.NotImplementedException();
      }
    }

    #endregion

    interface IWxeFunctionState
    {
      Exception Exception { get; }
      void Execute (WxeContext context);
    }

    class FirstExecutionFunctionState : ExecutionFunctionState
    {
      private WxeFunction _function;

      public override void Execute (WxeContext context)
      {
        _function.InitializeTransactionStrategy ();
        _function.SetCurrentExecutionState (new ExecutionFunctionState());
        base.Execute (context);
      }
    }

    class ExecutionFunctionState : IWxeFunctionState
    {
      private WxeFunction _function;

      public Exception Exception
      {
        get { return null; }
      }

      public virtual void Execute (WxeContext context)
      {
        try
        {
          _function.ExecutionListener.OnExecutionPlay (); // fatal exception => goes to WxeFatalTransactionException
          _function.BaseExecute (context);
          _function.ExecutionListener.OnExecutionStop (); // fatal exception  => goes to WxeFatalTransactionException, commit exception => goes to general catch block
          _function.SetCurrentExecutionState (new ExecutionFinishedFunctionState ());
        }
        catch (WxeFatalTransactionException) // bubble up
        {
          throw;
        }
        catch (ThreadAbortException)
        {
          _function.ExecutionListener.OnExecutionPause (); // fatal exception => let it bubble up
          throw;
        }
        catch (WxeExecuteUserControlStepException)
        {
          _function.ExecutionListener.OnExecutionPause (); // fatal exception => let it bubble up
          throw;
        }
        catch (WxeExecuteUserControlNextStepException)
        {
          _function.ExecutionListener.OnExecutionStop (); // fatal exception, commit exception
          _function.SetCurrentExecutionState (new ExecutionFinishedFunctionState ());
        }
        catch (Exception e)
        {
          Exception unwrappedException = PageUtility.GetUnwrappedExceptionFromHttpException (e) ?? e;

          if (!_function.ExceptionCatcher.Catch (unwrappedException)) // e is not a catchable exception, rethrow
          {
            try
            {
              _function.ExecutionListener.OnExecutionFail();
            }
            catch (WxeFatalTransactionException fatalException)
            {
              throw new WxeFatalTransactionException (e, fatalException); // e becomes inner exception, message etc. are taken from fatalException
            }

            _function.SetCurrentExecutionState (new ExecutionFailedFunctionState (e));

            if (unwrappedException is WxeUnhandledException)
              throw unwrappedException;
            throw new WxeUnhandledException (string.Format ("An unhandled exception ocured while executing WxeFunction  '{0}': {1}", GetType ().FullName, unwrappedException.Message), unwrappedException);
          }

          try
          {
            _function.ExecutionListener.OnExecutionFail ();
          }
          catch (WxeFatalTransactionException fatalException)
          {
            throw new WxeFatalTransactionException (e, fatalException); // e becomes inner exception, message etc. are taken from fatalException
          }

          _function.SetCurrentExecutionState (new ExecutionCaughtExceptionFunctionState (unwrappedException));
        }
      }
    }

    class WxeExceptionCatcher
    {
      // bool match = false;
      //if (_catchExceptions && _catchExceptionTypes != null)
      //{
      //  foreach (Type exceptionType in _catchExceptionTypes)
      //  {
      //    if (exceptionType.IsAssignableFrom (unwrappedException.GetType ()))
      //    {
      //      match = true;
      //      break;
      //    }
      //  }
      //}
    }

    class WxeFunction : WxeStepList
    {
      private readonly WxeTransactionMode _mode;
      private IWxeTransactionStrategy _strategy;
      private IWxeExecutionListener _executionListener;
      private IWxeFunctionState _executionState;

      public WxeFunction (WxeTransactionMode mode)
      {
        _mode = mode;
      }

      public WxeFunction ()
        : this (WxeTransactionMode.Null)
      {
      }

      public WxeTransactionMode Mode
      {
        get { return _mode; }
      }

      public IWxeFunctionState ExecutionState
      {
        get { return _executionState; }
      }

      public void SetCurrentExecutionState (IWxeFunctionState newState)
      {
        _executionState = newState;
      }

      public IWxeTransactionStrategyPublicApi Transaction
      {
        get
        {
          if (_strategy == null)
            throw new InvalidOperationException();
          return _strategy; 
        }
      }

      public IWxeExecutionListener ExecutionListener
      {
        get
        {
          if (_executionListener == null)
            throw new InvalidOperationException ();
          return _executionListener;
        }
        set 
        {
          _executionListener = value;
        }
      }

      public Exception Exception
      {
        get { return _executionState.Exception; }
      }

      public void InitializeTransactionStrategy ()
      {
        // if (_strategy != null) throw;
        _strategy = _mode.GetStrategy (this, _executionListener);
        _executionListener = _strategy;
      }

      public override void Execute (WxeContext context)
      {
        // State pattern!

        if (!IsExecutionStarted)
        {
          // ...
        }
        else
        {
          // ...
        }

        try
        {
          BaseExecute(context);
        }
        catch (ThreadAbortException)
        {
          throw;
        }
        catch (WxeExecuteUserControlStepException)
        {
          throw;
        }
        catch (WxeExecuteUserControlNextStepException)
        {
        }
        catch (Exception e)
        {
          Exception unwrappedException = PageUtility.GetUnwrappedExceptionFromHttpException (e) ?? e;

          bool match = false;
          // ...

          if (!match) // e is not a catchable exception, rethrow
          {
            if (unwrappedException is WxeUnhandledException)
              throw unwrappedException;
            throw new WxeUnhandledException (string.Format ("An unhandled exception ocured while executing WxeFunction  '{0}': {1}", GetType ().FullName, unwrappedException.Message), unwrappedException);
          }

          // _exception = unwrappedException;
        }

        // ...
      }

      public void BaseExecute (WxeContext context)
      {
        base.Execute (context);
      }
    }

    class UserCode
    {
      public static void Main ()
      {
        var wxeFunctionWithoutTx = new WxeFunction ();
        var wxeFunctionWithRootTx = new WxeFunction (WxeTransactionMode.CreateRoot);
        var wxeFunctionWithChildTx = new WxeFunction (WxeTransactionMode.CreateChildIfParent);

        wxeFunctionWithRootTx.Transaction.AutoCommit = false; // default is true
      }
    }
  }
}