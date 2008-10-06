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
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// The new <see cref="WxeFunction"/>. Will replace the <see cref="WxeFunction"/> type once implemtation is completed.
  /// </summary>
  [Serializable]
  public abstract class WxeFunction2 : WxeStepList, IWxeFunctionExecutionContext
  {
    private IWxeFunctionExecutionListener _executionListener = new NullExecutionListener();
    private ITransactionStrategy _transactionStrategy;
    private readonly ITransactionMode _transactionMode;

    protected WxeFunction2 (ITransactionMode transactionMode)
    {
      _transactionMode = transactionMode;
    }

    public override void Execute (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      Assertion.IsNotNull (_executionListener);

      if (!IsExecutionStarted)
        _transactionStrategy = _transactionMode.CreateTransactionStrategy (this, _executionListener);

      _executionListener.OnExecutionPlay (context);

      try
      {
        base.Execute (context);
      }
      catch (ThreadAbortException)
      {
        _executionListener.OnExecutionPause (context);
        throw;
      }
      catch (Exception stepException)
      {
        try
        {
          _executionListener.OnExecutionFail (context, stepException);
        }
        catch (Exception listenerException)
        {
          throw new WxeFatalExecutionException (stepException, listenerException);
        }
        throw;
      }

      _executionListener.OnExecutionStop (context);
    }

    public IWxeFunctionExecutionListener ExecutionListener
    {
      get { return _executionListener; }
      set { _executionListener = ArgumentUtility.CheckNotNull ("value", value); }
    }

    //TODO: Remove when WxeFunction2 merged to WxeFunction
    public new WxeFunction2 ParentFunction
    {
      get { return WxeStep.GetStepByType<WxeFunction2> (ParentStep); }
    }

    public ITransactionStrategy Transaction
    {
      get { return _transactionStrategy; }
    }

    public ITransactionMode TransactionMode
    {
      get { return _transactionMode; }
    }

    object[] IWxeFunctionExecutionContext.GetInParameters ()
    {
      return new object[0];
    }

    object[] IWxeFunctionExecutionContext.GetOutParameters ()
    {
      throw new NotImplementedException();
    }
  }
}