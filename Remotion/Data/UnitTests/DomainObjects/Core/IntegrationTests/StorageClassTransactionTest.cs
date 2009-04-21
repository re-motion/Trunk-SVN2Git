// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
  public class StorageClassTransactionTest : ClientTransactionBaseTest
  {
    [Test]
    public void Commit_Rollback_NewObject ()
    {
      SetDatabaseModifyable ();

      DateTime referenceDateTime = DateTime.Now;
      Employee referenceEmployee = Employee.NewObject ();

      Computer computer = Computer.NewObject ();
      CheckDefaultValueAndValueAfterSet (computer, referenceDateTime, referenceEmployee);
      CheckValueAfterCommitAndRollback(computer, referenceDateTime, referenceEmployee);
      CheckValueInParallelRootTransaction(computer, referenceEmployee);

      computer.Delete ();
      referenceEmployee.Delete ();
      ClientTransactionMock.Commit ();
    }

    [Test]
    public void Commit_Rollback_ExistingObject ()
    {
      DateTime referenceDateTime = DateTime.Now;
      Employee referenceEmployee = Employee.GetObject (DomainObjectIDs.Employee1);

      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      CheckDefaultValueAndValueAfterSet (computer, referenceDateTime, referenceEmployee);

      ClientTransactionMock.Rollback ();
      Assert.That (computer.Int32TransactionProperty, Is.EqualTo (0));
      Assert.That (computer.DateTimeTransactionProperty, Is.EqualTo (new DateTime()));
      Assert.That (computer.EmployeeTransactionProperty, Is.Null);

      computer.Int32TransactionProperty = 5;
      computer.DateTimeTransactionProperty = referenceDateTime;
      computer.EmployeeTransactionProperty = referenceEmployee;
      
      CheckValueAfterCommitAndRollback (computer, referenceDateTime, referenceEmployee);
      CheckValueInParallelRootTransaction (computer, referenceEmployee);
    }

    [Test]
    public void Commit_Rollback_SubtransactionExistingObject ()
    {
      DateTime referenceDateTime = DateTime.MinValue;
      DateTime referenceDateTime2 = DateTime.MaxValue;
      Employee referenceEmployee = Employee.GetObject (DomainObjectIDs.Employee1);
      Employee referenceEmployee2 = Employee.GetObject (DomainObjectIDs.Employee2);

      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      computer.Int32TransactionProperty = 5;
      computer.DateTimeTransactionProperty = referenceDateTime;
      computer.EmployeeTransactionProperty = referenceEmployee;

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        CheckPropertiesAfterSet(computer, referenceDateTime, referenceEmployee);

        computer.Int32TransactionProperty = 6;
        computer.DateTimeTransactionProperty = referenceDateTime2;
        computer.EmployeeTransactionProperty = referenceEmployee2;

        ClientTransaction.Current.Commit ();
      }

      Assert.That (computer.Int32TransactionProperty, Is.EqualTo (6));
      Assert.That (computer.DateTimeTransactionProperty, Is.EqualTo(referenceDateTime2));
      Assert.That (computer.EmployeeTransactionProperty, Is.SameAs (referenceEmployee2));
      Assert.That (computer.EmployeeTransactionProperty.ComputerTransactionProperty, Is.SameAs (computer));
      Assert.That (referenceEmployee.ComputerTransactionProperty, Is.Null);
    }

    [Test]
    public void Commit_Rollback_SubtransactionNewObject ()
    {
      DateTime referenceDateTime = DateTime.Now;
      Employee referenceEmployee = Employee.GetObject (DomainObjectIDs.Employee1);

      Computer computer;
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        computer = Computer.NewObject ();
        computer.Int32TransactionProperty = 5;
        computer.DateTimeTransactionProperty = referenceDateTime;
        computer.EmployeeTransactionProperty = referenceEmployee;
        CheckPropertiesAfterSet (computer, referenceDateTime, referenceEmployee);

        ClientTransaction.Current.Commit ();
      }

      CheckPropertiesAfterSet (computer, referenceDateTime, referenceEmployee);
    }

    private void CheckPropertiesAfterSet (Computer computer, DateTime referenceDateTime, Employee referenceEmployee)
    {
      Assert.That (computer.Int32TransactionProperty, Is.EqualTo (5));
      Assert.That (computer.DateTimeTransactionProperty, Is.EqualTo (referenceDateTime));
      Assert.That (computer.EmployeeTransactionProperty, Is.SameAs (referenceEmployee));
      Assert.That (computer.EmployeeTransactionProperty.ComputerTransactionProperty, Is.SameAs (computer));
    }

    private void CheckDefaultValueAndValueAfterSet (Computer computer, DateTime referenceDateTime, Employee referenceEmployee)
    {
      Assert.That (computer.Int32TransactionProperty, Is.EqualTo (0));
      Assert.That (computer.DateTimeTransactionProperty, Is.EqualTo (new DateTime()));
      Assert.That (computer.EmployeeTransactionProperty, Is.Null);

      computer.Int32TransactionProperty = 5;
      computer.DateTimeTransactionProperty = referenceDateTime;
      computer.EmployeeTransactionProperty = referenceEmployee;

      CheckPropertiesAfterSet (computer, referenceDateTime, referenceEmployee);
    }

    private void CheckValueAfterCommitAndRollback (Computer computer, DateTime referenceDateTime, Employee referenceEmployee)
    {
      ClientTransactionMock.Commit ();
      CheckPropertiesAfterSet (computer, referenceDateTime, referenceEmployee);

      ClientTransactionMock.Rollback ();
      CheckPropertiesAfterSet (computer, referenceDateTime, referenceEmployee);
    }

    private void CheckValueInParallelRootTransaction (Computer computer, Employee referenceEmployee)
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        Computer sameComputer = Computer.GetObject (computer.ID);
        Employee sameReferenceEmployee = Employee.GetObject (referenceEmployee.ID);
        Assert.That (sameComputer.Int32TransactionProperty, Is.EqualTo (0));
        Assert.That (sameComputer.DateTimeTransactionProperty, Is.EqualTo (new DateTime()));
        Assert.That (sameComputer.EmployeeTransactionProperty, Is.Null);
        Assert.That (sameReferenceEmployee.ComputerTransactionProperty, Is.Null);
      }
    }
  }
}
