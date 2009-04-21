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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
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

			Assert.AreEqual (employee.ID, computerWithoutEmployee.InternalDataContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.Employee"));

      Assert.AreSame (employee, computerWithoutEmployee.Employee);
      Assert.AreSame (computerWithoutEmployee, employee.Computer);
    }

    [Test]
    public void SetRelatedObjectOverVirtualEndPointAndOldRelatedObjectIsNull ()
    {
      Employee employeeWithoutComputer = Employee.GetObject (DomainObjectIDs.Employee1);
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      employeeWithoutComputer.Computer = computer;

			Assert.AreEqual (employeeWithoutComputer.ID, computer.InternalDataContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.Employee"));

      Assert.AreSame (computer, employeeWithoutComputer.Computer);
      Assert.AreSame (employeeWithoutComputer, computer.Employee);
    }

    [Test]
    public void SetNewRelatedObjectNull ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
      Computer computer = employee.Computer;
      computer.Employee = null;

			Assert.IsNull (computer.InternalDataContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.Employee"));

      Assert.IsNull (computer.Employee);
      Assert.IsNull (employee.Computer);
    }

    [Test]
    public void SetNewRelatedObjectNullOverVirtualEndPoint ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
      Computer computer = employee.Computer;
      employee.Computer = null;

			Assert.IsNull (computer.InternalDataContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.Employee"));

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
