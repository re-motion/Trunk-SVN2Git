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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.MixinTestDomain;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Mixins;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  [TestFixture]
  public class ReflectionBasedClassDefinitionValidatorTest : StandardMappingTest
  {
    [Test]
    public void ValidateCurrentMixinConfiguration_OkWhenNoPersistentChanges ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("x", "xx", "xxx", typeof (Order), false, typeof (MixinA));
      using (MixinConfiguration.BuildFromActive ().ForClass (typeof (Order)).Clear ().AddMixins (typeof (MixinA)).EnterScope ())
      {
        new ReflectionBasedClassDefinitionValidator (classDefinition).ValidateCurrentMixinConfiguration (); // ok, no changes
      }

      using (MixinConfiguration.BuildFromActive ().ForClass (typeof (Order)).Clear ().AddMixins (typeof (NonDomainObjectMixin), typeof (MixinA)).EnterScope ())
      {
        new ReflectionBasedClassDefinitionValidator (classDefinition).ValidateCurrentMixinConfiguration (); // ok, no persistence-related changes
      }
    }

    [Test]
    public void ValidateCurrentMixinConfiguration_OkOnInheritanceRootInheritingMixin ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("x", "xx", "xxx", typeof (InheritanceRootInheritingPersistentMixin), false, typeof (MixinAddingPersistentPropertiesAboveInheritanceRoot));
      new ReflectionBasedClassDefinitionValidator (classDefinition).ValidateCurrentMixinConfiguration (); // ok, no changes
    }

    [Test]
    public void CreateNewPersistentMixinFinder_IncludeInheritedMixins_InheritanceRoot ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("x", "xx", "xxx", typeof (InheritanceRootInheritingPersistentMixin), false);
      Assert.That (new ReflectionBasedClassDefinitionValidator (classDefinition).CreateNewPersistentMixinFinder ().IncludeInherited, Is.True);
    }

    [Test]
    public void CreateNewPersistentMixinFinder_IncludeInheritedMixins_BelowInheritanceRoot ()
    {
      var baseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("x", "xx", "xxx", typeof (Company), false);
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("x", "xx", "xxx", typeof (Customer), false, baseClassDefinition);
      Assert.That (new ReflectionBasedClassDefinitionValidator (classDefinition).CreateNewPersistentMixinFinder ().IncludeInherited, Is.False);
    }



    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "A persistence-related mixin was removed from the domain object type "
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order after the mapping information was built: "
        + "Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.MixinTestDomain.MixinA.")]
    public void ValidateCurrentMixinConfiguration_ThrowsWhenPersistentMixisMissing ()
    {
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("x", "xx", "xxx", typeof (Order), false, typeof (MixinA));
      using (MixinConfiguration.BuildFromActive ().ForClass<Order> ().Clear ().EnterScope ())
      {
        new ReflectionBasedClassDefinitionValidator (classDefinition).ValidateCurrentMixinConfiguration ();
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "One or more persistence-related mixins were added to the domain object type "
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order after the mapping information was built: "
        + "Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.MixinTestDomain.MixinB, "
        + "Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.MixinTestDomain.MixinC.")]
    public void ValidateCurrentMixinConfiguration_ThrowsWhenPersistentMixinsAdded ()
    {
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("x", "xx", "xxx", typeof (Order), false, typeof (MixinA));
      using (MixinConfiguration.BuildFromActive ().ForClass (typeof (Order)).Clear ().AddMixins (typeof (NonDomainObjectMixin), typeof (MixinA), typeof (MixinB), typeof (MixinC)).EnterScope ())
      {
        new ReflectionBasedClassDefinitionValidator (classDefinition).ValidateCurrentMixinConfiguration ();
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "One or more persistence-related mixins were added to the domain object type "
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.Company after the mapping information was built: "
        + "Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.MixinTestDomain.MixinB, "
        + "Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.MixinTestDomain.MixinC.")]
    public void ValidateCurrentMixinConfiguration_ThrowsWhenPersistentMixinsChangeOnParentClass ()
    {
      ReflectionBasedClassDefinition baseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("xbase", "xx", "xxx", typeof (Company), false, typeof (MixinA) );
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("x", "xx", "xxx", typeof (Customer), false, baseClassDefinition);
      using (MixinConfiguration.BuildFromActive ().ForClass (typeof (Company)).Clear ().AddMixins (typeof (NonDomainObjectMixin), typeof (MixinA), typeof (MixinB), typeof (MixinC)).EnterScope ())
      {
        new ReflectionBasedClassDefinitionValidator (classDefinition).ValidateCurrentMixinConfiguration ();
      }
    }
  }
}
