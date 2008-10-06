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

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  //TODO: Doc
  public class NoneTransactionStrategy : TransactionStrategyBase
  {
    public NoneTransactionStrategy (ITransactionStrategy parent, IWxeFunctionExecutionListener innerListener, IWxeFunctionExecutionContext executionContext)
      : base (false, parent, innerListener, executionContext)
    {
    }

    public override void OnExecutionPlay (WxeContext context)
    {
      InnerListener.OnExecutionPlay (context);
    }

    public override void OnExecutionStop (WxeContext context)
    {
      InnerListener.OnExecutionStop (context);
    }

    public override void OnExecutionPause (WxeContext context)
    {
      InnerListener.OnExecutionPause (context);
    }

    public override void OnExecutionFail (WxeContext context, Exception exception)
    {
      InnerListener.OnExecutionFail (context, exception);
    }

    public override void Commit ()
    {
      throw new NotSupportedException();
    }

    public override void Rollback ()
    {
      throw new NotSupportedException();
    }

    public override void Reset ()
    {
      throw new NotSupportedException();
    }

    public override TTransaction GetNativeTransaction<TTransaction> ()
    {
      return default (TTransaction);
    }

    public override void RegisterObjects (IEnumerable objects)
    {
      Parent.RegisterObjects (objects);
    }

    public override bool IsNull
    {
      get { return true; }
    }
  }
}