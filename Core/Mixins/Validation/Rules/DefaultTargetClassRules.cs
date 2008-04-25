using System;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Utilities;
using Remotion.Mixins.Validation;
using System.Reflection;

namespace Remotion.Mixins.Validation.Rules
{
  public class DefaultTargetClassRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.TargetClassRules.Add (new DelegateValidationRule<TargetClassDefinition> (TargetClassMustNotBeSealed));
      visitor.TargetClassRules.Add (new DelegateValidationRule<TargetClassDefinition> (TargetClassMustNotBeAnInterface));
			visitor.TargetClassRules.Add (new DelegateValidationRule<TargetClassDefinition> (TargetClassMustHavePublicOrProtectedCtor));
    	visitor.TargetClassRules.Add (new DelegateValidationRule<TargetClassDefinition> (TargetClassMustBePublic));
    }

  	[DelegateRuleDescription (Message = "A target class for mixins is declared sealed (or it is a value type).")]
    private void TargetClassMustNotBeSealed (DelegateValidationRule<TargetClassDefinition>.Args args)
    {
      SingleMust(!args.Definition.Type.IsSealed, args.Log, args.Self);
    }

    [DelegateRuleDescription (Message = "An interface is used as a target class for mixins.")]
    private void TargetClassMustNotBeAnInterface (DelegateValidationRule<TargetClassDefinition>.Args args)
    {
      SingleMust (!args.Definition.Type.IsInterface, args.Log, args.Self);
    }

    [DelegateRuleDescription (Message = "A target class for mixins does not have a public or protected constructor.")]
    private void TargetClassMustHavePublicOrProtectedCtor (DelegateValidationRule<TargetClassDefinition>.Args args)
    {
      ConstructorInfo[] ctors = args.Definition.Type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      ConstructorInfo[] publicOrProtectedCtors = Array.FindAll (ctors,
          delegate (ConstructorInfo ctor) { return ReflectionUtility.IsPublicOrProtected (ctor); });
      SingleMust (publicOrProtectedCtors.Length > 0, args.Log, args.Self);
    }

		[DelegateRuleDescription (Message = "A target class for mixins is not publicly visible.")]
		private void TargetClassMustBePublic (DelegateValidationRule<TargetClassDefinition>.Args args)
		{
			SingleMust (args.Definition.Type.IsVisible, args.Log, args.Self);
		
		}
  }
}
