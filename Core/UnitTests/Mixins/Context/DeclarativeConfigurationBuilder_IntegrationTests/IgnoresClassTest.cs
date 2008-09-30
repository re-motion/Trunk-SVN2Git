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
  public class IgnoresClassTest
  {
    [Test]
    public void IgnoredClass_IsExcluded ()
    {
      Assert.IsFalse (MixinTypeUtility.HasMixin (typeof (DerivedClassIgnoredByMixins), typeof (MixinIgnoringDerivedClass)));
    }

    [Test]
    public void BaseClass_IsNotExcluded ()
    {
      Assert.IsTrue (MixinTypeUtility.HasMixin (typeof (BaseClassForDerivedClassIgnoredByMixin), typeof (MixinIgnoringDerivedClass)));
    }

    [Test]
    public void DerivedClass_IsExcluded ()
    {
      Assert.IsFalse (MixinTypeUtility.HasMixin (typeof (DerivedDerivedClassIgnoredByMixin), typeof (MixinIgnoringDerivedClass)));
    }

    // The following test does not work because GenericClassForMixinIgnoringDerivedClass<int> inherits the mixin from its base class
    // (BaseClassForDerivedClassIgnoredByMixin) even though its generic type definition (GCFMIDC<>) ignores the mixin.
    // At the moment, this is by design due to the rule that a closed generic type inherits mixins from both its base class and its generic
    // type definition.
    //[Test]
    //public void GenericSpecialization_IsExcluded ()
    //{
    //  Assert.IsFalse (MixinTypeUtility.HasMixin (typeof (GenericClassForMixinIgnoringDerivedClass<int>), typeof (MixinIgnoringDerivedClass)));
    //}

    [Test]
    public void ClosedGenericSpecialization_IsExcluded ()
    {
      Assert.IsFalse (MixinTypeUtility.HasMixin (typeof (ClosedGenericClassForMixinIgnoringDerivedClass<int>), typeof (MixinIgnoringDerivedClass)));
    }

    [Test]
    public void ClosedGenericSpecializationVariant_IsNotExcluded ()
    {
      Assert.IsFalse (MixinTypeUtility.HasMixin (typeof (ClosedGenericClassForMixinIgnoringDerivedClass<string>), typeof (MixinIgnoringDerivedClass)));
    }
  }
}
