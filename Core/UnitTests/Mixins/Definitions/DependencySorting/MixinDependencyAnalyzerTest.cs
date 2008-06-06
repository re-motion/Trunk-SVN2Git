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
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Definitions.Building.DependencySorting;
using Remotion.Mixins.Utilities.DependencySort;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Definitions.DependencySorting
{
  [TestFixture]
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
      using (MixinConfiguration.BuildNew().ForClass (typeof (NullTarget))
          .AddMixin (typeof (NullMixin))
          .AddMixin (typeof (NullMixin2))
          .AddMixin (typeof (NullMixin3)).WithDependency (typeof (NullMixin2))
          .AddMixin (typeof (NullMixin4)).WithDependency (typeof (NullMixin3))
          .AddMixin (typeof (MixinAcceptingAlphabeticOrdering1))
          .AddMixin (typeof (MixinAcceptingAlphabeticOrdering2))
          .EnterScope())
      {
        _independent1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (NullMixin)];
        _independent2 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (NullMixin2)];
        _dependentSecond = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (NullMixin3)];
        _dependentThird = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (NullMixin4)];
        _alphabeticAccepter = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (MixinAcceptingAlphabeticOrdering1)];
        _alphabeticAccepter2 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (MixinAcceptingAlphabeticOrdering2)];
      }
    }

    [Test]
    public void AnalyzeDirectDependency_Independent ()
    {
      Assert.AreEqual (DependencyKind.None, _analyzer.AnalyzeDirectDependency (_independent1, _independent1));
    }

    [Test]
    public void AnalyzeDirectDependency_DirectDependent_SecondOnFirst ()
    {
      Assert.AreEqual (DependencyKind.SecondOnFirst, _analyzer.AnalyzeDirectDependency (_independent2, _dependentSecond));
      Assert.AreEqual (DependencyKind.SecondOnFirst, _analyzer.AnalyzeDirectDependency (_dependentSecond, _dependentThird));
    }

    [Test]
    public void AnalyzeDirectDependency_DirectDependent_FirstOnSecond ()
    {
      Assert.AreEqual (DependencyKind.FirstOnSecond, _analyzer.AnalyzeDirectDependency (_dependentSecond, _independent2));
      Assert.AreEqual (DependencyKind.FirstOnSecond, _analyzer.AnalyzeDirectDependency (_dependentThird, _dependentSecond));
    }

    [Test]
    public void AnalyzeDirectDependency_IndirectDependent ()
    {
      Assert.AreEqual (DependencyKind.None, _analyzer.AnalyzeDirectDependency (_independent2, _dependentThird));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The following mixins are applied to the same base class "
        + "Remotion.UnitTests.Mixins.SampleTypes.NullTarget and require a clear base call ordering, but do not provide enough dependency information: "
        + "Remotion.UnitTests.Mixins.SampleTypes.NullMixin, Remotion.UnitTests.Mixins.SampleTypes.NullMixin2, "
        + "Remotion.UnitTests.Mixins.SampleTypes.NullMixin4.\r\nPlease supply additional dependencies to the mixin definitions, use the "
        + "AcceptsAlphabeticOrderingAttribute, or adjust the mixin configuration accordingly.")]
    public void ResolveEqualRoots_Throws ()
    {
      _analyzer.ResolveEqualRoots (new MixinDefinition[] { _independent1, _independent2, _dependentThird });
    }

    [Test]
    public void ResolveEqualRoots_WithEnabledAlphabeticOrdering ()
    {
      Assert.AreSame (_alphabeticAccepter, _analyzer.ResolveEqualRoots (new MixinDefinition[] { _independent1, _alphabeticAccepter }));
    }

    [Test]
    public void ResolveEqualRoots_WithEnabledAlphabeticOrdering_TwoAccepters ()
    {
      Assert.AreSame (_alphabeticAccepter, 
          _analyzer.ResolveEqualRoots (new MixinDefinition[] {_independent1, _alphabeticAccepter, _alphabeticAccepter2}));
    }

    [Test]
    public void ResolveEqualRoots_WithEnabledAlphabeticOrdering_TwoAccepters_OtherOrder ()
    {
      Assert.AreSame (_alphabeticAccepter,
          _analyzer.ResolveEqualRoots (new MixinDefinition[] { _independent1, _alphabeticAccepter2, _alphabeticAccepter }));
    }

    [Test]
    public void SortAlphabetically ()
    {
      List<Tuple<string, MixinDefinition>> mixinsByTypeName = new List<Tuple<string, MixinDefinition>>();
      Tuple<string, MixinDefinition> zebra = Tuple.NewTuple ("Zebra", _independent1);
      Tuple<string, MixinDefinition> bravo = Tuple.NewTuple ("Bravo", _independent1);
      Tuple<string, MixinDefinition> charlie = Tuple.NewTuple ("Charlie", _independent1);
      Tuple<string, MixinDefinition> alpha = Tuple.NewTuple ("Alpha", _independent1);
      Tuple<string, MixinDefinition> delta = Tuple.NewTuple ("Delta", _independent1);

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

        List<Tuple<string, MixinDefinition>> mixinsByTypeName = new List<Tuple<string, MixinDefinition>> ();
        Tuple<string, MixinDefinition> a = Tuple.NewTuple ("A", _independent1);
        Tuple<string, MixinDefinition> ae = Tuple.NewTuple ("Ä", _independent1);
        Tuple<string, MixinDefinition> b = Tuple.NewTuple ("B", _independent1);

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
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The following mixins are applied to the same base class "
        + "Remotion.UnitTests.Mixins.SampleTypes.NullTarget and require a clear base call ordering, but do not provide enough dependency information: "
        + "Remotion.UnitTests.Mixins.SampleTypes.NullMixin, Remotion.UnitTests.Mixins.SampleTypes.NullMixin2, Remotion.UnitTests.Mixins.SampleTypes."
       + "MixinAcceptingAlphabeticOrdering1.\r\nPlease supply additional dependencies to the mixin definitions, use the "
       + "AcceptsAlphabeticOrderingAttribute, or adjust the mixin configuration accordingly.")]
    public void ResolveEqualRoots_WithEnabledAlphabeticOrdering_TwoNonAccepters ()
    {
      _analyzer.ResolveEqualRoots (new MixinDefinition[] { _independent1, _independent2, _alphabeticAccepter });
      Assert.Fail();
    }
  }
}
