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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Definitions.Building.DependencySorting;
using Remotion.Mixins.Utilities.DependencySort;
using Remotion.UnitTests.Mixins.SampleTypes;
using System.Linq;

namespace Remotion.UnitTests.Mixins.Definitions
{
  [TestFixture]
  public class MixinDependencySortIntegrationTest
  {
    [Test]
    public void MixinDefinitionsAreSortedCorrectlySmall()
    {
      TargetClassDefinition bt7 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType7));
      Assert.That (bt7.Mixins.Count, Is.EqualTo (7));
      // group 1
      Assert.That (bt7.Mixins[typeof (BT7Mixin0)].MixinIndex, Is.EqualTo (0));

      Assert.That (bt7.Mixins[typeof (BT7Mixin2)].MixinIndex, Is.EqualTo (1));
      Assert.That (bt7.Mixins[typeof (BT7Mixin3)].MixinIndex, Is.EqualTo (2));
      Assert.That (bt7.Mixins[typeof (BT7Mixin1)].MixinIndex, Is.EqualTo (3));

      // group 2
      Assert.That (bt7.Mixins[typeof (BT7Mixin10)].MixinIndex, Is.EqualTo (4));
      Assert.That (bt7.Mixins[typeof (BT7Mixin9)].MixinIndex, Is.EqualTo (5));

      // group 3
      Assert.That (bt7.Mixins[typeof (BT7Mixin5)].MixinIndex, Is.EqualTo (6));
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
        Assert.That (bt7.Mixins.Count, Is.EqualTo (11));
        // group 1
        Assert.That (bt7.Mixins[typeof (BT7Mixin0)].MixinIndex, Is.EqualTo (0)); // u
        Assert.That (bt7.Mixins[typeof (BT7Mixin7)].MixinIndex, Is.EqualTo (1)); // u
        Assert.That (bt7.Mixins[typeof (BT7Mixin4)].MixinIndex, Is.EqualTo (2)); // u
        Assert.That (bt7.Mixins[typeof (BT7Mixin6)].MixinIndex, Is.EqualTo (3)); // u

        Assert.That (bt7.Mixins[typeof (BT7Mixin2)].MixinIndex, Is.EqualTo (4));
        Assert.That (bt7.Mixins[typeof (BT7Mixin3)].MixinIndex, Is.EqualTo (5));
        Assert.That (bt7.Mixins[typeof (BT7Mixin1)].MixinIndex, Is.EqualTo (6));

        // group 2
        Assert.That (bt7.Mixins[typeof (BT7Mixin10)].MixinIndex, Is.EqualTo (7));
        Assert.That (bt7.Mixins[typeof (BT7Mixin9)].MixinIndex, Is.EqualTo (8)); // u
        Assert.That (bt7.Mixins[typeof (BT7Mixin8)].MixinIndex, Is.EqualTo (9)); // u

        // group 3
        Assert.That (bt7.Mixins[typeof (BT7Mixin5)].MixinIndex, Is.EqualTo (10));
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
        var analyzer = new MixinDependencyAnalyzer();

