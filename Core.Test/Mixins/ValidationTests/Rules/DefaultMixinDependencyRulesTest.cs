using System;
using NUnit.Framework;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;

namespace Remotion.UnitTests.Mixins.ValidationTests.Rules
{
  [TestFixture]
  public class DefaultMixinDependencyRulesTest : ValidationTestBase
  {
    [Test]
    public void FailsIfClassMixinDependencyNotFulfilled ()
    {
      ClassContext context = new ClassContextBuilder (typeof (TargetClassWithAdditionalDependencies)).AddMixin<MixinWithAdditionalClassDependency> ().WithDependency<MixinWithNoAdditionalDependency> ().BuildClassContext ();

      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (context);
      DefaultValidationLog log = Validator.Validate (definition.Mixins[typeof (MixinWithAdditionalClassDependency)]);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMixinDependencyRules.DependencyMustBeSatisfiedByAnotherMixin", log));
    }

    [Test]
    public void FailsIfInterfaceMixinDependencyNotFulfilled ()
    {
      ClassContext context = new ClassContextBuilder (typeof (TargetClassWithAdditionalDependencies)).AddMixin<MixinWithAdditionalInterfaceDependency> ().WithDependency<IMixinWithAdditionalClassDependency> ().BuildClassContext ();

      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (context);
      DefaultValidationLog log = Validator.Validate (definition.Mixins[typeof (MixinWithAdditionalInterfaceDependency)]);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMixinDependencyRules.DependencyMustBeSatisfiedByAnotherMixin", log));
    }

  }
}