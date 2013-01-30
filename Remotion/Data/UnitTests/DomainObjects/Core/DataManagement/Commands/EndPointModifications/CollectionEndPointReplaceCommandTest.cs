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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class CollectionEndPointReplaceCommandTest : CollectionEndPointModificationCommandTestBase
  {
    private CollectionEndPointReplaceCommand _command;
    private Order _replacedRelatedObject;
    private Order _replacementRelatedObject;

    public override void SetUp ()
    {
      base.SetUp();

      _replacedRelatedObject = DomainObjectIDs.Order1.GetObject<Order> ();
      _replacementRelatedObject = DomainObjectIDs.Order2.GetObject<Order> ();

      _command =
          new CollectionEndPointReplaceCommand (
              CollectionEndPoint, _replacedRelatedObject, 12, _replacementRelatedObject, CollectionDataMock, TransactionEventSinkWithMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_command.ModifiedEndPoint, Is.SameAs (CollectionEndPoint));
      Assert.That (_command.OldRelatedObject, Is.SameAs (_replacedRelatedObject));
      Assert.That (_command.NewRelatedObject, Is.SameAs (_replacementRelatedObject));
      Assert.That (_command.ModifiedCollection, Is.SameAs (CollectionEndPoint.Collection));
      Assert.That (_command.ModifiedCollectionData, Is.SameAs (CollectionDataMock));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Modified end point is null, a NullEndPointModificationCommand is needed.\r\n"
                                                                      + "Parameter name: modifiedEndPoint")]
    public void Initialization_FromNullEndPoint ()
    {
      var endPoint = new NullCollectionEndPoint (TestableClientTransaction, RelationEndPointID.Definition);
      new CollectionEndPointReplaceCommand (
          endPoint, _replacedRelatedObject, 12, _replacementRelatedObject, CollectionDataMock, TransactionEventSinkWithMock);
    }

    [Test]
    public void Begin ()
    {
      TransactionEventSinkWithMock.Expect (mock => mock.RaiseRelationChangingEvent (
          DomainObject, CollectionEndPoint.Definition, _replacedRelatedObject, _replacementRelatedObject))
          .WhenCalled (
              mock =>
              {
                Assert.That (CollectionEventReceiver.RemovingDomainObjects, Is.EqualTo (new[] { _replacedRelatedObject })); // collection got event first
                Assert.That (CollectionEventReceiver.AddingDomainObject, Is.SameAs (_replacementRelatedObject)); // collection got event first
              });

      _command.Begin ();

      TransactionEventSinkWithMock.VerifyAllExpectations();
      Assert.That (CollectionEventReceiver.AddedDomainObject, Is.Null); // operation was not finished
      Assert.That (CollectionEventReceiver.RemovedDomainObjects, Is.Empty); // operation was not finished
    }

    [Test]
    public void End ()
    {
      TransactionEventSinkWithMock.Expect (mock => mock.RaiseRelationChangedEvent (
          DomainObject, CollectionEndPoint.Definition, _replacedRelatedObject, _replacementRelatedObject))
          .WhenCalled (
              mock =>
              {
                Assert.That (CollectionEventReceiver.AddedDomainObject, Is.Null); // collection gets event later
                Assert.That (CollectionEventReceiver.RemovedDomainObjects, Is.Empty); // collection gets event later
              });

      _command.End ();

      TransactionEventSinkWithMock.VerifyAllExpectations();
      Assert.That (CollectionEventReceiver.RemovedDomainObjects, Is.EqualTo (new[] { _replacedRelatedObject })); // collection got event later
      Assert.That (CollectionEventReceiver.AddedDomainObject, Is.SameAs (_replacementRelatedObject)); // collection got event later
      Assert.That (CollectionEventReceiver.RemovingDomainObjects, Is.Empty); // operation was not started
      Assert.That (CollectionEventReceiver.AddingDomainObject, Is.Null); // operation was not started
    }

    [Test]
    public void Perform ()
    {
      CollectionDataMock.BackToRecord ();
      CollectionDataMock.Expect (mock => mock.Replace (12, _replacementRelatedObject));
      CollectionDataMock.Replay ();

      _command.Perform ();

      CollectionDataMock.VerifyAllExpectations ();

      Assert.That (CollectionEventReceiver.AddingDomainObject, Is.Null); // operation was not started
      Assert.That (CollectionEventReceiver.AddedDomainObject, Is.Null); // operation was not finished
      Assert.That (CollectionEventReceiver.RemovingDomainObjects, Is.Empty); // operation was not started
      Assert.That (CollectionEventReceiver.RemovedDomainObjects, Is.Empty); // operation was not finished
      Assert.That (CollectionEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      var bidirectionalModification = _command.ExpandToAllRelatedObjects ();

      // DomainObject.Orders[indexof (_replacedRelatedObject)] = _replacementRelatedObject
      var steps = bidirectionalModification.GetNestedCommands();
      Assert.That (steps.Count, Is.EqualTo (4));

      var oldCustomer = _replacementRelatedObject.Customer;

      // DomainObject.Orders[...].Customer = null
      Assert.That (steps[0], Is.InstanceOf (typeof (RealObjectEndPointRegistrationCommandDecorator)));
      var setReplacedOrderCustomerCommand = ((ObjectEndPointSetCommand) ((RealObjectEndPointRegistrationCommandDecorator) steps[0]).DecoratedCommand);
      Assert.That (setReplacedOrderCustomerCommand.ModifiedEndPoint.ID.Definition.PropertyName, Is.EqualTo (typeof (Order).FullName + ".Customer"));
      Assert.That (setReplacedOrderCustomerCommand.ModifiedEndPoint.ID.ObjectID, Is.EqualTo (_replacedRelatedObject.ID));
      Assert.That (setReplacedOrderCustomerCommand.OldRelatedObject, Is.SameAs (DomainObject));
      Assert.That (setReplacedOrderCustomerCommand.NewRelatedObject, Is.Null);

      // _replacementRelatedObject.Customer = DomainObject
      Assert.That (steps[1], Is.InstanceOf (typeof (RealObjectEndPointRegistrationCommandDecorator)));
      var setReplacementOrderCustomerCommand = ((ObjectEndPointSetCommand) ((RealObjectEndPointRegistrationCommandDecorator) steps[1]).DecoratedCommand);
      Assert.That (setReplacementOrderCustomerCommand.ModifiedEndPoint.ID.Definition.PropertyName, Is.EqualTo (typeof (Order).FullName + ".Customer"));
      Assert.That (setReplacementOrderCustomerCommand.ModifiedEndPoint.ID.ObjectID, Is.EqualTo (_replacementRelatedObject.ID));
      Assert.That (setReplacementOrderCustomerCommand.OldRelatedObject, Is.SameAs (oldCustomer));
      Assert.That (setReplacementOrderCustomerCommand.NewRelatedObject, Is.SameAs (DomainObject));

      // DomainObject.Orders[...] = _replacementRelatedObject
      Assert.That (steps[2], Is.SameAs (_command));

      // oldCustomer.Orders.Remove (_replacementRelatedObject)
      Assert.That (steps[3], Is.TypeOf<VirtualEndPointStateUpdatedRaisingCommandDecorator> ());
      var oldCustomerOrdersRemoveCommand = ((CollectionEndPointRemoveCommand) ((VirtualEndPointStateUpdatedRaisingCommandDecorator) steps[3]).DecoratedCommand);
      Assert.That (oldCustomerOrdersRemoveCommand.ModifiedEndPoint.ID.Definition.PropertyName, Is.EqualTo (typeof (Customer).FullName + ".Orders"));
      Assert.That (oldCustomerOrdersRemoveCommand.ModifiedEndPoint.ID.ObjectID, Is.EqualTo (oldCustomer.ID));
      Assert.That (oldCustomerOrdersRemoveCommand.OldRelatedObject, Is.SameAs (_replacementRelatedObject));
    }
  }
}
