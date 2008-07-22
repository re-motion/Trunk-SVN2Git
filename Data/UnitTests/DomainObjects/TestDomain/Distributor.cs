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
  [Instantiable]
  public abstract class Distributor : Partner
  {
    public static new Distributor NewObject ()
    {
      return NewObject<Distributor> ().With();
    }

    public new static Distributor GetObject (ObjectID id)
    {
      return GetObject<Distributor> (id);
    }

    protected Distributor ()
    {
    }

    public abstract int NumberOfShops { get; set; }

    [DBBidirectionalRelation ("Distributor")]
    private ClassWithoutRelatedClassIDColumn ClassWithoutRelatedClassIDColumn
    {
      get { return (ClassWithoutRelatedClassIDColumn) GetRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Distributor.ClassWithoutRelatedClassIDColumn"); }
      set { SetRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Distributor.ClassWithoutRelatedClassIDColumn", value); }
    }
  }
}
