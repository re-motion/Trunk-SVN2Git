using System;
using Remotion.Data;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  public class TestTransactionScope : ITransactionScope<TestTransaction>
  {
    public static TestTransactionScope CurrentScope;

    private readonly TestTransaction _scopedTransaction;
    private readonly TestTransactionScope _previousScope;
    private bool _left = false;

    public TestTransactionScope (TestTransaction scopedTransaction)
    {
      _scopedTransaction = scopedTransaction;
      _previousScope = CurrentScope;
      CurrentScope = this;
    }

    public TestTransaction ScopedTransaction
    {
      get { return _scopedTransaction; }
    }

    public void Leave ()
    {
      if (_left)
        throw new InvalidOperationException ("Has already been left.");
      CurrentScope = _previousScope;
      _left = true;
    }
  }
}