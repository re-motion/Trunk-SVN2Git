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
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionDataManagement
{
  [TestFixture]
  public class EndPointDelegatingCollectionDataTest : ClientTransactionBaseTest
  {
    private Order _owningOrder;

    private ICollectionEndPoint _collectionEndPointMock;
    private ICollectionEndPointData _endPointDataStub;
    private IDomainObjectCollectionData _dataStoreStub;
    private IDataManagementCommand _nestedCommandMock;
    private ExpandedCommand _expandedCommandFake;
    private IDataManagementCommand _commandStub;

    private EndPointDelegatingCollectionData _data;

    private OrderItem _orderItem1;
    private OrderItem _orderItem2;
    private OrderItem _orderItem3;

    public override void SetUp ()
    {
      base.SetUp();

      _owningOrder = Order.GetObject (DomainObjectIDs.Order1);

      _collectionEndPointMock = MockRepository.GenerateMock<ICollectionEndPoint>();
      StubCollectionEndPoint (_collectionEndPointMock, ClientTransactionMock, _owningOrder);

      _dataStoreStub = MockRepository.GenerateStub<IDomainObjectCollectionData>();

      _commandStub = MockRepository.GenerateStub<IDataManagementCommand>();
      _nestedCommandMock = MockRepository.GenerateMock<IDataManagementCommand> ();
      _expandedCommandFake = new ExpandedCommand (_nestedCommandMock);

      _endPointDataStub = MockRepository.GenerateStub<ICollectionEndPointData>();
      _endPointDataStub.Stub (stub => stub.CollectionData).Return (_dataStoreStub);
      _data = new EndPointDelegatingCollectionData (_collectionEndPointMock, _endPointDataStub);

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
      PrepareActualDataContents (_orderItem1);

      Assert.That (_data.Count, Is.EqualTo (1));
    }

    [Test]
    public void RequiredItemType ()
    {
      _dataStoreStub.Stub (mock => mock.RequiredItemType).Return (typeof (Computer));

      Assert.That (_data.RequiredItemType, Is.SameAs (typeof (Computer)));
    }

    [Test]
    public void IsReadOnly ()
    {
      _dataStoreStub.Stub (stub => stub.IsReadOnly).Return (false);
      Assert.That (_data.IsReadOnly, Is.False);

      _dataStoreStub.BackToRecord ();
      _dataStoreStub.Stub (stub => stub.IsReadOnly).Return (true);
      _dataStoreStub.Replay ();

      Assert.That (_data.IsReadOnly, Is.True);
    }

    [Test]
    public void AssociatedEndPoint ()
    {
      Assert.That (_data.AssociatedEndPoint, Is.SameAs (_collectionEndPointMock));
    }

    [Test]
    public void IsDataAvailable ()
    {
      _endPointDataStub.Stub (stub => stub.IsDataAvailable).Return (true);
      Assert.That (_data.IsDataAvailable, Is.True);

      _endPointDataStub.BackToRecord();
      _endPointDataStub.Stub (stub => stub.IsDataAvailable).Return (false);
      _dataStoreStub.Replay ();

      Assert.That (_data.IsDataAvailable, Is.False);
    }

    [Test]
    public void EnsureDataAvailable ()
    {
      _data.EnsureDataAvailable();

      _endPointDataStub.AssertWasCalled (mock => mock.EnsureDataAvailable());
    }

    [Test]
    public void GetDataStore ()
    {
      var dataStoreStub = new DomainObjectCollectionData();
      _dataStoreStub.Stub (mock => mock.GetDataStore()).Return (dataStoreStub);

      Assert.That (_data.GetDataStore(), Is.SameAs (dataStoreStub));
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
    public void Clear_CompositeCommand ()
    {
      PrepareActualDataContents (_orderItem1, _orderItem2, _orderItem3);

      var mockRepository = _collectionEndPointMock.GetMockRepository ();

      var removeCommandStub1 = mockRepository.Stub<IDataManagementCommand> ();
      var removeCommandStub2 = mockRepository.Stub<IDataManagementCommand> ();
      var removeCommandStub3 = mockRepository.Stub<IDataManagementCommand> ();

      _collectionEndPointMock.Expect (mock => mock.CreateRemoveCommand (_orderItem1)).Return (removeCommandStub1);
      _collectionEndPointMock.Expect (mock => mock.CreateRemoveCommand (_orderItem2)).Return (removeCommandStub2);
      _collectionEndPointMock.Expect (mock => mock.CreateRemoveCommand (_orderItem3)).Return (removeCommandStub3);

      var expandedCommand1 = new ExpandedCommand ();
      var expandedCommand2 = new ExpandedCommand ();
      var expandedCommand3 = new ExpandedCommand ();
      
      removeCommandStub1.Stub (stub => stub.ExpandToAllRelatedObjects ()).Return (expandedCommand1);
      removeCommandStub2.Stub (stub => stub.ExpandToAllRelatedObjects ()).Return (expandedCommand2);
      removeCommandStub3.Stub (stub => stub.ExpandToAllRelatedObjects ()).Return (expandedCommand3);

      mockRepository.ReplayAll ();

      var clearCommand = (CompositeCommand) PrivateInvoke.InvokeNonPublicMethod (_data, "GetClearCommand");
      var nestedClearCommands = clearCommand.GetNestedCommands ();

      Assert.That (nestedClearCommands.Count, Is.EqualTo (4));
      
      Assert.That (nestedClearCommands, List.Contains (expandedCommand1));
      Assert.That (nestedClearCommands, List.Contains (expandedCommand2));
      Assert.That (nestedClearCommands, List.Contains (expandedCommand3));
      
      Assert.That (nestedClearCommands[3], Is.InstanceOfType (typeof (RelationEndPointTouchCommand)));
      Assert.That (((RelationEndPointTouchCommand) nestedClearCommands[3]).EndPoint, Is.SameAs (_collectionEndPointMock));

      mockRepository.VerifyAll ();
    }

    [Test]
    public void Clear ()
    {
      PrepareActualDataContents (_orderItem1, _orderItem2, _orderItem3);

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

      _data.Clear();

      mockRepository.VerifyAll();
    }

    [Test]
    public void Clear_WithoutItems ()
    {
      PrepareActualDataContents();

      _collectionEndPointMock.Expect (mock => mock.Touch());
      _collectionEndPointMock.Replay();

      _data.Clear();

      _collectionEndPointMock.VerifyAllExpectations();
    }

    [Test]
    public void Insert ()
    {
      _collectionEndPointMock.Expect (mock => mock.CreateInsertCommand (_orderItem1, 17)).Return (_commandStub);
      _collectionEndPointMock.Expect (mock => mock.Touch());
      _collectionEndPointMock.Replay();
      _commandStub.Stub (stub => stub.ExpandToAllRelatedObjects()).Return (_expandedCommandFake);

      _data.Insert (17, _orderItem1);

      _collectionEndPointMock.VerifyAllExpectations();
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
      PrepareActualDataContents (_orderItem1);

      _collectionEndPointMock.Expect (mock => mock.CreateRemoveCommand (_orderItem1)).Return (_commandStub);
      _collectionEndPointMock.Expect (mock => mock.Touch());
      _collectionEndPointMock.Replay();
      _commandStub.Stub (stub => stub.ExpandToAllRelatedObjects()).Return (_expandedCommandFake);

      var result = _data.Remove (_orderItem1);

      _collectionEndPointMock.VerifyAllExpectations();
      DataManagementCommandTestHelper.AssertNotifyAndPerformWasCalled (_nestedCommandMock);

      Assert.That (result, Is.True);
    }

    [Test]
    public void Remove_ObjectNotContained ()
    {
      bool result = _data.Remove (_orderItem1);

      _collectionEndPointMock.AssertWasNotCalled (mock => mock.CreateRemoveCommand (Arg<DomainObject>.Is.Anything));
      _collectionEndPointMock.AssertWasCalled (mock => mock.Touch());

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
      PrepareActualDataContents (_orderItem1);

      _collectionEndPointMock.Expect (mock => mock.CreateRemoveCommand (_orderItem1)).Return (_commandStub);
      _collectionEndPointMock.Expect (mock => mock.Touch());
      _collectionEndPointMock.Replay();
      _commandStub.Stub (stub => stub.ExpandToAllRelatedObjects()).Return (_expandedCommandFake);

      var result = _data.Remove (_orderItem1.ID);

      _collectionEndPointMock.VerifyAllExpectations();
      DataManagementCommandTestHelper.AssertNotifyAndPerformWasCalled (_nestedCommandMock);

      Assert.That (result, Is.True);
    }

    [Test]
    public void Remove_ID_ObjectNotContained ()
    {
      var result = _data.Remove (_orderItem1.ID);

      _collectionEndPointMock.AssertWasNotCalled (mock => mock.CreateRemoveCommand (Arg<DomainObject>.Is.Anything));
      _collectionEndPointMock.AssertWasCalled (mock => mock.Touch());

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
      _collectionEndPointMock.Expect (mock => mock.Touch());
      _collectionEndPointMock.Replay();
      _commandStub.Stub (stub => stub.ExpandToAllRelatedObjects()).Return (_expandedCommandFake);

      _data.Replace (17, _orderItem1);

      _collectionEndPointMock.VerifyAllExpectations();
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
    }

    private void CheckClientTransactionDiffersException (Action<EndPointDelegatingCollectionData, DomainObject> action)
    {
      var orderItemInOtherTransaction = DomainObjectMother.CreateObjectInTransaction<OrderItem> (ClientTransaction.CreateRootTransaction());
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
      using (_data.AssociatedEndPoint.ClientTransaction.EnterNonDiscardingScope())
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
      using (_data.AssociatedEndPoint.ClientTransaction.EnterNonDiscardingScope())
      {
        deletedOwningObject = Order.GetObject (DomainObjectIDs.Order4);
      }

      var endPointStub = CreateCollectionEndPointStub (ClientTransactionMock, deletedOwningObject);
      var data = new EndPointDelegatingCollectionData (endPointStub, _endPointDataStub);

      using (_data.AssociatedEndPoint.ClientTransaction.EnterNonDiscardingScope())
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

    private void PrepareActualDataContents (params DomainObject[] contents)
    {
      _dataStoreStub.Stub (stub => stub.Count).Return (contents.Length);
      _dataStoreStub.Stub (stub => stub.GetEnumerator()).Return (((IEnumerable<DomainObject>) contents).GetEnumerator());

      for (int i = 0; i < contents.Length; i++)
      {
        int currentIndex = i; // required because Stub creates a closure
        _dataStoreStub.Stub (stub => stub.ContainsObjectID (contents[currentIndex].ID)).Return (true);
        _dataStoreStub.Stub (stub => stub.GetObject (contents[currentIndex].ID)).Return (contents[currentIndex]);
        _dataStoreStub.Stub (stub => stub.GetObject (currentIndex)).Return (contents[currentIndex]);
        _dataStoreStub.Stub (stub => stub.IndexOf (contents[currentIndex].ID)).Return (currentIndex);
      }
    }
  }
}