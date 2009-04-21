// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Mixins;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains
{
  [TestFixture]
  public class PersistentMixinIntegrationTest : ClientTransactionBaseTest
  {
    [Test]
    public void ClassDefinitionIncludesPersistentProperties ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (TargetClassForPersistentMixin));
      Assert.IsNotNull (classDefinition.GetPropertyDefinition (typeof (MixinAddingPersistentProperties).FullName + ".PersistentProperty"));
      Assert.IsNotNull (classDefinition.GetPropertyDefinition (typeof (MixinAddingPersistentProperties).FullName + ".ExtraPersistentProperty"));
    }

    [Test]
    public void ClassDefinitionIncludesPersistentPropertiesFromDerivedMixin ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (TargetClassForDerivedPersistentMixin));
      Assert.IsNotNull (classDefinition.GetPropertyDefinition (typeof (DerivedMixinAddingPersistentProperties).FullName + ".AdditionalPersistentProperty"));
      Assert.IsNotNull (classDefinition.GetPropertyDefinition (typeof (MixinAddingSimplePersistentProperties).FullName + ".PersistentProperty"));
    }

    [Test]
    public void ClassDefinitionExcludesNonPersistentProperties ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (TargetClassForPersistentMixin));
      Assert.IsNull (classDefinition.GetPropertyDefinition (typeof (MixinAddingPersistentProperties).FullName + ".NonPersistentProperty"));
    }

    [Test]
    public void ClassDefinition_RealSide_Unmixed ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Computer));
      var relationProperty = classDefinition.GetPropertyDefinition (typeof (Computer).FullName + ".Employee");
      var relation = classDefinition.GetRelationDefinition (typeof (Computer).FullName + ".Employee");
      Assert.IsNotNull (relationProperty);
      Assert.IsNotNull (relation);
      Assert.AreEqual (typeof (Computer).FullName + ".Employee", relation.ID);
    }

    [Test]
    public void ClassDefinition_VirtualSide_Unmixed ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Employee));
      var relationProperty = classDefinition.GetPropertyDefinition (typeof (Employee).FullName + ".Computer");
      var relation = classDefinition.GetRelationDefinition (typeof (Employee).FullName + ".Computer");
      Assert.IsNull (relationProperty);
      Assert.IsNotNull (relation);
      Assert.AreEqual (typeof (Computer).FullName + ".Employee", relation.ID);
    }

    [Test]
    public void ClassDefinition_RealSide_MixedReal ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (TargetClassForPersistentMixin));
      var relationProperty = classDefinition.GetPropertyDefinition (typeof (MixinAddingPersistentProperties).FullName + ".RelationProperty");
      var relation = classDefinition.GetRelationDefinition (typeof (MixinAddingPersistentProperties).FullName + ".RelationProperty");
      Assert.IsNotNull (relationProperty);
      Assert.IsNotNull (relation);
      Assert.AreEqual (string.Format ("{0}->{1}.RelationProperty", typeof (TargetClassForPersistentMixin).FullName,
          typeof (MixinAddingPersistentProperties).FullName), relation.ID);
    }

    [Test]
    public void ClassDefinition_VirtualSide_MixedReal ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (RelationTargetForPersistentMixin));
      var relationProperty = classDefinition.GetPropertyDefinition (typeof (RelationTargetForPersistentMixin).FullName + ".RelationProperty1");
      var relation = classDefinition.GetRelationDefinition (typeof (RelationTargetForPersistentMixin).FullName + ".RelationProperty1");
      Assert.IsNull (relationProperty);
      Assert.IsNotNull (relation);
      Assert.AreEqual (string.Format ("{0}->{1}.RelationProperty", typeof (TargetClassForPersistentMixin).FullName,
         typeof (MixinAddingPersistentProperties).FullName), relation.ID);
    }

    [Test]
    public void ClassDefinition_RealSide_MixedVirtual ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (RelationTargetForPersistentMixin));
      var relationProperty = classDefinition.GetPropertyDefinition (typeof (RelationTargetForPersistentMixin).FullName + ".RelationProperty2");
      var relation = classDefinition.GetRelationDefinition (typeof (RelationTargetForPersistentMixin).FullName + ".RelationProperty2");
      Assert.IsNotNull (relationProperty);
      Assert.IsNotNull (relation);
      Assert.AreEqual (typeof (RelationTargetForPersistentMixin).FullName + ".RelationProperty2", relation.ID);
    }

    [Test]
    public void ClassDefinition_VirtualSide_MixedVirtual ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (TargetClassForPersistentMixin));
      var relationProperty = classDefinition.GetPropertyDefinition (typeof (MixinAddingPersistentProperties).FullName + ".VirtualRelationProperty");
      var relation = classDefinition.GetRelationDefinition (typeof (MixinAddingPersistentProperties).FullName + ".VirtualRelationProperty");
      Assert.IsNull (relationProperty);
      Assert.IsNotNull (relation);
      Assert.AreEqual (typeof (RelationTargetForPersistentMixin).FullName + ".RelationProperty2", relation.ID);
    }

    [Test]
    public void ClassDefinition_Unidirectional_OneClassTwoMixins ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (TargetClassWithTwoUnidirectionalMixins));
      var relationProperty1 = classDefinition.GetPropertyDefinition (typeof (MixinAddingUnidirectionalRelation1).FullName + ".Computer");
      var relationProperty2 = classDefinition.GetPropertyDefinition (typeof (MixinAddingUnidirectionalRelation2).FullName + ".Computer");
      var relation1 = classDefinition.GetRelationDefinition (typeof (MixinAddingUnidirectionalRelation1).FullName + ".Computer");
      var relation2 = classDefinition.GetRelationDefinition (typeof (MixinAddingUnidirectionalRelation2).FullName + ".Computer");
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
      var classDefinition1 = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (TargetClassWithUnidirectionalMixin1));
      var classDefinition2 = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (TargetClassWithUnidirectionalMixin2));
      var relationProperty1 = classDefinition1.GetPropertyDefinition (typeof (MixinAddingUnidirectionalRelation1).FullName + ".Computer");
      var relationProperty2 = classDefinition2.GetPropertyDefinition (typeof (MixinAddingUnidirectionalRelation1).FullName + ".Computer");
      var relation1 = classDefinition1.GetRelationDefinition (typeof (MixinAddingUnidirectionalRelation1).FullName + ".Computer");
      var relation2 = classDefinition2.GetRelationDefinition (typeof (MixinAddingUnidirectionalRelation1).FullName + ".Computer");
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
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (RelationTargetForPersistentMixin));
      Assert.IsNull (classDefinition.GetPropertyDefinition (typeof (RelationTargetForPersistentMixin).FullName + ".RelationProperty"));
      Assert.IsNotNull (classDefinition.GetRelationDefinition (typeof (RelationTargetForPersistentMixin).FullName + ".RelationProperty1"));
    }


    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "A persistence-related mixin was removed from the domain object type "
       + "Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain.StubStorageTargetClassForPersistentMixin after the mapping information was built: "
       + "Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain.StubStoragePersistentMixin.")]
    public void DynamicChangeInPersistentMixinConfigurationThrowsInNewObject ()
    {
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        StubStorageTargetClassForPersistentMixin.NewObject ();
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "A persistence-related mixin was removed from the domain object type "
       + "Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain.StubStorageTargetClassForPersistentMixin after the mapping information was built: "
        + "Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain.StubStoragePersistentMixin.")]
    public void DynamicChangeInPersistentMixinConfigurationThrowsInGetObject ()
    {
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        StubStorageTargetClassForPersistentMixin.GetObject (new ObjectID (typeof (StubStorageTargetClassForPersistentMixin), 13));
      }
    }

    [Test]
    public void DynamicChangeInNonPersistentMixinConfigurationDoesntMatter ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass (typeof (StubStorageTargetClassForPersistentMixin)).Clear ().AddMixin (typeof (StubStoragePersistentMixin)).EnterScope ()) // no NullMixin
      {
        StubStorageTargetClassForPersistentMixin.NewObject ();
        StubStorageTargetClassForPersistentMixin.GetObject (new ObjectID (typeof (StubStorageTargetClassForPersistentMixin), 12));
      }
    }

    [Test]
    public void UnidirectionalRelationProperty ()
    {
      var tc = TargetClassForPersistentMixin.NewObject ();
      var relationTarget = RelationTargetForPersistentMixin.NewObject ();
      var mixin = Mixin.Get<MixinAddingPersistentProperties> (tc);
      mixin.UnidirectionalRelationProperty = relationTarget;
      Assert.AreSame (relationTarget, mixin.UnidirectionalRelationProperty);
    }

    [Test]
    public void RelationPropertyRealSide ()
    {
      var  tc = TargetClassForPersistentMixin.NewObject ();
      var relationTarget = RelationTargetForPersistentMixin.NewObject();
      var mixin = Mixin.Get<MixinAddingPersistentProperties> (tc);
      mixin.RelationProperty = relationTarget;
      Assert.AreSame (relationTarget, mixin.RelationProperty);
      Assert.AreSame (tc, relationTarget.RelationProperty1);
    }

    [Test]
    public void VirtualRelationProperty ()
    {
      var tc = TargetClassForPersistentMixin.NewObject ();
      var relationTarget = RelationTargetForPersistentMixin.NewObject ();
      var mixin = Mixin.Get<MixinAddingPersistentProperties> (tc);
      mixin.VirtualRelationProperty = relationTarget;
      Assert.AreSame (relationTarget, mixin.VirtualRelationProperty);
      Assert.AreSame (tc, relationTarget.RelationProperty2);
    }

    [Test]
    public void CollectionProperty1Side ()
    {
      var tc = TargetClassForPersistentMixin.NewObject ();
      var relationTarget1 = RelationTargetForPersistentMixin.NewObject ();
      var relationTarget2 = RelationTargetForPersistentMixin.NewObject ();
      var mixin = Mixin.Get<MixinAddingPersistentProperties> (tc);
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
      var tc1 = TargetClassForPersistentMixin.NewObject ();
      var tc2 = TargetClassForPersistentMixin.NewObject ();
      var relationTarget = RelationTargetForPersistentMixin.NewObject ();
      var mixin1 = Mixin.Get<MixinAddingPersistentProperties> (tc1);
      var mixin2 = Mixin.Get<MixinAddingPersistentProperties> (tc2);
      mixin1.CollectionPropertyNSide = relationTarget;
      mixin2.CollectionPropertyNSide = relationTarget;
      Assert.AreSame (relationTarget, mixin1.CollectionPropertyNSide);
      Assert.AreSame (relationTarget, mixin2.CollectionPropertyNSide);
      Assert.AreSame (tc1, relationTarget.RelationProperty4[0]);
      Assert.AreSame (tc2, relationTarget.RelationProperty4[1]);
    }

    [Test]
    public void DerivedMixin ()
    {
      var tc = TargetClassForDerivedPersistentMixin.NewObject();
      var mixin = Mixin.Get<DerivedMixinAddingPersistentProperties> (tc);
      mixin.AdditionalPersistentProperty = 12;
      Assert.AreEqual (12, mixin.AdditionalPersistentProperty);
      mixin.PersistentProperty = 10;
      Assert.AreEqual (10, mixin.PersistentProperty);
    }

    [Test]
    public void TargetClassAboveInheritanceRoot ()
    {
      var tc = InheritanceRootInheritingPersistentMixin.NewObject ();
      var mixin = Mixin.Get<MixinAddingPersistentPropertiesAboveInheritanceRoot> (tc);
      mixin.PersistentProperty = 10;
      Assert.AreEqual (10, mixin.PersistentProperty);
    }

    [Test]
    public void RelationOnTargetClassAboveInheritanceRoot ()
    {
      var tc = InheritanceRootInheritingPersistentMixin.NewObject ();
      var mixin = Mixin.Get<MixinAddingPersistentPropertiesAboveInheritanceRoot> (tc);
      var relationTarget = RelationTargetForPersistentMixinAboveInheritanceRoot.NewObject ();
      mixin.PersistentRelationProperty = relationTarget;
      Assert.That (mixin.PersistentRelationProperty, Is.SameAs (relationTarget));
    }

    [Test]
    public void BaseClassReceivingReferenceToDerivedClass ()
    {
      var tc = TargetClassReceivingReferenceToDerivedClass.NewObject();
      var mixin = Mixin.Get<MixinAddingReferenceToDerivedClass> (tc);
      var relationTarget = DerivedClassWithBaseReferenceViaMixin.NewObject();
      mixin.MyDerived.Add (relationTarget);
      Assert.That (relationTarget.MyBase, Is.SameAs (tc));
      relationTarget.MyBase = null;
      Assert.That (mixin.MyDerived, Is.Empty);
    }

    [Test]
    public void BaseClassReceivingTwoReferencesToDerivedClass ()
    {
      var tc = TargetClassReceivingTwoReferencesToDerivedClass.NewObject ();
      var mixin1 = Mixin.Get<MixinAddingTwoReferencesToDerivedClass1> (tc);
      var mixin2 = Mixin.Get<MixinAddingTwoReferencesToDerivedClass2> (tc);
      var relationTarget = DerivedClassWithTwoBaseReferencesViaMixins.NewObject ();
      mixin1.MyDerived1.Add (relationTarget);
      mixin2.MyDerived2.Add (relationTarget);
      Assert.That (relationTarget.MyBase1, Is.SameAs (tc));
      Assert.That (relationTarget.MyBase2, Is.SameAs (tc));
      relationTarget.MyBase1 = null;
      relationTarget.MyBase2 = null;
      Assert.That (mixin1.MyDerived1, Is.Empty);
      Assert.That (mixin2.MyDerived2, Is.Empty);
    }
  }
}
