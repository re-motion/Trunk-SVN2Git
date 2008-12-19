// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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

      object referenceObject = new object ();
      Employee referenceEmployee = Employee.NewObject ();

      Computer computer = Computer.NewObject ();
      CheckDefaultValueAndValueAfterSet(computer, referenceObject, referenceEmployee);
      CheckValueAfterCommitAndRollback(computer, referenceObject, referenceEmployee);
      CheckValueInParallelRootTransaction(computer, referenceEmployee);

      computer.Delete ();
      referenceEmployee.Delete ();
      ClientTransactionMock.Commit ();
    }

    [Test]
    public void Commit_Rollback_ExistingObject ()
    {
      object referenceObject = new object ();
      Employee referenceEmployee = Employee.GetObject (DomainObjectIDs.Employee1);

      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      CheckDefaultValueAndValueAfterSet (computer, referenceObject, referenceEmployee);

      ClientTransactionMock.Rollback ();
      Assert.That (computer.Int32TransactionProperty, Is.EqualTo (0));
      Assert.That (computer.ObjectTransactionProperty, Is.Null);
      Assert.That (computer.EmployeeTransactionProperty, Is.Null);

      computer.Int32TransactionProperty = 5;
      computer.ObjectTransactionProperty = referenceObject;
      computer.EmployeeTransactionProperty = referenceEmployee;
      
      CheckValueAfterCommitAndRollback (computer, referenceObject, referenceEmployee);
      CheckValueInParallelRootTransaction (computer, referenceEmployee);
    }

    [Test]
    public void Commit_Rollback_SubtransactionExistingObject ()
    {
      object referenceObject = new object ();
      object referenceObject2 = new object ();
      Employee referenceEmployee = Employee.GetObject (DomainObjectIDs.Employee1);
      Employee referenceEmployee2 = Employee.GetObject (DomainObjectIDs.Employee2);

      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      computer.Int32TransactionProperty = 5;
      computer.ObjectTransactionProperty = referenceObject;
      computer.EmployeeTransactionProperty = referenceEmployee;

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        CheckPropertiesAfterSet(computer, referenceObject, referenceEmployee);

        computer.Int32TransactionProperty = 6;
        computer.ObjectTransactionProperty = referenceObject2;
        computer.EmployeeTransactionProperty = referenceEmployee2;

        ClientTransaction.Current.Commit ();
      }

      Assert.That (computer.Int32TransactionProperty, Is.EqualTo (6));
      Assert.That (computer.ObjectTransactionProperty, Is.SameAs (referenceObject2));
      Assert.That (computer.EmployeeTransactionProperty, Is.SameAs (referenceEmployee2));
      Assert.That (computer.EmployeeTransactionProperty.ComputerTransactionProperty, Is.SameAs (computer));
      Assert.That (referenceEmployee.ComputerTransactionProperty, Is.Null);
    }

    [Test]
    public void Commit_Rollback_SubtransactionNewObject ()
    {
      object referenceObject = new object ();
      Employee referenceEmployee = Employee.GetObject (DomainObjectIDs.Employee1);

      Computer computer;
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        computer = Computer.NewObject ();
        computer.Int32TransactionProperty = 5;
        computer.ObjectTransactionProperty = referenceObject;
        computer.EmployeeTransactionProperty = referenceEmployee;
        CheckPropertiesAfterSet (computer, referenceObject, referenceEmployee);

        ClientTransaction.Current.Commit ();
      }

      CheckPropertiesAfterSet (computer, referenceObject, referenceEmployee);
    }

    private void CheckPropertiesAfterSet (Computer computer, object referenceObject, Employee referenceEmployee)
    {
      Assert.That (computer.Int32TransactionProperty, Is.EqualTo (5));
      Assert.That (computer.ObjectTransactionProperty, Is.SameAs (referenceObject));
      Assert.That (computer.EmployeeTransactionProperty, Is.SameAs (referenceEmployee));
      Assert.That (computer.EmployeeTransactionProperty.ComputerTransactionProperty, Is.SameAs (computer));
    }

    private void CheckDefaultValueAndValueAfterSet (Computer computer, object referenceObject, Employee referenceEmployee)
    {
      Assert.That (computer.Int32TransactionProperty, Is.EqualTo (0));
      Assert.That (computer.ObjectTransactionProperty, Is.Null);
      Assert.That (computer.EmployeeTransactionProperty, Is.Null);

      computer.Int32TransactionProperty = 5;
      computer.ObjectTransactionProperty = referenceObject;
      computer.EmployeeTransactionProperty = referenceEmployee;

      CheckPropertiesAfterSet (computer, referenceObject, referenceEmployee);
    }

    private void CheckValueAfterCommitAndRollback (Computer computer, object referenceObject, Employee referenceEmployee)
    {
      ClientTransactionMock.Commit ();
      CheckPropertiesAfterSet (computer, referenceObject, referenceEmployee);

      ClientTransactionMock.Rollback ();
      CheckPropertiesAfterSet (computer, referenceObject, referenceEmployee);
    }

    private void CheckValueInParallelRootTransaction (Computer computer, Employee referenceEmployee)
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        Computer sameComputer = Computer.GetObject (computer.ID);
        Employee sameReferenceEmployee = Employee.GetObject (referenceEmployee.ID);
        Assert.That (sameComputer.Int32TransactionProperty, Is.EqualTo (0));
        Assert.That (sameComputer.ObjectTransactionProperty, Is.Null);
        Assert.That (sameComputer.EmployeeTransactionProperty, Is.Null);
        Assert.That (sameReferenceEmployee.ComputerTransactionProperty, Is.Null);
      }
    }
  }
}
