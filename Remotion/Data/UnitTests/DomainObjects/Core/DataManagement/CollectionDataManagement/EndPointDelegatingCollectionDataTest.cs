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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Reflection;
using Rhino.Mocks;
using Remotion.Development.UnitTesting;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionDataManagement
{
  [TestFixture]
  public class EndPointDelegatingCollectionDataTest : ClientTransactionBaseTest
  {
    private Order _owningOrder;

    private ICollectionEndPoint _collectionEndPointMock;
    private DomainObjectCollectionData _actualData;
    private BidirectionalRelationModificationBase _bidirectionalModificationMock;
    private IRelationEndPointModification _modificationStub;

    private EndPointDelegatingCollectionData _data;

    private OrderItem _orderItem1;
    private OrderItem _orderItem2;
    private OrderItem _orderItem3;

    public override void SetUp ()
    {
      base.SetUp();

      _owningOrder = Order.GetObject (DomainObjectIDs.Order1);

      _collectionEndPointMock = MockRepository.GenerateMock<ICollectionEndPoint>();
      StubCollectionEndPoint(_collectionEndPointMock, ClientTransactionMock, _owningOrder);

      _actualData = new DomainObjectCollectionData ();

      _modificationStub = MockRepository.GenerateStub<IRelationEndPointModification> ();
      _bidirectionalModificationMock = MockRepository.GenerateMock<BidirectionalRelationModificationBase> (new[] { new IRelationEndPointModification[0] });

      _data = new EndPointDelegatingCollectionData (_collectionEndPointMock, _actualData);

      _orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      _orderItem2 = OrderItem.GetObject (DomainObjectIDs.OrderItem2);
      _orderItem3 = OrderItem.GetObject (DomainObjectIDs.OrderItem3);

      ClientTransactionScope.EnterNullScope (); // no active transaction
    }

    public override void TearDown ()
    {
      ClientTransactionScope.ActiveScope.Leave ();
      base.TearDown ();
    }

    [Test]
    public void Count ()
    {
      PrepareActualDataContents (_orderItem1);

      Assert.That (_data.Count, Is.EqualTo (1));
    }

    [Test]
    public void IsReadOnly ()
    {
      Assert.That (_data.IsReadOnly, Is.False);
    }

    [Test]
    public void AssociatedEndPoint ()
    {
      Assert.That (_data.AssociatedEndPoint, Is.SameAs (_collectionEndPointMock));
    }

    [Test]
    public void GetUndecoratedDataStore ()
    {
      Assert.That (_data.GetUndecoratedDataStore (), Is.SameAs (_actualData));
    }

    [Test]
    public void ContainsObjectID ()
    {
      PrepareActualDataContents (_orderItem1);

      Assert.That (_data.ContainsObjectID (DomainObjectIDs.OrderItem1), Is.True);
      Assert.That (_data.ContainsObjectID (DomainObjectIDs.OrderItem2), Is.False);
    }

    [Test]
    public void GetObject_Index ()
    {
      PrepareActualDataContents (_orderItem1, _orderItem2, _orderItem3);

      Assert.That (_data.GetObject (1), Is.SameAs (_orderItem2));
    }

    [Test]
    public void GetObject_ID ()
    {
      PrepareActualDataContents (_orderItem1, _orderItem2, _orderItem3);

      Assert.That (_data.GetObject (DomainObjectIDs.OrderItem1), Is.SameAs (_orderItem1));
    }

    [Test]
    public void IndexOf ()
    {
      PrepareActualDataContents (_orderItem1, _orderItem2, _orderItem3);

      Assert.That (_data.IndexOf (DomainObjectIDs.OrderItem2), Is.EqualTo (1));
    }

    [Test]
    public void GetEnumerator ()
    {
      PrepareActualDataContents (_orderItem1, _orderItem2, _orderItem3);

      IEnumerable<DomainObject> dataAsEnumerable = _data;
      Assert.That (dataAsEnumerable.ToArray(), Is.EqualTo (new[] { _orderItem1, _orderItem2, _orderItem3 }));
    }

    [Test]
    public void Clear ()
    {
      PrepareActualDataContents (_orderItem1, _orderItem2, _orderItem3);

      using (_collectionEndPointMock.GetMockRepository ().Ordered ())
      {
        _collectionEndPointMock.Expect (mock => mock.CreateRemoveModification (_orderItem3)).Return (_modificationStub);
        _collectionEndPointMock.Expect (mock => mock.CreateRemoveModification (_orderItem2)).Return (_modificationStub);
        _collectionEndPointMock.Expect (mock => mock.CreateRemoveModification (_orderItem1)).Return (_modificationStub);
      }
      _collectionEndPointMock.Expect (mock => mock.Touch());
      _collectionEndPointMock.Replay();

      _modificationStub.Stub (stub => stub.CreateBidirectionalModification()).Return (_bidirectionalModificationMock);

      _bidirectionalModificationMock.Expect (mock => mock.ExecuteAllSteps ()).Repeat.Times (3);
      _bidirectionalModificationMock.Replay ();

      _data.Clear();

      _collectionEndPointMock.VerifyAllExpectations ();
      _bidirectionalModificationMock.VerifyAllExpectations ();
    }

    [Test]
    public void Clear_WithoutItems ()
    {
      PrepareActualDataContents ();

      _collectionEndPointMock.Expect (mock => mock.Touch ());
      _collectionEndPointMock.Replay ();

      _data.Clear ();

      _collectionEndPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void Insert ()
    {
      _collectionEndPointMock.Expect (mock => mock.CreateInsertModification (_orderItem1, 17)).Return (_modificationStub);
      _collectionEndPointMock.Expect (mock => mock.Touch ());
      _collectionEndPointMock.Replay ();
      _modificationStub.Stub (stub => stub.CreateBidirectionalModification ()).Return (_bidirectionalModificationMock);

      _data.Insert (17, _orderItem1);

      _collectionEndPointMock.VerifyAllExpectations ();
      _bidirectionalModificationMock.AssertWasCalled (mock => mock.ExecuteAllSteps ());
    }

    [Test]
    public void Insert_ChecksErrorConditions ()
    {
      CheckClientTransactionDiffersException ((data, relatedObjectInOtherTransaction) => data.Insert (17, relatedObjectInOtherTransaction));
      CheckObjectDeletedException ((data, deletedRelatedObject) => data.Insert (17, deletedRelatedObject));
      CheckOwningObjectDeletedException ((data, relatedObject) => data.Insert (17, relatedObject));
    }

    [Test]
    public void Remove ()
    {
      PrepareActualDataContents (_orderItem1);

      _collectionEndPointMock.Expect (mock => mock.CreateRemoveModification (_orderItem1)).Return (_modificationStub);
      _collectionEndPointMock.Expect (mock => mock.Touch ());
      _collectionEndPointMock.Replay ();
      _modificationStub.Stub (stub => stub.CreateBidirectionalModification ()).Return (_bidirectionalModificationMock);

      _data.Remove (_orderItem1);

      _collectionEndPointMock.VerifyAllExpectations ();
      _bidirectionalModificationMock.AssertWasCalled (mock => mock.ExecuteAllSteps ());
    }

    [Test]
    public void Remove_ObjectNotContained ()
    {
      _data.Remove (_orderItem1);

      _collectionEndPointMock.AssertWasNotCalled (mock => mock.CreateRemoveModification (Arg<DomainObject>.Is.Anything));
      _collectionEndPointMock.AssertWasCalled (mock => mock.Touch ());
    }

    [Test]
    public void Remove_ChecksErrorConditions ()
    {
      CheckClientTransactionDiffersException ((data, relatedObjectInOtherTransaction) => data.Remove (relatedObjectInOtherTransaction));
      CheckObjectDeletedException ((data, deletedRelatedObject) => data.Remove (deletedRelatedObject));
      CheckOwningObjectDeletedException ((data, relatedObject) => data.Remove (relatedObject));
    }

    [Test]
    public void Remove_ID ()
    {
      PrepareActualDataContents (_orderItem1);

      _collectionEndPointMock.Expect (mock => mock.CreateRemoveModification (_orderItem1)).Return (_modificationStub);
      _collectionEndPointMock.Expect (mock => mock.Touch ());
      _collectionEndPointMock.Replay ();
      _modificationStub.Stub (stub => stub.CreateBidirectionalModification ()).Return (_bidirectionalModificationMock);

      _data.Remove (_orderItem1.ID);

      _collectionEndPointMock.VerifyAllExpectations ();
      _bidirectionalModificationMock.AssertWasCalled (mock => mock.ExecuteAllSteps ());
    }

    [Test]
    public void Remove_ID_ObjectNotContained ()
    {
      _data.Remove (_orderItem1.ID);

      _collectionEndPointMock.AssertWasNotCalled (mock => mock.CreateRemoveModification (Arg<DomainObject>.Is.Anything));
      _collectionEndPointMock.AssertWasCalled (mock => mock.Touch ());
    }

    [Test]
    public void Remove_ID_ChecksErrorConditions ()
    {
      CheckOwningObjectDeletedException ((data, relatedObject) => data.Remove (relatedObject.ID));
    }

    [Test]
    public void Replace ()
    {
      _collectionEndPointMock.Expect (mock => mock.CreateReplaceModification (17, _orderItem1)).Return (_modificationStub);
      _collectionEndPointMock.Expect (mock => mock.Touch ());
      _collectionEndPointMock.Replay ();
      _modificationStub.Stub (stub => stub.CreateBidirectionalModification ()).Return (_bidirectionalModificationMock);

      _data.Replace (17, _orderItem1);

      _collectionEndPointMock.VerifyAllExpectations ();
      _bidirectionalModificationMock.AssertWasCalled (mock => mock.ExecuteAllSteps ());
    }

    [Test]
    public void Replace_ChecksErrorConditions ()
    {
      CheckClientTransactionDiffersException ((data, relatedObjectInOtherTransaction) => data.Replace (17, relatedObjectInOtherTransaction));
      CheckObjectDeletedException ((data, deletedRelatedObject) => data.Replace (17, deletedRelatedObject));
      CheckOwningObjectDeletedException ((data, relatedObject) => data.Replace (17, relatedObject));
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = 
        @"Cannot xx DomainObject 'OrderItem\|.*@|System.Guid' from/to collection of property "
        + @"'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' of DomainObject 'Order\|.*\|System.Guid'. The objects do not belong "
        + @"to the same ClientTransaction.", MatchType = MessageMatch.Regex)]
    public void ClientTransactionsDiffer_NoObjectInBindingTransaction ()
    {
      var owningObject = CreateDomainObjectInTransaction<Order> (ClientTransactionMock);
      var relatedObject = CreateDomainObjectInTransaction<OrderItem> (ClientTransaction.CreateRootTransaction ());

      var endPointStub = CreateCollectionEndPointStub (ClientTransactionMock, owningObject);
      var data = new EndPointDelegatingCollectionData (endPointStub, _actualData);

      CallCheckClientTransaction(data, relatedObject);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage =
        @"Cannot xx DomainObject 'OrderItem\|.*@|System.Guid' from/to collection of property "
        + @"'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' of DomainObject 'Order\|.*\|System.Guid'. The objects do not belong "
        + @"to the same ClientTransaction. The OrderItem object is bound to a BindingClientTransaction.", MatchType = MessageMatch.Regex)]
    public void ClientTransactionsDiffer_RelatedObjectInBindingTransaction ()
    {
      var bindingTransaction = ClientTransaction.CreateBindingTransaction ();

      var owningObject = CreateDomainObjectInTransaction<Order> (ClientTransactionMock);
      var relatedObject = CreateDomainObjectInTransaction<OrderItem> (bindingTransaction);

      var endPointStub = CreateCollectionEndPointStub (ClientTransactionMock, owningObject);
      var data = new EndPointDelegatingCollectionData (endPointStub, _actualData);

      CallCheckClientTransaction (data, relatedObject);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage =
        @"Cannot xx DomainObject 'OrderItem\|.*@|System.Guid' from/to collection of property "
        + @"'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' of DomainObject 'Order\|.*\|System.Guid'. The objects do not belong "
        + @"to the same ClientTransaction. The Order object owning the collection is bound to a BindingClientTransaction.",
        MatchType = MessageMatch.Regex)]
    public void ClientTransactionsDiffer_OwningObjectInBindingTransaction ()
    {
      var bindingTransaction = ClientTransaction.CreateBindingTransaction();

      var owningObject = CreateDomainObjectInTransaction<Order> (bindingTransaction);
      var relatedObject = CreateDomainObjectInTransaction<OrderItem> (ClientTransaction.CreateRootTransaction ());

      var endPointStub = CreateCollectionEndPointStub (bindingTransaction, owningObject);
      var data = new EndPointDelegatingCollectionData (endPointStub, _actualData);

      CallCheckClientTransaction (data, relatedObject);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage =
        @"Cannot xx DomainObject 'OrderItem\|.*@|System.Guid' from/to collection of property "
        + @"'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' of DomainObject 'Order\|.*\|System.Guid'. The objects do not belong "
        + @"to the same ClientTransaction. The OrderItem object is bound to a BindingClientTransaction. The Order object owning the collection is "
        + @"also bound, but to a different BindingClientTransaction.",
        MatchType = MessageMatch.Regex)]
    public void ClientTransactionsDiffer_BothObjectsInBindingTransactions ()
    {
      var bindingTransaction1 = ClientTransaction.CreateBindingTransaction ();
      var bindingTransaction2 = ClientTransaction.CreateBindingTransaction ();
      var owningObject = CreateDomainObjectInTransaction<Order> (bindingTransaction1);
      var relatedObject = CreateDomainObjectInTransaction<OrderItem> (bindingTransaction2);

      var endPointStub = CreateCollectionEndPointStub (bindingTransaction1, owningObject);
      var data = new EndPointDelegatingCollectionData (endPointStub, _actualData);

      CallCheckClientTransaction (data, relatedObject);
    }

    private void CallCheckClientTransaction (EndPointDelegatingCollectionData data, DomainObject relatedObject)
    {
      PrivateInvoke.InvokeNonPublicMethod (
          data,
          "CheckClientTransaction",
          relatedObject,
          "Cannot xx DomainObject '{0}' from/to collection of property '{1}' of DomainObject '{2}'.");
    }

    private T CreateDomainObjectInTransaction<T> (ClientTransaction transaction) where T : DomainObject
    {
      using (transaction.EnterNonDiscardingScope ())
      {
        return (T) RepositoryAccessor.NewObject (typeof (T), ParamList.Empty);
      }
    }

    private ICollectionEndPoint CreateCollectionEndPointStub (ClientTransaction clientTransaction, Order owningOrder)
    {
      var endPointStub = MockRepository.GenerateStub<ICollectionEndPoint> ();
      StubCollectionEndPoint (endPointStub, clientTransaction, owningOrder);
      return endPointStub;
    }

    private void StubCollectionEndPoint (ICollectionEndPoint endPointStub, ClientTransaction clientTransaction, Order owningOrder)
    {
      endPointStub.Stub (stub => stub.ClientTransaction).Return (clientTransaction);
      var relationEndPointDefinition = owningOrder.ID.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");
      endPointStub.Stub (mock => mock.ObjectID).Return (owningOrder.ID);
      endPointStub.Stub (mock => mock.Definition).Return (relationEndPointDefinition);
      endPointStub.Stub (mock => mock.GetDomainObject ()).Return (owningOrder);
    }

    private void CheckClientTransactionDiffersException (Action<EndPointDelegatingCollectionData, DomainObject> action)
    {
      var orderItemInOtherTransaction = CreateDomainObjectInTransaction<OrderItem> (ClientTransaction.CreateRootTransaction ());
      try
      {
        action (_data, orderItemInOtherTransaction);
        Assert.Fail ("Expected ClientTransactionsDifferException");
      }
      catch (ClientTransactionsDifferException)
      {
        // ok
      }
    }

    private void CheckObjectDeletedException (Action<EndPointDelegatingCollectionData, DomainObject> action)
    {
      OrderItem deletedObject;
      using (_data.CollectionEndPoint.ClientTransaction.EnterNonDiscardingScope ())
      {
        deletedObject = OrderItem.GetObject (DomainObjectIDs.OrderItem5);
        deletedObject.Delete();
      }

      try
      {
        action (_data, deletedObject);
        Assert.Fail ("Expected ObjectDeletedException");
      }
      catch (ObjectDeletedException)
      {
        // ok
      }
    }

    private void CheckOwningObjectDeletedException (Action<EndPointDelegatingCollectionData, DomainObject> action)
    {
      Order deletedOwningObject;
      using (_data.CollectionEndPoint.ClientTransaction.EnterNonDiscardingScope ())
      {
        deletedOwningObject = Order.GetObject (DomainObjectIDs.Order4);
      }

      var endPointStub = CreateCollectionEndPointStub (ClientTransactionMock, deletedOwningObject);
      var data = new EndPointDelegatingCollectionData (endPointStub, _actualData);

      using (_data.CollectionEndPoint.ClientTransaction.EnterNonDiscardingScope ())
      {
        deletedOwningObject.Delete ();
      }

      try
      {
        action (data, _orderItem1);
        Assert.Fail ("Expected ObjectDeletedException");
      }
      catch (ObjectDeletedException)
      {
        // ok
      }
    }

    private void PrepareActualDataContents (params OrderItem[] orderItems)
    {
      for (int i = 0; i < orderItems.Length; i++)
      {
        _actualData.Insert (i, orderItems[i]);
      }
    }
  }
}