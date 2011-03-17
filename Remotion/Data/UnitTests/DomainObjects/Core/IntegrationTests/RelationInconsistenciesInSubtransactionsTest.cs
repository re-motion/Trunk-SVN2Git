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
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using System.Linq;
using Remotion.Reflection;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  public class RelationInconsistenciesInSubtransactionsTest : ClientTransactionBaseTest
  {
    [Test]
    public void VirtualEndPointQuery_OneMany_Consistent_ObjectLoadedFirst ()
    {
      OrderItem orderItem1;
      Order order1;

      // Sub-transaction loading OrderItem.Order before Order.OrderItems
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
        order1 = Order.GetObject (DomainObjectIDs.Order1);
        order1.OrderItems.EnsureDataComplete();

        Assert.That (orderItem1.Order, Is.SameAs (order1));
        Assert.That (order1.OrderItems, List.Contains (orderItem1));

        CheckSyncState (orderItem1, oi => oi.Order, true);
        CheckSyncState (orderItem1.Order, o => o.OrderItems, true);

        Assert.That (orderItem1.Order, Is.SameAs (order1));
        Assert.That (order1.OrderItems, List.Contains (orderItem1));

        CheckSyncState (orderItem1, oi => oi.Order, true);
        CheckSyncState (orderItem1.Order, o => o.OrderItems, true);

        // these do nothing
        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Create (orderItem1, oi => oi.Order));
        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Create (orderItem1.Order, o => o.OrderItems));

        CheckSyncState (orderItem1, oi => oi.Order, true);
        CheckSyncState (orderItem1.Order, o => o.OrderItems, true);
      }

      CheckSyncState (orderItem1, oi => oi.Order, true);
      CheckSyncState (orderItem1.Order, o => o.OrderItems, true);
    }

    [Test]
    public void VirtualEndPointQuery_OneMany_Consistent_CollectionLoadedFirst ()
    {
      Order order1;
      OrderItem orderItem1;

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        order1 = Order.GetObject (DomainObjectIDs.Order1);
        order1.OrderItems.EnsureDataComplete();
        orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);

        Assert.That (orderItem1.Order, Is.SameAs (order1));
        Assert.That (order1.OrderItems, List.Contains (orderItem1));

        CheckSyncState (orderItem1, oi => oi.Order, true);
        CheckSyncState (orderItem1.Order, o => o.OrderItems, true);

        Assert.That (order1.OrderItems, List.Contains (orderItem1));
        Assert.That (orderItem1.Order, Is.SameAs (order1));

        CheckSyncState (orderItem1, oi => oi.Order, true);
        CheckSyncState (orderItem1.Order, o => o.OrderItems, true);

        // these do nothing
        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Create (orderItem1, oi => oi.Order));
        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Create (orderItem1.Order, o => o.OrderItems));

        CheckSyncState (orderItem1, oi => oi.Order, true);
        CheckSyncState (orderItem1.Order, o => o.OrderItems, true);
      }

      CheckSyncState (orderItem1, oi => oi.Order, true);
      CheckSyncState (orderItem1.Order, o => o.OrderItems, true);
    }

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

    [Test]
    public void VirtualEndPointQuery_OneMany_ObjectIncluded_ThatLocallyPointsToSomewhereElse ()
    {
      Company company;
      IndustrialSector industrialSector; // virtual end point not yet resolved

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        PrepareInconsistentState_OneMany_ObjectIncluded (out company, out industrialSector);

        CheckSyncState (company, c => c.IndustrialSector, true);
        CheckSyncState (industrialSector, s => s.Companies, false);

        var otherCompany = industrialSector.Companies.FirstOrDefault (c => c != company);
        CheckSyncState (otherCompany, c => c.IndustrialSector, true);

        CheckActionWorks (company.Delete);
        ClientTransaction.Current.Rollback(); // required so that the remaining actions can be tried below

        // sync states not changed by Rollback
        CheckSyncState (company, c => c.IndustrialSector, true);
        CheckSyncState (industrialSector, s => s.Companies, false);

        CheckActionWorks (() => industrialSector.Companies.Remove (otherCompany));
        CheckActionWorks (() => industrialSector.Companies.Add (Company.NewObject()));

        var companyIndex = industrialSector.Companies.IndexOf (company);
        CheckActionThrows<InvalidOperationException> (
            () => industrialSector.Companies.Remove (company), "out of sync with the opposite object property");
        CheckActionThrows<InvalidOperationException> (
            () => industrialSector.Companies[companyIndex] = Company.NewObject(), "out of sync with the opposite object property");
        CheckActionThrows<InvalidOperationException> (
            () => industrialSector.Companies = new ObjectList<Company>(), "out of sync with the opposite object property");
        CheckActionThrows<InvalidOperationException> (industrialSector.Delete, "out of sync with the opposite object property");

        CheckActionWorks (() => company.IndustrialSector = IndustrialSector.NewObject());

        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Create (industrialSector, s => s.Companies));

        CheckSyncState (industrialSector, s => s.Companies, true);
        Assert.That (industrialSector.Companies, List.Not.Contains (company));

        CheckActionWorks (() => industrialSector.Companies.Add (company));
      }

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector, s => s.Companies, true);

      Assert.That (company.IndustrialSector, Is.Null);
      Assert.That (industrialSector.Companies, List.Not.Contains (company));
    }

    [Test]
    public void VirtualEndPointQuery_OneMany_ObjectIncluded_ThatLocallyPointsToSomewhereElse_SolvableViaReload ()
    {
      Company company;
      IndustrialSector industrialSector; // virtual end point not yet resolved

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        PrepareInconsistentState_OneMany_ObjectIncluded (out company, out industrialSector);

        Assert.That (company.IndustrialSector, Is.Null);
        Assert.That (industrialSector.Companies, List.Contains (company));
        CheckSyncState (industrialSector, s => s.Companies, false);

        UnloadService.UnloadData (ClientTransaction.Current, company.ID, UnloadTransactionMode.RecurseToRoot);
        company.EnsureDataAvailable();

        CheckSyncState (industrialSector, s => s.Companies, true);
        Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
        Assert.That (industrialSector.Companies, List.Contains (company));

        CheckActionWorks (() => industrialSector.Companies.Remove (company));
      }

      CheckSyncState (industrialSector, s => s.Companies, true);
      Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
      Assert.That (industrialSector.Companies, List.Contains (company));
    }

    [Test]
    public void VirtualEndPointQuery_OneMany_ObjectNotIncluded_ThatLocallyPointsToHere ()
    {
      Company company;
      IndustrialSector industrialSector;

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        PrepareInconsistentState_OneMany_ObjectNotIncluded (out company, out industrialSector);

        Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
        Assert.That (industrialSector.Companies, List.Not.Contains (company));

        CheckSyncState (company, c => c.IndustrialSector, false);
        CheckSyncState (industrialSector, s => s.Companies, true);
        CheckSyncState (industrialSector.Companies[0], c => c.IndustrialSector, true);

        CheckActionThrows<InvalidOperationException> (company.Delete, "out of sync with the opposite property");
        CheckActionThrows<InvalidOperationException> (industrialSector.Delete, "out of sync with the collection property");

        CheckActionWorks (() => industrialSector.Companies.RemoveAt (0));
        CheckActionWorks (() => industrialSector.Companies.Add (Company.NewObject()));

        CheckActionThrows<InvalidOperationException> (() => industrialSector.Companies.Add (company), "out of sync with the collection property");
        CheckActionThrows<InvalidOperationException> (() => company.IndustrialSector = null, "out of sync with the opposite property ");

        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Create (company, c => c.IndustrialSector));

        CheckSyncState (company, c => c.IndustrialSector, true);
        Assert.That (industrialSector.Companies, List.Contains (company));
        CheckActionWorks (() => company.IndustrialSector = null);
        CheckActionWorks (() => industrialSector.Companies.Add (company));
      }

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector, s => s.Companies, true);

      Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
      Assert.That (industrialSector.Companies, List.Contains (company));
    }

    [Test]
    public void VirtualEndPointQuery_OneMany_ObjectIncludedInTwoCollections ()
    {
      SetDatabaseModifyable();

      Company company;
      IndustrialSector industrialSector1;
      IndustrialSector industrialSector2;
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        var companyID = CreateCompanyAndSetIndustrialSectorInOtherTransaction (DomainObjectIDs.IndustrialSector1);
        company = Company.GetObject (companyID);

        industrialSector1 = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1);
        industrialSector1.Companies.EnsureDataComplete();

        SetIndustrialSectorInOtherTransaction (company.ID, DomainObjectIDs.IndustrialSector2);

        industrialSector2 = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector2);
        industrialSector2.Companies.EnsureDataComplete();

        Assert.That (company.IndustrialSector, Is.SameAs (industrialSector1));
        Assert.That (industrialSector1.Companies, List.Contains (company));
        Assert.That (industrialSector2.Companies, List.Contains (company));

        CheckSyncState (company, c => c.IndustrialSector, true);
        CheckSyncState (industrialSector1, s => s.Companies, true);
        CheckSyncState (industrialSector2, s => s.Companies, false);

        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Create (industrialSector2, s => s.Companies));

        Assert.That (company.IndustrialSector, Is.SameAs (industrialSector1));
        Assert.That (industrialSector1.Companies, List.Contains (company));
        Assert.That (industrialSector2.Companies, List.Not.Contains (company));

        CheckSyncState (company, c => c.IndustrialSector, true);
        CheckSyncState (industrialSector1, s => s.Companies, true);
        CheckSyncState (industrialSector2, s => s.Companies, true);
      }

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector1, s => s.Companies, true);
      CheckSyncState (industrialSector2, s => s.Companies, true);

      Assert.That (company.IndustrialSector, Is.SameAs (industrialSector1));
      Assert.That (industrialSector1.Companies, List.Contains (company));
      Assert.That (industrialSector2.Companies, List.Not.Contains (company));
    }

    [Test]
    public void ConsistentState_GuaranteedInSubTransaction_OneMany ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      order.OrderItems.EnsureDataComplete();

      var orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);

      Assert.That (order.OrderItems, List.Contains (orderItem));
      Assert.That (orderItem.Order, Is.SameAs (order));
      CheckSyncState (order, o => o.OrderItems, true);
      CheckSyncState (orderItem, oi => oi.Order, true);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (order.OrderItems, List.Contains (orderItem));
        Assert.That (orderItem.Order, Is.SameAs (order));
        CheckSyncState (order, o => o.OrderItems, true);
        CheckSyncState (orderItem, oi => oi.Order, true);
      }

      order.OrderItems.Remove (orderItem);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (order.OrderItems, List.Not.Contains (orderItem));
        Assert.That (orderItem.Order, Is.Null);
        CheckSyncState (order, o => o.OrderItems, true);
        CheckSyncState (orderItem, oi => oi.Order, true);
      }
      ClientTransaction.Current.Rollback();

      orderItem.Order = null;

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (order.OrderItems, List.Not.Contains (orderItem));
        Assert.That (orderItem.Order, Is.Null);
        CheckSyncState (order, o => o.OrderItems, true);
        CheckSyncState (orderItem, oi => oi.Order, true);
      }
    }

    [Test]
    public void InconsistentState_GuaranteedInSubTransaction_OneMany_ObjectIncluded ()
    {
      Company company;
      IndustrialSector industrialSector;
      PrepareInconsistentState_OneMany_ObjectIncluded (out company, out industrialSector);

      Assert.That (company.IndustrialSector, Is.Null);
      Assert.That (industrialSector.Companies, List.Contains (company));

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector, s => s.Companies, false);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (company.IndustrialSector, Is.Null);
        Assert.That (industrialSector.Companies, List.Contains (company));
        CheckSyncState (company, c => c.IndustrialSector, true);
        CheckSyncState (industrialSector, s => s.Companies, false);
      }

      CheckActionThrows<InvalidOperationException> (() => industrialSector.Companies.Remove (company), "out of sync");

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (company.IndustrialSector, Is.Null);
        Assert.That (industrialSector.Companies, List.Contains (company));
        CheckSyncState (company, c => c.IndustrialSector, true);
        CheckSyncState (industrialSector, s => s.Companies, false);
      }

      CheckActionThrows<InvalidOperationException> (() => company.IndustrialSector = industrialSector, "out of sync");

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (company.IndustrialSector, Is.Null);
        Assert.That (industrialSector.Companies, List.Contains (company));
        CheckSyncState (company, c => c.IndustrialSector, true);
        CheckSyncState (industrialSector, s => s.Companies, false);
      }
    }

    [Test]
    public void InconsistentState_GuaranteedInSubTransaction_OneMany_ObjectNotIncluded ()
    {
      Company company;
      IndustrialSector industrialSector;
      PrepareInconsistentState_OneMany_ObjectNotIncluded (out company, out industrialSector);

      Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
      Assert.That (industrialSector.Companies, List.Not.Contains (company));
      CheckSyncState (company, c => c.IndustrialSector, false);
      CheckSyncState (industrialSector, s => s.Companies, true);

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
        Assert.That (industrialSector.Companies, List.Not.Contains (company));
        CheckSyncState (company, c => c.IndustrialSector, false);
        CheckSyncState (industrialSector, s => s.Companies, true);
      }

      CheckActionThrows<InvalidOperationException> (() => industrialSector.Companies.Add (company), "out of sync");

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
        Assert.That (industrialSector.Companies, List.Not.Contains (company));
        CheckSyncState (company, c => c.IndustrialSector, false);
        CheckSyncState (industrialSector, s => s.Companies, true);
      }

      CheckActionThrows<InvalidOperationException> (() => company.IndustrialSector = null, "out of sync");

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
        Assert.That (industrialSector.Companies, List.Not.Contains (company));
        CheckSyncState (company, c => c.IndustrialSector, false);
        CheckSyncState (industrialSector, s => s.Companies, true);
      }
    }

    [Test]
    public void Commit_InSubTransactionSubTransaction_InconsistentState_OneMany_ObjectIncluded ()
    {
      Company company;
      IndustrialSector industrialSector;
      PrepareInconsistentState_OneMany_ObjectIncluded (out company, out industrialSector);

      Assert.That (company.IndustrialSector, Is.Null);
      Assert.That (industrialSector.Companies, List.Contains (company));

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector, s => s.Companies, false);

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (company.IndustrialSector, Is.Null);
        Assert.That (industrialSector.Companies.Count, Is.EqualTo (6));

        CheckActionThrows<InvalidOperationException> (() => company.IndustrialSector = industrialSector, "out of sync");
        company.IndustrialSector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector2);
        industrialSector.Companies.Insert (0, Company.GetObject (DomainObjectIDs.Company2));
        CheckActionThrows<InvalidOperationException> (() => industrialSector.Companies.Remove (company), "out of sync");

        Assert.That (company.IndustrialSector, Is.SameAs (IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector2)));
        Assert.That (industrialSector.Companies.Count, Is.EqualTo (7));

        ClientTransaction.Current.Commit();

        CheckSyncState (company, c => c.IndustrialSector, true);
        CheckSyncState (industrialSector, s => s.Companies, false);

        Assert.That (company.IndustrialSector, Is.SameAs (IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector2)));
        Assert.That (industrialSector.Companies.Count, Is.EqualTo (7));
      }

      Assert.That (company.IndustrialSector, Is.SameAs (IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector2)));
      Assert.That (industrialSector.Companies.Count, Is.EqualTo (7));

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector, s => s.Companies, false);
    }

    [Test]
    public void Commit_InSubTransactionSubTransaction_InconsistentState_OneMany_ObjectNotIncluded ()
    {
      Company company;
      IndustrialSector industrialSector;
      PrepareInconsistentState_OneMany_ObjectNotIncluded(out company, out industrialSector);

      Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
      Assert.That (industrialSector.Companies, List.Not.Contains (company));

      CheckSyncState (company, c => c.IndustrialSector, false);
      CheckSyncState (industrialSector, s => s.Companies, true);

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
        Assert.That (industrialSector.Companies.Count, Is.EqualTo (5));

        CheckActionThrows<InvalidOperationException> (() => company.IndustrialSector = null, "out of sync");
        industrialSector.Companies.Add (Company.GetObject (DomainObjectIDs.Company1));
        CheckActionThrows<InvalidOperationException> (() => industrialSector.Companies.Add (company), "out of sync");

        Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
        Assert.That (industrialSector.Companies.Count, Is.EqualTo (6));

        ClientTransaction.Current.Commit();

        Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
        Assert.That (industrialSector.Companies.Count, Is.EqualTo (6));

        CheckSyncState (company, c => c.IndustrialSector, false);
        CheckSyncState (industrialSector, s => s.Companies, true);
      }

      Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
      Assert.That (industrialSector.Companies.Count, Is.EqualTo (6));

      CheckSyncState (company, c => c.IndustrialSector, false);
      CheckSyncState (industrialSector, s => s.Companies, true);
    }

    [Test]
    public void Synchronize_WithSubtransactions_LoadedInRootOnly_SyncedInRoot ()
    {
      Company company;
      IndustrialSector industrialSector;
      PrepareInconsistentState_OneMany_ObjectIncluded (out company, out industrialSector);

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector, s => s.Companies, false);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        var relationEndPointID = RelationEndPointID.Create (industrialSector, s => s.Companies);
        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current.ParentTransaction, relationEndPointID);

        CheckSyncState (industrialSector, s => s.Companies, true);

        var dataManager = ClientTransactionTestHelper.GetDataManager (ClientTransaction.Current);
        Assert.That (dataManager.GetRelationEndPointWithoutLoading (relationEndPointID), Is.Null);

        var endPoint = dataManager.GetRelationEndPointWithLazyLoad (relationEndPointID);
        Assert.That (endPoint.IsSynchronized, Is.True);
        CheckSyncState (industrialSector, s => s.Companies, true);
      }

      CheckSyncState (industrialSector, s => s.Companies, true);
    }

    [Test]
    public void Synchronize_WithSubtransactions_LoadedInRootOnly_SyncedInSub ()
    {
      Company company;
      IndustrialSector industrialSector;
      PrepareInconsistentState_OneMany_ObjectIncluded (out company, out industrialSector);

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector, s => s.Companies, false);

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var relationEndPointID = RelationEndPointID.Create (industrialSector, s => s.Companies);
        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, relationEndPointID);

        CheckSyncState (company, c => c.IndustrialSector, true);

        var dataManager = ClientTransactionTestHelper.GetDataManager (ClientTransaction.Current);
        Assert.That (dataManager.GetRelationEndPointWithoutLoading (relationEndPointID), Is.Null);

        var endPoint = dataManager.GetRelationEndPointWithLazyLoad (relationEndPointID);
        Assert.That (endPoint.IsSynchronized, Is.True);
        CheckSyncState (company, c => c.IndustrialSector, true);
      }

      CheckSyncState (company, c => c.IndustrialSector, true);
    }

    [Test]
    public void Synchronize_WithSubtransactions_LoadedInRootAndSub_SyncedInRoot ()
    {
      Company company;
      IndustrialSector industrialSector;
      PrepareInconsistentState_OneMany_ObjectIncluded (out company, out industrialSector);

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector, s => s.Companies, false);

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var relationEndPointID = RelationEndPointID.Create (industrialSector, s => s.Companies);

        var dataManager = ClientTransactionTestHelper.GetDataManager (ClientTransaction.Current);
        var endPoint = dataManager.GetRelationEndPointWithLazyLoad (relationEndPointID);
        Assert.That (endPoint.IsSynchronized, Is.False);

        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current.ParentTransaction, relationEndPointID);

        CheckSyncState (industrialSector, s => s.Companies, true);
        Assert.That (endPoint.IsSynchronized, Is.True);
      }

      CheckSyncState (industrialSector, s => s.Companies, true);
    }

    [Test]
    public void Synchronize_WithSubtransactions_LoadedInRootAndSub_SyncedInSub ()
    {
      Company company;
      IndustrialSector industrialSector;
      PrepareInconsistentState_OneMany_ObjectIncluded (out company, out industrialSector);

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector, s => s.Companies, false);

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var relationEndPointID = RelationEndPointID.Create (industrialSector, s => s.Companies);

        var dataManager = ClientTransactionTestHelper.GetDataManager (ClientTransaction.Current);
        var endPoint = dataManager.GetRelationEndPointWithLazyLoad (relationEndPointID);
        Assert.That (endPoint.IsSynchronized, Is.False);

        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, relationEndPointID);

        CheckSyncState (industrialSector, s => s.Companies, true);
        Assert.That (endPoint.IsSynchronized, Is.True);
      }

      CheckSyncState (industrialSector, s => s.Companies, true);
    }

    private ObjectID CreateCompanyAndSetIndustrialSectorInOtherTransaction (ObjectID industrialSectorID)
    {
      return DomainObjectMother.CreateObjectAndSetRelationInOtherTransaction<Company, IndustrialSector> (
          industrialSectorID,
          (c, s) =>
          {
            c.IndustrialSector = s;
            c.Ceo = Ceo.NewObject();
          });
    }

    private void SetIndustrialSectorInOtherTransaction (ObjectID companyID, ObjectID industrialSectorID)
    {
      DomainObjectMother.SetRelationInOtherTransaction<Company, IndustrialSector> (companyID, industrialSectorID, (c, s) => c.IndustrialSector = s);
    }

    private ObjectID CreateComputerAndSetEmployeeInOtherTransaction (ObjectID employeeID)
    {
      return DomainObjectMother.CreateObjectAndSetRelationInOtherTransaction<Computer, Employee> (employeeID, (c, e) => c.Employee = e);
    }

    private void SetEmployeeInOtherTransaction (ObjectID computerID, ObjectID employeeID)
    {
      DomainObjectMother.SetRelationInOtherTransaction<Computer, Employee> (computerID, employeeID, (c, e) => c.Employee = e);
    }

    private void CheckSyncState<TOriginating, TRelated> (
        TOriginating originating,
        Expression<Func<TOriginating, TRelated>> propertyAccessExpression,
        bool expectedState)
        where TOriginating: DomainObject
    {
      var transaction = ClientTransaction.Current;
      CheckSyncState (transaction, originating, propertyAccessExpression, expectedState);
    }

    private void CheckSyncState<TOriginating, TRelated> (
        ClientTransaction transaction,
        TOriginating originating,
        Expression<Func<TOriginating, TRelated>> propertyAccessExpression,
        bool expectedState)
        where TOriginating: DomainObject
    {
      Assert.That (
          BidirectionalRelationSyncService.IsSynchronized (transaction, RelationEndPointID.Create (originating, propertyAccessExpression)),
          Is.EqualTo (expectedState));
    }

    private void CheckActionWorks (Action action)
    {
      action();
    }

    private void CheckActionThrows<TException> (Action action, string expectedMessage) where TException: Exception
    {
      var hadException = false;
      try
      {
        action();
      }
      catch (Exception ex)
      {
        hadException = true;
        Assert.That (ex, Is.TypeOf (typeof (TException)));
        Assert.That (
            ex.Message,
            NUnit.Framework.SyntaxHelpers.Text.Contains (expectedMessage),
            "Expected: " + expectedMessage + Environment.NewLine + "Was: " + ex.Message);
      }

      if (!hadException)
        Assert.Fail ("Expected " + typeof (TException).Name);
    }

    private Company CreateCompanyInDatabaseAndLoad ()
    {
      ObjectID objectID;
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var company = Company.NewObject();
        company.Ceo = Ceo.NewObject();
        ClientTransaction.Current.Commit();
        objectID = company.ID;
      }
      return Company.GetObject (objectID);
    }

    private T CreateObjectInDatabaseAndLoad<T> () where T: DomainObject
    {
      ObjectID objectID;
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var domainObject = LifetimeService.NewObject (ClientTransaction.Current, typeof (T), ParamList.Empty);
        ClientTransaction.Current.Commit();
        objectID = domainObject.ID;
      }
      return (T) LifetimeService.GetObject (ClientTransaction.Current, objectID, false);
    }

    private void PrepareInconsistentState_OneMany_ObjectIncluded (out Company company, out IndustrialSector industrialSector)
    {
      SetDatabaseModifyable ();

      company = CreateCompanyInDatabaseAndLoad ();
      Assert.That (company.IndustrialSector, Is.Null);

      industrialSector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1);

      SetIndustrialSectorInOtherTransaction (company.ID, industrialSector.ID);

      // Resolve virtual end point - the database says that company points to industrialSector, but the transaction says it points to null!
      industrialSector.Companies.EnsureDataComplete ();

      Assert.That (company.IndustrialSector, Is.Null);
      Assert.That (industrialSector.Companies, List.Contains (company));

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector, s => s.Companies, false);
    }

    private void PrepareInconsistentState_OneMany_ObjectNotIncluded (out Company company, out IndustrialSector industrialSector)
    {
      SetDatabaseModifyable ();

      var companyID = CreateCompanyAndSetIndustrialSectorInOtherTransaction (DomainObjectIDs.IndustrialSector1);
      company = Company.GetObject (companyID);

      Assert.That (company.Properties[typeof (Company), "IndustrialSector"].GetRelatedObjectID (), Is.EqualTo (DomainObjectIDs.IndustrialSector1));

      SetIndustrialSectorInOtherTransaction (company.ID, DomainObjectIDs.IndustrialSector2);

      industrialSector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1);

      // Resolve virtual end point - the database says that company does not point to IndustrialSector1, but the transaction says it does!
      industrialSector.Companies.EnsureDataComplete ();

      Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
      Assert.That (industrialSector.Companies, List.Not.Contains (company));
      CheckSyncState (company, c => c.IndustrialSector, false);
      CheckSyncState (industrialSector, s => s.Companies, true);
    }
  }
}