// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors
{
  public abstract class ClassWithInvalidBidirectionalRelationRightSide: DomainObject
  {
    protected ClassWithInvalidBidirectionalRelationRightSide ()
    {
    }

    [DBBidirectionalRelation ("NoContainsKeyLeftSide")]
    public abstract ClassWithInvalidBidirectionalRelationLeftSide NoContainsKeyRightSide { get; set; }

    [DBBidirectionalRelation ("InvalidOppositePropertyTypeLeftSide")]
    public abstract OtherClassWithInvalidBidirectionalRelationLeftSide InvalidOppositePropertyTypeRightSide { get; set; }

    [DBBidirectionalRelation ("BaseInvalidOppositePropertyTypeLeftSide")]
    public abstract OtherClassWithInvalidBidirectionalRelationLeftSide BaseInvalidOppositePropertyTypeRightSide { get; set; }

    [DBBidirectionalRelation ("InvalidOppositeCollectionPropertyTypeLeftSide")]
    public abstract OtherClassWithInvalidBidirectionalRelationLeftSide InvalidOppositeCollectionPropertyTypeRightSide { get; set; }

    [DBBidirectionalRelation ("BaseInvalidOppositeCollectionPropertyTypeLeftSide")]
    public abstract OtherClassWithInvalidBidirectionalRelationLeftSide BaseInvalidOppositeCollectionPropertyTypeRightSide { get; set; }

    //[DBBidirectionalRelation ("MissingBidirectionalRelationAttributeLeftSide")]
    public abstract ClassWithInvalidBidirectionalRelationLeftSide MissingBidirectionalRelationAttributeRightSide { get; set; }

    //[DBBidirectionalRelation ("MissingBidirectionalRelationAttributeForCollectionPropertyLeftSide")]
    public abstract ClassWithInvalidBidirectionalRelationLeftSide MissingBidirectionalRelationAttributeForCollectionPropertyRightSide { get; set; }    
  
    [DBBidirectionalRelation ("Invalid")]
    public abstract ClassWithInvalidBidirectionalRelationLeftSide InvalidPropertyNameInBidirectionalRelationAttributeOnOppositePropertyRightSide { get; set; }

    [DBBidirectionalRelation ("CollectionPropertyContainsKeyLeftSide")]
    public abstract ClassWithInvalidBidirectionalRelationLeftSide CollectionPropertyContainsKeyRightSide { get; set; }

    [DBBidirectionalRelation ("NonCollectionPropertyHavingASortExpressionLeftSide")]
    public abstract ObjectList<ClassWithInvalidBidirectionalRelationLeftSide> NonCollectionPropertyHavingASortExpressionRightSide { get; set; }
  }
}
