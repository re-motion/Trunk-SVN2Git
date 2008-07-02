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

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class Employee : TestDomainBase
  {
    public static Employee NewObject ()
    {
      return NewObject<Employee> ().With();
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
      get { return Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Computer"].GetValue<Computer>(); }
      set { Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Computer"].SetValue (value); }
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
