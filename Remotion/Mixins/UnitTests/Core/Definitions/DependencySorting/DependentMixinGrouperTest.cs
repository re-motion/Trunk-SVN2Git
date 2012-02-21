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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Definitions.Building.DependencySorting;
using Remotion.Mixins.UnitTests.Core.Definitions.DependencySorting.TestDomain;
using System.Linq;

namespace Remotion.Mixins.UnitTests.Core.Definitions.DependencySorting
{
  [TestFixture]
  public class DependentMixinGrouperTest
  {
    private MixinDefinition _independent1;
    private MixinDefinition _independent2;

    private MixinDefinition _overrideM1;
    private MixinDefinition _overrideM2;
    private MixinDefinition _overrideM1M2;

    private MixinDefinition _nextCallDependency0;
    private MixinDefinition _nextCallDependency1;

    private MixinDefinition _nextCallDependency2OverrideM1;

    private MixinDefinition _additionalDependency0;
    private MixinDefinition _additionalDependency1;

    private DependentMixinGrouper _grouper;

    [SetUp]
    public void SetUp ()
    {
      var classContext = MixinConfiguration.BuildNew ()
          .ForClass (typeof (TargetClass))
          .AddMixin (typeof (IndependentMixin1))
          .AddMixin (typeof (IndependentMixin2))
          .AddMixin (typeof (MixinOverridingM1))
          .AddMixin (typeof (MixinOverridingM2))
          .AddMixin (typeof (MixinOverridingM1M2))
          .AddMixin (typeof (MixinImplementingNextCallDependency1))
          .AddMixin (typeof (MixinWithNextCallDependency1))
          .AddMixin (typeof (MixinWithNextCallDependency2OverridingM1))
          .AddMixin (typeof (MixinImplementingAdditionalDependency))
          .AddMixin (typeof (MixinWithAdditionalDependency)).WithDependency (typeof (MixinImplementingAdditionalDependency))
          .BuildClassContext();

      var targetClassDefinition = DefinitionObjectMother.GetTargetClassDefinition (classContext);
      _independent1 = targetClassDefinition.Mixins[typeof (IndependentMixin1)];
      _independent2 = targetClassDefinition.Mixins[typeof (IndependentMixin2)];

      _overrideM1 = targetClassDefinition.Mixins[typeof (MixinOverridingM1)];
      _overrideM2 = targetClassDefinition.Mixins[typeof (MixinOverridingM2)];
      _overrideM1M2 = targetClassDefinition.Mixins[typeof (MixinOverridingM1M2)];

      _nextCallDependency0 = targetClassDefinition.Mixins[typeof (MixinImplementingNextCallDependency1)];
      _nextCallDependency1 = targetClassDefinition.Mixins[typeof (MixinWithNextCallDependency1)];
      _nextCallDependency2OverrideM1 = targetClassDefinition.Mixins[typeof (MixinWithNextCallDependency2OverridingM1)];
      _additionalDependency0 = targetClassDefinition.Mixins[typeof (MixinImplementingAdditionalDependency)];
      _additionalDependency1 = targetClassDefinition.Mixins[typeof (MixinWithAdditionalDependency)];

      _grouper = new DependentMixinGrouper();
    }

    [Test]
    public void UnrelatedMixins ()
    {
      var groups = GetGroups (_independent1, _independent2, _overrideM1);
      Assert.AreEqual (3, groups.Length);
      Assert.That (groups[0].ToArray (), Is.EquivalentTo (new object[] { _independent1 }));
      Assert.That (groups[1].ToArray (), Is.EquivalentTo (new object[] { _independent2 }));
      Assert.That (groups[2].ToArray (), Is.EquivalentTo (new object[] { _overrideM1 }));
    }

