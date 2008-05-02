using System;
using Remotion.Data;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  public class TestableWxeScopedTransaction : WxeScopedTransaction<TestTransaction, TestTransactionScope, TestTransactionScopeManager>
  {
    public TestableWxeScopedTransaction (WxeStepList steps, bool autoCommit, bool forceRoot)
        : base (steps, autoCommit, forceRoot)
    {
    }

    public TestableWxeScopedTransaction (bool autoCommit, bool forceRoot)
        : base (autoCommit, forceRoot)
    {
    }

    public TestableWxeScopedTransaction (bool autoCommit)
        : base (autoCommit)
    {
    }

    public TestableWxeScopedTransaction ()
    {
    }

    public new bool AutoCommit
    {
      get { return base.AutoCommit; }
    }

    public new bool ForceRoot
    {
      get { return base.ForceRoot; }
    }

    public new TestTransaction CurrentTransaction
    {
      get { return base.CurrentTransaction; }
    }

    public new void CheckCurrentTransactionResettable ()
    {
      base.CheckCurrentTransactionResettable ();
    }

    public new void SetCurrentTransaction (TestTransaction transaction)
    {
      base.SetCurrentTransaction (transaction);
    }

    public new void SetPreviousCurrentTransaction (TestTransaction previousTransaction)
    {
      base.SetPreviousCurrentTransaction (previousTransaction);
    }
  }
}