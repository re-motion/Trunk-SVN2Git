// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.UnitTests.Mixins.Validation.ValidationSampleTypes;

namespace Remotion.UnitTests.Mixins.Validation.Rules
{
  [TestFixture]
  public class DefaultMixinRulesTest : ValidationTestBase
  {
    [Test]
    public void FailsIfMixinAppliedToItself ()
    {
      TargetClassDefinition bc = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (object), typeof (object));
      DefaultValidationLog log = Validator.Validate (bc);
      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMixinRules.MixinCannotMixItself", log));
    }

    [Test]
    public void FailsIfMixinAppliedToItsBase ()
    {
      TargetClassDefinition bc = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (object), typeof (NullMixin));
      DefaultValidationLog log = Validator.Validate (bc);
      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMixinRules.MixinCannotMixItsBase", log));
    }

    [Test]
    public void FailsIfMixinIsInterface ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (IBT1Mixin1));
      DefaultValidationLog log = Validator.Validate (definition.Mixins[typeof (IBT1Mixin1)]);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMixinRules.MixinCannotBeInterface", log));
    }

    [Test]
    public void FailsIfMixinNonPublic ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType5), typeof (BT5Mixin2));
      DefaultValidationLog log = Validator.Validate (definition.Mixins[typeof (BT5Mixin2)]);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMixinRules.MixinMustBePublic", log));
    }

    [Test]
    public void SucceedsIfNestedPublicMixin ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (PublicNester.PublicNested));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }

    [Test]
    public void FailsIfNestedPublicMixinInNonPublic ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (InternalNester.PublicNested));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMixinRules.MixinMustBePublic", log));
    }

    [Test]
    public void FailsIfNestedPrivateMixin ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (PublicNester.InternalNested));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMixinRules.MixinMustBePublic", log));
    }

    [Test]
    public void FailsIfNestedPrivateMixinInNonPublic ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (InternalNester.InternalNested));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMixinRules.MixinMustBePublic", log));
    }

    [Test]
    public void FailsIfNoPublicOrProtectedDefaultCtorInMixinClassWithOverriddenMembers ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassOverridingSingleMixinMethod),
          typeof (MixinWithPrivateCtorAndVirtualMethod));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMixinRules.MixinWithOverriddenMembersMustHavePublicOrProtectedDefaultCtor",
          log));
    }

    [Test]
    public void FailsIfNoMixinBaseWith ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassWithVirtualMethod),
          typeof (MixinWithProtectedOverriderWithoutMixinBase));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMixinRules.MixinNeedingDerivedTypeMustBeDerivedFromMixinBase",
          log));
    }

    [Test]
    public void SucceedsIfNoPublicOrProtectedDefaultCtorInMixinClassWithoutOverriddenMembers ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (NullTarget),
          typeof (MixinWithPrivateCtorAndVirtualMethod));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }

  }
}
