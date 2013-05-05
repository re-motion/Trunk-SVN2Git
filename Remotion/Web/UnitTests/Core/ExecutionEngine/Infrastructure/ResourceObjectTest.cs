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
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class ResourceObjectTest
  {
    [SetUp]
    public void SetUp ()
    {
      HttpContextHelper.CreateHttpContext ("GET", "default.aspx", string.Empty);
      Assert.That (ResourceUrlResolver.GetAssemblyRoot (false, GetType().Assembly), Is.EqualTo ("/res/Remotion.Web.UnitTests/"));
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
