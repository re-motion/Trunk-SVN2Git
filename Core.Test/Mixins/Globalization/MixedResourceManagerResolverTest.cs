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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.UnitTests.Mixins.Globalization.SampleTypes;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Globalization;
using Remotion.Mixins;
using Remotion.Mixins.Globalization;
using Remotion.Utilities;

namespace Remotion.UnitTests.Mixins.Globalization
{
  [TestFixture]
  public class MixedResourceManagerResolverTest
  {
    private MixedResourceManagerResolver<MultiLingualResourcesAttribute> _resolver;

    [SetUp]
    public void SetUp ()
    {
      _resolver = new MixedResourceManagerResolver<MultiLingualResourcesAttribute> ();
    }

    [Test]
    public void FindFirstResourceDefinitions_SuccessOnSameType ()
    {
      Type definingType;
      MultiLingualResourcesAttribute[] attributes;

      _resolver.FindFirstResourceDefinitions (typeof (ClassWithMultiLingualResourcesAttributes), false, out definingType, out attributes);
      Assert.AreSame (typeof (ClassWithMultiLingualResourcesAttributes), definingType);
      Assert.AreEqual (1, attributes.Length);
      Assert.That (attributes, Is.EquivalentTo (
          AttributeUtility.GetCustomAttributes<MultiLingualResourcesAttribute> (typeof (ClassWithMultiLingualResourcesAttributes), false)));
    }

