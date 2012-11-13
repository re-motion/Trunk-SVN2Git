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
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Definitions.Building.DependencySorting;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Mixins.Utilities.DependencySort;
using Remotion.ServiceLocation;

namespace Remotion.Mixins.UnitTests.Core.Definitions.DependencySorting
{
  [TestFixture]
  [Obsolete ("This class will been removed. Use MixinDefinitionSorter instead. (1.13.175.0)")]
  public class MixinDependencyAnalyzerTest
  {
    private MixinDependencyAnalyzer _analyzer;
    private MixinDefinition _independent1;
    private MixinDefinition _independent2;
    private MixinDefinition _dependentSecond;
    private MixinDefinition _dependentThird;
    private MixinDefinition _alphabeticAccepter;
    private MixinDefinition _alphabeticAccepter2;

    [SetUp]
    public void SetUp ()
    {
      _analyzer = new MixinDependencyAnalyzer ();
      var classContext = MixinConfiguration.BuildNew ()
          .ForClass (typeof (NullTarget))
          .AddMixin (typeof (NullMixin))
          .AddMixin (typeof (NullMixin2))
          .AddMixin (typeof (NullMixin3)).WithDependency (typeof (NullMixin2))
          .AddMixin (typeof (NullMixin4)).WithDependency (typeof (NullMixin3))
          .AddMixin (typeof (MixinAcceptingAlphabeticOrdering1))
          .AddMixin (typeof (MixinAcceptingAlphabeticOrdering2))
          .BuildClassContext();

      var targetClassDefinition = DefinitionObjectMother.GetTargetClassDefinition (classContext);
      _independent1 = targetClassDefinition.Mixins[typeof (NullMixin)];
      _independent2 = targetClassDefinition.Mixins[typeof (NullMixin2)];
      _dependentSecond = targetClassDefinition.Mixins[typeof (NullMixin3)];
      _dependentThird = targetClassDefinition.Mixins[typeof (NullMixin4)];
      _alphabeticAccepter = targetClassDefinition.Mixins[typeof (MixinAcceptingAlphabeticOrdering1)];
      _alphabeticAccepter2 = targetClassDefinition.Mixins[typeof (MixinAcceptingAlphabeticOrdering2)];
    }

    [Test]
    public void Singleton_RegisteredAsDefaultInterfaceImplementation ()
    {
      var instance = SafeServiceLocator.Current.GetInstance<IMixinDependencyAnalyzer> ();
      Assert.That (instance, Is.TypeOf<MixinDependencyAnalyzer> ());

      Assert.That (instance, Is.SameAs (SafeServiceLocator.Current.GetInstance<IMixinDependencyAnalyzer> ()));
    }

    [Test]
    public void AnalyzeDirectDependency_Independent ()
    {
      Assert.That (_analyzer.AnalyzeDirectDependency (_independent1, _independent1), Is.EqualTo (DependencyKind.None));
    }

    [Test]
    public void AnalyzeDirectDependency_DirectDependent_SecondOnFirst ()
    {
      Assert.That (_analyzer.AnalyzeDirectDependency (_independent2, _dependentSecond), Is.EqualTo (DependencyKind.SecondOnFirst));
      Assert.That (_analyzer.AnalyzeDirectDependency (_dependentSecond, _dependentThird), Is.EqualTo (DependencyKind.SecondOnFirst));
    }

    [Test]
    public void AnalyzeDirectDependency_DirectDependent_FirstOnSecond ()
    {
      Assert.That (_analyzer.AnalyzeDirectDependency (_dependentSecond, _independent2), Is.EqualTo (DependencyKind.FirstOnSecond));
      Assert.That (_analyzer.AnalyzeDirectDependency (_dependentThird, _dependentSecond), Is.EqualTo (DependencyKind.FirstOnSecond));
    }

