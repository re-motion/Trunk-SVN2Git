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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class DataContainerMapTest : ClientTransactionBaseTest
  {
    private DataContainerMap _map;
    private DataContainer _newOrder;
    private DataContainer _existingOrder;

    public override void SetUp ()
    {
      base.SetUp();

      _map = new DataContainerMap (ClientTransactionMock);
      _newOrder = CreateNewOrderDataContainer();
      _existingOrder = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;
    }

    [Test]
    public void DeleteNewDataContainer ()
    {
      _map.Register (_newOrder);
      Assert.That (_map.Count, Is.EqualTo (1));

      _map.PerformDelete2 (_newOrder);
      Assert.That (_map.Count, Is.EqualTo (0));
    }

    [Test]
    public void RemoveDeletedDataContainerInCommit ()
    {
      _map.Register (_existingOrder);
      Assert.That (_map.Count, Is.EqualTo (1));

      Order order = (Order) _existingOrder.DomainObject;
      order.Delete();
      _map.Commit2();

      Assert.That (_map.Count, Is.EqualTo (0));
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException))]
    public void AccessDeletedDataContainerAfterCommit ()
    {
      _map.Register (_existingOrder);
      Assert.That (_map.Count, Is.EqualTo (1));

      Order order = (Order) _existingOrder.DomainObject;
      order.Delete();
      _map.Commit2();

      Dev.Null = _existingOrder.Timestamp;
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException))]
    public void GetByInvalidState ()
    {
      _map.GetByState2 ((StateType) 1000);
    }

    [Test]
    public void RollbackForDeletedObject ()
    {
      using (ClientTransactionMock.EnterDiscardingScope())
      {
        _map.Register (_existingOrder);

        Order order = (Order) _existingOrder.DomainObject;
        order.Delete();
        Assert.That (_existingOrder.State, Is.EqualTo (StateType.Deleted));

        _map.Rollback2();

        _existingOrder = _map[_existingOrder.ID];
        Assert.That (_existingOrder, Is.Not.Null);
        Assert.That (_existingOrder.State, Is.EqualTo (StateType.Unchanged));
      }
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException))]
    public void RollbackForNewObject ()
    {
      _map.Register (_newOrder);

      _map.Rollback2();

      Dev.Null = _newOrder.Timestamp;
    }

    [Test]
    public void Rollback_SingleDeletedObject ()
    {
      using (ClientTransactionMock.EnterDiscardingScope())
      {
        _map.Register (_existingOrder);

        Order order = (Order) _existingOrder.DomainObject;
        order.Delete();
        Assert.That (_existingOrder.State, Is.EqualTo (StateType.Deleted));

        _map.Rollback2 (_existingOrder);

        _existingOrder = _map[_existingOrder.ID];
        Assert.That (_existingOrder, Is.Not.Null);
        Assert.That (_existingOrder.State, Is.EqualTo (StateType.Unchanged));
      }
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException))]
    public void Rollback_SingleNewObject ()
    {
      _map.Register (_newOrder);

      _map.Rollback2 (_newOrder);

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

        _map.PerformDelete2 (order1.InternalDataContainer);
      }
    }

    private DataContainer CreateNewOrderDataContainer ()
    {
      Order order = Order.NewObject();
      order.OrderNumber = 10;
      order.DeliveryDate = new DateTime (2006, 1, 1);
      order.Official = Official.GetObject (DomainObjectIDs.Official1);
      order.Customer = Customer.GetObject (DomainObjectIDs.Customer1);

      return order.InternalDataContainer;
    }

    [Test]
    public void GetObjectWithoutLoading_LoadedObject ()
    {
      ClassWithAllDataTypes loadedOrder = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      Assert.That (ClientTransactionMock.DataManager.DataContainerMap.GetObjectWithoutLoading (DomainObjectIDs.ClassWithAllDataTypes1, false), Is.SameAs (
                  loadedOrder));
    }

    [Test]
    public void GetObjectWithoutLoading_NonLoadedObject ()
    {
      Assert.That (ClientTransactionMock.DataManager.DataContainerMap.GetObjectWithoutLoading (DomainObjectIDs.ClassWithAllDataTypes1, false), Is.Null);
    }

    [Test]
    public void GetObjectWithoutLoading_IncludeDeletedTrue ()
    {
      Order deletedOrder = Order.GetObject (DomainObjectIDs.Order1);
      deletedOrder.Delete();
      Assert.That (ClientTransactionMock.DataManager.DataContainerMap.GetObjectWithoutLoading (DomainObjectIDs.Order1, true), Is.SameAs (deletedOrder));
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void GetObjectWithoutLoading_IncludeDeletedFalse ()
    {
      Order deletedOrder = Order.GetObject (DomainObjectIDs.Order1);
      deletedOrder.Delete();
      ClientTransactionMock.DataManager.DataContainerMap.GetObjectWithoutLoading (DomainObjectIDs.Order1, false);
    }

    [Test]
    public void Commit_CommitsDataContainers ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      _map.Register (dataContainer);
      Assert.That (dataContainer.State, Is.EqualTo (StateType.New));

      _map.Commit();

      Assert.That (dataContainer.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void Rollback_RollsbackDataContainers ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      dataContainer.Delete();
      _map.Register (dataContainer);
      Assert.That (dataContainer.State, Is.EqualTo (StateType.Deleted));

      _map.Rollback();

      Assert.That (dataContainer.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void Discard_DiscardsDataContainer ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      _map.Register (dataContainer);
      Assert.That (dataContainer.IsDiscarded, Is.False);

      _map.Discard (dataContainer.ID);

      Assert.That (dataContainer.IsDiscarded, Is.True);
    }

    [Test]
    public void Discard_RemovesDataContainer ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      _map.Register (dataContainer);
      Assert.That (_map[DomainObjectIDs.Order1], Is.Not.Null);

      _map.Discard (dataContainer.ID);

      Assert.That (_map[DomainObjectIDs.Order1], Is.Null);
    }

    [Test]
    public void Discard_RaisesNotification_BeforeRemoving ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      _map.Register (dataContainer);

      Assert.That (_map[dataContainer.ID], Is.Not.Null);

      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      listenerMock.Expect (mock => mock.DataContainerMapUnregistering (dataContainer))
          .WhenCalled (mi => Assert.That (_map[dataContainer.ID], Is.Not.Null));
      ClientTransactionMock.AddListener (listenerMock);

      listenerMock.Replay ();

      _map.Discard (dataContainer.ID);

      listenerMock.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "Data container 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' is not part of this map.\r\nParameter name: id")]
    public void Discard_NonExistingDataContainer ()
    {
      _map.Discard (DomainObjectIDs.Order1);
    }
  }
}