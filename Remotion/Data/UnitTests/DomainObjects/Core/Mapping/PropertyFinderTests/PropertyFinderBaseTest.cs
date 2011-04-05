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
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.PropertyFinderTests
{
  [TestFixture]
  public class PropertyFinderBaseTest : PropertyFinderBaseTestBase
  {
    [Test]
    public void Initialize ()
    {
      var classDefinition = CreateReflectionBasedClassDefinition (typeof (ClassWithDifferentProperties));
      var propertyFinder = new StubPropertyFinderBase (typeof (ClassWithDifferentProperties), classDefinition, true, true, classDefinition.PersistentMixinFinder);

      Assert.That (propertyFinder.Type, Is.SameAs (typeof (ClassWithDifferentProperties)));
      Assert.That (propertyFinder.IncludeBaseProperties, Is.True);
      Assert.That (propertyFinder.IncludeMixinProperties, Is.True);
    }

    [Test]
    public void FindPropertyInfos_ForInheritanceRoot ()
    {
      var classDefinition = CreateReflectionBasedClassDefinition (typeof (ClassWithDifferentProperties));
      var propertyFinder = new StubPropertyFinderBase (typeof (ClassWithDifferentProperties), classDefinition, true, true, classDefinition.PersistentMixinFinder);

      Assert.That (
          propertyFinder.FindPropertyInfos (),
          Is.EqualTo (
              new[]
                  {
                      GetProperty (typeof (ClassWithDifferentPropertiesNotInMapping), "BaseString"),
                      GetProperty (typeof (ClassWithDifferentPropertiesNotInMapping), "BaseUnidirectionalOneToOne"),
                      GetProperty (typeof (ClassWithDifferentPropertiesNotInMapping), "BasePrivateUnidirectionalOneToOne"),
                      GetProperty (typeof (ClassWithDifferentProperties), "Int32"),
                      GetProperty (typeof (ClassWithDifferentProperties), "String"),
                      GetProperty (typeof (ClassWithDifferentProperties), "UnidirectionalOneToOne"),
                      GetProperty (typeof (ClassWithDifferentProperties), "PrivateString")
                  }));
    }

    [Test]
    public void FindPropertyInfos_ForDerivedClass ()
    {
      var classDefinition = CreateReflectionBasedClassDefinition (typeof (ClassWithDifferentProperties));
      var propertyFinder = new StubPropertyFinderBase (typeof (ClassWithDifferentProperties), classDefinition, false, true, classDefinition.PersistentMixinFinder);

      Assert.That (
          propertyFinder.FindPropertyInfos (),
          Is.EqualTo (
              new[]
                  {
                      GetProperty (typeof (ClassWithDifferentProperties), "Int32"),
                      GetProperty (typeof (ClassWithDifferentProperties), "String"),
                      GetProperty (typeof (ClassWithDifferentProperties), "UnidirectionalOneToOne"),
                      GetProperty (typeof (ClassWithDifferentProperties), "PrivateString")
                  }));
    }

    [Test]
    public void FindPropertyInfos_ForClassWithInterface ()
    {
      var classDefinition = CreateReflectionBasedClassDefinition (typeof (ClassWithInterface));
      var propertyFinder = new StubPropertyFinderBase (typeof (ClassWithInterface), classDefinition, false, true, classDefinition.PersistentMixinFinder);

      Assert.That (
          propertyFinder.FindPropertyInfos (),
          Is.EqualTo (
              new[]
                  {
                      GetProperty (typeof (ClassWithInterface), "Property"),
                      GetProperty (typeof (ClassWithInterface), "ImplicitProperty"),
                      GetProperty (typeof (ClassWithInterface), "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.IInterfaceWithProperties.ExplicitManagedProperty")
                  }));
    }
    
  }
}
