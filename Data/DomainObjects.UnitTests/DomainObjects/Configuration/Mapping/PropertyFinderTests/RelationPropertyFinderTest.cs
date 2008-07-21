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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Configuration.Mapping.PropertyFinderTests
{
  [TestFixture]
  public class RelationPropertyFinderTest
  {
    private PersistentMixinFinder _emptyPersistentMixinFinder;

    [SetUp]
    public void SetUp ()
    {
      _emptyPersistentMixinFinder = new PersistentMixinFinder (typeof (object));
    }

    [Test]
    public void Initialize ()
    {
      RelationPropertyFinder propertyFinder = new RelationPropertyFinder (typeof (DerivedClassWithMixedProperties), true, _emptyPersistentMixinFinder, new ReflectionBasedNameResolver());

      Assert.That (propertyFinder.Type, Is.SameAs (typeof (DerivedClassWithMixedProperties)));
      Assert.That (propertyFinder.IncludeBaseProperties, Is.True);
    }

    [Test]
    public void FindPropertyInfos_ForClassWithMixedProperties ()
    {
      RelationPropertyFinder propertyFinder = new RelationPropertyFinder (typeof (ClassWithMixedProperties), true, _emptyPersistentMixinFinder, new ReflectionBasedNameResolver ());

      Assert.That (
          propertyFinder.FindPropertyInfos (CreateReflectionBasedClassDefinition (typeof (ClassWithMixedProperties))),
          Is.EquivalentTo (
              new PropertyInfo[]
                  {
                      GetProperty (typeof (ClassWithMixedPropertiesNotInMapping), "BaseUnidirectionalOneToOne"),
                      GetProperty (typeof (ClassWithMixedPropertiesNotInMapping), "BasePrivateUnidirectionalOneToOne"),
                      GetProperty (typeof (ClassWithMixedProperties), "UnidirectionalOneToOne")
                  }));
    }

    [Test]
    public void FindPropertyInfos_ForClassWithOneSideRelationProperties ()
    {
      RelationPropertyFinder propertyFinder = new RelationPropertyFinder (typeof (ClassWithOneSideRelationProperties), true, _emptyPersistentMixinFinder, new ReflectionBasedNameResolver ());

      Assert.That (
          propertyFinder.FindPropertyInfos (CreateReflectionBasedClassDefinition (typeof (ClassWithOneSideRelationProperties))),
          Is.EquivalentTo (
              new PropertyInfo[]
                  {
                      GetProperty (typeof (ClassWithOneSideRelationPropertiesNotInMapping), "BaseBidirectionalOneToOne"),
                      GetProperty (typeof (ClassWithOneSideRelationPropertiesNotInMapping), "BaseBidirectionalOneToMany"),
                      GetProperty (typeof (ClassWithOneSideRelationPropertiesNotInMapping), "BasePrivateBidirectionalOneToOne"),
                      GetProperty (typeof (ClassWithOneSideRelationPropertiesNotInMapping), "BasePrivateBidirectionalOneToMany"),
                      GetProperty (typeof (ClassWithOneSideRelationProperties), "NoAttribute"),
                      GetProperty (typeof (ClassWithOneSideRelationProperties), "NotNullable"),
                      GetProperty (typeof (ClassWithOneSideRelationProperties), "BidirectionalOneToOne"),
                      GetProperty (typeof (ClassWithOneSideRelationProperties), "BidirectionalOneToMany")
                  }));
    }

    private PropertyInfo GetProperty (Type type, string propertyName)
    {
      PropertyInfo propertyInfo =
          type.GetProperty (propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      Assert.That (propertyInfo, Is.Not.Null, "Property '{0}' was not found on type '{1}'.", propertyName, type);

      return propertyInfo;
    }

    private ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (Type type)
    {
      return new ReflectionBasedClassDefinition (type.Name, type.Name, "TestDomain", type, false, new List<Type>());
    }
  }
}
