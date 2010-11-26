// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class RelationReflectorWithMixinsTest : MappingReflectionTestBase
  {
    private ReflectionBasedClassDefinition _mixinTargetClassDefinition;
    private ReflectionBasedClassDefinition _multiMixinTargetClassDefinition;
    private ReflectionBasedClassDefinition _multiMixinRelatedClassDefinition;
    private ReflectionBasedClassDefinition _relatedClassDefinition;
    private ReflectionBasedClassDefinition _inheritanceRootInheritingMixinClassDefinition;
    private ClassDefinitionCollection _classDefinitions;

    public override void SetUp ()
    {
      base.SetUp ();

      _mixinTargetClassDefinition =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (TargetClassForPersistentMixin), typeof (MixinAddingPersistentProperties));
      _multiMixinTargetClassDefinition =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (TargetClassReceivingTwoReferencesToDerivedClass), typeof (MixinAddingTwoReferencesToDerivedClass1), typeof (MixinAddingTwoReferencesToDerivedClass2));
      _multiMixinRelatedClassDefinition =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (DerivedClassWithTwoBaseReferencesViaMixins));
      _relatedClassDefinition =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (RelationTargetForPersistentMixin), typeof (MixinAddingPersistentProperties));
      _inheritanceRootInheritingMixinClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("InheritanceRootInheritingPersistentMixin",
          "InheritanceRootInheritingPersistentMixin", "TestDomain", typeof (InheritanceRootInheritingPersistentMixin), false,
          new PersistentMixinFinder (typeof (InheritanceRootInheritingPersistentMixin), true));

      _classDefinitions = new ClassDefinitionCollection { _mixinTargetClassDefinition, _relatedClassDefinition, _multiMixinTargetClassDefinition };
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
      var relationReflector = CreateRelationReflectorForProperty (_inheritanceRootInheritingMixinClassDefinition,
          typeof (MixinAddingPersistentPropertiesAboveInheritanceRoot), "PersistentRelationProperty");
      Assert.That (relationReflector.DeclaringDomainObjectTypeForProperty, Is.EqualTo (typeof (TargetClassAboveInheritanceRoot)));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "IPersistentMixinFinder.FindOriginalMixinTarget (DeclaringMixin) "
        + "evaluated and returned null.")]
    public void DomainObjectTypeDeclaringProperty_OriginalMixinTargetNull ()
    {
      var mockRepository = new MockRepository ();
      var mixinFinderMock = mockRepository.Stub<IPersistentMixinFinder> ();

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

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.ID,
          Is.EqualTo (typeof (TargetClassForPersistentMixin).FullName + ":" +
          typeof (MixinAddingPersistentProperties).FullName + ".UnidirectionalRelationProperty"));
    }

    [Test]
    public void GetMetadata_Mixed_VirtualSide ()
    {
      var propertyInfo = typeof (RelationTargetForPersistentMixin).GetProperty ("RelationProperty2", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      var propertyReflector = new PropertyReflector (_relatedClassDefinition, propertyInfo, new ReflectionBasedNameResolver ());
      var propertyDefinition = propertyReflector.GetMetadata ();

      _relatedClassDefinition.SetPropertyDefinitions (new[] { propertyDefinition });
      ;

      var relationReflector = CreateRelationReflectorForProperty (_mixinTargetClassDefinition,
          typeof (MixinAddingPersistentProperties), "VirtualRelationProperty");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.ID,
          Is.EqualTo (typeof (RelationTargetForPersistentMixin).FullName + ":" +
          typeof (RelationTargetForPersistentMixin).FullName + ".RelationProperty2->" + typeof (MixinAddingPersistentProperties) + ".VirtualRelationProperty"));
    }

    [Test]
    public void GetMetadata_Mixed_Unidirectional_EndPointDefinition0 ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_mixinTargetClassDefinition,
          typeof (MixinAddingPersistentProperties), "UnidirectionalRelationProperty");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[0], Is.InstanceOfType (typeof (RelationEndPointDefinition)));

      var endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];

      Assert.That (endPointDefinition.PropertyDefinition, Is.EqualTo (_mixinTargetClassDefinition.MyPropertyDefinitions[0]));
      Assert.That (endPointDefinition.ClassDefinition, Is.SameAs (_mixinTargetClassDefinition));
      Assert.That (endPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_Mixed_Unidirectional_EndPointDefinition1 ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_mixinTargetClassDefinition,
          typeof (MixinAddingPersistentProperties), "UnidirectionalRelationProperty");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[1], Is.InstanceOfType (typeof (AnonymousRelationEndPointDefinition)));
      var oppositeEndPointDefinition = (AnonymousRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];

      Assert.That (oppositeEndPointDefinition.ClassDefinition, Is.SameAs (_relatedClassDefinition));
      Assert.That (oppositeEndPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_Mixed_BidirectionalOneToOne_EndPointDefinition0 ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_mixinTargetClassDefinition,
          typeof (MixinAddingPersistentProperties), "RelationProperty");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[0], Is.InstanceOfType (typeof (RelationEndPointDefinition)));

      var endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];

      Assert.That (endPointDefinition.PropertyDefinition, Is.EqualTo (_mixinTargetClassDefinition.MyPropertyDefinitions[0]));
      Assert.That (endPointDefinition.ClassDefinition, Is.SameAs (_mixinTargetClassDefinition));
      Assert.That (endPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_Mixed_BidirectionalOneToOne_EndPointDefinition1 ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_mixinTargetClassDefinition,
          typeof (MixinAddingPersistentProperties), "RelationProperty");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[1], Is.InstanceOfType (typeof (VirtualRelationEndPointDefinition)));
      var oppositeEndPointDefinition = (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];

      Assert.That (oppositeEndPointDefinition.ClassDefinition, Is.SameAs (_relatedClassDefinition));
      Assert.That (oppositeEndPointDefinition.PropertyName,
          Is.EqualTo ("Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain.RelationTargetForPersistentMixin.RelationProperty1"));
      Assert.That (oppositeEndPointDefinition.PropertyType, Is.SameAs (typeof (TargetClassForPersistentMixin)));
      Assert.That (oppositeEndPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_Mixed_BidirectionalOneToMany_EndPoint0 ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_mixinTargetClassDefinition,
          typeof (MixinAddingPersistentProperties), "CollectionPropertyNSide");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[0], Is.InstanceOfType (typeof (RelationEndPointDefinition)));

      var endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];

      Assert.That (endPointDefinition.PropertyDefinition, Is.EqualTo (_mixinTargetClassDefinition.MyPropertyDefinitions[0]));
      Assert.That (endPointDefinition.ClassDefinition, Is.SameAs (_mixinTargetClassDefinition));
      Assert.That (endPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_Mixed_BidirectionalOneToMany_EndPoint1 ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_mixinTargetClassDefinition,
          typeof (MixinAddingPersistentProperties), "CollectionPropertyNSide");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
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

      Assert.That (relationReflector.GetMetadata (_classDefinitions), Is.Not.Null);
    }

    [Test]
    public void GetMetadata_Mixed_TwoMixins ()
    {
      var relationReflector1 = CreateRelationReflectorForProperty (_multiMixinRelatedClassDefinition,
          typeof (DerivedClassWithTwoBaseReferencesViaMixins), "MyBase1");
      var relationReflector2 = CreateRelationReflectorForProperty (_multiMixinRelatedClassDefinition,
          typeof (DerivedClassWithTwoBaseReferencesViaMixins), "MyBase2");

      var metadata1 = relationReflector1.GetMetadata (_classDefinitions);
      Assert.That (metadata1, Is.Not.Null);
      var metadata2 = relationReflector2.GetMetadata (_classDefinitions);
      Assert.That (metadata2, Is.Not.Null);
    }

    [Test]
    public void GetMetadata_Mixed_PropertyAboveInheritanceRoot ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_inheritanceRootInheritingMixinClassDefinition,
          typeof (MixinAddingPersistentPropertiesAboveInheritanceRoot), "PersistentRelationProperty");
      _classDefinitions.Add (ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (RelationTargetForPersistentMixinAboveInheritanceRoot)));

      Assert.That (relationReflector.GetMetadata (_classDefinitions), Is.Not.Null);
    }

    private RelationReflector CreateRelationReflectorForProperty (ReflectionBasedClassDefinition classDefinition, Type declaringType, string propertyName)
    {
      var propertyInfo = declaringType.GetProperty (propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      var propertyReflector = new PropertyReflector (classDefinition, propertyInfo, new ReflectionBasedNameResolver ());
      var propertyDefinition = propertyReflector.GetMetadata ();
      var properties = new List<PropertyDefinition> ();
      properties.Add (propertyDefinition);
      var propertyDefinitionsOfClass = (PropertyDefinitionCollection) PrivateInvoke.GetNonPublicField (classDefinition, "_propertyDefinitions");
      PrivateInvoke.SetNonPublicField (classDefinition, "_propertyDefinitions", null);
      if (propertyDefinitionsOfClass != null)
        properties.AddRange (propertyDefinitionsOfClass.Cast<PropertyDefinition> ());

      classDefinition.SetPropertyDefinitions (properties);
      ;
      return new RelationReflector (classDefinition, propertyInfo, new ReflectionBasedNameResolver (), new ReflectionBasedRelationEndPointDefinitionFactory ());
    }
  }
}
