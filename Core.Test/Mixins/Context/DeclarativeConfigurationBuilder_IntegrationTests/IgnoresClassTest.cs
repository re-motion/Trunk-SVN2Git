using System;
using NUnit.Framework;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.Context.DeclarativeConfigurationBuilder_IntegrationTests
{
  [TestFixture]
  public class IgnoresClassTest
  {
    [Test]
    public void IgnoredClass_IsExcluded ()
    {
      Assert.IsFalse (TypeUtility.HasMixin (typeof (DerivedClassIgnoredByMixins), typeof (MixinIgnoringDerivedClass)));
    }

    [Test]
    public void BaseClass_IsNotExcluded ()
    {
      Assert.IsTrue (TypeUtility.HasMixin (typeof (BaseClassForDerivedClassIgnoredByMixin), typeof (MixinIgnoringDerivedClass)));
    }

    [Test]
    public void DerivedClass_IsExcluded ()
    {
      Assert.IsFalse (TypeUtility.HasMixin (typeof (DerivedDerivedClassIgnoredByMixin), typeof (MixinIgnoringDerivedClass)));
    }

    // The following test does not work because GenericClassForMixinIgnoringDerivedClass<int> inherits the mixin from its base class
    // (BaseClassForDerivedClassIgnoredByMixin) even though its generic type definition (GCFMIDC<>) ignores the mixin.
    // At the moment, this is by design due to the rule that a closed generic type inherits mixins from both its base class and its generic
    // type definition.
    //[Test]
    //public void GenericSpecialization_IsExcluded ()
    //{
    //  Assert.IsFalse (TypeUtility.HasMixin (typeof (GenericClassForMixinIgnoringDerivedClass<int>), typeof (MixinIgnoringDerivedClass)));
    //}

    [Test]
    public void ClosedGenericSpecialization_IsExcluded ()
    {
      Assert.IsFalse (TypeUtility.HasMixin (typeof (ClosedGenericClassForMixinIgnoringDerivedClass<int>), typeof (MixinIgnoringDerivedClass)));
    }

    [Test]
    public void ClosedGenericSpecializationVariant_IsNotExcluded ()
    {
      Assert.IsFalse (TypeUtility.HasMixin (typeof (ClosedGenericClassForMixinIgnoringDerivedClass<string>), typeof (MixinIgnoringDerivedClass)));
    }
  }
}