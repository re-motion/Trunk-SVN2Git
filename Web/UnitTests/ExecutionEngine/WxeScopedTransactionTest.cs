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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  [TestFixture]
  public class WxeScopedTransactionTest
  {
    [SetUp]
    public void SetUp ()
    {
      TestTransactionScope.CurrentScope = null;
    }

    [TearDown]
    public void TearDown ()
    {
      TestTransactionScope.CurrentScope = null;
    }

    [Test]
    public void Constructor0 ()
    {
      TestableWxeScopedTransaction transaction = new TestableWxeScopedTransaction ();
      Assert.That (transaction.Count, Is.EqualTo (0));
      Assert.That (transaction.AutoCommit);
      Assert.That (transaction.ForceRoot);
    }

    [Test]
    public void Constructor1 ()
    {
      TestableWxeScopedTransaction transaction = new TestableWxeScopedTransaction (false);
      Assert.That (transaction.Count, Is.EqualTo (0));
      Assert.That (transaction.AutoCommit, Is.False);
      Assert.That (transaction.ForceRoot);
    }

    [Test]
    public void Constructor2 ()
    {
      TestableWxeScopedTransaction transaction = new TestableWxeScopedTransaction (false, false);
      Assert.That (transaction.Count, Is.EqualTo (0));
      Assert.That (transaction.AutoCommit, Is.False);
      Assert.That (transaction.ForceRoot, Is.False);
    }

    [Test]
    public void Constructor3 ()
    {
      WxeStepList steps = new WxeStepList ();
      steps.Add (new TestStep ());

      TestableWxeScopedTransaction transaction = new TestableWxeScopedTransaction (steps, false, false);
      Assert.That (transaction.Count, Is.EqualTo (steps.Count));
      Assert.That (transaction.AutoCommit, Is.False);
      Assert.That (transaction.ForceRoot, Is.False);
    }

    [Test]
    public void CurrentTransaction_NoActiveScope ()
    {
      TestableWxeScopedTransaction transaction = new TestableWxeScopedTransaction ();
      Assert.That (transaction.CurrentTransaction, Is.Null);
    }

    [Test]
    public void CurrentTransaction_ActiveScope_WithNull ()
    {
      TestTransactionScope.CurrentScope = new TestTransactionScope (null);
      TestableWxeScopedTransaction transaction = new TestableWxeScopedTransaction ();
      Assert.That (transaction.CurrentTransaction, Is.Null);
    }

    [Test]
    public void CurrentTransaction_ActiveScope_NonNull ()
    {
      TestTransaction testTransaction = new TestTransaction ();
      TestTransactionScope.CurrentScope = new TestTransactionScope (testTransaction);
      
      TestableWxeScopedTransaction transaction = new TestableWxeScopedTransaction ();
      Assert.That (transaction.CurrentTransaction, Is.SameAs (testTransaction));
    }

    [Test]
    public void CheckCurrentTransactionResettable_Succeeds ()
    {
      TestTransaction testTransaction = new TestTransaction ();
      TestTransactionScope.CurrentScope = new TestTransactionScope (testTransaction);

      TestableWxeScopedTransaction transaction = new TestableWxeScopedTransaction ();
      transaction.CheckCurrentTransactionResettable ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current transaction.")]
    public void CheckCurrentTransactionResettable_NoCurrentTransaction ()
    {
      TestableWxeScopedTransaction transaction = new TestableWxeScopedTransaction ();
      transaction.CheckCurrentTransactionResettable();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The current transaction cannot be reset as it is read-only. The reason" 
        + " might be an open child transaction.")]
    public void CheckCurrentTransactionResettable_ReadOnly ()
    {
      TestTransaction testTransaction = new TestTransaction ();
      testTransaction.IsReadOnly = true;
      TestTransactionScope.CurrentScope = new TestTransactionScope (testTransaction);

      TestableWxeScopedTransaction transaction = new TestableWxeScopedTransaction ();
      transaction.CheckCurrentTransactionResettable ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The current transaction cannot be reset as it is in a dirty state and " 
        + "needs to be committed or rolled back.")]
    public void CheckCurrentTransactionResettable_UncommittedChanges ()
    {
      TestTransaction testTransaction = new TestTransaction ();
      testTransaction.HasUncommittedChanges = true;
      TestTransactionScope.CurrentScope = new TestTransactionScope (testTransaction);

      TestableWxeScopedTransaction transaction = new TestableWxeScopedTransaction ();
      transaction.CheckCurrentTransactionResettable ();
    }

    [Test]
    public void SetCurrentTransaction ()
    {
      TestTransaction testTransaction = new TestTransaction ();
      TestableWxeScopedTransaction transaction = new TestableWxeScopedTransaction ();
      transaction.SetCurrentTransaction (testTransaction);
      Assert.That (TestTransactionScope.CurrentScope.ScopedTransaction, Is.SameAs (testTransaction));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "RestorePreviousTransaction must never be called more often than " 
        + "SetCurrentTransaction.")]
    public void SetPreviousCurrentTransaction_CalledTooOften ()
    {
      TestTransaction testTransaction = new TestTransaction ();
      TestableWxeScopedTransaction transaction = new TestableWxeScopedTransaction ();
      transaction.SetPreviousCurrentTransaction (testTransaction);
    }

    [Test]
    [ExpectedException (typeof (InconsistentClientTransactionScopeException), ExpectedMessage = "Somebody else has removed the active transaction " 
        + "scope.")]
    public void SetPreviousCurrentTransaction_NoActiveScope ()
    {
      TestTransaction testTransaction = new TestTransaction ();
      TestableWxeScopedTransaction transaction = new TestableWxeScopedTransaction ();
      transaction.SetCurrentTransaction (testTransaction);
      TestTransactionScope.CurrentScope = null;
      transaction.SetPreviousCurrentTransaction (testTransaction);
    }

    [Test]
    [ExpectedException (typeof (InconsistentClientTransactionScopeException), ExpectedMessage = "The active transaction scope does not contain the " 
        + "expected scope.")]
    public void SetPreviousCurrentTransaction_InvalidActiveScope ()
    {
      TestTransaction testTransaction = new TestTransaction ();
      TestableWxeScopedTransaction transaction = new TestableWxeScopedTransaction ();
      transaction.SetCurrentTransaction (testTransaction);
      new TestTransactionScope(null);
      transaction.SetPreviousCurrentTransaction (testTransaction);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The given transaction is not the previous transaction.\r\n" 
        + "Parameter name: previousTransaction")]
    public void SetPreviousCurrentTransaction_InvalidPreviousTransaction ()
    {
      TestTransaction previousTestTransaction = new TestTransaction ();
      TestTransaction newTestTransaction = new TestTransaction ();

      TestableWxeScopedTransaction transaction = new TestableWxeScopedTransaction ();
      new TestTransactionScope (previousTestTransaction);
      transaction.SetCurrentTransaction (newTestTransaction);
      transaction.SetPreviousCurrentTransaction (newTestTransaction);
    }

    [Test]
    public void SetPreviousCurrentTransaction_RestoresPreviousScope ()
    {
      TestTransaction previousTestTransaction = new TestTransaction ();
      TestTransaction newTestTransaction = new TestTransaction ();

      TestableWxeScopedTransaction transaction = new TestableWxeScopedTransaction ();
      TestTransactionScope previousScope = new TestTransactionScope (previousTestTransaction);
      transaction.SetCurrentTransaction (newTestTransaction);
      Assert.That (TestTransactionScope.CurrentScope, Is.Not.SameAs (previousScope));
      transaction.SetPreviousCurrentTransaction (previousTestTransaction);
      Assert.That (TestTransactionScope.CurrentScope, Is.SameAs (previousScope));
    }

    [Test]
    public void SetPreviousCurrentTransaction_RestoresPreviousScope_WithThreadTransition ()
    {
      TestTransaction previousTestTransaction = new TestTransaction ();
      TestTransaction newTestTransaction = new TestTransaction ();

      TestableWxeScopedTransaction transaction = new TestableWxeScopedTransaction ();
      // no previous scope
      transaction.SetCurrentTransaction (newTestTransaction);

      transaction.SetPreviousCurrentTransaction (previousTestTransaction);
      Assert.That (TestTransactionScope.CurrentScope, Is.Not.Null);
      Assert.That (TestTransactionScope.CurrentScope.ScopedTransaction, Is.SameAs (previousTestTransaction));
    }

  }
}
