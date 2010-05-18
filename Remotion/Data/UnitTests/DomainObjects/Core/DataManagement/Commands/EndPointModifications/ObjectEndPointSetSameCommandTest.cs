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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class ObjectEndPointSetSameCommandTest : ObjectEndPointSetCommandTestBase
  {
    protected override DomainObject OldRelatedObject
    {
      get { return Employee.GetObject (DomainObjectIDs.Employee3); }
    }

    protected override DomainObject NewRelatedObject
    {
      get { return Employee.GetObject (DomainObjectIDs.Employee3); }
    }

    protected override RelationEndPointID GetRelationEndPointID ()
    {
      return new RelationEndPointID (DomainObjectIDs.Computer1, typeof (Computer).FullName + ".Employee");
    }

    protected override ObjectEndPointSetCommand CreateCommand (IObjectEndPoint endPoint, DomainObject newRelatedObject)
    {
      return new ObjectEndPointSetSameCommand (endPoint);
    }

    protected override ObjectEndPointSetCommand CreateCommandMock (MockRepository repository, ObjectEndPoint endPoint, DomainObject newRelatedObject)
    {
      return repository.StrictMock<ObjectEndPointSetSameCommand> (endPoint);
    }

    [Test]
    public override void Begin ()
    {
      DomainObject domainObject = Order.GetObject (DomainObjectIDs.Order1);
      var eventReceiver = new DomainObjectEventReceiver (domainObject);

      Command.Begin ();

      Assert.IsFalse (eventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasRelationChangedEventBeenCalled);
    }

    [Test]
    public override void End ()
    {
      DomainObject domainObject = Order.GetObject (DomainObjectIDs.Order1);
      var eventReceiver = new DomainObjectEventReceiver (domainObject);

      Command.End ();

      Assert.IsFalse (eventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasRelationChangedEventBeenCalled);
    }

    [Test]
    public override void NotifyClientTransactionOfBegin ()
    {
      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      Command.NotifyClientTransactionOfBegin ();

      listenerMock.AssertWasNotCalled (mock => mock.RelationChanging (
          Arg<ClientTransaction>.Is.Anything, 
          Arg<DomainObject>.Is.Anything,
          Arg<string>.Is.Anything,
          Arg<DomainObject>.Is.Anything,
          Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public override void NotifyClientTransactionOfEnd ()
    {
      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      Command.NotifyClientTransactionOfBegin ();

      listenerMock.AssertWasNotCalled (mock => mock.RelationChanged (
          Arg<ClientTransaction>.Is.Anything,
          Arg<DomainObject>.Is.Anything,
          Arg<string>.Is.Anything));
    }

    [Test]
    public void ExpandToAllRelatedObjects_SetSame_Unidirectional ()
    {
      var client = Client.GetObject (DomainObjectIDs.Client2);
      var parentClientEndPointDefinition = client.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (Client).FullName + ".ParentClient");
      var unidirectionalEndPointID = new RelationEndPointID (client.ID, parentClientEndPointDefinition);
      var unidirectionalEndPoint = 
          (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (unidirectionalEndPointID);
      Assert.That (unidirectionalEndPoint.Definition.GetOppositeEndPointDefinition().IsAnonymous, Is.True);

      var setSameModification = new ObjectEndPointSetSameCommand (unidirectionalEndPoint);
      var bidirectionalModification = setSameModification.ExpandToAllRelatedObjects ();
      Assert.That (bidirectionalModification.GetNestedCommands(), Is.EqualTo (new[] { setSameModification }));
    }

    [Test]
    public void ExpandToAllRelatedObjects_SetSame_Bidirectional ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderTicketEndPointDefinition = order.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");
      var orderTicketEndPointID = new RelationEndPointID (order.ID, orderTicketEndPointDefinition);
      var bidirectionalEndPoint = 
          (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (orderTicketEndPointID);

      var oppositeEndPointID = new RelationEndPointID (
          bidirectionalEndPoint.GetOppositeObject (true).ID, 
          bidirectionalEndPoint.Definition.GetOppositeEndPointDefinition());

      var oppositeEndPoint = ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (oppositeEndPointID);
      var setSameCommand = new ObjectEndPointSetSameCommand (bidirectionalEndPoint);

      var bidirectionalModification = setSameCommand.ExpandToAllRelatedObjects ();

      var steps = bidirectionalModification.GetNestedCommands ();
      Assert.That (steps.Count, Is.EqualTo (2));

      Assert.That (steps[0], Is.SameAs (setSameCommand));

      Assert.That (steps[1], Is.InstanceOfType (typeof (RelationEndPointTouchCommand)));
      Assert.That (((RelationEndPointTouchCommand) steps[1]).EndPoint, Is.SameAs (oppositeEndPoint));
    }
  }
}
