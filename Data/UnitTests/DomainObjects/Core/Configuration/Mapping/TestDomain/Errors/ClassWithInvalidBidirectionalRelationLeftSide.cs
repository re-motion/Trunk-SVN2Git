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
  public abstract class ClassWithInvalidBidirectionalRelationLeftSide : ClassWithInvalidBidirectionalRelationLeftSideNotInMapping
  {
    protected ClassWithInvalidBidirectionalRelationLeftSide ()
    {
    }

    [DBBidirectionalRelation ("NoContainsKeyRightSide")]
    public abstract ClassWithInvalidBidirectionalRelationRightSide NoContainsKeyLeftSide { get; set; }

    [DBBidirectionalRelation ("Invalid")]
    public abstract ClassWithInvalidBidirectionalRelationRightSide InvalidOppositePropertyNameLeftSide { get; set; }

    [DBBidirectionalRelation ("InvalidOppositePropertyTypeRightSide")]
    public abstract ClassWithInvalidBidirectionalRelationRightSide InvalidOppositePropertyTypeLeftSide { get; set; }

    [DBBidirectionalRelation ("InvalidOppositeCollectionPropertyTypeRightSide")]
    public abstract ClassWithInvalidBidirectionalRelationRightSide InvalidOppositeCollectionPropertyTypeLeftSide { get; set; }

    [DBBidirectionalRelation ("MissingBidirectionalRelationAttributeRightSide")]
    public abstract ClassWithInvalidBidirectionalRelationRightSide MissingBidirectionalRelationAttributeLeftSide { get; set; }

    [DBBidirectionalRelation ("MissingBidirectionalRelationAttributeForCollectionPropertyRightSide")]
    public abstract ObjectList<ClassWithInvalidBidirectionalRelationRightSide> MissingBidirectionalRelationAttributeForCollectionPropertyLeftSide { get; }

    [DBBidirectionalRelation ("InvalidPropertyNameInBidirectionalRelationAttributeOnOppositePropertyRightSide")]
    public abstract ClassWithInvalidBidirectionalRelationRightSide InvalidPropertyNameInBidirectionalRelationAttributeOnOppositePropertyLeftSide { get; set; }

    [DBBidirectionalRelation ("CollectionPropertyContainsKeyRightSide", ContainsForeignKey = true)]
    public abstract ObjectList<ClassWithInvalidBidirectionalRelationRightSide> CollectionPropertyContainsKeyLeftSide { get; }

    [DBBidirectionalRelation ("NonCollectionPropertyHavingASortExpressionRightSide", SortExpression = "Sort Expression")]
    public abstract ClassWithInvalidBidirectionalRelationRightSide NonCollectionPropertyHavingASortExpressionLeftSide { get; }
  }
}
