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

namespace Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance.TestDomain
{
  [ClassID ("TI_Person")]
  [DBTable ("TableInheritance_Person")]
  [Instantiable]
  public abstract class Person: DomainBase
  {
    public static Person NewObject ()
    {
      return NewObject<Person> ().With ();
    }

    public static Person GetObject (ObjectID id)
    {
      return GetObject<Person> (id);
    }

    protected Person()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string FirstName { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string LastName { get; set; }

    public abstract DateTime DateOfBirth { get; set; }

    [DBBidirectionalRelation ("Person")]
    public abstract Address Address { get; }

    [BinaryProperty]
    public abstract byte[] Photo { get; set; }
  }
}
