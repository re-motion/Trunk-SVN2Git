using System;
using NUnit.Framework;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.UnitTests.Mixins.ValidationTests.ValidationSampleTypes;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;

namespace Remotion.UnitTests.Mixins.ValidationTests.Rules
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