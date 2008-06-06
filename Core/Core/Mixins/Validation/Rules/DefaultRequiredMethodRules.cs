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
using Remotion.Mixins.Utilities;

namespace Remotion.Mixins.Validation.Rules
{
  public class DefaultRequiredMethodRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.RequiredMethodRules.Add (new DelegateValidationRule<RequiredMethodDefinition> (RequiredBaseCallMethodMustBePublicOrProtected));
    }

    [DelegateRuleDescription (Message = "One of the methods specified via the TBase type parameter of a mixin is not implemented as a public "
        + "or protected method.")]
    private void RequiredBaseCallMethodMustBePublicOrProtected (DelegateValidationRule<RequiredMethodDefinition>.Args args)
    {
      SingleMust (!(args.Definition.DeclaringRequirement is RequiredBaseCallTypeDefinition)
          || ReflectionUtility.IsPublicOrProtected (args.Definition.ImplementingMethod.MethodInfo), args.Log, args.Self);
    }
  }
}
