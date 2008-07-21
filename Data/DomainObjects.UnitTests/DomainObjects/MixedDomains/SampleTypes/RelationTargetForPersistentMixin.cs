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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.MixedDomains.SampleTypes
{
  [DBTable ("MixedDomains_RelationTarget")]
  [Instantiable]
  [TestDomain]
  public abstract class RelationTargetForPersistentMixin : SimpleDomainObject<RelationTargetForPersistentMixin>
  {
    [DBBidirectionalRelation ("RelationProperty")]
    public abstract TargetClassForPersistentMixin RelationProperty1 { get; set; }

    [DBBidirectionalRelation ("VirtualRelationProperty", ContainsForeignKey = true)]
    public abstract TargetClassForPersistentMixin RelationProperty2 { get; set; }

    [DBBidirectionalRelation ("CollectionProperty1Side")]
    public abstract TargetClassForPersistentMixin RelationProperty3 { get; set; }

    [DBBidirectionalRelation ("CollectionPropertyNSide")]
    public abstract ObjectList<TargetClassForPersistentMixin> RelationProperty4 { get; }
  }
}
