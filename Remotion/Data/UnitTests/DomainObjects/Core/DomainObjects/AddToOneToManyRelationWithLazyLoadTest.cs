// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
