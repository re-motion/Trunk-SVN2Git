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
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.ServiceLocation;

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
    public void Singleton_RegisteredAsDefaultInterfaceImplementation ()
    {
      var instance = SafeServiceLocator.Current.GetInstance<IDependentMixinGrouper>();
      Assert.That (instance, Is.TypeOf<DependentMixinGrouper>());

      Assert.That (instance, Is.SameAs (SafeServiceLocator.Current.GetInstance<IDependentMixinGrouper>()));
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
        var mixinGroups = GetGroups (bt7.Mixins.ToArray()).ToList();
        Assert.That (mixinGroups.Count, Is.EqualTo (3));

        mixinGroups.Sort ((one, two) => one.Count.CompareTo (two.Count));

        var smaller = mixinGroups[0];
        var medium = mixinGroups[1];
        var larger = mixinGroups[2];

        Assert.Contains (bt7.Mixins[typeof (BT7Mixin0)], larger.ToArray (), "dependency+method");
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

    private HashSet<MixinDefinition>[] GetGroups (params MixinDefinition[] mixins)
    {
      return _grouper.GroupMixins (mixins).ToArray ();
    }
  }
}
