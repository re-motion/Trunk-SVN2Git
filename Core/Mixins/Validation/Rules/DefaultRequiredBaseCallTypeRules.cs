using System;
using System.Collections.Generic;
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.Validation.Rules
{
  public class DefaultRequiredBaseCallTypeRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.RequiredBaseCallTypeRules.Add (new DelegateValidationRule<RequiredBaseCallTypeDefinition> (RequiredBaseCallTypeMustBePublic));
    }

    // Now throws ConfigurationException when violated
    //private void BaseCallTypeMustBeInterface (DelegateValidationRule<RequiredBaseCallTypeDefinition>.Args args)
    //{
    //  SingleMust (args.Definition.Type.IsInterface, args.Log, args.Self);
    //}

    [DelegateRuleDescription (Message = "A type used as the TBase type parameter of a mixin does not have public visibility.")]
    private void RequiredBaseCallTypeMustBePublic (DelegateValidationRule<RequiredBaseCallTypeDefinition>.Args args)
    {
      SingleMust (args.Definition.Type.IsVisible, args.Log, args.Self);
    }

  }
}
