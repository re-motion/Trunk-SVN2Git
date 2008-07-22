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
