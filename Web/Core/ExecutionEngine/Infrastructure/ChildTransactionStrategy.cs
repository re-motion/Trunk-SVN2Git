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
  public class ChildTransactionStrategy : TransactionStrategyBase
  {
    public ChildTransactionStrategy (bool autoCommit, ITransactionStrategy parent, IWxeFunctionExecutionContext executionContext)
      : base (autoCommit, parent, executionContext)
    {
    }

    public override void Commit ()
    {
      throw new System.NotImplementedException ();
    }

    public override void Rollback ()
    {
      throw new System.NotImplementedException ();
    }

    public override void Reset ()
    {
      throw new System.NotImplementedException ();
    }

    public override IWxeFunctionExecutionListener CreateExecutionListener (IWxeFunctionExecutionListener innerListener)
    {
      throw new System.NotImplementedException();
    }

    public override void RegisterObjects (IEnumerable objects)
    {
      throw new System.NotImplementedException ();
    }

    public override void OnExecutionPlay (WxeContext context, IWxeFunctionExecutionListener listener)
    {
      throw new System.NotImplementedException();
    }

    public override void OnExecutionStop (WxeContext context, IWxeFunctionExecutionListener listener)
    {
      throw new System.NotImplementedException();
    }

    public override void OnExecutionPause (WxeContext context, IWxeFunctionExecutionListener listener)
    {
      throw new System.NotImplementedException();
    }

    public override void OnExecutionFail (WxeContext context, IWxeFunctionExecutionListener listener, Exception exception)
    {
      throw new System.NotImplementedException();
    }

    public override TTransaction GetNativeTransaction<TTransaction> ()
    {
      throw new System.NotImplementedException();
    }

    public override bool IsNull
    {
      get { return false; }
    }
  }
}