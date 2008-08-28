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
using NUnit.Framework;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine.TestFunctions
{
  public class TestTransactedFunctionWithNestedFunction : WxeTransactedFunctionBase<TestTransaction>
  {
    private readonly TestTransaction _transactionBeforeExecution;
    private TestTransaction _transactionInExecution;

    protected override WxeTransactionBase<TestTransaction> CreateWxeTransaction ()
    {
      return new TestWxeTransaction();
    }

    protected override TestTransaction CreateRootTransaction ()
    {
      TestTransaction newTransaction = new TestTransaction();
      newTransaction.CanCreateChild = true;
      return newTransaction;
    }

    public new TestTransaction Transaction
    {
      get { return base.Transaction; }
    }

    public TestTransactedFunctionWithNestedFunction (TestTransaction transactionBefore, WxeTransactedFunctionBase<TestTransaction> nestedFunction)
    {
      _transactionBeforeExecution = transactionBefore;

      Add (new WxeMethodStep (CheckBeforeNestedFunction));
      Add (nestedFunction);
      Add (new WxeMethodStep (CheckAfterNestedFunction));
    }

    private void CheckBeforeNestedFunction ()
    {
      Assert.AreNotSame (_transactionBeforeExecution, TestTransaction.Current);
      _transactionInExecution = TestTransaction.Current;
    }

    private void CheckAfterNestedFunction ()
    {
      Assert.AreSame (_transactionInExecution, TestTransaction.Current);
    }
  }
}