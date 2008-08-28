/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Threading;
using NUnit.Framework;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;

namespace Remotion.Web.UnitTests.ExecutionEngine.TestFunctions
{
  public class TestTransactedFunctionWithThreadAbort : WxeTransactedFunctionBase<TestTransaction>
  {
    public bool FirstStepExecuted = false;
    public bool SecondStepExecuted = false;
    public bool ThreadAborted = false;
    private TestTransaction TransactionInFirstStep;

    protected override WxeTransactionBase<TestTransaction> CreateWxeTransaction ()
    {
      return new TestWxeTransaction ();
    }

    protected override TestTransaction CreateRootTransaction ()
    {
      return new TestTransaction ();
    }

    private void Step1 ()
    {
      TestTransaction parentTransaction = GetParentTransaction ();
      Assert.AreNotSame (parentTransaction, TestTransaction.Current);

      Assert.IsFalse (FirstStepExecuted);
      TransactionInFirstStep = TestTransaction.Current;
      FirstStepExecuted = true;
    }

    private TestTransaction GetParentTransaction ()
    {
      return ((TestTransactedFunctionWithNestedFunction) ParentFunction).Transaction;
    }

    private void Step2 ()
    {
      Assert.IsTrue (FirstStepExecuted);
      Assert.IsFalse (SecondStepExecuted);
      Assert.AreSame (TransactionInFirstStep, TestTransaction.Current);
      if (!ThreadAborted)
      {
        ThreadAborted = true;
        Thread.CurrentThread.Abort ();
      }
      else
      {
        SecondStepExecuted = true;
      }
    }
  }
}