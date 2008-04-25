using System;
using Remotion.Data.DomainObjects.Web.ExecutionEngine;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using NUnit.Framework;

namespace Remotion.Data.DomainObjects.UnitTests.Web.WxeFunctions
{
  public class ResetTestTransactedFunction : WxeTransactedFunction
  {
    public bool CopyEventHandlers = false;

    protected override bool AutoCommit
    {
      get { return false; }
    }

    private void Step1 ()
    {
      ClientTransaction transactionBefore = ClientTransactionScope.CurrentTransaction;
      Order order = Order.GetObject (new DomainObjectIDs ().Order1);
      order.OrderNumber = 7;
      transactionBefore.Rollback ();

      bool addedCalled = false;
      order.OrderItems.Added += delegate { addedCalled = true; };

      bool loadedCalled = false;
      ClientTransactionScope.CurrentTransaction.Loaded += delegate { loadedCalled = true; };

      ResetTransaction (CopyEventHandlers);

      Assert.AreNotEqual (transactionBefore, ClientTransactionScope.CurrentTransaction);
      Assert.IsTrue (order.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));

      Assert.AreEqual (1, order.OrderNumber);

      Assert.IsFalse (addedCalled);
      order.OrderItems.Add (OrderItem.NewObject());
      Assert.AreEqual (CopyEventHandlers, addedCalled);

      loadedCalled = false;

      Order.GetObject (new DomainObjectIDs ().Order2);

      Assert.AreEqual (CopyEventHandlers, loadedCalled);
    }
  }
}