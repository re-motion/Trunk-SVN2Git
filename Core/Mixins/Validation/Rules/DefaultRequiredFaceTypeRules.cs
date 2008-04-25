using System;
using System.Collections.Generic;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;

namespace Remotion.Mixins.Validation.Rules
{
  public class DefaultRequiredFaceTypeRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.RequiredFaceTypeRules.Add (new DelegateValidationRule<RequiredFaceTypeDefinition> (FaceClassMustBeAssignableFromTargetType));
      visitor.RequiredFaceTypeRules.Add (new DelegateValidationRule<RequiredFaceTypeDefinition> (RequiredFaceTypeMustBePublic));
    }

    [DelegateRuleDescription (Message = "A class specified as the TThis type parameter of a mixin is not assignable from the target type.")]
    private void FaceClassMustBeAssignableFromTargetType (DelegateValidationRule<RequiredFaceTypeDefinition>.Args args)
    {
      SingleMust (args.Definition.Type.IsClass ? args.Definition.Type.IsAssignableFrom (args.Definition.TargetClass.Type) : true, args.Log, args.Self);
    }

    [DelegateRuleDescription (Message = "A type specified as the TThis type parameter of a mixin does not have public visibility.")]
    private void RequiredFaceTypeMustBePublic (DelegateValidationRule<RequiredFaceTypeDefinition>.Args args)
    {
      SingleMust (args.Definition.Type.IsVisible, args.Log, args.Self);
    }
  }
}
