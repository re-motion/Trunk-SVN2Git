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
  public class DefaultMethodIntroductionRulesTest : ValidationTestBase
  {
    [Test]
    public void SucceedsIfPrivateIntroducedMethod_HasSameNameAsTargetClassMethod ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (TargetClassWithSameNamesAsIntroducedMembers),
              typeof (MixinIntroducingMembersWithDifferentVisibilities));
      MethodIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedMethods[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetMethod ("MethodWithDefaultVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfPublicIntroducedMethod_HasSameNameButDifferentSignatureFromTargetClassMethod ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (TargetClassWithSameNamesDifferentSignaturesAsIntroducedMembers),
              typeof (MixinIntroducingMembersWithDifferentVisibilities));
      MethodIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedMethods[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetMethod ("MethodWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void FailsIfPublicIntroducedMethod_HasSameNameAndSignatureAsTargetClassMethod ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (TargetClassWithSameNamesAsIntroducedMembers),
              typeof (MixinIntroducingMembersWithDifferentVisibilities));
      MethodIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedMethods[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetMethod ("MethodWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMethodIntroductionRules.PublicMethodNameMustBeUniqueInTargetClass", log));
    }

    [Test]
    public void SucceedsIfPrivateIntroducedMethod_HasSameNameAsOther ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities),
              typeof (OtherMixinIntroducingMembersWithDifferentVisibilities));
      MethodIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedMethods[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetMethod ("MethodWithDefaultVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfPublicIntroducedMethod_DoesNotHaveSameNameAsOther ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities));
      MethodIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedMethods[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetMethod ("MethodWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfPublicIntroducedMethod_HasSameNameAsPrivateOther ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities),
              typeof (MixinIntroducingMembersWithPrivateVisibilities));
      MethodIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedMethods[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetMethod ("MethodWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfPublicIntroducedMethod_HasSameNameButDifferentSignatureAsOther ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities),
              typeof (OtherMixinIntroducingMembersWithPublicVisibilityDifferentSignatures));
      MethodIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedMethods[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetMethod ("MethodWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void FailsIfPublicIntroducedMethod_HasSameNameAsOther ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities),
              typeof (OtherMixinIntroducingMembersWithDifferentVisibilities));
      MethodIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedMethods[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetMethod ("MethodWithPublicVisibility")];

      DefaultValidationLog log = Validator.Validate (definition);
      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMethodIntroductionRules.PublicMethodNameMustBeUniqueInOtherMixins", log));
    }
  }
}
