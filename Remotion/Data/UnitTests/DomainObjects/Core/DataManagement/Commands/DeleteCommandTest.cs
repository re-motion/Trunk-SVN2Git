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
using System.Collections.ObjectModel;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using Remotion.Data.DomainObjects;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands
{
  [TestFixture]
  public class DeleteCommandTest : ClientTransactionBaseTest
  {
    private Order _order1;
    private DeleteCommand _deleteOrder1Command;

    public override void SetUp ()
    {
      base.SetUp ();

      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _deleteOrder1Command = new DeleteCommand (ClientTransactionMock, _order1);
    }

    [Test]
    public void NotifyClientTransactionOfBegin ()
    {
      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      _deleteOrder1Command.NotifyClientTransactionOfBegin ();

      listenerMock.AssertWasCalled (mock => mock.ObjectDeleting (_order1));
    }

    [Test]
    public void NotifyClientTransactionOfEnd ()
    {
      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      _deleteOrder1Command.NotifyClientTransactionOfEnd ();

      listenerMock.AssertWasCalled (mock => mock.ObjectDeleted (_order1));
    }

    [Test]
    public void Begin_CallsOnDeleting ()
    {
      var eventReceiver = new DomainObjectEventReceiver (_order1);
      Assert.That (eventReceiver.HasDeletingEventBeenCalled, Is.False);

      _deleteOrder1Command.Begin ();

      Assert.That (eventReceiver.HasDeletingEventBeenCalled, Is.True);
      Assert.That (eventReceiver.HasDeletedEventBeenCalled, Is.False);
    }

    [Test]
    [Ignore ("TODO 1953")]
    public void Begin_TriggersEndPointDeleting ()
    {
      var eventReceiver = new DomainObjectCollectionEventReceiver (_order1.OrderItems);
      Assert.That (eventReceiver.HasDeletingEventBeenCalled, Is.False);

      _deleteOrder1Command.Begin ();

      Assert.That (eventReceiver.HasDeletingEventBeenCalled, Is.True);
      Assert.That (eventReceiver.HasDeletedEventBeenCalled, Is.False);
    }

    [Test]
    [Ignore ("TODO 1953")]
    public void Begin_Sequence ()
    {
      var eventReceiver = new SequenceEventReceiver (new[] { _order1 }, new[] { _order1.OrderItems });
      _deleteOrder1Command.Begin ();

      eventReceiver.Check (new ChangeState[] { 
          new ObjectDeletionState (_order1, "1. _order1.OnDeleting"),
          new CollectionDeletionState (_order1.OrderItems, "2. _order1.OrderItems.OnDeleting")
      });
    }

    [Test]
    public void End_CallsOnDeleted ()
    {
      var eventReceiver = new DomainObjectEventReceiver (_order1);
      Assert.That (eventReceiver.HasDeletedEventBeenCalled, Is.False);

      _deleteOrder1Command.End ();

      Assert.That (eventReceiver.HasDeletedEventBeenCalled, Is.True);
      Assert.That (eventReceiver.HasDeletingEventBeenCalled, Is.False);
    }

    [Test]
    [Ignore ("TODO 1953")]
    public void End_TriggersEndPointDeleted ()
    {
      var eventReceiver = new DomainObjectCollectionEventReceiver (_order1.OrderItems);
      Assert.That (eventReceiver.HasDeletedEventBeenCalled, Is.False);

      _deleteOrder1Command.End ();

      Assert.That (eventReceiver.HasDeletedEventBeenCalled, Is.True);
      Assert.That (eventReceiver.HasDeletingEventBeenCalled, Is.False);
    }

    [Test]
    [Ignore ("TODO 1953")]
    public void End_Sequence ()
    {
      var eventReceiver = new SequenceEventReceiver (new[] { _order1 }, new[] { _order1.OrderItems });
      _deleteOrder1Command.End ();

      eventReceiver.Check (new ChangeState[] { 
          new CollectionDeletionState (_order1.OrderItems, "1. _order1.OrderItems.OnDeleted"),
          new ObjectDeletionState (_order1, "2. _order1.OnDeleted")
      });
    }

    [Test]
    public void Perform_PerformsEndPointDelete ()
    {
      _deleteOrder1Command.Perform ();

      Assert.That (_order1.OrderItems, Is.Empty);
    }

    [Test]
    public void Perform_DeletesExistingDataContainer ()
    {
      _deleteOrder1Command.Perform ();

      Assert.That (_order1.State, Is.EqualTo (StateType.Deleted));
    }

    [Test]
    public void Perform_DiscardsNewDataContainer ()
    {
      var newOrder = Order.NewObject ();
      var deleteNewOrderCommand = new DeleteCommand (ClientTransactionMock, newOrder);

      deleteNewOrderCommand.Perform ();

      Assert.That (newOrder.State, Is.EqualTo (StateType.Discarded));
      Assert.That (ClientTransactionMock.DataManager.IsDiscarded (newOrder.ID), Is.True);
    }

    [Test]
    public void ExtendToAllRelatedObjects ()
    {
      var extended = (CompositeDataManagementCommand) _deleteOrder1Command.ExtendToAllRelatedObjects ();

      var commands = extended.GetCommands();
      Assert.That (commands.Count, Is.EqualTo (6)); // self, Official, OrderTicket, Customer, OrderItem1, OrderItem2

      Assert.That (commands, List.Contains (_deleteOrder1Command));

      var officialCommand = GetEndPointModificationCommand (commands, DomainObjectIDs.Official1);
      Assert.That (officialCommand, Is.Not.Null);
      Assert.That (officialCommand.OldRelatedObject, Is.SameAs (_order1));
      Assert.That (officialCommand.NewRelatedObject, Is.Null);

      var orderTicketCommand = GetEndPointModificationCommand (commands, DomainObjectIDs.OrderTicket1);
      Assert.That (orderTicketCommand, Is.Not.Null);
      Assert.That (orderTicketCommand.OldRelatedObject, Is.SameAs (_order1));
      Assert.That (orderTicketCommand.NewRelatedObject, Is.Null);

      var customerCommand = GetEndPointModificationCommand (commands, DomainObjectIDs.Customer1);
      Assert.That (customerCommand, Is.Not.Null);
      Assert.That (customerCommand.OldRelatedObject, Is.SameAs (_order1));

      var orderItem1Command = GetEndPointModificationCommand (commands, DomainObjectIDs.OrderItem1);
      Assert.That (orderItem1Command, Is.Not.Null);
      Assert.That (orderItem1Command.OldRelatedObject, Is.SameAs (_order1));
      Assert.That (orderItem1Command.NewRelatedObject, Is.Null);

      var orderItem2Command = GetEndPointModificationCommand (commands, DomainObjectIDs.OrderItem2);
      Assert.That (orderItem2Command, Is.Not.Null);
      Assert.That (orderItem2Command.OldRelatedObject, Is.SameAs (_order1));
      Assert.That (orderItem2Command.NewRelatedObject, Is.Null);
    }

    private RelationEndPointModificationCommand GetEndPointModificationCommand (ReadOnlyCollection<IDataManagementCommand> commands, ObjectID objectID)
    {
      return commands.OfType<RelationEndPointModificationCommand>().SingleOrDefault (cmd => cmd.DomainObject.ID == objectID);
    }
  }
}