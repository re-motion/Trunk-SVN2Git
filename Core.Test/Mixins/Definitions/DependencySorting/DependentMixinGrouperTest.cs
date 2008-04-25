using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.UnitTests.Mixins.Definitions.DependencySorting.SampleTypes;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Definitions.Building.DependencySorting;
using Remotion.Utilities;

namespace Remotion.UnitTests.Mixins.Definitions.DependencySorting
{
  [TestFixture]
  public class DependentMixinGrouperTest
  {
    private MixinDefinition _independent1;
    private MixinDefinition _independent2;

    private MixinDefinition _overrideM1;
    private MixinDefinition _overrideM2;
    private MixinDefinition _overrideM1M2;

    private MixinDefinition _baseCallDependency0;
    private MixinDefinition _baseCallDependency1;

    private MixinDefinition _baseCallDependency2OverrideM1;

    private MixinDefinition _additionalDependency0;
    private MixinDefinition _additionalDependency1;

    private DependentMixinGrouper _grouper;

    [SetUp]
    public void SetUp ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass (typeof (TargetClass)).AddMixin (typeof (IndependentMixin1))
          .ForClass (typeof (TargetClass)).AddMixin (typeof (IndependentMixin2))
          .ForClass (typeof (TargetClass)).AddMixin (typeof (MixinOverridingM1))
          .ForClass (typeof (TargetClass)).AddMixin (typeof (MixinOverridingM2))
          .ForClass (typeof (TargetClass)).AddMixin (typeof (MixinOverridingM1M2))
          .ForClass (typeof (TargetClass)).AddMixin (typeof (MixinImplementingBaseCallDependency1))
          .ForClass (typeof (TargetClass)).AddMixin (typeof (MixinWithBaseCallDependency1))
          .ForClass (typeof (TargetClass)).AddMixin (typeof (MixinWithBaseCallDependency2OverridingM1))
          .ForClass (typeof (TargetClass)).AddMixin (typeof (MixinImplementingAdditionalDependency))
          .ForClass (typeof (TargetClass)).AddMixin (typeof (MixinWithAdditionalDependency)).WithDependency (typeof (MixinImplementingAdditionalDependency))
          .EnterScope ())
      {
        _independent1 = GetMixin(typeof (IndependentMixin1));
        _independent2 = GetMixin (typeof (IndependentMixin2));

        _overrideM1 = GetMixin (typeof (MixinOverridingM1));
        _overrideM2 = GetMixin (typeof (MixinOverridingM2));
        _overrideM1M2 = GetMixin (typeof (MixinOverridingM1M2));

        _baseCallDependency0 = GetMixin (typeof (MixinImplementingBaseCallDependency1));
        _baseCallDependency1 = GetMixin (typeof (MixinWithBaseCallDependency1));
        _baseCallDependency2OverrideM1 = GetMixin (typeof (MixinWithBaseCallDependency2OverridingM1));
        _additionalDependency0 = GetMixin (typeof (MixinImplementingAdditionalDependency));
        _additionalDependency1 = GetMixin (typeof (MixinWithAdditionalDependency));
      }

