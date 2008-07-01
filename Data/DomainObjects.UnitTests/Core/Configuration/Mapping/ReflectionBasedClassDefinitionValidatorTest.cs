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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.MixinTestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping
{
  [TestFixture]
  public class ReflectionBasedClassDefinitionValidatorTest : StandardMappingTest
  {
    [Test]
    public void ValidateCurrentMixinConfiguration_OkWhenNoPersistentChanges ()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition ("x", "xx", "xxx", typeof (Order), false,
          new Type[] { typeof (MixinA) });
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
    [ExpectedException (typeof (MappingException), ExpectedMessage = "A persistence-related mixin was removed from the domain object type "
        + "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order after the mapping information was built: "
        + "Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.MixinTestDomain.MixinA.")]
    public void ValidateCurrentMixinConfiguration_ThrowsWhenPersistentMixisMissing ()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition ("x", "xx", "xxx", typeof (Order), false,
          new Type[] { typeof (MixinA) });
      using (MixinConfiguration.BuildFromActive ().ForClass<Order> ().Clear ().EnterScope ())
      {
        new ReflectionBasedClassDefinitionValidator (classDefinition).ValidateCurrentMixinConfiguration ();
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "One or more persistence-related mixins were added to the domain object type "
        + "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order after the mapping information was built: "
        + "Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.MixinTestDomain.MixinB, "
        + "Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.MixinTestDomain.MixinC.")]
    public void ValidateCurrentMixinConfiguration_ThrowsWhenPersistentMixinsAdded ()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition ("x", "xx", "xxx", typeof (Order), false,
          new Type[] { typeof (MixinA) });
      using (MixinConfiguration.BuildFromActive ().ForClass (typeof (Order)).Clear ().AddMixins (typeof (NonDomainObjectMixin), typeof (MixinA), typeof (MixinB), typeof (MixinC)).EnterScope ())
      {
        new ReflectionBasedClassDefinitionValidator (classDefinition).ValidateCurrentMixinConfiguration ();
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "One or more persistence-related mixins were added to the domain object type "
        + "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company after the mapping information was built: "
        + "Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.MixinTestDomain.MixinB, "
        + "Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.MixinTestDomain.MixinC.")]
    public void ValidateCurrentMixinConfiguration_ThrowsWhenPersistentMixinsChangeOnParentClass ()
    {
      ReflectionBasedClassDefinition baseClassDefinition = new ReflectionBasedClassDefinition ("xbase", "xx", "xxx", typeof (Company), false,
          new Type[] { typeof (MixinA) });
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition ("x", "xx", "xxx", typeof (Customer), false, baseClassDefinition,
          new Type[0]);
      using (MixinConfiguration.BuildFromActive ().ForClass (typeof (Company)).Clear ().AddMixins (typeof (NonDomainObjectMixin), typeof (MixinA), typeof (MixinB), typeof (MixinC)).EnterScope ())
      {
        new ReflectionBasedClassDefinitionValidator (classDefinition).ValidateCurrentMixinConfiguration ();
      }
    }
  }
}