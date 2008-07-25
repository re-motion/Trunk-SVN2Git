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
  public class DefaultEventIntroductionRulesTest : ValidationTestBase
  {
    [Test]
    public void SucceedsIfPrivateIntroducedEvent_HasSameNameAsTargetClassEvent ()
    {
      TargetClassDefinition classDefinition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (TargetClassWithSameNamesAsIntroducedMembers),
              typeof (MixinIntroducingMembersWithDifferentVisibilities));
      EventIntroductionDefinition definition = classDefinition
          .ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)]
          .IntroducedEvents[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetEvent ("EventWithDefaultVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void FailsIfPublicIntroducedEvent_HasSameNameButDifferentSignatureFromTargetClassEvent ()
    {
      TargetClassDefinition classDefinition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (TargetClassWithSameNamesDifferentSignaturesAsIntroducedMembers),
              typeof (MixinIntroducingMembersWithDifferentVisibilities));
      EventIntroductionDefinition definition = classDefinition
          .ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)]
          .IntroducedEvents[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetEvent ("EventWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultEventIntroductionRules.PublicEventNameMustBeUniqueInTargetClass", log));
    }

    [Test]
    public void FailsIfPublicIntroducedEvent_HasSameNameAndSignatureAsTargetClassEvent ()
    {
      TargetClassDefinition classDefinition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (TargetClassWithSameNamesAsIntroducedMembers),
              typeof (MixinIntroducingMembersWithDifferentVisibilities));
      EventIntroductionDefinition definition = classDefinition
          .ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)]
          .IntroducedEvents[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetEvent ("EventWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultEventIntroductionRules.PublicEventNameMustBeUniqueInTargetClass", log));
    }

    [Test]
    public void SucceedsIfPrivateIntroducedEvent_HasSameNameAsOther ()
    {
      TargetClassDefinition classDefinition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities),
              typeof (OtherMixinIntroducingMembersWithDifferentVisibilities));
      EventIntroductionDefinition definition = classDefinition
          .ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)]
          .IntroducedEvents[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetEvent ("EventWithDefaultVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfPublicIntroducedEvent_DoesNotHaveSameNameAsOther ()
    {
      TargetClassDefinition classDefinition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities));
      EventIntroductionDefinition definition = classDefinition
          .ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)]
          .IntroducedEvents[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetEvent ("EventWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfPublicIntroducedEvent_HasSameNameAsPrivateOther ()
    {
      TargetClassDefinition classDefinition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities),
              typeof (MixinIntroducingMembersWithPrivateVisibilities));
      EventIntroductionDefinition definition = classDefinition
          .ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)]
          .IntroducedEvents[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetEvent ("EventWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      AssertSuccess (log);
    }


    [Test]
    public void FailsIfPublicIntroducedEvent_HasSameNameButDifferentSignatureAsOther ()
    {
      TargetClassDefinition classDefinition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities),
              typeof (OtherMixinIntroducingMembersWithPublicVisibilityDifferentSignatures));
      EventIntroductionDefinition definition = classDefinition
          .ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)]
          .IntroducedEvents[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetEvent ("EventWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultEventIntroductionRules.PublicEventNameMustBeUniqueInOtherMixins", log));
    }

    [Test]
    public void FailsIfPublicIntroducedEvent_HasSameNameAsOther ()
    {
      TargetClassDefinition classDefinition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities),
              typeof (OtherMixinIntroducingMembersWithDifferentVisibilities));
      EventIntroductionDefinition definition = classDefinition
          .ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)]
          .IntroducedEvents[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetEvent ("EventWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultEventIntroductionRules.PublicEventNameMustBeUniqueInOtherMixins", log));
    }
  }
}
