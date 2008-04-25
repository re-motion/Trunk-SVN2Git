using System;
using NUnit.Framework;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.UnitTests.Mixins.ValidationTests.ValidationSampleTypes;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;

namespace Remotion.UnitTests.Mixins.ValidationTests.Rules
{
  [TestFixture]
  public class DefaultBaseDependencyRulesTest : ValidationTestBase
  {
    [Test]
    public void FailsIfEmptyBaseDependencyNotFulfilled ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (MixinWithUnsatisfiedEmptyBaseDependency));
      DefaultValidationLog log = Validator.Validate (
          definition.Mixins[typeof (MixinWithUnsatisfiedEmptyBaseDependency)].
              BaseDependencies[typeof (IEmptyInterface)]);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultBaseDependencyRules.DependencyMustBeSatisfied", log));
    }

    [Test]
    public void SucceedsIfDuckBaseDependency ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassFulfillingAllMemberRequirementsDuck),
          typeof (MixinRequiringAllMembersBase));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfAggregateBaseDependencyIsFullyImplemented ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin4), typeof (BT3Mixin7Base));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }

    [Test]
    public void FailsIfEmptyAggregateBaseDependencyIsNotAvailable ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (NullTarget), typeof (MixinWithUnsatisfiedEmptyAggregateBaseDependency));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultBaseDependencyRules.DependencyMustBeSatisfied", log));
    }
  }
}