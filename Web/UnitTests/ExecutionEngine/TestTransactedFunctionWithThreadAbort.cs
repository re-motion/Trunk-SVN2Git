using System.Threading;
using NUnit.Framework;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
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