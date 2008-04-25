using System;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;

namespace Remotion.Mixins.Validation.Rules
{
  public class DefaultInterfaceIntroductionRules: RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.InterfaceIntroductionRules.Add (new DelegateValidationRule<InterfaceIntroductionDefinition> (IMixinTargetCannotBeIntroduced));
      visitor.InterfaceIntroductionRules.Add (new DelegateValidationRule<InterfaceIntroductionDefinition> (IntroducedInterfaceMustBePublic));
    }

    [DelegateRuleDescription (Message = "The interface 'IMixinTarget' is part of the mixin infrastructure and cannot be introduced by a mixin.")]
    private void IMixinTargetCannotBeIntroduced (DelegateValidationRule<InterfaceIntroductionDefinition>.Args args)
    {
      SingleMust (!typeof (IMixinTarget).Equals (args.Definition.Type), args.Log, args.Self);
    }

    [DelegateRuleDescription (Message = "An interface introduced by a mixin does not have public visibility.")]
    private void IntroducedInterfaceMustBePublic (DelegateValidationRule<InterfaceIntroductionDefinition>.Args args)
    {
      SingleMust (args.Definition.Type.IsVisible, args.Log, args.Self);
    }
  }
}