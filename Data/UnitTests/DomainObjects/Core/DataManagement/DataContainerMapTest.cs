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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class DataContainerMapTest : ClientTransactionBaseTest
  {
    // types

    // static members and constants

    // member fields

    private DataContainerMap _map;
    private DataContainer _newOrder;
    private DataContainer _existingOrder;

    // construction and disposing

    public DataContainerMapTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _map = new DataContainerMap (ClientTransactionMock);
      _newOrder = CreateNewOrderDataContainer ();
      _existingOrder = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;
    }

    [Test]
    public void DeleteNewDataContainer ()
    {
      _map.Register (_newOrder);
      Assert.AreEqual (1, _map.Count);

      _map.PerformDelete (_newOrder);
      Assert.AreEqual (0, _map.Count);
    }

    [Test]
    public void RemoveDeletedDataContainerInCommit ()
    {
      _map.Register (_existingOrder);
      Assert.AreEqual (1, _map.Count);

      Order order = (Order) _existingOrder.DomainObject;
      order.Delete();
      _map.Commit();

      Assert.AreEqual (0, _map.Count);
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException))]
    public void AccessDeletedDataContainerAfterCommit ()
    {
      _map.Register (_existingOrder);
      Assert.AreEqual (1, _map.Count);

      Order order = (Order) _existingOrder.DomainObject;
      order.Delete ();
      _map.Commit ();

      Dev.Null = _existingOrder.Timestamp;
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException))]
    public void GetByInvalidState ()
    {
      _map.GetByState ((StateType) 1000);
    }

    [Test]
    public void RollbackForDeletedObject ()
    {
      using (ClientTransactionMock.EnterDiscardingScope ())
      {
        _map.Register (_existingOrder);

        Order order = (Order) _existingOrder.DomainObject;
        order.Delete();
        Assert.AreEqual (StateType.Deleted, _existingOrder.State);

        _map.Rollback();

        _existingOrder = _map[_existingOrder.ID];
        Assert.IsNotNull (_existingOrder);
        Assert.AreEqual (StateType.Unchanged, _existingOrder.State);
      }
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException))]
    public void RollbackForNewObject ()
    {
      _map.Register (_newOrder);

      _map.Rollback ();

      Dev.Null = _newOrder.Timestamp;
    }

    [Test]
    public void Rollback_SingleDeletedObject ()
    {
      using (ClientTransactionMock.EnterDiscardingScope ())
      {
        _map.Register (_existingOrder);

        Order order = (Order) _existingOrder.DomainObject;
        order.Delete ();
        Assert.AreEqual (StateType.Deleted, _existingOrder.State);

        _map.Rollback (_existingOrder);

        _existingOrder = _map[_existingOrder.ID];
        Assert.IsNotNull (_existingOrder);
        Assert.AreEqual (StateType.Unchanged, _existingOrder.State);
      }
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException))]
    public void Rollback_SingleNewObject ()
    {
      _map.Register (_newOrder);

      _map.Rollback (_newOrder);

      Dev.Null = _newOrder.Timestamp;
    }
    
    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException),
        ExpectedMessage = "Cannot remove DataContainer '.*' from DataContainerMap, because it belongs to a different ClientTransaction.",
        MatchType = MessageMatch.Regex)]
    public void PerformDeleteWithOtherClientTransaction ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        Order order1 = Order.GetObject (DomainObjectIDs.Order1);

        _map.PerformDelete (order1.InternalDataContainer);
      }
    }

    private DataContainer CreateNewOrderDataContainer ()
    {
      Order order = Order.NewObject ();
      order.OrderNumber = 10;
      order.DeliveryDate = new DateTime (2006, 1, 1);
      order.Official = Official.GetObject (DomainObjectIDs.Official1);
      order.Customer = Customer.GetObject (DomainObjectIDs.Customer1);

      return order.InternalDataContainer;
    }

    [Test]
    public void CopyFromEmpty ()
    {
      ClientTransactionMock sourceTransaction = new ClientTransactionMock ();
      ClientTransactionMock destinationTransaction = new ClientTransactionMock ();

      DataContainerMap sourceMap = sourceTransaction.DataManager.DataContainerMap;
      DataContainerMap destinationMap = destinationTransaction.DataManager.DataContainerMap;

      destinationMap.CopyFrom (sourceMap);

      Assert.AreEqual (0, destinationMap.Count);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Source cannot be the destination DataContainerMap instance.",
        MatchType = MessageMatch.Contains)]
    public void CannotCopyFromSelf ()
    {
      _map.CopyFrom (_map);
    }

    [Test]
    public void CopyFromNonEmpty ()
    {
      ClientTransactionMock sourceTransaction = new ClientTransactionMock ();
      ClientTransactionMock destinationTransaction = new ClientTransactionMock ();

      DataContainerMap sourceMap = sourceTransaction.DataManager.DataContainerMap;
      DataContainerMap destinationMap = destinationTransaction.DataManager.DataContainerMap;

      Order newOrder;

      using (sourceTransaction.EnterNonDiscardingScope ())
      {
        newOrder = Order.NewObject ();
      }

      Assert.AreNotEqual (0, sourceMap.Count);
      Assert.IsNotNull (sourceMap[newOrder.ID]);

      Assert.AreEqual (0, destinationMap.Count);
      Assert.IsNull (destinationMap[newOrder.ID]);

      destinationMap.CopyFrom (sourceMap);

      Assert.AreNotEqual (0, destinationMap.Count);
      Assert.AreEqual (sourceMap.Count, destinationMap.Count);
      Assert.IsNotNull (destinationMap[newOrder.ID]);
      
      Assert.AreNotSame (sourceMap[newOrder.ID], destinationMap[newOrder.ID]);
      Assert.AreSame (destinationTransaction, destinationMap[newOrder.ID].ClientTransaction);
    }

    [Test]
    public void GetObjectWithoutLoading_LoadedObject ()
    {
      ClassWithAllDataTypes loadedOrder = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      Assert.AreSame (loadedOrder, ClientTransactionMock.DataManager.DataContainerMap.GetObjectWithoutLoading (DomainObjectIDs.ClassWithAllDataTypes1, false));
    }

    [Test]
    public void GetObjectWithoutLoading_NonLoadedObject ()
    {
      Assert.IsNull (ClientTransactionMock.DataManager.DataContainerMap.GetObjectWithoutLoading (DomainObjectIDs.ClassWithAllDataTypes1, false));
    }

    [Test]
    public void GetObjectWithoutLoading_IncludeDeletedTrue ()
    {
      Order deletedOrder = Order.GetObject (DomainObjectIDs.Order1);
      deletedOrder.Delete ();
      Assert.AreSame (deletedOrder, ClientTransactionMock.DataManager.DataContainerMap.GetObjectWithoutLoading (DomainObjectIDs.Order1, true));
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void GetObjectWithoutLoading_IncludeDeletedFalse ()
    {
      Order deletedOrder = Order.GetObject (DomainObjectIDs.Order1);
      deletedOrder.Delete ();
      ClientTransactionMock.DataManager.DataContainerMap.GetObjectWithoutLoading (DomainObjectIDs.Order1, false);
    }
  }
}
