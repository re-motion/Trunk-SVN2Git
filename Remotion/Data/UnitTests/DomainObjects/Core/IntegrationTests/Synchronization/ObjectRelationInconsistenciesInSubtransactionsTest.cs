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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Synchronization
{
  [TestFixture]
  public class ObjectRelationInconsistenciesInSubtransactionsTest : RelationInconsistenciesTestBase
  {
    [Test]
    public void VirtualEndPointQuery_OneOne_Consistent_RealEndPointLoadedFirst ()
    {
      OrderTicket orderTicket1;
      Order order1;

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        orderTicket1 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
        order1 = Order.GetObject (DomainObjectIDs.Order1);

        Assert.That (orderTicket1.Order, Is.SameAs (order1));
        Assert.That (order1.OrderTicket, Is.SameAs (orderTicket1));

        CheckSyncState (orderTicket1, oi => oi.Order, true);
        CheckSyncState (orderTicket1.Order, o => o.OrderTicket, true);

        Assert.That (orderTicket1.Order, Is.SameAs (order1));
        Assert.That (order1.OrderTicket, Is.SameAs (orderTicket1));

        CheckSyncState (orderTicket1, oi => oi.Order, true);
        CheckSyncState (orderTicket1.Order, o => o.OrderTicket, true);

        // these do nothing
        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Create (orderTicket1, oi => oi.Order));
        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Create (orderTicket1.Order, o => o.OrderTicket));

        CheckSyncState (orderTicket1, oi => oi.Order, true);
        CheckSyncState (orderTicket1.Order, o => o.OrderTicket, true);
      }

      CheckSyncState (orderTicket1, oi => oi.Order, true);
      CheckSyncState (orderTicket1.Order, o => o.OrderTicket, true);
    }

    [Test]
    public void VirtualEndPointQuery_OneOne_Consistent_VirtualEndPointLoadedFirst ()
    {
      Order order1;
      OrderTicket orderTicket1;

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        order1 = Order.GetObject (DomainObjectIDs.Order1);
        order1.OrderTicket.EnsureDataAvailable();
        orderTicket1 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);

        Assert.That (orderTicket1.Order, Is.SameAs (order1));
        Assert.That (order1.OrderTicket, Is.SameAs (orderTicket1));

        CheckSyncState (orderTicket1, oi => oi.Order, true);
        CheckSyncState (orderTicket1.Order, o => o.OrderTicket, true);

        Assert.That (order1.OrderTicket, Is.SameAs (orderTicket1));
        Assert.That (orderTicket1.Order, Is.SameAs (order1));

        CheckSyncState (orderTicket1, oi => oi.Order, true);
        CheckSyncState (orderTicket1.Order, o => o.OrderTicket, true);

        // these do nothing
        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Create (orderTicket1, oi => oi.Order));
        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Create (orderTicket1.Order, o => o.OrderTicket));

        CheckSyncState (orderTicket1, oi => oi.Order, true);
        CheckSyncState (orderTicket1.Order, o => o.OrderTicket, true);
      }

      CheckSyncState (orderTicket1, oi => oi.Order, true);
      CheckSyncState (orderTicket1.Order, o => o.OrderTicket, true);
    }

    [Test]
    [ExpectedException (typeof (LoadConflictException), ExpectedMessage =
        "Cannot load the related 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Computer' of "
        +
        @"'Employee\|51ece39b-f040-45b0-8b72-ad8b45353990\|System.Guid': The database returned related object 'Computer\|.*\|System.Guid', but that "
        + @"object already exists in the current ClientTransaction \(and points to a different object 'null'\).",
        MatchType = MessageMatch.Regex)]
    public void VirtualEndPointQuery_OneOne_ObjectReturned_ThatLocallyPointsToNull ()
    {
      SetDatabaseModifyable();

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        var computer = CreateObjectInDatabaseAndLoad<Computer>();
        Assert.That (computer.Employee, Is.Null);

        var employee = Employee.GetObject (DomainObjectIDs.Employee1); // virtual end point not yet resolved

        SetEmployeeInOtherTransaction (computer.ID, employee.ID);

        // Resolve virtual end point - the database says that computer points to employee, but the transaction says computer points to null!
        Dev.Null = employee.Computer;
      }
    }

    [Test]
    [ExpectedException (typeof (LoadConflictException), ExpectedMessage =
        "Cannot load the related 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Computer' of "
        +
        @"'Employee\|51ece39b-f040-45b0-8b72-ad8b45353990\|System.Guid': The database returned related object 'Computer\|.*\|System.Guid', but that "
        + @"object already exists in the current ClientTransaction \(and points to a different object "
        + @"'Employee\|c3b2bbc3-e083-4974-bac7-9cee1fb85a5e\|System.Guid'\).",
        MatchType = MessageMatch.Regex)]
    public void VirtualEndPointQuery_OneOne_ObjectReturned_ThatLocallyPointsSomewhereElse ()
    {
      SetDatabaseModifyable();

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        var computer = Computer.GetObject (CreateComputerAndSetEmployeeInOtherTransaction (DomainObjectIDs.Employee2));
        Assert.That (computer.Employee.ID, Is.EqualTo (DomainObjectIDs.Employee2));

        var employee = Employee.GetObject (DomainObjectIDs.Employee1); // virtual end point not yet resolved

        SetEmployeeInOtherTransaction (computer.ID, employee.ID);

        // Resolve virtual end point - the database says that computer points to employee, but the transaction says computer points to Employee2!
        Dev.Null = employee.Computer;
      }
    }
  }
}