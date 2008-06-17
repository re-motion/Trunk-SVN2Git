/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Cloning;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;
using Mocks_Is = Rhino.Mocks.Constraints.Is;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Cloning
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

      using (ClientTransaction.NewBindingTransaction ().EnterNonDiscardingScope ())
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
      ClientTransaction cloneTransaction = ClientTransaction.NewBindingTransaction ();
      _cloner.CloneTransaction = cloneTransaction;
      Assert.That (_cloner.CloneTransaction, Is.SameAs (cloneTransaction));
    }

    [Test]
    public void CloneTransaction_Reset ()
    {
      ClientTransaction cloneTransaction = ClientTransaction.NewBindingTransaction ();
      _cloner.CloneTransaction = cloneTransaction;
      _cloner.CloneTransaction = null;
      Assert.That (_cloner.CloneTransaction, Is.SameAs (ClientTransactionMock));
    }

    [Test]
    public void CreateValueClone_CreatesNewObject ()
    {
      DomainObject clone = _cloner.CreateValueClone (_classWithAllDataTypes);
      Assert.That (clone, Is.Not.SameAs (_classWithAllDataTypes));
      Assert.That (clone.ID, Is.Not.EqualTo (_classWithAllDataTypes));
      Assert.That (clone.State, Is.EqualTo (StateType.New));
    }

    [Test]
    public void CreateValueClone_CreatesNewObjectInCorrectTransaction ()
    {
      ClientTransaction cloneTransaction = ClientTransaction.NewBindingTransaction ();
      _cloner.CloneTransaction = cloneTransaction;
      DomainObject clone = _cloner.CreateValueClone (_classWithAllDataTypes);
      Assert.That (clone.ClientTransaction, Is.SameAs (cloneTransaction));
    }

    [Test]
    public void CreateValueClone_CreatesObjectOfSameType ()
    {
      DomainObject clone = _cloner.CreateValueClone<DomainObject> (_classWithAllDataTypes);
      Assert.That (clone.GetPublicDomainObjectType(), Is.SameAs (typeof (ClassWithAllDataTypes)));
    }

    [Test]
    public void CreateValueClone_CallsNoCtor ()
    {
      Order clone = _cloner.CreateValueClone (_order1);
      Assert.That (clone.CtorCalled, Is.False);
    }

    [Test]
    public void CreateValueClone_RegistersDataContainer ()
    {
      ClassWithAllDataTypes clone = _cloner.CreateValueClone (_classWithAllDataTypes);
      Assert.That (clone.InternalDataContainer.ClientTransaction, Is.SameAs (ClientTransactionMock));
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
      Assert.That (clone.InternalDataContainer.PropertyValues[ReflectionUtility.GetPropertyName (typeof (Computer), "Employee")].Value, Is.Null);
    }

    [Test]
    public void CreateValueClone_RelationProperties_ForeignKey_OriginalValue ()
    {
      Computer clone = _cloner.CreateValueClone (_computer1);
      Assert.That (_computer1.Employee, Is.Not.Null);
      Assert.That (clone.Properties[typeof (Computer), "Employee"].GetOriginalValue<Employee> (), Is.Null);
      Assert.That (clone.InternalDataContainer.PropertyValues[ReflectionUtility.GetPropertyName (typeof (Computer), "Employee")].OriginalValue, Is.Null);
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
      ICloneStrategy strategyMock = _mockRepository.Stub<ICloneStrategy> ();
      _mockRepository.ReplayAll ();
      
      Order clone = _cloner.CreateClone (_order1, strategyMock);
      Assert.That (clone.OrderNumber, Is.EqualTo (_order1.OrderNumber));
      Assert.That (clone.DeliveryDate, Is.EqualTo (_order1.DeliveryDate));
    }

    [Test]
    public void CreateClone_CallsStrategyForReferences ()
    {
      ICloneStrategy strategyMock = _mockRepository.CreateMock<ICloneStrategy>();
      CloneContext contextMock = _mockRepository.Stub<CloneContext>(_cloner);
      Order clone = Order.NewObject();
      Queue<Tuple<DomainObject, DomainObject>> shallowClonesFake = new Queue<Tuple<DomainObject, DomainObject>> ();

      SetupResult.For (contextMock.GetCloneFor (_order1)).Return (clone);
      shallowClonesFake.Enqueue (new Tuple<DomainObject, DomainObject> (_order1, clone));
      SetupResult.For (contextMock.ShallowClones).Return (shallowClonesFake);

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
      ICloneStrategy strategyMock = _mockRepository.CreateMock<ICloneStrategy> ();
      CloneContext contextMock = _mockRepository.Stub<CloneContext> (_cloner);
      Order clone = Order.NewObject ();
      OrderItem clone2 = OrderItem.NewObject ();
      Queue<Tuple<DomainObject, DomainObject>> shallowClonesFake = new Queue<Tuple<DomainObject, DomainObject>> ();

      SetupResult.For (contextMock.GetCloneFor (_order1)).Return (clone);
      shallowClonesFake.Enqueue (new Tuple<DomainObject, DomainObject> (_order1, clone));
      shallowClonesFake.Enqueue (new Tuple<DomainObject, DomainObject> (_order1.OrderItems[0], clone2));
      SetupResult.For (contextMock.ShallowClones).Return (shallowClonesFake);

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
    public void CreateClone_CallsStrategy_WithCorrectTransactions ()
    {
      ClientTransaction sourceTransaction = ClientTransaction.NewBindingTransaction ();
      ClientTransaction cloneTransaction = ClientTransaction.NewRootTransaction ();

      _cloner.CloneTransaction = cloneTransaction;

      ICloneStrategy strategyMock = _mockRepository.CreateMock<ICloneStrategy> ();
      CloneContext contextMock = _mockRepository.Stub<CloneContext> (_cloner);
      
      Order source;
      using (sourceTransaction.EnterNonDiscardingScope ())
        source = Order.NewObject ();
      Order clone;
      using (cloneTransaction.EnterNonDiscardingScope ())
        clone = Order.NewObject ();

      Queue<Tuple<DomainObject, DomainObject>> shallowClonesFake = new Queue<Tuple<DomainObject, DomainObject>> ();

      SetupResult.For (contextMock.GetCloneFor (source)).Return (clone);
      shallowClonesFake.Enqueue (new Tuple<DomainObject, DomainObject> (source, clone));
      SetupResult.For (contextMock.ShallowClones).Return (shallowClonesFake);

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
      strategyMock.HandleReference (new PropertyAccessor(), null, new PropertyAccessor(), null, null);
      LastCall.Constraints (
          Mocks_Is.Equal (original.Properties[original.GetPublicDomainObjectType (), propertyName]),
          Mocks_Is.Same (sourceTransaction),
          Mocks_Is.Equal (clone.Properties[clone.GetPublicDomainObjectType (), propertyName]),
          Mocks_Is.Same (cloneTransaction),
          Mocks_Is.Anything ());
    }
  }
}
