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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Utilities;

namespace Remotion.UnitTests.Mixins.Context.DeclarativeConfigurationBuilder_IntegrationTests
{
  [TestFixture]
  public class ExtendsAnalysisTest
  {
    [Extends (typeof (object))]
    [IgnoreForMixinConfiguration]
    public class ExtenderWithoutDependencies
    {
    }

    [Extends (typeof (object), AdditionalDependencies = new Type[] { typeof (string) })]
    [IgnoreForMixinConfiguration]
    public class ExtenderWithDependencies
    {
    }

    [Test]
    public void AdditionalDependencies ()
    {
      MixinConfiguration context =
          new DeclarativeConfigurationBuilder (null).AddType (typeof (ExtenderWithDependencies)).AddType (typeof (ExtenderWithoutDependencies)).BuildConfiguration ();
      Assert.AreEqual (0, context.ClassContexts.GetWithInheritance (typeof (object)).Mixins[typeof (ExtenderWithoutDependencies)].ExplicitDependencies.Count);
      Assert.AreEqual (1, context.ClassContexts.GetWithInheritance (typeof (object)).Mixins[typeof (ExtenderWithDependencies)].ExplicitDependencies.Count);
      Assert.IsTrue (context.ClassContexts.GetWithInheritance (typeof (object)).Mixins[typeof (ExtenderWithDependencies)].ExplicitDependencies.ContainsKey (typeof (string)));
    }

    public class ExtendsTargetBase { }

    [Extends (typeof (ExtendsTargetBase))]
    [Extends (typeof (ExtendsTargetDerivedWithExtends))]
    [IgnoreForMixinConfiguration]
    public class ExtendingMixin { }

    public class ExtendsTargetDerivedWithoutExtends : ExtendsTargetBase { }

    [Test]
    public void ExtendsAttributeAppliesToInheritanceChain ()
    {
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null).AddType (typeof (ExtendingMixin))
          .AddType (typeof (ExtendsTargetDerivedWithoutExtends)).BuildConfiguration ();
      Assert.IsTrue (context.ClassContexts.GetWithInheritance (typeof (ExtendsTargetDerivedWithoutExtends)).Mixins.ContainsKey (typeof (ExtendingMixin)));
      Assert.AreEqual (1, context.ClassContexts.GetWithInheritance (typeof (ExtendsTargetDerivedWithoutExtends)).Mixins.Count);
    }

    public class ExtendsTargetDerivedWithExtends : ExtendsTargetBase { }

