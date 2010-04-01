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
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Web.Factories;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.Factories
{
  [TestFixture]
  public class ResourceUrlFactoryTest
  {
    private ResourceTheme _resourceTheme;
    private IResourceUrlFactory _factory;

    [SetUp]
    public void SetUp ()
    {
      _resourceTheme = new ResourceTheme ("TestTheme");
      _factory = new ResourceUrlFactory (_resourceTheme);
    }

    [Test]
    public void CreateResourceUrl ()
    {
      var resourceUrl = _factory.CreateResourceUrl (typeof (ResourceUrlFactoryTest), ResourceType.Image, "theRelativeUrl.img");

      Assert.That (resourceUrl, Is.InstanceOfType (typeof (ResourceUrl)));
      Assert.That (((ResourceUrl) resourceUrl).DefiningType, Is.EqualTo (typeof (ResourceUrlFactoryTest)));
      Assert.That (((ResourceUrl) resourceUrl).ResourceType, Is.EqualTo (ResourceType.Image));
      Assert.That (((ResourceUrl) resourceUrl).RelativeUrl, Is.EqualTo ("theRelativeUrl.img"));
    }

    [Test]
    public void CreateThemedResourceUrl ()
    {
      var resourceUrl = _factory.CreateThemedResourceUrl (typeof (ResourceUrlFactoryTest), ResourceType.Image, "theRelativeUrl.img");

      Assert.That (resourceUrl, Is.InstanceOfType (typeof (ThemedResourceUrl)));
      Assert.That (((ThemedResourceUrl) resourceUrl).DefiningType, Is.EqualTo (typeof (ResourceUrlFactoryTest)));
      Assert.That (((ThemedResourceUrl) resourceUrl).ResourceType, Is.EqualTo (ResourceType.Image));
      Assert.That (((ThemedResourceUrl) resourceUrl).ResourceTheme, Is.EqualTo (_resourceTheme));
      Assert.That (((ThemedResourceUrl) resourceUrl).RelativeUrl, Is.EqualTo ("theRelativeUrl.img"));
    }
  }
}