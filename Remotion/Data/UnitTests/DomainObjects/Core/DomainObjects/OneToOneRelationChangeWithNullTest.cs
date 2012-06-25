// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
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

      Assert.AreEqual (employee.ID, computerWithoutEmployee.Properties[typeof (Computer), "Employee"].GetRelatedObjectID ());

      Assert.AreSame (employee, computerWithoutEmployee.Employee);
      Assert.AreSame (computerWithoutEmployee, employee.Computer);
    }

    [Test]
    public void SetRelatedObjectOverVirtualEndPointAndOldRelatedObjectIsNull ()
    {
      Employee employeeWithoutComputer = Employee.GetObject (DomainObjectIDs.Employee1);
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      employeeWithoutComputer.Computer = computer;

      Assert.AreEqual (employeeWithoutComputer.ID, computer.Properties[typeof (Computer), "Employee"].GetRelatedObjectID ());

      Assert.AreSame (computer, employeeWithoutComputer.Computer);
      Assert.AreSame (employeeWithoutComputer, computer.Employee);
    }

    [Test]
    public void SetNewRelatedObjectNull ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
      Computer computer = employee.Computer;
      computer.Employee = null;

      Assert.IsNull (computer.Properties[typeof (Computer), "Employee"].GetRelatedObjectID ());

      Assert.IsNull (computer.Employee);
      Assert.IsNull (employee.Computer);
    }

    [Test]
    public void SetNewRelatedObjectNullOverVirtualEndPoint ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
      Computer computer = employee.Computer;
      employee.Computer = null;

			Assert.IsNull (computer.Properties[typeof (Computer), "Employee"].GetRelatedObjectID());

      Assert.IsNull (employee.Computer);
      Assert.IsNull (computer.Employee);
    }

    [Test]
    public void HasBeenTouchedWithNull_RealSide ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
      Computer computer = employee.Computer;

      CheckTouching (delegate { computer.Employee = null; }, computer, "Employee",
          RelationEndPointID.Create(employee.ID, typeof (Employee).FullName + ".Computer"),
          RelationEndPointID.Create(computer.ID, typeof (Computer).FullName + ".Employee"));
    }

    [Test]
    public void HasBeenTouchedWithNullTwice_RealSide ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
      Computer computer = employee.Computer;

      computer.Employee = null;
      
      SetDatabaseModifyable ();
      TestableClientTransaction.Commit ();

      CheckTouching (delegate { computer.Employee = null; }, computer, "Employee",
          RelationEndPointID.Create(computer.ID, typeof (Computer).FullName + ".Employee"));
    }

    [Test]
    public void HasBeenTouchedWithNull_VirtualSide ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
      Computer computer = employee.Computer;

      CheckTouching (delegate { employee.Computer = null; }, computer, "Employee",
          RelationEndPointID.Create(employee.ID, typeof (Employee).FullName + ".Computer"),
          RelationEndPointID.Create(computer.ID, typeof (Computer).FullName + ".Employee"));
    }

    [Test]
    public void HasBeenTouchedWithNullTwice_VirtualSide ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);

      employee.Computer = null;

      SetDatabaseModifyable ();
      TestableClientTransaction.Commit ();

      CheckTouching (delegate { employee.Computer = null; }, null, null,
          RelationEndPointID.Create(employee.ID, typeof (Employee).FullName + ".Computer"));
    }
  }
}
