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
using System.Resources;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Globalization;
using Remotion.Globalization.Implementation;

namespace Remotion.UnitTests.Globalization
{
  [TestFixture]
  public class ResourceManagerFactoryTest
  {
    private ResourceManagerFactory _resolver;

    [SetUp]
    public void SetUp ()
    {
      _resolver = new ResourceManagerFactory ();
    }

    [Test]
    public void GetResourceManagers ()
    {
      MultiLingualResourcesAttribute[] attributes = new MultiLingualResourcesAttribute[]
          {
            new MultiLingualResourcesAttribute ("One"),
            new MultiLingualResourcesAttribute ("Two")
          };
      ResourceManager[] resourceManagers = _resolver.GetResourceManagers (typeof (object).Assembly, attributes);
      Assert.That (resourceManagers.Length, Is.EqualTo (2));
      Assert.That (resourceManagers[0].BaseName, Is.EqualTo ("One"));
      Assert.That (resourceManagers[1].BaseName, Is.EqualTo ("Two"));
    }

    [Test]
    public void GetResourceManagers_UsesCache ()
    {
      MultiLingualResourcesAttribute[] attributes = new MultiLingualResourcesAttribute[]
          {
            new MultiLingualResourcesAttribute ("One"),
            new MultiLingualResourcesAttribute ("Two")
          };
      ResourceManager[] resourceManagers1 = _resolver.GetResourceManagers (typeof (object).Assembly, attributes);
      ResourceManager[] resourceManagers2 = _resolver.GetResourceManagers (typeof (object).Assembly, attributes);

      Assert.That (resourceManagers2, Is.Not.SameAs (resourceManagers1));
      Assert.That (resourceManagers2[0], Is.SameAs (resourceManagers1[0]));
      Assert.That (resourceManagers2[1], Is.SameAs (resourceManagers1[1]));
      Assert.That (resourceManagers1[1], Is.Not.SameAs (resourceManagers1[0]));
      Assert.That (resourceManagers2[1], Is.Not.SameAs (resourceManagers2[0]));
    }

    [Test]
    public void GetResourceManagers_NoSpecificAssembly ()
    {
      MultiLingualResourcesAttribute[] attributes = new MultiLingualResourcesAttribute[]
          {
            new MultiLingualResourcesAttribute ("One"),
            new MultiLingualResourcesAttribute ("Two")
          };
      ResourceManager[] resourceManagers = _resolver.GetResourceManagers (typeof (object).Assembly, attributes);
      Assert.That (resourceManagers.Length, Is.EqualTo (2));
      Assert.That (PrivateInvoke.GetNonPublicField (resourceManagers[0], "MainAssembly"), Is.EqualTo (typeof (object).Assembly));
      Assert.That (PrivateInvoke.GetNonPublicField (resourceManagers[1], "MainAssembly"), Is.EqualTo (typeof (object).Assembly));
    }

    [Test]
    public void GetResourceManagers_SpecificAssembly ()
    {
      MultiLingualResourcesAttribute[] attributes = new MultiLingualResourcesAttribute[]
          {
            new MultiLingualResourcesAttribute ("One"),
            new MultiLingualResourcesAttribute ("Two")
          };

      PrivateInvoke.InvokeNonPublicMethod (attributes[0], "SetResourceAssembly", typeof (ResourceManagerResolverTest).Assembly);

      ResourceManager[] resourceManagers = _resolver.GetResourceManagers (typeof (object).Assembly, attributes);
      Assert.That (resourceManagers.Length, Is.EqualTo (2));
      Assert.That (PrivateInvoke.GetNonPublicField (resourceManagers[0], "MainAssembly"), Is.EqualTo (typeof (ResourceManagerResolverTest).Assembly));
      Assert.That (PrivateInvoke.GetNonPublicField (resourceManagers[1], "MainAssembly"), Is.EqualTo (typeof (object).Assembly));
    }
  }
}
