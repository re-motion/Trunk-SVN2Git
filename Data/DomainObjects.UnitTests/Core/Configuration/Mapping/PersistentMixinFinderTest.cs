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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.MixinTestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping
{
  [TestFixture]
  public class PersistentMixinFinderTest
  {
    [Test]
    public void ForTargetClassBase ()
    {
      Type targetType = typeof (TargetClassBase);
      CheckPersistentMixins (targetType, typeof (MixinBase));
    }

    [Test]
    public void ForTargetClassA ()
    {
      Type targetType = typeof (TargetClassA);
      CheckPersistentMixins (targetType, typeof (MixinA), typeof (MixinC), typeof (MixinD));
    }

    [Test]
    public void ForTargetClassB ()
    {
      Type targetType = typeof (TargetClassB);
      CheckPersistentMixins (targetType, typeof (MixinB), typeof (MixinE));
    }

    public class PersistedGenericMixin<T> : DomainObjectMixin<T>
        where T : DomainObject
    {
    }

    public class NonPersistedGenericMixin<T> : Mixin<T>
        where T : DomainObject
    {
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "The persistence-relevant mixin "
        + "Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.PersistentMixinFinderTest+PersistedGenericMixin`1 applied to class "
        + "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order has open generic type parameters. All type parameters of the mixin must be "
        + "specified when it is applied to a DomainObject.")]
    public void PersistenceRelevant_OpenGenericMixin ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass (typeof (Order)).Clear ().AddMixins (typeof (PersistedGenericMixin<>)).EnterScope ())
      {
        PersistentMixinFinder.GetPersistentMixins (typeof (Order));
      }
    }

    [Test]
    public void PersistenceIrrelevant_OpenGenericMixin ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass (typeof (Order)).Clear ().AddMixins (typeof (NonPersistedGenericMixin<>)).EnterScope ())
      {
        List<Type> persistentMixins = PersistentMixinFinder.GetPersistentMixins (typeof (Order));
        Assert.That (persistentMixins, Is.Empty);
      }
    }

    private void CheckPersistentMixins (Type targetType, params Type[] expectedTypes)
    {
      List<Type> mixinTypes = PersistentMixinFinder.GetPersistentMixins (targetType);
      Assert.That (mixinTypes, Is.EquivalentTo (expectedTypes));
    }
  }
}
