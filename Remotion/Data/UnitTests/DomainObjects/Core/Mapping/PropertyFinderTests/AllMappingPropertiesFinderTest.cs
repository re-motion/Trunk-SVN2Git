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
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using
    Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection.
        MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.PropertyFinderTests
{
  [TestFixture]
  public class AllMappingPropertiesFinderTest : PropertyFinderBaseTestBase
  {
    [Test]
    public void FindMappingProperties_IncludeBasePropertiesIsFalse ()
    {
      var classDefinition = CreateReflectionBasedClassDefinition (typeof (DerivedClassWithMappingAttribute));
      var propertyFinder = new AllMappingPropertiesFinder (
          typeof (DerivedClassWithMappingAttribute),
          false,
          true,
          new ReflectionBasedNameResolver(),
          classDefinition.PersistentMixinFinder);

      var properties = propertyFinder.FindPropertyInfos();

      Assert.That (properties.Length, Is.EqualTo (2));
      Assert.That (
          properties,
          Is.EqualTo (
              new[]
              {
                  GetProperty (typeof (DerivedClassWithMappingAttribute), "Property1"),
                  GetProperty (typeof (DerivedClassWithMappingAttribute), "Property3")
              }));
    }

    [Test]
    public void FindMappingProperties_IncludeBasePropertiesIsTrue ()
    {
      var classDefinition = CreateReflectionBasedClassDefinition (typeof (DerivedClassWithMappingAttribute));
      var propertyFinder = new AllMappingPropertiesFinder (
          typeof (DerivedClassWithMappingAttribute),
          true,
          true,
          new ReflectionBasedNameResolver(),
          classDefinition.PersistentMixinFinder);

      var properties = propertyFinder.FindPropertyInfos();

      Assert.That (properties.Length, Is.EqualTo (4));
      Assert.That (
          properties,
          Is.EqualTo (
              new[]
              {
                  GetProperty (typeof (BaseMappingAttributesClass), "Property1"),
                  GetProperty (typeof (BaseMappingAttributesClass), "Property4"),
                  GetProperty (typeof (DerivedClassWithMappingAttribute), "Property1"),
                  GetProperty (typeof (DerivedClassWithMappingAttribute), "Property3")
              }));
    }
  }
}