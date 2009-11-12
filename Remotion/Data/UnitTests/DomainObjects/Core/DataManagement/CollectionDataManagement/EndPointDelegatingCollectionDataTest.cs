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

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionDataManagement
{
  [TestFixture]
  public class EndPointDelegatingCollectionDataTest : ClientTransactionBaseTest
  {
    private Order _owningOrder;

    private ICollectionEndPoint _collectionEndPointMock;
    private IDomainObjectCollectionData _actualDataStub;
    private BidirectionalRelationModificationBase _bidirectionalModificationMock;
    private IRelationEndPointModification _modificationStub;

    private EndPointDelegatingCollectionData _data;

    private OrderItem _orderItem;
    private OrderItem _orderItemInOtherTransaction;

    public override void SetUp ()
    {
      base.SetUp();

      _owningOrder = Order.GetObject (DomainObjectIDs.Order1);

      _collectionEndPointMock = MockRepository.GenerateMock<ICollectionEndPoint>();
      StubCollectionEndPoint(_collectionEndPointMock, ClientTransactionMock, _owningOrder);

      _actualDataStub = MockRepository.GenerateStub<IDomainObjectCollectionData>();

      _modificationStub = MockRepository.GenerateStub<IRelationEndPointModification> ();
      _bidirectionalModificationMock = MockRepository.GenerateMock<BidirectionalRelationModificationBase> (new[] { new IRelationEndPointModification[0] });

      _data = new EndPointDelegatingCollectionData (_collectionEndPointMock, _actualDataStub);

      _orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      _orderItemInOtherTransaction = CreateDomainObjectInTransaction<OrderItem> (ClientTransaction.CreateRootTransaction ());

      ClientTransactionScope.EnterNullScope (); // no active transaction
    }

    public override void TearDown ()
    {
      ClientTransactionScope.ActiveScope.Leave ();
      base.TearDown ();
    }

    [Test]
    public void IsReadOnly ()
    {
      Assert.That (_data.IsReadOnly, Is.False);
    }

    [Test]
    public void Count ()
    {
      _actualDataStub.Stub (stub => stub.Count).Return (12);
      Assert.That (_data.Count, Is.EqualTo (12));
    }

    [Test]
    public void ContainsObjectID ()
    {
      _actualDataStub.Stub (stub => stub.ContainsObjectID (DomainObjectIDs.OrderItem1)).Return (true);
      _actualDataStub.Stub (stub => stub.ContainsObjectID (DomainObjectIDs.OrderItem2)).Return (false);

      Assert.That (_data.ContainsObjectID (DomainObjectIDs.OrderItem1), Is.True);
      Assert.That (_data.ContainsObjectID (DomainObjectIDs.OrderItem2), Is.False);
    }

    [Test]
    public void GetObject_Index ()
    {
      _actualDataStub.Stub (stub => stub.GetObject (12)).Return (_orderItem);

      Assert.That (_data.GetObject (12), Is.SameAs (_orderItem));
    }

    [Test]
    public void GetObject_ID ()
    {
      _actualDataStub.Stub (stub => stub.GetObject (DomainObjectIDs.OrderItem1)).Return (_orderItem);

      Assert.That (_data.GetObject (DomainObjectIDs.OrderItem1), Is.SameAs (_orderItem));
    }

    [Test]
    public void IndexOf ()
    {
      _actualDataStub.Stub (stub => stub.IndexOf (DomainObjectIDs.OrderItem1)).Return (17);

      Assert.That (_data.IndexOf (DomainObjectIDs.OrderItem1), Is.EqualTo (17));
    }

    [Test]
    public void GetEnumerator ()
    {
      var enumeratorStub = MockRepository.GenerateStub<IEnumerator<DomainObject>> ();
      _actualDataStub.Stub (stub => stub.GetEnumerator()).Return (enumeratorStub);

      Assert.That (_data.GetEnumerator(), Is.SameAs (enumeratorStub));
    }

    [Test]
    [Ignore ("TODO 1780")]
    public void Clear ()
    {
      Assert.Fail ("TODO");
    }

    [Test]
    [Ignore ("TODO 1780")]
    public void Insert ()
    {
      Assert.Fail ("TODO");
    }

    [Test]
    public void Remove ()
    {
      _actualDataStub.Stub (stub => stub.ContainsObjectID (_orderItem.ID)).Return (true);
      _collectionEndPointMock.Expect (mock => mock.CreateRemoveModification (_orderItem)).Return (_modificationStub);
      _collectionEndPointMock.Replay ();
      _modificationStub.Stub (stub => stub.CreateBidirectionalModification ()).Return (_bidirectionalModificationMock);

      _data.Remove (_orderItem);

      _collectionEndPointMock.VerifyAllExpectations ();
      _bidirectionalModificationMock.AssertWasCalled (mock => mock.ExecuteAllSteps ());
    }

    [Test]
    public void Remove_ObjectNotContained ()
    {
      _actualDataStub.Stub (stub => stub.ContainsObjectID (_orderItem.ID)).Return (false);

      _data.Remove (_orderItem);

      _collectionEndPointMock.AssertWasNotCalled (mock => mock.CreateRemoveModification (Arg<DomainObject>.Is.Anything));
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = 
        @"Cannot remove DomainObject 'OrderItem\|.*\|System.Guid' from collection of property "
        + @"'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' of DomainObject 'Order\|.*\|System.Guid'. The objects do not belong "
        + @"to the same ClientTransaction.", MatchType = MessageMatch.Regex)]
    public void Remove_ClientTransactionDiffers ()
    {
      _data.Remove (_orderItemInOtherTransaction);
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void Remove_ObjectDeleted ()
    {
      SetTransactionAndDelete (_orderItem);

      _data.Remove (_orderItem);
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void Remove_OwningObjectDeleted ()
    {
      SetTransactionAndDelete (_owningOrder);

      _data.Remove (_orderItem);
    }
    
    [Test]
    [Ignore ("TODO 1780")]
    public void Replace ()
    {
      Assert.Fail ("TODO");
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
      var data = new EndPointDelegatingCollectionData (endPointStub, _actualDataStub);

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
      var data = new EndPointDelegatingCollectionData (endPointStub, _actualDataStub);

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
      var data = new EndPointDelegatingCollectionData (endPointStub, _actualDataStub);

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
      var data = new EndPointDelegatingCollectionData (endPointStub, _actualDataStub);

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

    private void SetTransactionAndDelete (DomainObject domainObject)
    {
      using (ClientTransactionMock.EnterNonDiscardingScope ())
      {
        RepositoryAccessor.DeleteObject (domainObject);
      }
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
      var relationEndPointID = new RelationEndPointID (owningOrder.ID, typeof (Order).FullName + ".OrderItems");
      endPointStub.Stub (mock => mock.ID).Return (relationEndPointID);
      endPointStub.Stub (mock => mock.GetDomainObject ()).Return (owningOrder);
    }
  }
}