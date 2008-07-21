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
  [ClassID ("TI_AbstractBaseClassWithHierarchy")]
  [TableInheritanceTestDomain]
  public abstract class AbstractBaseClassWithHierarchy : DomainObject
  {
    protected AbstractBaseClassWithHierarchy ()
    {
    }

    public static AbstractBaseClassWithHierarchy GetObject (ObjectID id)
    {
      return GetObject<AbstractBaseClassWithHierarchy> (id);
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("ChildAbstractBaseClassesWithHierarchy")]
    public abstract AbstractBaseClassWithHierarchy ParentAbstractBaseClassWithHierarchy { get; set; }

    [DBBidirectionalRelation ("ParentAbstractBaseClassWithHierarchy", SortExpression = "Name DESC")]
    public abstract ObjectList<AbstractBaseClassWithHierarchy> ChildAbstractBaseClassesWithHierarchy { get;}

    public abstract Client ClientFromAbstractBaseClass { get; set; }

    public abstract FileSystemItem FileSystemItemFromAbstractBaseClass { get; set; }
  }
}
