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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.DomainObjects
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
