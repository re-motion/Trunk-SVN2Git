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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class ObjectEndPointSetUnidirectionalCommandTest : ObjectEndPointSetCommandTestBase
  {
    protected override DomainObject OldRelatedObject
    {
      get { return Client.GetObject (DomainObjectIDs.Client1); }
    }

    protected override DomainObject NewRelatedObject
    {
      get { return Client.GetObject (DomainObjectIDs.Client2); }
    }

    protected override RelationEndPointID GetRelationEndPointID ()
    {
      return new RelationEndPointID (DomainObjectIDs.Client3, typeof (Client).FullName + ".ParentClient");
    }

    protected override ObjectEndPointSetCommand CreateCommand (IObjectEndPoint endPoint, DomainObject newRelatedObject)
    {
      return new ObjectEndPointSetUnidirectionalCommand (endPoint, newRelatedObject);
    }

    protected override ObjectEndPointSetCommand CreateCommandMock (MockRepository repository, ObjectEndPoint endPoint, DomainObject newRelatedObject)
    {
      return repository.StrictMock<ObjectEndPointSetUnidirectionalCommand> (endPoint, newRelatedObject);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "EndPoint 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order' "
        + "is from a bidirectional relation - use a ObjectEndPointSetOneOneCommand or ObjectEndPointSetOneManyCommand instead.\r\nParameter name: modifiedEndPoint")]
    public void Initialization_Bidirectional_OneMany ()
    {
      var definition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem))
          .GetMandatoryRelationEndPointDefinition (typeof (OrderItem).FullName + ".Order");
      var orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var id = new RelationEndPointID (orderItem.ID, definition);

      var endPoint = (ObjectEndPoint)ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (id);
      new ObjectEndPointSetUnidirectionalCommand (endPoint, Order.NewObject ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "EndPoint 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order' "
        + "is from a bidirectional relation - use a ObjectEndPointSetOneOneCommand or ObjectEndPointSetOneManyCommand instead.\r\nParameter name: modifiedEndPoint")]
    public void Initialization_Bidirectional_OneOne ()
    {
      var definition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderTicket))
          .GetMandatoryRelationEndPointDefinition (typeof (OrderTicket).FullName + ".Order");
      var relationEndPointID = new RelationEndPointID (OrderTicket.GetObject (DomainObjectIDs.OrderTicket1).ID, definition);
      var endPoint = (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (relationEndPointID);
      new ObjectEndPointSetUnidirectionalCommand (endPoint, Order.NewObject ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "New related object for EndPoint "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Client.ParentClient' is the same as its old value - use a ObjectEndPointSetSameCommand "
        + "instead.\r\nParameter name: newRelatedObject")]
    public void Initialization_Same ()
    {
      var endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (GetRelationEndPointID (), OldRelatedObject.ID);
      new ObjectEndPointSetUnidirectionalCommand (endPoint, OldRelatedObject);
    }

    [Test]
    public void ExtendToAllRelatedObjects_SetDifferent_Unidirectional ()
    {
      var client = Client.GetObject (DomainObjectIDs.Client2);
      var parentClientEndPointDefinition = client.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (Client).FullName + ".ParentClient");
      var unidirectionalEndPointID = new RelationEndPointID (client.ID, parentClientEndPointDefinition);
      var unidirectionalEndPoint = 
          (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (unidirectionalEndPointID);
      Assert.That (unidirectionalEndPoint.Definition.GetOppositeEndPointDefinition().IsAnonymous, Is.True);
      var newClient = Client.NewObject ();

      var setDifferentCommand = new ObjectEndPointSetUnidirectionalCommand (unidirectionalEndPoint, newClient);
      var bidirectionalModification = setDifferentCommand.ExtendToAllRelatedObjects ();
      Assert.That (bidirectionalModification, Is.SameAs (setDifferentCommand));
    }
  }
}
