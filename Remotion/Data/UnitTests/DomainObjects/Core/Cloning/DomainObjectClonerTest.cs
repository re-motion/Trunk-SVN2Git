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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Cloning;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using Mocks_Is = Rhino.Mocks.Constraints.Is;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Cloning
{
  [TestFixture]
  public class DomainObjectClonerTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private DomainObjectCloner _cloner;
    private ClassWithAllDataTypes _classWithAllDataTypes;
    private Order _order1;
    private Computer _computer1;
    private ClassWithAllDataTypes _boundSource;

    public override void SetUp ()
    {
      base.SetUp();
      _mockRepository = new MockRepository ();
      _cloner = new DomainObjectCloner ();
      _classWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _computer1 = Computer.GetObject (DomainObjectIDs.Computer1);

      using (ClientTransaction.CreateBindingTransaction ().EnterNonDiscardingScope ())
      {
        _boundSource = ClassWithAllDataTypes.NewObject ();
        _boundSource.Int32Property = 123;
      }
    }

    [Test]
    public void CloneTransaction_CurrentByDefault ()
    {
      Assert.That (_cloner.CloneTransaction, Is.SameAs (ClientTransactionMock));
    }

    [Test]
    public void CloneTransaction_ManualSet ()
    {
      ClientTransaction cloneTransaction = ClientTransaction.CreateBindingTransaction ();
      _cloner.CloneTransaction = cloneTransaction;
      Assert.That (_cloner.CloneTransaction, Is.SameAs (cloneTransaction));
    }

    [Test]
    public void CloneTransaction_Reset ()
    {
      ClientTransaction cloneTransaction = ClientTransaction.CreateBindingTransaction ();
      _cloner.CloneTransaction = cloneTransaction;
      _cloner.CloneTransaction = null;
      Assert.That (_cloner.CloneTransaction, Is.SameAs (ClientTransactionMock));
    }

    [Test]
    public void CreateCloneHull_CreatesNewObject ()
    {
      DomainObject clone = _cloner.CreateCloneHull (_classWithAllDataTypes);
      Assert.That (clone, Is.Not.SameAs (_classWithAllDataTypes));
      Assert.That (clone.ID, Is.Not.EqualTo (_classWithAllDataTypes));
      Assert.That (clone.State, Is.EqualTo (StateType.New));
    }

    [Test]
    public void CreateCloneHull_CreatesObjectOfSameType ()
    {
      var clone = _cloner.CreateCloneHull<DomainObject> (_classWithAllDataTypes);
      Assert.That (clone.GetPublicDomainObjectType (), Is.SameAs (typeof (ClassWithAllDataTypes)));
    }

    [Test]
    public void CreateCloneHull_CallsNoCtor_UsesFactory ()
    {
      Order clone = _cloner.CreateCloneHull (_order1);
      Assert.That (clone.CtorCalled, Is.False);
      Assert.That (InterceptedDomainObjectCreator.Instance.Factory.WasCreatedByFactory (((object) clone).GetType()));
    }

    [Test]
    public void CreateCloneHull_RegistersDataContainer ()
    {
      var transaction = ClientTransaction.CreateRootTransaction ();
      _cloner.CloneTransaction = transaction;
      var clone = _cloner.CreateCloneHull (_classWithAllDataTypes);
      Assert.That (clone.GetInternalDataContainerForTransaction (transaction).ClientTransaction, Is.SameAs (transaction));
    }

    [Test]
    public void CreateCloneHull_SetsDomainObjectOfDataContainer ()
    {
      ClassWithAllDataTypes clone = _cloner.CreateCloneHull (_classWithAllDataTypes);
      Assert.That (clone.InternalDataContainer.DomainObject, Is.SameAs (clone));
    }

    [Test]
    public void CreateCloneHull_BindsObjectToBindingClientTransaction ()
    {
      var cloneTransaction = ClientTransaction.CreateBindingTransaction ();
      _cloner.CloneTransaction = cloneTransaction;
      DomainObject clone = _cloner.CreateCloneHull (_classWithAllDataTypes);
      Assert.That (clone.HasBindingTransaction, Is.True);
      Assert.That (clone.GetBindingTransaction (), Is.SameAs (cloneTransaction));
    }

    [Test]
    public void CreateCloneHull_DoesntBindObjectToOtherClientTransaction ()
    {
      var cloneTransaction = ClientTransaction.CreateRootTransaction ();
      _cloner.CloneTransaction = cloneTransaction;
      DomainObject clone = _cloner.CreateCloneHull (_classWithAllDataTypes);
      Assert.That (clone.HasBindingTransaction, Is.False);
    }

    [Test]
    public void CreateCloneHull_EnlistsObjectInCorrectTransaction ()
    {
      var cloneTransaction = ClientTransaction.CreateRootTransaction ();
      _cloner.CloneTransaction = cloneTransaction;
      DomainObject clone = _cloner.CreateCloneHull (_classWithAllDataTypes);
      Assert.That (cloneTransaction.IsEnlisted (clone), Is.True);
    }
    
    [Test]
    public void CreateCloneHull_TouchesNoProperties ()
    {
      ClassWithAllDataTypes clone = _cloner.CreateCloneHull (_classWithAllDataTypes);
      Assert.That (clone.Int32Property, Is.Not.EqualTo (_classWithAllDataTypes.Int32Property));
      Assert.That (clone.Properties[typeof (ClassWithAllDataTypes), "Int32Property"].HasBeenTouched, Is.False);
    }

    [Test]
    public void CreateValueClone_SimpleProperties ()
    {
      ClassWithAllDataTypes clone = _cloner.CreateValueClone (_classWithAllDataTypes);
      Assert.That (clone.Int32Property, Is.EqualTo (_classWithAllDataTypes.Int32Property));
    }

    [Test]
    public void CreateValueClone_OriginalValueNotCloned ()
    {
      _classWithAllDataTypes.Int32Property = 2 * _classWithAllDataTypes.Int32Property;
      ClassWithAllDataTypes clone = _cloner.CreateValueClone (_classWithAllDataTypes);

      Assert.That (clone.Int32Property, Is.EqualTo (_classWithAllDataTypes.Int32Property));
      Assert.That (clone.Properties[typeof (ClassWithAllDataTypes), "Int32Property"].GetOriginalValue<int> (),
          Is.Not.EqualTo (_classWithAllDataTypes.Properties[typeof (ClassWithAllDataTypes), "Int32Property"].GetOriginalValue<int> ()));
      Assert.That (clone.Properties[typeof (ClassWithAllDataTypes), "Int32Property"].GetOriginalValue<int> (), Is.EqualTo (0));
    }

    [Test]
    public void CreateValueClone_RelationProperties_NonForeignKey ()
    {
      Order clone = _cloner.CreateValueClone (_order1);
      Assert.That (clone.OrderItems, Is.Empty);
      Assert.That (clone.OrderTicket, Is.Null);
    }

    [Test]
    public void CreateValueClone_RelationProperties_ForeignKey ()
    {
      Computer clone = _cloner.CreateValueClone (_computer1);
      Assert.That (_computer1.Employee, Is.Not.Null);
      Assert.That (clone.Employee, Is.Null);
      Assert.That (clone.InternalDataContainer.PropertyValues[ReflectionMappingHelper.GetPropertyName (typeof (Computer), "Employee")].Value, Is.Null);
    }

    [Test]
    public void CreateValueClone_RelationProperties_ForeignKey_OriginalValue ()
    {
      Computer clone = _cloner.CreateValueClone (_computer1);
      Assert.That (_computer1.Employee, Is.Not.Null);
      Assert.That (clone.Properties[typeof (Computer), "Employee"].GetOriginalValue<Employee> (), Is.Null);
      Assert.That (clone.InternalDataContainer.PropertyValues[ReflectionMappingHelper.GetPropertyName (typeof (Computer), "Employee")].OriginalValue, Is.Null);
    }

    [Test]
    public void SourceTransaction_IsRespected ()
    {
      ClassWithAllDataTypes unboundClone = _cloner.CreateValueClone (_boundSource);
      Assert.That (unboundClone.Int32Property, Is.EqualTo (123));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void NullTransaction_ForCloneTransaction ()
    {
      using (ClientTransactionScope.EnterNullScope ())
      {
        _cloner.CreateValueClone (_boundSource);
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread or this object.")]
    public void NullTransaction_ForSourceTransaction ()
    {
      _cloner.CloneTransaction = ClientTransactionMock;
      using (ClientTransactionScope.EnterNullScope ())
      {
        _cloner.CreateValueClone (_computer1);
      }
    }

    [Test]
    public void CreateClone_CreatesValueClone ()
    {
      var strategyMock = _mockRepository.Stub<ICloneStrategy> ();
      _mockRepository.ReplayAll ();
      
      Order clone = _cloner.CreateClone (_order1, strategyMock);
      Assert.That (clone.OrderNumber, Is.EqualTo (_order1.OrderNumber));
      Assert.That (clone.DeliveryDate, Is.EqualTo (_order1.DeliveryDate));
    }

    [Test]
    public void CreateClone_CallsStrategyForReferences ()
    {
      var strategyMock = _mockRepository.StrictMock<ICloneStrategy>();
      var contextMock = _mockRepository.Stub<CloneContext>(_cloner);
      Order clone = Order.NewObject();
      var shallowClonesFake = new Queue<Tuple<DomainObject, DomainObject>> ();

      SetupResult.For (contextMock.GetCloneFor (_order1)).Return (clone);
      shallowClonesFake.Enqueue (new Tuple<DomainObject, DomainObject> (_order1, clone));
      SetupResult.For (contextMock.CloneHulls).Return (shallowClonesFake);

      using (_mockRepository.Unordered ())
      {
        ExpectHandleReference (strategyMock, _order1, clone, "OrderItems", ClientTransaction.Current, ClientTransaction.Current);
        ExpectHandleReference (strategyMock, _order1, clone, "OrderTicket", ClientTransaction.Current, ClientTransaction.Current);
        ExpectHandleReference (strategyMock, _order1, clone, "Official", ClientTransaction.Current, ClientTransaction.Current);
        ExpectHandleReference (strategyMock, _order1, clone, "Customer", ClientTransaction.Current, ClientTransaction.Current);
      }
      _mockRepository.ReplayAll ();

      _cloner.CreateClone (_order1, strategyMock, contextMock);
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void CreateClone_CallsStrategyForReferences_OnReferencedObjectsToo ()
    {
      var strategyMock = _mockRepository.StrictMock<ICloneStrategy> ();
      var contextMock = _mockRepository.Stub<CloneContext> (_cloner);
      Order clone = Order.NewObject ();
      OrderItem clone2 = OrderItem.NewObject ();
      var shallowClonesFake = new Queue<Tuple<DomainObject, DomainObject>> ();

      SetupResult.For (contextMock.GetCloneFor (_order1)).Return (clone);
      shallowClonesFake.Enqueue (new Tuple<DomainObject, DomainObject> (_order1, clone));
      shallowClonesFake.Enqueue (new Tuple<DomainObject, DomainObject> (_order1.OrderItems[0], clone2));
      SetupResult.For (contextMock.CloneHulls).Return (shallowClonesFake);

      using (_mockRepository.Unordered ())
      {
        ExpectHandleReference (strategyMock, _order1, clone, "OrderItems", ClientTransaction.Current, ClientTransaction.Current);
        ExpectHandleReference (strategyMock, _order1, clone, "OrderTicket", ClientTransaction.Current, ClientTransaction.Current);
        ExpectHandleReference (strategyMock, _order1, clone, "Official", ClientTransaction.Current, ClientTransaction.Current);
        ExpectHandleReference (strategyMock, _order1, clone, "Customer", ClientTransaction.Current, ClientTransaction.Current);

        ExpectHandleReference (strategyMock, _order1.OrderItems[0], clone2, "Order", ClientTransaction.Current, ClientTransaction.Current);
      }
      _mockRepository.ReplayAll ();

      _cloner.CreateClone (_order1, strategyMock, contextMock);
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void CreateClone_CallsStrategyForReferences_OnlyWhenNotTouched ()
    {
      var strategyMock = _mockRepository.StrictMock<ICloneStrategy> ();
      var contextMock = _mockRepository.Stub<CloneContext> (_cloner);
      Order clone = Order.NewObject ();
      OrderItem clone2 = OrderItem.NewObject ();
      var shallowClonesFake = new Queue<Tuple<DomainObject, DomainObject>> ();

      clone.OrderTicket = clone.OrderTicket;
      clone.Official = clone.Official;
      clone.Customer = clone.Customer;
      clone.OrderItems.Add (clone2);

      SetupResult.For (contextMock.GetCloneFor (_order1)).Return (clone);
      shallowClonesFake.Enqueue (new Tuple<DomainObject, DomainObject> (_order1, clone));
      shallowClonesFake.Enqueue (new Tuple<DomainObject, DomainObject> (_order1.OrderItems[0], clone2));
      SetupResult.For (contextMock.CloneHulls).Return (shallowClonesFake);

      using (_mockRepository.Unordered ())
      {
        // not called: ExpectHandleReference (strategyMock, _order1, clone, "OrderItems", ClientTransaction.Current, ClientTransaction.Current);
        // not called: ExpectHandleReference (strategyMock, _order1, clone, "OrderTicket", ClientTransaction.Current, ClientTransaction.Current);
        // not called: ExpectHandleReference (strategyMock, _order1, clone, "Official", ClientTransaction.Current, ClientTransaction.Current);
        // not called: ExpectHandleReference (strategyMock, _order1, clone, "Customer", ClientTransaction.Current, ClientTransaction.Current);
        // not called: ExpectHandleReference (strategyMock, _order1.OrderItems[0], clone2, "Order", ClientTransaction.Current, ClientTransaction.Current);
      }
      _mockRepository.ReplayAll ();

      _cloner.CreateClone (_order1, strategyMock, contextMock);
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void CreateClone_CallsStrategy_WithCorrectTransactions ()
    {
      ClientTransaction sourceTransaction = ClientTransaction.CreateBindingTransaction ();
      ClientTransaction cloneTransaction = ClientTransaction.CreateRootTransaction ();

      _cloner.CloneTransaction = cloneTransaction;

      var strategyMock = _mockRepository.StrictMock<ICloneStrategy> ();
      var contextMock = _mockRepository.Stub<CloneContext> (_cloner);
      
      Order source;
      using (sourceTransaction.EnterNonDiscardingScope ())
        source = Order.NewObject ();
      Order clone;
      using (cloneTransaction.EnterNonDiscardingScope ())
        clone = Order.NewObject ();

      var shallowClonesFake = new Queue<Tuple<DomainObject, DomainObject>> ();

      SetupResult.For (contextMock.GetCloneFor (source)).Return (clone);
      shallowClonesFake.Enqueue (new Tuple<DomainObject, DomainObject> (source, clone));
      SetupResult.For (contextMock.CloneHulls).Return (shallowClonesFake);

      using (_mockRepository.Unordered ())
      {
        ExpectHandleReference (strategyMock, source, clone, "OrderItems", sourceTransaction, cloneTransaction);
        ExpectHandleReference (strategyMock, source, clone, "OrderTicket", sourceTransaction, cloneTransaction);
        ExpectHandleReference (strategyMock, source, clone, "Official", sourceTransaction, cloneTransaction);
        ExpectHandleReference (strategyMock, source, clone, "Customer", sourceTransaction, cloneTransaction);
      }
      _mockRepository.ReplayAll ();

      _cloner.CreateClone (source, strategyMock, contextMock);
      _mockRepository.VerifyAll ();
    }

    private void ExpectHandleReference (ICloneStrategy strategyMock, TestDomainBase original, TestDomainBase clone, string propertyName,
        ClientTransaction sourceTransaction, ClientTransaction cloneTransaction)
    {
      strategyMock.HandleReference (new PropertyAccessor(), new PropertyAccessor(), null);
      LastCall.Constraints (
          Mocks_Is.Equal (original.Properties[original.GetPublicDomainObjectType (), propertyName, sourceTransaction]),
          Mocks_Is.Equal (clone.Properties[clone.GetPublicDomainObjectType (), propertyName, cloneTransaction]),
          Mocks_Is.Anything ());
    }
  }
}