        Assert.That (analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin1)], bt7.Mixins[typeof (BT7Mixin0)]), Is.EqualTo (DependencyKind.None));
        Assert.That (analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin0)], bt7.Mixins[typeof (BT7Mixin1)]), Is.EqualTo (DependencyKind.None));

        Assert.That (analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin1)], bt7.Mixins[typeof (BT7Mixin2)]), Is.EqualTo (DependencyKind.None));
        Assert.That (analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin2)], bt7.Mixins[typeof (BT7Mixin1)]), Is.EqualTo (DependencyKind.None));

        Assert.That (analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin4)], bt7.Mixins[typeof (BT7Mixin0)]), Is.EqualTo (DependencyKind.None));
        Assert.That (analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin4)], bt7.Mixins[typeof (BT7Mixin1)]), Is.EqualTo (DependencyKind.None));
        Assert.That (analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin4)], bt7.Mixins[typeof (BT7Mixin2)]), Is.EqualTo (DependencyKind.None));
        Assert.That (analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin4)], bt7.Mixins[typeof (BT7Mixin3)]), Is.EqualTo (DependencyKind.None));

        Assert.That (analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin0)], bt7.Mixins[typeof (BT7Mixin2)]), Is.EqualTo (
                      DependencyKind.FirstOnSecond));

        Assert.That (analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin1)], bt7.Mixins[typeof (BT7Mixin3)]), Is.EqualTo (
                      DependencyKind.SecondOnFirst));
        Assert.That (analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin3)], bt7.Mixins[typeof (BT7Mixin1)]), Is.EqualTo (
                      DependencyKind.FirstOnSecond));

        Assert.That (analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin2)], bt7.Mixins[typeof (BT7Mixin3)]), Is.EqualTo (
                      DependencyKind.FirstOnSecond));
        Assert.That (analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin3)], bt7.Mixins[typeof (BT7Mixin2)]), Is.EqualTo (
                      DependencyKind.SecondOnFirst));

        Assert.That (analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin3)], bt7.Mixins[typeof (BT7Mixin2)]), Is.EqualTo (
                      DependencyKind.SecondOnFirst));

        Assert.That (analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin7)], bt7.Mixins[typeof (BT7Mixin2)]), Is.EqualTo (
                      DependencyKind.FirstOnSecond));
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
        var grouper = new DependentMixinGrouper();
        var mixinGroups = new List<HashSet<MixinDefinition>> (grouper.GroupMixins (bt7.Mixins));
        Assert.That (mixinGroups.Count, Is.EqualTo (3));

        mixinGroups.Sort ((one, two) => one.Count.CompareTo (two.Count));

        var smaller = mixinGroups[0];
        var medium = mixinGroups[1];
        var larger = mixinGroups[2];

        Assert.Contains (bt7.Mixins[typeof (BT7Mixin0)], larger.ToArray(), "dependency+method");
        Assert.Contains (bt7.Mixins[typeof (BT7Mixin1)], larger.ToArray (), "dependency+method");
        Assert.Contains (bt7.Mixins[typeof (BT7Mixin2)], larger.ToArray (), "dependency+method");
        Assert.Contains (bt7.Mixins[typeof (BT7Mixin3)], larger.ToArray (), "dependency+method");
        Assert.Contains (bt7.Mixins[typeof (BT7Mixin4)], larger.ToArray (), "method");
        Assert.Contains (bt7.Mixins[typeof (BT7Mixin6)], larger.ToArray (), "method");
        Assert.Contains (bt7.Mixins[typeof (BT7Mixin7)], larger.ToArray (), "dependency");
        Assert.That (larger.Count, Is.EqualTo (7));

        Assert.Contains (bt7.Mixins[typeof (BT7Mixin8)], medium.ToArray (), "method");
        Assert.Contains (bt7.Mixins[typeof (BT7Mixin9)], medium.ToArray (), "dependency+method");
        Assert.Contains (bt7.Mixins[typeof (BT7Mixin10)], medium.ToArray (), "dependency");
        Assert.That (medium.Count, Is.EqualTo (3));

        Assert.Contains (bt7.Mixins[typeof (BT7Mixin5)], smaller.ToArray (), "nothing");
        Assert.That (smaller.Count, Is.EqualTo (1));
      }
    }

    [Test]
    public void SortingWithExplicitDependencies ()
    {
      ClassContext context = new ClassContextBuilder (typeof (TargetClassWithAdditionalDependencies))
          .AddMixin<MixinWithAdditionalClassDependency> ().WithDependency<MixinWithNoAdditionalDependency>()
          .AddMixin<MixinWithNoAdditionalDependency>()
          .AddMixin<MixinWithAdditionalInterfaceDependency> ().WithDependency<IMixinWithAdditionalClassDependency> ()
          .BuildClassContext(new ClassContext[0]);
      CheckExplicitDependencyOrdering (context);

      context = new ClassContextBuilder (typeof (TargetClassWithAdditionalDependencies))
          .AddMixin<MixinWithNoAdditionalDependency> ()
          .AddMixin<MixinWithAdditionalClassDependency> ().WithDependency<MixinWithNoAdditionalDependency> ()
          .AddMixin<MixinWithAdditionalInterfaceDependency> ().WithDependency<IMixinWithAdditionalClassDependency> ()
          .BuildClassContext (new ClassContext[0]);
      CheckExplicitDependencyOrdering (context);

      context = new ClassContextBuilder (typeof (TargetClassWithAdditionalDependencies))
          .AddMixin<MixinWithNoAdditionalDependency> ()
          .AddMixin<MixinWithAdditionalInterfaceDependency> ().WithDependency<IMixinWithAdditionalClassDependency> ()
          .AddMixin<MixinWithAdditionalClassDependency> ().WithDependency<MixinWithNoAdditionalDependency> ()
          .BuildClassContext (new ClassContext[0]);
      CheckExplicitDependencyOrdering (context);
    }

    [Test]
    public void AlphabeticOrdering ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassWithMixinsAcceptingAlphabeticOrdering));
      Assert.That (targetClass.Mixins[0].Type, Is.EqualTo (typeof (MixinAcceptingAlphabeticOrdering1)));
      Assert.That (targetClass.Mixins[1].Type, Is.EqualTo (typeof (MixinAcceptingAlphabeticOrdering2)));
    }

    private void CheckExplicitDependencyOrdering (ClassContext classContext)
    {
      TargetClassDefinition targetClass = TargetClassDefinitionFactory.CreateTargetClassDefinition (classContext);
      Assert.That (targetClass.Mixins[typeof (MixinWithAdditionalInterfaceDependency)].MixinIndex, Is.EqualTo (0));
      Assert.That (targetClass.Mixins[typeof (MixinWithAdditionalClassDependency)].MixinIndex, Is.EqualTo (1));
      Assert.That (targetClass.Mixins[typeof (MixinWithNoAdditionalDependency)].MixinIndex, Is.EqualTo (2));
    }
  }
}
