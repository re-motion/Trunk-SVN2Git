using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  public class TestWxeTransactionFailingResetValidation : TestWxeTransaction
  {
    protected override void CheckCurrentTransactionResettable()
    {
      throw new InvalidOperationException ("The current transaction cannot be reset.");
    }
  }
}