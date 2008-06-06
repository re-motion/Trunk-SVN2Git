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
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.Globalization;
using Remotion.UnitTests.Mixins.Globalization.SampleTypes;

namespace Remotion.UnitTests.Mixins.Globalization.MixedMultiLingualResourcesTests
{
  [TestFixture]
  public class ExistsResourceTest
  {
    [Test]
    public void NoAttribute ()
    {
      Assert.IsFalse (MixedMultiLingualResources.ExistsResource (typeof (ClassWithoutMultiLingualResourcesAttributes)));
    }

    [Test]
    public void AttributesOnClass ()
    {
      Assert.IsTrue (MixedMultiLingualResources.ExistsResource (typeof (ClassWithMultiLingualResourcesAttributes)));
    }

    [Test]
    public void AttributesOnBase ()
    {
      Assert.IsTrue (MixedMultiLingualResources.ExistsResource (typeof (InheritedClassWithoutMultiLingualResourcesAttributes)));
    }

    [Test]
    public void AttributesOnBaseAndClass ()
    {
      Assert.IsTrue (MixedMultiLingualResources.ExistsResource (typeof (InheritedClassWithMultiLingualResourcesAttributes)));
    }

    [Test]
    public void AttributesFromMixin_InheritedFalse ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes> ().AddMixin<MixinAddingMultiLingualResourcesAttributes1> ().EnterScope ())
      {
        Assert.IsTrue (MixedMultiLingualResources.ExistsResource (typeof (ClassWithoutMultiLingualResourcesAttributes)));
      }
    }

    [Test]
    public void AttributesFromMixinsAndBaseAndClass ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<InheritedClassWithMultiLingualResourcesAttributes> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes2> ()
          .EnterScope ())
      {
        Assert.IsTrue (MixedMultiLingualResources.ExistsResource (typeof (InheritedClassWithMultiLingualResourcesAttributes)));
      }
    }

    [Test]
    public void AttributesFromMixins ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes2> ()
          .EnterScope ())
      {
        Assert.IsTrue (MixedMultiLingualResources.ExistsResource (typeof (ClassWithoutMultiLingualResourcesAttributes)));
      }
    }
  }
}
