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
      // MyTransaction
      // AutoCommit
    }

    interface IWxeTransactionStrategyInternalApi // create tx in ctor
    {
      void OnExecutionPlay (); // make transaction current; must only throw WxeFatalTransactionException
      void OnExecutionStop (); // autocommit, restore previous, release; can throw WxeFatalTransactionException or user transactions from Commit
      void OnExecutionFail (); // restore previous, release; must only throw WxeFatalTransactionException
      void OnExecutionPause (); // restore previous; must only throw WxeFatalTransactionException
    }

    interface IWxeTransactionStrategy : IWxeTransactionStrategyPublicApi, IWxeTransactionStrategyInternalApi { }

    class NullTransactionStrategy : IWxeTransactionStrategy { }
    class RootTransactionStrategy : IWxeTransactionStrategy { }
    class ChildTransactionStrategy : IWxeTransactionStrategy { }

    #region Transaction Mode
    abstract class WxeTransactionMode
    {
      public static readonly WxeTransactionMode Null = new NullWxeTransactionMode ();
      public static readonly WxeTransactionMode CreateRoot = new CreateRootWxeTransactionMode ();
      public static readonly WxeTransactionMode CreateChildIfParent = new CreateChildIfParentWxeTransactionMode ();
      public abstract IWxeTransactionStrategy GetStrategy (WxeFunction function);
    }

    class NullWxeTransactionMode : WxeTransactionMode
    {
      public override IWxeTransactionStrategy GetStrategy (WxeFunction function)
      {
        return new NullTransactionStrategy();
      }
    }

    class CreateRootWxeTransactionMode : WxeTransactionMode
    {
      public override IWxeTransactionStrategy GetStrategy (WxeFunction function)
      {
        return new RootTransactionStrategy();
      }
    }

    class CreateChildIfParentWxeTransactionMode : WxeTransactionMode
    {
      public override IWxeTransactionStrategy GetStrategy (WxeFunction function)
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
          _function.InternalTransactionApi.OnExecutionPlay (); // fatal exception => goes to WxeFatalTransactionException
          _function.BaseExecute (context);
          _function.InternalTransactionApi.OnExecutionStop (); // fatal exception  => goes to WxeFatalTransactionException, commit exception => goes to general catch block
          _function.SetCurrentExecutionState (new ExecutionFinishedFunctionState ());
        }
        catch (WxeFatalTransactionException) // bubble up
        {
          throw;
        }
        catch (ThreadAbortException)
        {
          _function.InternalTransactionApi.OnExecutionPause (); // fatal exception => let it bubble up
          throw;
        }
        catch (WxeExecuteUserControlStepException)
        {
          _function.InternalTransactionApi.OnExecutionPause (); // fatal exception => let it bubble up
          throw;
        }
        catch (WxeExecuteUserControlNextStepException)
        {
          _function.InternalTransactionApi.OnExecutionStop (); // fatal exception, commit exception
          _function.SetCurrentExecutionState (new ExecutionFinishedFunctionState ());
        }
        catch (Exception e)
        {
          Exception unwrappedException = PageUtility.GetUnwrappedExceptionFromHttpException (e) ?? e;

          if (!_function.ExceptionCatcher.Catch (unwrappedException)) // e is not a catchable exception, rethrow
          {
            try
            {
              _function.InternalTransactionApi.OnExecutionFail();
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
            _function.InternalTransactionApi.OnExecutionFail ();
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

      public IWxeTransactionStrategyInternalApi InternalTransactionApi
      {
        get
        {
          if (_strategy == null)
            throw new InvalidOperationException ();
          return _strategy;
        }
      }

      public Exception Exception
      {
        get { return _executionState.Exception; }
      }

      public void InitializeTransactionStrategy ()
      {
        // if (_strategy != null) throw;
        _strategy = _mode.GetStrategy (this);
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
  }
}