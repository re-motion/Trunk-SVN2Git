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
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  [TestFixture]
  public class AddToOneToManyRelationWithLazyLoadTest : ClientTransactionBaseTest
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public AddToOneToManyRelationWithLazyLoadTest ()
    {
    }

    // methods and properties

    [Test]
    public void Insert ()
    {
      Employee newSupervisor = Employee.GetObject (DomainObjectIDs.Employee1);
      Employee subordinate = Employee.GetObject (DomainObjectIDs.Employee3);

      int countBeforeInsert = newSupervisor.Subordinates.Count;

      newSupervisor.Subordinates.Insert (0, subordinate);

      Assert.AreEqual (countBeforeInsert + 1, newSupervisor.Subordinates.Count);
      Assert.AreEqual (0, newSupervisor.Subordinates.IndexOf (subordinate));
      Assert.AreSame (newSupervisor, subordinate.Supervisor);

      Employee oldSupervisor = Employee.GetObject (DomainObjectIDs.Employee2);
      Assert.IsFalse (oldSupervisor.Subordinates.ContainsObject (subordinate));
    }
  }
}
