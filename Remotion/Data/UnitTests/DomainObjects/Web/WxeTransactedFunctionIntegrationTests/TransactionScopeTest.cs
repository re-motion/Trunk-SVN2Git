using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.Web.WxeTransactedFunctionIntegrationTests.WxeFunctions;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.UnitTests.DomainObjects.Web.WxeTransactedFunctionIntegrationTests
{
  [TestFixture]
  public class TransactionScopeTest : WxeTransactedFunctionIntegrationTestBase
  {
    [Test]
    public void Execute_CreatesTransaction_ThenRestoresOriginal ()
    {
      Assert.That (ClientTransactionScope.HasCurrentTransaction, Is.False);

      ExecuteDelegateInWxeFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, (ctx, f) =>
      {
        Assert.That (ClientTransactionScope.HasCurrentTransaction, Is.True);
        Assert.That (f.Transaction.GetNativeTransaction<ClientTransaction> (), Is.Not.Null.And.SameAs (ClientTransaction.Current));
      });

      Assert.That (ClientTransactionScope.HasCurrentTransaction, Is.False);
    }

    [Test]
    public void RemoveCurrentScopeFromWithinFunctionThrows ()
    {
      try
      {
        new RemoveCurrentTransactionScopeFunction ().Execute (Context);
      }
      catch (WxeFatalExecutionException ex)
      {
        Assert.IsInstanceOf (typeof (InvalidOperationException), ex.InnerException);
        Assert.That (ex.InnerException.Message, Is.EqualTo ("The ClientTransactionScope has already been left."));
      }
    }

    [Test]
    public void RemoveCurrentScopeFromWithinFunctionThrowsWithPreviouslyExistingScope ()
    {
      try
      {
        ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ();
        new RemoveCurrentTransactionScopeFunction ().Execute (Context);
      }
      catch (WxeFatalExecutionException ex)
      {
        Assert.IsInstanceOf (typeof (InvalidOperationException), ex.InnerException);
        Assert.That (ex.InnerException.Message, Is.EqualTo ("The ClientTransactionScope has already been left."));
      }
    }
  }
}