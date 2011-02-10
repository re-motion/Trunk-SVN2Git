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
using Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionEndPointDataManagement
{
  [TestFixture]
  public class EndPointDelegatingCollectionDataTest : ClientTransactionBaseTest
  {
    private Order _owningOrder;

    private ICollectionEndPoint _collectionEndPointMock;
    private IDomainObjectCollectionData _endPointDataStub;
    private IDataManagementCommand _nestedCommandMock;
    private ExpandedCommand _expandedCommandFake;
    private IDataManagementCommand _commandStub;

    private EndPointDelegatingCollectionData _delegatingData;

    private OrderItem _orderItem1;
    private OrderItem _orderItem2;
    private OrderItem _orderItem3;

    public override void SetUp ()
    {
      base.SetUp();

      _owningOrder = Order.GetObject (DomainObjectIDs.Order1);

      _collectionEndPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint>();
      StubCollectionEndPoint (_collectionEndPointMock, ClientTransactionMock, _owningOrder);

      _endPointDataStub = MockRepository.GenerateStub<IDomainObjectCollectionData>();

      _commandStub = MockRepository.GenerateStub<IDataManagementCommand>();
      _nestedCommandMock = MockRepository.GenerateMock<IDataManagementCommand> ();
      _expandedCommandFake = new ExpandedCommand (_nestedCommandMock);

      _delegatingData = new EndPointDelegatingCollectionData (_collectionEndPointMock);

      _orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      _orderItem2 = OrderItem.GetObject (DomainObjectIDs.OrderItem2);
      _orderItem3 = OrderItem.GetObject (DomainObjectIDs.OrderItem3);

      ClientTransactionScope.EnterNullScope(); // no active transaction
    }

    public override void TearDown ()
    {
      ClientTransactionScope.ActiveScope.Leave();
      base.TearDown();
    }

    [Test]
    public void Count ()
    {
      _endPointDataStub.Stub (stub => stub.Count).Return (42);
      _collectionEndPointMock.Expect (mock => mock.GetCollectionData()).Return (_endPointDataStub);
      _collectionEndPointMock.Replay();

      Assert.That (_delegatingData.Count, Is.EqualTo (42));

      _collectionEndPointMock.VerifyAllExpectations();
    }

    [Test]
    public void RequiredItemType ()
    {
      Assert.That (_delegatingData.RequiredItemType, Is.Null);
    }

    [Test]
    public void IsReadOnly ()
    {
      Assert.That (_delegatingData.IsReadOnly, Is.False);
    }

    [Test]
    public void AssociatedEndPoint ()
    {
      Assert.That (_delegatingData.AssociatedEndPoint, Is.SameAs (_collectionEndPointMock));
    }

    [Test]
    public void IsDataComplete ()
    {
      _collectionEndPointMock.Stub (stub => stub.IsDataComplete).Return (true).Repeat.Once();
      Assert.That (_delegatingData.IsDataComplete, Is.True);

      _collectionEndPointMock.Stub (stub => stub.IsDataComplete).Return (false).Repeat.Once ();
      Assert.That (_delegatingData.IsDataComplete, Is.False);
    }

    [Test]
    public void EnsureDataComplete ()
    {
      _collectionEndPointMock.Expect (mock => mock.EnsureDataComplete ());
      _collectionEndPointMock.Replay();

      _delegatingData.EnsureDataComplete ();

      _collectionEndPointMock.VerifyAllExpectations();
    }

    [Test]
    public void GetDataStore ()
    {
      var dataStoreStub = new DomainObjectCollectionData ();
      _endPointDataStub.Stub (stub => stub.GetDataStore()).Return (dataStoreStub);
      _collectionEndPointMock.Expect (mock => mock.GetCollectionData ()).Return (_endPointDataStub);
      _collectionEndPointMock.Replay ();

      Assert.That (_delegatingData.GetDataStore(), Is.SameAs(dataStoreStub));

      _collectionEndPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void ContainsObjectID ()
    {
      _endPointDataStub.Stub (stub => stub.ContainsObjectID (_orderItem1.ID)).Return (true);
      _collectionEndPointMock.Expect (mock => mock.GetCollectionData ()).Return (_endPointDataStub);
      _collectionEndPointMock.Replay ();

      Assert.That (_delegatingData.ContainsObjectID (_orderItem1.ID), Is.True);

      _collectionEndPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetObject_Index ()
    {
      _endPointDataStub.Stub (stub => stub.GetObject (1)).Return (_orderItem1);
      _collectionEndPointMock.Expect (mock => mock.GetCollectionData ()).Return (_endPointDataStub);
      _collectionEndPointMock.Replay ();

      Assert.That (_delegatingData.GetObject (1), Is.SameAs(_orderItem1));

      _collectionEndPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetObject_ID ()
    {
      _endPointDataStub.Stub (stub => stub.GetObject (_orderItem1.ID)).Return (_orderItem1);
      _collectionEndPointMock.Expect (mock => mock.GetCollectionData ()).Return (_endPointDataStub);
      _collectionEndPointMock.Replay ();

      Assert.That (_delegatingData.GetObject (_orderItem1.ID), Is.SameAs (_orderItem1));

      _collectionEndPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void IndexOf ()
    {
      _endPointDataStub.Stub (stub => stub.IndexOf(_orderItem1.ID)).Return (3);
      _collectionEndPointMock.Expect (mock => mock.GetCollectionData ()).Return (_endPointDataStub);
      _collectionEndPointMock.Replay ();

      Assert.That (_delegatingData.IndexOf(_orderItem1.ID), Is.EqualTo(3));

      _collectionEndPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetEnumerator ()
    {
      var fakeEnumerator = MockRepository.GenerateStub<IEnumerator<DomainObject>>();
      _endPointDataStub.Stub (stub => stub.GetEnumerator ()).Return (fakeEnumerator);
      _collectionEndPointMock.Expect (mock => mock.GetCollectionData ()).Return (_endPointDataStub);
      _collectionEndPointMock.Replay ();

      Assert.That (_delegatingData.GetEnumerator(), Is.SameAs(fakeEnumerator));

      _collectionEndPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void Clear ()
    {
      var mockRepository = _collectionEndPointMock.GetMockRepository ();

      var removeCommandStub1 = mockRepository.Stub<IDataManagementCommand> ();
      var removeCommandStub2 = mockRepository.Stub<IDataManagementCommand> ();
      var removeCommandStub3 = mockRepository.Stub<IDataManagementCommand> ();

      var nestedCommandMock1 = mockRepository.StrictMock<IDataManagementCommand> ();
      var nestedCommandMock2 = mockRepository.StrictMock<IDataManagementCommand> ();
      var nestedCommandMock3 = mockRepository.StrictMock<IDataManagementCommand> ();

      removeCommandStub1.Stub (stub => stub.ExpandToAllRelatedObjects ()).Return (new ExpandedCommand (nestedCommandMock1));
      removeCommandStub2.Stub (stub => stub.ExpandToAllRelatedObjects ()).Return (new ExpandedCommand (nestedCommandMock2));
      removeCommandStub3.Stub (stub => stub.ExpandToAllRelatedObjects ()).Return (new ExpandedCommand (nestedCommandMock3));

      _endPointDataStub.Stub (stub => stub.Count).Return (3);
      _endPointDataStub.Stub (stub => stub.GetObject (2)).Return (_orderItem3);
      _endPointDataStub.Stub (stub => stub.GetObject (1)).Return (_orderItem2);
      _endPointDataStub.Stub (stub => stub.GetObject (0)).Return (_orderItem1);

      _collectionEndPointMock.Expect (mock => mock.GetCollectionData ()).Return (_endPointDataStub).Repeat.Any();
      _collectionEndPointMock.Expect (mock => mock.CreateRemoveCommand (_orderItem1)).Return (removeCommandStub1);
      _collectionEndPointMock.Expect (mock => mock.CreateRemoveCommand (_orderItem2)).Return (removeCommandStub2);
      _collectionEndPointMock.Expect (mock => mock.CreateRemoveCommand (_orderItem3)).Return (removeCommandStub3);

      using (mockRepository.Ordered ())
      {
        nestedCommandMock3.Expect (mock => mock.NotifyClientTransactionOfBegin ()).Message ("nestedCommandMock3.NotifyClientTransactionOfBegin");
        nestedCommandMock2.Expect (mock => mock.NotifyClientTransactionOfBegin ()).Message ("nestedCommandMock2.NotifyClientTransactionOfBegin");
        nestedCommandMock1.Expect (mock => mock.NotifyClientTransactionOfBegin ()).Message ("nestedCommandMock1.NotifyClientTransactionOfBegin");
        nestedCommandMock3.Expect (mock => mock.Begin ()).Message ("nestedCommandMock3.Begin");
        nestedCommandMock2.Expect (mock => mock.Begin ()).Message ("nestedCommandMock2.Begin");
        nestedCommandMock1.Expect (mock => mock.Begin ()).Message ("nestedCommandMock1.Begin");
        nestedCommandMock3.Expect (mock => mock.Perform ()).Message ("nestedCommandMock3.Perform");
        nestedCommandMock2.Expect (mock => mock.Perform ()).Message ("nestedCommandMock2.Perform");
        nestedCommandMock1.Expect (mock => mock.Perform ()).Message ("nestedCommandMock1.Perform");
        _collectionEndPointMock.Expect (mock => mock.Touch ()).Message ("endPoint.Touch");
        nestedCommandMock1.Expect (mock => mock.End ()).Message ("nestedCommandMock1.End");
        nestedCommandMock2.Expect (mock => mock.End ()).Message ("nestedCommandMock2.End");
        nestedCommandMock3.Expect (mock => mock.End ()).Message ("nestedCommandMock3.End");
        nestedCommandMock1.Expect (mock => mock.NotifyClientTransactionOfEnd ()).Message ("nestedCommandMock1.NotifyClientTransactionOfEnd");
        nestedCommandMock2.Expect (mock => mock.NotifyClientTransactionOfEnd ()).Message ("nestedCommandMock2.NotifyClientTransactionOfEnd");
        nestedCommandMock3.Expect (mock => mock.NotifyClientTransactionOfEnd ()).Message ("nestedCommandMock3.NotifyClientTransactionOfEnd");
      }

      mockRepository.ReplayAll ();

      _delegatingData.Clear();

      mockRepository.VerifyAll ();
    }

    [Test]
    public void Clear_WithoutItems ()
    {
      _endPointDataStub.Stub (stub => stub.Count).Return (0);
      _collectionEndPointMock.Expect (mock => mock.GetCollectionData ()).Return (_endPointDataStub);
      _collectionEndPointMock.Expect (mock => mock.Touch ());
      _collectionEndPointMock.Replay();

      _delegatingData.Clear();

      _collectionEndPointMock.VerifyAllExpectations();
    }

    [Test]
    public void Insert ()
    {
      _collectionEndPointMock.Expect (mock => mock.CreateInsertCommand (_orderItem1, 17)).Return (_commandStub);
      _collectionEndPointMock.Expect (mock => mock.Touch ());
      _collectionEndPointMock.Replay ();
      _commandStub.Stub (stub => stub.ExpandToAllRelatedObjects()).Return (_expandedCommandFake);

      _delegatingData.Insert (17, _orderItem1);

      _collectionEndPointMock.VerifyAllExpectations ();
      DataManagementCommandTestHelper.AssertNotifyAndPerformWasCalled (_nestedCommandMock);
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
      _endPointDataStub.Stub (stub => stub.ContainsObjectID (_orderItem1.ID)).Return (true);
      _collectionEndPointMock.Expect (mock => mock.GetCollectionData ()).Return (_endPointDataStub);
      _collectionEndPointMock.Expect (mock => mock.CreateRemoveCommand (_orderItem1)).Return (_commandStub);
      _collectionEndPointMock.Expect (mock => mock.Touch ());
      _collectionEndPointMock.Replay ();

      _commandStub.Stub (stub => stub.ExpandToAllRelatedObjects()).Return (_expandedCommandFake);

      var result = _delegatingData.Remove (_orderItem1);

      _collectionEndPointMock.VerifyAllExpectations ();
      DataManagementCommandTestHelper.AssertNotifyAndPerformWasCalled (_nestedCommandMock);

      Assert.That (result, Is.True);
    }

    [Test]
    public void Remove_ObjectNotContained ()
    {
      _endPointDataStub.Stub (stub => stub.ContainsObjectID(_orderItem1.ID)).Return (false);
      _collectionEndPointMock.Expect (mock => mock.GetCollectionData ()).Return (_endPointDataStub);
      _collectionEndPointMock.Expect(mock => mock.Touch ());
      _collectionEndPointMock.Replay ();

      bool result = _delegatingData.Remove (_orderItem1);

      _collectionEndPointMock.AssertWasNotCalled (mock => mock.CreateRemoveCommand (Arg<DomainObject>.Is.Anything));
      _collectionEndPointMock.VerifyAllExpectations();
      Assert.That (result, Is.False);
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
      _endPointDataStub.Stub (stub => stub.GetObject (_orderItem1.ID)).Return (_orderItem1);
      _collectionEndPointMock.Expect (mock => mock.GetCollectionData ()).Return (_endPointDataStub);
      _collectionEndPointMock.Expect (mock => mock.CreateRemoveCommand (_orderItem1)).Return (_commandStub);
      _collectionEndPointMock.Expect (mock => mock.Touch());
      _collectionEndPointMock.Replay();
      _commandStub.Stub (stub => stub.ExpandToAllRelatedObjects()).Return (_expandedCommandFake);

      var result = _delegatingData.Remove (_orderItem1.ID);

      _collectionEndPointMock.VerifyAllExpectations ();
      DataManagementCommandTestHelper.AssertNotifyAndPerformWasCalled (_nestedCommandMock);

      Assert.That (result, Is.True);
    }

    [Test]
    public void Remove_ID_ObjectNotContained ()
    {
      _endPointDataStub.Stub (stub => stub.GetObject (_orderItem1.ID)).Return (null);
      _collectionEndPointMock.Expect (mock => mock.GetCollectionData ()).Return (_endPointDataStub);
      _collectionEndPointMock.Expect (mock => mock.Touch ());
      _collectionEndPointMock.Replay ();

      var result = _delegatingData.Remove (_orderItem1.ID);

      _collectionEndPointMock.AssertWasNotCalled (mock => mock.CreateRemoveCommand (Arg<DomainObject>.Is.Anything));
      _collectionEndPointMock.VerifyAllExpectations();

      Assert.That (result, Is.False);
    }

    [Test]
    public void Remove_ID_ChecksErrorConditions ()
    {
      CheckOwningObjectDeletedException ((data, relatedObject) => data.Remove (relatedObject.ID));
    }

    [Test]
    public void Replace ()
    {
      _collectionEndPointMock.Expect (mock => mock.CreateReplaceCommand (17, _orderItem1)).Return (_commandStub);
      _collectionEndPointMock.Expect (mock => mock.Touch ());
      _collectionEndPointMock.Replay ();
      _commandStub.Stub (stub => stub.ExpandToAllRelatedObjects()).Return (_expandedCommandFake);

      _delegatingData.Replace (17, _orderItem1);

      _collectionEndPointMock.VerifyAllExpectations ();
      DataManagementCommandTestHelper.AssertNotifyAndPerformWasCalled (_nestedCommandMock);
    }

    [Test]
    public void Replace_ChecksErrorConditions ()
    {
      CheckClientTransactionDiffersException ((data, relatedObjectInOtherTransaction) => data.Replace (17, relatedObjectInOtherTransaction));
      CheckObjectDeletedException ((data, deletedRelatedObject) => data.Replace (17, deletedRelatedObject));
      CheckOwningObjectDeletedException ((data, relatedObject) => data.Replace (17, relatedObject));
    }

    private ICollectionEndPoint CreateCollectionEndPointStub (ClientTransaction clientTransaction, Order owningOrder)
    {
      var endPointStub = MockRepository.GenerateStub<ICollectionEndPoint>();
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
      endPointStub.Stub (mock => mock.GetDomainObjectReference ()).Return (owningOrder);
    }

    private void CheckClientTransactionDiffersException (Action<EndPointDelegatingCollectionData, DomainObject> action)
    {
      var orderItemInOtherTransaction = DomainObjectMother.CreateObjectInTransaction<OrderItem> (ClientTransaction.CreateRootTransaction());
      try
      {
        action (_delegatingData, orderItemInOtherTransaction);
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
      using (_delegatingData.AssociatedEndPoint.ClientTransaction.EnterNonDiscardingScope())
      {
        deletedObject = OrderItem.GetObject (DomainObjectIDs.OrderItem5);
        deletedObject.Delete();
      }

      try
      {
        action (_delegatingData, deletedObject);
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
      using (_delegatingData.AssociatedEndPoint.ClientTransaction.EnterNonDiscardingScope())
      {
        deletedOwningObject = Order.GetObject (DomainObjectIDs.Order4);
      }

      var endPointStub = CreateCollectionEndPointStub (ClientTransactionMock, deletedOwningObject);
      var data = new EndPointDelegatingCollectionData (endPointStub);

      using (_delegatingData.AssociatedEndPoint.ClientTransaction.EnterNonDiscardingScope())
      {
        deletedOwningObject.Delete();
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
  }
}