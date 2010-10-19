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
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  public class RelationInconsistenciesTest : ClientTransactionBaseTest
  {
    [Test]
    [Ignore ("TODO 2984: Find a way to deal with this (currently: AssertionException)")]
    public void VirtualEndPointQuery_OneOne_ObjectReturned_ThatLocallyPointsToNull ()
    {
      SetDatabaseModifyable ();

      var computer = Computer.NewObject ();
      ClientTransactionMock.Commit ();

      Assert.That (computer.Employee, Is.Null);

      var employee = Employee.GetObject (DomainObjectIDs.Employee1); // virtual end point not yet resolved

      SetEmployeeInOtherTransaction (computer.ID, employee.ID);

      // Resolve virtual end point - the database says that computer points to employee, but the transaction says it points to null!
      var computerOfEmployee = employee.Computer;

      Assert.That (computer.Employee, Is.Null);
      Assert.That (computerOfEmployee, Is.Null);
    }

    [Test]
    [Ignore ("TODO 2984: Find a way to deal with this (currently: inconsistent state of relation end points)")]
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
      companiesOfIndustrialSector.EnsureDataAvailable ();

      Assert.That (company.IndustrialSector, Is.Null);
      Assert.That (companiesOfIndustrialSector, List.Not.Contains (company));
    }

    [Test]
    [Ignore ("TODO 2984: Find a way to deal with this (currently: inconsistent state of relation end points)")]
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
      companiesOfIndustrialSector.EnsureDataAvailable ();

      Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
      Assert.That (companiesOfIndustrialSector, List.Contains (company));
    }

    [Test]
    [Ignore ("TODO 731: Find a way to deal with this (currently: InvalidOperationException)")]
    public void ObjectLoaded_WithInconsistentForeignKey_OneOne_Null ()
    {
      SetDatabaseModifyable();

      var employee = Employee.NewObject();
      ClientTransactionMock.Commit();

      Assert.That (employee.Computer, Is.Null);

      ObjectID newComputerID = CreateComputerAndSetEmployeeInOtherTransaction (employee.ID);

      // This computer has a foreign key to employee; but employee's virtual end point already points to null!
      Computer.GetObject (newComputerID);
      Assert.Fail ("TODO 731: Proper behavior yet undecided");
    }

    [Test]
    [Ignore ("TODO 731: Find a way to deal with this (currently: InvalidOperationException)")]
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
      Assert.Fail ("TODO 731: Proper behavior yet undecided");
    }

    [Test]
    [Ignore ("TODO 731: Find a way to deal with this (currently: inconsistent relations)")]
    public void ObjectLoaded_WithInconsistentForeignKey_OneMany ()
    {
      SetDatabaseModifyable();

      // set up new IndustrialSector object in database
      var industrialSector = CreateNewIndustrialSector();
      ClientTransactionMock.Commit();

      // in parallel transaction, add a Company to the IndustrialSector
      var newCompanyID = CreateCompanyAndSetIndustrialSectorInOtherTransaction (industrialSector.ID);

      // load Company into this transaction; in the database, the Company has a foreign key to the IndustrialSector
      var newCompany = Company.GetObject (newCompanyID);

      // this prints true/false => the bidirectional relation is inconsistent
      Console.WriteLine (
          "{0}.IndustrialSector = {1} (the company has a reference to the industrial sector: {2})",
          newCompany,
          newCompany.IndustrialSector,
          newCompany.IndustrialSector == industrialSector);
      Console.WriteLine (
          "{0}.Companies.Count = {1} (the industrial sector has a reference to the company: {2})",
          industrialSector,
          industrialSector.Companies.Count,
          industrialSector.Companies.ContainsObject (industrialSector));

      Assert.Fail ("TODO 731: Proper behavior yet undecided");
    }

    [Test]
    [Ignore ("TODO 921: Probably obsolete after COMMONS-731")]
    public void ObjectLoaded_WithInconsistentForeignKey_OneMany_ProblemDetectedInAdd ()
    {
      SetDatabaseModifyable();

      // setup new IndustrialSector object in database
      var industrialSector = CreateNewIndustrialSector();
      ClientTransactionMock.Commit();

      // in parallel transaction, add a Company to the IndustrialSector
      var newCompanyID = CreateCompanyAndSetIndustrialSectorInOtherTransaction (industrialSector.ID);
      // load Company into this transaction; in the database, the Company has a foreign key to the IndustrialSector
      var newCompany = Company.GetObject (newCompanyID); // TODO 731 (re-motion 2.1): This should throw an exception.

      industrialSector.Companies.Add (newCompany); // TODO 921: Throw here
      Console.WriteLine (
          "{0}.IndustrialSector = {1} (the company has a reference to the industrial sector: {2})",
          newCompany,
          newCompany.IndustrialSector,
          newCompany.IndustrialSector == industrialSector);
      Console.WriteLine (
          "{0}.Companies.Count = {1} (the industrial sector has a reference to the company: {2})",
          industrialSector,
          industrialSector.Companies.Count,
          industrialSector.Companies.ContainsObject (industrialSector));
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