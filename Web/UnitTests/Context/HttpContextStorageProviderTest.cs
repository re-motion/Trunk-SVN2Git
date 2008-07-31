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
using System.Runtime.Remoting.Messaging;
using System.Web;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Web.Context;
using Remotion.Development.Web.UnitTesting.AspNetFramework;

namespace Remotion.Web.UnitTests.Context
{
  [TestFixture]
  public class HttpContextStorageProviderTest
  {
    private HttpContextStorageProvider _provider;
    private HttpContext _testContext;

    [SetUp]
    public void SetUp ()
    {
      _testContext = HttpContextHelper.CreateHttpContext ("x", "y", "z");
      HttpContext.Current = _testContext;
      _provider = new HttpContextStorageProvider ();
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
      Assert.That (_testContext.Items["Foo"], Is.EqualTo (45));
    }

    [Test]
    public void SetData_Null ()
    {
      _provider.SetData ("Foo", 45);
      _provider.SetData ("Foo", null);
      Assert.That (_provider.GetData ("Foo"), Is.Null);
      Assert.That (_testContext.Items["Foo"], Is.Null);
    }

    [Test]
    public void FreeData ()
    {
      _provider.SetData ("Foo", 45);
      _provider.FreeData ("Foo");
      Assert.That (_provider.GetData ("Foo"), Is.Null);
      Assert.That (_testContext.Items.Contains ("Foo"), Is.False);
    }

    [Test]
    public void FreeData_WithoutValue ()
    {
      _provider.FreeData ("Foo");
      Assert.That (_provider.GetData ("Foo"), Is.Null);
      Assert.That (_testContext.Items.Contains ("Foo"), Is.False);
    }

    [Test]
    public void FallbackToCallContext_IfNoCurrentHttpContext ()
    {
      HttpContext.Current = null;
      
      _provider.SetData ("Foo", 123);
      Assert.That (_provider.GetData ("Foo"), Is.EqualTo (123));
      Assert.That (CallContext.GetData ("Foo"), Is.EqualTo (123));
      
      _provider.FreeData ("Foo");
      Assert.That (CallContext.GetData ("Foo"), Is.Null);
    }
  }
}