      _grouper = new DependentMixinGrouper();
    }

    private MixinDefinition GetMixin (Type mixinType)
    {
      return TargetClassDefinitionUtility.GetActiveConfiguration (typeof (TargetClass)).Mixins[mixinType];
    }

    [Test]
    public void UnrelatedMixins ()
    {
      Set<MixinDefinition>[] groups = GetGroups (_independent1, _independent2, _overrideM1);
      Assert.AreEqual (3, groups.Length);
      Assert.That (groups[0], Is.EquivalentTo (new object[] { _independent1 }));
      Assert.That (groups[1], Is.EquivalentTo (new object[] { _independent2 }));
      Assert.That (groups[2], Is.EquivalentTo (new object[] { _overrideM1 }));
    }

    [Test]
    public void Overrides_NoCut ()
    {
      Set<MixinDefinition>[] groups = GetGroups (_overrideM1, _overrideM2);
      Assert.AreEqual (2, groups.Length);
      Assert.That (groups[0], Is.EquivalentTo (new object[] { _overrideM1 }));
      Assert.That (groups[1], Is.EquivalentTo (new object[] { _overrideM2 }));
    }

    [Test]
    public void Overrides_Cut ()
    {
      Set<MixinDefinition>[] groups = GetGroups (_overrideM1, _overrideM1M2);
      Assert.AreEqual (1, groups.Length);
      Assert.That (groups[0], Is.EquivalentTo (new object[] { _overrideM1, _overrideM1M2 }));
    }

    [Test]
    public void Overrides_TransitiveCut ()
    {
      Set<MixinDefinition>[] groups = GetGroups (_overrideM1, _overrideM2, _overrideM1M2);
      Assert.AreEqual (1, groups.Length);
      Assert.That (groups[0], Is.EquivalentTo (new object[] { _overrideM1, _overrideM2, _overrideM1M2 }));
    }

    [Test]
    public void BaseCallDependency_NoCut ()
    {
      Set<MixinDefinition>[] groups = GetGroups (_baseCallDependency1, _independent1);
      Assert.AreEqual (2, groups.Length);
      Assert.That (groups[0], Is.EquivalentTo (new object[] { _baseCallDependency1 }));
      Assert.That (groups[1], Is.EquivalentTo (new object[] { _independent1 }));
    }

    [Test]
    public void BaseCallDependency_Cut ()
    {
      Set<MixinDefinition>[] groups = GetGroups (_baseCallDependency0, _baseCallDependency1);
      Assert.AreEqual (1, groups.Length);
      Assert.That (groups[0], Is.EquivalentTo (new object[] { _baseCallDependency0, _baseCallDependency1 }));
    }

    [Test]
    public void BaseCallDependency_TransitiveCut ()
    {
      Set<MixinDefinition>[] groups = GetGroups (_baseCallDependency0, _baseCallDependency1, _baseCallDependency2OverrideM1);
      Assert.AreEqual (1, groups.Length);
      Assert.That (groups[0], Is.EquivalentTo (new object[] { _baseCallDependency0, _baseCallDependency1, _baseCallDependency2OverrideM1 }));
    }

    [Test]
    public void BaseCallDependency_TransitiveCutAndOverride ()
    {
      Set<MixinDefinition>[] groups = GetGroups (_overrideM2, _overrideM1M2, _baseCallDependency1, _baseCallDependency2OverrideM1);
      Assert.AreEqual (1, groups.Length);
      Assert.That (groups[0], Is.EquivalentTo (new object[] { _overrideM2, _overrideM1M2, _baseCallDependency1, _baseCallDependency2OverrideM1 }));
    }

    [Test]
    public void ExplicitDependency_NoCut ()
    {
      Set<MixinDefinition>[] groups = GetGroups (_additionalDependency1, _independent1);
      Assert.AreEqual (2, groups.Length);
      Assert.That (groups[0], Is.EquivalentTo (new object[] { _additionalDependency1 }));
      Assert.That (groups[1], Is.EquivalentTo (new object[] { _independent1 }));
    }

    [Test]
    public void ExplicitDependency_Cut ()
    {
      Set<MixinDefinition>[] groups = GetGroups (_additionalDependency0, _additionalDependency1);
      Assert.AreEqual (1, groups.Length);
      Assert.That (groups[0], Is.EquivalentTo (new object[] { _additionalDependency0, _additionalDependency1 }));
    }

    [Test]
    public void ExplicitDependency_Cut_OtherDirection ()
    {
      Set<MixinDefinition>[] groups = GetGroups (_additionalDependency1, _additionalDependency0);
      Assert.AreEqual (1, groups.Length);
      Assert.That (groups[0], Is.EquivalentTo (new object[] { _additionalDependency0, _additionalDependency1 }));
    }

    private Set<MixinDefinition>[] GetGroups (params MixinDefinition[] mixins)
    {
      return EnumerableUtility.ToArray (_grouper.GroupMixins (mixins));
    }
  }
}