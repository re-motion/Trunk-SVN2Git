// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class ObjectEndPointSetSameCommandTest : ClientTransactionBaseTest
  {
    private Computer _domainObject;
    private Employee _relatedObject;

    private RelationEndPointID _endPointID;
    private ObjectEndPoint _endPoint;
    private ClientTransactionEventSinkWithMock _transactionEventSinkWithMock;

    private ObjectEndPointSetSameCommand _command;

    public override void SetUp ()
    {
      base.SetUp();

      _domainObject = Computer.GetObject (DomainObjectIDs.Computer1);
      _relatedObject = Employee.GetObject (DomainObjectIDs.Employee3);

      _endPointID = RelationEndPointID.Resolve (_domainObject, c => c.Employee);
      _endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (_endPointID, _relatedObject.ID);
      _transactionEventSinkWithMock = ClientTransactionEventSinkWithMock.CreateWithStrictMock(TestableClientTransaction);

      _command = new ObjectEndPointSetSameCommand (_endPoint, _transactionEventSinkWithMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.AreSame (_endPoint, _command.ModifiedEndPoint);
      Assert.AreSame (_relatedObject, _command.OldRelatedObject);
      Assert.AreSame (_relatedObject, _command.NewRelatedObject);
    }

    [Test]
    public void Perform_TouchesEndPoint ()
    {
      Assert.That (_endPoint.HasBeenTouched, Is.False);
      _command.Perform();
      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void NotifyClientTransactionOfBegin ()
    {
      _transactionEventSinkWithMock.ReplayMock();

      _command.NotifyClientTransactionOfBegin();

      _transactionEventSinkWithMock.AssertWasNotCalledMock (
          mock => mock.RelationChanging (
              Arg<ClientTransaction>.Is.Anything,
              Arg<DomainObject>.Is.Anything,
              Arg<IRelationEndPointDefinition>.Is.Anything,
              Arg<DomainObject>.Is.Anything,
              Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void NotifyClientTransactionOfEnd ()
    {
      _transactionEventSinkWithMock.ReplayMock ();

      _command.NotifyClientTransactionOfBegin();

      _transactionEventSinkWithMock.AssertWasNotCalledMock (
          mock => mock.RelationChanged (
              Arg<ClientTransaction>.Is.Anything,
              Arg<DomainObject>.Is.Anything,
              Arg<IRelationEndPointDefinition>.Is.Anything, 
              Arg<DomainObject>.Is.Anything,
              Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void ExpandToAllRelatedObjects_SetSame_Unidirectional ()
    {
      var client = Client.GetObject (DomainObjectIDs.Client2);
      var unidirectionalEndPointID = RelationEndPointID.Resolve (client, c => c.ParentClient);
      var unidirectionalEndPoint =
          (IObjectEndPoint) TestableClientTransaction.DataManager.GetRelationEndPointWithLazyLoad (unidirectionalEndPointID);
      Assert.That (unidirectionalEndPoint.Definition.GetOppositeEndPointDefinition().IsAnonymous, Is.True);

      var setSameModification = new ObjectEndPointSetSameCommand (unidirectionalEndPoint, _transactionEventSinkWithMock);

      var bidirectionalModification = setSameModification.ExpandToAllRelatedObjects();
      Assert.That (bidirectionalModification.GetNestedCommands(), Is.EqualTo (new[] { setSameModification }));
    }

    [Test]
    public void ExpandToAllRelatedObjects_SetSame_Bidirectional ()
    {
      var oppositeEndPointID = RelationEndPointID.Resolve (_relatedObject, e => e.Computer);
      var oppositeEndPoint = TestableClientTransaction.DataManager.GetRelationEndPointWithLazyLoad (oppositeEndPointID);

      var bidirectionalModification = _command.ExpandToAllRelatedObjects();

      var steps = bidirectionalModification.GetNestedCommands();
      Assert.That (steps.Count, Is.EqualTo (2));

      Assert.That (steps[0], Is.SameAs (_command));

      Assert.That (steps[1], Is.InstanceOf (typeof (RelationEndPointTouchCommand)));
      Assert.That (((RelationEndPointTouchCommand) steps[1]).EndPoint, Is.SameAs (oppositeEndPoint));
    }
  }
}