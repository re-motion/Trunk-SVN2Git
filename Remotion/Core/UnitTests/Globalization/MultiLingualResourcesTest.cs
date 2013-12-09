﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.UnitTests.Globalization.TestDomain;

namespace Remotion.UnitTests.Globalization
{
  [Obsolete]
  public class MultiLingualResourcesTest
  {
    [Test]
    public void GetResourceManager_TypeWithResources_ReturnsResources ()
    {
      var resourceManager = MultiLingualResources.GetResourceManager (typeof (ClassWithMultiLingualResourcesAttributes), true);

      Assert.That (resourceManager.IsNull, Is.False);
      Assert.That (resourceManager, Is.InstanceOf<ResourceManagerSet>());

      var resourceManagerSet = (ResourceManagerSet) resourceManager;

      Assert.That (resourceManagerSet.ResourceManagers.Select (rm => rm.Name), Is.EquivalentTo (new[] { "One", "Two", "Three" }));
    }

    [Test]
    public void GetResourceManager_TypeWithoutResources_ThrowsResourceException ()
    {
      Assert.That (
          () => MultiLingualResources.GetResourceManager (typeof (ClassWithoutMultiLingualResourcesAttributes), true),
          Throws.TypeOf<ResourceException>());
    }

    [Test]
    public void GetResourceManager_WithTypeDefiningAndInheritingMultipleResources_ReturnsResourceManagersInOrderOfDefinition ()
    {
      var resourceManager = MultiLingualResources.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), true);

      Assert.That (resourceManager, Is.InstanceOf<ResourceManagerSet>());
      var resourceManagerSet = (ResourceManagerSet) resourceManager;
      Assert.That (resourceManagerSet.ResourceManagers.Select (rm => rm.Name), Is.EquivalentTo (new[] { "Four", "Five", "One", "Two", "Three" }));
      Assert.That (resourceManagerSet.ResourceManagers.Take (2).Select (rm => rm.Name), Is.EquivalentTo (new[] { "Four", "Five" }));
      Assert.That (resourceManagerSet.ResourceManagers.Skip (2).Select (rm => rm.Name), Is.EquivalentTo (new[] { "One", "Two", "Three" }));
    }

    [Test]
    public void GetResourceManagerWithoutInheritance_WithTypeDefiningAndInheritingMultipleResources_ReturnsResourceManagersInOrderOfDefinition ()
    {
      var resourceManager = MultiLingualResources.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes));

      Assert.That (resourceManager, Is.InstanceOf<ResourceManagerSet>());
      var resourceManagerSet = (ResourceManagerSet) resourceManager;
      Assert.That (resourceManagerSet.ResourceManagers.Select (rm => rm.Name), Is.EquivalentTo (new[] { "Four", "Five" }));
    }

    [Test]
    public void GetResourceManager_WithTypeDefiningAndInheritingMultipleResources_AndDoNotGetInheritedResources_ReturnsResourceManagersInOrderOfDefinition ()
    {
      var resourceManager = MultiLingualResources.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), false);

      Assert.That (resourceManager, Is.InstanceOf<ResourceManagerSet>());
      var resourceManagerSet = (ResourceManagerSet) resourceManager;
      Assert.That (resourceManagerSet.ResourceManagers.Select (rm => rm.Name), Is.EquivalentTo (new[] { "Four", "Five" }));
    }

    [Test]
    [Ignore ("Should throw a ResourceException, but instead returns inherited resource manager")]
    public void GetResourceManager_TypeWithOnlyInheritedResources_AndDoNotGetInheritedResources_ThrowsResourceException ()
    {
      Assert.That (
          () => MultiLingualResources.GetResourceManager (typeof (InheritedClassWithoutMultiLingualResourcesAttributes), false),
          Throws.TypeOf<ResourceException>());
    }

    [Test]
    public void GetResourceManager_TypeWithOnlyInheritedResources_AndDoNotGetInheritedResources_DoesNotThrowResourceExceptionBecauseOfBug ()
    {
      var resourceManager = MultiLingualResources.GetResourceManager (typeof (InheritedClassWithoutMultiLingualResourcesAttributes), false);

      Assert.That (resourceManager.IsNull, Is.False);
      Assert.That (resourceManager, Is.InstanceOf<ResourceManagerSet>());

      var resourceManagerSet = (ResourceManagerSet) resourceManager;

      Assert.That (resourceManagerSet.ResourceManagers.Select (rm => rm.Name), Is.EquivalentTo (new[] { "One", "Two", "Three" }));
    }
  }
}