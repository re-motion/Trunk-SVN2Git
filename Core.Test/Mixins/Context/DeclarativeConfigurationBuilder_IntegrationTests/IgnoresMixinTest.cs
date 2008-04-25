using System;
using NUnit.Framework;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins;

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