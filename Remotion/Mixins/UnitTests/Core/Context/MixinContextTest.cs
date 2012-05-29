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
using Remotion.Mixins.Context.Serialization;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Rhino.Mocks;
using System.Reflection;

namespace Remotion.Mixins.UnitTests.Core.Context
{
  [TestFixture]
  public class MixinContextTest
  {
    [Test]
    public void ExplicitDependencies_Empty ()
    {
      var mixinContext = new MixinContext (MixinKind.Extending, typeof (BT7Mixin1), MemberVisibility.Private, Enumerable.Empty<Type> ());

      Assert.AreEqual (0, mixinContext.ExplicitDependencies.Count);
      Assert.IsFalse (mixinContext.ExplicitDependencies.ContainsKey (typeof (IBaseType2)));

      Assert.That (mixinContext.ExplicitDependencies, Is.Empty);
    }

    [Test]
    public void ExplicitInterfaceDependencies_NonEmpty ()
    {
      var mixinContext = new MixinContext (
          MixinKind.Extending, typeof (BT6Mixin1), MemberVisibility.Private, new[] { typeof (IBT6Mixin2), typeof (IBT6Mixin3) });

      Assert.AreEqual (2, mixinContext.ExplicitDependencies.Count);
      Assert.IsTrue (mixinContext.ExplicitDependencies.ContainsKey (typeof (IBT6Mixin2)));
      Assert.IsTrue (mixinContext.ExplicitDependencies.ContainsKey (typeof (IBT6Mixin3)));

      Assert.That(mixinContext.ExplicitDependencies, Is.EquivalentTo(new[] { typeof(IBT6Mixin2), typeof(IBT6Mixin3) }));
    }

    [Test]
    public void ExplicitMixinDependencies_NonEmpty ()
    {
      var mixinContext = new MixinContext (
          MixinKind.Extending, typeof (BT6Mixin1), MemberVisibility.Private, new[] { typeof (BT6Mixin2), typeof (BT6Mixin3<>) });

      Assert.AreEqual (2, mixinContext.ExplicitDependencies.Count);
      Assert.IsTrue (mixinContext.ExplicitDependencies.ContainsKey (typeof (BT6Mixin2)));
      Assert.IsTrue (mixinContext.ExplicitDependencies.ContainsKey (typeof (BT6Mixin3<>)));

      Assert.That(mixinContext.ExplicitDependencies, Is.EquivalentTo(new[] { typeof(BT6Mixin2), typeof(BT6Mixin3<>) }));
    }

    [Test]
    public void Equals_True ()
    {
      var c1a = new MixinContext (MixinKind.Extending, typeof (BT6Mixin1), MemberVisibility.Private, new[] { typeof (BT6Mixin2), typeof (BT6Mixin3<>) });
      var c1b = new MixinContext (MixinKind.Extending, typeof (BT6Mixin1), MemberVisibility.Private, new[] { typeof (BT6Mixin2), typeof (BT6Mixin3<>) });
      var c1c = new MixinContext (MixinKind.Extending, typeof (BT6Mixin1), MemberVisibility.Private, new[] { typeof (BT6Mixin3<>), typeof (BT6Mixin2) });
      var c2a = new MixinContext (MixinKind.Used, typeof (BT6Mixin1), MemberVisibility.Public, Enumerable.Empty<Type>());
      var c2b = new MixinContext (MixinKind.Used, typeof (BT6Mixin1), MemberVisibility.Public, Enumerable.Empty<Type> ());

      Assert.AreEqual (c1a, c1b);
      Assert.AreEqual (c1a, c1c);
      Assert.AreEqual (c2a, c2b);
    }

    [Test]
    public void Equals_False()
    {
      var c1 = new MixinContext (MixinKind.Extending, typeof (BT6Mixin1), MemberVisibility.Private, new[] { typeof (BT6Mixin2), typeof (BT6Mixin3<>) });
      var c2 = new MixinContext (MixinKind.Extending, typeof (BT6Mixin1), MemberVisibility.Private, new[] { typeof (BT6Mixin3<>) });
      var c3 = new MixinContext (MixinKind.Extending, typeof (BT6Mixin2), MemberVisibility.Private, new[] { typeof (BT6Mixin3<>) });
      var c4 = new MixinContext (MixinKind.Used, typeof (BT6Mixin2), MemberVisibility.Private, new[] { typeof (BT6Mixin3<>) });
      var c5 = new MixinContext (MixinKind.Used, typeof (BT6Mixin2), MemberVisibility.Public, new[] { typeof (BT6Mixin3<>) });

      Assert.AreNotEqual (c1, c2);
      Assert.AreNotEqual (c2, c3);
      Assert.AreNotEqual (c3, c4);
      Assert.AreNotEqual (c4, c5);
    }

