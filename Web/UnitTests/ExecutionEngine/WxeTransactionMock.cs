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
using System.Collections.Generic;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;
using NUnit.Framework;

namespace Remotion.Web.UnitTests.ExecutionEngine
{

  /// <summary> Provides a test implementation of the <see langword="abstract"/> <see cref="WxeTransactionBase{TTransaction}"/> type. </summary>
  [Serializable]
  public class WxeTransactionMock : WxeTransactionBase<TestTransaction>
  {
    private bool _hasCreatedRootTransaction;
    private Stack<TestTransaction> _previousTransactions = new Stack<TestTransaction> ();
    private bool _throwOnSetCurrent = false;
    private bool _throwOnSetPrevious;
    private bool _throwOnGetRootTransaction;

    public WxeTransactionMock (WxeStepList steps, bool autoCommit, bool forceRoot)
      : base (steps, autoCommit, forceRoot)
    {
    }

    protected override TestTransaction GetRootTransactionFromFunction ()
    {
      if (_throwOnGetRootTransaction)
        throw new InvalidOperationException ("Get root");

      _hasCreatedRootTransaction = true;
      return new TestTransaction ();
    }

    protected override TestTransaction CurrentTransaction
    {
      get { return TestTransaction.Current; }
    }

    protected override void CheckCurrentTransactionResettable ()
    {
      // always succeeds
    }

    public void InternalReset ()
    {
      base.Reset ();
    }

    protected override void SetCurrentTransaction (TestTransaction transaction)
    {
      Assert.That (transaction, Is.Not.Null);

      if (ThrowOnSetCurrent)
        throw new SetCurrentException ();

      _previousTransactions.Push (TestTransaction.Current);
      TestTransaction.Current = transaction;
    }

    public void PublicCheckAndSetCurrentTransaction (TestTransaction transaction)
    {
      CheckAndSetCurrentTransaction (transaction);
    }

    protected override void SetPreviousCurrentTransaction (TestTransaction previousTransaction)
    {
      if (ThrowOnSetPrevious)
        throw new SetPreviousCurrentException ();

      Assert.IsNotEmpty (_previousTransactions);
      Assert.AreSame (_previousTransactions.Pop (), previousTransaction);
      TestTransaction.Current = previousTransaction;
    }

    public void PublicCheckAndRestorePreviousCurrentTransaction ()
    {
      CheckAndRestorePreviousCurrentTransaction ();
    }

    public ArrayList Steps
    {
      get { return (ArrayList) PrivateInvoke.GetNonPublicField (this, "_steps"); }
    }

    public void StartExecution ()
    {
      PrivateInvoke.SetNonPublicField (this, "_lastExecutedStep", 0);
    }

    public new bool AutoCommit
    {
      get { return base.AutoCommit; }
      set { PrivateInvoke.SetNonPublicField (this, "_autoCommit", value); }
    }

    public new bool ForceRoot
    {
      get { return base.ForceRoot; }
      set { PrivateInvoke.SetNonPublicField (this, "_forceRoot", value); }
    }

    public bool IsPreviousCurrentTransactionRestored
    {
      get { return (bool) PrivateInvoke.GetNonPublicField (this, "_isPreviousCurrentTransactionRestored"); }
      set { PrivateInvoke.SetNonPublicField (this, "_isPreviousCurrentTransactionRestored", value); }
    }

    public new TestTransaction Transaction
    {
      get { return base.Transaction; }
      set { PrivateInvoke.SetNonPublicField (this, "_transaction", value); }
    }

    public new TestTransaction CreateTransaction ()
    {
      return base.CreateTransaction ();
    }

    public new TestTransaction CreateChildTransaction (TestTransaction parentTransaction)
    {
      return base.CreateChildTransaction (parentTransaction);
    }

    public new WxeTransactionBase<TestTransaction> GetParentTransaction ()
    {
      return base.GetParentTransaction ();
    }

    public bool HasCreatedRootTransaction
    {
      get { return _hasCreatedRootTransaction; }
    }

    public Stack<TestTransaction> PreviousTransactions
    {
      get { return _previousTransactions; }
    }

    public bool ThrowOnSetCurrent
    {
      get { return _throwOnSetCurrent; }
      set { _throwOnSetCurrent = value; }
    }

    public bool ThrowOnSetPrevious
    {
      get { return _throwOnSetPrevious; }
      set { _throwOnSetPrevious = value; }
    }

    public bool ThrowOnGetRootTransaction
    {
      get { return _throwOnGetRootTransaction; }
      set { _throwOnGetRootTransaction = value; }
    }

    public new void OnTransactionCommitting ()
    {
      base.OnTransactionCommitting ();
    }

    public new void OnTransactionCommitted ()
    {
      base.OnTransactionCommitted ();
    }

    public new void OnTransactionRollingBack ()
    {
      base.OnTransactionRollingBack ();
    }

    public new void OnTransactionRolledBack ()
    {
      base.OnTransactionRolledBack ();
    }

    public void CommitTransaction ()
    {
      base.CommitTransaction (Transaction);
    }

    public void RollbackTransaction ()
    {
      base.RollbackTransaction (Transaction);
    }

    public new void CommitAndReleaseTransaction ()
    {
      base.CommitAndReleaseTransaction ();
    }

    public new void RollbackAndReleaseTransaction ()
    {
      base.RollbackAndReleaseTransaction ();
    }

    public void PublicSetPreviousTransaction (TestTransaction transaction)
    {
      PrivateInvoke.SetNonPublicField (this, "_previousCurrentTransaction", transaction);
    }
  }
}
