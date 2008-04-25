using System;
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.Validation.Rules
{
  public class DefaultMixinDependencyRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.MixinDependencyRules.Add (new DelegateValidationRule<MixinDependencyDefinition> (DependencyMustBeSatisfiedByAnotherMixin));
    }

    [DelegateRuleDescription (Message = "A mixin is configured with a dependency to another mixin, but that dependency is not satisfied.")]
    private void DependencyMustBeSatisfiedByAnotherMixin (DelegateValidationRule<MixinDependencyDefinition>.Args args)
    {
      SingleMust (args.Definition.GetImplementer() != null || args.Definition.IsAggregate, args.Log, args.Self);
    }
  }
}