    [Test]
    public void Overrides_NoCut ()
    {
      var groups = GetGroups (_overrideM1, _overrideM2);
      Assert.AreEqual (2, groups.Length);
      Assert.That (groups[0].ToArray (), Is.EquivalentTo (new object[] { _overrideM1 }));
      Assert.That (groups[1].ToArray (), Is.EquivalentTo (new object[] { _overrideM2 }));
    }

    [Test]
    public void Overrides_Cut ()
    {
      var groups = GetGroups (_overrideM1, _overrideM1M2);
      Assert.AreEqual (1, groups.Length);
      Assert.That (groups[0].ToArray (), Is.EquivalentTo (new object[] { _overrideM1, _overrideM1M2 }));
    }

    [Test]
    public void Overrides_TransitiveCut ()
    {
      var groups = GetGroups (_overrideM1, _overrideM2, _overrideM1M2);
      Assert.AreEqual (1, groups.Length);
      Assert.That (groups[0].ToArray (), Is.EquivalentTo (new object[] { _overrideM1, _overrideM2, _overrideM1M2 }));
    }

    [Test]
    public void NextCallDependency_NoCut ()
    {
      var groups = GetGroups (_nextCallDependency1, _independent1);
      Assert.AreEqual (2, groups.Length);
      Assert.That (groups[0].ToArray (), Is.EquivalentTo (new object[] { _nextCallDependency1 }));
      Assert.That (groups[1].ToArray (), Is.EquivalentTo (new object[] { _independent1 }));
    }

    [Test]
    public void NextCallDependency_Cut ()
    {
      var groups = GetGroups (_nextCallDependency0, _nextCallDependency1);
      Assert.AreEqual (1, groups.Length);
      Assert.That (groups[0].ToArray(), Is.EquivalentTo (new object[] { _nextCallDependency0, _nextCallDependency1 }));
    }

    [Test]
    public void NextCallDependency_TransitiveCut ()
    {
      var groups = GetGroups (_nextCallDependency0, _nextCallDependency1, _nextCallDependency2OverrideM1);
      Assert.AreEqual (1, groups.Length);
      Assert.That (groups[0].ToArray (), Is.EquivalentTo (new object[] { _nextCallDependency0, _nextCallDependency1, _nextCallDependency2OverrideM1 }));
    }

    [Test]
    public void NextCallDependency_TransitiveCutAndOverride ()
    {
      var groups = GetGroups (_overrideM2, _overrideM1M2, _nextCallDependency1, _nextCallDependency2OverrideM1);
      Assert.AreEqual (1, groups.Length);
      Assert.That (groups[0].ToArray (), Is.EquivalentTo (new object[] { _overrideM2, _overrideM1M2, _nextCallDependency1, _nextCallDependency2OverrideM1 }));
    }

    [Test]
    public void ExplicitDependency_NoCut ()
    {
      var groups = GetGroups (_additionalDependency1, _independent1);
      Assert.AreEqual (2, groups.Length);
      Assert.That (groups[0].ToArray (), Is.EquivalentTo (new object[] { _additionalDependency1 }));
      Assert.That (groups[1].ToArray (), Is.EquivalentTo (new object[] { _independent1 }));
    }

    [Test]
    public void ExplicitDependency_Cut ()
    {
      var groups = GetGroups (_additionalDependency0, _additionalDependency1);
      Assert.AreEqual (1, groups.Length);
      Assert.That (groups[0].ToArray (), Is.EquivalentTo (new object[] { _additionalDependency0, _additionalDependency1 }));
    }

    [Test]
    public void ExplicitDependency_Cut_OtherDirection ()
    {
      var groups = GetGroups (_additionalDependency1, _additionalDependency0);
      Assert.AreEqual (1, groups.Length);
      Assert.That (groups[0].ToArray (), Is.EquivalentTo (new object[] { _additionalDependency0, _additionalDependency1 }));
    }

    private HashSet<MixinDefinition>[] GetGroups (params MixinDefinition[] mixins)
    {
      return _grouper.GroupMixins (mixins).ToArray ();
    }
  }
}
