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
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class RelationEndPointMapTest : ClientTransactionBaseTest
  {
    private RelationEndPointMap _map;

    public override void SetUp ()
    {
      base.SetUp();

      _map = ClientTransactionMock.DataManager.RelationEndPointMap;
    }

    [Test]
    public void CommitForDeletedObject ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      Assert.That (_map.Count > 0, Is.True);

      computer.Delete();

      var deletedDomainObjects = new DomainObjectCollection();
      deletedDomainObjects.Add (computer);

      _map.Commit2 (deletedDomainObjects);

      Assert.That (_map.Count, Is.EqualTo (0));
    }

    [Test]
    public void DeleteNew ()
    {
      Order newOrder = Order.NewObject();
      Assert.That (_map.Count > 0, Is.True);

      _map.PerformDelete2 (newOrder, _map.GetRemoveModificationsForOppositeEndPoints (newOrder));
      Assert.That (_map.Count, Is.EqualTo (0));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "GetOriginalRelatedObjects can only be called for end points with a cardinality of 'Many'.\r\nParameter name: endPointID")]
    public void GetOriginalRelatedObjectsWithEndPointIDOfWrongCardinality ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      _map.GetOriginalRelatedObjects (new RelationEndPointID (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"));
    }

    [Test]
    public void GetOriginalRelatedObjectsWithLazyLoad ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      var endPointID = new RelationEndPointID (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");
      DomainObjectCollection originalOrderItems = _map.GetOriginalRelatedObjects (endPointID);
      DomainObjectCollection orderItems = _map.GetRelatedObjects (endPointID);

      Assert.That (ReferenceEquals (originalOrderItems, orderItems), Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "GetOriginalRelatedObject can only be called for end points with a cardinality of 'One'.\r\nParameter name: endPointID")]
    public void GetOriginalRelatedObjectWithEndPointIDOfWrongCardinality ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      _map.GetOriginalRelatedObject (new RelationEndPointID (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"));
    }

    [Test]
    public void GetOriginalRelatedObjectWithLazyLoad ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      var endPointID = new RelationEndPointID (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
      DomainObject originalOrderTicket = _map.GetOriginalRelatedObject (endPointID);
      DomainObject orderTicket = _map.GetRelatedObject (endPointID, false);

      Assert.That (ReferenceEquals (originalOrderTicket, orderTicket), Is.True);
    }

    [Test]
    public void GetRelatedObject ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      var endPointID = new RelationEndPointID (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
      DomainObject orderTicket = _map.GetRelatedObject (endPointID, false);

      Assert.That (orderTicket, Is.Not.Null);
      Assert.That (orderTicket.ID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
      Assert.That (orderTicket, Is.SameAs (OrderTicket.GetObject (DomainObjectIDs.OrderTicket1)));
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void GetRelatedObjectIncludeDeletedFalse ()
    {
      Location location = Location.GetObject (DomainObjectIDs.Location1);

      location.Client.Delete();

      var endPointID = new RelationEndPointID (location.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client");
      _map.GetRelatedObject (endPointID, false);
    }

    [Test]
    public void GetRelatedObjectIncludeDeletedTrue ()
    {
      Location location = Location.GetObject (DomainObjectIDs.Location1);

      location.Client.Delete();

      var endPointID = new RelationEndPointID (location.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client");
      DomainObject client = _map.GetRelatedObject (endPointID, true);
      Assert.That (client, Is.Not.Null);
      Assert.That (client.ID, Is.EqualTo (DomainObjectIDs.Client1));
      Assert.That (client.State, Is.EqualTo (StateType.Deleted));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "GetRelatedObjects can only be called for end points with a cardinality of 'Many'.\r\nParameter name: endPointID")]
    public void GetRelatedObjectsWithEndPointIDOfWrongCardinality ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      _map.GetRelatedObjects (new RelationEndPointID (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"));
    }

    [Test]
    public void GetRelatedObjectWithDiscarded ()
    {
      Location location = Location.GetObject (DomainObjectIDs.Location1);
      Client newClient = Client.NewObject();
      location.Client = newClient;
      location.Client.Delete();

      var endPointID = new RelationEndPointID (location.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client");
      DomainObject client = _map.GetRelatedObject (endPointID, true);
      Assert.That (client, Is.Not.Null);
      Assert.That (client, Is.SameAs (newClient));
      Assert.That (client.IsDiscarded, Is.True);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "GetRelatedObject can only be called for end points with a cardinality of 'One'.\r\nParameter name: endPointID")]
    public void GetRelatedObjectWithEndPointIDOfWrongCardinality ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      _map.GetRelatedObject (new RelationEndPointID (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"), false);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot get a RelationEndPoint for an anonymous end point definition. "
                                                                              +
                                                                              "There are no end points for the non-existing side of unidirectional relations."
        )]
    public void GetRelationEndPointWithLazyLoad_DoesNotSupportAnonymousEndPoints ()
    {
      var client = Client.GetObject (DomainObjectIDs.Client2);
      var parentClientEndPointDefinition = client.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (Client) + ".ParentClient");
      IEndPoint unidirectionalEndPoint = _map.GetRelationEndPointWithLazyLoad (new RelationEndPointID (client.ID, parentClientEndPointDefinition));

      Client parentClient = client.ParentClient;
      Assert.That (parentClient, Is.Not.Null);

      var anonymousEndPointDefinition = unidirectionalEndPoint.Definition.GetOppositeEndPointDefinition();
      _map.GetRelationEndPointWithLazyLoad (new RelationEndPointID (parentClient.ID, anonymousEndPointDefinition));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot lazily load the real part of a relation. RegisterExistingDataContainer or RegisterNewDataContainer must be called before any "
        + "non-virtual end points are retrieved.")]
    public void GetRelationEndPointWithLazyLoad_DoesNotSupportRealEndPoints_OfObjectsNotYetRegistered ()
    {
      IRelationEndPointDefinition locationEndPointDefinition =
          DomainObjectIDs.Location1.ClassDefinition.GetRelationEndPointDefinition (typeof (Location) + ".Client");
      var locationEndPointID = new RelationEndPointID (DomainObjectIDs.Location1, locationEndPointDefinition);
      _map.GetRelationEndPointWithLazyLoad (locationEndPointID);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException),
        ExpectedMessage = "Cannot remove DomainObject '.*' from RelationEndPointMap, because it belongs to a different ClientTransaction.",
        MatchType = MessageMatch.Regex)]
    public void PerformDeletionWithOtherClientTransaction ()
    {
      Order order1;
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        order1 = Order.GetObject (DomainObjectIDs.Order1);
      }

      _map.PerformDelete2 (order1, new CompositeRelationModificationWithEvents());
    }

    [Test]
    public void RegisterVirtualObjectEndPoint_CreatesVirtualObjectEndPoint ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");

      var objectEndPoint = _map.RegisterVirtualObjectEndPoint (id, DomainObjectIDs.OrderTicket1);

      Assert.That (objectEndPoint.ID, Is.EqualTo (id));
      Assert.That (objectEndPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
      Assert.That (objectEndPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
    }

    [Test]
    public void RegisterVirtualObjectEndPoint_RegistersEndPoint ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");

      var objectEndPoint = _map.RegisterVirtualObjectEndPoint (id, DomainObjectIDs.OrderTicket1);

      Assert.That (_map[id], Is.SameAs (objectEndPoint));
    }

    [Test]
    public void RegisterRealObjectEndPoint_CreatesRealObjectEndPoint ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var foreignKeyProperty = new PropertyValue (id.Definition.ClassDefinition.GetMandatoryPropertyDefinition (id.Definition.PropertyName));

      var objectEndPoint = _map.RegisterRealObjectEndPoint (id, foreignKeyProperty);

      Assert.That (objectEndPoint.ForeignKeyProperty, Is.SameAs (foreignKeyProperty));
    }

    [Test]
    public void RegisterRealObjectEndPoint_RegistersEndPoint ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var foreignKeyProperty = new PropertyValue (id.Definition.ClassDefinition.GetMandatoryPropertyDefinition (id.Definition.PropertyName));

      var objectEndPoint = _map.RegisterRealObjectEndPoint (id, foreignKeyProperty);

      Assert.That (_map[id], Is.SameAs (objectEndPoint));
    }

    [Test]
    public void RegisterCollectionEndPoint_UsesChangeDetectionStrategy ()
    {
      RelationEndPointID endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      var endPoint = _map.RegisterCollectionEndPoint (endPointID, new DomainObject[0]);

      Assert.That (endPoint.ChangeDetectionStrategy, Is.SameAs (_map.CollectionEndPointChangeDetectionStrategy));
    }

    [Test]
    public void RegisterExistingDataContainer_RegistersRealObjectEndPoints ()
    {
      var foreignKeyPropertyName = typeof (OrderTicket) + ".Order";
      var dataContainer = DataContainer.CreateForExisting (
          DomainObjectIDs.OrderTicket1, 
          null, 
          pd => pd.PropertyName == foreignKeyPropertyName ? DomainObjectIDs.Order2 :  pd.DefaultValue);
      var foreignKeyProperty = dataContainer.PropertyValues[foreignKeyPropertyName];

      _map.RegisterExistingDataContainer (dataContainer);

      var expectedID = new RelationEndPointID (dataContainer.ID, typeof (OrderTicket) + ".Order");
      var endPoint = (RealObjectEndPoint) _map[expectedID];

      Assert.That (endPoint, Is.Not.Null);
      Assert.That (endPoint.ForeignKeyProperty, Is.SameAs (foreignKeyProperty));
      Assert.That (endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.Order2));
    }

    [Test]
    public void RegisterExistingDataContainer_RegistersVirtualObjectEndPoints ()
    {
      var foreignKeyPropertyName = typeof (OrderTicket) + ".Order";
      var dataContainer = DataContainer.CreateForExisting (
          DomainObjectIDs.OrderTicket1,
          null,
          pd => pd.PropertyName == foreignKeyPropertyName ? DomainObjectIDs.Order2 : pd.DefaultValue);

      _map.RegisterExistingDataContainer (dataContainer);

      var expectedID = new RelationEndPointID (DomainObjectIDs.Order2, typeof (Order) + ".OrderTicket");
      var endPoint = (VirtualObjectEndPoint) _map[expectedID];

      Assert.That (endPoint, Is.Not.Null);
      Assert.That (endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
    }

    [Test]
    public void RegisterExistingDataContainer_RegistersNoNullObjectEndPoints ()
    {
      var foreignKeyPropertyName = typeof (OrderTicket) + ".Order";
      var dataContainer = DataContainer.CreateForExisting (
          DomainObjectIDs.OrderTicket1,
          null,
          pd => pd.PropertyName == foreignKeyPropertyName ? null : pd.DefaultValue);

      _map.RegisterExistingDataContainer (dataContainer);

      var endPointDefinition = DomainObjectIDs.Order1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Order) + ".OrderTicket");
      var expectedID = new RelationEndPointID (null, endPointDefinition);

      Assert.That (_map[expectedID], Is.Null);
    }

    [Test]
    public void RegisterExistingDataContainer_RegistersNoCollectionEndPoints ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);

      _map.RegisterExistingDataContainer (dataContainer);

      var expectedID = new RelationEndPointID (DomainObjectIDs.Order1, typeof (Order) + ".OrderItems");

      Assert.That (_map[expectedID], Is.Null);
    }

    [Test]
    public void RegisterNewDataContainer_RegistersVirtualObjectEndPoints ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);

      _map.RegisterNewDataContainer (dataContainer);

      var expectedID = new RelationEndPointID (DomainObjectIDs.Order1, typeof (Order) + ".OrderTicket");
      var objectEndPoint = (VirtualObjectEndPoint) _map[expectedID];
      Assert.That (objectEndPoint, Is.Not.Null);
      Assert.That (objectEndPoint.OppositeObjectID, Is.Null);
    }

    [Test]
    public void RegisterNewDataContainer_RegistersRealObjectEndPoints ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderTicket1);

      _map.RegisterNewDataContainer (dataContainer);

      var expectedID = new RelationEndPointID (DomainObjectIDs.OrderTicket1, typeof (OrderTicket) + ".Order");
      var objectEndPoint = (RealObjectEndPoint) _map[expectedID];
      Assert.That (objectEndPoint.ForeignKeyProperty, Is.Not.Null);
      Assert.That (objectEndPoint.ForeignKeyProperty, Is.SameAs (dataContainer.PropertyValues[typeof (OrderTicket) + ".Order"]));
    }

    [Test]
    public void RegisterNewDataContainer_RegistersCollectionEndPoints ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);

      _map.RegisterNewDataContainer (dataContainer);

      var expectedID = new RelationEndPointID (DomainObjectIDs.Order1, typeof (Order) + ".OrderItems");
      var collectionEndPoint = (CollectionEndPoint) _map[expectedID];
      Assert.That (collectionEndPoint, Is.Not.Null);
      Assert.That (collectionEndPoint.OppositeDomainObjects, Is.Empty);
    }

    [Test]
    public void Commit_CommitsEndPoints ()
    {
      RelationEndPointID endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      var endPoint = _map.RegisterCollectionEndPoint (endPointID, new DomainObject[0]);

      var addedObject = Order.NewObject ();
      endPoint.OppositeDomainObjects.Add (addedObject);
      Assert.That (endPoint.HasChanged, Is.True);

      _map.Commit ();

      Assert.That (endPoint.HasChanged, Is.False);
      Assert.That (endPoint.OppositeDomainObjects, Is.EqualTo (new[] { addedObject }));
    }

    [Test]
    public void Rollback_RollsBackEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      var endPoint = _map.RegisterCollectionEndPoint (endPointID, new DomainObject[0]);

      var addedObject = Order.NewObject ();
      endPoint.OppositeDomainObjects.Add (addedObject);
      Assert.That (endPoint.HasChanged, Is.True);

      _map.Rollback ();

      Assert.That (endPoint.HasChanged, Is.False);
      Assert.That (endPoint.OppositeDomainObjects, Is.Empty);
    }

    [Test]
    public void Discard_RemovesEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      _map.RegisterCollectionEndPoint (endPointID, new DomainObject[0]);
      Assert.That (_map[endPointID], Is.Not.Null);

      _map.Discard (endPointID);

      Assert.That (_map[endPointID], Is.Null);
    }

    [Test]
    public void Discard_RaisesNotification_BeforeRemoving ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      _map.RegisterCollectionEndPoint (endPointID, new DomainObject[0]);
      Assert.That (_map[endPointID], Is.Not.Null);

      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      listenerMock.Expect (mock => mock.RelationEndPointMapUnregistering (endPointID))
          .WhenCalled (mi => Assert.That (_map[endPointID], Is.Not.Null));
      ClientTransactionMock.AddListener (listenerMock);

      listenerMock.Replay ();

      _map.Discard (endPointID);

      listenerMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "End point 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders' is "
        + "not part of this map.\r\nParameter name: endPointID")]
    public void Discard_NonExistingEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      _map.Discard (endPointID);
    }
  }
}