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
  public class DefaultMethodIntroductionRulesTest : ValidationTestBase
  {
    [Test]
    public void SucceedsIfPrivateIntroducedMethod_HasSameNameAsTargetClassMethod ()
    {
      TargetClassDefinition classDefinition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (TargetClassWithSameNamesAsIntroducedMembers),
              typeof (MixinIntroducingMembersWithDifferentVisibilities));
      MethodIntroductionDefinition definition = classDefinition
          .IntroducedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)]
          .IntroducedMethods[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetMethod ("MethodWithDefaultVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfPublicIntroducedMethod_HasSameNameButDifferentSignatureFromTargetClassMethod ()
    {
      TargetClassDefinition classDefinition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (TargetClassWithSameNamesDifferentSignaturesAsIntroducedMembers),
              typeof (MixinIntroducingMembersWithDifferentVisibilities));
      MethodIntroductionDefinition definition = classDefinition
          .IntroducedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)]
          .IntroducedMethods[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetMethod ("MethodWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void FailsIfPublicIntroducedMethod_HasSameNameAndSignatureAsTargetClassMethod ()
    {
      TargetClassDefinition classDefinition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (TargetClassWithSameNamesAsIntroducedMembers),
              typeof (MixinIntroducingMembersWithDifferentVisibilities));
      MethodIntroductionDefinition definition = classDefinition
          .IntroducedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)]
          .IntroducedMethods[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetMethod ("MethodWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMethodIntroductionRules.PublicMethodNameMustBeUniqueInTargetClass", log));
    }

    [Test]
    public void SucceedsIfPrivateIntroducedMethod_HasSameNameAsOther ()
    {
      TargetClassDefinition classDefinition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities),
              typeof (OtherMixinIntroducingMembersWithDifferentVisibilities));
      MethodIntroductionDefinition definition = classDefinition
          .IntroducedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)]
          .IntroducedMethods[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetMethod ("MethodWithDefaultVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfPublicIntroducedMethod_DoesNotHaveSameNameAsOther ()
    {
      TargetClassDefinition classDefinition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities));
      MethodIntroductionDefinition definition = classDefinition
          .IntroducedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)]
          .IntroducedMethods[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetMethod ("MethodWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfPublicIntroducedMethod_HasSameNameButDifferentSignatureAsOther ()
    {
      TargetClassDefinition classDefinition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities),
              typeof (OtherMixinIntroducingMembersWithPublicVisibilityDifferentSignatures));
      MethodIntroductionDefinition definition = classDefinition
          .IntroducedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)]
          .IntroducedMethods[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetMethod ("MethodWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfPublicIntroducedMethod_HasSameNameAsPrivatgeOther ()
    {
      TargetClassDefinition classDefinition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities),
              typeof (MixinIntroducingMembersWithPrivateVisibilities));
      MethodIntroductionDefinition definition = classDefinition
          .IntroducedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)]
          .IntroducedMethods[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetMethod ("MethodWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void FailsIfPublicIntroducedMethod_HasSameNameAsOther ()
    {
      TargetClassDefinition classDefinition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities),
              typeof (OtherMixinIntroducingMembersWithDifferentVisibilities));
      MethodIntroductionDefinition definition = classDefinition
          .IntroducedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)]
          .IntroducedMethods[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetMethod ("MethodWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMethodIntroductionRules.PublicMethodNameMustBeUniqueInOtherMixins", log));
    }
  }
}
