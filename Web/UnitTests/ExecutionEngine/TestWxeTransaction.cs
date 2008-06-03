/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  public class TestWxeTransaction : WxeTransactionBase<TestTransaction>
  {
    private readonly Stack<TestTransaction> _previousTransactionsStack = new Stack<TestTransaction> ();

    public TestWxeTransaction ()
        : base (null, false, true)
    {
    }

    public TestWxeTransaction (bool forceRoot)
      : base (null, false, forceRoot)
    {
    }

    protected override TestTransaction CurrentTransaction
    {
      get { return TestTransaction.Current; }
    }

    protected override void CheckCurrentTransactionResettable ()
    {
      // always succeeds
    }

    protected override void SetCurrentTransaction (TestTransaction transaction)
    {
      _previousTransactionsStack.Push (TestTransaction.Current);
      TestTransaction.Current = transaction;
    }

    protected override void SetPreviousCurrentTransaction (TestTransaction previousTransaction)
    {
      TestTransaction storedPreviousTransaction = _previousTransactionsStack.Pop ();
      Assert.AreSame (storedPreviousTransaction, previousTransaction);
      TestTransaction.Current = previousTransaction;
    }
  }
}
