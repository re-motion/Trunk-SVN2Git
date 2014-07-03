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
using System.Linq;
using System.Resources;
using NUnit.Framework;
using Remotion.Globalization.Implementation;
using Remotion.Globalization.UnitTests.TestDomain;

namespace Remotion.Globalization.UnitTests.Implementation
{
  [TestFixture]
  public class ResourceManagerFactoryTest
  {
    private ResourceManagerFactory _factory;

    [SetUp]
    public void SetUp ()
    {
      _factory = new ResourceManagerFactory();
    }

    [Test]
    public void GetResourceManagers ()
    {
      MultiLingualResourcesAttribute[] attributes =
      {
          new MultiLingualResourcesAttribute (NamedResources.One),
          new MultiLingualResourcesAttribute (NamedResources.Two)
      };
      var resourceManagers = _factory.GetResourceManagers (typeof (ResourceManagerFactoryTest).Assembly, attributes).ToArray();
      Assert.That (resourceManagers.Length, Is.EqualTo (2));
      Assert.That (resourceManagers[0].BaseName, Is.EqualTo (NamedResources.One));
      Assert.That (resourceManagers[1].BaseName, Is.EqualTo (NamedResources.Two));
    }

    [Test]
    public void GetResourceManagers_UsesCache ()
    {
      MultiLingualResourcesAttribute[] attributes =
      {
          new MultiLingualResourcesAttribute (NamedResources.One),
          new MultiLingualResourcesAttribute (NamedResources.Two)
      };
      var resourceManagers1 = _factory.GetResourceManagers (typeof (ResourceManagerFactoryTest).Assembly, attributes).ToArray();
      var resourceManagers2 = _factory.GetResourceManagers (typeof (ResourceManagerFactoryTest).Assembly, attributes).ToArray();

      Assert.That (resourceManagers2, Is.Not.SameAs (resourceManagers1));
      Assert.That (resourceManagers2[0], Is.SameAs (resourceManagers1[0]));
      Assert.That (resourceManagers2[1], Is.SameAs (resourceManagers1[1]));
      Assert.That (resourceManagers1[1], Is.Not.SameAs (resourceManagers1[0]));
      Assert.That (resourceManagers2[1], Is.Not.SameAs (resourceManagers2[0]));
    }

    [Test]
    public void GetResourceManagers_MissingResourceFile_ThrowsMissingManifestResourceException ()
    {
      MultiLingualResourcesAttribute[] attributes =
      {
          new MultiLingualResourcesAttribute ("Missing"),
      };

      Assert.That (
          () => _factory.GetResourceManagers (typeof (ResourceManagerFactoryTest).Assembly, attributes).ToArray(),
          Throws.TypeOf<MissingManifestResourceException>()
              .With.Message.EqualTo (
                  "Could not find any resources appropriate for the neutral culture. "
                  + "Make sure 'Missing.resources' was correctly embedded into assembly 'Remotion.Globalization.UnitTests' at compile time."));
    }
  }
}
