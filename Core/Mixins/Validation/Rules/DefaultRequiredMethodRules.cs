using System;
using System.Collections.Generic;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Utilities;
using Remotion.Mixins.Validation;

namespace Remotion.Mixins.Validation.Rules
{
  public class DefaultRequiredMethodRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.RequiredMethodRules.Add (new DelegateValidationRule<RequiredMethodDefinition> (RequiredBaseCallMethodMustBePublicOrProtected));
    }

    [DelegateRuleDescription (Message = "One of the methods specified via the TBase type parameter of a mixin is not implemented as a public "
        + "or protected method.")]
    private void RequiredBaseCallMethodMustBePublicOrProtected (DelegateValidationRule<RequiredMethodDefinition>.Args args)
    {
      SingleMust (!(args.Definition.DeclaringRequirement is RequiredBaseCallTypeDefinition)
          || ReflectionUtility.IsPublicOrProtected (args.Definition.ImplementingMethod.MethodInfo), args.Log, args.Self);
    }
  }
}
