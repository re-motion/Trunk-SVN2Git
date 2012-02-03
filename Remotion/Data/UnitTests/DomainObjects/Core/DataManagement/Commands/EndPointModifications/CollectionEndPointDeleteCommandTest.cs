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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class CollectionEndPointDeleteCommandTest : CollectionEndPointModificationCommandTestBase
  {
    private CollectionEndPointDeleteCommand _command;

    public override void SetUp ()
    {
      base.SetUp();

      _command = new CollectionEndPointDeleteCommand (CollectionEndPoint, CollectionDataMock, TransactionEventSinkWithMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_command.ModifiedEndPoint, Is.SameAs (CollectionEndPoint));
      Assert.That (_command.OldRelatedObject, Is.Null);
      Assert.That (_command.NewRelatedObject, Is.Null);
      Assert.That (_command.ModifiedCollection, Is.SameAs (CollectionEndPoint.Collection));
      Assert.That (_command.ModifiedCollectionData, Is.SameAs (CollectionDataMock));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Modified end point is null, a NullEndPointModificationCommand is needed.\r\n"
                                                                      + "Parameter name: modifiedEndPoint")]
    public void Initialization_FromNullEndPoint ()
    {
      var endPoint = new NullCollectionEndPoint (TestableClientTransaction, RelationEndPointID.Definition);
      new CollectionEndPointDeleteCommand (endPoint, CollectionDataMock, TransactionEventSinkWithMock);
    }

    [Test]
    public void NotifyClientTransactionOfBegin_NoEvents()
    {
      TransactionEventSinkWithMock.Replay();
      
      _command.NotifyClientTransactionOfBegin();
    }

    [Test]
    public void NotifyClientTransactionOfEnd_NoEvents ()
    {
      TransactionEventSinkWithMock.Replay ();

      _command.NotifyClientTransactionOfEnd ();
    }

    [Test]
    public void Begin ()
    {
      bool relationChangingCalled = false;
      bool relationChangedCalled = false;

      DomainObject.RelationChanging += (sender, args) => relationChangingCalled = true;
      DomainObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      _command.Begin ();

      Assert.That (relationChangingCalled, Is.False); // object does not get a notification
      Assert.That (relationChangedCalled, Is.False); // operation was not finished
      Assert.That (CollectionEventReceiver.HasDeletingEventBeenCalled, Is.True);
      Assert.That (CollectionEventReceiver.HasDeletedEventBeenCalled, Is.False);
    }

    [Test]
    public void End ()
    {
      bool relationChangingCalled = false;
      bool relationChangedCalled = false;

      DomainObject.RelationChanging += (sender, args) => relationChangingCalled = true;
      DomainObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      _command.End ();

      Assert.That (relationChangingCalled, Is.False); // object does not get a notification
      Assert.That (relationChangedCalled, Is.False); // operation was not finished
      Assert.That (CollectionEventReceiver.HasDeletingEventBeenCalled, Is.False);
      Assert.That (CollectionEventReceiver.HasDeletedEventBeenCalled, Is.True);
    }

    [Test]
    public void Perform ()
    {
      bool relationChangingCalled = false;
      bool relationChangedCalled = false;

      DomainObject.RelationChanging += (sender, args) => relationChangingCalled = true;
      DomainObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      CollectionDataMock.BackToRecord ();
      CollectionDataMock.Expect (mock => mock.Clear());
      CollectionDataMock.Replay ();

      Assert.That (CollectionEndPoint.HasBeenTouched, Is.False);

      _command.Perform ();

      CollectionDataMock.VerifyAllExpectations ();

      Assert.That (relationChangingCalled, Is.False); // operation was not started
      Assert.That (relationChangedCalled, Is.False); // operation was not finished
      Assert.That (CollectionEventReceiver.HasDeletingEventBeenCalled, Is.False); // operation was not started
      Assert.That (CollectionEventReceiver.HasDeletedEventBeenCalled, Is.False); // operation was not finished
      Assert.That (CollectionEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      var bidirectionalModification = _command.ExpandToAllRelatedObjects ();

      var steps = bidirectionalModification.GetNestedCommands();
      Assert.That (steps.Count, Is.EqualTo (1));
      Assert.That (steps[0], Is.SameAs (_command));

    }
  }
}
