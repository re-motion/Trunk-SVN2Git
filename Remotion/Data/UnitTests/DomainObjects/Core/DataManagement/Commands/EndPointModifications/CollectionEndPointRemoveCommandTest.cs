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
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints;
using Rhino.Mocks;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class CollectionEndPointRemoveCommandTest : CollectionEndPointModificationCommandTestBase
  {
    private Order _removedRelatedObject;
    private CollectionEndPointRemoveCommand _command;

    public override void SetUp ()
    {
      base.SetUp();
      
      _removedRelatedObject = Order.GetObject (DomainObjectIDs.Order1);

      _command = new CollectionEndPointRemoveCommand (
          CollectionEndPoint, _removedRelatedObject, CollectionDataMock, EndPointProviderStub, TransactionEventSinkWithMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_command.ModifiedEndPoint, Is.SameAs (CollectionEndPoint));
      Assert.That (_command.OldRelatedObject, Is.SameAs (_removedRelatedObject));
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
      new CollectionEndPointRemoveCommand (endPoint, _removedRelatedObject, CollectionDataMock, EndPointProviderStub, TransactionEventSinkWithMock);
    }

    [Test]
    public void NotifyClientTransactionOfBegin ()
    {
      TransactionEventSinkWithMock
          .ExpectMock (
              mock => mock.RelationChanging (TestableClientTransaction, DomainObject, CollectionEndPoint.Definition, _removedRelatedObject, null))
          .WhenCalled (
              mock => Assert.That (CollectionEventReceiver.RemovingDomainObjects, Is.EqualTo (new[] { _removedRelatedObject }))); // collection got event first

      _command.NotifyClientTransactionOfBegin ();

      TransactionEventSinkWithMock.VerifyMock();
      Assert.That (CollectionEventReceiver.RemovedDomainObjects, Is.Empty); // operation was not finished
    }

    [Test]
    public void NotifyClientTransactionOfEnd ()
    {
      TransactionEventSinkWithMock
          .ExpectMock (
              mock => mock.RelationChanged (TestableClientTransaction, DomainObject, CollectionEndPoint.Definition, _removedRelatedObject, null))
          .WhenCalled (
              mock => Assert.That (CollectionEventReceiver.RemovedDomainObjects, Is.Empty)); // collection gets event later

      _command.NotifyClientTransactionOfEnd ();

      TransactionEventSinkWithMock.VerifyMock ();
      Assert.That (CollectionEventReceiver.RemovedDomainObjects, Is.EqualTo (new[] { _removedRelatedObject })); // collection got event later
      Assert.That (CollectionEventReceiver.RemovingDomainObjects, Is.Empty); // operation was not started
    }

    [Test]
    public void Perform ()
    {
      CollectionDataMock.BackToRecord ();
      CollectionDataMock.Expect (mock => mock.Remove (_removedRelatedObject)).Return (true);
      CollectionDataMock.Replay ();

      _command.Perform();

      CollectionDataMock.VerifyAllExpectations ();

      Assert.That (CollectionEventReceiver.RemovingDomainObjects, Is.Empty); // operation was not started
      Assert.That (CollectionEventReceiver.RemovedDomainObjects, Is.Empty); // operation was not finished
      Assert.That (CollectionEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      var removedEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (_removedRelatedObject.ID, "Customer");
      var removedEndPoint = (IObjectEndPoint) TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (removedEndPointID);
      Assert.That (removedEndPoint, Is.Not.Null);

      EndPointProviderStub.Stub (stub => stub.GetRelationEndPointWithLazyLoad (removedEndPoint.ID)).Return (removedEndPoint);

      var bidirectionalModification = _command.ExpandToAllRelatedObjects ();

      // DomainObject.Orders.Remove (_removedRelatedObject)
      var steps = bidirectionalModification.GetNestedCommands();
      Assert.That (steps.Count, Is.EqualTo (2));

      // _removedRelatedObject.Customer = null
      Assert.That (steps[0], Is.InstanceOf (typeof (RealObjectEndPointRegistrationCommandDecorator)));
      var setCustomerCommand = ((ObjectEndPointSetCommand) ((RealObjectEndPointRegistrationCommandDecorator) steps[0]).DecoratedCommand);
      Assert.That (setCustomerCommand.ModifiedEndPoint, Is.SameAs (removedEndPoint));
      Assert.That (setCustomerCommand.OldRelatedObject, Is.SameAs (DomainObject));
      Assert.That (setCustomerCommand.NewRelatedObject, Is.Null);

      // DomainObject.Orders.Remove (_removedRelatedObject)
      Assert.That (steps[1], Is.SameAs (_command));
    }
  }
}
