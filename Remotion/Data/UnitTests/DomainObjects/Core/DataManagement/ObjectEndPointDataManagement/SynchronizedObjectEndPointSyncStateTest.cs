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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.ObjectEndPointDataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Serialization;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.ObjectEndPointDataManagement
{
  [TestFixture]
  public class SynchronizedObjectEndPointSyncStateTest : StandardMappingTest
  {
    private IObjectEndPoint _endPointMock;
    private SynchronizedObjectEndPointSyncState _state;

    private Order _order;
    private Location _location;

    private IRelationEndPointDefinition _orderOrderTicketEndPointDefinition;
    private IRelationEndPointDefinition _locationClientEndPointDefinition;
    private IRelationEndPointDefinition _orderCustomerEndPointDefinition;

    private Action<ObjectID> _fakeSetter;

    public override void SetUp ()
    {
      base.SetUp ();
      
      _endPointMock = MockRepository.GenerateStrictMock<IObjectEndPoint>();
      _state = new SynchronizedObjectEndPointSyncState ();

      _order = DomainObjectMother.CreateFakeObject<Order> ();
      _location = DomainObjectMother.CreateFakeObject<Location> ();

      _orderOrderTicketEndPointDefinition = GetRelationEndPointDefinition(typeof (Order), "OrderTicket");
      _locationClientEndPointDefinition = GetRelationEndPointDefinition (typeof (Location), "Client");
      _orderCustomerEndPointDefinition = GetRelationEndPointDefinition (typeof (Order), "Customer");

      _fakeSetter = id => { };
    }

    [Test]
    public void IsSynchronized ()
    {
      Assert.That (_state.IsSynchronized(_endPointMock), Is.True);
    }

    [Test]
    public void Synchronize ()
    {
      var oppositeEndPointMock = MockRepository.GenerateStrictMock<IRelationEndPoint>();
      oppositeEndPointMock.Replay();
      
      _state.Synchronize (_endPointMock, oppositeEndPointMock);

      oppositeEndPointMock.AssertWasNotCalled(mock=>mock.SynchronizeOppositeEndPoint (_endPointMock));
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      _endPointMock.Stub (stub => stub.GetDomainObject ()).Return (_order);
      _endPointMock.Stub (stub => stub.IsNull).Return (false);

      var command = (RelationEndPointModificationCommand) _state.CreateDeleteCommand (_endPointMock, _fakeSetter);

      Assert.That (command, Is.TypeOf (typeof (ObjectEndPointDeleteCommand)));
      Assert.That (command.DomainObject, Is.SameAs (_order));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_endPointMock));
      Assert.That (GetOppositeObjectIDSetter (command), Is.SameAs (_fakeSetter));
    }

    [Test]
    public void CreateSetCommand_Same ()
    {
      var relatedObject = DomainObjectMother.CreateFakeObject<OrderTicket> ();

      _endPointMock.Stub (stub => stub.Definition).Return (_orderOrderTicketEndPointDefinition);
      _endPointMock.Stub (stub => stub.GetDomainObject ()).Return (_order);
      _endPointMock.Stub (stub => stub.IsNull).Return (false);

      _endPointMock.Stub (stub => stub.OppositeObjectID).Return (relatedObject.ID);
      _endPointMock.Stub (stub => stub.GetOppositeObject (Arg<bool>.Is.Anything)).Return (relatedObject);

      var command = (RelationEndPointModificationCommand) _state.CreateSetCommand (_endPointMock, relatedObject, _fakeSetter);

      Assert.That (command, Is.TypeOf (typeof (ObjectEndPointSetSameCommand)));
      Assert.That (command.DomainObject, Is.SameAs (_order));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_endPointMock));
      Assert.That (command.OldRelatedObject, Is.SameAs (relatedObject));
      Assert.That (command.NewRelatedObject, Is.SameAs (relatedObject));
      Assert.That (GetOppositeObjectIDSetter (command), Is.SameAs (_fakeSetter));
    }

    [Test]
    public void CreateSetCommand_Same_Null ()
    {
      _endPointMock.Stub (stub => stub.Definition).Return (_orderOrderTicketEndPointDefinition);
      _endPointMock.Stub (stub => stub.GetDomainObject ()).Return (_order);
      _endPointMock.Stub (stub => stub.IsNull).Return (false);

      _endPointMock.Stub (stub => stub.OppositeObjectID).Return (null);
      _endPointMock.Stub (stub => stub.GetOppositeObject (Arg<bool>.Is.Anything)).Return (null);

      var command = (RelationEndPointModificationCommand) _state.CreateSetCommand (_endPointMock, null, _fakeSetter);

      Assert.That (command, Is.TypeOf (typeof (ObjectEndPointSetSameCommand)));
      Assert.That (command.DomainObject, Is.SameAs (_order));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_endPointMock));
      Assert.That (command.OldRelatedObject, Is.Null);
      Assert.That (command.NewRelatedObject, Is.Null);
      Assert.That (GetOppositeObjectIDSetter (command), Is.SameAs (_fakeSetter));
    }

    [Test]
    public void CreateSetCommand_Unidirectional ()
    {
      var oldRelatedObject = DomainObjectMother.CreateFakeObject<Client> ();
      var newRelatedObject = DomainObjectMother.CreateFakeObject<Client> ();

      _endPointMock.Stub (stub => stub.Definition).Return (_locationClientEndPointDefinition);
      _endPointMock.Stub (stub => stub.GetDomainObject ()).Return (_location);
      _endPointMock.Stub (stub => stub.IsNull).Return (false);

      _endPointMock.Stub (stub => stub.OppositeObjectID).Return (oldRelatedObject.ID);
      _endPointMock.Stub (stub => stub.GetOppositeObject (Arg<bool>.Is.Anything)).Return (oldRelatedObject);

      var command = (RelationEndPointModificationCommand) _state.CreateSetCommand (_endPointMock, newRelatedObject, _fakeSetter);

      Assert.That (command.GetType (), Is.EqualTo (typeof (ObjectEndPointSetUnidirectionalCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_endPointMock));
      Assert.That (command.NewRelatedObject, Is.SameAs (newRelatedObject));
      Assert.That (command.OldRelatedObject, Is.SameAs (oldRelatedObject));
      Assert.That (GetOppositeObjectIDSetter (command), Is.SameAs (_fakeSetter));
    }

    [Test]
    public void CreateSetCommand_OneOne ()
    {
      var oldRelatedObject = DomainObjectMother.CreateFakeObject<OrderTicket>();
      var newRelatedObject = DomainObjectMother.CreateFakeObject<OrderTicket> ();

      _endPointMock.Stub (stub => stub.Definition).Return (_orderOrderTicketEndPointDefinition);
      _endPointMock.Stub (stub => stub.GetDomainObject ()).Return (_order);
      _endPointMock.Stub (stub => stub.IsNull).Return (false);

      _endPointMock.Stub (stub => stub.OppositeObjectID).Return (oldRelatedObject.ID);
      _endPointMock.Stub (stub => stub.GetOppositeObject (Arg<bool>.Is.Anything)).Return (oldRelatedObject);

      var command = (RelationEndPointModificationCommand) _state.CreateSetCommand (_endPointMock, newRelatedObject, _fakeSetter);

      Assert.That (command.GetType (), Is.EqualTo (typeof (ObjectEndPointSetOneOneCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_endPointMock));
      Assert.That (command.NewRelatedObject, Is.SameAs (newRelatedObject));
      Assert.That (command.OldRelatedObject, Is.SameAs (oldRelatedObject));
      Assert.That (GetOppositeObjectIDSetter (command), Is.SameAs (_fakeSetter));
    }

    [Test]
    public void CreateSetCommand_OneMany ()
    {
      var oldRelatedObject = DomainObjectMother.CreateFakeObject<Customer> ();
      var newRelatedObject = DomainObjectMother.CreateFakeObject<Customer> ();

      _endPointMock.Stub (stub => stub.Definition).Return (_orderCustomerEndPointDefinition);
      _endPointMock.Stub (stub => stub.GetDomainObject ()).Return (_order);
      _endPointMock.Stub (stub => stub.IsNull).Return (false);

      _endPointMock.Stub (stub => stub.OppositeObjectID).Return (oldRelatedObject.ID);
      _endPointMock.Stub (stub => stub.GetOppositeObject (Arg<bool>.Is.Anything)).Return (oldRelatedObject);

      var command = (RelationEndPointModificationCommand) _state.CreateSetCommand (_endPointMock, newRelatedObject, _fakeSetter);
      Assert.That (command.GetType (), Is.EqualTo (typeof (ObjectEndPointSetOneManyCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_endPointMock));
      Assert.That (command.NewRelatedObject, Is.SameAs (newRelatedObject));
      Assert.That (command.OldRelatedObject, Is.SameAs (oldRelatedObject));
      Assert.That (GetOppositeObjectIDSetter (command), Is.SameAs (_fakeSetter));
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var state = new SynchronizedObjectEndPointSyncState ();

      var result = FlattenedSerializer.SerializeAndDeserialize (state);

      Assert.That (result, Is.Not.Null);
    }

    private IRelationEndPointDefinition GetRelationEndPointDefinition (Type classType, string shortPropertyName)
    {
      return Configuration.ClassDefinitions.GetMandatory (classType).GetRelationEndPointDefinition (classType.FullName + "." + shortPropertyName);
    }

    private object GetOppositeObjectIDSetter (RelationEndPointModificationCommand command)
    {
      return PrivateInvoke.GetNonPublicField (command, "_oppositeObjectIDSetter");
    }

  }
}