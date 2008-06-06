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
  public class DefaultRequiredFaceTypeRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.RequiredFaceTypeRules.Add (new DelegateValidationRule<RequiredFaceTypeDefinition> (FaceClassMustBeAssignableFromTargetType));
      visitor.RequiredFaceTypeRules.Add (new DelegateValidationRule<RequiredFaceTypeDefinition> (RequiredFaceTypeMustBePublic));
    }

    [DelegateRuleDescription (Message = "A class specified as the TThis type parameter of a mixin is not assignable from the target type.")]
    private void FaceClassMustBeAssignableFromTargetType (DelegateValidationRule<RequiredFaceTypeDefinition>.Args args)
    {
      SingleMust (args.Definition.Type.IsClass ? args.Definition.Type.IsAssignableFrom (args.Definition.TargetClass.Type) : true, args.Log, args.Self);
    }

    [DelegateRuleDescription (Message = "A type specified as the TThis type parameter of a mixin does not have public visibility.")]
    private void RequiredFaceTypeMustBePublic (DelegateValidationRule<RequiredFaceTypeDefinition>.Args args)
    {
      SingleMust (args.Definition.Type.IsVisible, args.Log, args.Self);
    }
  }
}
