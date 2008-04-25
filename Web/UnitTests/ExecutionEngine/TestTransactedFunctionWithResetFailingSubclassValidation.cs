using System.Threading;
using NUnit.Framework;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  public class TestTransactedFunctionWithResetFailingSubclassValidation : WxeTransactedFunctionBase<TestTransaction>
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
      ResetTransaction ();
    }
  }
}