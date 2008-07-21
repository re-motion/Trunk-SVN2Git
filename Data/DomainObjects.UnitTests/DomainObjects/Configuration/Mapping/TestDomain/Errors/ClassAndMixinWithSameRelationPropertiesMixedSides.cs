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
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Configuration.Mapping.TestDomain.Errors
{
  [DBTable]
  [Instantiable]
  [Uses (typeof (MixinMixedSides))]
  public abstract class TargetClassWithRelationPropertyMixedSides : DomainObject
  {
    [DBBidirectionalRelation ("Opposite", ContainsForeignKey = true)]
    [DBColumn("One")]
    public abstract RelationTargetMixedSides Relation { get; set; }
  }

  [DBTable]
  [Instantiable]
  public abstract class RelationTargetMixedSides : DomainObject
  {
    [DBBidirectionalRelation ("Relation", ContainsForeignKey = false)]
    public abstract TargetClassWithRelationPropertyMixedSides Opposite { get; set; }
  }

  public class MixinMixedSides : DomainObjectMixin<TargetClassWithRelationPropertyMixedSides>
  {
    [DBBidirectionalRelation ("Opposite", ContainsForeignKey = false)]
    [DBColumn ("Two")]
    public RelationTargetMixedSides Relation
    {
      get { return Properties[typeof (MixinMixedSides), "Relation"].GetValue<RelationTargetMixedSides> (); }
      set { Properties[typeof (MixinMixedSides), "Relation"].SetValue (value); }
    }
  }
}
