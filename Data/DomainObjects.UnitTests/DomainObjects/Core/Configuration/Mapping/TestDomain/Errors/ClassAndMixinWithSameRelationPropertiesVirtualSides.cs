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
using Remotion.Mixins;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors
{
  [DBTable]
  [Instantiable]
  [Uses (typeof (MixinVirtualSides))]
  public abstract class TargetClassWithRelationPropertyVirtualSides : DomainObject
  {
    [DBBidirectionalRelation ("Opposite", ContainsForeignKey = false)]
    [DBColumn("One")]
    public abstract RelationTargetVirtualSides Relation { get; set; }
  }

  [DBTable]
  [Instantiable]
  public abstract class RelationTargetVirtualSides : DomainObject
  {
    [DBBidirectionalRelation ("Relation", ContainsForeignKey = true)]
    public abstract TargetClassWithRelationPropertyVirtualSides Opposite { get; set; }
  }

  public class MixinVirtualSides : DomainObjectMixin<TargetClassWithRelationPropertyVirtualSides>
  {
    [DBBidirectionalRelation ("Opposite", ContainsForeignKey = false)]
    [DBColumn ("Two")]
    public RelationTargetVirtualSides Relation
    {
      get { return Properties[typeof (MixinVirtualSides), "Relation"].GetValue<RelationTargetVirtualSides> (); }
      set { Properties[typeof (MixinVirtualSides), "Relation"].SetValue (value); }
    }
  }
}
