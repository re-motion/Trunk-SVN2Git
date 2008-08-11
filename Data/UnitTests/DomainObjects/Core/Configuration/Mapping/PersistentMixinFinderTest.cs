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
      var finder = new PersistentMixinFinder (typeof (TargetClassBase));
      Assert.That (finder.ParentClassContext, Is.Null);
      Assert.That (finder.IsInParentContext (typeof (MixinBase)), Is.False);
    }

    [Test]
    public void IsInParentContext_WithParentContext ()
    {
      var finder = new PersistentMixinFinder (typeof (TargetClassA));
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
      CheckPersistentMixins (targetType, false, typeof (MixinBase));
    }

    [Test]
    public void ForTargetClassA ()
    {
      Type targetType = typeof (TargetClassA);
      CheckPersistentMixins (targetType, false, typeof (MixinA), typeof (MixinC), typeof (MixinD));
    }

    [Test]
    public void ForTargetClassB ()
    {
      Type targetType = typeof (TargetClassB);
      CheckPersistentMixins (targetType, false, typeof (MixinB), typeof (MixinE));
    }

    [Test]
    public void ForTargetClassB_WithIncludeInheritedMixins ()
    {
      Type targetType = typeof (TargetClassB);
      CheckPersistentMixins (targetType, true, typeof (MixinB), typeof (MixinE), typeof (MixinC), typeof (MixinD));
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
        Type[] persistentMixins = new PersistentMixinFinder (typeof (Order)).GetPersistentMixins ();
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
        Type[] persistentMixins = new PersistentMixinFinder (typeof (TargetClassA)).GetPersistentMixins ();
        Assert.That (persistentMixins, Is.Empty);
      }
    }

    [Test]
    public void FindOriginalMixinTarget_MixinNotDefined ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass (typeof (TargetClassA))
          .AddMixin (typeof (MixinA))
          .EnterScope ())
      {
        Type originalTarget = new PersistentMixinFinder (typeof (TargetClassA)).FindOriginalMixinTarget (typeof (MixinB));
        Assert.That (originalTarget, Is.Null);
      }
    }

    [Test]
    public void FindOriginalMixinTarget_MixinOnClass ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass (typeof (TargetClassA))
          .AddMixin (typeof (MixinA))
          .EnterScope ())
      {
        Type originalTarget = new PersistentMixinFinder (typeof (TargetClassA)).FindOriginalMixinTarget(typeof (MixinA));
        Assert.That (originalTarget, Is.SameAs (typeof (TargetClassA)));
      }
    }

    [Test]
    public void FindOriginalMixinTarget_MixinOnBase ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass (typeof (TargetClassBase))
          .AddMixin (typeof (MixinA))
          .EnterScope ())
      {
        Type originalTarget = new PersistentMixinFinder (typeof (TargetClassA)).FindOriginalMixinTarget (typeof (MixinA));
        Assert.That (originalTarget, Is.SameAs (typeof (TargetClassBase)));
      }
    }

    [Test]
    public void FindOriginalMixinTarget_MixinOnBaseBase ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass (typeof (TargetClassBase))
          .AddMixin (typeof (MixinA))
          .EnterScope ())
      {
        Type originalTarget = new PersistentMixinFinder (typeof (TargetClassB)).FindOriginalMixinTarget (typeof (MixinA));
        Assert.That (originalTarget, Is.SameAs (typeof (TargetClassBase)));
      }
    }

    [Test]
    public void FindOriginalMixinTarget_MixinOnBaseBaseBase ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass (typeof (object))
          .AddMixin (typeof (MixinA))
          .EnterScope ())
      {
        Type originalTarget = new PersistentMixinFinder (typeof (TargetClassB)).FindOriginalMixinTarget (typeof (MixinA));
        Assert.That (originalTarget, Is.SameAs (typeof (object)));
      }
    }

    [Test]
    public void FindOriginalMixinTarget_MixinOnBaseBaseBase_WithNonCurrentConfiguration ()
    {
      PersistentMixinFinder persistentMixinFinder;
      using (MixinConfiguration.BuildNew ()
          .ForClass (typeof (object))
          .AddMixin (typeof (MixinA))
          .EnterScope ())
      {
        persistentMixinFinder = new PersistentMixinFinder (typeof (TargetClassB));
      }
      Type originalTarget = persistentMixinFinder.FindOriginalMixinTarget (typeof (MixinA));
      Assert.That (originalTarget, Is.SameAs (typeof (object)));
    }

    private void CheckPersistentMixins (Type targetType, bool includeInherited, params Type[] expectedTypes)
    {
      Type[] mixinTypes = new PersistentMixinFinder (targetType, includeInherited).GetPersistentMixins ();
      Assert.That (mixinTypes, Is.EquivalentTo (expectedTypes));
    }
  }
}
