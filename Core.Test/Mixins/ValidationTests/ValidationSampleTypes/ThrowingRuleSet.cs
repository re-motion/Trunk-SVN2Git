using System;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;
using Remotion.Mixins.Validation.Rules;

namespace Remotion.UnitTests.Mixins.ValidationTests.ValidationSampleTypes
{
  public class ThrowingRuleSet : IRuleSet
  {
    public void Install (ValidatingVisitor visitor)
    {
      visitor.TargetClassRules.Add (new DelegateValidationRule<TargetClassDefinition> (delegate { throw new InvalidOperationException (); }));
    }
  }
}