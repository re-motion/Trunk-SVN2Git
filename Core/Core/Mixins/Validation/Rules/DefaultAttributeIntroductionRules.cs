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
using Remotion.Utilities;

namespace Remotion.Mixins.Validation.Rules
{
  public class DefaultAttributeIntroductionRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.AttributeIntroductionRules.Add (
          new DelegateValidationRule<AttributeIntroductionDefinition> (AllowMultipleRequiredIfAttributeIntroducedMultipleTimes));
    }

    [DelegateRuleDescription (Message = "Multiple attributes of the same attribute type are introduced by mixins, but the attribute type does "
        + "not specify 'AllowMultiple = true' in its AttributeUsage declaration.")]
    private void AllowMultipleRequiredIfAttributeIntroducedMultipleTimes (DelegateValidationRule<AttributeIntroductionDefinition>.Args args)
    {
      SingleMust (AttributeUtility.IsAttributeAllowMultiple (args.Definition.AttributeType)
        || args.Definition.Target.ReceivedAttributes.GetItemCount (args.Definition.AttributeType) < 2, args.Log, args.Self);
    }
  }
}
