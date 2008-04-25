using System;
using NUnit.Framework;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.UnitTests.Mixins.ValidationTests.ValidationSampleTypes;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;

namespace Remotion.UnitTests.Mixins.ValidationTests.Rules
{
  [TestFixture]
  public class DefaultThisDependencyRulesTest : ValidationTestBase
  {
    [Test]
    public void SucceedsIfEmptyThisDependencyNotFulfilled ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (MixinWithUnsatisfiedEmptyThisDependency));
      DefaultValidationLog log = Validator.Validate (
          definition.Mixins[typeof (MixinWithUnsatisfiedEmptyThisDependency)].
              ThisDependencies[typeof (IEmptyInterface)]);

      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfCircularThisDependency ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (MixinWithCircularThisDependency1), typeof (MixinWithCircularThisDependency2));
      DefaultValidationLog log = Validator.Validate (
          definition.Mixins[typeof (MixinWithCircularThisDependency1)]);

      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfDuckThisDependency ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassFulfillingAllMemberRequirementsDuck),
          typeof (MixinRequiringAllMembersFace));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfAggregateThisDependencyIsFullyImplemented ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin4), typeof (BT3Mixin7Face));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }


    [Test]
    public void SucceedsIfEmptyAggregateThisDependencyIsNotAvailable ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (NullTarget), typeof (MixinWithUnsatisfiedEmptyAggregateThisDependency));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }


  }
}