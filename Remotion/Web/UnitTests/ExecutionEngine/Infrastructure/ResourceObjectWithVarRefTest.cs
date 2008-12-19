// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Collections;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure
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
