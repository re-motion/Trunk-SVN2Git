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
using System.Resources;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Globalization;

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
      Assert.AreEqual (2, resourceManagers.Length);
      Assert.AreEqual ("One", resourceManagers[0].BaseName);
      Assert.AreEqual ("Two", resourceManagers[1].BaseName);
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

      Assert.AreNotSame (resourceManagers1, resourceManagers2);
      Assert.AreSame (resourceManagers1[0], resourceManagers2[0]);
      Assert.AreSame (resourceManagers1[1], resourceManagers2[1]);
      Assert.AreNotSame (resourceManagers1[0], resourceManagers1[1]);
      Assert.AreNotSame (resourceManagers2[0], resourceManagers2[1]);
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
      Assert.AreEqual (2, resourceManagers.Length);
      Assert.AreEqual (typeof (object).Assembly, PrivateInvoke.GetNonPublicField (resourceManagers[0], "MainAssembly"));
      Assert.AreEqual (typeof (object).Assembly, PrivateInvoke.GetNonPublicField (resourceManagers[1], "MainAssembly"));
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
      Assert.AreEqual (2, resourceManagers.Length);
      Assert.AreEqual (typeof (ResourceManagerResolverTest).Assembly, PrivateInvoke.GetNonPublicField (resourceManagers[0], "MainAssembly"));
      Assert.AreEqual (typeof (object).Assembly, PrivateInvoke.GetNonPublicField (resourceManagers[1], "MainAssembly"));
    }
  }
}
