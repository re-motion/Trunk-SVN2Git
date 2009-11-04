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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Context;

namespace Remotion.UnitTests.Context
{
  [TestFixture]
  public class BootstrapStorageProviderTest
  {
    private BootstrapStorageProvider _provider;

    [SetUp]
    public void SetUp ()
    {
      _provider = new BootstrapStorageProvider ();
    }

    [Test]
    public void GetData_WithoutValue ()
    {
      Assert.That (_provider.GetData ("Foo"), Is.Null);
    }

    [Test]
    public void SetData ()
    {
      _provider.SetData ("Foo", 45);
      Assert.That (_provider.GetData ("Foo"), Is.EqualTo (45));
    }

    [Test]
    public void SetData_Null ()
    {
      _provider.SetData ("Foo", 45);
      _provider.SetData ("Foo", null);
      Assert.That (_provider.GetData ("Foo"), Is.Null);
    }

    [Test]
    public void FreeData ()
    {
      _provider.SetData ("Foo", 45);
      _provider.FreeData ("Foo");
      Assert.That (_provider.GetData ("Foo"), Is.Null);
    }

    [Test]
    public void FreeData_WithoutValue ()
    {
      _provider.FreeData ("Foo");
      Assert.That (_provider.GetData ("Foo"), Is.Null);
    }

    [Test]
    public void BootstrapStorageProvider_IsIsolated ()
    {
      BootstrapStorageProvider one = new BootstrapStorageProvider ();
      BootstrapStorageProvider two = new BootstrapStorageProvider ();

      one.SetData ("x", "y");
      Assert.That (one.GetData ("x"), Is.EqualTo ("y"));
      Assert.That (two.GetData ("x"), Is.Null);
    }
  }
}
