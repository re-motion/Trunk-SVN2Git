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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.MixinTestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  [TestFixture]
  public class PersistentMixinFinderTest
  {
    [Test]
    public void IsInParentContext_WithNullParentContext ()
    {
      PersistentMixinFinder finder = new PersistentMixinFinder (typeof (TargetClassBase));
      Assert.That (finder.ParentClassContext, Is.Null);
      Assert.That (finder.IsInParentContext (typeof (MixinBase)), Is.False);
    }

    [Test]
    public void IsInParentContext_WithParentContext ()
    {
      PersistentMixinFinder finder = new PersistentMixinFinder (typeof (TargetClassA));
      Assert.That (finder.ParentClassContext, Is.Not.Null);
      Assert.That (finder.IsInParentContext (typeof (MixinBase)), Is.True);
      Assert.That (finder.IsInParentContext (typeof (MixinA)), Is.False);
    }

    [Test]
    public void IsPersistenceRelevant ()
    {
      Assert.That (PersistentMixinFinder.IsPersistenceRelevant (typeof (MixinBase)), Is.True);
      Assert.That (PersistentMixinFinder.IsPersistenceRelevant (typeof (MixinA)), Is.True);
      Assert.That (PersistentMixinFinder.IsPersistenceRelevant (typeof (NonDomainObjectMixin)), Is.False);
    }

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
        + "Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.PersistentMixinFinderTest+PersistedGenericMixin`1 applied to class "
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order has open generic type parameters. All type parameters of the mixin must be "
        + "specified when it is applied to a DomainObject.")]
    public void PersistenceRelevant_OpenGenericMixin ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass (typeof (Order)).Clear ().AddMixins (typeof (PersistedGenericMixin<>)).EnterScope ())
      {
        new PersistentMixinFinder (typeof (Order)).GetPersistentMixins ();
      }
    }

    [Test]
    public void PersistenceIrrelevant_OpenGenericMixin ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass (typeof (Order)).Clear ().AddMixins (typeof (NonPersistedGenericMixin<>)).EnterScope ())
      {
        List<Type> persistentMixins = new PersistentMixinFinder (typeof (Order)).GetPersistentMixins ();
        Assert.That (persistentMixins, Is.Empty);
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Class 'Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping."
      + "MixinTestDomain.TargetClassA' suppresses mixin 'MixinA' inherited from its base class 'TargetClassBase'. This is not allowed because "
      + "the mixin adds persistence information to the base class which must also be present in the derived class.")]
    public void PersistenceRelevant_MixinSuppressingInherited ()
    {
      using (MixinConfiguration.BuildNew()
          .ForClass (typeof (TargetClassBase))
          .AddMixins (typeof (MixinA))
          .ForClass (typeof (TargetClassA))
          .AddMixin (typeof (MixinB)).SuppressMixin (typeof (MixinA))
          .EnterScope ())
      {
        new PersistentMixinFinder (typeof (TargetClassA)).GetPersistentMixins ();
      }
    }

    [Test]
    public void PersistenceIrrelevant_MixinSuppressingInherited ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass (typeof (TargetClassBase))
          .AddMixins (typeof (NonDomainObjectMixin))
          .ForClass (typeof (TargetClassA))
          .AddMixin (typeof (NonPersistedGenericMixin<>)).SuppressMixin (typeof (NonDomainObjectMixin))
          .EnterScope ())
      {
        List<Type> persistentMixins = new PersistentMixinFinder (typeof (TargetClassA)).GetPersistentMixins ();
        Assert.That (persistentMixins, Is.Empty);
      }
    }

    private void CheckPersistentMixins (Type targetType, params Type[] expectedTypes)
    {
      List<Type> mixinTypes = new PersistentMixinFinder (targetType).GetPersistentMixins ();
      Assert.That (mixinTypes, Is.EquivalentTo (expectedTypes));
    }
  }
}
