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
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  //TODO: Doc
  [Serializable]
  public class NoneTransactionStrategy : TransactionStrategyBase
  {
    private readonly TransactionStrategyBase _outerTransactionStrategy;

    public NoneTransactionStrategy (TransactionStrategyBase outerTransactionStrategy)
    {
      ArgumentUtility.CheckNotNull ("outerTransactionStrategy", outerTransactionStrategy);

      _outerTransactionStrategy = outerTransactionStrategy;
    }

    public override void Commit ()
    {
      throw new NotSupportedException ();
    }

    public override void Rollback ()
    {
      throw new NotSupportedException ();
    }

    public override void Reset ()
    {
      throw new NotSupportedException ();
    }

    public override IWxeFunctionExecutionListener CreateExecutionListener (IWxeFunctionExecutionListener innerListener)
    {
      ArgumentUtility.CheckNotNull ("innerListener", innerListener);

      return innerListener;
    }

    public override TransactionStrategyBase CreateChildTransactionStrategy (bool autoCommit, IWxeFunctionExecutionContext executionContext, WxeContext wxeContext)
    {
      ArgumentUtility.CheckNotNull ("executionContext", executionContext);

      return OuterTransactionStrategy.CreateChildTransactionStrategy (autoCommit, executionContext, wxeContext);
    }

    public override void UnregisterChildTransactionStrategy (TransactionStrategyBase childTransactionStrategy)
    {
      ArgumentUtility.CheckNotNull ("childTransactionStrategy", childTransactionStrategy);

      OuterTransactionStrategy.UnregisterChildTransactionStrategy (childTransactionStrategy);
    }

    public override void RegisterObjects (IEnumerable objects)
    {
      ArgumentUtility.CheckNotNull ("objects", objects);

      OuterTransactionStrategy.RegisterObjects (objects);
    }

    public override TTransaction GetNativeTransaction<TTransaction> ()
    {
      return default (TTransaction);
    }

    public override bool IsNull
    {
      get { return true; }
    }

    public override TransactionStrategyBase OuterTransactionStrategy
    {
      get { return _outerTransactionStrategy; }
    }

    public override void OnExecutionPlay (WxeContext context, IWxeFunctionExecutionListener listener)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("listener", listener);

      listener.OnExecutionPlay (context);
    }

    public override void OnExecutionStop (WxeContext context, IWxeFunctionExecutionListener listener)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("listener", listener);

      listener.OnExecutionStop (context);
    }

    public override void OnExecutionPause (WxeContext context, IWxeFunctionExecutionListener listener)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("listener", listener);

      listener.OnExecutionPause (context);
    }

    public override void OnExecutionFail (WxeContext context, IWxeFunctionExecutionListener listener, Exception exception)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("listener", listener);

      listener.OnExecutionFail (context, exception);
    }
  }
}