    [Test]
    public void InheritedDuplicateExtensionIsIgnored ()
    {
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null).AddType (typeof (ExtendingMixin))
          .AddType (typeof (ExtendsTargetDerivedWithExtends)).BuildConfiguration ();
      Assert.IsTrue (context.ClassContexts.GetWithInheritance (typeof (ExtendsTargetDerivedWithExtends)).Mixins.ContainsKey (typeof (ExtendingMixin)));
      Assert.AreEqual (1, context.ClassContexts.GetWithInheritance (typeof (ExtendsTargetDerivedWithExtends)).Mixins.Count);
    }

    [Extends (typeof (ExtendsTargetDerivedWithDerivedExtends))]
    [IgnoreForMixinConfiguration]
    public class DerivedExtendingMixin : ExtendingMixin { }

    public class ExtendsTargetDerivedWithDerivedExtends : ExtendsTargetBase { }

    [Test]
    public void SubclassExtensionOverridesBaseExtends ()
    {
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null).AddType (typeof (ExtendingMixin)).AddType (typeof (DerivedExtendingMixin))
          .BuildConfiguration();

      Assert.IsFalse (context.ClassContexts.GetWithInheritance (typeof (ExtendsTargetDerivedWithDerivedExtends)).Mixins.ContainsKey (typeof (ExtendingMixin)));
      Assert.IsTrue (context.ClassContexts.GetWithInheritance (typeof (ExtendsTargetDerivedWithDerivedExtends)).Mixins.ContainsKey (typeof (DerivedExtendingMixin)));
      Assert.AreEqual (1, context.ClassContexts.GetWithInheritance (typeof (ExtendsTargetDerivedWithDerivedExtends)).Mixins.Count);
    }

    [Extends (typeof (ExtendsTargetDerivedWithDerivedExtends))]
    [IgnoreForMixinConfiguration]
    public class DerivedExtendingMixin2 : DerivedExtendingMixin { }

    [Test]
    public void ExplicitApplicationOfBaseAndDerivedMixinToSameClass()
    {
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null).AddType (typeof (ExtendingMixin)).AddType (typeof (DerivedExtendingMixin))
          .AddType (typeof (DerivedExtendingMixin2)).BuildConfiguration ();

      Assert.IsFalse (context.ClassContexts.GetWithInheritance (typeof (ExtendsTargetDerivedWithDerivedExtends)).Mixins.ContainsKey (typeof (ExtendingMixin)));
      Assert.IsTrue (context.ClassContexts.GetWithInheritance (typeof (ExtendsTargetDerivedWithDerivedExtends)).Mixins.ContainsKey (typeof (DerivedExtendingMixin)));
      Assert.IsTrue (context.ClassContexts.GetWithInheritance (typeof (ExtendsTargetDerivedWithDerivedExtends)).Mixins.ContainsKey (typeof (DerivedExtendingMixin2)));
      Assert.AreEqual (2, context.ClassContexts.GetWithInheritance (typeof (ExtendsTargetDerivedWithDerivedExtends)).Mixins.Count);
    }

    [Extends (typeof (ExtendsTargetBase))]
    [Extends (typeof (ExtendsTargetBase))]
    [IgnoreForMixinConfiguration]
    public class DoubleExtendingMixin { }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Two instances of mixin .*DoubleExtendingMixin are "
        + "configured for target type .*ExtendsTargetBase.", MatchType = MessageMatch.Regex)]
    public void ThrowsOnDuplicateExtendsForSameClass ()
    {
      new DeclarativeConfigurationBuilder (null).AddType (typeof (DoubleExtendingMixin)).BuildConfiguration ();
    }

    [Extends (typeof (ExtendsTargetBase))]
    [Extends (typeof (ExtendsTargetDerivedWithoutExtends))]
    [IgnoreForMixinConfiguration]
    public class MixinExtendingBaseAndDerived { }

    [Test]
    public void DuplicateExtendsForSameClassInInheritanceHierarchyIsIgnored ()
    {
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null).AddType (typeof (MixinExtendingBaseAndDerived)).BuildConfiguration ();
      Assert.IsTrue (context.ClassContexts.GetWithInheritance (typeof (ExtendsTargetBase)).Mixins.ContainsKey (typeof (MixinExtendingBaseAndDerived)));
      Assert.IsTrue (context.ClassContexts.GetWithInheritance (typeof (ExtendsTargetDerivedWithoutExtends)).Mixins.ContainsKey (typeof (MixinExtendingBaseAndDerived)));
      Assert.AreEqual (1, context.ClassContexts.GetWithInheritance (typeof (ExtendsTargetDerivedWithoutExtends)).Mixins.Count);
    }

    [Extends (typeof (ExtendsTargetBase), MixinTypeArguments = new Type[] { typeof (List<int>), typeof (IList<int>) })]
    [IgnoreForMixinConfiguration]
    public class GenericMixinWithSpecialization<TThis, TBase> : Mixin<TThis, TBase>
        where TThis : class
        where TBase : class
    {
    }

    [Test]
    public void ExtendsCanSpecializeGenericMixin ()
    {
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null).AddType (typeof (GenericMixinWithSpecialization<,>)).BuildConfiguration ();
      MixinContext mixinContext = new List<MixinContext> (context.ClassContexts.GetWithInheritance (typeof (ExtendsTargetBase)).Mixins)[0];
      Assert.IsTrue (ReflectionUtility.CanAscribe (mixinContext.MixinType, typeof (GenericMixinWithSpecialization<,>)));
      Assert.IsFalse (mixinContext.MixinType.IsGenericTypeDefinition);
      Assert.IsFalse (mixinContext.MixinType.ContainsGenericParameters);
      Assert.AreEqual (new Type[] {typeof (List<int>), typeof (IList<int>)}, mixinContext.MixinType.GetGenericArguments());
    }

    [Extends (typeof (ExtendsTargetBase), MixinTypeArguments = new Type[] { typeof (List<int>) })]
    [IgnoreForMixinConfiguration]
    public class InvalidGenericMixin<TThis, TBase> : Mixin<TThis, TBase>
        where TThis : class
        where TBase : class
    {
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The ExtendsAttribute for target class "
        + ".*ExtendsTargetBase applied to mixin type "
            + ".*InvalidGenericMixin`2 specified invalid generic type arguments.", MatchType = MessageMatch.Regex)]
    public void InvalidTypeParametersThrowConfigurationException ()
    {
      new DeclarativeConfigurationBuilder (null).AddType (typeof (InvalidGenericMixin<,>)).BuildConfiguration ();
    }

    [Extends (typeof (ExtendsTargetBase), SuppressedMixins = new Type[] { typeof (ExtendingMixin) })]
    [IgnoreForMixinConfiguration]
    public class SuppressingExtender { }

    [Test]
    public void SuppressedMixins ()
    {
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null).AddType (typeof (ExtendingMixin)).AddType (typeof (SuppressingExtender))
          .BuildConfiguration ();
      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (ExtendsTargetBase));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (SuppressingExtender)));
      Assert.IsFalse (classContext.Mixins.ContainsKey (typeof (ExtendingMixin)));
    }

    [Extends (typeof (ExtendsTargetBase), SuppressedMixins = new Type[] { typeof (CircularSuppressingExtender2) })]
    [IgnoreForMixinConfiguration]
    public class CircularSuppressingExtender1 { }

    [Extends (typeof (ExtendsTargetBase), SuppressedMixins = new Type[] { typeof (CircularSuppressingExtender1) })]
    [IgnoreForMixinConfiguration]
    public class CircularSuppressingExtender2 { }

    [Test]
    public void CircularSuppressingMixins ()
    {
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null).AddType (typeof (CircularSuppressingExtender1))
          .AddType (typeof (CircularSuppressingExtender2)).BuildConfiguration ();
      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (ExtendsTargetBase));
      Assert.IsFalse (classContext.Mixins.ContainsKey (typeof (CircularSuppressingExtender1)));
      Assert.IsFalse (classContext.Mixins.ContainsKey (typeof (CircularSuppressingExtender2)));
    }

    [Extends (typeof (ExtendsTargetBase), SuppressedMixins = new Type[] { typeof (SelfSuppressingExtender2) })]
    [IgnoreForMixinConfiguration]
    public class SelfSuppressingExtender2 { }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Mixin type "
        + ".*SelfSuppressingExtender2 applied to target class .*ExtendsTargetBase suppresses itself.", MatchType = MessageMatch.Regex)]
    public void SelfSuppressingMixin ()
    {
      new DeclarativeConfigurationBuilder (null).AddType (typeof (SelfSuppressingExtender2)).BuildConfiguration ();
    }

    [Extends (typeof (ExtendsTargetBase))]
    [IgnoreForMixinConfiguration]
    public class GenericMixinWithoutSpecialization<TThis, TBase> : Mixin<TThis, TBase>
        where TThis : class
        where TBase : class
    {
    }

    [Extends (typeof (ExtendsTargetBase), SuppressedMixins = new Type[] { typeof (GenericMixinWithoutSpecialization<,>),
        typeof (GenericMixinWithSpecialization<,>) })]
    [IgnoreForMixinConfiguration]
    public class GenericSuppressingExtender { }

    [Test]
    public void GenericSuppressingMixinWithSpecialization ()
    {
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null)
          .AddType (typeof (GenericMixinWithSpecialization<,>))
          .AddType (typeof (GenericSuppressingExtender))
          .BuildConfiguration ();
      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (ExtendsTargetBase));
      Assert.IsFalse (classContext.Mixins.ContainsKey (typeof (GenericMixinWithSpecialization<List<int>, IList<int>>)));
    }

    [Test]
    public void GenericSuppressingMixinWithoutSpecialization ()
    {
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null)
          .AddType (typeof (GenericMixinWithoutSpecialization<,>))
          .AddType (typeof (GenericSuppressingExtender))
          .BuildConfiguration ();
      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (ExtendsTargetBase));
      Assert.IsFalse (classContext.Mixins.ContainsKey (typeof (GenericMixinWithoutSpecialization<,>)));
    }

    [Extends (typeof (ExtendsTargetBase), SuppressedMixins = new Type[] { typeof (GenericMixinWithSpecialization<List<int>, IList<int>>) })]
    [IgnoreForMixinConfiguration]
    public class ClosedGenericSuppressingExtender { }

    [Test]
    public void ClosedGenericSuppressingMixin ()
    {
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null)
          .AddType (typeof (GenericMixinWithSpecialization<,>))
          .AddType (typeof (ClosedGenericSuppressingExtender)).BuildConfiguration ();
      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (ExtendsTargetBase));
      Assert.IsFalse (classContext.Mixins.ContainsKey (typeof (GenericMixinWithSpecialization<List<int>, IList<int>>)));
    }

    [Extends (typeof (ExtendsTargetBase), SuppressedMixins = new Type[] { typeof (GenericMixinWithSpecialization<object, string>) })]
    [IgnoreForMixinConfiguration]
    public class ClosedGenericSuppressingExtender_WithWrongParameterTypes { }

    [Test]
    public void ClosedGenericSuppressingMixin_WithWrongParameterTypes ()
    {
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null)
          .AddType (typeof (GenericMixinWithSpecialization<,>))
          .AddType (typeof (ClosedGenericSuppressingExtender_WithWrongParameterTypes)).BuildConfiguration ();
      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (ExtendsTargetBase));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (GenericMixinWithSpecialization<List<int>, IList<int>>)));
    }

    [Test]
    public void ExtendsAppliedToSpecificGenericClass ()
    {
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null).AddType (typeof (MixinExtendingSpecificGenericClass)).BuildConfiguration();
      Assert.IsTrue (context.ClassContexts.ContainsWithInheritance (typeof (GenericClassExtendedByMixin<int>)));
      Assert.IsFalse (context.ClassContexts.ContainsWithInheritance (typeof (GenericClassExtendedByMixin<string>)));
      Assert.IsFalse (context.ClassContexts.ContainsWithInheritance (typeof (GenericClassExtendedByMixin<>)));

      Assert.IsNotNull (context.ClassContexts.GetWithInheritance (typeof (GenericClassExtendedByMixin<int>)));
      Assert.IsNull (context.ClassContexts.GetWithInheritance (typeof (GenericClassExtendedByMixin<string>)));
      Assert.IsNull (context.ClassContexts.GetWithInheritance (typeof (GenericClassExtendedByMixin<>)));
    }
  }
}
