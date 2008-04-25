using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.MixedDomains
{
  [TestFixture]
  public class PersistentMixinTest : ClientTransactionBaseTest
  {
    [Test]
    public void ClassDefinitionIncludesPersistentProperties ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (TargetClassForPersistentMixin));
      Assert.IsNotNull (classDefinition.GetPropertyDefinition (typeof (MixinAddingPersistentProperties).FullName + ".PersistentProperty"));
      Assert.IsNotNull (classDefinition.GetPropertyDefinition (typeof (MixinAddingPersistentProperties).FullName + ".ExtraPersistentProperty"));
    }

    [Test]
    [Ignore ("TODO: FS - Implement derived mixins")]
    public void ClassDefinitionIncludesPersistentPropertiesFromDerivedMixin ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (TargetClassForDerivedPersistentMixin));
      Assert.IsNotNull (classDefinition.GetPropertyDefinition (typeof (DerivedMixinAddingPersistentProperties).FullName + ".AdditionalPersistentProperty"));
      Assert.IsNotNull (classDefinition.GetPropertyDefinition (typeof (MixinAddingPersistentProperties).FullName + ".PersistentProperty"));
    }

    [Test]
    public void ClassDefinitionExcludesNonPersistentProperties ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (TargetClassForPersistentMixin));
      Assert.IsNull (classDefinition.GetPropertyDefinition (typeof (MixinAddingPersistentProperties).FullName + ".NonPersistentProperty"));
    }

    [Test]
    public void ClassDefinition_RealSide_Unmixed ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Computer));
      PropertyDefinition relationProperty = classDefinition.GetPropertyDefinition (typeof (Computer).FullName + ".Employee");
      RelationDefinition relation = classDefinition.GetRelationDefinition (typeof (Computer).FullName + ".Employee");
      Assert.IsNotNull (relationProperty);
      Assert.IsNotNull (relation);
      Assert.AreEqual (typeof (Computer).FullName + ".Employee", relation.ID);
    }

    [Test]
    public void ClassDefinition_VirtualSide_Unmixed ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Employee));
      PropertyDefinition relationProperty = classDefinition.GetPropertyDefinition (typeof (Employee).FullName + ".Computer");
      RelationDefinition relation = classDefinition.GetRelationDefinition (typeof (Employee).FullName + ".Computer");
      Assert.IsNull (relationProperty);
      Assert.IsNotNull (relation);
      Assert.AreEqual (typeof (Computer).FullName + ".Employee", relation.ID);
    }

    [Test]
    public void ClassDefinition_RealSide_MixedReal ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (TargetClassForPersistentMixin));
      PropertyDefinition relationProperty = classDefinition.GetPropertyDefinition (typeof (MixinAddingPersistentProperties).FullName + ".RelationProperty");
      RelationDefinition relation = classDefinition.GetRelationDefinition (typeof (MixinAddingPersistentProperties).FullName + ".RelationProperty");
      Assert.IsNotNull (relationProperty);
      Assert.IsNotNull (relation);
      Assert.AreEqual (string.Format ("{0}->{1}.RelationProperty", typeof (TargetClassForPersistentMixin).FullName,
          typeof (MixinAddingPersistentProperties).FullName), relation.ID);
    }

    [Test]
    public void ClassDefinition_VirtualSide_MixedReal ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (RelationTargetForPersistentMixin));
      PropertyDefinition relationProperty = classDefinition.GetPropertyDefinition (typeof (RelationTargetForPersistentMixin).FullName + ".RelationProperty1");
      RelationDefinition relation = classDefinition.GetRelationDefinition (typeof (RelationTargetForPersistentMixin).FullName + ".RelationProperty1");
      Assert.IsNull (relationProperty);
      Assert.IsNotNull (relation);
      Assert.AreEqual (string.Format ("{0}->{1}.RelationProperty", typeof (TargetClassForPersistentMixin).FullName,
         typeof (MixinAddingPersistentProperties).FullName), relation.ID);
    }

    [Test]
    public void ClassDefinition_RealSide_MixedVirtual ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (RelationTargetForPersistentMixin));
      PropertyDefinition relationProperty = classDefinition.GetPropertyDefinition (typeof (RelationTargetForPersistentMixin).FullName + ".RelationProperty2");
      RelationDefinition relation = classDefinition.GetRelationDefinition (typeof (RelationTargetForPersistentMixin).FullName + ".RelationProperty2");
      Assert.IsNotNull (relationProperty);
      Assert.IsNotNull (relation);
      Assert.AreEqual (typeof (RelationTargetForPersistentMixin).FullName + ".RelationProperty2", relation.ID);
    }

    [Test]
    public void ClassDefinition_VirtualSide_MixedVirtual ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (TargetClassForPersistentMixin));
      PropertyDefinition relationProperty = classDefinition.GetPropertyDefinition (typeof (MixinAddingPersistentProperties).FullName + ".VirtualRelationProperty");
      RelationDefinition relation = classDefinition.GetRelationDefinition (typeof (MixinAddingPersistentProperties).FullName + ".VirtualRelationProperty");
      Assert.IsNull (relationProperty);
      Assert.IsNotNull (relation);
      Assert.AreEqual (typeof (RelationTargetForPersistentMixin).FullName + ".RelationProperty2", relation.ID);
    }

    [Test]
    public void ClassDefinition_Unidirectional_OneClassTwoMixins ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (TargetClassWithTwoUnidirectionalMixins));
      PropertyDefinition relationProperty1 = classDefinition.GetPropertyDefinition (typeof (MixinAddingUnidirectionalRelation1).FullName + ".Computer");
      PropertyDefinition relationProperty2 = classDefinition.GetPropertyDefinition (typeof (MixinAddingUnidirectionalRelation2).FullName + ".Computer");
      RelationDefinition relation1 = classDefinition.GetRelationDefinition (typeof (MixinAddingUnidirectionalRelation1).FullName + ".Computer");
      RelationDefinition relation2 = classDefinition.GetRelationDefinition (typeof (MixinAddingUnidirectionalRelation2).FullName + ".Computer");
      Assert.IsNotNull (relationProperty1);
      Assert.IsNotNull (relationProperty2);
      Assert.IsNotNull (relation1);
      Assert.IsNotNull (relation2);
      Assert.AreNotSame (relation1, relation2);
      Assert.AreEqual (string.Format ("{0}->{1}.Computer", typeof (TargetClassWithTwoUnidirectionalMixins).FullName,
         typeof (MixinAddingUnidirectionalRelation1).FullName), relation1.ID);
      Assert.AreEqual (string.Format ("{0}->{1}.Computer", typeof (TargetClassWithTwoUnidirectionalMixins).FullName,
         typeof (MixinAddingUnidirectionalRelation2).FullName), relation2.ID);
    }

    [Test]
    public void ClassDefinition_Unidirectional_TwoClassesOneMixin ()
    {
      ClassDefinition classDefinition1 = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (TargetClassWithUnidirectionalMixin1));
      ClassDefinition classDefinition2 = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (TargetClassWithUnidirectionalMixin2));
      PropertyDefinition relationProperty1 = classDefinition1.GetPropertyDefinition (typeof (MixinAddingUnidirectionalRelation1).FullName + ".Computer");
      PropertyDefinition relationProperty2 = classDefinition2.GetPropertyDefinition (typeof (MixinAddingUnidirectionalRelation1).FullName + ".Computer");
      RelationDefinition relation1 = classDefinition1.GetRelationDefinition (typeof (MixinAddingUnidirectionalRelation1).FullName + ".Computer");
      RelationDefinition relation2 = classDefinition2.GetRelationDefinition (typeof (MixinAddingUnidirectionalRelation1).FullName + ".Computer");
      Assert.IsNotNull (relationProperty1);
      Assert.IsNotNull (relationProperty2);
      Assert.IsNotNull (relation1);
      Assert.IsNotNull (relation2);
      Assert.AreNotSame (relation1, relation2);
      Assert.AreEqual (string.Format ("{0}->{1}.Computer", typeof (TargetClassWithUnidirectionalMixin1).FullName,
         typeof (MixinAddingUnidirectionalRelation1).FullName), relation1.ID);
      Assert.AreEqual (string.Format ("{0}->{1}.Computer", typeof (TargetClassWithUnidirectionalMixin2).FullName,
         typeof (MixinAddingUnidirectionalRelation1).FullName), relation2.ID);
    }

    [Test]
    public void RelationTargetClassDefinitionIncludesRelationProperty ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (RelationTargetForPersistentMixin));
      Assert.IsNull (classDefinition.GetPropertyDefinition (typeof (RelationTargetForPersistentMixin).FullName + ".RelationProperty"));
      Assert.IsNotNull (classDefinition.GetRelationDefinition (typeof (RelationTargetForPersistentMixin).FullName + ".RelationProperty1"));
    }


    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "A persistence-related mixin was removed from the domain object type "
       + "Remotion.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes.StubStorageTargetClassForPersistentMixin after the mapping information was built: "
       + "Remotion.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes.StubStoragePersistentMixin.")]
    public void DynamicChangeInPersistentMixinConfigurationThrowsInNewObject ()
    {
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        StubStorageTargetClassForPersistentMixin.NewObject ();
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "A persistence-related mixin was removed from the domain object type "
       + "Remotion.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes.StubStorageTargetClassForPersistentMixin after the mapping information was built: "
        + "Remotion.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes.StubStoragePersistentMixin.")]
    public void DynamicChangeInPersistentMixinConfigurationThrowsInGetObject ()
    {
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        StubStorageTargetClassForPersistentMixin.GetObject (new ObjectID (typeof (StubStorageTargetClassForPersistentMixin), Guid.NewGuid ()));
      }
    }

    [Test]
    public void DynamicChangeInNonPersistentMixinConfigurationDoesntMatter ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass (typeof (StubStorageTargetClassForPersistentMixin)).Clear ().AddMixin (typeof (StubStoragePersistentMixin)).EnterScope ()) // no NullMixin
      {
        StubStorageTargetClassForPersistentMixin.NewObject ();
        StubStorageTargetClassForPersistentMixin.GetObject (new ObjectID (typeof (StubStorageTargetClassForPersistentMixin), Guid.NewGuid ()));
      }
    }

    [Test]
    public void UnidirectionalRelationProperty ()
    {
      TargetClassForPersistentMixin tc = TargetClassForPersistentMixin.NewObject ();
      RelationTargetForPersistentMixin relationTarget = RelationTargetForPersistentMixin.NewObject ().With ();
      MixinAddingPersistentProperties mixin = Mixin.Get<MixinAddingPersistentProperties> (tc);
      mixin.UnidirectionalRelationProperty = relationTarget;
      Assert.AreSame (relationTarget, mixin.UnidirectionalRelationProperty);
    }

    [Test]
    public void RelationPropertyRealSide ()
    {
      TargetClassForPersistentMixin  tc = TargetClassForPersistentMixin.NewObject ();
      RelationTargetForPersistentMixin relationTarget = RelationTargetForPersistentMixin.NewObject().With();
      MixinAddingPersistentProperties mixin = Mixin.Get<MixinAddingPersistentProperties> (tc);
      mixin.RelationProperty = relationTarget;
      Assert.AreSame (relationTarget, mixin.RelationProperty);
      Assert.AreSame (tc, relationTarget.RelationProperty1);
    }

    [Test]
    public void VirtualRelationProperty ()
    {
      TargetClassForPersistentMixin tc = TargetClassForPersistentMixin.NewObject ();
      RelationTargetForPersistentMixin relationTarget = RelationTargetForPersistentMixin.NewObject ().With ();
      MixinAddingPersistentProperties mixin = Mixin.Get<MixinAddingPersistentProperties> (tc);
      mixin.VirtualRelationProperty = relationTarget;
      Assert.AreSame (relationTarget, mixin.VirtualRelationProperty);
      Assert.AreSame (tc, relationTarget.RelationProperty2);
    }

    [Test]
    public void CollectionProperty1Side ()
    {
      TargetClassForPersistentMixin tc = TargetClassForPersistentMixin.NewObject ();
      RelationTargetForPersistentMixin relationTarget1 = RelationTargetForPersistentMixin.NewObject ().With ();
      RelationTargetForPersistentMixin relationTarget2 = RelationTargetForPersistentMixin.NewObject ().With ();
      MixinAddingPersistentProperties mixin = Mixin.Get<MixinAddingPersistentProperties> (tc);
      mixin.CollectionProperty1Side.Add (relationTarget1);
      mixin.CollectionProperty1Side.Add (relationTarget2);
      Assert.AreSame (relationTarget1, mixin.CollectionProperty1Side[0]);
      Assert.AreSame (relationTarget2, mixin.CollectionProperty1Side[1]);
      Assert.AreSame (tc, relationTarget1.RelationProperty3);
      Assert.AreSame (tc, relationTarget2.RelationProperty3);
    }

    [Test]
    public void CollectionPropertyNSide ()
    {
      TargetClassForPersistentMixin tc1 = TargetClassForPersistentMixin.NewObject ();
      TargetClassForPersistentMixin tc2 = TargetClassForPersistentMixin.NewObject ();
      RelationTargetForPersistentMixin relationTarget = RelationTargetForPersistentMixin.NewObject ().With ();
      MixinAddingPersistentProperties mixin1 = Mixin.Get<MixinAddingPersistentProperties> (tc1);
      MixinAddingPersistentProperties mixin2 = Mixin.Get<MixinAddingPersistentProperties> (tc2);
      mixin1.CollectionPropertyNSide = relationTarget;
      mixin2.CollectionPropertyNSide = relationTarget;
      Assert.AreSame (relationTarget, mixin1.CollectionPropertyNSide);
      Assert.AreSame (relationTarget, mixin2.CollectionPropertyNSide);
      Assert.AreSame (tc1, relationTarget.RelationProperty4[0]);
      Assert.AreSame (tc2, relationTarget.RelationProperty4[1]);
    }

    [Test]
    [Ignore ("TODO: FS - Implement derived mixins")]
    public void DerivedMixin ()
    {
      TargetClassForDerivedPersistentMixin tc = TargetClassForDerivedPersistentMixin.NewObject().With();
      DerivedMixinAddingPersistentProperties mixin = Mixin.Get<DerivedMixinAddingPersistentProperties> (tc);
      mixin.AdditionalPersistentProperty = 12;
      Assert.AreEqual (12, mixin.AdditionalPersistentProperty);
      mixin.PersistentProperty = 10;
      Assert.AreEqual (10, mixin.PersistentProperty);
    }
  }
}