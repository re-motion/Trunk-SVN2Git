using System.Threading;
using NUnit.Framework;
using Remotion.Web.ExecutionEngine;
using Remotion.Development.UnitTesting;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  public class TestTransactedFunctionWithResetFailingMyTransactionValidation : WxeTransactedFunctionBase<TestTransaction>
  {
    protected override WxeTransactionBase<TestTransaction> CreateWxeTransaction ()
    {
      return new TestWxeTransactionFailingResetValidation ();
    }

    protected override TestTransaction CreateRootTransaction ()
    {
      return new TestTransaction ();
    }

    private void Step1 ()
    {
      WxeTransactedFunctionBase<TestTransaction> parent = ParentFunction as WxeTransactedFunctionBase<TestTransaction>;
      Assert.IsNotNull (parent, "must be tested as a nested function");
      parent.ResetTransaction ();
    }
  }
}