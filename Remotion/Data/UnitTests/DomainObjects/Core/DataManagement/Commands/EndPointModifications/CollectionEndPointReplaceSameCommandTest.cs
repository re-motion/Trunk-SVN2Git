// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class CollectionEndPointReplaceSameCommandTest : CollectionEndPointModificationCommandTestBase
  {
    private CollectionEndPointReplaceSameCommand _command;
    private Order _replacedRelatedObject;

    public override void SetUp ()
    {
      base.SetUp ();

      _replacedRelatedObject = Order.GetObject (DomainObjectIDs.Order1);

      _command = new CollectionEndPointReplaceSameCommand (CollectionEndPoint, _replacedRelatedObject);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_command.ModifiedEndPoint, Is.SameAs (CollectionEndPoint));
      Assert.That (_command.OldRelatedObject, Is.SameAs (_replacedRelatedObject));
      Assert.That (_command.NewRelatedObject, Is.SameAs (_replacedRelatedObject));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Modified end point is null, a NullEndPointModificationCommand is needed.\r\n"
                                                                      + "Parameter name: modifiedEndPoint")]
    public void Initialization_FromNullEndPoint ()
    {
      var endPoint = new NullCollectionEndPoint (ClientTransactionMock, RelationEndPointID.Definition);
      new CollectionEndPointReplaceSameCommand (endPoint, _replacedRelatedObject);
    }

    [Test]
    public void Begin ()
    {
      bool relationChangingCalled = false;
      bool relationChangedCalled = false;

      DomainObject.RelationChanging += (sender, args) => relationChangingCalled = true;
      DomainObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      _command.Begin ();

      Assert.That (relationChangingCalled, Is.False); // no change notification
      Assert.That (relationChangedCalled, Is.False); // no change notification
      Assert.That (CollectionEventReceiver.AddedDomainObject, Is.Null); // no change notification
      Assert.That (CollectionEventReceiver.RemovedDomainObjects, Is.Empty); // no change notification
    }

    [Test]
    public void End ()
    {
      bool relationChangingCalled = false;
      bool relationChangedCalled = false;

      DomainObject.RelationChanging += (sender, args) => relationChangingCalled = true;
      DomainObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      _command.End ();

      Assert.That (relationChangingCalled, Is.False); // no change notification
      Assert.That (relationChangedCalled, Is.False); // no change notification
      Assert.That (CollectionEventReceiver.RemovingDomainObjects, Is.Empty); // no change notification
      Assert.That (CollectionEventReceiver.RemovedDomainObjects, Is.Empty); // no change notification
      Assert.That (CollectionEventReceiver.AddingDomainObject, Is.Null); // no change notification
      Assert.That (CollectionEventReceiver.AddedDomainObject, Is.Null); // no change notification
    }

    [Test]
    public void NotifyClientTransactionOfBegin ()
    {
      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);

      _command.NotifyClientTransactionOfBegin ();
    }

    [Test]
    public void NotifyClientTransactionOfEnd ()
    {
      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);

      _command.NotifyClientTransactionOfEnd ();
    }

    [Test]
    public void Perform ()
    {
      bool relationChangingCalled = false;
      bool relationChangedCalled = false;

      DomainObject.RelationChanging += (sender, args) => relationChangingCalled = true;
      DomainObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      CollectionDataMock.BackToRecord ();
      CollectionDataMock.Replay ();

      _command.Perform ();

      CollectionDataMock.VerifyAllExpectations ();

      Assert.That (relationChangingCalled, Is.False); // operation was not started
      Assert.That (relationChangedCalled, Is.False); // operation was not finished
      Assert.That (CollectionEventReceiver.AddingDomainObject, Is.Null); // operation was not started
      Assert.That (CollectionEventReceiver.AddedDomainObject, Is.Null); // operation was not finished
      Assert.That (CollectionEventReceiver.RemovingDomainObjects, Is.Empty); // operation was not started
      Assert.That (CollectionEventReceiver.RemovedDomainObjects, Is.Empty); // operation was not finished
      Assert.That (CollectionEndPoint.HasBeenTouched, Is.True);

      Assert.That (CollectionEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      var bidirectionalModification = _command.ExpandToAllRelatedObjects ();

      var relationEndPointID = RelationEndPointID.Create(_replacedRelatedObject.ID, CollectionEndPoint.Definition.GetOppositeEndPointDefinition());
      var oppositeEndPoint = ClientTransactionMock.DataManager.GetRelationEndPointWithLazyLoad (relationEndPointID);

      var steps = bidirectionalModification.GetNestedCommands ();
      Assert.That (steps.Count, Is.EqualTo (2));

      // customer.Orders.Touch()
      Assert.That (steps[0], Is.SameAs (_command));

      // customer.Orders[index].Touch()
      Assert.That (steps[1], Is.InstanceOf (typeof (RelationEndPointTouchCommand)));
      Assert.That (((RelationEndPointTouchCommand) steps[1]).EndPoint, Is.SameAs (oppositeEndPoint));
    }
  }
}
