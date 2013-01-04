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

      Assert.That (employee, Is.Not.Null);
      Assert.That (employee.ID, Is.EqualTo (DomainObjectIDs.Employee1));
    }

    [Test]
    public void GetChildren ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee1);
      DomainObjectCollection subordinates = employee.Subordinates;

      Assert.That (subordinates, Is.Not.Null);
      Assert.That (subordinates.Count, Is.EqualTo (2));
      Assert.That (subordinates[DomainObjectIDs.Employee4], Is.Not.Null);
      Assert.That (subordinates[DomainObjectIDs.Employee5], Is.Not.Null);
    }

    [Test]
    public void GetChildrenTwice ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee1);
      DomainObjectCollection subordinates = employee.Subordinates;

      Assert.That (ReferenceEquals (subordinates, employee.Subordinates), Is.True);
    }

    [Test]
    public void GetParent ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee4);
      Employee supervisor = employee.Supervisor;

      Assert.That (supervisor, Is.Not.Null);
      Assert.That (supervisor.ID, Is.EqualTo (DomainObjectIDs.Employee1));
    }

    [Test]
    public void GetParentTwice ()
    {
      Employee employee1 = Employee.GetObject (DomainObjectIDs.Employee4);
      Employee employee2 = Employee.GetObject (DomainObjectIDs.Employee5);

      Assert.That (ReferenceEquals (employee1.Supervisor, employee2.Supervisor), Is.True);
    }
  }
}
