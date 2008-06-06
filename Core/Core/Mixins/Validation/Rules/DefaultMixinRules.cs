/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Reflection;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Utilities;
using Remotion.Mixins.Validation;

namespace Remotion.Mixins.Validation.Rules
{
  public class DefaultMixinRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.MixinRules.Add (new DelegateValidationRule<MixinDefinition> (MixinCannotBeInterface));
      visitor.MixinRules.Add (new DelegateValidationRule<MixinDefinition> (MixinMustBePublic));
      visitor.MixinRules.Add (new DelegateValidationRule<MixinDefinition> (MixinWithOverriddenMembersMustHavePublicOrProtectedDefaultCtor));
      visitor.MixinRules.Add (new DelegateValidationRule<MixinDefinition> (MixinCannotMixItself));
      visitor.MixinRules.Add (new DelegateValidationRule<MixinDefinition> (MixinCannotMixItsBase));
    }

    [DelegateRuleDescription (Message = "An interface is configured as a mixin, but mixins must be classes or value types.")]
    private void MixinCannotBeInterface (DelegateValidationRule<MixinDefinition>.Args args)
    {
      SingleMust (!args.Definition.Type.IsInterface, args.Log, args.Self);
    }

    [DelegateRuleDescription (Message = "A mixin type does not have public visibility.")]
    private void MixinMustBePublic (DelegateValidationRule<MixinDefinition>.Args args)
    {
      SingleMust (args.Definition.Type.IsVisible, args.Log, args.Self);
    }

    [DelegateRuleDescription (Message = "A mixin whose members are overridden by the target class must have a public or protected default constructor.")]
    private void MixinWithOverriddenMembersMustHavePublicOrProtectedDefaultCtor (DelegateValidationRule<MixinDefinition>.Args args)
    {
      ConstructorInfo defaultCtor = args.Definition.Type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
          null, Type.EmptyTypes, null);
      SingleMust (!args.Definition.HasOverriddenMembers() || (defaultCtor != null && ReflectionUtility.IsPublicOrProtected (defaultCtor)),
          args.Log, args.Self);
    }

    [DelegateRuleDescription (Message = "A mixin is applied to itself.")]
    private void MixinCannotMixItself (DelegateValidationRule<MixinDefinition>.Args args)
    {
      SingleMust (args.Definition.Type != args.Definition.TargetClass.Type, args.Log, args.Self);
    }

    [DelegateRuleDescription (Message = "A mixin is applied to one of its base types.")]
    private void MixinCannotMixItsBase (DelegateValidationRule<MixinDefinition>.Args args)
    {
      SingleMust (!args.Definition.TargetClass.Type.IsAssignableFrom (args.Definition.Type), args.Log, args.Self);
    }
  }
}
