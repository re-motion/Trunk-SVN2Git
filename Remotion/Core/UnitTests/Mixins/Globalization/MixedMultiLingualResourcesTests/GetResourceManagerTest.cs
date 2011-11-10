// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Collections;
using Remotion.Globalization;
using Remotion.Mixins;
using Remotion.Mixins.Globalization;
using Remotion.UnitTests.Mixins.Globalization.TestDomain;

namespace Remotion.UnitTests.Mixins.Globalization.MixedMultiLingualResourcesTests
{
  [TestFixture]
  public class GetResourceManagerTest
  {
    [Test]
    [ExpectedException (typeof (ResourceException), ExpectedMessage = "Type Remotion.UnitTests.Mixins.Globalization.TestDomain."
        + "ClassWithoutMultiLingualResourcesAttributes and its base classes do not define the attribute MultiLingualResourcesAttribute.")]
    public void NoAttributes_NoInheritance ()
    {
      MixedMultiLingualResources.GetResourceManager (typeof (ClassWithoutMultiLingualResourcesAttributes), false);
    }

    [Test]
    [ExpectedException (typeof (ResourceException), ExpectedMessage = "Type Remotion.UnitTests.Mixins.Globalization.TestDomain."
        + "ClassWithoutMultiLingualResourcesAttributes and its base classes do not define the attribute MultiLingualResourcesAttribute.")]
    public void NoAttributes_Inheritance ()
    {
      MixedMultiLingualResources.GetResourceManager (typeof (ClassWithoutMultiLingualResourcesAttributes), true);
    }

    [Test]
    public void AttributesOnClass ()
    {
      ResourceManagerSet resourceManager =
          (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (ClassWithMultiLingualResourcesAttributes), false);

      Assert.AreEqual (1, resourceManager.Count);
      Assert.AreEqual ("OnTarget", resourceManager[0].Name);
    }

    [Test]
    public void AttributesOnBase ()
    {
      ResourceManagerSet resourceManager =
          (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (InheritedClassWithoutMultiLingualResourcesAttributes), false);

      Assert.AreEqual (1, resourceManager.Count);
      Assert.AreEqual ("OnTarget", resourceManager[0].Name);
    }

		[Test]
		public void AttributesOnBaseAndClass_InheritedDefault ()
		{
			ResourceManagerSet resourceManager =
					(ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes));

			Assert.AreEqual (1, resourceManager.Count);
			Assert.AreEqual ("OnInherited", resourceManager[0].Name);
		}

    [Test]
    public void AttributesOnBaseAndClass_InheritedFalse ()
    {
      ResourceManagerSet resourceManager =
          (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), false);

      Assert.AreEqual (1, resourceManager.Count);
      Assert.AreEqual ("OnInherited", resourceManager[0].Name);
    }

    [Test]
    public void AttributesOnBaseAndClass_InheritedTrue ()
    {
      ResourceManagerSet resourceManager =
          (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), true);

      Assert.AreEqual (2, resourceManager.Count);
      Set<string> names = new Set<string> (resourceManager[0].Name, resourceManager[1].Name);
      Assert.That (names, Is.EquivalentTo (new string[] {"OnTarget", "OnInherited"}));
    }

    [Test]
    public void AttributesFromMixin_InheritedFalse ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes> ().AddMixin<MixinAddingMultiLingualResourcesAttributes1> ().EnterScope ())
      {
        ResourceManagerSet resourceManager =
            (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (ClassWithoutMultiLingualResourcesAttributes), false);

        Assert.AreEqual (1, resourceManager.Count);
        Assert.AreEqual ("OnMixin1", resourceManager[0].Name);
      }
    }

    [Test]
    public void AttributesFromMixin_InheritedTrue ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes> ().AddMixin<MixinAddingMultiLingualResourcesAttributes1> ().EnterScope ())
      {
        ResourceManagerSet resourceManager =
            (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (ClassWithoutMultiLingualResourcesAttributes), true);

        Assert.AreEqual (1, resourceManager.Count);
        Assert.AreEqual ("OnMixin1", resourceManager[0].Name);
      }
    }

    [Test]
    public void AttributesFromMultipleMixins_InheritedFalse ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes2> ()
          .EnterScope ())
      {
        ResourceManagerSet resourceManager =
            (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (ClassWithoutMultiLingualResourcesAttributes), false);

        Assert.AreEqual (3, resourceManager.Count);
        string[] names = new string[] { resourceManager[0].Name, resourceManager[1].Name, resourceManager[2].Name };
        Assert.That (names, Is.EquivalentTo (new object[] {"OnMixin1", "OnMixin2a", "OnMixin2b"}));
      }
    }

    [Test]
    public void AttributesFromMixinsAndBaseAndClass_InheritedFalse ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<InheritedClassWithMultiLingualResourcesAttributes> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes2> ()
          .EnterScope ())
      {
        ResourceManagerSet resourceManager =
            (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), false);

        Assert.AreEqual (4, resourceManager.Count);
				Set<string> names = new Set<string> (resourceManager[0].Name, resourceManager[1].Name, resourceManager[2].Name, resourceManager[3].Name,
						resourceManager[3].Name);
				Assert.That (names, Is.EquivalentTo (new string[] { "OnInherited", "OnMixin1", "OnMixin2a", "OnMixin2b" }));
      }
    }

    [Test]
    public void AttributesFromMixinsAndBaseAndClass_InheritedTrue ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<InheritedClassWithMultiLingualResourcesAttributes> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes2> ()
          .EnterScope ())
      {
        ResourceManagerSet resourceManager =
            (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), true);

        Assert.AreEqual (5, resourceManager.Count);
        Set<string> names = new Set<string> (resourceManager[0].Name, resourceManager[1].Name, resourceManager[2].Name, resourceManager[3].Name,
            resourceManager[4].Name);
        Assert.That (names, Is.EquivalentTo (new string[] { "OnTarget", "OnInherited", "OnMixin1", "OnMixin2a", "OnMixin2b" }));
      }
    }

    [Test]
    public void Cache ()
    {
      IResourceManager set1 = MixedMultiLingualResources.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), true);
      IResourceManager set2;
      IResourceManager set3;
      IResourceManager set4;
      IResourceManager set5;

      using (MixinConfiguration.BuildNew ()
          .ForClass<InheritedClassWithMultiLingualResourcesAttributes> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes2> ()
          .EnterScope ())
      {
        set2 = MixedMultiLingualResources.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), true);
        set3 = MixedMultiLingualResources.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), true);
      }
      set4 = MixedMultiLingualResources.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), true);
      using (MixinConfiguration.BuildNew ()
          .ForClass<InheritedClassWithMultiLingualResourcesAttributes> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes2> ()
          .EnterScope ())
      {
        set5 = MixedMultiLingualResources.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), true);
      }

      Assert.AreSame (set1, set4);
      Assert.AreSame (set2, set3);
      Assert.AreSame (set2, set5);
      Assert.AreNotSame (set1, set2);

      Assert.AreEqual (2, ((ResourceManagerSet) set1).Count);
      Assert.AreEqual (5, ((ResourceManagerSet) set2).Count);
    }
  }
}