    [Test]
    public void GetHashCode_Equal ()
    {
      var c1a = new MixinContext (MixinKind.Extending, typeof (BT6Mixin1), MemberVisibility.Private, new[] { typeof (BT6Mixin2), typeof (BT6Mixin3<>) });
      var c1b = new MixinContext (MixinKind.Extending, typeof (BT6Mixin1), MemberVisibility.Private, new[] { typeof (BT6Mixin2), typeof (BT6Mixin3<>) });
      var c1c = new MixinContext (MixinKind.Extending, typeof (BT6Mixin1), MemberVisibility.Private, new[] { typeof (BT6Mixin3<>), typeof (BT6Mixin2) });

      var c2a = new MixinContext (MixinKind.Extending, typeof (BT6Mixin1), MemberVisibility.Private, Enumerable.Empty<Type>());
      var c2b = new MixinContext (MixinKind.Extending, typeof (BT6Mixin1), MemberVisibility.Private, Enumerable.Empty<Type> ());

      var c3a = new MixinContext (MixinKind.Used, typeof (BT6Mixin1), MemberVisibility.Public, new[] { typeof (BT6Mixin3<>), typeof (BT6Mixin2) });
      var c3b = new MixinContext (MixinKind.Used, typeof (BT6Mixin1), MemberVisibility.Public, new[] { typeof (BT6Mixin3<>), typeof (BT6Mixin2) });

      Assert.AreEqual (c1a.GetHashCode (), c1b.GetHashCode ());
      Assert.AreEqual (c1a.GetHashCode (), c1c.GetHashCode ());
      Assert.AreEqual (c2a.GetHashCode (), c2b.GetHashCode ());
      Assert.AreEqual (c3a.GetHashCode (), c3b.GetHashCode ());
    }

    [Test]
    public void MixinKindProperty ()
    {
      var c1 = new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), MemberVisibility.Private, Enumerable.Empty<Type> ());
      var c2 = new MixinContext (MixinKind.Used, typeof (BT1Mixin1), MemberVisibility.Private, Enumerable.Empty<Type> ());
      Assert.That (c1.MixinKind, Is.EqualTo (MixinKind.Extending));
      Assert.That (c2.MixinKind, Is.EqualTo (MixinKind.Used));
    }

    [Test]
    public void IntroducedMemberVisibility_Private ()
    {
      var context = new MixinContext (MixinKind.Used, typeof (BT1Mixin1), MemberVisibility.Private, Enumerable.Empty<Type> ());
      Assert.That (context.IntroducedMemberVisibility, Is.EqualTo (MemberVisibility.Private));
    }

    [Test]
    public void IntroducedMemberVisibility_Public ()
    {
      var context = new MixinContext (MixinKind.Used, typeof (BT1Mixin1), MemberVisibility.Public, Enumerable.Empty<Type> ());
      Assert.That (context.IntroducedMemberVisibility, Is.EqualTo (MemberVisibility.Public));
    }

    [Test]
    public void Serialize()
    {
      var serializer = MockRepository.GenerateMock<IMixinContextSerializer> ();
      var context = new MixinContext (MixinKind.Used, typeof (BT1Mixin1), MemberVisibility.Public, new[] { typeof (int), typeof (string) });
      context.Serialize (serializer);

      serializer.AssertWasCalled (mock => mock.AddMixinType (typeof (BT1Mixin1)));
      serializer.AssertWasCalled (mock => mock.AddMixinKind (MixinKind.Used));
      serializer.AssertWasCalled (mock => mock.AddIntroducedMemberVisibility (MemberVisibility.Public));
      serializer.AssertWasCalled (mock => mock.AddExplicitDependencies (Arg<IEnumerable<Type>>.List.Equal (new[] {typeof (int), typeof (string) })));
    }

    [Test]
    public void Deserialize ()
    {
      var expectedContext = new MixinContext (MixinKind.Used, typeof (BT1Mixin1), MemberVisibility.Public, new[] { typeof (int), typeof (string) });

      var deserializer = MockRepository.GenerateMock<IMixinContextDeserializer> ();
      deserializer.Expect (mock => mock.GetMixinType ()).Return (typeof (BT1Mixin1));
      deserializer.Expect (mock => mock.GetMixinKind()).Return (MixinKind.Used);
      deserializer.Expect (mock => mock.GetIntroducedMemberVisibility ()).Return (MemberVisibility.Public);
      deserializer.Expect (mock => mock.GetExplicitDependencies ()).Return (new[] { typeof (int), typeof (string) });

      var context = MixinContext.Deserialize(deserializer);

      Assert.That (context, Is.EqualTo (expectedContext));
    }

    [Test]
    public void Serialization_IsUpToDate()
    {
      var properties = typeof (MixinContext).GetProperties (BindingFlags.Public |BindingFlags.Instance);
      Assert.That (typeof (IMixinContextSerializer).GetMethods ().Length, Is.EqualTo (properties.Length));
      Assert.That (typeof (IMixinContextDeserializer).GetMethods ().Length, Is.EqualTo (properties.Length));
    }
  }
}
