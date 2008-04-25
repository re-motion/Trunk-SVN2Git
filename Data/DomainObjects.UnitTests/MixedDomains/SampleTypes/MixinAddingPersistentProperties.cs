using System;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Remotion.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes
{
  public class MixinAddingPersistentProperties : DomainObjectMixin<DomainObject>, IMixinAddingPeristentProperties
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

  //public class MixinAddingPersistentProperties2 : DomainObjectMixin<DomainObject>
  //{
  //  [DBColumn("Storm")]
  //  public int PersistentProperty
  //  {
  //    get { return Properties[typeof (MixinAddingPersistentProperties2), "PersistentProperty"].GetValue<int> (); }
  //    set { Properties[typeof (MixinAddingPersistentProperties2), "PersistentProperty"].SetValue (value); }
  //  }

  //  [DBBidirectionalRelation ("RelationProperty4")]
  //  [DBColumn ("Winter")]
  //  public RelationTargetForPersistentMixin CollectionPropertyNSide
  //  {
  //    get { return Properties[typeof (MixinAddingPersistentProperties2), "CollectionPropertyNSide"].GetValue<RelationTargetForPersistentMixin> (); }
  //    set { Properties[typeof (MixinAddingPersistentProperties2), "CollectionPropertyNSide"].SetValue (value); }
  //  }
  //}
}