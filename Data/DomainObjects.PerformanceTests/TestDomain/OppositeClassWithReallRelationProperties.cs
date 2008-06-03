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

namespace Remotion.Data.DomainObjects.PerformanceTests.TestDomain
{
  [Instantiable]
  [DBTable]
  [Serializable]
  public abstract class OppositeClassWithRealRelationProperties : SimpleDomainObject<OppositeClassWithRealRelationProperties>
  {
    [DBBidirectionalRelation ("Virtual1", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real1 { get; set; }
    [DBBidirectionalRelation ("Virtual2", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real2 { get; set; }
    [DBBidirectionalRelation ("Virtual3", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real3 { get; set; }
    [DBBidirectionalRelation ("Virtual4", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real4 { get; set; }
    [DBBidirectionalRelation ("Virtual5", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real5 { get; set; }
    [DBBidirectionalRelation ("Virtual6", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real6 { get; set; }
    [DBBidirectionalRelation ("Virtual7", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real7 { get; set; }
    [DBBidirectionalRelation ("Virtual8", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real8 { get; set; }
    [DBBidirectionalRelation ("Virtual9", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real9 { get; set; }
    [DBBidirectionalRelation ("Virtual10", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real10 { get; set; }
  }
}
