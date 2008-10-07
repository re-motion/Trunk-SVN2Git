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

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  public class TransactionExecutionListener : IWxeFunctionExecutionListener
  {
    private readonly TransactionStrategyBase _transactionStrategy;
    private readonly IWxeFunctionExecutionListener _innerListener;

    public TransactionExecutionListener (TransactionStrategyBase transactionStrategy, IWxeFunctionExecutionListener innerListener)
    {
      ArgumentUtility.CheckNotNull ("transactionStrategy", transactionStrategy);
      ArgumentUtility.CheckNotNull ("innerListener", innerListener);

      _transactionStrategy = transactionStrategy;
      _innerListener = innerListener;
    }

    public TransactionStrategyBase TransactionStrategy
    {
      get { return _transactionStrategy; }
    }

    public IWxeFunctionExecutionListener InnerListener
    {
      get { return _innerListener; }
    }

    public bool IsNull
    {
      get { return false; }
    }

    public void OnExecutionPlay (WxeContext context)
    {
      _transactionStrategy.OnExecutionPlay (context, _innerListener);
    }

    public void OnExecutionStop (WxeContext context)
    {
      _transactionStrategy.OnExecutionStop (context, _innerListener);
    }

    public void OnExecutionPause (WxeContext context)
    {
      _transactionStrategy.OnExecutionPause (context, _innerListener);
    }

    public void OnExecutionFail (WxeContext context, Exception exception)
    {
      _transactionStrategy.OnExecutionFail (context, _innerListener, exception);
    }
  }
}