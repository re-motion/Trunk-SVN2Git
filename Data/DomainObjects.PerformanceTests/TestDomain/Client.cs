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

namespace Remotion.Data.DomainObjects.PerformanceTests.TestDomain
{
  [Instantiable]
  [DBTable]
  public abstract class Client : DomainObject
  {
    public static Client NewObject()
    {
      return NewObject<Client>().With();
    }

    public static Client GetObject (ObjectID id)
    {
      return DomainObject.GetObject<Client> (id);
    }

    protected Client ()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength= 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("Client")]
    public abstract ObjectList<File> Files { get; }

    [DBBidirectionalRelation ("Client")]
    public abstract ObjectList<ClientBoundBaseClass> ClientBoundBaseClasses { get; }
  }
}
