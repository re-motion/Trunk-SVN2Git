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
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins.Context;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.Context
{
  [TestFixture]
  public class MixinContextOriginTest
  {
    private Assembly _someAssembly;

    [SetUp]
    public void SetUp ()
    {
      _someAssembly = GetType ().Assembly;
    }

    [Test]
    public void CreateForCustomAttribute_OnMemberInfo ()
    {
      var origin = MixinContextOrigin.CreateForCustomAttribute (new UsesAttribute (typeof (NullMixin)), typeof (MixinContextOriginTest));

      Assert.That (origin.Kind, Is.EqualTo ("UsesAttribute"));
      Assert.That (origin.Assembly, Is.EqualTo (typeof (MixinContextOriginTest).Assembly));
      Assert.That (origin.Location, Is.EqualTo ("Remotion.Mixins.UnitTests.Core.Context.MixinContextOriginTest"));
    }

    [Test]
    public void CreateForCustomAttribute_OnAssembly ()
    {
      var origin = MixinContextOrigin.CreateForCustomAttribute (new MixAttribute (typeof (object), typeof (NullMixin)), _someAssembly);

      Assert.That (origin.Kind, Is.EqualTo ("MixAttribute"));
      Assert.That (origin.Assembly, Is.EqualTo (_someAssembly));
      Assert.That (origin.Location, Is.EqualTo ("assembly"));
    }

    [Test]
    public void CreateForMethod ()
    {
      var origin = MixinContextOrigin.CreateForMethod (MethodBase.GetCurrentMethod());

      Assert.That (origin.Kind, Is.EqualTo ("Method"));
      Assert.That (origin.Assembly, Is.EqualTo (GetType().Assembly));
      Assert.That (origin.Location, Is.EqualTo ("Void CreateForMethod(), declaring type: Remotion.Mixins.UnitTests.Core.Context.MixinContextOriginTest"));
    }

    [Test]
    public new void ToString ()
    {
      var origin = new MixinContextOrigin ("SomeKind", _someAssembly, "some location");

      var expectedCodeBase = _someAssembly.CodeBase;
      var expected = string.Format (
          "SomeKind, Location: 'some location' (Assembly: 'Remotion.Mixins.UnitTests', code base: {0})", 
          expectedCodeBase);
      Assert.That (origin.ToString(), Is.EqualTo (expected));
    }
  }
}