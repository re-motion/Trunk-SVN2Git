using System;
using Remotion.Mixins.Definitions;
using System.Reflection;
using Remotion.Mixins.Validation;

namespace Remotion.Mixins.Validation.Rules
{
  public class DefaultPropertyRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.PropertyRules.Add (new DelegateValidationRule<PropertyDefinition> (NewMemberAddedByOverride));
    }

    [DelegateRuleDescription (Message = "A property override adds a new accessor method to the property; this method won't be accessible from the "
        + "mixed instance.")]
    private void NewMemberAddedByOverride (DelegateValidationRule<PropertyDefinition>.Args args)
    {
      SingleShould (args.Definition.Base != null ? (args.Definition.GetMethod == null || args.Definition.GetMethod.Base != null)
          && (args.Definition.SetMethod == null || args.Definition.SetMethod.Base != null)
          : true,
          args.Log,
          args.Self);
    }
  }
}
