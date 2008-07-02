/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.IntegrationTests
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

    private void CheckDefaultValueAndValueAfterSet (Computer computer, object referenceObject, Employee referenceEmployee)
    {
      Assert.That (computer.Int32TransactionProperty, Is.EqualTo (0));
      Assert.That (computer.ObjectTransactionProperty, Is.Null);
      Assert.That (computer.EmployeeTransactionProperty, Is.Null);
      computer.Int32TransactionProperty = 5;
      computer.ObjectTransactionProperty = referenceObject;
      computer.EmployeeTransactionProperty = referenceEmployee;
      Assert.That (computer.Int32TransactionProperty, Is.EqualTo (5));
      Assert.That (computer.ObjectTransactionProperty, Is.SameAs (referenceObject));
      Assert.That (computer.EmployeeTransactionProperty, Is.SameAs (referenceEmployee));
      Assert.That (computer.EmployeeTransactionProperty.ComputerTransactionProperty, Is.SameAs (computer));
    }

    private void CheckValueAfterCommitAndRollback (Computer computer, object referenceObject, Employee referenceEmployee)
    {
      ClientTransactionMock.Commit ();
      Assert.That (computer.Int32TransactionProperty, Is.EqualTo (5));
      Assert.That (computer.ObjectTransactionProperty, Is.SameAs (referenceObject));
      Assert.That (computer.EmployeeTransactionProperty, Is.SameAs (referenceEmployee));
      Assert.That (computer.EmployeeTransactionProperty.ComputerTransactionProperty, Is.SameAs (computer));

      ClientTransactionMock.Rollback ();
      Assert.That (computer.Int32TransactionProperty, Is.EqualTo (5));
      Assert.That (computer.ObjectTransactionProperty, Is.SameAs (referenceObject));
      Assert.That (computer.EmployeeTransactionProperty, Is.SameAs (referenceEmployee));
      Assert.That (computer.EmployeeTransactionProperty.ComputerTransactionProperty, Is.SameAs (computer));
    }

    private void CheckValueInParallelRootTransaction (Computer computer, Employee referenceEmployee)
    {
      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
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