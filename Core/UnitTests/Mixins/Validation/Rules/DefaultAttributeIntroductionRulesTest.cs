/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using NUnit.Framework;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.UnitTests.Mixins.Validation.ValidationSampleTypes;

namespace Remotion.UnitTests.Mixins.Validation.Rules
{
  [TestFixture]
  public class DefaultAttributeIntroductionRulesTest : ValidationTestBase
  {
    [Test]
    public void SucceedsIfTargetClassWinsWhenDefiningAttributes ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1),
          typeof (MixinAddingBT1Attribute));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }

    [Test]
    public void FailsTwiceIfDuplicateAttributeAddedByMixin ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType2), typeof (MixinAddingBT1Attribute),
          typeof (MixinAddingBT1Attribute2));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure (
          "Remotion.Mixins.Validation.Rules.DefaultAttributeIntroductionRules.AllowMultipleRequiredIfAttributeIntroducedMultipleTimes", log));
      Assert.AreEqual (2, log.GetNumberOfFailures ());
    }

    [Test]
    public void FailsTwiceIfDuplicateAttributeAddedByMixinToMember ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassWithVirtualMethod),
          typeof (MixinAddingBT1AttributeToMember), typeof (MixinAddingBT1AttributeToMember2));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (
          HasFailure (
              "Remotion.Mixins.Validation.Rules.DefaultAttributeIntroductionRules.AllowMultipleRequiredIfAttributeIntroducedMultipleTimes", log));
      Assert.AreEqual (2, log.GetNumberOfFailures ());
    }

    [Test]
    public void SucceedsIfDuplicateAttributeAddedByMixinAllowsMultiple ()
    {
      TargetClassDefinition definition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseTypeWithAllowMultiple), typeof (MixinAddingAllowMultipleToClassAndMember));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }
  }
}
