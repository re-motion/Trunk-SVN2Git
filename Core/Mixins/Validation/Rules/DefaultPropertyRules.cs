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
