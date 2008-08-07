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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.SampleTypes
{
  [DBTable]
  [IgnoreForMappingConfiguration]
  public abstract class RelationTargetForPersistentMixinAboveInheritanceRoot : SimpleDomainObject<RelationTargetForPersistentMixinAboveInheritanceRoot>
  {
    [DBBidirectionalRelation("PersistentRelationProperty", ContainsForeignKey = false)]
    public abstract TargetClassAboveInheritanceRoot RelationProperty1 { get; set; }
  }
}