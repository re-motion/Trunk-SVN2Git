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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  [TestFixture]
  public class RelationReflectorWithMixinsTest : StandardMappingTest
  {
    private ReflectionBasedClassDefinition _mixinTargetClassDefinition;
    private ReflectionBasedClassDefinition _multiMixinTargetClassDefinition;
    private ReflectionBasedClassDefinition _multiMixinRelatedClassDefinition;
    private ReflectionBasedClassDefinition _relatedClassDefinition;
    private ReflectionBasedClassDefinition _inheritanceRootInheritingMixinClassDefinition;

    private ClassDefinitionCollection _classDefinitions;
    private RelationDefinitionCollection _relationDefinitions;

    public override void SetUp ()
    {
      base.SetUp();

      _mixinTargetClassDefinition = 
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (TargetClassForPersistentMixin), typeof (MixinAddingPersistentProperties));
      _multiMixinTargetClassDefinition =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (TargetClassReceivingTwoReferencesToDerivedClass), typeof (MixinAddingTwoReferencesToDerivedClass1), typeof (MixinAddingTwoReferencesToDerivedClass2));
      _multiMixinRelatedClassDefinition =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (DerivedClassWithTwoBaseReferencesViaMixins));
      _relatedClassDefinition = 
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (RelationTargetForPersistentMixin), typeof (MixinAddingPersistentProperties));
      _inheritanceRootInheritingMixinClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition("InheritanceRootInheritingPersistentMixin", 
          "InheritanceRootInheritingPersistentMixin", "TestDomain", typeof (InheritanceRootInheritingPersistentMixin), false, 
          new PersistentMixinFinder(typeof (InheritanceRootInheritingPersistentMixin), true));

      _classDefinitions = new ClassDefinitionCollection {_mixinTargetClassDefinition, _relatedClassDefinition, _multiMixinTargetClassDefinition};
      _relationDefinitions = new RelationDefinitionCollection();
    }

    [Test]
    public void IsMixedProperty_True ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_mixinTargetClassDefinition, typeof (MixinAddingPersistentProperties), 
          "UnidirectionalRelationProperty");
      Assert.That (relationReflector.IsMixedProperty, Is.True);
    }

    [Test]
    public void DeclaringMixin ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_mixinTargetClassDefinition, typeof (MixinAddingPersistentProperties),
          "UnidirectionalRelationProperty");
      Assert.That (relationReflector.DeclaringMixin, Is.SameAs (typeof (MixinAddingPersistentProperties)));
    }

    [Test]
    public void DomainObjectTypeDeclaringProperty ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_mixinTargetClassDefinition, typeof (MixinAddingPersistentProperties),
          "PersistentProperty");
      Assert.That (relationReflector.DeclaringDomainObjectTypeForProperty, Is.EqualTo (typeof (TargetClassForPersistentMixin)));
    }

    [Test]
    public void DomainObjectTypeDeclaringProperty_WithMixinAboveInheritanceRoot ()
    {
      var relationReflector = CreateRelationReflectorForProperty(_inheritanceRootInheritingMixinClassDefinition, 
          typeof (MixinAddingPersistentPropertiesAboveInheritanceRoot), "PersistentRelationProperty");
      Assert.That (relationReflector.DeclaringDomainObjectTypeForProperty, Is.EqualTo (typeof (TargetClassAboveInheritanceRoot)));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "IPersistentMixinFinder.FindOriginalMixinTarget (DeclaringMixin) "
        + "evaluated and returned null.")]
    public void DomainObjectTypeDeclaringProperty_OriginalMixinTargetNull ()
    {
      var mockRepository = new MockRepository ();
      var mixinFinderMock = mockRepository.StrictMock<IPersistentMixinFinder> ();

      Expect.Call (mixinFinderMock.GetPersistentMixins ()).Return (new[] { typeof (MixinAddingPersistentProperties) });
      Expect.Call (mixinFinderMock.FindOriginalMixinTarget (typeof (MixinAddingPersistentProperties))).Return (null);

      mockRepository.ReplayAll ();

      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order), false,
          mixinFinderMock);
      CreateRelationReflectorForProperty (classDefinition, typeof (MixinAddingPersistentProperties), "UnidirectionalRelationProperty");
    }

    [Test]
    public void GetMetadata_Mixed_RealSide_ID ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_mixinTargetClassDefinition,
          typeof (MixinAddingPersistentProperties), "UnidirectionalRelationProperty");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (actualRelationDefinition.ID,
          Is.EqualTo (typeof (TargetClassForPersistentMixin).FullName + "->" + 
          typeof (MixinAddingPersistentProperties).FullName + ".UnidirectionalRelationProperty"));
    }

    [Test]
    public void GetMetadata_Mixed_VirtualSide ()
    {
      PropertyInfo propertyInfo = typeof (MixinAddingPersistentProperties).GetProperty ("VirtualRelationProperty");
      var relationReflector = new RelationReflector (_mixinTargetClassDefinition, propertyInfo, new ReflectionBasedNameResolver ());

      Assert.That (relationReflector.GetMetadata (_classDefinitions, _relationDefinitions), Is.Null);
      Assert.That (_relationDefinitions.Count, Is.EqualTo (0));
    }

    [Test]
    public void GetMetadata_Mixed_Unidirectional_AddedToRightClassDefinition ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_mixinTargetClassDefinition,
          typeof (MixinAddingPersistentProperties), "UnidirectionalRelationProperty");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (_mixinTargetClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));
      Assert.That (_relatedClassDefinition.MyRelationDefinitions, List.Not.Contains (actualRelationDefinition));

      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions, List.Contains (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_Mixed_Unidirectional_EndPointDefinition0 ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_mixinTargetClassDefinition,
          typeof (MixinAddingPersistentProperties), "UnidirectionalRelationProperty");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[0], Is.InstanceOfType (typeof (RelationEndPointDefinition)));
      var endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];

      Assert.That (endPointDefinition.PropertyDefinition, Is.EqualTo (_mixinTargetClassDefinition.GetPropertyDefinitions ()[0]));
      Assert.That (endPointDefinition.ClassDefinition, Is.SameAs (_mixinTargetClassDefinition));
      Assert.That (endPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_Mixed_Unidirectional_EndPointDefinition1 ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_mixinTargetClassDefinition,
          typeof (MixinAddingPersistentProperties), "UnidirectionalRelationProperty");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[1], Is.InstanceOfType (typeof (AnonymousRelationEndPointDefinition)));
      var oppositeEndPointDefinition = (AnonymousRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];

      Assert.That (oppositeEndPointDefinition.ClassDefinition, Is.SameAs (_relatedClassDefinition));
      Assert.That (oppositeEndPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_Mixed_BidirectionalOneToOne_AddedToBothClassDefinitions ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_mixinTargetClassDefinition,
          typeof (MixinAddingPersistentProperties), "RelationProperty");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (_relatedClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));
      Assert.That (_mixinTargetClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions, List.Contains (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_Mixed_BidirectionalOneToOne_EndPointDefinition0 ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_mixinTargetClassDefinition, 
          typeof (MixinAddingPersistentProperties), "RelationProperty");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[0], Is.InstanceOfType (typeof (RelationEndPointDefinition)));
      var endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];

      Assert.That (endPointDefinition.PropertyDefinition, Is.EqualTo (_mixinTargetClassDefinition.GetPropertyDefinitions()[0]));
      Assert.That (endPointDefinition.ClassDefinition, Is.SameAs (_mixinTargetClassDefinition));
      Assert.That (endPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_Mixed_BidirectionalOneToOne_EndPointDefinition1 ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_mixinTargetClassDefinition,
          typeof (MixinAddingPersistentProperties), "RelationProperty");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[1], Is.InstanceOfType (typeof (VirtualRelationEndPointDefinition)));
      var oppositeEndPointDefinition = (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];

      Assert.That (oppositeEndPointDefinition.ClassDefinition, Is.SameAs (_relatedClassDefinition));
      Assert.That (oppositeEndPointDefinition.PropertyName,
          Is.EqualTo ("Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain.RelationTargetForPersistentMixin.RelationProperty1"));
      Assert.That (oppositeEndPointDefinition.PropertyType, Is.SameAs (typeof (TargetClassForPersistentMixin)));
      Assert.That (oppositeEndPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_Mixed_BidirectionalOneToMany_AddedToBothClassDefinitions ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_mixinTargetClassDefinition,
          typeof (MixinAddingPersistentProperties), "CollectionPropertyNSide");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (_mixinTargetClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));
      Assert.That (_relatedClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions, List.Contains (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_Mixed_BidirectionalOneToMany_EndPoint0 ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_mixinTargetClassDefinition,
          typeof (MixinAddingPersistentProperties), "CollectionPropertyNSide");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[0], Is.InstanceOfType (typeof (RelationEndPointDefinition)));
      var endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];
      Assert.That (endPointDefinition.PropertyDefinition, Is.EqualTo (_mixinTargetClassDefinition.GetPropertyDefinitions()[0]));
      Assert.That (endPointDefinition.ClassDefinition, Is.SameAs (_mixinTargetClassDefinition));
      Assert.That (endPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_Mixed_BidirectionalOneToMany_EndPoint1 ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_mixinTargetClassDefinition,
          typeof (MixinAddingPersistentProperties), "CollectionPropertyNSide");
      
      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[1], Is.InstanceOfType (typeof (VirtualRelationEndPointDefinition)));
      var oppositeEndPointDefinition = (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.That (oppositeEndPointDefinition.ClassDefinition, Is.SameAs (_relatedClassDefinition));
      Assert.That (oppositeEndPointDefinition.PropertyName,
          Is.EqualTo ("Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain.RelationTargetForPersistentMixin.RelationProperty4"));
      Assert.That (oppositeEndPointDefinition.PropertyType, Is.SameAs (typeof (ObjectList<TargetClassForPersistentMixin>)));
      Assert.That (oppositeEndPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_Mixed_OppositePropertyPrivateOnBase ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_mixinTargetClassDefinition, 
          typeof (BaseForMixinAddingPersistentProperties), "PrivateBaseRelationProperty");

      Assert.That (relationReflector.GetMetadata (_classDefinitions, _relationDefinitions), Is.Not.Null);
      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions[0].ID, NUnit.Framework.SyntaxHelpers.Text.Contains (relationReflector.PropertyInfo.Name));
    }

    [Test]
    public void GetMetadata_Mixed_TwoMixins ()
    {
      var relationReflector1 = CreateRelationReflectorForProperty (_multiMixinRelatedClassDefinition,
          typeof (DerivedClassWithTwoBaseReferencesViaMixins), "MyBase1");
      var relationReflector2 = CreateRelationReflectorForProperty (_multiMixinRelatedClassDefinition,
          typeof (DerivedClassWithTwoBaseReferencesViaMixins), "MyBase2");

      var metadata1 = relationReflector1.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (metadata1, Is.Not.Null);
      var metadata2 = relationReflector2.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (metadata2, Is.Not.Null);
      Assert.That (_relationDefinitions.Count, Is.EqualTo (2));
    }

    [Test]
    public void GetMetadata_Mixed_PropertyAboveInheritanceRoot ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_inheritanceRootInheritingMixinClassDefinition,
          typeof (MixinAddingPersistentPropertiesAboveInheritanceRoot), "PersistentRelationProperty");
      _classDefinitions.Add (ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (RelationTargetForPersistentMixinAboveInheritanceRoot)));

      Assert.That (relationReflector.GetMetadata (_classDefinitions, _relationDefinitions), Is.Not.Null);
      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions[0].ID, NUnit.Framework.SyntaxHelpers.Text.Contains (relationReflector.PropertyInfo.Name));
    }

    private RelationReflector CreateRelationReflectorForProperty (ReflectionBasedClassDefinition classDefinition, Type declaringType, string propertyName)
    {
      var propertyInfo = declaringType.GetProperty (propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      var propertyReflector = new PropertyReflector (classDefinition, propertyInfo, new ReflectionBasedNameResolver ());
      var propertyDefinition = propertyReflector.GetMetadata ();

      classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      return new RelationReflector (classDefinition, propertyInfo, new ReflectionBasedNameResolver ());
    }
  }
}
