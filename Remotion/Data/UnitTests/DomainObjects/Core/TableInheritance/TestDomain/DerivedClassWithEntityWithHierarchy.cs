// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain
{
  [ClassID ("TI_DerivedClassWithEntityWithHierarchy")]
  [DBTable ("TableInheritance_DerivedClassWithEntityWithHierarchy")]
  [Instantiable]
  public abstract class DerivedClassWithEntityWithHierarchy: AbstractBaseClassWithHierarchy
  {
    public static DerivedClassWithEntityWithHierarchy NewObject ()
    {
      return NewObject<DerivedClassWithEntityWithHierarchy> ();
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
