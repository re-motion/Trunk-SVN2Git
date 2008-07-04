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