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
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.Utilities;
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
    public void Ctor_WithVirtualPropertyCall_Allowed ()
    {
      var orderItem = OrderItem.NewObject ("Test Toast");
      Assert.That (orderItem.Product, Is.EqualTo ("Test Toast"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The object cannot be initialized, it already has an ID.")]
    public void Initialize_ThrowsForNewObject ()
    {
      var orderItem = OrderItem.NewObject ("Test Toast");
      orderItem.Initialize (DomainObjectIDs.OrderItem1, null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The object cannot be initialized, it already has an ID.")]
    public void Initialize_ThrowsForLoadedObject ()
    {
      var orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      orderItem.Initialize (DomainObjectIDs.OrderItem1, null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The object cannot be initialized, it already has an ID.")]
    public void Initialize_ThrowsForDeserializedObject ()
    {
      var orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var deserializedOrderItem = Serializer.SerializeAndDeserialize (orderItem);
      deserializedOrderItem.Initialize (DomainObjectIDs.OrderItem1, null);
    }

    [Test]
    public void Initialize_WithUninitializedObject_SetsID ()
    {
      var type = InterceptedDomainObjectCreator.Instance.Factory.GetConcreteDomainObjectType (typeof (OrderItem));
      var orderItem = (OrderItem) FormatterServices.GetSafeUninitializedObject (type);
      orderItem.Initialize (DomainObjectIDs.OrderItem1, ClientTransaction.Current);

      Assert.That (orderItem.ID, Is.EqualTo (DomainObjectIDs.OrderItem1));
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
    public void FinishReferenceInitialization_IDAndBindingTransaction ()
    {
      var domainObject = Order.NewObject(); // indirect call of FinishReferenceInitialization
      Assert.That (domainObject.OnReferenceInitializedCalled, Is.True);

      Assert.That (domainObject.OnReferenceInitializedID, Is.EqualTo (domainObject.ID));
      Assert.That (domainObject.OnReferenceInitializedBindingTransaction, Is.Null);

      var bindingTransaction = ClientTransaction.CreateBindingTransaction ();
      var boundDomainObject = (Order) InterceptedDomainObjectCreator.Instance.CreateObjectReference (DomainObjectIDs.Order1, bindingTransaction);
      Assert.That (boundDomainObject.OnReferenceInitializedBindingTransaction, Is.SameAs (bindingTransaction));
    }

    [Test]
    [Ignore ("TODO 2256")]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "?")]
    public void FinishReferenceInitialization_CallsReferenceInitialized_PropertyAccessForbidden ()
    {
      EventHandler handler = (sender, args) => Dev.Null = ((Order) sender).OrderNumber;

      Order.StaticInitializationHandler += handler;
      try
      {
        Order.NewObject (); // indirect call of FinishReferenceInitialization
      }
      finally
      {
        Order.StaticInitializationHandler -= handler;
      }
    }

    [Test]
    public void FinishReferenceInitialization_InvokesMixinHook ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass (typeof (Order)).Clear ().AddMixins (typeof (HookedDomainObjectMixin)).EnterScope ())
      {
        var order = Order.NewObject(); // indirect call of FinishReferenceInitialization
        var mixinInstance = Mixin.Get<HookedDomainObjectMixin> (order);

        Assert.That (mixinInstance.OnDomainObjectReferenceInitializedCalled, Is.True);
      }
    }

    [Test]
    [Ignore ("TODO 2256")]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "?")]
    public void FinishReferenceInitialization_InvokesMixinHook_WhilePropertyAccessForbidden ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass (typeof (Order)).Clear ().AddMixins (typeof (HookedDomainObjectMixin)).EnterScope ())
      {
        var mixinInstance = new HookedDomainObjectMixin ();
        mixinInstance.InitializationHandler += (sender, args) => Dev.Null = ((Order) sender).OrderNumber;

        using (new MixedObjectInstantiationScope (mixinInstance))
        {
          Order.NewObject(); // indirect call of FinishReferenceInitialization
        }
      }
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

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "DomainObject.GetType should not be used.")]
    public void GetType_Throws ()
    {
      try
      {
        Order order = Order.NewObject ();
        typeof (DomainObject).GetMethod ("GetType", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Invoke (
            order, new object[0]);
      }
      catch (TargetInvocationException ex)
      {
        throw ex.InnerException;
      }
    }

    [Test]
    public void GetPublicDomainObjectType ()
    {
      Customer customer = Customer.NewObject ();
      Assert.That (customer.GetPublicDomainObjectType(), Is.SameAs (typeof (Customer)));
    }

    [Test]
    public new void ToString ()
    {
      Order order = Order.NewObject ();
      Assert.That (order.ToString (), Is.EqualTo (order.ID.ToString ()));
    }

    [Test]
    public void State ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      Assert.That (customer.State, Is.EqualTo (StateType.Unchanged));
      customer.Name = "New name";
      Assert.That (customer.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void MarkAsChanged ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));

      order.MarkAsChanged ();
      Assert.That (order.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Only existing DomainObjects can be marked as changed.")]
    public void MarkAsChangedThrowsWhenNew ()
    {
      Order order = Order.NewObject ();
      order.MarkAsChanged ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Only existing DomainObjects can be marked as changed.")]
    public void MarkAsChangedThrowsWhenDeleted ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.Delete ();
      order.MarkAsChanged ();
    }

    [Test]
    public void NewObject_CallsCtor ()
    {
      var order = Order.NewObject ();
      Assert.That (order.CtorCalled, Is.True);
    }

    [Test]
    public void NewObject_ProtectedConstructor ()
    {
      Dev.Null = Company.NewObject ();
    }

    [Test]
    public void NewObject_PublicConstructor ()
    {
      Dev.Null = Customer.NewObject ();
    }

    [Test]
    public void NewObject_SetsNeedsLoadModeDataContainerOnly_True ()
    {
      var order = Order.NewObject ();
      Assert.That (order.NeedsLoadModeDataContainerOnly, Is.True);
    }

    [Test]
    public void GetObject_SetsNeedsLoadModeDataContainerOnly_True_ ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      Assert.That (order.NeedsLoadModeDataContainerOnly, Is.True);
    }

    [Test]
    public void GetObject_WithTransaction ()
    {
      Order order;
      var clientTransactionMock = new ClientTransactionMock ();
      using (clientTransactionMock.EnterDiscardingScope ())
      {
        order = Order.GetObject (DomainObjectIDs.Order1);
      }
      Assert.That (clientTransactionMock.IsEnlisted (order), Is.True);
      Assert.That (ClientTransactionScope.CurrentTransaction.IsEnlisted (order), Is.False);
    }

    [Test]
    public void GetObject_Deleted_WithTransaction ()
    {
      Order order;
      var clientTransactionMock = new ClientTransactionMock ();
      using (clientTransactionMock.EnterDiscardingScope ())
      {
        order = Order.GetObject (DomainObjectIDs.Order1);

        order.Delete ();

        order = Order.GetObject (DomainObjectIDs.Order1, true);

        Assert.That (order.State, Is.EqualTo (StateType.Deleted));
      }
      Assert.That (clientTransactionMock.IsEnlisted (order), Is.True);
      Assert.That (ClientTransactionScope.CurrentTransaction.IsEnlisted (order), Is.False);
    }

    [Test]
    public void NeedsLoadModeDataContainerOnly_False_BeforeGetObject ()
    {
      var creator = DomainObjectIDs.Order1.ClassDefinition.GetDomainObjectCreator ();
      var order = (Order) creator.CreateObjectReference (DomainObjectIDs.Order1, ClientTransactionMock);
      Assert.That (order.NeedsLoadModeDataContainerOnly, Is.False);
    }

    [Test]
    public void NeedsLoadModeDataContainerOnly_True_AfterOnLoaded ()
    {
      var creator = DomainObjectIDs.Order1.ClassDefinition.GetDomainObjectCreator ();
      var order = (Order) creator.CreateObjectReference (DomainObjectIDs.Order1, ClientTransactionMock);
      Assert.That (order.NeedsLoadModeDataContainerOnly, Is.False);

      PrivateInvoke.InvokeNonPublicMethod (order, typeof (DomainObject), "OnLoaded");

      Assert.That (order.NeedsLoadModeDataContainerOnly, Is.True);
    }

    [Test]
    public void NeedsLoadModeDataContainerOnly_Serialization_True ()
    {
      var order = Order.NewObject ();
      Assert.That (order.NeedsLoadModeDataContainerOnly, Is.True);

      var deserializedOrder = Serializer.SerializeAndDeserialize (order);
      Assert.That (deserializedOrder.NeedsLoadModeDataContainerOnly, Is.True);
    }

    [Test]
    public void NeedsLoadModeDataContainerOnly_Serialization_False ()
    {
      var creator = DomainObjectIDs.Order1.ClassDefinition.GetDomainObjectCreator ();
      var order = (Order) creator.CreateObjectReference (DomainObjectIDs.Order1, ClientTransactionMock);

      Assert.That (order.NeedsLoadModeDataContainerOnly, Is.False);

      var deserializedOrder = Serializer.SerializeAndDeserialize (order);
      Assert.That (deserializedOrder.NeedsLoadModeDataContainerOnly, Is.False);
    }

    [Test]
    public void NeedsLoadModeDataContainerOnly_Serialization_ISerializable_True ()
    {
      var classWithAllDataTypes = ClassWithAllDataTypes.NewObject ();
      Assert.That (classWithAllDataTypes.NeedsLoadModeDataContainerOnly, Is.True);

      var deserializedClassWithAllDataTypes = Serializer.SerializeAndDeserialize (classWithAllDataTypes);
      Assert.That (deserializedClassWithAllDataTypes.NeedsLoadModeDataContainerOnly, Is.True);
    }

    [Test]
    public void NeedsLoadModeDataContainerOnly_Serialization_ISerializable_False ()
    {
      var creator = DomainObjectIDs.Order1.ClassDefinition.GetDomainObjectCreator ();
      var classWithAllDataTypes = (ClassWithAllDataTypes) creator.CreateObjectReference (DomainObjectIDs.ClassWithAllDataTypes1, ClientTransactionMock);

      Assert.That (classWithAllDataTypes.NeedsLoadModeDataContainerOnly, Is.False);

      var deserializedClassWithAllDataTypes = Serializer.SerializeAndDeserialize (classWithAllDataTypes);
      Assert.That (deserializedClassWithAllDataTypes.NeedsLoadModeDataContainerOnly, Is.False);
    }

    [Test]
    public void Properties ()
    {
      var order = Order.NewObject ();
      var propertyIndexer = order.Properties;
      Assert.That (propertyIndexer, Is.Not.Null);
      Assert.That (propertyIndexer.DomainObject, Is.SameAs (order));

      var propertyIndexer2 = order.Properties;
      Assert.That (propertyIndexer, Is.SameAs (propertyIndexer2));
    }

    [Test]
    public void Properties_Serialization ()
    {
      var order = Order.NewObject ();
      var propertyIndexer = order.Properties;
      Assert.That (propertyIndexer, Is.Not.Null);
      Assert.That (propertyIndexer.DomainObject, Is.SameAs (order));

      var deserializedOrder = Serializer.SerializeAndDeserialize (order);
      var newPropertyIndexer = deserializedOrder.Properties;
      Assert.That (newPropertyIndexer, Is.Not.Null);
      Assert.That (newPropertyIndexer.DomainObject, Is.SameAs (deserializedOrder));
    }

    [Test]
    public void TransactionContext ()
    {
      var order = Order.NewObject ();
      var transactionContextIndexer = order.TransactionContext;

      Assert.That (transactionContextIndexer, Is.InstanceOfType (typeof (DomainObjectTransactionContextIndexer)));
      Assert.That (((DomainObjectTransactionContext) transactionContextIndexer[ClientTransaction.Current]).DomainObject, Is.SameAs (order));
    }
  }
}
