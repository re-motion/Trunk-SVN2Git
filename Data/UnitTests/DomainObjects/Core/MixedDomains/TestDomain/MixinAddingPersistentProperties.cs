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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain
{
  public class MixinAddingPersistentProperties : BaseForMixinAddingPersistentProperties, IMixinAddingPeristentProperties
  {
    private int _nonPersistentProperty = 0;

    public int PersistentProperty
    {
      get { return Properties[typeof (MixinAddingPersistentProperties), "PersistentProperty"].GetValue<int>(); }
      set { Properties[typeof (MixinAddingPersistentProperties), "PersistentProperty"].SetValue (value); }
    }

    [StorageClass (StorageClass.Persistent)]
    public int ExtraPersistentProperty
    {
      get { return Properties[typeof (MixinAddingPersistentProperties), "ExtraPersistentProperty"].GetValue<int> (); }
      set { Properties[typeof (MixinAddingPersistentProperties), "ExtraPersistentProperty"].SetValue (value); }
    }

    [StorageClassNone]
    public int NonPersistentProperty
    {
      get { return _nonPersistentProperty; }
      set { _nonPersistentProperty = value; }
    }

    public RelationTargetForPersistentMixin UnidirectionalRelationProperty
    {
      get { return Properties[typeof (MixinAddingPersistentProperties), "UnidirectionalRelationProperty"].GetValue<RelationTargetForPersistentMixin> (); }
      set { Properties[typeof (MixinAddingPersistentProperties), "UnidirectionalRelationProperty"].SetValue (value); }
    }

    [DBBidirectionalRelation ("RelationProperty1", ContainsForeignKey = true)]
    public RelationTargetForPersistentMixin RelationProperty
    {
      get { return Properties[typeof (MixinAddingPersistentProperties), "RelationProperty"].GetValue<RelationTargetForPersistentMixin>(); }
      set { Properties[typeof (MixinAddingPersistentProperties), "RelationProperty"].SetValue (value); }
    }

    [DBBidirectionalRelation ("RelationProperty2", ContainsForeignKey = false)]
    public RelationTargetForPersistentMixin VirtualRelationProperty
    {
      get { return Properties[typeof (MixinAddingPersistentProperties), "VirtualRelationProperty"].GetValue<RelationTargetForPersistentMixin> (); }
      set { Properties[typeof (MixinAddingPersistentProperties), "VirtualRelationProperty"].SetValue (value); }
    }

    [DBBidirectionalRelation ("RelationProperty3")]
    public ObjectList<RelationTargetForPersistentMixin> CollectionProperty1Side
    {
      get { return Properties[typeof (MixinAddingPersistentProperties), "CollectionProperty1Side"].GetValue<ObjectList<RelationTargetForPersistentMixin>> (); }
    }

    [DBBidirectionalRelation ("RelationProperty4")]
    public RelationTargetForPersistentMixin CollectionPropertyNSide
    {
      get { return Properties[typeof (MixinAddingPersistentProperties), "CollectionPropertyNSide"].GetValue<RelationTargetForPersistentMixin> (); }
      set { Properties[typeof (MixinAddingPersistentProperties), "CollectionPropertyNSide"].SetValue (value); }
    }
  }
}
