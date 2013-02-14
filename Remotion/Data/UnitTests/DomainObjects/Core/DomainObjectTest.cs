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
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  [TestFixture]
  public class DomainObjectTest : StandardMappingTest
  {
    private TestableClientTransaction _transaction;

    public override void SetUp ()
    {
      base.SetUp ();

      _transaction = new TestableClientTransaction ();
    }

    [Test]
    public void Ctor_SetsObjectID ()
    {
      var instance = _transaction.Execute (() => Order.NewObject ());

      Assert.That (instance.ID, Is.Not.Null);
      Assert.That (instance.ID.ClassDefinition, Is.SameAs (MappingConfiguration.Current.GetTypeDefinition (typeof (Order))));
    }

    [Test]
    public void Ctor_SetsBindingTransaction ()
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
    public void Ctor_RegistersObject ()
    {
      TestDomainBase.StaticCtorHandler +=
          (sender, args) =>
          Assert.That (ObjectInititalizationContextScope.CurrentObjectInitializationContext.RegisteredObject, Is.SameAs (sender));

      Order instance;
      try
      {
        instance = _transaction.Execute (() => Order.NewObject ());
      }
      finally
      {
        TestDomainBase.ClearStaticCtorHandlers();
      }

      Assert.That (_transaction.IsEnlisted (instance), Is.True);
      var dataContainer = _transaction.DataManager.DataContainers[instance.ID];
      Assert.That (dataContainer, Is.Not.Null);
      Assert.That (dataContainer.DomainObject, Is.SameAs (instance));
      Assert.That (dataContainer.ClientTransaction, Is.SameAs (_transaction));
    }

    [Test]
    public void Ctor_RaisesReferenceInitializing ()
    {
      var domainObject = _transaction.Execute (() => Order.NewObject ());
      Assert.That (domainObject.OnReferenceInitializingCalled, Is.True);
    }

    [Test]
    public void Ctor_RaisesReferenceInitializing_InRightTransaction ()
    {
      var domainObject = _transaction.Execute (() => Order.NewObject ());
      Assert.That (domainObject.OnReferenceInitializingTx, Is.SameAs (_transaction));
    }

    [Test]
    public void Ctor_RaisesReferenceInitializing_CalledBeforeCtor ()
    {
      var domainObject = _transaction.Execute (() => Order.NewObject ());
      Assert.That (domainObject.OnReferenceInitializingCalledBeforeCtor, Is.True);
    }

    [Test]
    public void Ctor_WithVirtualPropertyCall_Allowed ()
    {
      var orderItem = _transaction.Execute (() => OrderItem.NewObject ("Test Toast"));
      Assert.That (_transaction.Execute (() => orderItem.Product), Is.EqualTo ("Test Toast"));
    }

    [Test]
    public void Ctor_DirectCall ()
    {
      Assert.That (
          () => new DomainObjectWithPublicCtor(),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "DomainObject constructors must not be called directly. Use DomainObject.NewObject to create DomainObject instances."));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The object cannot be initialized, it already has an ID.")]
    public void Initialize_ThrowsForNewObject ()
    {
      var orderItem = _transaction.Execute (() => OrderItem.NewObject ("Test Toast"));
      orderItem.Initialize (DomainObjectIDs.OrderItem1, null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The object cannot be initialized, it already has an ID.")]
    public void Initialize_ThrowsForLoadedObject ()
    {
      var orderItem = _transaction.Execute (() => DomainObjectIDs.OrderItem1.GetObject<OrderItem>());
      orderItem.Initialize (DomainObjectIDs.OrderItem1, null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The object cannot be initialized, it already has an ID.")]
    [UseLegacyCodeGeneration]
    public void Initialize_ThrowsForDeserializedObject ()
    {
      //TODO 5370: Remove
      SetUp();

      var orderItem = _transaction.Execute (() => DomainObjectIDs.OrderItem1.GetObject<OrderItem>());


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
      var bindingTransaction = ClientTransaction.CreateBindingTransaction ();
      
      var obj = bindingTransaction.Execute (() => Order.NewObject ());
      Assert.That (obj.HasBindingTransaction, Is.True);
    }

    [Test]
    public void HasBindingTransaction_UnboundObject ()
    {
      var obj = _transaction.Execute (() => Order.NewObject ());
      Assert.That (obj.HasBindingTransaction, Is.False);
    }

    [Test]
    public void GetBindingTransaction_BoundObject ()
    {
      var bindingTransaction = ClientTransaction.CreateBindingTransaction ();
      var obj = bindingTransaction.Execute (() => Order.NewObject ());
      Assert.That (obj.GetBindingTransaction(), Is.SameAs (bindingTransaction));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This object has not been bound to a specific transaction, it "
        + "uses the current transaction when it is accessed. Use HasBindingTransaction to check whether an object has been bound or not.")]
    public void GetBindingTransaction_UnboundObject ()
    {
      var obj = _transaction.Execute (() => Order.NewObject ());
      Dev.Null = obj.GetBindingTransaction();
    }

    [Test]
    public void RaiseReferenceInitializatingEvent_IDAndBindingTransaction ()
    {
      var domainObject = _transaction.Execute (() => Order.NewObject()); // indirect call of RaiseReferenceInitializatingEvent
      Assert.That (domainObject.OnReferenceInitializingCalled, Is.True);

      Assert.That (domainObject.OnReferenceInitializingID, Is.EqualTo (domainObject.ID));
      Assert.That (domainObject.OnReferenceInitializingBindingTransaction, Is.Null);

      var bindingTransaction = ClientTransaction.CreateBindingTransaction ();
      var boundDomainObject = (Order) LifetimeService.GetObjectReference (bindingTransaction, DomainObjectIDs.Order1);
      Assert.That (boundDomainObject.OnReferenceInitializingBindingTransaction, Is.SameAs (bindingTransaction));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "While the OnReferenceInitializing event is executing, this member cannot be used.")]
    public void RaiseReferenceInitializatingEvent_CallsReferenceInitializing_PropertyAccessForbidden ()
    {
      _transaction.Execute (() => DomainObjectTestHelper.ExecuteInReferenceInitializing_NewObject (o => o.OrderNumber));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "While the OnReferenceInitializing event is executing, this member cannot be used.")]
    public void RaiseReferenceInitializatingEvent_CallsReferenceInitializing_PropertiesForbidden ()
    {
      _transaction.Execute (() => DomainObjectTestHelper.ExecuteInReferenceInitializing_NewObject (o => o.Properties));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "While the OnReferenceInitializing event is executing, this member cannot be used.")]
    public void RaiseReferenceInitializatingEvent_CallsReferenceInitializing_CurrentPropertyForbidden ()
    {
      _transaction.Execute (() => DomainObjectTestHelper.ExecuteInReferenceInitializing_NewObject (o => o.CurrentProperty));
    }

    [Test]
    public void RaiseReferenceInitializatingEvent_CallsReferenceInitializing_TransactionContextIsRestricted ()
    {
      var result = _transaction.Execute (() => DomainObjectTestHelper.ExecuteInReferenceInitializing_NewObject (o => o.DefaultTransactionContext));
      Assert.That (result, Is.TypeOf (typeof (InitializedEventDomainObjectTransactionContextDecorator)));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "While the OnReferenceInitializing event is executing, this member cannot be used.")]
    public void RaiseReferenceInitializatingEvent_CallsReferenceInitializing_DeleteForbidden ()
    {
      _transaction.Execute (() => DomainObjectTestHelper.ExecuteInReferenceInitializing_NewObject (o => { o.Delete (); return o; }));
    }

    [Test]
    public void RaiseReferenceInitializatingEvent_InvokesMixinHook ()
    {
      var domainObject = _transaction.Execute (() => HookedTargetClass.NewObject()); // indirect call of RaiseReferenceInitializatingEvent
      var mixinInstance = Mixin.Get<HookedDomainObjectMixin> (domainObject);

      Assert.That (mixinInstance.OnDomainObjectReferenceInitializingCalled, Is.True);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "While the OnReferenceInitializing event is executing, this member cannot be used.")]
    public void RaiseReferenceInitializatingEvent_InvokesMixinHook_WhilePropertyAccessForbidden ()
    {
      var mixinInstance = new HookedDomainObjectMixin ();
      mixinInstance.InitializationHandler += (sender, args) => Dev.Null = ((HookedDomainObjectMixin) sender).Target.Property;

      using (new MixedObjectInstantiationScope (mixinInstance))
      {
        _transaction.Execute (() => HookedTargetClass.NewObject ()); // indirect call of RaiseReferenceInitializatingEvent
      }
    }

    [Test]
    public void RaiseReferenceInitializatingEvent_ResetsFlagAfterNotification ()
    {
      var order = _transaction.Execute (() => DomainObjectTestHelper.ExecuteInReferenceInitializing_NewObject (o => o));
      Dev.Null = _transaction.Execute (() => order.OrderNumber); // succeeds
    }

    [Test]
    public void DefaultTransactionContext_Current ()
    {
      var order = _transaction.Execute (() => Order.NewObject ());
      Assert.That (_transaction.Execute (() => order.DefaultTransactionContext.ClientTransaction), Is.SameAs (_transaction));
    }

    [Test]
    public void DefaultTransactionContext_Bound ()
    {
      var bindingTransaction = ClientTransaction.CreateBindingTransaction ();
      var order = bindingTransaction.Execute (() => Order.NewObject ());
      Assert.That (order.DefaultTransactionContext.ClientTransaction, Is.SameAs (bindingTransaction));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "No ClientTransaction has been associated with the current thread or this object.")]
    public void DefaultTransactionContext_Null ()
    {
      var order = _transaction.Execute (() => Order.NewObject ());
      Dev.Null = order.DefaultTransactionContext;
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = 
        @"Cannot delete DomainObject 'Order|.*|System\.Guid', because it belongs to a different ClientTransaction.",
        MatchType = MessageMatch.Regex)]
    public void Delete_ChecksTransaction ()
    {
      var order = DomainObjectMother.CreateObjectInOtherTransaction<Order> ();
      Assert.That (_transaction.IsEnlisted (order), Is.False);
      _transaction.Execute (order.Delete);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = "Domain object '.*' cannot be used in the given transaction "
        + "as it was loaded or created in another transaction. Enter a scope for the transaction, or enlist the object in "
        + "the transaction. \\(If no transaction was explicitly given, ClientTransaction.Current was used.\\)", MatchType = MessageMatch.Regex)]
    public void PropertyAccess_ThrowsWhenNotEnlisted ()
    {
      Order order = _transaction.Execute (() => Order.NewObject ());
      var otherTransaction = ClientTransaction.CreateRootTransaction ();
      Assert.That (otherTransaction.IsEnlisted (order), Is.False);
      Dev.Null = otherTransaction.Execute (() => order.OrderNumber);
    }

    [Test]
    public void OnLoaded_CanAccessPropertyValues ()
    {
      Order order = _transaction.Execute (() => DomainObjectIDs.Order1.GetObjectReference<Order> ());
      order.ProtectedLoaded += ((sender, e) => Assert.That (((Order) sender).OrderNumber, Is.EqualTo (1)));

      Assert.That (order.OnLoadedCalled, Is.False);

      _transaction.Execute (order.EnsureDataAvailable);

      Assert.That (order.OnLoadedCalled, Is.True);
    }

    [Test]
    public void EnsureDataAvailable ()
    {
      var order = DomainObjectMother.GetNotLoadedObject (_transaction, DomainObjectIDs.Order1);
      Assert.That (_transaction.DataManager.DataContainers[order.ID], Is.Null);
      
      _transaction.Execute (order.EnsureDataAvailable);

      Assert.That (_transaction.DataManager.DataContainers[order.ID], Is.Not.Null);
      Assert.That (_transaction.DataManager.DataContainers[order.ID].DomainObject, Is.SameAs (order));
    }

    [Test]
    public void TryEnsureDataAvailable ()
    {
      var order = DomainObjectMother.GetNotLoadedObject (_transaction, DomainObjectIDs.Order1);
      Assert.That (_transaction.DataManager.DataContainers[order.ID], Is.Null);

      _transaction.Execute (() => Assert.That (() => order.TryEnsureDataAvailable(), Is.True));

      Assert.That (_transaction.DataManager.DataContainers[order.ID], Is.Not.Null);
      Assert.That (_transaction.DataManager.DataContainers[order.ID].DomainObject, Is.SameAs (order));

      var nonExistingOrder = DomainObjectMother.GetNotLoadedNonExistingObject (_transaction);
      Assert.That (_transaction.DataManager.DataContainers[nonExistingOrder.ID], Is.Null);

      _transaction.Execute (() => Assert.That (() => nonExistingOrder.TryEnsureDataAvailable (), Is.False));

      Assert.That (_transaction.DataManager.DataContainers[nonExistingOrder.ID], Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "DomainObject.GetType should not be used.")]
    public void GetType_Throws ()
    {
      try
      {
        Order order = _transaction.Execute (() => Order.NewObject ());
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
      Customer customer = _transaction.Execute (() => Customer.NewObject ());
      Assert.That (customer.GetPublicDomainObjectType(), Is.SameAs (typeof (Customer)));
    }

    [Test]
    public new void ToString ()
    {
      Order order = _transaction.Execute (() => Order.NewObject ());
      Assert.That (order.ToString (), Is.EqualTo (order.ID.ToString ()));
    }

    [Test]
    public void State ()
    {
      Customer customer = _transaction.Execute (() => DomainObjectIDs.Customer1.GetObject<Customer> ());

      _transaction.Execute (() => Assert.That (customer.State, Is.EqualTo (StateType.Unchanged)));
      _transaction.Execute (() => customer.Name = "New name");
      _transaction.Execute (() => Assert.That (customer.State, Is.EqualTo (StateType.Changed)));
    }

    [Test]
    public void RegisterForCommit ()
    {
      Order order = _transaction.Execute (() => DomainObjectIDs.Order1.GetObject<Order> ());
      _transaction.Execute (() => Assert.That (order.State, Is.EqualTo (StateType.Unchanged)));

      _transaction.Execute (order.RegisterForCommit);

      _transaction.Execute (() => Assert.That (order.State, Is.EqualTo (StateType.Changed)));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void NewObject_WithoutTransaction ()
    {
      Order.NewObject ();
    }

    [Test]
    public void NewObject_CallsCtor ()
    {
      var order = _transaction.Execute (() => Order.NewObject ());
      Assert.That (order.CtorCalled, Is.True);
    }

    [Test]
    public void NewObject_ProtectedConstructor ()
    {
      Dev.Null = _transaction.Execute (() => Company.NewObject ());
    }

    [Test]
    public void NewObject_PublicConstructor ()
    {
      Dev.Null = _transaction.Execute (() => Customer.NewObject ());
    }

    [Test]
    public void NewObject_SetsNeedsLoadModeDataContainerOnly_True ()
    {
      var order = _transaction.Execute (() => Order.NewObject ());
      Assert.That (order.NeedsLoadModeDataContainerOnly, Is.True);
    }

    [Test]
    public void GetObject_SetsNeedsLoadModeDataContainerOnly_ToTrue ()
    {
      var order = _transaction.Execute (() => DomainObjectIDs.Order1.GetObject<Order> ());
      Assert.That (order.NeedsLoadModeDataContainerOnly, Is.True);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void GetObject_WithoutTransaction ()
    {
      DomainObjectIDs.Order1.GetObject<Order> ();
    }

    [Test]
    public void GetObject_Deleted ()
    {
      var order = _transaction.Execute (() => DomainObjectIDs.Order1.GetObject<Order> ());

      _transaction.Execute (order.Delete);

      _transaction.Execute (() => Assert.That (DomainObjectIDs.Order1.GetObject<Order> (includeDeleted: true), Is.SameAs (order)));
      _transaction.Execute (() => Assert.That (order.State, Is.EqualTo (StateType.Deleted)));
    }

    [Test]
    public void TryGetObject ()
    {
      Assert.That (_transaction.DataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);

      var order = _transaction.Execute (() => DomainObjectIDs.Order1.TryGetObject<TestDomainBase> ());

      Assert.That (order.ID, Is.EqualTo (DomainObjectIDs.Order1));
      Assert.That (_transaction.DataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);
      Assert.That (order.TransactionContext[_transaction].State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void TryGetObject_NotFound ()
    {
      var objectID = new ObjectID(typeof (Order), Guid.NewGuid());
      Assert.That (_transaction.IsInvalid (objectID), Is.False);

      var order = _transaction.Execute (() => objectID.TryGetObject<TestDomainBase> ());

      Assert.That (order, Is.Null);
      Assert.That (_transaction.IsInvalid (objectID), Is.True);
    }

    [Test]
    public void NeedsLoadModeDataContainerOnly_False_BeforeGetObject ()
    {
      var order = (Order) LifetimeService.GetObjectReference (_transaction, DomainObjectIDs.Order1);
      Assert.That (order.NeedsLoadModeDataContainerOnly, Is.False);
    }

    [Test]
    public void NeedsLoadModeDataContainerOnly_True_AfterOnLoaded ()
    {
      var order = (Order) LifetimeService.GetObjectReference (_transaction, DomainObjectIDs.Order1);
      Assert.That (order.NeedsLoadModeDataContainerOnly, Is.False);

      PrivateInvoke.InvokeNonPublicMethod (order, typeof (DomainObject), "OnLoaded");

      Assert.That (order.NeedsLoadModeDataContainerOnly, Is.True);
    }

    [Test]
    [UseLegacyCodeGeneration]
    public void NeedsLoadModeDataContainerOnly_Serialization_True ()
    {
      // TODO 5370: Remove.
      SetUp ();

      var order = _transaction.Execute (() => Order.NewObject ());
      Assert.That (order.NeedsLoadModeDataContainerOnly, Is.True);

      var deserializedOrder = Serializer.SerializeAndDeserialize (order);
      Assert.That (deserializedOrder.NeedsLoadModeDataContainerOnly, Is.True);
    }

    [Test]
    [UseLegacyCodeGeneration]
    public void NeedsLoadModeDataContainerOnly_Serialization_False ()
    {
      // TODO 5370: Remove.
      SetUp();

      var order = (Order) LifetimeService.GetObjectReference (_transaction, DomainObjectIDs.Order1);

      Assert.That (order.NeedsLoadModeDataContainerOnly, Is.False);

      var deserializedOrder = Serializer.SerializeAndDeserialize (order);
      Assert.That (deserializedOrder.NeedsLoadModeDataContainerOnly, Is.False);
    }

    [Test]
    [UseLegacyCodeGeneration]
    public void NeedsLoadModeDataContainerOnly_Serialization_ISerializable_True ()
    {
      // TODO 5370: Remove.
      SetUp ();

      var classWithAllDataTypes = _transaction.Execute (() => ClassWithAllDataTypes.NewObject ());
      Assert.That (classWithAllDataTypes.NeedsLoadModeDataContainerOnly, Is.True);

      var deserializedClassWithAllDataTypes = Serializer.SerializeAndDeserialize (classWithAllDataTypes);
      Assert.That (deserializedClassWithAllDataTypes.NeedsLoadModeDataContainerOnly, Is.True);
    }

    [Test]
    [UseLegacyCodeGeneration]
    public void NeedsLoadModeDataContainerOnly_Serialization_ISerializable_False ()
    {
      // TODO 5370: Remove.
      SetUp ();

      var classWithAllDataTypes = (ClassWithAllDataTypes) LifetimeService.GetObjectReference (_transaction, DomainObjectIDs.ClassWithAllDataTypes1);

      Assert.That (classWithAllDataTypes.NeedsLoadModeDataContainerOnly, Is.False);

      var deserializedClassWithAllDataTypes = Serializer.SerializeAndDeserialize (classWithAllDataTypes);
      Assert.That (deserializedClassWithAllDataTypes.NeedsLoadModeDataContainerOnly, Is.False);
    }

    [Test]
    public void Properties ()
    {
      var order = _transaction.Execute (() => Order.NewObject ());
      var propertyIndexer = _transaction.Execute (() => order.Properties);
      Assert.That (propertyIndexer, Is.Not.Null);
      Assert.That (propertyIndexer.DomainObject, Is.SameAs (order));

      var propertyIndexer2 = _transaction.Execute (() => order.Properties);
      Assert.That (propertyIndexer, Is.SameAs (propertyIndexer2));
    }

    [Test]
    [UseLegacyCodeGeneration]
    public void Properties_Serialization ()
    {
      // TODO 5370: Remove.
      SetUp ();

      var order = _transaction.Execute (() => Order.NewObject ());
      var propertyIndexer = _transaction.Execute (() => order.Properties);
      Assert.That (propertyIndexer, Is.Not.Null);
      Assert.That (propertyIndexer.DomainObject, Is.SameAs (order));

      var deserializedOrder = Serializer.SerializeAndDeserialize (order);
      var newPropertyIndexer = _transaction.Execute (() => deserializedOrder.Properties);
      Assert.That (newPropertyIndexer, Is.Not.Null);
      Assert.That (newPropertyIndexer.DomainObject, Is.SameAs (deserializedOrder));
    }

    [Test]
    public void TransactionContext ()
    {
      var order = _transaction.Execute (() => Order.NewObject ());
      var transactionContextIndexer = order.TransactionContext;

      Assert.That (transactionContextIndexer, Is.InstanceOf (typeof (DomainObjectTransactionContextIndexer)));
      Assert.That (((DomainObjectTransactionContext) transactionContextIndexer[_transaction]).DomainObject, Is.SameAs (order));
    }

    [DBTable]
    [ClassID ("DomainObjectTest_DomainObjectWithPublicCtor")]
    public class DomainObjectWithPublicCtor : DomainObject
    {
      public DomainObjectWithPublicCtor ()
      {
      }
    }
  }
}
