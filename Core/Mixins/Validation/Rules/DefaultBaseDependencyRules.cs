using System;
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.Validation.Rules
{
  public class DefaultBaseDependencyRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.BaseDependencyRules.Add (new DelegateValidationRule<BaseDependencyDefinition> (DependencyMustBeSatisfied));
    }

    [DelegateRuleDescription (Message = "An interface specified via the mixins's TBase type parameter is neither implemented by the target "
        + "type nor another mixin.")]
    private void DependencyMustBeSatisfied (DelegateValidationRule<BaseDependencyDefinition>.Args args)
    {
      SingleMust (args.Definition.GetImplementer() != null || args.Definition.IsAggregate, args.Log, args.Self);
    }
  }
}