using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.UnitTests.Mixins.Globalization.SampleTypes;
using Remotion.Globalization;
using Remotion.Mixins;
using Remotion.Mixins.Globalization;

namespace Remotion.UnitTests.Mixins.Globalization.MixedMultiLingualResourcesTests
{
  [TestFixture]
  public class GetResourceManagerTest
  {
    [Test]
    [ExpectedException (typeof (ResourceException), ExpectedMessage = "Type Remotion.UnitTests.Mixins.Globalization.SampleTypes."
        + "ClassWithoutMultiLingualResourcesAttributes and its base classes do not define the attribute MultiLingualResourcesAttribute.")]
    public void NoAttributes_NoInheritance ()
    {
      MixedMultiLingualResources.GetResourceManager (typeof (ClassWithoutMultiLingualResourcesAttributes), false);
    }

    [Test]
    [ExpectedException (typeof (ResourceException), ExpectedMessage = "Type Remotion.UnitTests.Mixins.Globalization.SampleTypes."
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

        Assert.AreEqual (1, resourceManager.Count);
        Assert.AreEqual ("OnInherited", resourceManager[0].Name);
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