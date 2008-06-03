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
