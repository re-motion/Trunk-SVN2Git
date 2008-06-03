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
