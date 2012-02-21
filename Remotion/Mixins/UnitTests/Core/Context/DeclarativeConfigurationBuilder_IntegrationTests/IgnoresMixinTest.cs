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
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.Context.DeclarativeConfigurationBuilder_IntegrationTests
{
  [TestFixture]
  public class IgnoresMixinTest
  {
    [Test]
    public void BaseClass_HasMixins ()
    {
      Assert.IsTrue (MixinTypeUtility.HasMixin (typeof (BaseClassForDerivedClassIgnoringMixin), typeof (NullMixin)));
      Assert.IsTrue (MixinTypeUtility.HasMixin (typeof (BaseClassForDerivedClassIgnoringMixin), typeof (DerivedNullMixin)));
      Assert.IsTrue (MixinTypeUtility.HasMixin (typeof (BaseClassForDerivedClassIgnoringMixin), typeof (DerivedDerivedNullMixin)));
      Assert.IsTrue (MixinTypeUtility.HasMixin (typeof (BaseClassForDerivedClassIgnoringMixin), typeof (GenericMixinWithVirtualMethod<>)));
      Assert.IsTrue (MixinTypeUtility.HasMixin (typeof (BaseClassForDerivedClassIgnoringMixin), typeof (GenericMixinWithVirtualMethod2<object>)));
    }

    [Test]
    public void DerivedClass_ExcludesMixins ()
    {
      Assert.IsTrue (MixinTypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (NullMixin)));
      Assert.IsFalse (MixinTypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (DerivedNullMixin)));
      Assert.IsFalse (MixinTypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (DerivedDerivedNullMixin)));
      Assert.IsFalse (MixinTypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (GenericMixinWithVirtualMethod<>)));
      Assert.IsFalse (MixinTypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (GenericMixinWithVirtualMethod2<object>)));
    }

    [Test]
    public void DerivedDerivedClass_ExcludesMixins ()
    {
      Assert.IsTrue (MixinTypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (NullMixin)));
      Assert.IsFalse (MixinTypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (DerivedNullMixin)));
      Assert.IsFalse (MixinTypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (DerivedDerivedNullMixin)));
      Assert.IsFalse (MixinTypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (GenericMixinWithVirtualMethod<>)));
      Assert.IsFalse (MixinTypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (GenericMixinWithVirtualMethod2<object>)));
    }
  }
}
