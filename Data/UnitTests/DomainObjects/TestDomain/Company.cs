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
  public abstract class Company : TestDomainBase
  {
    public static Company NewObject ()
    {
      return NewObject<Company> ().With();
    }

    public new static Company GetObject (ObjectID id)
    {
      return GetObject<Company> (id);
    }

    protected Company ()
    {
    }

    [StorageClassNone]
    internal int NamePropertyOfInvalidType
    {
      set { Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.Name"].SetValue (value); }
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("Company")]
    [Mandatory]
    public abstract Ceo Ceo { get; set; }

    [DBBidirectionalRelation ("Companies")]
    public virtual IndustrialSector IndustrialSector
    {
      get { return CurrentProperty.GetValue<IndustrialSector> (); }
      set { CurrentProperty.SetValue<IndustrialSector> (value); }
    }

    [DBBidirectionalRelation ("Company")]
    private ClassWithoutRelatedClassIDColumnAndDerivation ClassWithoutRelatedClassIDColumnAndDerivation
    {
      get { return (ClassWithoutRelatedClassIDColumnAndDerivation) GetRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.ClassWithoutRelatedClassIDColumnAndDerivation"); }
      set { SetRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.ClassWithoutRelatedClassIDColumnAndDerivation", value); }
    }
  }
}
