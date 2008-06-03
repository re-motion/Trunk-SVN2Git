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
  [ClassID ("TI_DerivedClassWithEntityFromBaseClassWithHierarchy")]
  [Instantiable]
  public abstract class DerivedClassWithEntityFromBaseClassWithHierarchy: DerivedClassWithEntityWithHierarchy
  {
    public new static DerivedClassWithEntityFromBaseClassWithHierarchy NewObject ()
    {
      return NewObject<DerivedClassWithEntityFromBaseClassWithHierarchy> ().With ();
    }

    public new static DerivedClassWithEntityFromBaseClassWithHierarchy GetObject (ObjectID id)
    {
      return GetObject<DerivedClassWithEntityFromBaseClassWithHierarchy> (id);
    }

    protected DerivedClassWithEntityFromBaseClassWithHierarchy ()
    {
    }

    [DBBidirectionalRelation ("ChildDerivedClassesWithEntityFromBaseClassWithHierarchy")]
    public abstract DerivedClassWithEntityFromBaseClassWithHierarchy ParentDerivedClassWithEntityFromBaseClassWithHierarchy { get; set; }

    [DBBidirectionalRelation ("ParentDerivedClassWithEntityFromBaseClassWithHierarchy", SortExpression = "Name ASC")]
    public abstract ObjectList<DerivedClassWithEntityFromBaseClassWithHierarchy> ChildDerivedClassesWithEntityFromBaseClassWithHierarchy { get; }

    public abstract Client ClientFromDerivedClassWithEntityFromBaseClass { get; set; }

    public abstract FileSystemItem FileSystemItemFromDerivedClassWithEntityFromBaseClass { get; set; }
  }
}
