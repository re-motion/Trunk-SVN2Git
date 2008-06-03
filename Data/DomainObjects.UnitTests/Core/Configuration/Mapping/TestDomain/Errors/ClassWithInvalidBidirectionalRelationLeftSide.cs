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

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.TestDomain.Errors
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
