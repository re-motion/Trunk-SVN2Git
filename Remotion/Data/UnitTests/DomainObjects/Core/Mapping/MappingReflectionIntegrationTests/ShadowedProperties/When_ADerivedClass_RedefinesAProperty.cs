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
using System.Reflection;
using NUnit.Framework;
using Remotion.Reflection;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.MappingReflectionIntegrationTests.ShadowedProperties
{
  [TestFixture]
  public class When_ADerivedClass_RedefinesAProperty : MappingReflectionIntegrationTestBase
  {
    [Test]
    public void BothProperties_ShouldBePresent ()
    {
      var derivedClassDefinition = ClassDefinitions[typeof (Shadower)];
      var baseClassDefinition = derivedClassDefinition.BaseClass;

      var basePropertyInfo = PropertyInfoAdapter.Create (
          typeof (Base).GetProperty ("Name", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
      var basePropertyDefinition = baseClassDefinition.ResolveProperty (basePropertyInfo);
      Assert.That (basePropertyDefinition, Is.Not.Null);

      var newPropertyInfo = PropertyInfoAdapter.Create (
          typeof (Shadower).GetProperty ("Name", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
      var newPropertyDefinition = derivedClassDefinition.ResolveProperty (newPropertyInfo);
      Assert.That (newPropertyDefinition, Is.Not.Null);

      Assert.That (newPropertyDefinition, Is.Not.SameAs (basePropertyDefinition));

      var basePropertyDefinitionOnDerivedClass = derivedClassDefinition.ResolveProperty (basePropertyInfo);
      Assert.That (basePropertyDefinitionOnDerivedClass, Is.Not.Null);
      Assert.That (basePropertyDefinitionOnDerivedClass, Is.SameAs (basePropertyDefinition));
    }
  }
}