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
using Rhino.Mocks;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  [TestFixture]
  public class WxeTransactionBaseExecuteTest : WxeTest
  {
    [Test]
    public void SimpleExecute ()
    {
      PerformExecuteAndCheckTransaction (false, delegate { }, delegate { }); // empty steps
    }

    [Test]
    public void SetAndResetTransaction ()
    {
      TestTransaction.Current = new TestTransaction ();

      TestTransaction executeTransaction = null;
      PerformExecuteAndCheckTransaction (false, delegate { executeTransaction = TestTransaction.Current; });
      Assert.IsFalse (executeTransaction.IsCommitted);
    }

    [Test]
    public void SetAndResetTransactionWithAutoCommit ()
    {
      TestTransaction.Current = new TestTransaction ();

      TestTransaction executeTransaction = null;
      PerformExecuteAndCheckTransaction (true, delegate { executeTransaction = TestTransaction.Current; });
      Assert.IsTrue (executeTransaction.IsCommitted);
    }

    [Test]
    public void SetAndResetTransaction_WithException ()
    {
      TestTransaction.Current = new TestTransaction ();

      TestTransaction executeTransaction = null;
      try
      {
        PerformExecuteAndCheckTransaction (false, delegate
        {
          executeTransaction = TestTransaction.Current;
          throw new ArgumentException ("fifi");
        });
        Assert.Fail ("Expected exception");
      }
      catch (ArgumentException ex)
      {
        Assert.AreEqual ("fifi", ex.Message);
      }
      
      Assert.IsFalse (executeTransaction.IsCommitted);
    }

    [Test]
    public void SetAndResetTransaction_AutoCommit_WithException ()
    {
      TestTransaction.Current = new TestTransaction ();

      TestTransaction executeTransaction = null;
      try
      {
        PerformExecuteAndCheckTransaction (true, delegate
        {
          executeTransaction = TestTransaction.Current;
          throw new ArgumentException ("fifi");
        });
        Assert.Fail ("Expected exception");
      }
      catch (ArgumentException ex)
      {
        Assert.AreEqual ("fifi", ex.Message);
      }

      Assert.IsFalse (executeTransaction.IsCommitted);
    }

    [Test]
    public void SetAndResetTransaction_AutoCommit_WithCommitException ()
    {
      TestTransaction.Current = new TestTransaction ();
      try
      {
        PerformExecuteAndCheckTransaction (true, delegate
        {
          TestTransaction.Current.ThrowOnCommit = true;
        });
        Assert.Fail ("Expected exception");
      }
      catch (CommitException)
      {
        // expected
      }
    }

    [Test]
    public void SetAndResetTransaction_NoAutoCommit_WithRollbackException ()
    {
      TestTransaction.Current = new TestTransaction ();
      try
      {
        PerformExecuteAndCheckTransaction (false, delegate
        {
          TestTransaction.Current.ThrowOnRollback = true;
        });
        Assert.Fail ("Expected exception");
      }
      catch (RollbackException)
      {
        // expected
      }
    }

    private void PerformExecuteAndCheckTransaction (bool autoCommit, params Proc<WxeContext>[] stepDelegates)
    {
      MockRepository mockRepository = new MockRepository ();

      WxeStepList steps = new WxeStepList ();
      for (int i = 0; i < stepDelegates.Length; i++)
      {
        WxeStep step = mockRepository.CreateMock<WxeStep> ();
        steps.Add (step);
      }

      WxeTransactionMock transaction = new WxeTransactionMock (steps, autoCommit, false);

      TestTransaction originalTransaction = TestTransaction.Current;

      // expectations
      for (int i = 0; i < stepDelegates.Length; i++)
      {
        steps[i].Execute (CurrentWxeContext);

        Proc<WxeContext> stepDelegate = stepDelegates[i];
        LastCall.Do ((Proc<WxeContext>) delegate
        {
          Assert.AreNotSame (originalTransaction, TestTransaction.Current, "WxeTransactionBase must set a new current transaction");
          Assert.AreEqual (1, transaction.PreviousTransactions.Count);

          stepDelegate (CurrentWxeContext);
        });
      }

      mockRepository.ReplayAll ();

      try
      {
        transaction.Execute (CurrentWxeContext);
      }
      finally
      {
        mockRepository.VerifyAll ();

        Assert.AreEqual (0, transaction.PreviousTransactions.Count, "WxeTransactionBase must restore original transaction.");
        Assert.AreSame (originalTransaction, TestTransaction.Current);
      }
    }
  }
}
