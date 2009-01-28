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
using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class Employee : TestDomainBase
  {
    public static Employee NewObject ()
    {
      return NewObject<Employee> ();
    }

    public new static Employee GetObject (ObjectID id)
    {
      return GetObject<Employee> (id);
    }

    public new static Employee GetObject (ObjectID id, bool includeDeleted)
    {
      return GetObject<Employee> (id, includeDeleted);
    }

    protected Employee ()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("Supervisor")]
    public abstract ObjectList<Employee> Subordinates { get; }

    [DBBidirectionalRelation ("Subordinates")]
    public abstract Employee Supervisor { get; set; }

    [DBBidirectionalRelation ("Employee")]
    public Computer Computer
    {
      get { return Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Computer"].GetValue<Computer>(); }
      set { Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Computer"].SetValue (value); }
    }

    [StorageClassTransaction]
    [DBBidirectionalRelation ("EmployeeTransactionProperty")]
    public abstract Computer ComputerTransactionProperty { get; set; }

    public void DeleteWithSubordinates ()
    {
      foreach (Employee employee in Subordinates.Clone ())
        employee.Delete ();

      this.Delete ();
    }
  }
}
