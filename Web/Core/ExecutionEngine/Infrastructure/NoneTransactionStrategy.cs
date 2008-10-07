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
  public class NoneTransactionStrategy : TransactionStrategyBase
  {
    public NoneTransactionStrategy (ITransactionStrategy parent, IWxeFunctionExecutionContext executionContext)
      : base (false, parent, executionContext)
    {
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

    public override void RegisterObjects (IEnumerable objects)
    {
      ArgumentUtility.CheckNotNull ("objects", objects);
      Parent.RegisterObjects (objects);
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

    public override TTransaction GetNativeTransaction<TTransaction> ()
    {
      return default (TTransaction);
    }

    public override bool IsNull
    {
      get { return true; }
    }
  }
}