using System;
using Remotion.Data;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  public class TestTransactionScope : ITransactionScope<TestTransaction>
  {
    private static TestTransactionScope _currentScope;

    private readonly TestTransaction _scopedTransaction;
    private readonly TestTransactionScope _previousScope;
    private bool _left = false;

    public TestTransactionScope (TestTransaction scopedTransaction)
    {
      _scopedTransaction = scopedTransaction;
      _previousScope = _currentScope;
      CurrentScope = this;
    }

    public TestTransaction ScopedTransaction
    {
      get { return _scopedTransaction; }
    }

    public static TestTransactionScope CurrentScope
    {
      get { return _currentScope; }
      set
      {
        _currentScope = value;
        TestTransaction.Current = value != null ? value.ScopedTransaction : null;
      }
    }

    public void Leave ()
    {
      if (_left)
        throw new InvalidOperationException ("Has already been left.");
      _currentScope = _previousScope;
      _left = true;
    }
  }
}