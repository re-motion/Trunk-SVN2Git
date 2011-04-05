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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.MixinTestDomain;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class ClassDefinitionValidatorTest : MappingReflectionTestBase
  {
    [Test]
    public void ValidateCurrentMixinConfiguration_OkWhenNoChanges ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "x", "xx", UnitTestDomainStorageProviderDefinition, typeof (Order), false, typeof (MixinA));
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (Order)).Clear().AddMixins (typeof (MixinA)).EnterScope())
      {
        new ClassDefinitionValidator (classDefinition).ValidateCurrentMixinConfiguration(); // ok, no changes
      }
    }

    [Test]
    public void ValidateCurrentMixinConfiguration_OkOnInheritanceRootInheritingMixin ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "x",
          "xx",
          UnitTestDomainStorageProviderDefinition,
          typeof (InheritanceRootInheritingPersistentMixin),
          false,
          typeof (MixinAddingPersistentPropertiesAboveInheritanceRoot));

      using (MixinConfiguration
          .BuildNew()
          .ForClass (typeof (TargetClassAboveInheritanceRoot))
          .AddMixins (typeof (MixinAddingPersistentPropertiesAboveInheritanceRoot))
          .EnterScope())
      {
        // ok, no changes, even though the mixins stem from a base class
        new ClassDefinitionValidator (classDefinition).ValidateCurrentMixinConfiguration();
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The mixin configuration for domain object type 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order' "
        + "was changed after the mapping information was built.\r\n"
        + "Original configuration: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order + "
        + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.MixinTestDomain.MixinA.\r\n"
        + "Active configuration: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order + "
        + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.MixinTestDomain.NonDomainObjectMixin + "
        + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.MixinTestDomain.MixinA")]
    public void ValidateCurrentMixinConfiguration_ThrowsWhenAnyChanges_EvenToNonPersistentMixins ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "x", "xx", UnitTestDomainStorageProviderDefinition, typeof (Order), false, typeof (MixinA));

      using (
          MixinConfiguration.BuildFromActive().ForClass (typeof (Order)).Clear().AddMixins (typeof (NonDomainObjectMixin), typeof (MixinA)).EnterScope
              ())
      {
        new ClassDefinitionValidator (classDefinition).ValidateCurrentMixinConfiguration();
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The mixin configuration for domain object type 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order' "
        + "was changed after the mapping information was built.",
        MatchType = MessageMatch.Contains)]
    public void ValidateCurrentMixinConfiguration_ThrowsWhenPersistentMixinMissing ()
    {
      ClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "x", "xx", UnitTestDomainStorageProviderDefinition, typeof (Order), false, typeof (MixinA));
      using (MixinConfiguration.BuildFromActive().ForClass<Order>().Clear().EnterScope())
      {
        new ClassDefinitionValidator (classDefinition).ValidateCurrentMixinConfiguration();
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The mixin configuration for domain object type 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order' "
        + "was changed after the mapping information was built.",
        MatchType = MessageMatch.Contains)]
    public void ValidateCurrentMixinConfiguration_ThrowsWhenPersistentMixinsAdded ()
    {
      ClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "x", "xx", UnitTestDomainStorageProviderDefinition, typeof (Order), false, typeof (MixinA));
      using (
          MixinConfiguration.BuildFromActive().ForClass (typeof (Order)).Clear().AddMixins (
              typeof (NonDomainObjectMixin), typeof (MixinA), typeof (MixinB), typeof (MixinC)).EnterScope())
      {
        new ClassDefinitionValidator (classDefinition).ValidateCurrentMixinConfiguration();
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The mixin configuration for domain object type 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Customer' "
        + "was changed after the mapping information was built.",
        MatchType = MessageMatch.Contains)]
    public void ValidateCurrentMixinConfiguration_ThrowsWhenPersistentMixinsChangeOnParentClass ()
    {
      ClassDefinition baseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "xbase", "xx", UnitTestDomainStorageProviderDefinition, typeof (Company), false, typeof (MixinA));
      ClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "x", "xx", UnitTestDomainStorageProviderDefinition, typeof (Customer), false, baseClassDefinition);
      using (
          MixinConfiguration.BuildFromActive().ForClass (typeof (Company)).Clear().AddMixins (
              typeof (NonDomainObjectMixin), typeof (MixinA), typeof (MixinB), typeof (MixinC)).EnterScope())
      {
        new ClassDefinitionValidator (classDefinition).ValidateCurrentMixinConfiguration();
      }
    }
  }
}