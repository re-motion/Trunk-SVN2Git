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
// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.

using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class ObjectEndPointSetOneManyCommandTest : ObjectEndPointSetCommandTestBase
  {
    private OrderItem _domainObject;
    private Order _oldRelatedObject;
    private Order _newRelatedObject;

    private RelationEndPointID _endPointID;
    private RealObjectEndPoint _endPoint;

    private ObjectEndPointSetCommand _command;

    public override void SetUp ()
    {
      base.SetUp ();

      _domainObject = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      _oldRelatedObject = _domainObject.Order;
      _newRelatedObject = Order.GetObject (DomainObjectIDs.Order2);

      _endPointID = RelationEndPointID.Create (_domainObject, oi => oi.Order);
      _endPoint = (RealObjectEndPoint) RelationEndPointObjectMother.CreateObjectEndPoint (_endPointID, _oldRelatedObject.ID);

      _command = new ObjectEndPointSetOneManyCommand (_endPoint, _newRelatedObject, OppositeObjectSetter, EndPointProviderStub);
    }

    [Test]
    public void Initialization ()
    {
      Assert.AreSame (_endPoint, _command.ModifiedEndPoint);
      Assert.AreSame (_oldRelatedObject, _command.OldRelatedObject);
      Assert.AreSame (_newRelatedObject, _command.NewRelatedObject);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Modified end point is null, a NullEndPointModificationCommand is needed.\r\n"
                                                                      + "Parameter name: modifiedEndPoint")]
    public void Initialization_FromNullEndPoint ()
    {
      var endPoint = MockRepository.GenerateStub<IRealObjectEndPoint>();
      endPoint.Stub (stub => stub.IsNull).Return (true);

      new ObjectEndPointSetOneManyCommand (endPoint, _newRelatedObject, OppositeObjectSetter, EndPointProviderStub);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "EndPoint 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Client.ParentClient' "
        + "is from a unidirectional relation - use a ObjectEndPointSetUnidirectionalCommand instead.\r\nParameter name: modifiedEndPoint")]
    public void Initialization_Unidirectional ()
    {
      var definition = MappingConfiguration.Current.GetTypeDefinition (typeof (Client))
          .GetMandatoryRelationEndPointDefinition (typeof (Client).FullName + ".ParentClient");
      var relationEndPointID = RelationEndPointID.Create (Client.GetObject (DomainObjectIDs.Client1).ID, definition);
      var endPoint = (IRealObjectEndPoint)
          ClientTransactionMock.DataManager.GetRelationEndPointWithLazyLoad (relationEndPointID);
      new ObjectEndPointSetOneManyCommand (endPoint, Client.NewObject (), mi => { }, EndPointProviderStub);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "EndPoint 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order' "
        + "is from a 1:1 relation - use a ObjectEndPointSetOneOneCommand instead.\r\nParameter name: modifiedEndPoint")]
    public void Initialization_Bidirectional_OneOne ()
    {
      var definition = MappingConfiguration.Current.GetTypeDefinition (typeof (OrderTicket))
          .GetMandatoryRelationEndPointDefinition (typeof (OrderTicket).FullName + ".Order");
      var relationEndPointID = RelationEndPointID.Create (OrderTicket.GetObject (DomainObjectIDs.OrderTicket1).ID, definition);
      var endPoint = (IRealObjectEndPoint) ClientTransactionMock.DataManager.GetRelationEndPointWithLazyLoad (relationEndPointID);
      new ObjectEndPointSetOneManyCommand (endPoint, Order.NewObject (), mi => { }, EndPointProviderStub);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "New related object for EndPoint "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order' is the same as its old value - use a ObjectEndPointSetSameCommand "
        + "instead.\r\nParameter name: newRelatedObject")]
    public void Initialization_Same ()
    {
      var endPoint = (IRealObjectEndPoint) RelationEndPointObjectMother.CreateObjectEndPoint (_endPointID, _oldRelatedObject.ID);
      new ObjectEndPointSetOneManyCommand (endPoint, _oldRelatedObject, mi => { }, EndPointProviderStub);
    }

    [Test]
    public virtual void Begin ()
    {
      DomainObject domainObject = ((IObjectEndPoint) _endPoint).GetDomainObject ();
      var eventReceiver = new DomainObjectEventReceiver (domainObject);

      _command.Begin();

      Assert.IsTrue (eventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasRelationChangedEventBeenCalled);
    }

    [Test]
    public void Perform ()
    {
      Assert.That (OppositeObjectSetterCalled, Is.False);
      Assert.That (_endPoint.HasBeenTouched, Is.False);

      _command.Perform ();

      Assert.That (OppositeObjectSetterCalled, Is.True);
      Assert.That (OppositeObjectSetterObject, Is.SameAs (_newRelatedObject));
      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public virtual void End ()
    {
      DomainObject domainObject = ((IObjectEndPoint) _endPoint).GetDomainObject ();
      var eventReceiver = new DomainObjectEventReceiver (domainObject);

      _command.End();

      Assert.IsFalse (eventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsTrue (eventReceiver.HasRelationChangedEventBeenCalled);
    }

    [Test]
    public virtual void NotifyClientTransactionOfBegin ()
    {
      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      _command.NotifyClientTransactionOfBegin();

      listenerMock.AssertWasCalled(mock => mock.RelationChanging (
          ClientTransactionMock, 
          _endPoint.GetDomainObject (), 
          _endPoint.Definition, 
          _oldRelatedObject, 
          _newRelatedObject));
    }

    [Test]
    public virtual void NotifyClientTransactionOfEnd ()
    {
      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      _command.NotifyClientTransactionOfEnd ();

      listenerMock.AssertWasCalled (mock => mock.RelationChanged (
          ClientTransactionMock, 
          _endPoint.GetDomainObject (), 
          _endPoint.Definition));
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      // Scenario: orderItem.Order = newOrder;

      var oldRelatedEndPointID = RelationEndPointID.Create (_oldRelatedObject, o => o.OrderItems);
      var oldRelatedEndPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();

      var newRelatedEndPointID = RelationEndPointID.Create (_newRelatedObject, o => o.OrderItems);
      var newRelatedEndPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();

      EndPointProviderStub.Stub (stub => stub.GetRelationEndPointWithLazyLoad (oldRelatedEndPointID)).Return (oldRelatedEndPointMock);
      EndPointProviderStub.Stub (stub => stub.GetRelationEndPointWithLazyLoad (newRelatedEndPointID)).Return (newRelatedEndPointMock);


      // oldOrder.OrderItems.Remove (orderItem)
      var fakeRemoveCommand = MockRepository.GenerateStub<IDataManagementCommand>();
      fakeRemoveCommand.Stub (stub => stub.GetAllExceptions ()).Return (new Exception[0]);
      oldRelatedEndPointMock.Expect (mock => mock.CreateRemoveCommand (_domainObject)).Return (fakeRemoveCommand);
      oldRelatedEndPointMock.Replay();

      // newOrder.OrderItems.Add (orderItem);
      var fakeAddCommand = MockRepository.GenerateStub<IDataManagementCommand> ();
      fakeAddCommand.Stub (stub => stub.GetAllExceptions ()).Return (new Exception[0]);
      newRelatedEndPointMock.Expect (mock => mock.CreateAddCommand (_domainObject)).Return (fakeAddCommand);
      newRelatedEndPointMock.Replay ();

      var bidirectionalModification = _command.ExpandToAllRelatedObjects ();

      oldRelatedEndPointMock.VerifyAllExpectations ();
      newRelatedEndPointMock.VerifyAllExpectations ();

      var steps = bidirectionalModification.GetNestedCommands();
      Assert.That (steps.Count, Is.EqualTo (3));

      // orderItem.Order = newOrder;
      Assert.That (steps[0], Is.SameAs (_command));

      // newOrder.OrderItems.Add (orderItem);
      Assert.That (steps[1], Is.SameAs (fakeAddCommand));

      // oldOrder.OrderItems.Remove (orderItem)
      Assert.That (steps[2], Is.SameAs (fakeRemoveCommand));
    }
  }
}
