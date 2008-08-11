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
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.UnitTests.Mixins.ValidationTests.ValidationSampleTypes;

namespace Remotion.UnitTests.Mixins.ValidationTests.Rules
{
  [TestFixture]
  public class DefaultPropertyIntroductionRulesTest : ValidationTestBase
  {
    [Test]
    public void SucceedsIfPrivateIntroducedProperty_HasSameNameAsTargetClassProperty ()
    {
      TargetClassDefinition classDefinition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (TargetClassWithSameNamesAsIntroducedMembers),
              typeof (MixinIntroducingMembersWithDifferentVisibilities));
      PropertyIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedProperties[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetProperty ("PropertyWithDefaultVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void FailsIfPublicIntroducedProperty_HasSameNameButDifferentSignatureFromTargetClassProperty ()
    {
      TargetClassDefinition classDefinition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (TargetClassWithSameNamesDifferentSignaturesAsIntroducedMembers),
              typeof (MixinIntroducingMembersWithDifferentVisibilities));
      PropertyIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedProperties[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetProperty ("PropertyWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultPropertyIntroductionRules.PublicPropertyNameMustBeUniqueInTargetClass", log));
    }

    [Test]
    public void FailsIfPublicIntroducedProperty_HasSameNameAndSignatureAsTargetClassProperty ()
    {
      TargetClassDefinition classDefinition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (TargetClassWithSameNamesAsIntroducedMembers),
              typeof (MixinIntroducingMembersWithDifferentVisibilities));
      PropertyIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedProperties[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetProperty ("PropertyWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultPropertyIntroductionRules.PublicPropertyNameMustBeUniqueInTargetClass", log));
    }

    [Test]
    public void SucceedsIfPrivateIntroducedProperty_HasSameNameAsOther ()
    {
      TargetClassDefinition classDefinition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities),
              typeof (OtherMixinIntroducingMembersWithDifferentVisibilities));
      PropertyIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedProperties[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetProperty ("PropertyWithDefaultVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfPublicIntroducedProperty_DoesNotHaveSameNameAsOther ()
    {
      TargetClassDefinition classDefinition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities));
      PropertyIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedProperties[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetProperty ("PropertyWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfPublicIntroducedProperty_HasSameNameAsPrivateOther ()
    {
      TargetClassDefinition classDefinition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities),
              typeof (MixinIntroducingMembersWithPrivateVisibilities));
      PropertyIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedProperties[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetProperty ("PropertyWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      AssertSuccess (log);
    }


    [Test]
    public void FailsIfPublicIntroducedProperty_HasSameNameButDifferentSignatureAsOther ()
    {
      TargetClassDefinition classDefinition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities),
              typeof (OtherMixinIntroducingMembersWithPublicVisibilityDifferentSignatures));
      PropertyIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedProperties[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetProperty ("PropertyWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultPropertyIntroductionRules.PublicPropertyNameMustBeUniqueInOtherMixins", log));
    }

    [Test]
    public void FailsIfPublicIntroducedProperty_HasSameNameAsOther ()
    {
      TargetClassDefinition classDefinition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities),
              typeof (OtherMixinIntroducingMembersWithDifferentVisibilities));
      PropertyIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedProperties[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetProperty ("PropertyWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultPropertyIntroductionRules.PublicPropertyNameMustBeUniqueInOtherMixins", log));
    }
  }
}
