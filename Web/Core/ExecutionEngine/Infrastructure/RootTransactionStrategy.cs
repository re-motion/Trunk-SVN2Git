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
using Remotion.Data;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  //TODO: Doc
  public class RootTransactionStrategy : ScopedTransactionStrategyBase
  {
    public RootTransactionStrategy (
        bool autoCommit, ITransaction transaction, TransactionStrategyBase outerTransactionStrategy, IWxeFunctionExecutionContext executionContext)
      : base (autoCommit, transaction, outerTransactionStrategy, executionContext)
    {
    }

    public override IWxeFunctionExecutionListener CreateExecutionListener (IWxeFunctionExecutionListener innerListener)
    {
      ArgumentUtility.CheckNotNull ("innerListener", innerListener);

      return new TransactionExecutionListener (this, innerListener);
    }

    public override ScopedTransactionStrategyBase CreateChildTransactionStrategy (bool autoCommit, IWxeFunctionExecutionContext executionContext)
    {
      ArgumentUtility.CheckNotNull ("executionContext", executionContext);

      ChildTransactionStrategy childTransactionStrategy = new ChildTransactionStrategy (autoCommit, Transaction.CreateChild(), this, executionContext);
      SetChild (childTransactionStrategy);

      return childTransactionStrategy;
    }
  }
}