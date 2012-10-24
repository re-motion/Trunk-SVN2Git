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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.Mixins.Context;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Utilities;

namespace Remotion.Mixins.UnitTests.Core.Context.DeclarativeConfigurationBuilder_IntegrationTests
{
  [TestFixture]
  public class ExtendsAnalysisTest
  {
    [Extends (typeof (object))]
    [IgnoreForMixinConfiguration]
    public class Extender
    {
    }

    [Test]
    public void Origin ()
    {
      var configuration = new DeclarativeConfigurationBuilder (null).AddType (typeof (Extender)).BuildConfiguration ();
      var context = configuration.GetContext (typeof (object));
      var mixinContext = context.Mixins.Single ();

      var expectedOrigin = new MixinContextOrigin (
          "ExtendsAttribute",
          typeof (Extender).Assembly,
          "Remotion.Mixins.UnitTests.Core.Context.DeclarativeConfigurationBuilder_IntegrationTests.ExtendsAnalysisTest+Extender");
      Assert.That (mixinContext.Origin, Is.EqualTo (expectedOrigin));
    }

    [Extends (typeof (object))]
    [IgnoreForMixinConfiguration]
    public class ExtenderWithoutDependencies
    {
    }

    [Extends (typeof (object), AdditionalDependencies = new[] { typeof (string) })]
    [IgnoreForMixinConfiguration]
    public class ExtenderWithDependencies
    {
    }

    [Test]
    public void AdditionalDependencies ()
    {
      MixinConfiguration configuration =
          new DeclarativeConfigurationBuilder (null).AddType (typeof (ExtenderWithDependencies)).AddType (typeof (ExtenderWithoutDependencies)).BuildConfiguration ();
      Assert.AreEqual (0, configuration.GetContext (typeof (object)).Mixins[typeof (ExtenderWithoutDependencies)].ExplicitDependencies.Count);
      Assert.AreEqual (1, configuration.GetContext (typeof (object)).Mixins[typeof (ExtenderWithDependencies)].ExplicitDependencies.Count);
      Assert.That (configuration.GetContext (typeof (object)).Mixins[typeof (ExtenderWithDependencies)].ExplicitDependencies, Has.Member (typeof (string)));
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
      MixinConfiguration configuration = new DeclarativeConfigurationBuilder (null).AddType (typeof (ExtendingMixin))
          .AddType (typeof (ExtendsTargetDerivedWithoutExtends)).BuildConfiguration ();
      Assert.IsTrue (configuration.GetContext (typeof (ExtendsTargetDerivedWithoutExtends)).Mixins.ContainsKey (typeof (ExtendingMixin)));
      Assert.AreEqual (1, configuration.GetContext (typeof (ExtendsTargetDerivedWithoutExtends)).Mixins.Count);
    }

    public class ExtendsTargetDerivedWithExtends : ExtendsTargetBase { }

    [Test]
    public void InheritedDuplicateExtensionIsIgnored ()
    {
      MixinConfiguration configuration = new DeclarativeConfigurationBuilder (null).AddType (typeof (ExtendingMixin))
          .AddType (typeof (ExtendsTargetDerivedWithExtends)).BuildConfiguration ();
      Assert.IsTrue (configuration.GetContext (typeof (ExtendsTargetDerivedWithExtends)).Mixins.ContainsKey (typeof (ExtendingMixin)));
      Assert.AreEqual (1, configuration.GetContext (typeof (ExtendsTargetDerivedWithExtends)).Mixins.Count);
    }

    [Extends (typeof (ExtendsTargetDerivedWithDerivedExtends))]
    [IgnoreForMixinConfiguration]
    public class DerivedExtendingMixin : ExtendingMixin { }

    public class ExtendsTargetDerivedWithDerivedExtends : ExtendsTargetBase { }

    [Test]
    public void SubclassExtensionOverridesBaseExtends ()
    {
      MixinConfiguration configuration = new DeclarativeConfigurationBuilder (null)
          .AddType (typeof (ExtendingMixin))
          .AddType (typeof (DerivedExtendingMixin))
          .BuildConfiguration();

      var classContext = configuration.GetContext (typeof (ExtendsTargetDerivedWithDerivedExtends));

      Assert.IsFalse (classContext.Mixins.ContainsKey (typeof (ExtendingMixin)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (DerivedExtendingMixin)));
      Assert.AreEqual (1, classContext.Mixins.Count);
    }

