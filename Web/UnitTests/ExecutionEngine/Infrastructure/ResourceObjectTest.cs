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
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure
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
      Assert.That (resourceObject.Path, Is.EqualTo ("path.aspx"));
    }

    [Test]
    public void InitializeWithResourceAssembly_WithAssembly ()
    {
      ResourceObject resourceObject = new ResourceObject (GetType().Assembly, "path.aspx");

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
      ResourceObject resourceObject = new ResourceObject (GetType().Assembly, "path.aspx");

      Assert.That (resourceObject.GetResourcePath (null), Is.EqualTo ("/res/Remotion.Web.UnitTests/path.aspx"));
    }
  }
}
