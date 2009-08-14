// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Collections;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Definitions.Building.DependencySorting;
using Remotion.Mixins.Utilities.DependencySort;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Definitions
{
  [TestFixture]
  public class MixinDependencySortTest
  {
    [Test]
    public void MixinDefinitionsAreSortedCorrectlySmall()
    {
      TargetClassDefinition bt7 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType7));
      Assert.AreEqual (7, bt7.Mixins.Count);
      // group 1
      Assert.AreEqual (0, bt7.Mixins[typeof (BT7Mixin0)].MixinIndex);

      Assert.AreEqual (1, bt7.Mixins[typeof (BT7Mixin2)].MixinIndex);
      Assert.AreEqual (2, bt7.Mixins[typeof (BT7Mixin3)].MixinIndex);
      Assert.AreEqual (3, bt7.Mixins[typeof (BT7Mixin1)].MixinIndex);

      // group 2
      Assert.AreEqual (4, bt7.Mixins[typeof (BT7Mixin10)].MixinIndex);
      Assert.AreEqual (5, bt7.Mixins[typeof (BT7Mixin9)].MixinIndex);

      // group 3
      Assert.AreEqual (6, bt7.Mixins[typeof (BT7Mixin5)].MixinIndex);
    }

    [Test]
    public void MixinDefinitionsAreSortedCorrectlyGrand ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType7> ().Clear().AddMixins (typeof (BT7Mixin0), typeof (BT7Mixin1), typeof (BT7Mixin2), typeof (BT7Mixin3), typeof (BT7Mixin4), typeof (BT7Mixin5), typeof (BT7Mixin6), typeof (BT7Mixin7), typeof (BT7Mixin8), typeof (BT7Mixin9), typeof (BT7Mixin10)).EnterScope())
      {
        CheckGrandOrdering();
      }
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType7> ().Clear().AddMixins (typeof (BT7Mixin10), typeof (BT7Mixin9), typeof (BT7Mixin8), typeof (BT7Mixin7), typeof (BT7Mixin6), typeof (BT7Mixin5), typeof (BT7Mixin4), typeof (BT7Mixin3), typeof (BT7Mixin2), typeof (BT7Mixin1), typeof (BT7Mixin0)).EnterScope())
      {
        CheckGrandOrdering ();
      }
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType7> ().Clear().AddMixins (typeof (BT7Mixin5), typeof (BT7Mixin8), typeof (BT7Mixin9), typeof (BT7Mixin2), typeof (BT7Mixin1), typeof (BT7Mixin10), typeof (BT7Mixin4), typeof (BT7Mixin0), typeof (BT7Mixin6), typeof (BT7Mixin3), typeof (BT7Mixin7)).EnterScope())
      {
        CheckGrandOrdering ();
      }
    }

    private void CheckGrandOrdering ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass<BaseType7> ()
          .EnsureMixin (typeof (BT7Mixin0)).WithDependency<IBT7Mixin7> ()
          .EnsureMixin (typeof (BT7Mixin7)).WithDependency<IBT7Mixin4> ()
          .EnsureMixin (typeof (BT7Mixin4)).WithDependency<IBT7Mixin6> ()
          .EnsureMixin (typeof (BT7Mixin6)).WithDependency<IBT7Mixin2> ()
          .EnsureMixin (typeof (BT7Mixin9)).WithDependency<IBT7Mixin8> ()
          .EnterScope ())
      {
        TargetClassDefinition bt7 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType7));
        Assert.AreEqual (11, bt7.Mixins.Count);
        // group 1
        Assert.AreEqual (0, bt7.Mixins[typeof (BT7Mixin0)].MixinIndex); // u
        Assert.AreEqual (1, bt7.Mixins[typeof (BT7Mixin7)].MixinIndex); // u
        Assert.AreEqual (2, bt7.Mixins[typeof (BT7Mixin4)].MixinIndex); // u
        Assert.AreEqual (3, bt7.Mixins[typeof (BT7Mixin6)].MixinIndex); // u

        Assert.AreEqual (4, bt7.Mixins[typeof (BT7Mixin2)].MixinIndex);
        Assert.AreEqual (5, bt7.Mixins[typeof (BT7Mixin3)].MixinIndex);
        Assert.AreEqual (6, bt7.Mixins[typeof (BT7Mixin1)].MixinIndex);

        // group 2
        Assert.AreEqual (7, bt7.Mixins[typeof (BT7Mixin10)].MixinIndex);
        Assert.AreEqual (8, bt7.Mixins[typeof (BT7Mixin9)].MixinIndex); // u
        Assert.AreEqual (9, bt7.Mixins[typeof (BT7Mixin8)].MixinIndex); // u

        // group 3
        Assert.AreEqual (10, bt7.Mixins[typeof (BT7Mixin5)].MixinIndex);
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = @"The following mixins are applied to the same base class .*BaseType7 and "
                                                                           + "require a clear base call ordering, but do not provide enough dependency information: "
                                                                           + @"((.*BT7Mixin0)|(.*BT7Mixin4)|(.*BT7Mixin6)|(.*BT7Mixin7)){4}\.",
        MatchType = MessageMatch.Regex)]
    public void ThrowsIfConnectedMixinsCannotBeSorted()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType7> ().Clear().AddMixins (typeof (BT7Mixin0), typeof (BT7Mixin4), typeof (BT7Mixin6), typeof (BT7Mixin7), typeof (BT7Mixin2), typeof (BT7Mixin5)).EnterScope())
      {
        DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType7));
      }
    }

    public interface ICircular1 { }

    public class Circular1 : Mixin<object, ICircular2>, ICircular1
    {
    }

    public interface ICircular2 { }

    public class Circular2 : Mixin<object, ICircular1>, ICircular2
    {
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "circular dependencies.*Circular[12].*Circular[12]",
        MatchType = MessageMatch.Regex)]
    public void ThrowsOnCircularDependencies()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<object> ().Clear().AddMixins (typeof (Circular1), typeof (Circular2)).EnterScope())
      {
        DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (object));
      }
    }

    interface ISelfDependency { }

    class SelfDependency : Mixin<object, ISelfDependency>, ISelfDependency
    {
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "circular dependencies.*SelfDependency",
        MatchType = MessageMatch.Regex)]
    public void ThrowsOnSelfDependencies ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<object> ().Clear().AddMixins (typeof (SelfDependency)).EnterScope())
      {
        DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (object));
      }
    }

    [Test]
    public void DependencyAnalyzer ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType7> ().Clear()
          .AddMixin (typeof (BT7Mixin0)).WithDependency (typeof (IBT7Mixin7))
          .AddMixin (typeof (BT7Mixin7)).WithDependency (typeof (IBT7Mixin4))
          .AddMixin (typeof (BT7Mixin4)).WithDependency (typeof (IBT7Mixin6))
          .AddMixin (typeof (BT7Mixin6)).WithDependency (typeof (IBT7Mixin2))
          .AddMixin (typeof (BT7Mixin9)).WithDependency (typeof (IBT7Mixin8))
          .AddMixins (typeof (BT7Mixin1), typeof (BT7Mixin2), typeof (BT7Mixin3), typeof (BT7Mixin5), typeof (BT7Mixin8), typeof (BT7Mixin10))
          .EnterScope())
      {
        TargetClassDefinition bt7 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType7));
        MixinDependencyAnalyzer analyzer = new MixinDependencyAnalyzer();

        Assert.AreEqual (DependencyKind.None, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin1)], bt7.Mixins[typeof (BT7Mixin0)]));
        Assert.AreEqual (DependencyKind.None, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin0)], bt7.Mixins[typeof (BT7Mixin1)]));

        Assert.AreEqual (DependencyKind.None, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin1)], bt7.Mixins[typeof (BT7Mixin2)]));
        Assert.AreEqual (DependencyKind.None, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin2)], bt7.Mixins[typeof (BT7Mixin1)]));

        Assert.AreEqual (DependencyKind.None, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin4)], bt7.Mixins[typeof (BT7Mixin0)]));
        Assert.AreEqual (DependencyKind.None, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin4)], bt7.Mixins[typeof (BT7Mixin1)]));
        Assert.AreEqual (DependencyKind.None, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin4)], bt7.Mixins[typeof (BT7Mixin2)]));
        Assert.AreEqual (DependencyKind.None, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin4)], bt7.Mixins[typeof (BT7Mixin3)]));

        Assert.AreEqual (
            DependencyKind.FirstOnSecond, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin0)], bt7.Mixins[typeof (BT7Mixin2)]));

        Assert.AreEqual (
            DependencyKind.SecondOnFirst, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin1)], bt7.Mixins[typeof (BT7Mixin3)]));
        Assert.AreEqual (
            DependencyKind.FirstOnSecond, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin3)], bt7.Mixins[typeof (BT7Mixin1)]));

        Assert.AreEqual (
            DependencyKind.FirstOnSecond, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin2)], bt7.Mixins[typeof (BT7Mixin3)]));
        Assert.AreEqual (
            DependencyKind.SecondOnFirst, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin3)], bt7.Mixins[typeof (BT7Mixin2)]));

        Assert.AreEqual (
            DependencyKind.SecondOnFirst, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin3)], bt7.Mixins[typeof (BT7Mixin2)]));

        Assert.AreEqual (
            DependencyKind.FirstOnSecond, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin7)], bt7.Mixins[typeof (BT7Mixin2)]));
      }
    }
    
    [Test]
    public void MixinGroupBuilder ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass<BaseType7> ().Clear ()
          .AddMixin (typeof (BT7Mixin0)).WithDependency (typeof (IBT7Mixin7))
          .AddMixin (typeof (BT7Mixin7)).WithDependency (typeof (IBT7Mixin4))
          .AddMixin (typeof (BT7Mixin4)).WithDependency (typeof (IBT7Mixin6))
          .AddMixin (typeof (BT7Mixin6)).WithDependency (typeof (IBT7Mixin2))
          .AddMixin (typeof (BT7Mixin9)).WithDependency (typeof (IBT7Mixin8))
          .AddMixins (typeof (BT7Mixin1), typeof (BT7Mixin2), typeof (BT7Mixin3), typeof (BT7Mixin5), typeof (BT7Mixin8), typeof (BT7Mixin10))
          .EnterScope ())
      {
        TargetClassDefinition bt7 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType7));
        DependentMixinGrouper grouper = new DependentMixinGrouper();
        List<Set<MixinDefinition>> mixinGroups = new List<Set<MixinDefinition>> (grouper.GroupMixins (bt7.Mixins));
        Assert.AreEqual (3, mixinGroups.Count);

        mixinGroups.Sort (delegate (Set<MixinDefinition> one, Set<MixinDefinition> two) { return one.Count.CompareTo (two.Count); });

        Set<MixinDefinition> smaller = mixinGroups[0];
        Set<MixinDefinition> medium = mixinGroups[1];
        Set<MixinDefinition> larger = mixinGroups[2];

        Assert.Contains (bt7.Mixins[typeof (BT7Mixin0)], larger, "dependency+method");
        Assert.Contains (bt7.Mixins[typeof (BT7Mixin1)], larger, "dependency+method");
        Assert.Contains (bt7.Mixins[typeof (BT7Mixin2)], larger, "dependency+method");
        Assert.Contains (bt7.Mixins[typeof (BT7Mixin3)], larger, "dependency+method");
        Assert.Contains (bt7.Mixins[typeof (BT7Mixin4)], larger, "method");
        Assert.Contains (bt7.Mixins[typeof (BT7Mixin6)], larger, "method");
        Assert.Contains (bt7.Mixins[typeof (BT7Mixin7)], larger, "dependency");
        Assert.AreEqual (7, larger.Count);

        Assert.Contains (bt7.Mixins[typeof (BT7Mixin8)], medium, "method");
        Assert.Contains (bt7.Mixins[typeof (BT7Mixin9)], medium, "dependency+method");
        Assert.Contains (bt7.Mixins[typeof (BT7Mixin10)], medium, "dependency");
        Assert.AreEqual (3, medium.Count);

        Assert.Contains (bt7.Mixins[typeof (BT7Mixin5)], smaller, "nothing");
        Assert.AreEqual (1, smaller.Count);
      }
    }

    [Test]
    public void SortingWithExplicitDependencies ()
    {
      ClassContext context = new ClassContextBuilder (new MixinConfigurationBuilder (null), typeof (TargetClassWithAdditionalDependencies), null)
          .AddMixin<MixinWithAdditionalClassDependency> ().WithDependency<MixinWithNoAdditionalDependency>()
          .AddMixin<MixinWithNoAdditionalDependency>()
          .AddMixin<MixinWithAdditionalInterfaceDependency> ().WithDependency<IMixinWithAdditionalClassDependency> ()
          .BuildClassContext(new ClassContext[0]);
      CheckExplicitDependencyOrdering (context);

      context = new ClassContextBuilder (new MixinConfigurationBuilder (null), typeof (TargetClassWithAdditionalDependencies), null)
          .AddMixin<MixinWithNoAdditionalDependency> ()
          .AddMixin<MixinWithAdditionalClassDependency> ().WithDependency<MixinWithNoAdditionalDependency> ()
          .AddMixin<MixinWithAdditionalInterfaceDependency> ().WithDependency<IMixinWithAdditionalClassDependency> ()
          .BuildClassContext (new ClassContext[0]);
      CheckExplicitDependencyOrdering (context);

      context = new ClassContextBuilder (new MixinConfigurationBuilder (null), typeof (TargetClassWithAdditionalDependencies), null)
          .AddMixin<MixinWithNoAdditionalDependency> ()
          .AddMixin<MixinWithAdditionalInterfaceDependency> ().WithDependency<IMixinWithAdditionalClassDependency> ()
          .AddMixin<MixinWithAdditionalClassDependency> ().WithDependency<MixinWithNoAdditionalDependency> ()
          .BuildClassContext (new ClassContext[0]);
      CheckExplicitDependencyOrdering (context);
    }

    private void CheckExplicitDependencyOrdering (ClassContext classContext)
    {
      MixinConfiguration configuration = new MixinConfiguration (null);
      configuration.ClassContexts.Add (classContext);

      using (configuration.EnterScope())
      {
        TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (TargetClassWithAdditionalDependencies));
        Assert.AreEqual (0, targetClass.Mixins[typeof (MixinWithAdditionalInterfaceDependency)].MixinIndex);
        Assert.AreEqual (1, targetClass.Mixins[typeof (MixinWithAdditionalClassDependency)].MixinIndex);
        Assert.AreEqual (2, targetClass.Mixins[typeof (MixinWithNoAdditionalDependency)].MixinIndex);
      }
    }

    [Test]
    public void AlphabeticOrdering ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassWithMixinsAcceptingAlphabeticOrdering));
      Assert.AreEqual (typeof (MixinAcceptingAlphabeticOrdering1), targetClass.Mixins[0].Type);
      Assert.AreEqual (typeof (MixinAcceptingAlphabeticOrdering2), targetClass.Mixins[1].Type);
    }
  }
}
