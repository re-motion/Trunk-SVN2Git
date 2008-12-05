// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.PropertyFinderTests
{
  [TestFixture]
  public class PropertyFinderBaseTest : PropertyFinderBaseTestBase
  {
    [Test]
    public void Initialize ()
    {
      PropertyFinderBase propertyFinder = new StubPropertyFinderBase (typeof (ClassWithMixedProperties), true);

      Assert.That (propertyFinder.Type, Is.SameAs (typeof (ClassWithMixedProperties)));
      Assert.That (propertyFinder.IncludeBaseProperties, Is.True);
    }

    [Test]
    public void FindPropertyInfos_ForInheritanceRoot ()
    {
      PropertyFinderBase propertyFinder = new StubPropertyFinderBase (typeof (ClassWithMixedProperties), true);

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
    public void FindPropertyInfos_ForDerivedClass ()
    {
      PropertyFinderBase propertyFinder = new StubPropertyFinderBase (typeof (ClassWithMixedProperties), false);

      Assert.That (
          propertyFinder.FindPropertyInfos (CreateReflectionBasedClassDefinition (typeof (ClassWithMixedProperties))),
          Is.EqualTo (
              new PropertyInfo[]
                  {
                      GetProperty (typeof (ClassWithMixedProperties), "Int32"),
                      GetProperty (typeof (ClassWithMixedProperties), "String"),
                      GetProperty (typeof (ClassWithMixedProperties), "UnidirectionalOneToOne"),
                      GetProperty (typeof (ClassWithMixedProperties), "PrivateString")
                  }));
    }

    [Test]
    public void FindPropertyInfos_ForClassWithInterface ()
    {
      PropertyFinderBase propertyFinder = new StubPropertyFinderBase (typeof (ClassWithInterface), false);

      Assert.That (
          propertyFinder.FindPropertyInfos (CreateReflectionBasedClassDefinition (typeof (ClassWithInterface))),
          Is.EqualTo (
              new PropertyInfo[]
                  {
                      GetProperty (typeof (ClassWithInterface), "Property"),
                      GetProperty (typeof (ClassWithInterface), "ImplicitProperty"),
                      GetProperty (typeof (ClassWithInterface), "Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample.IInterfaceWithProperties.ExplicitManagedProperty")
                  }));
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage =
        "The 'Remotion.Data.DomainObjects.StorageClassNoneAttribute' is a mapping attribute and may only be applied at the property's base definition.\r\n  "
        + "Type: Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.DerivedClassHavingAnOverriddenPropertyWithMappingAttribute, "
        + "property: Int32")]
    public void FindPropertyInfos_ForDerivedClassHavingAnOverriddenPropertyWithMappingAttribute ()
    {
      Type type = TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.DerivedClassHavingAnOverriddenPropertyWithMappingAttribute",
          true,
          false);
      PropertyFinderBase propertyFinder = new StubPropertyFinderBase (type, false);

      propertyFinder.FindPropertyInfos (CreateReflectionBasedClassDefinition (type));
    }
  }
}
