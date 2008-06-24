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

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.PropertyFinderTests
{
  [TestFixture]
  public class PropertyFinderTest
  {
    [Test]
    public void Initialize ()
    {
      PropertyFinder propertyFinder = new PropertyFinder (typeof (ClassWithMixedProperties), true, new List<Type> (), new ReflectionBasedNameResolver());

      Assert.That (propertyFinder.Type, Is.SameAs (typeof (ClassWithMixedProperties)));
      Assert.That (propertyFinder.IncludeBaseProperties, Is.True);
    }

    [Test]
    public void FindPropertyInfos_ForClassWithMixedProperties ()
    {
      PropertyFinder propertyFinder = new PropertyFinder (typeof (ClassWithMixedProperties), true, new List<Type> (), new ReflectionBasedNameResolver ());

      Assert.That (
          propertyFinder.FindPropertyInfos (CreateReflectionBasedClassDefinition (typeof (ClassWithMixedProperties))),
          Is.EqualTo (
              new PropertyInfo[]
                  {
                      GetProperty (typeof (ClassWithMixedPropertiesNotInMapping), "BaseString"),
                      GetProperty (typeof (ClassWithMixedPropertiesNotInMapping), "BaseUnidirectionalOneToOne"),
                      GetProperty (typeof (ClassWithMixedPropertiesNotInMapping), "BasePrivateUnidirectionalOneToOne"),
                      GetProperty (typeof (ClassWithMixedProperties), "Int32"),
                      GetProperty (typeof (ClassWithMixedProperties), "String"),
                      GetProperty (typeof (ClassWithMixedProperties), "UnidirectionalOneToOne"),
                      GetProperty (typeof (ClassWithMixedProperties), "PrivateString")
                  }));
    }

    [Test]
    public void FindPropertyInfos_ForClassWithOneSideRelationProperties ()
    {
      PropertyFinder propertyFinder = new PropertyFinder (typeof (ClassWithOneSideRelationProperties), true, new List<Type> (), new ReflectionBasedNameResolver ());

      Assert.That (
          propertyFinder.FindPropertyInfos (CreateReflectionBasedClassDefinition (typeof (ClassWithOneSideRelationProperties))), 
          Is.Empty);
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
