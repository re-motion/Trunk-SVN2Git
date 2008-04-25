using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins.Context;

namespace Remotion.UnitTests.Mixins.Context
{
  [TestFixture]
  public class MixinContextTest
  {
    [Test]
    public void ExplicitDependencies_Empty ()
    {
      MixinContext mixinContext = new MixinContext (typeof (BT7Mixin1), new Type[0]);

      Assert.AreEqual (0, mixinContext.ExplicitDependencies.Count);
      Assert.IsFalse (mixinContext.ExplicitDependencies.ContainsKey (typeof (IBaseType2)));

      Assert.That (mixinContext.ExplicitDependencies, Is.Empty);
    }

    [Test]
    public void ExplicitInterfaceDependencies_NonEmpty ()
    {
      MixinContext mixinContext = new MixinContext (typeof (BT6Mixin1), new Type[] {typeof (IBT6Mixin2), typeof (IBT6Mixin3)});

      Assert.AreEqual (2, mixinContext.ExplicitDependencies.Count);
      Assert.IsTrue (mixinContext.ExplicitDependencies.ContainsKey (typeof (IBT6Mixin2)));
      Assert.IsTrue (mixinContext.ExplicitDependencies.ContainsKey (typeof (IBT6Mixin3)));

      Assert.That(mixinContext.ExplicitDependencies, Is.EquivalentTo(new Type[] { typeof(IBT6Mixin2), typeof(IBT6Mixin3) }));
    }

    [Test]
    public void ExplicitMixinDependencies_NonEmpty ()
    {
      MixinContext mixinContext = new MixinContext (typeof (BT6Mixin1), new Type[] { typeof (BT6Mixin2), typeof (BT6Mixin3<>) });

      Assert.AreEqual (2, mixinContext.ExplicitDependencies.Count);
      Assert.IsTrue (mixinContext.ExplicitDependencies.ContainsKey (typeof (BT6Mixin2)));
      Assert.IsTrue (mixinContext.ExplicitDependencies.ContainsKey (typeof (BT6Mixin3<>)));

      Assert.That(mixinContext.ExplicitDependencies, Is.EquivalentTo(new Type[] { typeof(BT6Mixin2), typeof(BT6Mixin3<>) }));
    }

    [Test]
    public void Equals_True ()
    {
      MixinContext c1 = new MixinContext (typeof (BT6Mixin1), typeof (BT6Mixin2), typeof (BT6Mixin3<>));
      MixinContext c2 = new MixinContext (typeof (BT6Mixin1), typeof (BT6Mixin2), typeof (BT6Mixin3<>));
      MixinContext c3 = new MixinContext (typeof (BT6Mixin1));
      MixinContext c4 = new MixinContext (typeof (BT6Mixin1));
      MixinContext c5 = new MixinContext (typeof (BT6Mixin1), typeof (BT6Mixin3<>), typeof (BT6Mixin2));

      Assert.AreEqual (c1, c2);
      Assert.AreEqual (c3, c4);
      Assert.AreEqual (c1, c5);
    }

    [Test]
    public void Equals_False()
    {
      MixinContext c1 = new MixinContext (typeof (BT6Mixin1), typeof (BT6Mixin2), typeof (BT6Mixin3<>));
      MixinContext c2 = new MixinContext (typeof (BT6Mixin1), typeof (BT6Mixin3<>));
      MixinContext c3 = new MixinContext (typeof (BT6Mixin2), typeof (BT6Mixin3<>));

      Assert.AreNotEqual (c1, c2);
      Assert.AreNotEqual (c2, c3);
    }

    [Test]
    public void GetHashCode_Equal ()
    {
      MixinContext c1 = new MixinContext (typeof (BT6Mixin1), typeof (BT6Mixin2), typeof (BT6Mixin3<>));
      MixinContext c2 = new MixinContext (typeof (BT6Mixin1), typeof (BT6Mixin2), typeof (BT6Mixin3<>));
      MixinContext c3 = new MixinContext (typeof (BT6Mixin1));
      MixinContext c4 = new MixinContext (typeof (BT6Mixin1));
      MixinContext c5 = new MixinContext (typeof (BT6Mixin1), typeof (BT6Mixin3<>), typeof (BT6Mixin2));

      Assert.AreEqual (c1.GetHashCode (), c2.GetHashCode ());
      Assert.AreEqual (c3.GetHashCode (), c4.GetHashCode ());
      Assert.AreEqual (c1.GetHashCode (), c5.GetHashCode ());
    }
  }
}