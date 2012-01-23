// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction
{
  [TestFixture]
  public class ClientTransactionWrapperTest : ClientTransactionBaseTest
  {
    private ITransaction _transaction;

    public override void SetUp ()
    {
      base.SetUp();

      _transaction = TestableClientTransaction.ToITransation();
    }

    [Test]
    public void To_ClientTransaction ()
    {
      var actual = _transaction.To<TestableClientTransaction>();

      Assert.That (actual, Is.SameAs (TestableClientTransaction));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage =
        "Argument TTransaction is a Remotion.Data.DomainObjects.DomainObject, "
        + "which cannot be assigned to type Remotion.Data.DomainObjects.ClientTransaction.\r\nParameter name: TTransaction")]
    public void To_InvalidType ()
    {
      _transaction.To<DomainObject>();
    }

    [Test]
    public void CanCreateChild ()
    {
      Assert.IsTrue (_transaction.CanCreateChild);
    }

    [Test]
    public void CreateChild ()
    {
      ITransaction child = _transaction.CreateChild();
      Assert.IsNotNull (child);
      Assert.IsInstanceOf (typeof (ClientTransactionWrapper), child);
      Assert.IsInstanceOf (typeof (ClientTransaction), ((ClientTransactionWrapper) child).WrappedInstance);

      var persistenceStrategy = ClientTransactionTestHelper.GetPersistenceStrategy (((ClientTransactionWrapper) child).WrappedInstance);
      Assert.IsInstanceOf (typeof (SubPersistenceStrategy), persistenceStrategy);
    }

    [Test]
    public void IsChild ()
    {
      ITransaction child = _transaction.CreateChild();
      Assert.IsTrue (child.IsChild);
      Assert.IsFalse (_transaction.IsChild);
      Assert.IsTrue (child.CreateChild().IsChild);
    }

    [Test]
    public void Parent ()
    {
      ITransaction child = _transaction.CreateChild();
      Assert.AreSame (((ClientTransactionWrapper) _transaction).WrappedInstance, ((ClientTransactionWrapper) child.Parent).WrappedInstance);
      Assert.AreSame (((ClientTransactionWrapper) child).WrappedInstance, ((ClientTransactionWrapper) child.CreateChild().Parent).WrappedInstance);
    }

    [Test]
    public void Release ()
    {
      ITransaction child = _transaction.CreateChild();
      Assert.IsTrue (((ClientTransactionWrapper) _transaction).WrappedInstance.IsReadOnly);
      Assert.IsFalse (((ClientTransactionWrapper) child).WrappedInstance.IsDiscarded);
      child.Release();
      Assert.IsFalse (((ClientTransactionWrapper) _transaction).WrappedInstance.IsReadOnly);
      Assert.IsTrue (((ClientTransactionWrapper) child).WrappedInstance.IsDiscarded);
    }

    [Test]
    public void EnterScope ()
    {
      ITransaction transaction = ClientTransaction.CreateRootTransaction().ToITransation();

      ClientTransactionScope.ResetActiveScope();
      Assert.That (ClientTransactionScope.ActiveScope, Is.Null);

      ITransactionScope transactionScope = transaction.EnterScope();

      Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (transactionScope));
      Assert.That (ClientTransactionScope.ActiveScope.ScopedTransaction, Is.SameAs (((ClientTransactionWrapper) transaction).WrappedInstance));
      Assert.That (ClientTransactionScope.ActiveScope.AutoRollbackBehavior, Is.EqualTo (AutoRollbackBehavior.None));
      ClientTransactionScope.ResetActiveScope();
    }

    [Test]
    public void RegisterObjects ()
    {
      ClientTransaction firstClientTransaction = ClientTransaction.CreateRootTransaction();

      var domainObject1 = LifetimeService.GetObject (firstClientTransaction, DomainObjectIDs.ClassWithAllDataTypes2, false);
      var domainObject2 = LifetimeService.GetObject (firstClientTransaction, DomainObjectIDs.Partner1, false);

      var secondClientTransaction = TestableClientTransaction;
      ITransaction secondTransaction = secondClientTransaction.ToITransation();
      Assert.That (secondClientTransaction.IsEnlisted (domainObject1), Is.False);
      Assert.That (secondClientTransaction.IsEnlisted (domainObject2), Is.False);

      secondTransaction.RegisterObjects (new object[] { null, domainObject1, 1, domainObject2, domainObject2 });

      Assert.That (secondClientTransaction.IsEnlisted (domainObject1), Is.True);
      Assert.That (secondClientTransaction.Execute (() => domainObject1.State), Is.EqualTo (StateType.NotLoadedYet));

      Assert.That (secondClientTransaction.IsEnlisted (domainObject2), Is.True);
      Assert.That (secondClientTransaction.Execute (() => domainObject2.State), Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void RegisterObjects_WithNewObject ()
    {
      ClientTransaction firstClientTransaction = ClientTransaction.CreateRootTransaction();

      var domainObject = LifetimeService.NewObject (firstClientTransaction, typeof (Partner), ParamList.Empty);

      var secondClientTransaction = TestableClientTransaction;
      ITransaction secondTransaction = secondClientTransaction.ToITransation();

      secondTransaction.RegisterObjects (new object[] { domainObject });

      Assert.That (secondClientTransaction.Execute (() => domainObject.State), Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (() => secondClientTransaction.Execute (domainObject.EnsureDataAvailable), Throws.TypeOf<ObjectsNotFoundException>());
    }

    [Test]
    public void Reset ()
    {
      var clientTransactionBefore = ClientTransaction.CreateRootTransaction();
      var clientTransactionWrapper = (ClientTransactionWrapper) clientTransactionBefore.ToITransation();

      Order order = clientTransactionBefore.Execute (() => Order.GetObject (DomainObjectIDs.Order1));
      clientTransactionBefore.Execute (() => order.OrderNumber = 7);
      clientTransactionBefore.Rollback ();

      clientTransactionWrapper.Reset ();

      var clientTransactionAfter = clientTransactionWrapper.WrappedInstance;

      Assert.That (clientTransactionAfter, Is.Not.SameAs (clientTransactionBefore));
      Assert.That (clientTransactionBefore.IsDiscarded, Is.True);

      Assert.That (clientTransactionAfter.IsEnlisted (order), Is.False);
    }

    [Test]
    [Ignore ("TODO 4592")]
    public void Reset_InvalidObjectsStayInvalid ()
    {
      var clientTransactionBefore = ClientTransaction.CreateRootTransaction ();
      var clientTransactionWrapper = (ClientTransactionWrapper) clientTransactionBefore.ToITransation ();

      var invalidObject = clientTransactionBefore.Execute (() => Order.NewObject ());
      clientTransactionBefore.Execute (invalidObject.Delete);

      clientTransactionWrapper.Reset ();

      var clientTransactionAfter = clientTransactionWrapper.WrappedInstance;
      Assert.That (clientTransactionAfter.IsInvalid (invalidObject.ID), Is.True);
      Assert.That (clientTransactionAfter.IsEnlisted (invalidObject), Is.True);
    }

    [Test]
    public void Reset_SubTransaction ()
    {
      var rootTransaction = ClientTransaction.CreateRootTransaction ();
      var clientTransactionBefore = rootTransaction.CreateSubTransaction();
      var clientTransactionWrapper = (ClientTransactionWrapper) clientTransactionBefore.ToITransation ();

      Order order = clientTransactionBefore.Execute (() => Order.GetObject (DomainObjectIDs.Order1));
      clientTransactionBefore.Execute (() => order.OrderNumber = 7);
      clientTransactionBefore.Rollback ();

      bool addedCalled = false;
      bool loadedCalled = false;

      clientTransactionBefore.Execute (() => order.OrderItems.Added += delegate { addedCalled = true; });
      clientTransactionBefore.Loaded += delegate { loadedCalled = true; };
      
      clientTransactionWrapper.Reset ();

      var clientTransactionAfter = clientTransactionWrapper.WrappedInstance;
      Assert.That (clientTransactionAfter, Is.Not.SameAs (clientTransactionBefore));
      Assert.That (clientTransactionBefore.IsDiscarded, Is.True);
      Assert.That (clientTransactionAfter.ParentTransaction, Is.SameAs (rootTransaction));

      Assert.That (clientTransactionAfter.IsEnlisted (order), Is.True);
      Assert.That (clientTransactionAfter.Execute (() => order.OrderNumber), Is.EqualTo (1));

      Assert.That (addedCalled, Is.False);
      clientTransactionAfter.Execute (() => order.OrderItems.Add (OrderItem.NewObject ()));
      Assert.That (addedCalled, Is.False, "Collection event handlers registered in old transaction are no longer valid.");

      loadedCalled = false;
      clientTransactionAfter.Execute (() => Order.GetObject (DomainObjectIDs.Order2));

      Assert.That (loadedCalled, Is.False, "Transaction event handlers registered in old transaction are no longer valid.");
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "The transaction cannot be reset as it is read-only. The reason might be an open child transaction.")]
    public void Reset_WithSubTransaction_Throws ()
    {
      ClientTransaction rootTransaction = ClientTransaction.CreateRootTransaction();
      rootTransaction.CreateSubTransaction();
      ITransaction transaction = rootTransaction.ToITransation ();
      transaction.Reset();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "The transaction cannot be reset as it is in a dirty state and needs to be committed or rolled back.")]
    public void Reset_HasChanged_Throws ()
    {
      ClientTransaction rootTransaction = ClientTransaction.CreateRootTransaction();
      ITransaction transaction = rootTransaction.ToITransation();
      using (rootTransaction.EnterNonDiscardingScope())
      {
        Order.NewObject();
        transaction.Reset();
      }
    }

    [Test]
    [Ignore ("TODO: Test")]
    public void TransactionResettableWhenNotReadOnly ()
    {
      //using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      //{
      //  WxeScopedTransaction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager> tx = new WxeScopedTransaction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager> ();
      //  PrivateInvoke.InvokeNonPublicMethod (tx, "CheckCurrentTransactionResettable");
      //}
    }

    [Test]
    [Ignore ("TODO: Test")]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The current transaction cannot be reset as it is read-only. "
                                                                              + "The reason might be an open child transaction.")]
    public void TransactionNotResettableWhenReadOnly ()
    {
      //using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      //{
      //  ClientTransactionScope.CurrentTransaction.CreateSubTransaction ();

      //  WxeScopedTransaction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager> tx = new WxeScopedTransaction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager> ();
      //  PrivateInvoke.InvokeNonPublicMethod (tx, "CheckCurrentTransactionResettable");
      //}
    }

    [Test]
    [Ignore ("TODO: Test")]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current transaction.")]
    public void TransactionNotResettableWhenNull ()
    {
      //using (ClientTransactionScope.EnterNullScope ())
      //{
      //  WxeScopedTransaction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager> tx = new WxeScopedTransaction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager> ();
      //  PrivateInvoke.InvokeNonPublicMethod (tx, "CheckCurrentTransactionResettable");
      //}
    }

    [Test]
    [Ignore ("TODO: Test")]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The current transaction cannot be reset as it is in a dirty state and "
                                                                              + "needs to be committed or rolled back.")]
    public void TransactionNotResettableWhenNewObject ()
    {
      //using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      //{
      //  WxeScopedTransaction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager> tx = new WxeScopedTransaction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager> ();
      //  Order.NewObject ();
      //  PrivateInvoke.InvokeNonPublicMethod (tx, "CheckCurrentTransactionResettable");
      //}
    }

    [Test]
    [Ignore ("TODO: Test")]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The current transaction cannot be reset as it is in a dirty state and "
                                                                              + "needs to be committed or rolled back.")]
    public void TransactionNotResettableWhenChangedObject ()
    {
      //using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      //{
      //  WxeScopedTransaction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager> tx = new WxeScopedTransaction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager> ();
      //  ++Order.GetObject (DomainObjectIDs.Order1).OrderNumber;
      //  PrivateInvoke.InvokeNonPublicMethod (tx, "CheckCurrentTransactionResettable");
      //}
    }

    [Test]
    [Ignore ("TODO: Test")]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The current transaction cannot be reset as it is in a dirty state and "
                                                                              + "needs to be committed or rolled back.")]
    public void TransactionNotResettableWhenChangedRelation ()
    {
      //using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      //{
      //  WxeScopedTransaction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager> tx = new WxeScopedTransaction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager> ();
      //  Order.GetObject (DomainObjectIDs.Order1).OrderItems.Clear ();
      //  PrivateInvoke.InvokeNonPublicMethod (tx, "CheckCurrentTransactionResettable");
      //}
    }

    [Test]
    public void CanBeDerivedFrom ()
    {
      var ctor =  typeof (ClientTransactionWrapper).GetConstructor (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, 
          new[] {typeof (ClientTransaction)}, null);
      Assert.That (typeof (ClientTransactionWrapper).IsSealed, Is.False);
      Assert.That (ctor.IsFamilyOrAssembly);
    }
  }
}
