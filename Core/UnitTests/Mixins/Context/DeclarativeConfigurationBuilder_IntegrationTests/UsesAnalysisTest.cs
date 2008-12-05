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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Utilities;

namespace Remotion.UnitTests.Mixins.Context.DeclarativeConfigurationBuilder_IntegrationTests
{
  [TestFixture]
  public class UsesAnalysisTest
  {
    [Uses (typeof (NullMixin))]
    [IgnoreForMixinConfiguration]
    public class UserWithoutDependencies
    {
    }

    [Uses (typeof (NullMixin), AdditionalDependencies = new Type[] { typeof (string) })]
    [IgnoreForMixinConfiguration]
    public class UserWithDependencies
    {
    }

    [Test]
    public void AdditionalDependencies ()
    {
      MixinConfiguration context =
          new DeclarativeConfigurationBuilder (null).AddType (typeof (UserWithoutDependencies)).AddType (typeof (UserWithDependencies)).BuildConfiguration();
      Assert.AreEqual (0, context.ClassContexts.GetWithInheritance (typeof (UserWithoutDependencies)).Mixins[typeof (NullMixin)].ExplicitDependencies.Count);
      Assert.AreEqual (1, context.ClassContexts.GetWithInheritance (typeof (UserWithDependencies)).Mixins[typeof (NullMixin)].ExplicitDependencies.Count);
      Assert.IsTrue (context.ClassContexts.GetWithInheritance (typeof (UserWithDependencies)).Mixins[typeof (NullMixin)].ExplicitDependencies.ContainsKey (typeof (string)));
    }

    [Uses (typeof (NullMixin), AdditionalDependencies = new Type[] { typeof (object) })]
    [IgnoreForMixinConfiguration]
    public class BaseWithUses { }

    public class DerivedWithoutUses : BaseWithUses { }

    [Test]
    public void UsesAttributeIsInherited ()
    {
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithoutUses)).BuildConfiguration ();
      Assert.IsTrue (context.ClassContexts.GetWithInheritance (typeof (DerivedWithoutUses)).Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (context.ClassContexts.GetWithInheritance (typeof (DerivedWithoutUses)).Mixins[typeof (NullMixin)].ExplicitDependencies
          .ContainsKey (typeof (object)));
      Assert.AreEqual (1, context.ClassContexts.GetWithInheritance (typeof (DerivedWithoutUses)).Mixins.Count);
    }

    public class DedicatedMixin {}

    [Uses( typeof (DedicatedMixin))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithOwnUses : BaseWithUses { }

    [Test]
    public void UsesAttributeIsInherited_AndAugmentedWithOwn ()
    {
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithOwnUses)).BuildConfiguration ();
      Assert.IsTrue (context.ClassContexts.GetWithInheritance (typeof (DerivedWithOwnUses)).Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (context.ClassContexts.GetWithInheritance (typeof (DerivedWithOwnUses)).Mixins.ContainsKey (typeof (DedicatedMixin)));

      Type[] mixinTypes = EnumerableUtility.SelectToArray<MixinContext, Type> (
          context.ClassContexts.GetWithInheritance (typeof (DerivedWithOwnUses)).Mixins, delegate (MixinContext mixin) { return mixin.MixinType; });
      
      Assert.That (mixinTypes, Is.EquivalentTo (new Type[] {typeof (NullMixin), typeof (DedicatedMixin)}));
      Assert.AreEqual (2, context.ClassContexts.GetWithInheritance (typeof (DerivedWithOwnUses)).Mixins.Count);
    }

    [Uses (typeof (NullMixin))]
    public class GenericBaseWithMixin<T>
    {
    }

    public class GenericDerivedWithInheritedMixin<T> : GenericBaseWithMixin<T>
    {
    }

