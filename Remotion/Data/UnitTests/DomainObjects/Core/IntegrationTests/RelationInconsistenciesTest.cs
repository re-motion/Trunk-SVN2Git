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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  public class RelationInconsistenciesTest : ClientTransactionBaseTest
  {
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
      Assert.That (companiesOfIndustrialSector, List.Not.Contains (company));
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
      // Resolve virtual end point - the database says that company points to IndustrialSector2, but the transaction says it points to IndustrialSector1!
      var companiesOfIndustrialSector = industrialSector.Companies;
      companiesOfIndustrialSector.EnsureDataComplete ();

      Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
      Assert.That (companiesOfIndustrialSector, List.Contains (company));
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
      // TODO 3737: Should only be 1 company, List should not contain new company
      Assert.That (industrialSector.Companies.Count, Is.EqualTo (2)); // Note: An additional item has been added to the collection.
      Assert.That (industrialSector.Companies, List.Contains (newCompany));

      // TODO 3737: In a try catch block, try to set newCompany.IndustrialSector to a different value - an InvalidOperationExcpetion should occur
    }

    [Test]
    [ExpectedException (
        typeof (ArgumentException), 
        ExpectedMessage = "The collection already contains an object with ID 'Company",
        MatchType = MessageMatch.Contains)]
    public void ObjectLoaded_WithInconsistentForeignKey_OneMany_ProblemDetectedInAdd ()
    {
      SetDatabaseModifyable();

      // setup new IndustrialSector object in database
      var industrialSector = CreateNewIndustrialSector();
      ClientTransactionMock.Commit();

      // in parallel transaction, add a Company to the IndustrialSector
      var newCompanyID = CreateCompanyAndSetIndustrialSectorInOtherTransaction (industrialSector.ID);
      // load Company into this transaction; in the database, the Company has a foreign key to the IndustrialSector
      var newCompany = Company.GetObject (newCompanyID);

      industrialSector.Companies.Add (newCompany);
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
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var newCompany = Company.NewObject();
        newCompany.Ceo = Ceo.NewObject();
        
        newCompany.IndustrialSector = IndustrialSector.GetObject (industrialSectorID);
        
        ClientTransaction.Current.Commit();
        return newCompany.ID;
      }
    }

    private void SetIndustrialSectorInOtherTransaction (ObjectID companyID, ObjectID industrialSectorID)
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var company = Company.GetObject (companyID);
        company.IndustrialSector = IndustrialSector.GetObject (industrialSectorID);

        ClientTransaction.Current.Commit ();
      }
    }

    private ObjectID CreateComputerAndSetEmployeeInOtherTransaction (ObjectID employeeID)
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var computer = Computer.NewObject ();
        computer.Employee = Employee.GetObject (employeeID);
        ClientTransaction.Current.Commit ();

        return computer.ID;
      }
    }

    private void SetEmployeeInOtherTransaction (ObjectID computerID, ObjectID employeeID)
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var computer = Computer.GetObject (computerID);
        computer.Employee = Employee.GetObject (employeeID);
        ClientTransaction.Current.Commit ();
      }
    }

  }
}