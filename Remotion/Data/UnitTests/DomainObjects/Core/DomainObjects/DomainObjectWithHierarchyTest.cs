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
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  [TestFixture]
  public class DomainObjectWithHierarchyTest : ClientTransactionBaseTest
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public DomainObjectWithHierarchyTest ()
    {
    }

    // methods and properties

    [Test]
    public void GetObjectInHierarchy ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee1);

      Assert.IsNotNull (employee);
      Assert.AreEqual (DomainObjectIDs.Employee1, employee.ID);
    }

    [Test]
    public void GetChildren ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee1);
      DomainObjectCollection subordinates = employee.Subordinates;

      Assert.IsNotNull (subordinates);
      Assert.AreEqual (2, subordinates.Count);
      Assert.IsNotNull (subordinates[DomainObjectIDs.Employee4]);
      Assert.IsNotNull (subordinates[DomainObjectIDs.Employee5]);
    }

    [Test]
    public void GetChildrenTwice ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee1);
      DomainObjectCollection subordinates = employee.Subordinates;

      Assert.IsTrue (ReferenceEquals (subordinates, employee.Subordinates));
    }

    [Test]
    public void GetParent ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee4);
      Employee supervisor = employee.Supervisor;

      Assert.IsNotNull (supervisor);
      Assert.AreEqual (DomainObjectIDs.Employee1, supervisor.ID);
    }

    [Test]
    public void GetParentTwice ()
    {
      Employee employee1 = Employee.GetObject (DomainObjectIDs.Employee4);
      Employee employee2 = Employee.GetObject (DomainObjectIDs.Employee5);

      Assert.IsTrue (ReferenceEquals (employee1.Supervisor, employee2.Supervisor));
    }
  }
}
