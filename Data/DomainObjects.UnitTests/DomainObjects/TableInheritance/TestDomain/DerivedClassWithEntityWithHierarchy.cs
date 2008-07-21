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
  [ClassID ("TI_DerivedClassWithEntityWithHierarchy")]
  [DBTable ("TableInheritance_DerivedClassWithEntityWithHierarchy")]
  [Instantiable]
  public abstract class DerivedClassWithEntityWithHierarchy: AbstractBaseClassWithHierarchy
  {
    public static DerivedClassWithEntityWithHierarchy NewObject ()
    {
      return NewObject<DerivedClassWithEntityWithHierarchy> ().With ();
    }

    public new static DerivedClassWithEntityWithHierarchy GetObject (ObjectID id)
    {
      return GetObject<DerivedClassWithEntityWithHierarchy> (id);
    }

    protected DerivedClassWithEntityWithHierarchy ()
    {
    }

    [DBBidirectionalRelation ("ChildDerivedClassesWithEntityWithHierarchy")]
    public abstract DerivedClassWithEntityWithHierarchy ParentDerivedClassWithEntityWithHierarchy { get; set; }

    [DBBidirectionalRelation ("ParentDerivedClassWithEntityWithHierarchy", SortExpression = "Name ASC")]
    public abstract ObjectList<DerivedClassWithEntityWithHierarchy> ChildDerivedClassesWithEntityWithHierarchy { get; }

    public abstract Client ClientFromDerivedClassWithEntity { get; set; }

    public abstract FileSystemItem FileSystemItemFromDerivedClassWithEntity { get; set; }
  }
}
