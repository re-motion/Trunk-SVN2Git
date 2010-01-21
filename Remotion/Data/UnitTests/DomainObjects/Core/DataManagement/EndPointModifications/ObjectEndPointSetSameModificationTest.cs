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
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.EndPointModifications
{
  [TestFixture]
  public class ObjectEndPointSetSameModificationTest : ObjectEndPointSetModificationBaseTest
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

    protected override ObjectEndPointSetModificationBase CreateModification (IObjectEndPoint endPoint, DomainObject newRelatedObject)
    {
      return new ObjectEndPointSetSameModification (endPoint);
    }

    protected override ObjectEndPointSetModificationBase CreateModificationMock (MockRepository repository, ObjectEndPoint endPoint, DomainObject newRelatedObject)
    {
      return repository.StrictMock<ObjectEndPointSetSameModification> (endPoint);
    }

    [Test]
    public override void Begin ()
    {
      DomainObject domainObject = Order.GetObject (DomainObjectIDs.Order1);
      var eventReceiver = new DomainObjectEventReceiver (domainObject);

      Modification.Begin ();

      Assert.IsFalse (eventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasRelationChangedEventBeenCalled);
    }

    [Test]
    public override void End ()
    {
      DomainObject domainObject = Order.GetObject (DomainObjectIDs.Order1);
      var eventReceiver = new DomainObjectEventReceiver (domainObject);

      Modification.End ();

      Assert.IsFalse (eventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasRelationChangedEventBeenCalled);
    }

    [Test]
    public override void NotifyClientTransactionOfBegin ()
    {
      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      Modification.NotifyClientTransactionOfBegin ();

      listenerMock.AssertWasNotCalled (mock => mock.RelationChanging (
          EndPoint.GetDomainObject (), 
          EndPoint.PropertyName, 
          OldRelatedObject, 
          NewRelatedObject));
    }

    [Test]
    public override void NotifyClientTransactionOfEnd ()
    {
      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      Modification.NotifyClientTransactionOfBegin ();

      listenerMock.AssertWasNotCalled (mock => mock.RelationChanged (EndPoint.GetDomainObject (), EndPoint.PropertyName));
    }

    [Test]
    public void CreateBidirectionalModification_SetSame_Unidirectional ()
    {
      var client = Client.GetObject (DomainObjectIDs.Client2);
      var parentClientEndPointDefinition = client.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (Client).FullName + ".ParentClient");
      var unidirectionalEndPointID = new RelationEndPointID (client.ID, parentClientEndPointDefinition);
      var unidirectionalEndPoint = 
          (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (unidirectionalEndPointID);
      Assert.That (unidirectionalEndPoint.Definition.GetOppositeEndPointDefinition().IsAnonymous, Is.True);

      var setSameModification = new ObjectEndPointSetSameModification (unidirectionalEndPoint);
      var bidirectionalModification = setSameModification.ExtendToAllRelatedObjects ();
      Assert.That (bidirectionalModification, Is.InstanceOfType (typeof (CompositeRelationModificationWithoutEvents)));

      var steps = bidirectionalModification.GetModificationSteps ();
      Assert.That (steps.Count, Is.EqualTo (1));

      Assert.That (steps[0], Is.SameAs (setSameModification));
    }

    [Test]
    public void CreateBidirectionalModification_SetSame_Bidirectional ()
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
      var setSameModification = new ObjectEndPointSetSameModification (bidirectionalEndPoint);

      var bidirectionalModification = setSameModification.ExtendToAllRelatedObjects ();
      Assert.That (bidirectionalModification, Is.InstanceOfType (typeof (CompositeRelationModificationWithoutEvents)));

      var steps = GetModificationSteps (bidirectionalModification);
      Assert.That (steps.Count, Is.EqualTo (2));

      Assert.That (steps[0], Is.SameAs (setSameModification));

      Assert.That (steps[1], Is.InstanceOfType (typeof (RelationEndPointTouchModification)));
      Assert.That (steps[1].ModifiedEndPoint, Is.SameAs (oppositeEndPoint));
    }
  }
}
