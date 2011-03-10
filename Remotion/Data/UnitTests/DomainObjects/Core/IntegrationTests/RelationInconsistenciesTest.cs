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

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  public class RelationInconsistenciesTest : ClientTransactionBaseTest
  {
    [Test]
    public void VirtualEndPointQuery_OneMany_Consistent_ObjectLoadedFirst ()
    {
      var orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      order1.OrderItems.EnsureDataComplete();

      Assert.That (orderItem1.Order, Is.SameAs (order1));
      Assert.That (order1.OrderItems, List.Contains (orderItem1));

      CheckSyncState (orderItem1, oi => oi.Order, true);
      CheckSyncState (orderItem1.Order, o => o.OrderItems, true);

      // these do nothing
      BidirectionalRelationSyncService.Synchronize (ClientTransactionMock, RelationEndPointID.Create (orderItem1, oi => oi.Order));
      BidirectionalRelationSyncService.Synchronize (ClientTransactionMock, RelationEndPointID.Create (orderItem1.Order, o => o.OrderItems));

      CheckSyncState (orderItem1, oi => oi.Order, true);
      CheckSyncState (orderItem1.Order, o => o.OrderItems, true);
    }

    [Test]
    public void VirtualEndPointQuery_OneMany_Consistent_CollectionLoadedFirst ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      order1.OrderItems.EnsureDataComplete ();
      var orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);

      Assert.That (orderItem1.Order, Is.SameAs (order1));
      Assert.That (order1.OrderItems, List.Contains (orderItem1));

      CheckSyncState (orderItem1, oi => oi.Order, true);
      CheckSyncState (orderItem1.Order, o => o.OrderItems, true);

      // these do nothing
      BidirectionalRelationSyncService.Synchronize (ClientTransactionMock, RelationEndPointID.Create (orderItem1, oi => oi.Order));
      BidirectionalRelationSyncService.Synchronize (ClientTransactionMock, RelationEndPointID.Create (orderItem1.Order, o => o.OrderItems));

      CheckSyncState (orderItem1, oi => oi.Order, true);
      CheckSyncState (orderItem1.Order, o => o.OrderItems, true);
    }

    [Test]
    public void VirtualEndPointQuery_OneOne_Consistent_RealEndPointLoadedFirst ()
    {
      var orderTicket1 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      var order1 = Order.GetObject (DomainObjectIDs.Order1);

      Assert.That (orderTicket1.Order, Is.SameAs (order1));
      Assert.That (order1.OrderTicket, Is.SameAs (orderTicket1));

      CheckSyncState (orderTicket1, oi => oi.Order, true);
      CheckSyncState (orderTicket1.Order, o => o.OrderTicket, true);

      // these do nothing
      BidirectionalRelationSyncService.Synchronize (ClientTransactionMock, RelationEndPointID.Create (orderTicket1, oi => oi.Order));
      BidirectionalRelationSyncService.Synchronize (ClientTransactionMock, RelationEndPointID.Create (orderTicket1.Order, o => o.OrderTicket));

      CheckSyncState (orderTicket1, oi => oi.Order, true);
      CheckSyncState (orderTicket1.Order, o => o.OrderTicket, true);
    }

    [Test]
    public void VirtualEndPointQuery_OneOne_Consistent_VirtualEndPointLoadedFirst ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      order1.OrderTicket.EnsureDataAvailable();
      var orderTicket1 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);

      Assert.That (orderTicket1.Order, Is.SameAs (order1));
      Assert.That (order1.OrderTicket, Is.SameAs (orderTicket1));

      CheckSyncState (orderTicket1, oi => oi.Order, true);
      CheckSyncState (orderTicket1.Order, o => o.OrderTicket, true);

      // these do nothing
      BidirectionalRelationSyncService.Synchronize (ClientTransactionMock, RelationEndPointID.Create (orderTicket1, oi => oi.Order));
      BidirectionalRelationSyncService.Synchronize (ClientTransactionMock, RelationEndPointID.Create (orderTicket1.Order, o => o.OrderTicket));

      CheckSyncState (orderTicket1, oi => oi.Order, true);
      CheckSyncState (orderTicket1.Order, o => o.OrderTicket, true);
    }

    [Test]
    [ExpectedException (typeof (LoadConflictException), ExpectedMessage =
        "Cannot load the related 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Computer' of "
        + @"'Employee\|51ece39b-f040-45b0-8b72-ad8b45353990\|System.Guid': The database returned related object 'Computer\|.*\|System.Guid', but that "
        + @"object already exists in the current ClientTransaction \(and points to a different object 'null'\).", 
        MatchType = MessageMatch.Regex)]
    public void VirtualEndPointQuery_OneOne_ObjectReturned_ThatLocallyPointsToNull ()
    {
      SetDatabaseModifyable ();

      var computer = Computer.NewObject ();
      ClientTransactionMock.Commit ();

      Assert.That (computer.Employee, Is.Null);

      var employee = Employee.GetObject (DomainObjectIDs.Employee1); // virtual end point not yet resolved

      SetEmployeeInOtherTransaction (computer.ID, employee.ID);

      // Resolve virtual end point - the database says that computer points to employee, but the transaction says computer points to null!
      Dev.Null = employee.Computer;
    }

    [Test]
    [ExpectedException (typeof (LoadConflictException), ExpectedMessage =
        "Cannot load the related 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Computer' of "
        + @"'Employee\|51ece39b-f040-45b0-8b72-ad8b45353990\|System.Guid': The database returned related object 'Computer\|.*\|System.Guid', but that "
        + @"object already exists in the current ClientTransaction \(and points to a different object "
        + @"'Employee\|c3b2bbc3-e083-4974-bac7-9cee1fb85a5e\|System.Guid'\).",
        MatchType = MessageMatch.Regex)]
    public void VirtualEndPointQuery_OneOne_ObjectReturned_ThatLocallyPointsSomewhereElse ()
    {
      SetDatabaseModifyable ();

      var computer = Computer.NewObject ();
      computer.Employee = Employee.GetObject (DomainObjectIDs.Employee2);
      ClientTransactionMock.Commit ();

      Assert.That (computer.Employee.ID, Is.EqualTo (DomainObjectIDs.Employee2));

      var employee = Employee.GetObject (DomainObjectIDs.Employee1); // virtual end point not yet resolved

      SetEmployeeInOtherTransaction (computer.ID, employee.ID);

      // Resolve virtual end point - the database says that computer points to employee, but the transaction says computer points to Employee2!
      Dev.Null = employee.Computer;
    }

    [Test]
    [Ignore ("TODO 3780")]
    public void VirtualEndPointQuery_OneMany_ObjectIncluded_ThatLocallyPointsToSomewhereElse ()
    {
      SetDatabaseModifyable ();

      var company = Company.NewObject ();
      company.Ceo = Ceo.NewObject (); // mandatory
      ClientTransactionMock.Commit ();

      Assert.That (company.IndustrialSector, Is.Null);

      var industrialSector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1); // virtual end point not yet resolved

      SetIndustrialSectorInOtherTransaction (company.ID, industrialSector.ID);

      // Resolve virtual end point - the database says that company points to industrialSector, but the transaction says it points to null!
      var companiesOfIndustrialSector = industrialSector.Companies;
      companiesOfIndustrialSector.EnsureDataComplete ();

      Assert.That (company.IndustrialSector, Is.Null);
      Assert.That (companiesOfIndustrialSector, List.Contains (company));

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector, s => s.Companies, false);

      var otherCompany = companiesOfIndustrialSector.FirstOrDefault (c => c != company);
      CheckSyncState (otherCompany, c => c.IndustrialSector, true);

      CheckActionWorks (() => industrialSector.Companies.Remove (otherCompany));
      CheckActionWorks (() => industrialSector.Companies.Add (Company.NewObject ()));

      CheckActionThrows<InvalidOperationException> (() => industrialSector.Companies.Remove (company), "out of sync with the opposite property");
      CheckActionWorks (() => company.IndustrialSector = IndustrialSector.NewObject());

      BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Create (industrialSector, s => s.Companies));

      CheckSyncState (industrialSector, s => s.Companies, true);
      Assert.That (companiesOfIndustrialSector, List.Not.Contains (company));
      
      CheckActionWorks (() => industrialSector.Companies.Add (company));
    }

    [Test]
    public void VirtualEndPointQuery_OneMany_ObjectIncluded_ThatLocallyPointsToSomewhereElse_SolvableViaReload ()
    {
      SetDatabaseModifyable ();

      var company = Company.NewObject ();
      company.Ceo = Ceo.NewObject (); // mandatory
      ClientTransactionMock.Commit ();

      Assert.That (company.IndustrialSector, Is.Null);

      var industrialSector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1); // virtual end point not yet resolved

      SetIndustrialSectorInOtherTransaction (company.ID, industrialSector.ID);

      // Resolve virtual end point - the database says that company points to industrialSector, but the transaction says it points to null!
      var companiesOfIndustrialSector = industrialSector.Companies;
      companiesOfIndustrialSector.EnsureDataComplete ();

      Assert.That (company.IndustrialSector, Is.Null);
      Assert.That (companiesOfIndustrialSector, List.Contains (company));
      CheckSyncState (industrialSector, s => s.Companies, false);

      UnloadService.UnloadData (ClientTransactionMock, company.ID, UnloadTransactionMode.ThisTransactionOnly);
      company.EnsureDataAvailable();

      CheckSyncState (industrialSector, s => s.Companies, true);
      Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
      Assert.That (companiesOfIndustrialSector, List.Contains (company));

      CheckActionWorks (() => industrialSector.Companies.Remove (company));
    }

    [Test]
    public void VirtualEndPointQuery_OneMany_ObjectNotIncluded_ThatLocallyPointsToHere ()
    {
      SetDatabaseModifyable ();

      var companyID = CreateCompanyAndSetIndustrialSectorInOtherTransaction (DomainObjectIDs.IndustrialSector1);
      var company = Company.GetObject (companyID);

      Assert.That (company.Properties[typeof (Company), "IndustrialSector"].GetRelatedObjectID(), Is.EqualTo (DomainObjectIDs.IndustrialSector1));

      SetIndustrialSectorInOtherTransaction (company.ID, DomainObjectIDs.IndustrialSector2);

      var industrialSector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1);
      // Resolve virtual end point - the database says that company does not point to IndustrialSector1, but the transaction says it does!
      var companiesOfIndustrialSector = industrialSector.Companies;
      companiesOfIndustrialSector.EnsureDataComplete ();

      Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
      Assert.That (companiesOfIndustrialSector, List.Not.Contains (company));

      CheckSyncState (company, c => c.IndustrialSector, false);
      CheckSyncState (industrialSector, s => s.Companies, true);
      CheckSyncState (companiesOfIndustrialSector[0], c => c.IndustrialSector, true);

      CheckActionWorks (() => industrialSector.Companies.RemoveAt (0));
      CheckActionWorks (() => industrialSector.Companies.Add (Company.NewObject ()));

      CheckActionThrows<InvalidOperationException> (() => industrialSector.Companies.Add (company), "out of sync with the opposite property");
      CheckActionThrows<InvalidOperationException> (() => company.IndustrialSector = null, "out of sync with the opposite property");

      BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Create (company, c => c.IndustrialSector));

      CheckSyncState (company, c => c.IndustrialSector, true);
      Assert.That (companiesOfIndustrialSector, List.Contains (company));
      CheckActionWorks (() => company.IndustrialSector = null);
      CheckActionWorks (() => industrialSector.Companies.Add (company));
    }

    [Test]
    [ExpectedException (typeof (LoadConflictException), ExpectedMessage = 
        @"The data of object 'Computer\|.*\|System.Guid' conflicts with existing data: It has a foreign key property "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.Employee' which points to object "
        + @"'Employee\|.*\|System.Guid'. However, that object has previously been determined to point back to object "
        + "'<null>'. These two pieces of information contradict each other.",
        MatchType = MessageMatch.Regex)]
    public void ObjectLoaded_WithInconsistentForeignKey_OneOne_Null ()
    {
      SetDatabaseModifyable();

      var employee = Employee.NewObject();
      ClientTransactionMock.Commit();

      Assert.That (employee.Computer, Is.Null);

      ObjectID newComputerID = CreateComputerAndSetEmployeeInOtherTransaction (employee.ID);

      // This computer has a foreign key to employee; but employee's virtual end point already points to null!
      Computer.GetObject (newComputerID);
    }

    [Test]
    [ExpectedException (typeof (LoadConflictException), ExpectedMessage = 
        @"The data of object 'Computer\|.*\|System.Guid' conflicts with existing data: It has a foreign key property "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.Employee' which points to object "
        + @"'Employee\|.*|System.Guid'. However, that object has previously been determined to point back to object "
        + @"'Computer\|.*\|System.Guid'. These two pieces of information contradict each other.",
        MatchType = MessageMatch.Regex)]
    public void ObjectLoaded_WithInconsistentForeignKey_OneOne_NonNull ()
    {
      SetDatabaseModifyable ();

      var originalComputer = Computer.GetObject (DomainObjectIDs.Computer1);
      var employee = originalComputer.Employee;
      
      Assert.That (employee.Computer, Is.SameAs (originalComputer));

      ObjectID newComputerID = CreateComputerAndSetEmployeeInOtherTransaction (employee.ID);

      // This computer has a foreign key to employee; but employee's virtual end point already points to originalComputer; the new computer's 
      // foreign key contradicts the existing foreign key
      Computer.GetObject (newComputerID);
    }

    [Test]
    public void ObjectLoaded_WithInconsistentForeignKey_OneMany ()
    {
      SetDatabaseModifyable();

      // set up new IndustrialSector object in database with one company
      var industrialSector = CreateNewIndustrialSector();
      ClientTransactionMock.Commit();

      // in parallel transaction, add a second Company to the IndustrialSector
      var newCompanyID = CreateCompanyAndSetIndustrialSectorInOtherTransaction (industrialSector.ID);

      Assert.That (industrialSector.Companies.Count, Is.EqualTo (1));

      // load Company into this transaction; in the database, the Company has a foreign key to the IndustrialSector
      var newCompany = Company.GetObject (newCompanyID);

      Assert.That (newCompany.IndustrialSector, Is.SameAs (industrialSector));
      Assert.That (industrialSector.Companies.Count, Is.EqualTo (1));
      Assert.That (industrialSector.Companies, List.Not.Contains (newCompany));

      CheckSyncState (industrialSector, s => s.Companies, true);
      CheckSyncState (newCompany, c => c.IndustrialSector, false);

      CheckSyncState (newCompany, c => c.IndustrialSector, false);
      CheckSyncState (industrialSector, s => s.Companies, true);
      CheckSyncState (industrialSector.Companies[0], c => c.IndustrialSector, true);

      CheckActionWorks (() => industrialSector.Companies.RemoveAt (0));
      CheckActionWorks (() => industrialSector.Companies.Add (Company.NewObject ()));

      CheckActionThrows<InvalidOperationException> (() => industrialSector.Companies.Add (newCompany), "out of sync with the opposite property");
      CheckActionThrows<InvalidOperationException> (() => newCompany.IndustrialSector = null, "out of sync with the opposite property");

      BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Create (newCompany, c => c.IndustrialSector));

      CheckSyncState (newCompany, c => c.IndustrialSector, true);
      Assert.That (industrialSector.Companies, List.Contains (newCompany));
      CheckActionWorks (() => newCompany.IndustrialSector = null);
      CheckActionWorks (() => industrialSector.Companies.Add (newCompany));
    }

    private IndustrialSector CreateNewIndustrialSector ()
    {
      IndustrialSector industrialSector = IndustrialSector.NewObject();
      Company oldCompany = Company.NewObject();
      oldCompany.Ceo = Ceo.NewObject();
      industrialSector.Companies.Add (oldCompany);
      return industrialSector;
    }

    private ObjectID CreateCompanyAndSetIndustrialSectorInOtherTransaction (ObjectID industrialSectorID)
    {
      return DomainObjectMother.CreateObjectAndSetRelationInOtherTransaction<Company, IndustrialSector> (industrialSectorID, (c, s) =>
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
        TOriginating company,
        Expression<Func<TOriginating, TRelated>> propertyAccessExpression,
        bool expectedState)
        where TOriginating: DomainObject
    {
      Assert.That (
          BidirectionalRelationSyncService.IsSynchronized (ClientTransaction.Current, RelationEndPointID.Create (company, propertyAccessExpression)),
          Is.EqualTo (expectedState));
    }

    private void CheckActionWorks (Action action)
    {
      action ();
    }

    private void CheckActionThrows<TException> (Action action, string expectedMessage) where TException : Exception
    {
      try
      {
        action ();
      }
      catch (Exception ex)
      {
        Assert.That (ex, Is.TypeOf (typeof (TException)));
        Assert.That (
            ex.Message, 
            NUnit.Framework.SyntaxHelpers.Text.Contains (expectedMessage), 
            "Expected: " + expectedMessage + Environment.NewLine + "Was: " + ex.Message);
      }
    }
  }
}