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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.DomainObjects
{
  [TestFixture]
  public class OneToOneRelationChangeWithNullTest : RelationChangeBaseTest
  {
    [Test]
    public void OldRelatedObjectOfNewRelatedObjectIsNull ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
      Computer newComputerWithoutEmployee = Computer.GetObject (DomainObjectIDs.Computer4);

      employee.Computer = newComputerWithoutEmployee;

      // expectation: no exception
    }

    [Test]
    public void NewRelatedObjectIsNull ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
      employee.Computer = null;

      // expectation: no exception
    }

    [Test]
    public void OldRelatedObjectIsNull ()
    {
      Employee employeeWithoutComputer = Employee.GetObject (DomainObjectIDs.Employee1);
      Computer computerWithoutEmployee = Computer.GetObject (DomainObjectIDs.Computer4);
      employeeWithoutComputer.Computer = computerWithoutEmployee;

      // expectation: no exception
    }

    [Test]
    public void SetRelatedObjectAndOldRelatedObjectIsNull ()
    {
      Computer computerWithoutEmployee = Computer.GetObject (DomainObjectIDs.Computer4);
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee1);
      computerWithoutEmployee.Employee = employee;

			Assert.AreEqual (employee.ID, computerWithoutEmployee.InternalDataContainer.GetValue ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"));

      Assert.AreSame (employee, computerWithoutEmployee.Employee);
      Assert.AreSame (computerWithoutEmployee, employee.Computer);
    }

    [Test]
    public void SetRelatedObjectOverVirtualEndPointAndOldRelatedObjectIsNull ()
    {
      Employee employeeWithoutComputer = Employee.GetObject (DomainObjectIDs.Employee1);
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      employeeWithoutComputer.Computer = computer;

			Assert.AreEqual (employeeWithoutComputer.ID, computer.InternalDataContainer.GetValue ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"));

      Assert.AreSame (computer, employeeWithoutComputer.Computer);
      Assert.AreSame (employeeWithoutComputer, computer.Employee);
    }

    [Test]
    public void SetNewRelatedObjectNull ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
      Computer computer = employee.Computer;
      computer.Employee = null;

			Assert.IsNull (computer.InternalDataContainer.GetValue ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"));

      Assert.IsNull (computer.Employee);
      Assert.IsNull (employee.Computer);
    }

    [Test]
    public void SetNewRelatedObjectNullOverVirtualEndPoint ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
      Computer computer = employee.Computer;
      employee.Computer = null;

			Assert.IsNull (computer.InternalDataContainer.GetValue ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"));

      Assert.IsNull (employee.Computer);
      Assert.IsNull (computer.Employee);
    }

    [Test]
    public void HasBeenTouchedWithNull_RealSide ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
      Computer computer = employee.Computer;

      CheckTouching (delegate { computer.Employee = null; }, computer, "Employee",
          new RelationEndPointID (employee.ID, typeof (Employee).FullName + ".Computer"),
          new RelationEndPointID (computer.ID, typeof (Computer).FullName + ".Employee"));
    }

    [Test]
    public void HasBeenTouchedWithNullTwice_RealSide ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
      Computer computer = employee.Computer;

      computer.Employee = null;
      
      SetDatabaseModifyable ();
      ClientTransactionMock.Commit ();

      CheckTouching (delegate { computer.Employee = null; }, computer, "Employee",
          new RelationEndPointID (computer.ID, typeof (Computer).FullName + ".Employee"));
    }

    [Test]
    public void HasBeenTouchedWithNull_VirtualSide ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
      Computer computer = employee.Computer;

      CheckTouching (delegate { employee.Computer = null; }, computer, "Employee",
          new RelationEndPointID (employee.ID, typeof (Employee).FullName + ".Computer"),
          new RelationEndPointID (computer.ID, typeof (Computer).FullName + ".Employee"));
    }

    [Test]
    public void HasBeenTouchedWithNullTwice_VirtualSide ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);

      employee.Computer = null;

      SetDatabaseModifyable ();
      ClientTransactionMock.Commit ();

      CheckTouching (delegate { employee.Computer = null; }, null, null,
          new RelationEndPointID (employee.ID, typeof (Employee).FullName + ".Computer"));
    }
  }
}
