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
  public abstract class IndustrialSector : TestDomainBase
  {
    public static IndustrialSector NewObject ()
    {
      return NewObject<IndustrialSector> ().With ();
    }

    public new static IndustrialSector GetObject (ObjectID id)
    {
      return GetObject<IndustrialSector> (id);
    }

    protected IndustrialSector ()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public virtual string Name
    {
      get
      {
        return CurrentProperty.GetValue<string> ();
      }
      set
      {
        CurrentProperty.SetValue (value);
      }
    }

    [DBBidirectionalRelationAttribute ("IndustrialSector")]
    [Mandatory]
    public virtual ObjectList<Company> Companies
    {
      get
      {
        return CurrentProperty.GetValue<ObjectList<Company>> ();
      }
    }
  }
}
