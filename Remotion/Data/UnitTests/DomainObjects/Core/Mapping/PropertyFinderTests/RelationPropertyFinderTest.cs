// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.PropertyFinderTests
{
  [TestFixture]
  public class RelationPropertyFinderTest : PropertyFinderBaseTestBase
  {
    [Test]
    public void Initialize ()
    {
      var classDefinition = CreateClassDefinition (typeof (ClassWithDifferentProperties));
      var propertyFinder = new RelationPropertyFinder (
          typeof (DerivedClassWithDifferentProperties),
          true,
          true,
          new ReflectionBasedNameResolver(),
          classDefinition.PersistentMixinFinder);

      Assert.That (propertyFinder.Type, Is.SameAs (typeof (DerivedClassWithDifferentProperties)));
      Assert.That (propertyFinder.IncludeBaseProperties, Is.True);
    }

    [Test]
    public void FindPropertyInfos_ForClassWithMixedProperties ()
    {
      var classDefinition = CreateClassDefinition (typeof (ClassWithDifferentProperties));
      var propertyFinder = new RelationPropertyFinder (
          typeof (ClassWithDifferentProperties), true, true, new ReflectionBasedNameResolver(), classDefinition.PersistentMixinFinder);

      Assert.That (
          propertyFinder.FindPropertyInfos(),
          Is.EquivalentTo (
              new[]
              {
                  GetProperty (typeof (ClassWithDifferentPropertiesNotInMapping), "BaseUnidirectionalOneToOne"),
                  GetProperty (typeof (ClassWithDifferentPropertiesNotInMapping), "BasePrivateUnidirectionalOneToOne"),
                  GetProperty (typeof (ClassWithDifferentProperties), "UnidirectionalOneToOne")
              }));
    }

    [Test]
    public void FindPropertyInfos_ForClassWithOneSideRelationProperties ()
    {
      var classDefinition = CreateClassDefinition (typeof (ClassWithVirtualRelationEndPoints));
      var propertyFinder = new RelationPropertyFinder (
          typeof (ClassWithVirtualRelationEndPoints), true, true, new ReflectionBasedNameResolver(), classDefinition.PersistentMixinFinder);

      Assert.That (
          propertyFinder.FindPropertyInfos(),
          Is.EquivalentTo (
              new[]
              {
                  GetProperty (typeof (ClassWithOneSideRelationPropertiesNotInMapping), "BaseBidirectionalOneToOne"),
                  GetProperty (typeof (ClassWithOneSideRelationPropertiesNotInMapping), "BaseBidirectionalOneToMany"),
                  GetProperty (typeof (ClassWithOneSideRelationPropertiesNotInMapping), "BasePrivateBidirectionalOneToOne"),
                  GetProperty (typeof (ClassWithOneSideRelationPropertiesNotInMapping), "BasePrivateBidirectionalOneToMany"),
                  GetProperty (typeof (ClassWithVirtualRelationEndPoints), "NoAttribute"),
                  GetProperty (typeof (ClassWithVirtualRelationEndPoints), "NotNullable"),
                  GetProperty (typeof (ClassWithVirtualRelationEndPoints), "BidirectionalOneToOne"),
                  GetProperty (typeof (ClassWithVirtualRelationEndPoints), "BidirectionalOneToMany")
              }));
    }
  }
}