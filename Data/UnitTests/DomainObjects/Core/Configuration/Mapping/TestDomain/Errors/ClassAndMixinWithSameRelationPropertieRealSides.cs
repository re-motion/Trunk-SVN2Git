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
using Remotion.Mixins;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors
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
