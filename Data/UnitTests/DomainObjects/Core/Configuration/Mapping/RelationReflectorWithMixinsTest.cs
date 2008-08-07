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
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.SampleTypes;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  [TestFixture]
  public class RelationReflectorWithMixinsTest : StandardMappingTest
  {
    private ReflectionBasedClassDefinition _mixinTargetClassDefinition;
    private ReflectionBasedClassDefinition _relatedClassDefinition;
    private ReflectionBasedClassDefinition _inheritanceRootInheritingMixinClassDefinition;

    private ClassDefinitionCollection _classDefinitions;
    private RelationDefinitionCollection _relationDefinitions;

    public override void SetUp ()
    {
      base.SetUp();

      _mixinTargetClassDefinition = 
          CreateReflectionBasedClassDefinition (typeof (TargetClassForPersistentMixin), typeof (MixinAddingPersistentProperties));
      _relatedClassDefinition = 
          CreateReflectionBasedClassDefinition (typeof (RelationTargetForPersistentMixin), typeof (MixinAddingPersistentProperties));
      _inheritanceRootInheritingMixinClassDefinition =
          CreateReflectionBasedClassDefinition (typeof (InheritanceRootInheritingPersistentMixin), typeof (MixinAddingPersistentPropertiesAboveInheritanceRoot));

      _classDefinitions = new ClassDefinitionCollection {_mixinTargetClassDefinition, _relatedClassDefinition};
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
      Assert.That (relationReflector.DomainObjectTypeDeclaringProperty, Is.EqualTo (typeof (TargetClassForPersistentMixin)));
    }

    [Test]
    [Ignore ("TODO: Support relations with mixins above inheritance root")]
    public void DomainObjectTypeDeclaringProperty_WithMixinAboveInheritanceRoot ()
    {
      var relationReflector = CreateRelationReflectorForProperty(_inheritanceRootInheritingMixinClassDefinition, 
          typeof (MixinAddingPersistentPropertiesAboveInheritanceRoot), "PersistentRelationProperty");
      Assert.That (relationReflector.DomainObjectTypeDeclaringProperty, Is.EqualTo (typeof (TargetClassAboveInheritanceRoot)));
    }

    [Test]
    public void GetMetadata_Mixed_Unidirectional ()
    {
      PropertyInfo propertyInfo = typeof (MixinAddingPersistentProperties).GetProperty ("UnidirectionalRelationProperty");
      PropertyReflector propertyReflector = new PropertyReflector (_mixinTargetClassDefinition, propertyInfo, new ReflectionBasedNameResolver());
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata ();
      _mixinTargetClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RelationReflector relationReflector = new RelationReflector (_mixinTargetClassDefinition, propertyInfo, new ReflectionBasedNameResolver());

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);

      Assert.AreEqual (typeof (TargetClassForPersistentMixin).FullName + "->" + typeof (MixinAddingPersistentProperties).FullName 
          + ".UnidirectionalRelationProperty", actualRelationDefinition.ID);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[0]);
      RelationEndPointDefinition endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];
      Assert.AreEqual (propertyDefinition, endPointDefinition.PropertyDefinition);
      Assert.AreSame (_mixinTargetClassDefinition, endPointDefinition.ClassDefinition);
      Assert.AreSame (actualRelationDefinition, endPointDefinition.RelationDefinition);
      Assert.That (_mixinTargetClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.IsInstanceOfType (typeof (AnonymousRelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[1]);
      AnonymousRelationEndPointDefinition oppositeEndPointDefinition =
          (AnonymousRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.AreSame (_relatedClassDefinition, oppositeEndPointDefinition.ClassDefinition);
      Assert.AreSame (actualRelationDefinition, oppositeEndPointDefinition.RelationDefinition);
      Assert.That (_relatedClassDefinition.MyRelationDefinitions, List.Not.Contains (actualRelationDefinition));

      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions, List.Contains (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_Mixed_BidirectionalOneToOne ()
    {
      PropertyInfo propertyInfo = typeof (MixinAddingPersistentProperties).GetProperty ("RelationProperty");
      PropertyReflector propertyReflector = new PropertyReflector (_mixinTargetClassDefinition, propertyInfo, new ReflectionBasedNameResolver());
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata ();
      _mixinTargetClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RelationReflector relationReflector = new RelationReflector (_mixinTargetClassDefinition, propertyInfo, new ReflectionBasedNameResolver());

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);

      Assert.AreEqual (typeof (TargetClassForPersistentMixin).FullName + "->" + typeof (MixinAddingPersistentProperties) + ".RelationProperty",
          actualRelationDefinition.ID);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[0]);
      RelationEndPointDefinition endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];
      Assert.AreEqual (propertyDefinition, endPointDefinition.PropertyDefinition);
      Assert.AreSame (_mixinTargetClassDefinition, endPointDefinition.ClassDefinition);
      Assert.AreSame (actualRelationDefinition, endPointDefinition.RelationDefinition);
      Assert.That (_mixinTargetClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[1]);
      VirtualRelationEndPointDefinition oppositeEndPointDefinition =
          (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.AreSame (_relatedClassDefinition, oppositeEndPointDefinition.ClassDefinition);
      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.SampleTypes.RelationTargetForPersistentMixin.RelationProperty1",
          oppositeEndPointDefinition.PropertyName);
      Assert.AreSame (typeof (TargetClassForPersistentMixin), oppositeEndPointDefinition.PropertyType);
      Assert.AreSame (actualRelationDefinition, oppositeEndPointDefinition.RelationDefinition);
      Assert.That (_relatedClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions, List.Contains (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_Mixed_BidirectionalOneToOne_VirtualEndPoint ()
    {
      PropertyInfo propertyInfo = typeof (MixinAddingPersistentProperties).GetProperty ("VirtualRelationProperty");
      RelationReflector relationReflector = new RelationReflector (_mixinTargetClassDefinition, propertyInfo, new ReflectionBasedNameResolver());

      Assert.IsNull (relationReflector.GetMetadata (_classDefinitions, _relationDefinitions));
      Assert.That (_relationDefinitions.Count, Is.EqualTo (0));
    }

    [Test]
    public void GetMetadata_Mixed_BidirectionalOneToMany ()
    {
      PropertyInfo propertyInfo = typeof (MixinAddingPersistentProperties).GetProperty ("CollectionPropertyNSide");
      PropertyReflector propertyReflector = new PropertyReflector (_mixinTargetClassDefinition, propertyInfo, new ReflectionBasedNameResolver());
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata ();
      _mixinTargetClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RelationReflector relationReflector = new RelationReflector (_mixinTargetClassDefinition, propertyInfo, new ReflectionBasedNameResolver());

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);

      Assert.AreEqual (
          typeof (TargetClassForPersistentMixin).FullName + "->" + typeof (MixinAddingPersistentProperties) + ".CollectionPropertyNSide",
          actualRelationDefinition.ID);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[0]);
      RelationEndPointDefinition endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];
      Assert.AreEqual (propertyDefinition, endPointDefinition.PropertyDefinition);
      Assert.AreSame (_mixinTargetClassDefinition, endPointDefinition.ClassDefinition);
      Assert.AreSame (actualRelationDefinition, endPointDefinition.RelationDefinition);
      Assert.That (_mixinTargetClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[1]);
      VirtualRelationEndPointDefinition oppositeEndPointDefinition =
          (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.AreSame (_relatedClassDefinition, oppositeEndPointDefinition.ClassDefinition);
      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.SampleTypes.RelationTargetForPersistentMixin.RelationProperty4",
          oppositeEndPointDefinition.PropertyName);
      Assert.AreSame (typeof (ObjectList<TargetClassForPersistentMixin>), oppositeEndPointDefinition.PropertyType);
      Assert.AreSame (actualRelationDefinition, oppositeEndPointDefinition.RelationDefinition);
      Assert.That (_relatedClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions, List.Contains (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_Mixed_BidirectionalOneToMany_VirtualEndPoint ()
    {
      PropertyInfo propertyInfo = typeof (MixinAddingPersistentProperties).GetProperty ("CollectionProperty1Side");
      RelationReflector relationReflector = new RelationReflector (_mixinTargetClassDefinition, propertyInfo, new ReflectionBasedNameResolver());

      Assert.IsNull (relationReflector.GetMetadata (_classDefinitions, _relationDefinitions));
      Assert.That (_relationDefinitions.Count, Is.EqualTo (0));
    }

    [Test]
    public void GetMetadata_Mixed_OppositePropertyPrivateOnBase ()
    {
      PropertyInfo propertyInfo = 
          typeof (BaseForMixinAddingPersistentProperties).GetProperty ("PrivateBaseRelationProperty", BindingFlags.NonPublic | BindingFlags.Instance);
      var propertyReflector = new PropertyReflector (_mixinTargetClassDefinition, propertyInfo, new ReflectionBasedNameResolver ());
      var propertyDefinition = propertyReflector.GetMetadata ();
      _mixinTargetClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);

      var relationReflector = new RelationReflector (_mixinTargetClassDefinition, propertyInfo, new ReflectionBasedNameResolver ());

      Assert.IsNotNull (relationReflector.GetMetadata (_classDefinitions, _relationDefinitions));
      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions[0].ID, NUnit.Framework.SyntaxHelpers.Text.Contains (propertyDefinition.PropertyName));
    }

    [Test]
    [Ignore ("TODO: Support relations with mixins above inheritance root")]
    public void GetMetadata_Mixed_PropertyAboveInheritanceRoot ()
    {
      PropertyInfo propertyInfo = typeof (MixinAddingPersistentPropertiesAboveInheritanceRoot).GetProperty ("PersistentRelationProperty",
          BindingFlags.Public | BindingFlags.Instance);
      var propertyReflector = new PropertyReflector (_inheritanceRootInheritingMixinClassDefinition, propertyInfo, new ReflectionBasedNameResolver ());
      var propertyDefinition = propertyReflector.GetMetadata ();
      _inheritanceRootInheritingMixinClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);

      var relationReflector = new RelationReflector (_inheritanceRootInheritingMixinClassDefinition, propertyInfo, new ReflectionBasedNameResolver ());

      Assert.IsNotNull (relationReflector.GetMetadata (_classDefinitions, _relationDefinitions));
      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions[0].ID, NUnit.Framework.SyntaxHelpers.Text.Contains (propertyDefinition.PropertyName));
    }

    private ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (Type type, params Type[] mixins)
    {
      return new ReflectionBasedClassDefinition (type.Name, type.Name, "TestDomain", type, false, new List<Type> (mixins));
    }

    private RelationReflector CreateRelationReflectorForProperty (ReflectionBasedClassDefinition classDefinition, Type declaringType, string propertyName)
    {
      var propertyInfo = declaringType.GetProperty (propertyName);
      var propertyReflector = new PropertyReflector (classDefinition, propertyInfo, new ReflectionBasedNameResolver ());
      var propertyDefinition = propertyReflector.GetMetadata ();

      classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      return new RelationReflector (classDefinition, propertyInfo, new ReflectionBasedNameResolver ());
    }
  }
}
