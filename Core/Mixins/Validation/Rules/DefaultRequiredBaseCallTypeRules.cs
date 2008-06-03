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
using System.Collections.Generic;
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.Validation.Rules
{
  public class DefaultRequiredBaseCallTypeRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.RequiredBaseCallTypeRules.Add (new DelegateValidationRule<RequiredBaseCallTypeDefinition> (RequiredBaseCallTypeMustBePublic));
    }

    // Now throws ConfigurationException when violated
    //private void BaseCallTypeMustBeInterface (DelegateValidationRule<RequiredBaseCallTypeDefinition>.Args args)
    //{
    //  SingleMust (args.Definition.Type.IsInterface, args.Log, args.Self);
    //}

    [DelegateRuleDescription (Message = "A type used as the TBase type parameter of a mixin does not have public visibility.")]
    private void RequiredBaseCallTypeMustBePublic (DelegateValidationRule<RequiredBaseCallTypeDefinition>.Args args)
    {
      SingleMust (args.Definition.Type.IsVisible, args.Log, args.Self);
    }

  }
}
