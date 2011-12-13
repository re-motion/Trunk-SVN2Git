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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using System.Collections.Generic;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class CollectionEndPointSetCollectionCommandTest : CollectionEndPointModificationCommandTestBase
  {
    private DomainObjectCollection _newCollection;

    private Action<DomainObjectCollection> _collectionSetter;

    private MockRepository _mockRepository;
    private IDomainObjectCollectionManager _collectionManagerMock;

    private CollectionEndPointSetCollectionCommand _command;

    private Order _order1;
    private Order _orderWithoutOrderItem;
    private Order _order2;

    public override void SetUp ()
    {
      base.SetUp();

      _newCollection = new OrderCollection();

      _collectionSetter = collection => CollectionEndPointTestHelper.SetCollection (CollectionEndPoint, collection);

      _mockRepository = new MockRepository ();
      _collectionManagerMock = _mockRepository.StrictMock<IDomainObjectCollectionManager> ();

      _command = new CollectionEndPointSetCollectionCommand (
          CollectionEndPoint, 
          _newCollection,
          _collectionSetter,
          _collectionManagerMock);

      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _orderWithoutOrderItem = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);
      _order2 = Order.GetObject (DomainObjectIDs.Order2);
    }

    public override void TearDown ()
    {
      _mockRepository.BackToRecordAll (); // For Discard
      base.TearDown ();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_command.ModifiedEndPoint, Is.SameAs (CollectionEndPoint));
      Assert.That (_command.OldRelatedObject, Is.Null);
      Assert.That (_command.NewRelatedObject, Is.Null);
      Assert.That (_command.NewCollection, Is.SameAs (_newCollection));
      Assert.That (_command.DomainObjectCollectionManager, Is.SameAs (_collectionManagerMock));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Modified end point is null, a NullEndPointModificationCommand is needed.\r\n"
                                                                      + "Parameter name: modifiedEndPoint")]
    public void Initialization_FromNullEndPoint ()
    {
      var endPoint = new NullCollectionEndPoint (TestableClientTransaction, RelationEndPointID.Definition);
      new CollectionEndPointSetCollectionCommand (endPoint, _newCollection, collection => { }, _collectionManagerMock);
    }

    [Test]
    public void Begin ()
    {
      // _order1 will stay, _orderWithoutOrderItem will be removed

      _newCollection.Add (_order1); // same as existing
      _newCollection.Add (_order2); // new

      var relationChangingEventArgs = new List<RelationChangingEventArgs> ();
      bool relationChangedCalled = false;

      CollectionEventReceiver.Reset();

      DomainObject.RelationChanging += (sender, args) => relationChangingEventArgs.Add (args);
      DomainObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      _command.Begin ();

      Assert.That (CollectionEventReceiver.RemovingDomainObjects, Is.Empty);
      Assert.That (CollectionEventReceiver.AddingDomainObject, Is.Null);

      Assert.That (relationChangingEventArgs.Count, Is.EqualTo (2)); // operation was started
      Assert.That (relationChangedCalled, Is.False); // operation was not finished

      Assert.That (relationChangingEventArgs[0].RelationEndPointDefinition.PropertyName, Is.EqualTo (CollectionEndPoint.Definition.PropertyName));
      Assert.That (relationChangingEventArgs[0].OldRelatedObject, Is.SameAs (_orderWithoutOrderItem));
      Assert.That (relationChangingEventArgs[0].NewRelatedObject, Is.Null);

      Assert.That (relationChangingEventArgs[1].RelationEndPointDefinition.PropertyName, Is.EqualTo (CollectionEndPoint.Definition.PropertyName));
      Assert.That (relationChangingEventArgs[1].OldRelatedObject, Is.Null);
      Assert.That (relationChangingEventArgs[1].NewRelatedObject, Is.SameAs (_order2));
    }

    [Test]
    public void End ()
    {
      // _order1 will stay, _orderWithoutOrderItem will be removed

      _newCollection.Add (_order1); // same as existing
      _newCollection.Add (_order2); // new

      var relationChangedEventArgs = new List<RelationChangedEventArgs> ();
      bool relationChangingCalled = false;

      CollectionEventReceiver.Reset ();

      DomainObject.RelationChanging += (sender, args) => relationChangingCalled = true;
      DomainObject.RelationChanged += (sender, args) => relationChangedEventArgs.Add (args);

      _command.End ();

      Assert.That (CollectionEventReceiver.RemovedDomainObjects, Is.Empty);
      Assert.That (CollectionEventReceiver.AddedDomainObject, Is.Null);

      Assert.That (relationChangedEventArgs.Count, Is.EqualTo (2)); // operation was started
      Assert.That (relationChangingCalled, Is.False); // operation was not finished

      Assert.That (relationChangedEventArgs[0].RelationEndPointDefinition.PropertyName, Is.EqualTo (CollectionEndPoint.Definition.PropertyName));

      Assert.That (relationChangedEventArgs[1].RelationEndPointDefinition.PropertyName, Is.EqualTo (CollectionEndPoint.Definition.PropertyName));
    }

    [Test]
    public void NotifyClientTransactionOfBegin ()
    {
      // _order1 will stay, _orderWithoutOrderItem will be removed
      
      _newCollection.Add (_order1); // same as existing
      _newCollection.Add (_order2); // new
      
      var listenerMock = _mockRepository.StrictMock<IClientTransactionListener> ();
      listenerMock.Expect (mock => mock.RelationChanging (
          TestableClientTransaction, 
          DomainObject, 
          CollectionEndPoint.Definition, 
          _orderWithoutOrderItem, 
          null));
      listenerMock.Expect (mock => mock.RelationChanging (
          TestableClientTransaction, 
          DomainObject, 
          CollectionEndPoint.Definition, 
          null, 
          _order2));
      listenerMock.Replay ();

      TestableClientTransaction.AddListener (listenerMock);

      _command.NotifyClientTransactionOfBegin ();

      listenerMock.VerifyAllExpectations ();
    }

    [Test]
    public void NotifyClientTransactionOfEnd ()
    {
      // _order1 will stay, _orderWithoutOrderItem will be removed

      _newCollection.Add (_order1); // same as existing
      _newCollection.Add (_order2); // new

      var listenerMock = _mockRepository.StrictMock<IClientTransactionListener> ();
      listenerMock.Expect (mock => mock.RelationChanged (
          TestableClientTransaction, 
          DomainObject, 
          CollectionEndPoint.Definition));
      listenerMock.Expect (mock => mock.RelationChanged (
          TestableClientTransaction, 
          DomainObject, 
          CollectionEndPoint.Definition));

      listenerMock.Replay ();

      TestableClientTransaction.AddListener (listenerMock);

      _command.NotifyClientTransactionOfEnd();

      listenerMock.VerifyAllExpectations ();
    }

    [Test]
    public void Perform ()
    {
      _newCollection.Add (_order1);

      bool relationChangingCalled = false;
      bool relationChangedCalled = false;

      DomainObject.RelationChanging += (sender, args) => relationChangingCalled = true;
      DomainObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      _collectionManagerMock.Expect (mock => mock.AssociateCollectionWithEndPoint (CollectionEndPoint, _newCollection));

      _mockRepository.ReplayAll ();
      
      _command.Perform ();

      _mockRepository.VerifyAll ();

      Assert.That (relationChangingCalled, Is.False); // operation was not started
      Assert.That (relationChangedCalled, Is.False); // operation was not finished

      Assert.That (CollectionEndPoint.Collection, Is.SameAs (_newCollection));
      Assert.That (CollectionEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      // _order1 will stay, _orderWithoutOrderItem will be removed

      Assert.That (_order1.Customer, Is.SameAs (CollectionEndPoint.GetDomainObject ()));
      Assert.That (_orderWithoutOrderItem.Customer, Is.SameAs (CollectionEndPoint.GetDomainObject ()));

      var customer3 = Customer.GetObject (DomainObjectIDs.Customer3);
      Assert.That (_order2.Customer, Is.SameAs (customer3));

      _newCollection.Add (_order1);
      _newCollection.Add (_order2);
      
      var bidirectionalModification = _command.ExpandToAllRelatedObjects ();

      // DomainObject.Orders = _newCollection

      var steps = bidirectionalModification.GetNestedCommands();
      Assert.That (steps.Count, Is.EqualTo (4));

      // orderWithoutOrderItem.Customer = null;
      // order2.Customer.Orders.Remove (order2);
      // order2.Customer = DomainObject;
      // DomainObject.Orders = _newCollection

      // orderWithoutOrderItem.Customer = null;
      Assert.That (steps[0], Is.InstanceOf (typeof (RealObjectEndPointRegistrationCommandDecorator)));
      var setOrderWithoutOrderItemCustomerCommand = ((ObjectEndPointSetCommand) ((RealObjectEndPointRegistrationCommandDecorator) steps[0]).DecoratedCommand);
      Assert.That (setOrderWithoutOrderItemCustomerCommand.ModifiedEndPoint.ID.Definition.PropertyName, Is.EqualTo (typeof (Order).FullName + ".Customer"));
      Assert.That (setOrderWithoutOrderItemCustomerCommand.ModifiedEndPoint.ID.ObjectID, Is.EqualTo (_orderWithoutOrderItem.ID));
      Assert.That (setOrderWithoutOrderItemCustomerCommand.OldRelatedObject, Is.SameAs (DomainObject));
      Assert.That (setOrderWithoutOrderItemCustomerCommand.NewRelatedObject, Is.Null);

      // order2.Customer.Orders.Remove (order2);
      Assert.That (steps[1], Is.InstanceOf (typeof (CollectionEndPointRemoveCommand)));
      var order2CustomerOrdersRemoveCommand = (CollectionEndPointRemoveCommand) steps[1];
      Assert.That (order2CustomerOrdersRemoveCommand.ModifiedEndPoint.ID.Definition.PropertyName, Is.EqualTo (typeof (Customer).FullName + ".Orders"));
      Assert.That (order2CustomerOrdersRemoveCommand.ModifiedEndPoint.ID.ObjectID, Is.EqualTo (customer3.ID));
      Assert.That (order2CustomerOrdersRemoveCommand.OldRelatedObject, Is.SameAs (_order2));
      Assert.That (order2CustomerOrdersRemoveCommand.NewRelatedObject, Is.Null);

      // order2.Customer = DomainObject
      Assert.That (steps[2], Is.InstanceOf (typeof (RealObjectEndPointRegistrationCommandDecorator)));
      var setOrder2CustomerCommand = ((ObjectEndPointSetCommand) ((RealObjectEndPointRegistrationCommandDecorator) steps[2]).DecoratedCommand);
      Assert.That (setOrder2CustomerCommand.ModifiedEndPoint.ID.Definition.PropertyName, Is.EqualTo (typeof (Order).FullName + ".Customer"));
      Assert.That (setOrder2CustomerCommand.ModifiedEndPoint.ID.ObjectID, Is.EqualTo (_order2.ID));
      Assert.That (setOrder2CustomerCommand.OldRelatedObject, Is.SameAs (customer3));
      Assert.That (setOrder2CustomerCommand.NewRelatedObject, Is.SameAs (DomainObject));

      // DomainObject.Orders = _newCollection
      Assert.That (steps[3], Is.SameAs (_command));
    }
  }
}
