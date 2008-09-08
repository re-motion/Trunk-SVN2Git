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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Web.ExecutionEngine;
using Remotion.Development.Web.UnitTesting.AspNetFramework;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  [TestFixture]
  public class ResourceObjectTest
  {
    [SetUp]
    public void SetUp ()
    {
      HttpContextHelper.CreateHttpContext ("GET", "default.aspx", string.Empty);
      Assert.That (ResourceUrlResolver.GetRoot (false), Is.EqualTo ("/res/"));
    }

    [Test]
    public void InitializeWithResourceAssembly_WithoutAssembly ()
    {
      ResourceObject resourceObject = new ResourceObject (null, "path.aspx");

      Assert.That (resourceObject.ResourceRoot, Is.Empty);
      Assert.That (resourceObject.Path, Is.EqualTo("path.aspx"));
    }

    [Test]
    public void InitializeWithResourceAssembly_WithAssembly ()
    {
      ResourceObject resourceObject = new ResourceObject (GetType ().Assembly, "path.aspx");

      Assert.That (resourceObject.ResourceRoot, Is.EqualTo ("/res/Remotion.Web.UnitTests/"));
      Assert.That (resourceObject.Path, Is.EqualTo ("path.aspx"));
    }

    [Test]
    public void GetResourcePath_WithoutAssembly ()
    {
      ResourceObject resourceObject = new ResourceObject (null, "path.aspx");

      Assert.That (resourceObject.GetResourcePath (null), Is.EqualTo ("path.aspx"));
    }

    [Test]
    public void GetResourcePath_WithAssembly ()
    {
      ResourceObject resourceObject = new ResourceObject (GetType ().Assembly, "path.aspx");

      Assert.That (resourceObject.GetResourcePath (null), Is.EqualTo ("/res/Remotion.Web.UnitTests/path.aspx"));
    }
  }
}