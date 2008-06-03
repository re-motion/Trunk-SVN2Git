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

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.TestDomain.Errors
{
  [DBTable]
  [Instantiable]
  [Uses (typeof (MixinRealSides))]
  public abstract class TargetClassWithRelationPropertyRealSides : DomainObject
  {
    [DBBidirectionalRelation ("Opposite", ContainsForeignKey = true)]
    [DBColumn("One")]
    public abstract RelationTargetRealSides Relation { get; set; }
  }

  [DBTable]
  [Instantiable]
  public abstract class RelationTargetRealSides : DomainObject
  {
    [DBBidirectionalRelation ("Relation", ContainsForeignKey = false)]
    public abstract TargetClassWithRelationPropertyRealSides Opposite { get; set; }
  }

  public class MixinRealSides : DomainObjectMixin<TargetClassWithRelationPropertyRealSides>
  {
    [DBBidirectionalRelation ("Opposite", ContainsForeignKey = true)]
    [DBColumn ("Two")]
    public RelationTargetRealSides Relation
    {
      get { return Properties[typeof (MixinRealSides), "Relation"].GetValue<RelationTargetRealSides> (); }
      set { Properties[typeof (MixinRealSides), "Relation"].SetValue (value); }
    }
  }
}
