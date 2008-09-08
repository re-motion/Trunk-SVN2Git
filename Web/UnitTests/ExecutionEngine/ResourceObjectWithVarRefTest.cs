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
using Remotion.Collections;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  [TestFixture]
  public class ResourceObjectWithVarRefTest
  {
    private NameObjectCollection _variables;

    [SetUp]
    public void SetUp ()
    {
      HttpContextHelper.CreateHttpContext ("GET", "default.aspx", string.Empty);
      Assert.That (ResourceUrlResolver.GetRoot (false), Is.EqualTo ("/res/"));

      _variables = new NameObjectCollection { { "ThePath", "path.aspx" } };
    }

    [Test]
    public void InitializeWithResourceAssembly_WithoutAssembly ()
    {
      ResourceObjectWithVarRef resourceObject = new ResourceObjectWithVarRef (null, new WxeVariableReference ("ThePage"));

      Assert.That (resourceObject.ResourceRoot, Is.Empty);
      Assert.That (resourceObject.PathReference.Name, Is.EqualTo ("ThePage"));
    }

    [Test]
    public void InitializeWithResourceAssembly_WithAssembly ()
    {
      ResourceObjectWithVarRef resourceObject = new ResourceObjectWithVarRef (GetType().Assembly, new WxeVariableReference ("ThePage"));

      Assert.That (resourceObject.ResourceRoot, Is.EqualTo ("/res/Remotion.Web.UnitTests/"));
      Assert.That (resourceObject.PathReference.Name, Is.EqualTo ("ThePage"));
    }

    [Test]
    public void GetResourcePath_WithoutAssembly ()
    {
      ResourceObjectWithVarRef resourceObject = new ResourceObjectWithVarRef (null, new WxeVariableReference ("ThePath"));

      Assert.That (resourceObject.GetResourcePath (_variables), Is.EqualTo ("path.aspx"));
    }

    [Test]
    public void GetResourcePath_WithAssembly ()
    {
      ResourceObjectWithVarRef resourceObject = new ResourceObjectWithVarRef (GetType().Assembly, new WxeVariableReference ("ThePath"));

      Assert.That (resourceObject.GetResourcePath (_variables), Is.EqualTo ("/res/Remotion.Web.UnitTests/path.aspx"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The variable 'InvalidIdentifier' could not be found in the list of variables.")]
    public void GetResourcePath_WithInvalidReference ()
    {
      ResourceObjectWithVarRef resourceObject = new ResourceObjectWithVarRef (null, new WxeVariableReference ("InvalidIdentifier"));

      resourceObject.GetResourcePath (_variables);
    }

    [Test]
    [ExpectedException (typeof (InvalidCastException), ExpectedMessage = "The variable 'InvalidType' was of type 'System.Int32'. Expected type is 'System.String'.")]
    public void GetResourcePath_WithInvalidTypeInVariable ()
    {
      _variables.Add ("InvalidType", 1);
      ResourceObjectWithVarRef resourceObject = new ResourceObjectWithVarRef (null, new WxeVariableReference ("InvalidType"));

      resourceObject.GetResourcePath (_variables);
    }
  }
}