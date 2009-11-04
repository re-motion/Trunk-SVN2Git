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