    [Extends (typeof (ExtendsTargetDerivedWithDerivedExtends))]
    [IgnoreForMixinConfiguration]
    public class DerivedExtendingMixin2 : DerivedExtendingMixin { }

    [Test]
    public void ExplicitApplicationOfBaseAndDerivedMixinToSameClass()
    {
      MixinConfiguration configuration = new DeclarativeConfigurationBuilder (null).AddType (typeof (ExtendingMixin)).AddType (typeof (DerivedExtendingMixin))
          .AddType (typeof (DerivedExtendingMixin2)).BuildConfiguration ();

      Assert.IsFalse (configuration.GetContext (typeof (ExtendsTargetDerivedWithDerivedExtends)).Mixins.ContainsKey (typeof (ExtendingMixin)));
      Assert.IsTrue (configuration.GetContext (typeof (ExtendsTargetDerivedWithDerivedExtends)).Mixins.ContainsKey (typeof (DerivedExtendingMixin)));
      Assert.IsTrue (configuration.GetContext (typeof (ExtendsTargetDerivedWithDerivedExtends)).Mixins.ContainsKey (typeof (DerivedExtendingMixin2)));
      Assert.AreEqual (2, configuration.GetContext (typeof (ExtendsTargetDerivedWithDerivedExtends)).Mixins.Count);
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
      MixinConfiguration configuration = new DeclarativeConfigurationBuilder (null).AddType (typeof (MixinExtendingBaseAndDerived)).BuildConfiguration ();
      Assert.IsTrue (configuration.GetContext (typeof (ExtendsTargetBase)).Mixins.ContainsKey (typeof (MixinExtendingBaseAndDerived)));
      Assert.IsTrue (configuration.GetContext (typeof (ExtendsTargetDerivedWithoutExtends)).Mixins.ContainsKey (typeof (MixinExtendingBaseAndDerived)));
      Assert.AreEqual (1, configuration.GetContext (typeof (ExtendsTargetDerivedWithoutExtends)).Mixins.Count);
    }

    [Extends (typeof (ExtendsTargetBase), MixinTypeArguments = new[] { typeof (List<int>), typeof (IList<int>) })]
    [IgnoreForMixinConfiguration]
    public class GenericMixinWithSpecialization<TTarget, TNext> : Mixin<TTarget, TNext>
        where TTarget : class
        where TNext : class
    {
    }

    [Test]
    public void ExtendsCanSpecializeGenericMixin ()
    {
      MixinConfiguration configuration = new DeclarativeConfigurationBuilder (null).AddType (typeof (GenericMixinWithSpecialization<,>)).BuildConfiguration ();
      MixinContext mixinContext = new List<MixinContext> (configuration.GetContext (typeof (ExtendsTargetBase)).Mixins)[0];
      Assert.IsTrue (ReflectionUtility.CanAscribe (mixinContext.MixinType, typeof (GenericMixinWithSpecialization<,>)));
      Assert.IsFalse (mixinContext.MixinType.IsGenericTypeDefinition);
      Assert.IsFalse (mixinContext.MixinType.ContainsGenericParameters);
      Assert.AreEqual (new[] {typeof (List<int>), typeof (IList<int>)}, mixinContext.MixinType.GetGenericArguments());
    }

    [Extends (typeof (ExtendsTargetBase), MixinTypeArguments = new[] { typeof (List<int>) })]
    [IgnoreForMixinConfiguration]
    public class InvalidGenericMixin<TTarget, TNext> : Mixin<TTarget, TNext>
        where TTarget : class
        where TNext : class
    {
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "generic type argument", MatchType = MessageMatch.Contains)]
    public void InvalidTypeParametersThrowConfigurationException ()
    {
      new DeclarativeConfigurationBuilder (null).AddType (typeof (InvalidGenericMixin<,>)).BuildConfiguration ();
    }

    [Test]
    public void ExtendsAppliedToSpecificGenericClass ()
    {
      MixinConfiguration configuration = new DeclarativeConfigurationBuilder (null).AddType (typeof (MixinExtendingSpecificGenericClass)).BuildConfiguration();

      Assert.IsNotNull (configuration.GetContext (typeof (GenericClassExtendedByMixin<int>)));
      Assert.IsNull (configuration.GetContext (typeof (GenericClassExtendedByMixin<string>)));
      Assert.IsNull (configuration.GetContext (typeof (GenericClassExtendedByMixin<>)));
    }
  }
}
