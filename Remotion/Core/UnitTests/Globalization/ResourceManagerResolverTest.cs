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
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.Globalization.Implementation;
using Remotion.UnitTests.Globalization.TestDomain;

namespace Remotion.UnitTests.Globalization
{
  [TestFixture]
  public class ResourceManagerResolverTest
  {
    private ResourceManagerResolver<MultiLingualResourcesAttribute> _resolver;

    [SetUp]
    public void SetUp ()
    {
      _resolver = new ResourceManagerResolver<MultiLingualResourcesAttribute>();
    }

    [Test]
    public void GetResourceManager_NoSuccess ()
    {
      var result = _resolver.GetResourceManager (typeof (ClassWithoutMultiLingualResourcesAttributes));

      Assert.That (result, Is.TypeOf (typeof (NullResourceManager)));
    }

    [Test]
    public void GetResourceManager_Hierarchy ()
    {
      var resourceManagerSet = (ResourceManagerSet) _resolver.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes));
      Assert.That (resourceManagerSet.ResourceManagers.Count(), Is.EqualTo (5));
      var names = new[]
                  {
                      resourceManagerSet.ResourceManagers.ElementAt (4).Name, resourceManagerSet.ResourceManagers.ElementAt (3).Name,
                      resourceManagerSet.ResourceManagers.ElementAt (2).Name, resourceManagerSet.ResourceManagers.ElementAt (1).Name,
                      resourceManagerSet.ResourceManagers.ElementAt (0).Name
                  };
      Assert.That (names, Is.EquivalentTo (new[] { "One", "Two", "Three", "Four", "Five" }));
      Assert.That (Array.IndexOf (names, "One"), Is.LessThan (Array.IndexOf (names, "Four")));
    }

    [Test]
    public void GetResourceManager_DefiningType_Hierarchy ()
    {
      var resourceManagerSet =
          (ResourceManagerSet) _resolver.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes));
      Assert.That (resourceManagerSet.ResourceManagers.Count(), Is.EqualTo (5));
      var names = new[]
                  {
                      resourceManagerSet.ResourceManagers.ElementAt (4).Name, resourceManagerSet.ResourceManagers.ElementAt (3).Name,
                      resourceManagerSet.ResourceManagers.ElementAt (2).Name, resourceManagerSet.ResourceManagers.ElementAt (1).Name,
                      resourceManagerSet.ResourceManagers.ElementAt (0).Name
                  };
      Assert.That (names, Is.EquivalentTo (new[] { "One", "Two", "Three", "Four", "Five" }));
      Assert.That (Array.IndexOf (names, "One"), Is.LessThan (Array.IndexOf (names, "Four")));
    }

    [Test]
    public void GetResourceManager_NoDefiningType_Hierarchy ()
    {
      var resourceManagerSet = (ResourceManagerSet) _resolver.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes));
      Assert.That (resourceManagerSet.ResourceManagers.Count(), Is.EqualTo (5));
      Assert.That (
          new[]
          {
              resourceManagerSet.ResourceManagers.ElementAt (0).Name,
              resourceManagerSet.ResourceManagers.ElementAt (1).Name,
              resourceManagerSet.ResourceManagers.ElementAt (2).Name,
              resourceManagerSet.ResourceManagers.ElementAt (3).Name,
              resourceManagerSet.ResourceManagers.ElementAt (4).Name
          },
          Is.EquivalentTo (new[] { "One", "Two", "Three", "Four", "Five" }));
    }
  }
}