    [Test]
    public void FindFirstResourceDefinitions_SuccessOnSameType_WithMixins ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<ClassWithMultiLingualResourcesAttributes> ().AddMixin<MixinAddingMultiLingualResourcesAttributes1> ().EnterScope ())
      {
        Type definingType;
        MultiLingualResourcesAttribute[] attributes;

        _resolver.FindFirstResourceDefinitions (typeof (ClassWithMultiLingualResourcesAttributes), false, out definingType, out attributes);
        Assert.AreSame (typeof (ClassWithMultiLingualResourcesAttributes), definingType);
        Assert.AreEqual (1, attributes.Length);
        Assert.That (attributes, Is.EquivalentTo (
            AttributeUtility.GetCustomAttributes<MultiLingualResourcesAttribute> (typeof (ClassWithMultiLingualResourcesAttributes), false)));
      }
    }

    [Test]
    public void FindFirstResourceDefinitions_SuccessOnMixinType ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes> ().AddMixin<MixinAddingMultiLingualResourcesAttributes1> ().EnterScope ())
      {
        Type definingType;
        MultiLingualResourcesAttribute[] attributes;

        _resolver.FindFirstResourceDefinitions (typeof (ClassWithoutMultiLingualResourcesAttributes), false, out definingType, out attributes);
        Assert.AreSame (typeof (MixinAddingMultiLingualResourcesAttributes1), definingType);
        Assert.AreEqual (1, attributes.Length);
        Assert.That (attributes, Is.EquivalentTo (
            AttributeUtility.GetCustomAttributes<MultiLingualResourcesAttribute> (typeof (MixinAddingMultiLingualResourcesAttributes1), false)));
      }
    }

    [Test]
    public void FindFirstResourceDefinitions_SuccessOnMixinType_NotBase ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<InheritedClassWithoutMultiLingualResourcesAttributes> ().AddMixin<MixinAddingMultiLingualResourcesAttributes1> ().EnterScope ())
      {
        Type definingType;
        MultiLingualResourcesAttribute[] attributes;

        _resolver.FindFirstResourceDefinitions (typeof (InheritedClassWithoutMultiLingualResourcesAttributes), false, out definingType, out attributes);
        Assert.AreSame (typeof (MixinAddingMultiLingualResourcesAttributes1), definingType);
        Assert.AreEqual (1, attributes.Length);
        Assert.That (attributes, Is.EquivalentTo (
            AttributeUtility.GetCustomAttributes<MultiLingualResourcesAttribute> (typeof (MixinAddingMultiLingualResourcesAttributes1), false)));
      }
    }

    [Test]
    public void FindFirstResourceDefinitions_NoSuccessNoException ()
    {
      Type definingType;
      MultiLingualResourcesAttribute[] attributes;

      _resolver.FindFirstResourceDefinitions (typeof (ClassWithoutMultiLingualResourcesAttributes), true, out definingType, out attributes);
      Assert.IsNull (definingType);
      Assert.IsEmpty (attributes);
    }

    [Test]
    public void FindFirstResourceDefinitions_NoSuccessNoExceptionButMixins ()
    {
      Type definingType;
      MultiLingualResourcesAttribute[] attributes;

      using (MixinConfiguration.BuildNew().ForClass<ClassWithoutMultiLingualResourcesAttributes>().AddMixin<NullMixin>().EnterScope())
      _resolver.FindFirstResourceDefinitions (typeof (ClassWithoutMultiLingualResourcesAttributes), true, out definingType, out attributes);
      Assert.IsNull (definingType);
      Assert.IsEmpty (attributes);
    }

    [Test]
    [ExpectedException (typeof (ResourceException), ExpectedMessage = "Type Remotion.UnitTests.Mixins.Globalization.SampleTypes."
        + "ClassWithoutMultiLingualResourcesAttributes and its base classes do not define the attribute MultiLingualResourcesAttribute.")]
    public void FindFirstResourceDefinitions_NoSuccessException ()
    {
      Type definingType;
      MultiLingualResourcesAttribute[] attributes;

      _resolver.FindFirstResourceDefinitions (typeof (ClassWithoutMultiLingualResourcesAttributes), false, out definingType, out attributes);
    }

    [Test]
    public void GetResourceManager_NoDefiningType_NoHierarchy ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<InheritedClassWithMultiLingualResourcesAttributes> ().AddMixin<MixinAddingMultiLingualResourcesAttributes1> ().EnterScope ())
      {
        ResourceManagerSet resourceManagerSet =
            (ResourceManagerSet) _resolver.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), false);
        Assert.AreEqual (1, resourceManagerSet.Count);
        Assert.AreEqual ("OnInherited", resourceManagerSet[0].Name);
      }
    }

    [Test]
    public void GetResourceManager_NoDefiningType_WithHierarchy ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<InheritedClassWithMultiLingualResourcesAttributes> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes2> ()
          .EnterScope ())
      {
        ResourceManagerSet resourceManagerSet =
            (ResourceManagerSet) _resolver.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), true);
        Assert.AreEqual (5, resourceManagerSet.Count);
        Set<string> names = new Set<string> (resourceManagerSet[0].Name, resourceManagerSet[1].Name, resourceManagerSet[2].Name,
            resourceManagerSet[3].Name, resourceManagerSet[4].Name);
        Assert.That (names, Is.EquivalentTo (new string[] { "OnTarget", "OnInherited", "OnMixin1", "OnMixin2a", "OnMixin2b" }));
      }
    }

    [Test]
    [ExpectedException (typeof (ResourceException), ExpectedMessage = "Type Remotion.UnitTests.Mixins.Globalization.SampleTypes."
        + "ClassWithoutMultiLingualResourcesAttributes and its base classes do not define the attribute MultiLingualResourcesAttribute.")]
    public void GetResourceManager_NoDefiningType_NoSuccess ()
    {
      _resolver.GetResourceManager (typeof (ClassWithoutMultiLingualResourcesAttributes), true);
    }

    [Test]
    [ExpectedException (typeof (MissingManifestResourceException), ExpectedMessage = "Could not find any resources appropriate for the specified "
        + "culture or the neutral culture.  Make sure \"OnTarget.resources\" was correctly embedded or linked into assembly "
        + "\"Remotion.UnitTests\" at compile time, or that all the satellite assemblies required are loadable and fully signed.")]
    public void GetResourceManager_ForGeneratedType_GetString ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<ClassWithMultiLingualResourcesAttributes> ().AddMixin<NullMixin>().EnterScope ())
      {
        IResourceManager resourceManager = _resolver.GetResourceManager (TypeFactory.GetConcreteType (typeof (ClassWithMultiLingualResourcesAttributes)), true);
        resourceManager.GetString ("Foo");
      }
    }
  }
}
