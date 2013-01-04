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
using Remotion.Collections;
using Remotion.Globalization;
using Remotion.UnitTests.Globalization.TestDomain;
using Remotion.Utilities;

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
    public void GetResourceDefinitionStream_SuccessOnSameType ()
    {
      ResourceDefinition<MultiLingualResourcesAttribute>[] definitions =
          _resolver.GetResourceDefinitionStream (typeof (ClassWithMultiLingualResourcesAttributes), false).ToArray();

      Assert.That (definitions.Length, Is.EqualTo (1));
      Assert.That (definitions[0].Type, Is.SameAs (typeof (ClassWithMultiLingualResourcesAttributes)));
      Assert.That (definitions[0].SupplementingAttributes.ToArray(), Is.Empty);
      Assert.That (
          definitions[0].OwnAttributes,
          Is.EquivalentTo (
              AttributeUtility.GetCustomAttributes<MultiLingualResourcesAttribute> (typeof (ClassWithMultiLingualResourcesAttributes), false)));
    }

    [Test]
    public void GetResourceDefinitionStream_InheritanceFalse ()
    {
      ResourceDefinition<MultiLingualResourcesAttribute>[] definitions =
          _resolver.GetResourceDefinitionStream (typeof (InheritedClassWithMultiLingualResourcesAttributes), false).ToArray();

      Assert.That (definitions.Length, Is.EqualTo (1));
      Assert.That (definitions[0].Type, Is.SameAs (typeof (InheritedClassWithMultiLingualResourcesAttributes)));
      Assert.That (definitions[0].SupplementingAttributes.ToArray(), Is.Empty);
      Assert.That (
          definitions[0].OwnAttributes,
          Is.EquivalentTo (
              AttributeUtility.GetCustomAttributes<MultiLingualResourcesAttribute> (typeof (InheritedClassWithMultiLingualResourcesAttributes), false)));
    }

    [Test]
    public void GetResourceDefinitionStream_InheritanceTrue ()
    {
      ResourceDefinition<MultiLingualResourcesAttribute>[] definitions =
          _resolver.GetResourceDefinitionStream (typeof (InheritedClassWithMultiLingualResourcesAttributes), true).ToArray();

      Assert.That (definitions.Length, Is.EqualTo (2));

      Assert.That (definitions[0].Type, Is.SameAs (typeof (InheritedClassWithMultiLingualResourcesAttributes)));
      Assert.That (definitions[0].SupplementingAttributes.ToArray(), Is.Empty);
      Assert.That (
          definitions[0].OwnAttributes,
          Is.EquivalentTo (
              AttributeUtility.GetCustomAttributes<MultiLingualResourcesAttribute> (typeof (InheritedClassWithMultiLingualResourcesAttributes), false)));

      Assert.That (definitions[1].Type, Is.SameAs (typeof (ClassWithMultiLingualResourcesAttributes)));
      Assert.That (definitions[1].SupplementingAttributes.ToArray(), Is.Empty);
      Assert.That (
          definitions[1].OwnAttributes,
          Is.EquivalentTo (
              AttributeUtility.GetCustomAttributes<MultiLingualResourcesAttribute> (typeof (ClassWithMultiLingualResourcesAttributes), false)));
    }

    [Test]
    public void GetResourceDefinitionStream_SuccessOnBase ()
    {
      ResourceDefinition<MultiLingualResourcesAttribute>[] definitions =
          _resolver.GetResourceDefinitionStream (typeof (InheritedClassWithoutMultiLingualResourcesAttributes), false).ToArray();
      Assert.That (definitions.Length, Is.EqualTo (1));
      Assert.That (definitions[0].Type, Is.SameAs (typeof (ClassWithMultiLingualResourcesAttributes)));

      Assert.That (
          definitions[0].OwnAttributes,
          Is.EquivalentTo (
              AttributeUtility.GetCustomAttributes<MultiLingualResourcesAttribute> (typeof (ClassWithMultiLingualResourcesAttributes), false)));
    }

    [Test]
    public void GetResourceDefinitionStream_NoSuccess ()
    {
      ResourceDefinition<MultiLingualResourcesAttribute>[] definitions =
          _resolver.GetResourceDefinitionStream (typeof (ClassWithoutMultiLingualResourcesAttributes), false).ToArray();
      Assert.That (definitions, Is.Empty);
    }

    [Test]
    public void GetResourceManagerCacheEntry_NoSuccess ()
    {
      var result = _resolver.GetResourceManagerCacheEntry (typeof (ClassWithoutMultiLingualResourcesAttributes), false);

      Assert.That (result.IsEmpty, Is.True);
    }

    [Test]
    public void TryGetResourceManager_NoHierarchy ()
    {
      var entry = _resolver.GetResourceManagerCacheEntry (typeof (InheritedClassWithMultiLingualResourcesAttributes), false);
      Assert.That (entry.DefiningType, Is.SameAs (typeof (InheritedClassWithMultiLingualResourcesAttributes)));

      var resourceManagerSet = (ResourceManagerSet) entry.ResourceManager;
      Assert.That (resourceManagerSet.Count, Is.EqualTo (2));
      var names = new Set<string> (resourceManagerSet[0].Name, resourceManagerSet[1].Name);
      Assert.That (names, Is.EquivalentTo (new[] { "Four", "Five" }));
    }

    [Test]
    public void TryGetResourceManager_NoHierarchy_SuccessOnBase ()
    {
      var entry = _resolver.GetResourceManagerCacheEntry (typeof (InheritedClassWithoutMultiLingualResourcesAttributes), false);
      Assert.That (entry.DefiningType, Is.SameAs (typeof (ClassWithMultiLingualResourcesAttributes)));
      var resourceManagerSet = (ResourceManagerSet) entry.ResourceManager;
      Assert.That (resourceManagerSet.Count, Is.EqualTo (3));
      var names = new Set<string> (resourceManagerSet[0].Name, resourceManagerSet[1].Name, resourceManagerSet[2].Name);
      Assert.That (names, Is.EquivalentTo (new[] { "One", "Two", "Three" }));
    }

    [Test]
    public void TryGetResourceManager_Hierarchy ()
    {
      var entry = _resolver.GetResourceManagerCacheEntry (typeof (InheritedClassWithMultiLingualResourcesAttributes), true);
      Assert.That (entry.DefiningType, Is.SameAs (typeof (InheritedClassWithMultiLingualResourcesAttributes)));
      var resourceManagerSet = (ResourceManagerSet) entry.ResourceManager;
      Assert.That (resourceManagerSet.Count, Is.EqualTo (5));
      var names = new[]
                  {
                      resourceManagerSet[0].Name, resourceManagerSet[1].Name, resourceManagerSet[2].Name,
                      resourceManagerSet[3].Name, resourceManagerSet[4].Name
                  };
      Assert.That (names, Is.EquivalentTo (new[] { "One", "Two", "Three", "Four", "Five" }));
      Assert.That (Array.IndexOf (names, "One"), Is.LessThan (Array.IndexOf (names, "Four")));
    }

    [Test]
    [ExpectedException (typeof (ResourceException), ExpectedMessage = 
        "Type Remotion.UnitTests.Globalization.TestDomain.ClassWithoutMultiLingualResourcesAttributes and its base classes do not define the "
        + "attribute MultiLingualResourcesAttribute.")]
    public void GetResourceManager_DefiningType_NoSuccess ()
    {
      Type definingType;
      _resolver.GetResourceManager (typeof (ClassWithoutMultiLingualResourcesAttributes), true, out definingType);
    }

    [Test]
    public void GetResourceManager_DefiningType_NoHierarchy ()
    {
      Type definingType;
      var resourceManagerSet =
          (ResourceManagerSet) _resolver.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), false, out definingType);
      Assert.That (definingType, Is.SameAs (typeof (InheritedClassWithMultiLingualResourcesAttributes)));
      Assert.That (resourceManagerSet.Count, Is.EqualTo (2));
      var names = new Set<string> (resourceManagerSet[0].Name, resourceManagerSet[1].Name);
      Assert.That (names, Is.EquivalentTo (new[] { "Four", "Five" }));
    }

    [Test]
    public void GetResourceManager_DefiningType_NoHierarchy_SuccessOnBase ()
    {
      Type definingType;
      var resourceManagerSet =
          (ResourceManagerSet) _resolver.GetResourceManager (typeof (InheritedClassWithoutMultiLingualResourcesAttributes), false, out definingType);
      Assert.That (definingType, Is.SameAs (typeof (ClassWithMultiLingualResourcesAttributes)));
      Assert.That (resourceManagerSet.Count, Is.EqualTo (3));
      var names = new Set<string> (resourceManagerSet[0].Name, resourceManagerSet[1].Name, resourceManagerSet[2].Name);
      Assert.That (names, Is.EquivalentTo (new[] { "One", "Two", "Three" }));
    }

    [Test]
    public void GetResourceManager_DefiningType_Hierarchy ()
    {
      Type definingType;
      var resourceManagerSet =
          (ResourceManagerSet) _resolver.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), true, out definingType);
      Assert.That (definingType, Is.SameAs (typeof (InheritedClassWithMultiLingualResourcesAttributes)));
      Assert.That (resourceManagerSet.Count, Is.EqualTo (5));
      var names = new[]
                  {
                      resourceManagerSet[0].Name, resourceManagerSet[1].Name, resourceManagerSet[2].Name,
                      resourceManagerSet[3].Name, resourceManagerSet[4].Name
                  };
      Assert.That (names, Is.EquivalentTo (new[] { "One", "Two", "Three", "Four", "Five" }));
      Assert.That (Array.IndexOf (names, "One"), Is.LessThan (Array.IndexOf (names, "Four")));
    }

    [Test]
    [ExpectedException (typeof (ResourceException), ExpectedMessage =
        "Type Remotion.UnitTests.Globalization.TestDomain.ClassWithoutMultiLingualResourcesAttributes and its base classes do not define the "
        + "attribute MultiLingualResourcesAttribute.")]
    public void GetResourceManager_NoDefiningType_NoSuccess ()
    {
      _resolver.GetResourceManager (typeof (ClassWithoutMultiLingualResourcesAttributes), false);
    }

    [Test]
    public void GetResourceManager_NoDefiningType_NoHierarchy ()
    {
      var resourceManagerSet = (ResourceManagerSet) _resolver.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), false);
      Assert.That (resourceManagerSet.Count, Is.EqualTo (2));
      var names = new Set<string> (resourceManagerSet[0].Name, resourceManagerSet[1].Name);
      Assert.That (names, Is.EquivalentTo (new[] { "Four", "Five" }));
    }

    [Test]
    public void GetResourceManager_NoDefiningType_NoHierarchy_SuccessOnBase ()
    {
      var resourceManagerSet =
          (ResourceManagerSet) _resolver.GetResourceManager (typeof (InheritedClassWithoutMultiLingualResourcesAttributes), false);
      Assert.That (resourceManagerSet.Count, Is.EqualTo (3));
      var names = new Set<string> (resourceManagerSet[0].Name, resourceManagerSet[1].Name, resourceManagerSet[2].Name);
      Assert.That (names, Is.EquivalentTo (new[] { "One", "Two", "Three" }));
    }

    [Test]
    public void GetResourceManager_NoDefiningType_Hierarchy ()
    {
      var resourceManagerSet = (ResourceManagerSet) _resolver.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), true);
      Assert.That (resourceManagerSet.Count, Is.EqualTo (5));
      var names = new Set<string> (
          resourceManagerSet[0].Name,
          resourceManagerSet[1].Name,
          resourceManagerSet[2].Name,
          resourceManagerSet[3].Name,
          resourceManagerSet[4].Name);
      Assert.That (names, Is.EquivalentTo (new[] { "One", "Two", "Three", "Four", "Five" }));
    }
  }
}