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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Context
{
  [TestFixture]
  public class MixinContextTest
  {
    [Test]
    public void ExplicitDependencies_Empty ()
    {
      MixinContext mixinContext = new MixinContext (MixinKind.Extending, typeof (BT7Mixin1), MemberVisibility.Private);

      Assert.AreEqual (0, mixinContext.ExplicitDependencies.Count);
      Assert.IsFalse (mixinContext.ExplicitDependencies.ContainsKey (typeof (IBaseType2)));

      Assert.That (mixinContext.ExplicitDependencies, Is.Empty);
    }

    [Test]
    public void ExplicitInterfaceDependencies_NonEmpty ()
    {
      MixinContext mixinContext = new MixinContext (MixinKind.Extending, typeof (BT6Mixin1), MemberVisibility.Private, typeof (IBT6Mixin2), typeof (IBT6Mixin3));

      Assert.AreEqual (2, mixinContext.ExplicitDependencies.Count);
      Assert.IsTrue (mixinContext.ExplicitDependencies.ContainsKey (typeof (IBT6Mixin2)));
      Assert.IsTrue (mixinContext.ExplicitDependencies.ContainsKey (typeof (IBT6Mixin3)));

      Assert.That(mixinContext.ExplicitDependencies, Is.EquivalentTo(new Type[] { typeof(IBT6Mixin2), typeof(IBT6Mixin3) }));
    }

    [Test]
    public void ExplicitMixinDependencies_NonEmpty ()
    {
      MixinContext mixinContext = new MixinContext (MixinKind.Extending, typeof (BT6Mixin1), MemberVisibility.Private, typeof (BT6Mixin2), typeof (BT6Mixin3<>));

      Assert.AreEqual (2, mixinContext.ExplicitDependencies.Count);
      Assert.IsTrue (mixinContext.ExplicitDependencies.ContainsKey (typeof (BT6Mixin2)));
      Assert.IsTrue (mixinContext.ExplicitDependencies.ContainsKey (typeof (BT6Mixin3<>)));

      Assert.That(mixinContext.ExplicitDependencies, Is.EquivalentTo(new Type[] { typeof(BT6Mixin2), typeof(BT6Mixin3<>) }));
    }

    [Test]
    public void Equals_True ()
    {
      MixinContext c1a = new MixinContext (MixinKind.Extending, typeof (BT6Mixin1), MemberVisibility.Private, typeof (BT6Mixin2), typeof (BT6Mixin3<>));
      MixinContext c1b = new MixinContext (MixinKind.Extending, typeof (BT6Mixin1), MemberVisibility.Private, typeof (BT6Mixin2), typeof (BT6Mixin3<>));
      MixinContext c1c = new MixinContext (MixinKind.Extending, typeof (BT6Mixin1), MemberVisibility.Private, typeof (BT6Mixin3<>), typeof (BT6Mixin2));
      MixinContext c2a = new MixinContext (MixinKind.Used, typeof (BT6Mixin1), MemberVisibility.Public);
      MixinContext c2b = new MixinContext (MixinKind.Used, typeof (BT6Mixin1), MemberVisibility.Public);

      Assert.AreEqual (c1a, c1b);
      Assert.AreEqual (c1a, c1c);
      Assert.AreEqual (c2a, c2b);
    }

    [Test]
    public void Equals_False()
    {
      MixinContext c1 = new MixinContext (MixinKind.Extending, typeof (BT6Mixin1), MemberVisibility.Private, typeof (BT6Mixin2), typeof (BT6Mixin3<>));
      MixinContext c2 = new MixinContext (MixinKind.Extending, typeof (BT6Mixin1), MemberVisibility.Private, typeof (BT6Mixin3<>));
      MixinContext c3 = new MixinContext (MixinKind.Extending, typeof (BT6Mixin2), MemberVisibility.Private, typeof (BT6Mixin3<>));
      MixinContext c4 = new MixinContext (MixinKind.Used, typeof (BT6Mixin2), MemberVisibility.Private, typeof (BT6Mixin3<>));
      MixinContext c5 = new MixinContext (MixinKind.Used, typeof (BT6Mixin2), MemberVisibility.Public, typeof (BT6Mixin3<>));

      Assert.AreNotEqual (c1, c2);
      Assert.AreNotEqual (c2, c3);
      Assert.AreNotEqual (c3, c4);
      Assert.AreNotEqual (c4, c5);
    }

    [Test]
    public void GetHashCode_Equal ()
    {
      MixinContext c1a = new MixinContext (MixinKind.Extending, typeof (BT6Mixin1), MemberVisibility.Private, typeof (BT6Mixin2), typeof (BT6Mixin3<>));
      MixinContext c1b = new MixinContext (MixinKind.Extending, typeof (BT6Mixin1), MemberVisibility.Private, typeof (BT6Mixin2), typeof (BT6Mixin3<>));
      MixinContext c1c = new MixinContext (MixinKind.Extending, typeof (BT6Mixin1), MemberVisibility.Private, typeof (BT6Mixin3<>), typeof (BT6Mixin2));

      MixinContext c2a = new MixinContext (MixinKind.Extending, typeof (BT6Mixin1), MemberVisibility.Private);
      MixinContext c2b = new MixinContext (MixinKind.Extending, typeof (BT6Mixin1), MemberVisibility.Private);

      MixinContext c3a = new MixinContext (MixinKind.Used, typeof (BT6Mixin1), MemberVisibility.Public, typeof (BT6Mixin3<>), typeof (BT6Mixin2));
      MixinContext c3b = new MixinContext (MixinKind.Used, typeof (BT6Mixin1), MemberVisibility.Public, typeof (BT6Mixin3<>), typeof (BT6Mixin2));

      Assert.AreEqual (c1a.GetHashCode (), c1b.GetHashCode ());
      Assert.AreEqual (c1a.GetHashCode (), c1c.GetHashCode ());
      Assert.AreEqual (c2a.GetHashCode (), c2b.GetHashCode ());
      Assert.AreEqual (c3a.GetHashCode (), c3b.GetHashCode ());
    }

    [Test]
    public void MixinKindProperty ()
    {
      MixinContext c1 = new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), MemberVisibility.Private);
      MixinContext c2 = new MixinContext (MixinKind.Used, typeof (BT1Mixin1), MemberVisibility.Private);
      Assert.That (c1.MixinKind, Is.EqualTo (MixinKind.Extending));
      Assert.That (c2.MixinKind, Is.EqualTo (MixinKind.Used));
    }

    [Test]
    public void IntroducedMemberVisibility_Private ()
    {
      MixinContext context = new MixinContext (MixinKind.Used, typeof (BT1Mixin1), MemberVisibility.Private);
      Assert.That (context.IntroducedMemberVisibility, Is.EqualTo (MemberVisibility.Private));
    }

    [Test]
    public void IntroducedMemberVisibility_Public ()
    {
      MixinContext context = new MixinContext (MixinKind.Used, typeof (BT1Mixin1), MemberVisibility.Public);
      Assert.That (context.IntroducedMemberVisibility, Is.EqualTo (MemberVisibility.Public));
    }
  }
}
