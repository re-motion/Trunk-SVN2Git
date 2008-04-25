using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  public class TestFunctionWithSpecificTransaction : WxeTransactedFunctionBase<TestTransaction>
  {
    private readonly TestTransaction _transaction;
    
    public TestTransaction TransactionInFirstStep;
    public TestWxeTransaction WxeTransaction;

    public TestFunctionWithSpecificTransaction (TestTransaction transaction)
    {
      _transaction = transaction;
    }

    protected override WxeTransactionBase<TestTransaction> CreateWxeTransaction ()
    {
      WxeTransaction = new TestWxeTransaction ();
      return WxeTransaction;
    }

    protected override TestTransaction CreateRootTransaction ()
    {
      return _transaction;
    }

    private void Step1 ()
    {
      TransactionInFirstStep = TestTransaction.Current;
    }
  }
}