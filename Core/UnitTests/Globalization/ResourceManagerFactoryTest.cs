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