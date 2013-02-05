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

using NUnit.Framework;
using Remotion.Globalization;
using Remotion.Mixins.Globalization;
using Remotion.Mixins.UnitTests.Core.Globalization.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.Globalization.MixedMultiLingualResourcesTests
{
  [TestFixture]
  public class GetResourceManagerTest
  {
    [Test]
    [ExpectedException (typeof (ResourceException), ExpectedMessage = "Type Remotion.Mixins.UnitTests.Core.Globalization.TestDomain."
        + "ClassWithoutMultiLingualResourcesAttributes and its base classes do not define the attribute MultiLingualResourcesAttribute.")]
    public void NoAttributes_NoInheritance ()
    {
      MixedMultiLingualResources.GetResourceManager (typeof (ClassWithoutMultiLingualResourcesAttributes), false);
    }

    [Test]
    [ExpectedException (typeof (ResourceException), ExpectedMessage = "Type Remotion.Mixins.UnitTests.Core.Globalization.TestDomain."
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

      Assert.That (resourceManager.Count, Is.EqualTo (1));
      Assert.That (resourceManager[0].Name, Is.EqualTo ("OnTarget"));
    }

    [Test]
    public void AttributesOnBase ()
    {
      ResourceManagerSet resourceManager =
          (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (InheritedClassWithoutMultiLingualResourcesAttributes), false);

      Assert.That (resourceManager.Count, Is.EqualTo (1));
      Assert.That (resourceManager[0].Name, Is.EqualTo ("OnTarget"));
    }

    [Test]
    public void AttributesOnBaseAndClass_InheritedDefault ()
    {
      ResourceManagerSet resourceManager =
          (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes));

      Assert.That (resourceManager.Count, Is.EqualTo (1));
      Assert.That (resourceManager[0].Name, Is.EqualTo ("OnInherited"));
    }

    [Test]
    public void AttributesOnBaseAndClass_InheritedFalse ()
    {
      ResourceManagerSet resourceManager =
          (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), false);

      Assert.That (resourceManager.Count, Is.EqualTo (1));
      Assert.That (resourceManager[0].Name, Is.EqualTo ("OnInherited"));
    }

    [Test]
    public void AttributesOnBaseAndClass_InheritedTrue ()
    {
      ResourceManagerSet resourceManager =
          (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), true);

      Assert.That (resourceManager.Count, Is.EqualTo (2));
      Assert.That (new[] { resourceManager[0].Name, resourceManager[1].Name }, Is.EquivalentTo (new[] { "OnTarget", "OnInherited" }));
    }

    [Test]
    public void AttributesFromMixin_InheritedFalse ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes> ().AddMixin<MixinAddingMultiLingualResourcesAttributes1> ().EnterScope ())
      {
        ResourceManagerSet resourceManager =
            (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (ClassWithoutMultiLingualResourcesAttributes), false);

        Assert.That (resourceManager.Count, Is.EqualTo (1));
        Assert.That (resourceManager[0].Name, Is.EqualTo ("OnMixin1"));
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

        Assert.That (resourceManager.Count, Is.EqualTo (1));
        Assert.That (resourceManager[0].Name, Is.EqualTo ("OnMixin1"));
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

        Assert.That (resourceManager.Count, Is.EqualTo (3));
        Assert.That (
            new[] { resourceManager[0].Name, resourceManager[1].Name, resourceManager[2].Name },
            Is.EquivalentTo (new object[] { "OnMixin1", "OnMixin2a", "OnMixin2b" }));
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

        Assert.That (resourceManager.Count, Is.EqualTo (4));
        Assert.That (
            new[]
            {
                resourceManager[0].Name, resourceManager[1].Name, resourceManager[2].Name, resourceManager[3].Name, resourceManager[3].Name
            },
            Is.EquivalentTo (new[] { "OnInherited", "OnInherited", "OnMixin1", "OnMixin2a", "OnMixin2b" }));
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

        Assert.That (resourceManager.Count, Is.EqualTo (5));
        Assert.That (
            new[]
            {
                resourceManager[0].Name, resourceManager[1].Name, resourceManager[2].Name, resourceManager[3].Name,
                resourceManager[4].Name
            },
            Is.EquivalentTo (new[] { "OnTarget", "OnInherited", "OnMixin1", "OnMixin2a", "OnMixin2b" }));
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

      Assert.That (set4, Is.SameAs (set1));
      Assert.That (set3, Is.SameAs (set2));
      Assert.That (set5, Is.SameAs (set2));
      Assert.That (set2, Is.Not.SameAs (set1));

      Assert.That (((ResourceManagerSet) set1).Count, Is.EqualTo (2));
      Assert.That (((ResourceManagerSet) set2).Count, Is.EqualTo (5));
    }
  }
}