    [Test]
    public void AnalyzeDirectDependency_IndirectDependent ()
    {
      Assert.That (_analyzer.AnalyzeDirectDependency (_independent2, _dependentThird), Is.EqualTo (DependencyKind.None));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "The following mixins require a clear base call ordering, but do not provide enough dependency information:\r\n"
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.NullMixin',\r\n"
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.NullMixin2',\r\n"
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.NullMixin4'.\r\n"
        + "Please supply additional dependencies to the mixin definitions, use the "
        + "AcceptsAlphabeticOrderingAttribute, or adjust the mixin configuration accordingly.")]
    public void ResolveEqualRoots_Throws ()
    {
      _analyzer.ResolveEqualRoots (new[] { _independent1, _independent2, _dependentThird });
    }

    [Test]
    public void ResolveEqualRoots_WithEnabledAlphabeticOrdering ()
    {
      Assert.That (_analyzer.ResolveEqualRoots (new[] { _independent1, _alphabeticAccepter }), Is.SameAs (_alphabeticAccepter));
    }

    [Test]
    public void ResolveEqualRoots_WithEnabledAlphabeticOrdering_TwoAccepters ()
    {
      Assert.That (
                  _analyzer.ResolveEqualRoots (new[] { _independent1, _alphabeticAccepter, _alphabeticAccepter2 }), Is.SameAs (_alphabeticAccepter));
    }

    [Test]
    public void ResolveEqualRoots_WithEnabledAlphabeticOrdering_TwoAccepters_OtherOrder ()
    {
      Assert.That (
                  _analyzer.ResolveEqualRoots (new[] { _independent1, _alphabeticAccepter2, _alphabeticAccepter }), Is.SameAs (_alphabeticAccepter));
    }

    [Test]
    public void SortAlphabetically ()
    {
      var mixinsByTypeName = new List<Tuple<string, MixinDefinition>>();
      Tuple<string, MixinDefinition> zebra = Tuple.Create ("Zebra", _independent1);
      Tuple<string, MixinDefinition> bravo = Tuple.Create ("Bravo", _independent1);
      Tuple<string, MixinDefinition> charlie = Tuple.Create ("Charlie", _independent1);
      Tuple<string, MixinDefinition> alpha = Tuple.Create ("Alpha", _independent1);
      Tuple<string, MixinDefinition> delta = Tuple.Create ("Delta", _independent1);

      mixinsByTypeName.Add (zebra);
      mixinsByTypeName.Add (bravo);
      mixinsByTypeName.Add (charlie);
      mixinsByTypeName.Add (alpha);
      mixinsByTypeName.Add (delta);

      _analyzer.SortAlphabetically (mixinsByTypeName);

      Assert.That (mixinsByTypeName, Is.EqualTo (new object[] { alpha, bravo, charlie, delta, zebra }));
    }

    [Test]
    public void SortAlphabetically_UsesOrdinalComparison ()
    {
      CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
      try
      {
        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo ("de-de");

        var mixinsByTypeName = new List<Tuple<string, MixinDefinition>> ();
        Tuple<string, MixinDefinition> a = Tuple.Create ("A", _independent1);
        Tuple<string, MixinDefinition> ae = Tuple.Create ("Ä", _independent1);
        Tuple<string, MixinDefinition> b = Tuple.Create ("B", _independent1);

        mixinsByTypeName.Add (a);
        mixinsByTypeName.Add (ae);
        mixinsByTypeName.Add (b);

        _analyzer.SortAlphabetically (mixinsByTypeName);

        Assert.That (mixinsByTypeName, Is.EqualTo (new object[] { a, b, ae }));
      }
      finally
      {
        Thread.CurrentThread.CurrentCulture = originalCulture;
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "The following mixins require a clear base call ordering, but do not provide enough dependency information:\r\n"
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.NullMixin',\r\n"
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.NullMixin2',\r\n"
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.MixinAcceptingAlphabeticOrdering1'.\r\n"
        + "Please supply additional dependencies to the mixin definitions, use the "
       + "AcceptsAlphabeticOrderingAttribute, or adjust the mixin configuration accordingly.")]
    public void ResolveEqualRoots_WithEnabledAlphabeticOrdering_TwoNonAccepters ()
    {
      _analyzer.ResolveEqualRoots (new[] { _independent1, _independent2, _alphabeticAccepter });
      Assert.Fail();
    }

    [Test]
    public void BigTestDomain ()
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
        var analyzer = new MixinDependencyAnalyzer ();

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
  }
}
