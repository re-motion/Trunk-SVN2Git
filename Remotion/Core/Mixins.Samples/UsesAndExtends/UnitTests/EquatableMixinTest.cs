// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Mixins.Samples.UsesAndExtends.Core;
using Remotion.Reflection;

namespace Remotion.Mixins.Samples.UsesAndExtends.UnitTests
{
  [TestFixture]
  public class EquatableMixinTest
  {
    [Uses(typeof (EquatableMixin<C>))]
    public class C
    {
      public int I;
      public string S;
      public bool B;
    }

    [Test]
    public void ImplementsEquatable()
    {
      C c = new C();
      Assert.IsFalse (c is IEquatable<C>);

      C c2 = ObjectFactory.Create<C>(ParamList.Empty);
      Assert.IsTrue (c2 is IEquatable<C>);
    }

    [Test]
    public void EqualsRespectsMembers ()
    {
      C c = ObjectFactory.Create<C> (ParamList.Empty);
      C c2 = ObjectFactory.Create<C> (ParamList.Empty);
      Assert.AreEqual (c, c2);

      c2.S = "foo";
      Assert.AreNotEqual (c, c2);
      c2.I = 5;
      c2.B = true;
      Assert.AreNotEqual (c, c2);
      c.S = "foo";
      Assert.AreNotEqual (c, c2);
      c.I = 5;
      Assert.AreNotEqual (c, c2);
      c.B = true;
      Assert.AreEqual (c, c2);
    }

    [Test]
    public void GetHashCodeRespectsMembers ()
    {
      C c = ObjectFactory.Create<C> (ParamList.Empty);
      C c2 = ObjectFactory.Create<C> (ParamList.Empty);
      Assert.AreEqual (c.GetHashCode(), c2.GetHashCode());

      c2.S = "foo";
      Assert.AreNotEqual (c.GetHashCode(), c2.GetHashCode());
      c2.I = 5;
      c2.B = true;
      Assert.AreNotEqual (c.GetHashCode (), c2.GetHashCode ());
      c.S = "foo";
      Assert.AreNotEqual (c.GetHashCode (), c2.GetHashCode ());
      c.I = 5;
      Assert.AreNotEqual (c.GetHashCode (), c2.GetHashCode ());
      c.B = true;
      Assert.AreEqual (c.GetHashCode (), c2.GetHashCode ());
    }
  }
}
