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

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.TableInheritance.TestDomain
{
  [ClassID ("TI_Client")]
  [DBTable ("TableInheritance_Client")]
  [Instantiable]
  [TableInheritanceTestDomain]
  public abstract class Client : DomainObject
  {
    public static Client NewObject ()
    {
      return NewObject<Client> ().With();
    }

    public static Client GetObject (ObjectID id)
    {
      return GetObject<Client> (id);
    }

    protected Client ()
    {
    }

    [DBBidirectionalRelation ("Client", SortExpression = "CreatedAt asc")]
    public abstract ObjectList<DomainBase> AssignedObjects { get; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }
  }
}
