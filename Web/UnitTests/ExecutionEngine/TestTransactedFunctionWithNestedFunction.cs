using NUnit.Framework;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  public class TestTransactedFunctionWithNestedFunction : WxeTransactedFunctionBase<TestTransaction>
  {
    private readonly TestTransaction _transactionBeforeExecution;
    private TestTransaction _transactionInExecution;

    protected override WxeTransactionBase<TestTransaction> CreateWxeTransaction ()
    {
      return new TestWxeTransaction ();
    }

    protected override TestTransaction CreateRootTransaction ()
    {
      TestTransaction newTransaction = new TestTransaction ();
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