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
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Context.DeclarativeConfigurationBuilder_IntegrationTests
{
  [TestFixture]
  public class IgnoresMixinTest
  {
    [Test]
    public void BaseClass_HasMixins ()
    {
      Assert.IsTrue (TypeUtility.HasMixin (typeof (BaseClassForDerivedClassIgnoringMixin), typeof (NullMixin)));
      Assert.IsTrue (TypeUtility.HasMixin (typeof (BaseClassForDerivedClassIgnoringMixin), typeof (DerivedNullMixin)));
      Assert.IsTrue (TypeUtility.HasMixin (typeof (BaseClassForDerivedClassIgnoringMixin), typeof (DerivedDerivedNullMixin)));
      Assert.IsTrue (TypeUtility.HasMixin (typeof (BaseClassForDerivedClassIgnoringMixin), typeof (GenericMixinWithVirtualMethod<>)));
      Assert.IsTrue (TypeUtility.HasMixin (typeof (BaseClassForDerivedClassIgnoringMixin), typeof (GenericMixinWithVirtualMethod2<object>)));
    }

    [Test]
    public void DerivedClass_ExcludesMixins ()
    {
      Assert.IsTrue (TypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (NullMixin)));
      Assert.IsFalse (TypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (DerivedNullMixin)));
      Assert.IsFalse (TypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (DerivedDerivedNullMixin)));
      Assert.IsFalse (TypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (GenericMixinWithVirtualMethod<>)));
      Assert.IsFalse (TypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (GenericMixinWithVirtualMethod2<object>)));
    }

    [Test]
    public void DerivedDerivedClass_ExcludesMixins ()
    {
      Assert.IsTrue (TypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (NullMixin)));
      Assert.IsFalse (TypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (DerivedNullMixin)));
      Assert.IsFalse (TypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (DerivedDerivedNullMixin)));
      Assert.IsFalse (TypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (GenericMixinWithVirtualMethod<>)));
      Assert.IsFalse (TypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (GenericMixinWithVirtualMethod2<object>)));
    }
  }
}
