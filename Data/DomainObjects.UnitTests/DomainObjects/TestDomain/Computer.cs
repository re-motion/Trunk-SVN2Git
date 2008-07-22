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
using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class Computer : TestDomainBase
  {
    public static Computer NewObject ()
    {
      return NewObject<Computer> ().With();
    }

    public new static Computer GetObject (ObjectID id)
    {
      return GetObject<Computer> (id);
    }

    protected Computer ()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 20)]
    public abstract string SerialNumber { get; set; }

    [DBBidirectionalRelation ("Computer", ContainsForeignKey = true)]
    public abstract Employee Employee { get; set; }

    [StorageClassTransaction]
    public abstract int Int32TransactionProperty { get; set; }

    [StorageClassTransaction]
    public abstract object ObjectTransactionProperty { get; set; }

    [StorageClassTransaction]
    [DBBidirectionalRelation ("ComputerTransactionProperty", ContainsForeignKey = true)]
    public abstract Employee EmployeeTransactionProperty { get; set; }
  }
}
