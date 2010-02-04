// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  [TestFixture]
  public class DomainObjectTest : ClientTransactionBaseTest
  {
    [Test]
    public void Ctor_RaisesNewObjectCreating ()
    {
      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (ClientTransactionMock);

      var instance = Order.NewObject ();

      listenerMock.AssertWasCalled (mock => mock.NewObjectCreating (typeof (Order), instance));
    }

    [Test]
    public void Ctor_CreatesObjectID ()
    {
      var instance = Order.NewObject ();

      Assert.That (instance.ID, Is.Not.Null);
      Assert.That (instance.ID.ClassDefinition, Is.SameAs (MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order))));
    }

    [Test]
    public void Ctor_Binding ()
    {
      var bindingTransaction = ClientTransaction.CreateBindingTransaction ();
      var boundInstance = bindingTransaction.Execute (() => Order.NewObject ());

      Assert.That (boundInstance.HasBindingTransaction, Is.True);
      Assert.That (boundInstance.GetBindingTransaction(), Is.SameAs (bindingTransaction));

      var nonBindingTransaction = ClientTransaction.CreateRootTransaction ();
      var nonBoundInstance = nonBindingTransaction.Execute (() => Order.NewObject ());

      Assert.That (nonBoundInstance.HasBindingTransaction, Is.False);
    }

    [Test]
    public void Ctor_CreatesAndRegistersDataContainer ()
    {
      var instance = Order.NewObject ();

      var dataContainer = ClientTransactionMock.DataManager.DataContainerMap[instance.ID];
      Assert.That (dataContainer, Is.Not.Null);
      Assert.That (dataContainer.DomainObject, Is.SameAs (instance));
      Assert.That (dataContainer.ClientTransaction, Is.SameAs (ClientTransactionMock));
    }

    [Test]
    public void Ctor_EnlistsObjectInTransaction ()
    {
      var instance = Order.NewObject ();

      Assert.That (ClientTransactionMock.IsEnlisted (instance), Is.True);
    }

    [Test]
    public void HasBindingTransaction_BoundObject ()
    {
      using (ClientTransaction.CreateBindingTransaction ().EnterDiscardingScope ())
      {
        var obj = Order.NewObject ();
        Assert.That (obj.HasBindingTransaction, Is.True);
      }
    }

    [Test]
    public void HasBindingTransaction_UnboundObject ()
    {
      var obj = Order.NewObject ();
      Assert.That (obj.HasBindingTransaction, Is.False);
    }

    [Test]
    public void GetBindingTransaction_BoundObject ()
    {
      using (ClientTransaction.CreateBindingTransaction ().EnterDiscardingScope ())
      {
        var obj = Order.NewObject ();
        Assert.That (obj.GetBindingTransaction(), Is.SameAs (ClientTransaction.Current));
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This object has not been bound to a specific transaction, it "
        + "uses the current transaction when it is accessed. Use HasBindingTransaction to check whether an object has been bound or not.")]
    public void GetBindingTransaction_UnboundObject ()
    {
      var obj = Order.NewObject ();
      Dev.Null = obj.GetBindingTransaction();
    }

    [Test]
    public void DefaultTransactionContext_Current ()
    {
      var order = Order.NewObject ();
      Assert.That (order.DefaultTransactionContext.ClientTransaction, Is.SameAs (ClientTransaction.Current));
    }

    [Test]
    public void DefaultTransactionContext_Bound ()
    {
      Order order;
      var bindingTransaction = ClientTransaction.CreateBindingTransaction ();
      using (bindingTransaction.EnterNonDiscardingScope ())
      {
        order = Order.NewObject ();
      }
      Assert.That (order.DefaultTransactionContext.ClientTransaction, Is.SameAs (bindingTransaction));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "No ClientTransaction has been associated with the current thread or this object.")]
    public void DefaultTransactionContext_Null ()
    {
      var order = Order.NewObject ();
      using (ClientTransactionScope.EnterNullScope ())
      {
        Dev.Null = order.DefaultTransactionContext;
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = 
        @"Cannot delete DomainObject 'Order|.*|System\.Guid', because it belongs to a different ClientTransaction.",
        MatchType = MessageMatch.Regex)]
    public void Delete_ChecksTransaction ()
    {
      var order = DomainObjectMother.CreateObjectInOtherTransaction<Order> ();
      Assert.That (ClientTransaction.Current.IsEnlisted (order), Is.False);
      order.Delete ();
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = "Domain object '.*' cannot be used in the given transaction "
        + "as it was loaded or created in another transaction. Enter a scope for the transaction, or enlist the object in "
        + "the transaction. \\(If no transaction was explicitly given, ClientTransaction.Current was used.\\)", MatchType = MessageMatch.Regex)]
    public void PropertyAccess_ThrowsWhenNotEnlisted ()
    {
      Order order = Order.NewObject ();
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        Assert.That (ClientTransaction.Current.IsEnlisted (order), Is.False);
        Dev.Null = order.OrderNumber;
      }
    }

    [Test]
    public void OnLoaded_CanAccessPropertyValues ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();
      order.ProtectedLoaded += ((sender, e) => Assert.That (((Order) sender).OrderNumber, Is.EqualTo (1)));

      newTransaction.EnlistDomainObject (order);
      order.TransactionContext[newTransaction].EnsureDataAvailable ();
    }

    [Test]
    public void EnsureDataAvailable ()
    {
      var order = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);
      ClientTransactionMock.EnlistDomainObject (order);
      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[order.ID], Is.Null);
      
      order.EnsureDataAvailable ();

      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[order.ID], Is.Not.Null);
      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[order.ID].DomainObject, Is.SameAs (order));
    }
  }
}