    [Test]
    public void UsesAttributeIsInheritedOnOpenGenericTypes ()
    {
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null).AddType (typeof (GenericDerivedWithInheritedMixin<>)).BuildConfiguration ();
      Assert.IsTrue (context.ClassContexts.GetWithInheritance (typeof (GenericDerivedWithInheritedMixin<>)).Mixins.ContainsKey (typeof (NullMixin)));
      Assert.AreEqual (1, context.ClassContexts.GetWithInheritance (typeof (GenericDerivedWithInheritedMixin<>)).Mixins.Count);
    }

    public class NonGenericDerivedWithInheritedMixinFromGeneric : GenericBaseWithMixin<int>
    {
    }

    [Test]
    public void UsesAttributeIsInheritedOnNonGenericTypesInheritingFromGeneric ()
    {
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null).AddType (typeof (NonGenericDerivedWithInheritedMixinFromGeneric)).BuildConfiguration ();
      Assert.IsTrue (context.ClassContexts.GetWithInheritance (typeof (NonGenericDerivedWithInheritedMixinFromGeneric)).Mixins.ContainsKey (typeof (NullMixin)));
      Assert.AreEqual (1, context.ClassContexts.GetWithInheritance (typeof (NonGenericDerivedWithInheritedMixinFromGeneric)).Mixins.Count);
    }

    [Uses (typeof (NullMixin))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithUses : BaseWithUses
    {
    }

    [Test]
    public void InheritedUsesDuplicateIsIgnored ()
    {
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithUses)).BuildConfiguration ();
      Assert.IsTrue (context.ClassContexts.GetWithInheritance (typeof (DerivedWithUses)).Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsFalse (context.ClassContexts.GetWithInheritance (typeof (DerivedWithUses)).Mixins[typeof (NullMixin)]
          .ExplicitDependencies.ContainsKey (typeof (object)));
      Assert.AreEqual (1, context.ClassContexts.GetWithInheritance (typeof (DerivedWithUses)).Mixins.Count);
    }

    [Uses (typeof (DerivedNullMixin))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithMoreSpecificUses : BaseWithUses
    {
    }

    [Test]
    public void InheritedUsesCanBeOverridden ()
    {
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithMoreSpecificUses)).BuildConfiguration ();
      Assert.IsTrue (context.ClassContexts.GetWithInheritance (typeof (DerivedWithMoreSpecificUses)).Mixins.ContainsKey (typeof (DerivedNullMixin)));
      Assert.IsFalse (context.ClassContexts.GetWithInheritance (typeof (DerivedWithMoreSpecificUses)).Mixins.ContainsKey (typeof (NullMixin)));
      Assert.AreEqual (1, context.ClassContexts.GetWithInheritance (typeof (DerivedWithMoreSpecificUses)).Mixins.Count);
    }

    public class BaseGenericMixin<TThis, TBase> : Mixin<TThis, TBase>
        where TThis : class
        where TBase : class
    { }

    public class DerivedGenericMixin<TThis, TBase> : BaseGenericMixin<TThis, TBase>
        where TThis : class
        where TBase : class
    {
    }

    public class DerivedClosedMixin : BaseGenericMixin<object, object> { }

    [Uses (typeof (BaseGenericMixin<,>))]
    [IgnoreForMixinConfiguration]
    public class BaseWithOpenGeneric
    {
    }

    [Uses (typeof (BaseGenericMixin<BaseWithClosedGeneric, object>))]
    [IgnoreForMixinConfiguration]
    public class BaseWithClosedGeneric
    {
    }

    [Uses (typeof (DerivedGenericMixin<,>))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithOpenOverridingOpen : BaseWithOpenGeneric
    {
    }

    [Uses (typeof (DerivedGenericMixin<object, object>))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithClosedOverridingOpen : BaseWithOpenGeneric
    {
    }

    [Uses (typeof (DerivedGenericMixin<,>))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithOpenOverridingClosed : BaseWithClosedGeneric
    {
    }

    [Uses (typeof (DerivedGenericMixin<object, object>))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithClosedOverridingClosed : BaseWithClosedGeneric
    {
    }

    [Uses (typeof (DerivedClosedMixin))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithRealClosedOverridingOpen : BaseWithOpenGeneric
    {
    }

    [Uses (typeof (DerivedClosedMixin))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithRealClosedOverridingClosed : BaseWithClosedGeneric
    {
    }

    [Test]
    public void OverrideAlsoWorksForGenericsOpenOpen ()
    {
      ClassContext ctx = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithOpenOverridingOpen)).BuildConfiguration ().ClassContexts.GetWithInheritance (typeof (DerivedWithOpenOverridingOpen));
      Assert.IsTrue (ctx.Mixins.ContainsKey (typeof (DerivedGenericMixin<,>)));
      Assert.IsFalse (ctx.Mixins.ContainsKey (typeof (BaseGenericMixin<,>)));
      Assert.AreEqual (1, ctx.Mixins.Count);
    }

    [Test]
    public void OverrideAlsoWorksForGenericsOpenClosed ()
    {
      ClassContext ctx = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithOpenOverridingClosed)).BuildConfiguration ().ClassContexts.GetWithInheritance (typeof (DerivedWithOpenOverridingClosed));
      Assert.IsTrue (ctx.Mixins.ContainsKey (typeof (DerivedGenericMixin<,>)));
      Assert.IsFalse (ctx.Mixins.ContainsKey (typeof (BaseGenericMixin<BaseWithClosedGeneric, object>)));
      Assert.AreEqual (1, ctx.Mixins.Count);
    }

    [Test]
    public void OverrideAlsoWorksForGenericsClosedOpen ()
    {
      ClassContext ctx = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithClosedOverridingOpen)).BuildConfiguration ().ClassContexts.GetWithInheritance (typeof (DerivedWithClosedOverridingOpen));
      Assert.IsTrue (ctx.Mixins.ContainsKey (typeof (DerivedGenericMixin<object, object>)));
      Assert.IsFalse (ctx.Mixins.ContainsKey (typeof (BaseGenericMixin<,>)));
      Assert.AreEqual (1, ctx.Mixins.Count);
    }

    [Test]
    public void OverrideAlsoWorksForGenericsClosedClosed ()
    {
      ClassContext ctx = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithClosedOverridingClosed)).BuildConfiguration ().ClassContexts.GetWithInheritance (typeof (DerivedWithClosedOverridingClosed));
      Assert.IsTrue (ctx.Mixins.ContainsKey (typeof (DerivedGenericMixin<object, object>)));
      Assert.IsFalse (ctx.Mixins.ContainsKey (typeof (BaseGenericMixin<BaseWithClosedGeneric, object>)));
      Assert.AreEqual (1, ctx.Mixins.Count);
    }

    [Test]
    public void OverrideAlsoWorksForGenericsRealClosedOpen ()
    {
      ClassContext ctx = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithRealClosedOverridingOpen)).BuildConfiguration ().ClassContexts.GetWithInheritance (typeof (DerivedWithRealClosedOverridingOpen));
      Assert.IsTrue (ctx.Mixins.ContainsKey (typeof (DerivedClosedMixin)));
      Assert.IsFalse (ctx.Mixins.ContainsKey (typeof (BaseGenericMixin<,>)));
      Assert.AreEqual (1, ctx.Mixins.Count);
    }

    [Test]
    public void OverrideAlsoWorksForGenericsRealClosedClosed ()
    {
      ClassContext ctx = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithRealClosedOverridingClosed)).BuildConfiguration ().ClassContexts.GetWithInheritance (typeof (DerivedWithRealClosedOverridingClosed));
      Assert.IsTrue (ctx.Mixins.ContainsKey (typeof (DerivedClosedMixin)));
      Assert.IsFalse (ctx.Mixins.ContainsKey (typeof (BaseGenericMixin<BaseWithClosedGeneric, object>)));
      Assert.AreEqual (1, ctx.Mixins.Count);
    }

    [Uses (typeof (NullMixin))]
    [Uses (typeof (NullMixin))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithDuplicateUses : BaseWithUses
    {
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Two instances of mixin .*NullMixin are configured for target type "
        + ".*DerivedWithDuplicateUses.", MatchType = MessageMatch.Regex)]
    public void ThrowsOnUsesDuplicateOnSameClass ()
    {
      new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithDuplicateUses)).BuildConfiguration ();
    }

    [Uses (typeof (BaseGenericMixin<,>))]
    [Uses (typeof (BaseGenericMixin<,>))]
    [IgnoreForMixinConfiguration]
    public class DuplicateWithGenerics1
    {
    }

    [Uses (typeof (BaseGenericMixin<,>))]
    [Uses (typeof (BaseGenericMixin<object, object>))]
    [IgnoreForMixinConfiguration]
    public class DuplicateWithGenerics2
    {
    }

    [Uses (typeof (BaseGenericMixin<DuplicateWithGenerics3, object>))]
    [Uses (typeof (BaseGenericMixin<object, object>))]
    [IgnoreForMixinConfiguration]
    public class DuplicateWithGenerics3
    {
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Two instances of mixin .*BaseGenericMixin`2 are configured for target "
        + "type .*DuplicateWithGenerics1.", MatchType = MessageMatch.Regex)]
    public void DuplicateDetectionAlsoWorksForGenerics1 ()
    {
      new DeclarativeConfigurationBuilder (null).AddType (typeof (DuplicateWithGenerics1)).BuildConfiguration ();
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Two instances of mixin .*BaseGenericMixin`2 are configured for target "
        + "type .*DuplicateWithGenerics2.", MatchType = MessageMatch.Regex)]
    public void DuplicateDetectionAlsoWorksForGenerics2 ()
    {
      new DeclarativeConfigurationBuilder (null).AddType (typeof (DuplicateWithGenerics2)).BuildConfiguration ();
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Two instances of mixin .*BaseGenericMixin`2 are configured for target "
        + "type .*DuplicateWithGenerics3.", MatchType = MessageMatch.Regex)]
    public void DuplicateDetectionAlsoWorksForGenerics3 ()
    {
      new DeclarativeConfigurationBuilder (null).AddType (typeof (DuplicateWithGenerics3)).BuildConfiguration ();
    }

    [Uses (typeof (NullMixin), SuppressedMixins = new Type[] { typeof (SuppressedExtender) })]
    [IgnoreForMixinConfiguration]
    public class SuppressingUser { }

    [Extends (typeof (SuppressingUser))]
    public class SuppressedExtender
    {
    }

    [Test]
    public void SuppressedMixins ()
    {
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null)
          .AddType (typeof (SuppressingUser))
          .AddType (typeof (SuppressedExtender))
          .BuildConfiguration ();
      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (SuppressingUser));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsFalse (classContext.Mixins.ContainsKey (typeof (SuppressedExtender)));
    }

    [Uses (typeof (NullMixin), SuppressedMixins = new Type[] { typeof (NullMixin) })]
    [IgnoreForMixinConfiguration]
    public class SelfSuppressingUser { }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Mixin type Remotion.UnitTests.Mixins.SampleTypes.NullMixin applied to "
        + "target class .*SelfSuppressingUser suppresses itself.", MatchType = MessageMatch.Regex)]
    public void SelfSuppresser ()
    {
      new DeclarativeConfigurationBuilder (null).AddType (typeof (SelfSuppressingUser)).BuildConfiguration ();
    }
  }
}
