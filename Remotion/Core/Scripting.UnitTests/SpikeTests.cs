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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Scripting.UnitTests.TestDomain;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class SpikeTests
  {
    class Base
    {
      public virtual void Test ()
      {
      }
    }

    class Derived1 : Base
    {
      public sealed override void Test ()
      {
        base.Test ();
      }
    }

    class Derived1Child : Derived1
    {
      public new void Test ()
      {
        base.Test ();
      }
    }

    class Derived2 : Proxied
    {
    }

    [Test]
    [Explicit]
    public void name ()
    {
      var method1 = typeof (Base).GetMethod ("Test");
      var method2 = typeof (Derived1).GetMethod ("Test");
      var method3 = typeof (Derived2).GetMethod ("Test");

      Assert.That (method1, Is.EqualTo (method2.GetBaseDefinition ()));
      Assert.That (method1, Is.EqualTo (method3.GetBaseDefinition ()));
      Assert.That (method2.GetBaseDefinition (), Is.EqualTo (method3.GetBaseDefinition ()));
    }

    [Test]
    [Explicit]
    public void name2 ()
    {
      var method1 = typeof (Proxied).GetMethod ("Sum");
      var method3 = typeof (Derived2).GetMethod ("Sum");

      Assert.That (method1.GetBaseDefinition (), Is.EqualTo (method3.GetBaseDefinition ()));
    }
  }
}