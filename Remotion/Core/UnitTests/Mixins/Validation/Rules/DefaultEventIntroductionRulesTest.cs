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
using NUnit.Framework;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.UnitTests.Mixins.Validation.ValidationSampleTypes;

namespace Remotion.UnitTests.Mixins.Validation.Rules
{
  [TestFixture]
  public class DefaultEventIntroductionRulesTest : ValidationTestBase
  {
    [Test]
    public void SucceedsIfPrivateIntroducedEvent_HasSameNameAsTargetClassEvent ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (TargetClassWithSameNamesAsIntroducedMembers),
              typeof (MixinIntroducingMembersWithDifferentVisibilities));
      EventIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedEvents[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetEvent ("EventWithDefaultVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void FailsIfPublicIntroducedEvent_HasSameNameButDifferentSignatureFromTargetClassEvent ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (TargetClassWithSameNamesDifferentSignaturesAsIntroducedMembers),
              typeof (MixinIntroducingMembersWithDifferentVisibilities));
      EventIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedEvents[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetEvent ("EventWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultEventIntroductionRules.PublicEventNameMustBeUniqueInTargetClass", log));
    }

    [Test]
    public void FailsIfPublicIntroducedEvent_HasSameNameAndSignatureAsTargetClassEvent ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (TargetClassWithSameNamesAsIntroducedMembers),
              typeof (MixinIntroducingMembersWithDifferentVisibilities));
      EventIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedEvents[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetEvent ("EventWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultEventIntroductionRules.PublicEventNameMustBeUniqueInTargetClass", log));
    }

    [Test]
    public void SucceedsIfPrivateIntroducedEvent_HasSameNameAsOther ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities),
              typeof (OtherMixinIntroducingMembersWithDifferentVisibilities));
      EventIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedEvents[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetEvent ("EventWithDefaultVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfPublicIntroducedEvent_DoesNotHaveSameNameAsOther ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities));
      EventIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedEvents[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetEvent ("EventWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfPublicIntroducedEvent_HasSameNameAsPrivateOther ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities),
              typeof (MixinIntroducingMembersWithPrivateVisibilities));
      EventIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedEvents[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetEvent ("EventWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      AssertSuccess (log);
    }


    [Test]
    public void FailsIfPublicIntroducedEvent_HasSameNameButDifferentSignatureAsOther ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities),
              typeof (OtherMixinIntroducingMembersWithPublicVisibilityDifferentSignatures));
      EventIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedEvents[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetEvent ("EventWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultEventIntroductionRules.PublicEventNameMustBeUniqueInOtherMixins", log));
    }

    [Test]
    public void FailsIfPublicIntroducedEvent_HasSameNameAsOther ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities),
              typeof (OtherMixinIntroducingMembersWithDifferentVisibilities));
      EventIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedEvents[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetEvent ("EventWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultEventIntroductionRules.PublicEventNameMustBeUniqueInOtherMixins", log));
    }
  }
}
