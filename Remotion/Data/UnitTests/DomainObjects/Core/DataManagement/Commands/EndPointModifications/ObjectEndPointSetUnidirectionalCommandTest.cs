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
  public class ObjectEndPointSetUnidirectionalCommandTest : ObjectEndPointSetCommandTestBase
  {
    private Client _domainObject;
    private Client _oldRelatedObject;
    private Client _newRelatedObject;

    private RelationEndPointID _endPointID;
    private ObjectEndPoint _endPoint;
    
    private ObjectEndPointSetCommand _command;

    public override void SetUp ()
    {
      base.SetUp ();

      _domainObject = Client.GetObject (DomainObjectIDs.Client3);
      _oldRelatedObject = Client.GetObject (DomainObjectIDs.Client1);
      _newRelatedObject = Client.GetObject (DomainObjectIDs.Client2);

      _endPointID = RelationEndPointID.Create (_domainObject, c => c.ParentClient);
      _endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (_endPointID, _oldRelatedObject.ID);

      _command = new ObjectEndPointSetUnidirectionalCommand (_endPoint, _newRelatedObject, OppositeObjectSetter);
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
      var endPoint = new NullObjectEndPoint (ClientTransactionMock, _endPointID.Definition);
      new ObjectEndPointSetUnidirectionalCommand (endPoint, _newRelatedObject, OppositeObjectSetter);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "EndPoint 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order' "
        + "is from a bidirectional relation - use a ObjectEndPointSetOneOneCommand or ObjectEndPointSetOneManyCommand instead.\r\nParameter name: modifiedEndPoint")]
    public void Initialization_Bidirectional_OneMany ()
    {
      var definition = MappingConfiguration.Current.GetTypeDefinition (typeof (OrderItem))
          .GetMandatoryRelationEndPointDefinition (typeof (OrderItem).FullName + ".Order");
      var orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var id = RelationEndPointID.Create(orderItem.ID, definition);

      var endPoint = (IObjectEndPoint) ClientTransactionMock.DataManager.GetRelationEndPointWithLazyLoad (id);
      new ObjectEndPointSetUnidirectionalCommand (endPoint, Order.NewObject (), mi => { });
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "EndPoint 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order' "
        + "is from a bidirectional relation - use a ObjectEndPointSetOneOneCommand or ObjectEndPointSetOneManyCommand instead.\r\nParameter name: modifiedEndPoint")]
    public void Initialization_Bidirectional_OneOne ()
    {
      var definition = MappingConfiguration.Current.GetTypeDefinition (typeof (OrderTicket))
          .GetMandatoryRelationEndPointDefinition (typeof (OrderTicket).FullName + ".Order");
      var relationEndPointID = RelationEndPointID.Create(OrderTicket.GetObject (DomainObjectIDs.OrderTicket1).ID, definition);
      var endPoint = (IObjectEndPoint) ClientTransactionMock.DataManager.GetRelationEndPointWithLazyLoad (relationEndPointID);
      new ObjectEndPointSetUnidirectionalCommand (endPoint, Order.NewObject (), mi => { });
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "New related object for EndPoint "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Client.ParentClient' is the same as its old value - use a ObjectEndPointSetSameCommand "
        + "instead.\r\nParameter name: newRelatedObject")]
    public void Initialization_Same ()
    {
      var endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (_endPointID, _oldRelatedObject.ID);
      new ObjectEndPointSetUnidirectionalCommand (endPoint, _oldRelatedObject, mi => { });
    }

    [Test]
    public virtual void Begin ()
    {
      var eventReceiver = new DomainObjectEventReceiver (_domainObject);

      _command.Begin();

      Assert.IsTrue (eventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasRelationChangedEventBeenCalled);
    }

    [Test]
    public void Perform_InvokesPerformRelationChange ()
    {
      Assert.That (OppositeObjectSetterCalled, Is.False);

      _command.Perform ();

      Assert.That (OppositeObjectSetterCalled, Is.True);
      Assert.That (OppositeObjectSetterObject, Is.SameAs (_newRelatedObject));
    }

    [Test]
    public void Perform_TouchesEndPoint ()
    {
      Assert.That (_endPoint.HasBeenTouched, Is.False);

      _command.Perform();

      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public virtual void End ()
    {
      var eventReceiver = new DomainObjectEventReceiver (_domainObject);

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
    public void ExpandToAllRelatedObjects_SetDifferent_Unidirectional ()
    {
      var bidirectionalModification = _command.ExpandToAllRelatedObjects ();
      Assert.That (bidirectionalModification.GetNestedCommands (), Is.EqualTo (new[] { _command }));
    }
  }
}
