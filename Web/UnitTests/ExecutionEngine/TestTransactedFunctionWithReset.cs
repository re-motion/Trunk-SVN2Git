using System.Threading;
using NUnit.Framework;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  public class TestTransactedFunctionWithReset : WxeTransactedFunctionBase<TestTransaction>
  {
    private bool _createWxeTransactionCalled = false;
    private bool _createRootTransactionCalled = false;

    private readonly bool _isRoot;

    public TestTransactedFunctionWithReset (bool isRoot)
    {
      _isRoot = isRoot;
    }

    protected override WxeTransactionBase<TestTransaction> CreateWxeTransaction ()
    {
      _createWxeTransactionCalled = true;
      return new TestWxeTransaction (false);
    }

    protected override TestTransaction CreateRootTransaction ()
    {
      _createRootTransactionCalled = true;
      return new TestTransaction ();
    }

    private void Step1 ()
    {
      TestTransaction transactionBeforeReset = TestTransaction.Current;
      Assert.AreSame (transactionBeforeReset, MyTransaction);

      Assert.IsTrue (_createWxeTransactionCalled);
      
      if (_isRoot)
        Assert.IsTrue (_createRootTransactionCalled);
      else
        Assert.IsTrue (transactionBeforeReset.IsChild);

      _createWxeTransactionCalled = false;
      _createRootTransactionCalled = false;

      ResetTransaction ();

      Assert.AreNotSame (transactionBeforeReset, TestTransaction.Current);
      Assert.AreSame (TestTransaction.Current, MyTransaction);

      Assert.IsFalse (_createWxeTransactionCalled);
      if (_isRoot)
      {
        Assert.IsTrue (_createRootTransactionCalled);
        Assert.IsFalse (MyTransaction.IsChild);
      }
      else
      {
        Assert.IsFalse (_createRootTransactionCalled);
        Assert.IsTrue (MyTransaction.IsChild);
        Assert.AreSame (transactionBeforeReset.Parent, MyTransaction.Parent);
      }
    }

    public new void ResetTransaction ()
    {
      base.ResetTransaction ();
    }
  }
}