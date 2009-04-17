// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.EndPointModifications
{
  [TestFixture]
  public class ObjectEndPointSetUnidirectionalModificationTest : ObjectEndPointSetModificationBaseTest
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

    protected override ObjectEndPointSetModificationBase CreateModification (ObjectEndPoint endPoint, DomainObject newRelatedObject)
    {
      return new ObjectEndPointSetUnidirectionalModification (endPoint, newRelatedObject);
    }

    protected override ObjectEndPointSetModificationBase CreateModificationMock (MockRepository repository, ObjectEndPoint endPoint, DomainObject newRelatedObject)
    {
      return repository.StrictMock<ObjectEndPointSetUnidirectionalModification> (endPoint, newRelatedObject);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "EndPoint 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order' "
        + "is from a bidirectional relation - use a ObjectEndPointSetOneOneModification or ObjectEndPointSetOneManyModification instead.\r\nParameter name: modifiedEndPoint")]
    public void Initialization_Bidirectional_OneMany ()
    {
      var definition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem))
          .GetMandatoryRelationEndPointDefinition (typeof (OrderItem).FullName + ".Order");
      var endPoint = (ObjectEndPoint)
          ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (OrderItem.GetObject (DomainObjectIDs.OrderItem1), definition);
      new ObjectEndPointSetUnidirectionalModification (endPoint, Order.NewObject ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "EndPoint 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order' "
        + "is from a bidirectional relation - use a ObjectEndPointSetOneOneModification or ObjectEndPointSetOneManyModification instead.\r\nParameter name: modifiedEndPoint")]
    public void Initialization_Bidirectional_OneOne ()
    {
      var definition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderTicket))
          .GetMandatoryRelationEndPointDefinition (typeof (OrderTicket).FullName + ".Order");
      var endPoint = (ObjectEndPoint)
          ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (OrderTicket.GetObject (DomainObjectIDs.OrderTicket1), definition);
      new ObjectEndPointSetUnidirectionalModification (endPoint, Order.NewObject ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "New related object for EndPoint "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Client.ParentClient' is the same as its old value - use a ObjectEndPointSetSameModification "
        + "instead.\r\nParameter name: newRelatedObject")]
    public void Initialization_Same ()
    {
      var endPoint = new ObjectEndPoint (ClientTransactionMock, GetRelationEndPointID (), OldRelatedObject.ID);
      new ObjectEndPointSetUnidirectionalModification (endPoint, OldRelatedObject);
    }

    [Test]
    public void CreateBidirectionalModification_SetDifferent_Unidirectional ()
    {
      var client = Client.GetObject (DomainObjectIDs.Client2);
      var parentClientEndPointDefinition = client.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (Client).FullName + ".ParentClient");
      var unidirectionalEndPoint = (ObjectEndPoint)
                                   ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (
                                       client, parentClientEndPointDefinition);
      Assert.That (unidirectionalEndPoint.OppositeEndPointDefinition.IsAnonymous, Is.True);
      var newClient = Client.NewObject ();

      var setDifferentModification = new ObjectEndPointSetUnidirectionalModification (unidirectionalEndPoint, newClient);
      var bidirectionalModification = setDifferentModification.CreateBidirectionalModification ();
      Assert.That (bidirectionalModification, Is.InstanceOfType (typeof (NotifyingBidirectionalRelationModification)));

      var steps = bidirectionalModification.GetModificationSteps ();
      Assert.That (steps.Count, Is.EqualTo (1));

      Assert.That (steps[0], Is.SameAs (setDifferentModification));
    }
  }
}