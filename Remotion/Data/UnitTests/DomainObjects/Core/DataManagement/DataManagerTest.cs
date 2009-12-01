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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  /// <summary>
  /// 
  /// </summary>
  [TestFixture]
  public class DataManagerTest : ClientTransactionBaseTest
  {
    private DataManager _dataManager;

    public override void SetUp ()
    {
      base.SetUp ();

      _dataManager = ClientTransactionMock.DataManager;
    }

    [Test]
    public void GetEmptyDomainObjectsFromStateTypeOverload ()
    {
      DomainObjectCollection domainObjects = _dataManager.GetDomainObjects (StateType.Unchanged);
      Assert.IsNotNull (domainObjects);
      Assert.AreEqual (0, domainObjects.Count);
    }

    [Test]
    public void GetUnchangedDomainObjectsFromStateTypeOverload ()
    {
      DataContainer container = CreateOrder1DataContainer ();

      DomainObjectCollection domainObjects = _dataManager.GetDomainObjects (StateType.Unchanged);
      Assert.IsNotNull (domainObjects);
      Assert.AreEqual (1, domainObjects.Count);
      Assert.AreSame (container.DomainObject, domainObjects[0]);
    }

    [Test]
    public void GetUnchangedDomainObjects ()
    {
      DataContainer container = CreateOrder1DataContainer ();

      DomainObjectCollection domainObjects = _dataManager.GetDomainObjects (new StateType[] { StateType.Unchanged });
      Assert.IsNotNull (domainObjects);
      Assert.AreEqual (1, domainObjects.Count);
      Assert.AreSame (container.DomainObject, domainObjects[0]);
    }

    [Test]
    public void GetChangedAndUnchangedDomainObjects ()
    {
      DataContainer container1 = CreateOrder1DataContainer ();
      DataContainer container2 = CreateOrder2DataContainer();

      container1.SetValue (
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber",
          (int) container1.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber") + 1);

      DomainObjectCollection domainObjects = _dataManager.GetDomainObjects (new StateType[] { StateType.Changed });
      Assert.IsNotNull (domainObjects);
      Assert.AreEqual (1, domainObjects.Count);
      Assert.AreSame (container1.DomainObject, domainObjects[0]);

      domainObjects = _dataManager.GetDomainObjects (new StateType[] { StateType.Unchanged });
      Assert.IsNotNull (domainObjects);
      Assert.AreEqual (1, domainObjects.Count);
      Assert.AreSame (container2.DomainObject, domainObjects[0]);    
    }

    [Test]
    public void GetDeletedAndUnchangedDomainObjects ()
    {
      DataContainer container1 = CreateClassWithAllDataTypesDataContainer();
      DataContainer container2 = CreateOrder2DataContainer ();

      _dataManager.Delete (container1.DomainObject);

      DomainObjectCollection domainObjects = _dataManager.GetDomainObjects (new StateType[] { StateType.Deleted });
      Assert.IsNotNull (domainObjects);
      Assert.AreEqual (1, domainObjects.Count);
      Assert.AreSame (container1.DomainObject, domainObjects[0]);

      domainObjects = _dataManager.GetDomainObjects (new StateType[] { StateType.Unchanged });
      Assert.IsNotNull (domainObjects);
      Assert.AreEqual (1, domainObjects.Count);
      Assert.AreSame (container2.DomainObject, domainObjects[0]);
    }

    [Test]
    public void GetNewAndUnchangedDomainObjects ()
    {
			DataContainer container1 = Order.NewObject ().InternalDataContainer;
      DataContainer container2 = CreateOrder2DataContainer ();

      DomainObjectCollection domainObjects = _dataManager.GetDomainObjects (new StateType[] { StateType.New });
      Assert.IsNotNull (domainObjects);
      Assert.AreEqual (1, domainObjects.Count);
      Assert.AreSame (container1.DomainObject, domainObjects[0]);

      domainObjects = _dataManager.GetDomainObjects (new StateType[] { StateType.Unchanged });
      Assert.IsNotNull (domainObjects);
      Assert.AreEqual (1, domainObjects.Count);
      Assert.AreSame (container2.DomainObject, domainObjects[0]);
    }

    [Test]
    public void GetEmptyChangedDomainObjects ()
    {
      Assert.AreEqual (0, _dataManager.GetChangedDomainObjects ().Count);
    }

    [Test]
    public void GetChangedDomainObjects ()
    {
      DataContainer container = CreateOrder1DataContainer ();
      container["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 42;

      DomainObjectCollection changedObjects = _dataManager.GetChangedDomainObjects ();
      Assert.AreEqual (1, changedObjects.Count);
      Assert.AreEqual (container.ID, changedObjects[0].ID);
    }

    [Test]
    public void GetChangedDomainObjectsForMultipleObjects ()
    {
      CreateOrder1DataContainer ();
      DataContainer container2 = CreateOrderTicket1DataContainer ();

      container2["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.FileName"] = @"C:\NewFile.jpg";

      DomainObjectCollection changedObjects = _dataManager.GetChangedDomainObjects ();
      Assert.AreEqual (1, changedObjects.Count);
      Assert.AreEqual (container2.ID, changedObjects[0].ID);
    }

    [Test]
    public void GetChangedDomainObjectsForRelationChange ()
    {
      DataContainer order1 = CreateOrder1DataContainer ();
      CreateOrderTicket1DataContainer ();
      DataContainer orderTicket2 = CreateOrderTicket2DataContainer ();
      CreateOrderWithoutOrderItemDataContainer();

      var order1EndPointID = new RelationEndPointID (order1.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
      ((ObjectEndPoint) _dataManager.RelationEndPointMap[order1EndPointID]).SetOppositeObjectAndNotify (orderTicket2.DomainObject);

      DomainObjectCollection changedObjects = _dataManager.GetChangedDomainObjects ();
      Assert.AreEqual (4, changedObjects.Count);
    }

    [Test]
    public void GetChangedRelationEndPoints ()
    {
      Order order1 = Order.GetObject (DomainObjectIDs.Order1);
      Order order2 = Order.GetObject (DomainObjectIDs.Order2);

      OrderItem orderItem1 = order1.OrderItems[0];
      OrderTicket orderTicket = order1.OrderTicket;

      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      Employee employee = computer.Employee;

      Location location = Location.GetObject (DomainObjectIDs.Location1);
      Client client = location.Client;

      Assert.IsEmpty (new List<RelationEndPoint> (_dataManager.GetChangedRelationEndPoints ()));

      orderItem1.Order = null; // 2 endpoints
      orderTicket.Order = null; // 2 endpoints

      computer.Employee = Employee.NewObject (); // 3 endpoints
      employee.Computer = null; // (1 endpoint)

      location.Client = Client.NewObject (); // 1 endpoint

      List<RelationEndPoint> changedEndPoints = new List<RelationEndPoint> (_dataManager.GetChangedRelationEndPoints ());

      Assert.AreEqual (8, changedEndPoints.Count);

      Assert.Contains (_dataManager.RelationEndPointMap[new RelationEndPointID (order1.ID, typeof (Order) + ".OrderItems")],
          changedEndPoints);
      Assert.Contains (_dataManager.RelationEndPointMap[new RelationEndPointID (orderItem1.ID, typeof (OrderItem) + ".Order")],
          changedEndPoints);
      Assert.Contains (_dataManager.RelationEndPointMap[new RelationEndPointID (orderTicket.ID, typeof (OrderTicket) + ".Order")],
          changedEndPoints);
      Assert.Contains (_dataManager.RelationEndPointMap[new RelationEndPointID (order1.ID, typeof (Order) + ".OrderTicket")],
          changedEndPoints);
      Assert.Contains (_dataManager.RelationEndPointMap[new RelationEndPointID (computer.ID, typeof (Computer) + ".Employee")],
          changedEndPoints);
      Assert.Contains (_dataManager.RelationEndPointMap[new RelationEndPointID (computer.Employee.ID, typeof (Employee) + ".Computer")],
          changedEndPoints);
      Assert.Contains (_dataManager.RelationEndPointMap[new RelationEndPointID (employee.ID, typeof (Employee) + ".Computer")],
          changedEndPoints);
      Assert.Contains (_dataManager.RelationEndPointMap[new RelationEndPointID (location.ID, typeof (Location) + ".Client")],
          changedEndPoints);
    }

    [Test]
    public void Commit ()
    {
      DataContainer container = CreateOrder1DataContainer();
      container["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 42;

      _dataManager.Commit ();

      Assert.AreEqual (0, _dataManager.GetChangedDomainObjects ().Count);
      Assert.AreEqual (42, container.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].OriginalValue);
      Assert.AreEqual (42, container["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"]);
      Assert.AreEqual (StateType.Unchanged, container.State);
    }

    [Test]
    public void CommitOneToOneRelationChange ()
    {
      DataContainer order1 = CreateOrder1DataContainer ();
      DataContainer orderTicket1 = CreateOrderTicket1DataContainer();
      DataContainer orderTicket2 = CreateOrderTicket2DataContainer();
      DataContainer orderWithoutOrderItemDataContainer = CreateOrderWithoutOrderItemDataContainer ();

      RelationEndPointID order1EndPointID = new RelationEndPointID (order1.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
      ((ObjectEndPoint) _dataManager.RelationEndPointMap[order1EndPointID]).SetOppositeObjectAndNotify (orderTicket2.DomainObject);

      _dataManager.Commit ();

      Assert.AreEqual (0, _dataManager.GetChangedDomainObjects ().Count);
      Assert.AreSame (orderTicket2.DomainObject, ClientTransactionMock.GetRelatedObject (order1EndPointID));
      Assert.AreSame (order1.DomainObject, ClientTransactionMock.GetRelatedObject (new RelationEndPointID (orderTicket2.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order")));
      Assert.IsNull (ClientTransactionMock.GetRelatedObject (new RelationEndPointID (orderTicket1.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order")));
      Assert.IsNull (ClientTransactionMock.GetRelatedObject (new RelationEndPointID (orderWithoutOrderItemDataContainer.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket")));
      Assert.IsFalse (_dataManager.RelationEndPointMap.HasRelationChanged (order1));
      Assert.IsFalse (_dataManager.RelationEndPointMap.HasRelationChanged (orderWithoutOrderItemDataContainer));
      Assert.IsFalse (_dataManager.RelationEndPointMap.HasRelationChanged (orderTicket1));
      Assert.IsFalse (_dataManager.RelationEndPointMap.HasRelationChanged (orderTicket2));
    }

    [Test]
    public void CommitOneToManyRelationChange ()
    {
      Customer customer1 = Customer.GetObject (DomainObjectIDs.Customer1);
      Order order1 = Order.GetObject (DomainObjectIDs.Order1);
      customer1.Orders.Clear ();

      _dataManager.Commit ();

      Assert.AreEqual (0, _dataManager.GetChangedDomainObjects ().Count);
      Assert.IsNull (order1.Customer);
      Assert.AreEqual (0, ClientTransactionMock.GetOriginalRelatedObjects (new RelationEndPointID (customer1.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders")).Count);
      Assert.AreEqual (0, customer1.Orders.Count);
    }

    [Test]
    public void RollbackDataContainerMap ()
    {
      Computer computer = Computer.NewObject ();
      ObjectID id = computer.ID;

			Assert.AreSame (computer.InternalDataContainer, _dataManager.DataContainerMap[id]);

      _dataManager.Rollback ();

      Assert.IsNull (_dataManager.DataContainerMap[id]);
    }

    [Test]
    public void RollbackObjectEndPoint ()
    {
      Computer computer = Computer.NewObject ();
      Employee employee = Employee.NewObject ();

      computer.Employee = employee;

      RelationEndPointID employeeEndPointID = new RelationEndPointID (employee.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Computer");
      RelationEndPointID computerEndPointID = new RelationEndPointID (computer.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.Employee");

      ObjectEndPoint employeeEndPoint = (ObjectEndPoint) _dataManager.RelationEndPointMap[employeeEndPointID];
      ObjectEndPoint computerEndPoint = (ObjectEndPoint) _dataManager.RelationEndPointMap[computerEndPointID];

      Assert.AreSame (computer.ID, employeeEndPoint.OppositeObjectID);
      Assert.AreSame (employee.ID, computerEndPoint.OppositeObjectID);

      _dataManager.Rollback ();

      Assert.IsNull (_dataManager.RelationEndPointMap[employeeEndPointID]);
      Assert.IsNull (_dataManager.RelationEndPointMap[computerEndPointID]);
    }

    [Test]
    public void RollbackCollectionEndPoint ()
    {
      Order order = Order.NewObject ();
      OrderItem orderItem = OrderItem.NewObject ();

      orderItem.Order = order;

      RelationEndPointID orderEndPointID = new RelationEndPointID (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");
      RelationEndPointID orderItemEndPointID = new RelationEndPointID (orderItem.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");

      ObjectEndPoint orderItemEndPoint = (ObjectEndPoint) _dataManager.RelationEndPointMap[orderItemEndPointID];
      Assert.AreEqual (order.ID, orderItemEndPoint.OppositeObjectID);

      CollectionEndPoint orderEndPoint = (CollectionEndPoint) _dataManager.RelationEndPointMap[orderEndPointID];

      Assert.AreEqual (1, orderEndPoint.OppositeDomainObjects.Count);
      Assert.IsNotNull (orderEndPoint.OppositeDomainObjects[orderItem.ID]);

      _dataManager.Rollback ();

      orderItemEndPoint = (ObjectEndPoint) _dataManager.RelationEndPointMap[orderItemEndPointID];
      orderEndPoint = (CollectionEndPoint) _dataManager.RelationEndPointMap[orderEndPointID];

      Assert.IsNull (orderEndPoint);
      Assert.IsNull (orderItemEndPoint);
    }

    [Test]
    public void GetChangedDataContainersForCommitWithDeletedObject ()
    {
      OrderItem orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      orderItem1.Delete ();

      _dataManager.GetChangedDataContainersForCommit ();

      // expectation: no exception
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException),
       ExpectedMessage = "Cannot delete DomainObject '.*', because it belongs to a different ClientTransaction.",
       MatchType = MessageMatch.Regex)]
    public void DeleteWithOtherClientTransaction ()
    {
      Order order1;
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        order1 = Order.GetObject (DomainObjectIDs.Order1);
      }
      _dataManager.Delete (order1);
    }

    [Test]
    public void DeleteWithOtherClientTransaction_UsesStoredTransaction ()
    {
      Order order1 = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        _dataManager.Delete (order1); // deletes in _dataManager's transaction, not in current transaction
      }
      Assert.AreEqual (StateType.Deleted, Order.GetObject (DomainObjectIDs.Order1, true).State);
    }

    [Test]
    public void IsDiscarded ()
    {
      OrderItem orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      Assert.IsFalse (_dataManager.IsDiscarded (orderItem1.ID));
      Assert.AreEqual (0, _dataManager.DiscardedObjectCount);
      _dataManager.MarkDiscarded (orderItem1.InternalDataContainer);
      Assert.IsTrue (_dataManager.IsDiscarded (orderItem1.ID));
      Assert.AreEqual (1, _dataManager.DiscardedObjectCount);
    }

    [Test]
    public void GetDiscardedDataContainer ()
    {
      OrderItem orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      DataContainer dataContainer = orderItem1.InternalDataContainer;
      _dataManager.MarkDiscarded (dataContainer);
      Assert.AreSame (dataContainer, _dataManager.GetDiscardedDataContainer (orderItem1.ID));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' has "
        + "not been discarded.\r\nParameter name: id")]
    public void GetDiscardedDataContainerThrowsWhenNotDiscarded ()
    {
      _dataManager.GetDiscardedDataContainer (DomainObjectIDs.Order1);
    }

    [Test]
    public void DeleteLoadedDiscardsOnCommit ()
    {
      using (ClientTransactionMock.EnterDiscardingScope ())
      {
        SetDatabaseModifyable();

        OrderItem orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
        orderItem1.Delete();

        Assert.IsFalse (_dataManager.IsDiscarded (orderItem1.ID));

        ClientTransactionMock.Commit();

        Assert.IsTrue (_dataManager.IsDiscarded (orderItem1.ID));
      }
    }

    [Test]
    public void DeleteNewDiscardsImmediatelyOnCommit ()
    {
      using (ClientTransactionMock.EnterDiscardingScope ())
      {
        OrderItem orderItem1 = OrderItem.NewObject();

        Assert.IsFalse (_dataManager.IsDiscarded (orderItem1.ID));

        orderItem1.Delete();

        Assert.IsTrue (_dataManager.IsDiscarded (orderItem1.ID));
      }
    }

    private DataContainer CreateOrder1DataContainer ()
    {
      var dataContainer = TestDataContainerFactory.CreateOrder1DataContainer ();
      dataContainer.SetDomainObject (ClientTransactionMock.GetObjectForDataContainer (dataContainer));
      _dataManager.RegisterExistingDataContainer (dataContainer);
      return dataContainer;
    }

    private DataContainer CreateOrderTicket1DataContainer ()
    {
      var dataContainer = TestDataContainerFactory.CreateOrderTicket1DataContainer ();
      dataContainer.SetDomainObject (ClientTransactionMock.GetObjectForDataContainer (dataContainer));
      _dataManager.RegisterExistingDataContainer (dataContainer);
      return dataContainer;
    }

    private DataContainer CreateOrderTicket2DataContainer ()
    {
      var dataContainer = TestDataContainerFactory.CreateOrderTicket2DataContainer ();
      dataContainer.SetDomainObject (ClientTransactionMock.GetObjectForDataContainer (dataContainer));
      _dataManager.RegisterExistingDataContainer (dataContainer);
      return dataContainer;
    }

    private DataContainer CreateOrder2DataContainer ()
    {
      var dataContainer = TestDataContainerFactory.CreateOrder2DataContainer ();
      dataContainer.SetDomainObject (ClientTransactionMock.GetObjectForDataContainer (dataContainer));
      _dataManager.RegisterExistingDataContainer (dataContainer);
      return dataContainer;
    }

    private DataContainer CreateOrderWithoutOrderItemDataContainer ()
    {
      var dataContainer = TestDataContainerFactory.CreateOrderWithoutOrderItemDataContainer ();
      dataContainer.SetDomainObject (ClientTransactionMock.GetObjectForDataContainer (dataContainer));
      _dataManager.RegisterExistingDataContainer (dataContainer);
      return dataContainer;
    }

    private DataContainer CreateClassWithAllDataTypesDataContainer ()
    {
      var dataContainer = TestDataContainerFactory.CreateClassWithAllDataTypesDataContainer ();
      dataContainer.SetDomainObject (ClientTransactionMock.GetObjectForDataContainer (dataContainer));
      _dataManager.RegisterExistingDataContainer (dataContainer);
      return dataContainer;
    }
  }
}
