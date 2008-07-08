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
  public class DefaultMethodIntroductionRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.MethodIntroductionRules.Add (new DelegateValidationRule<MethodIntroductionDefinition> (PublicMethodNameMustBeUniqueInTargetClass));
      visitor.MethodIntroductionRules.Add (new DelegateValidationRule<MethodIntroductionDefinition> (PublicMethodNameMustBeUniqueInOtherMixins));
    }

    private void PublicMethodNameMustBeUniqueInTargetClass (DelegateValidationRule<MethodIntroductionDefinition>.Args args)
    {
      if (args.Definition.Visibility == MemberVisibility.Public)
      {
        string methodName = args.Definition.InterfaceMember.Name;
        foreach (MethodDefinition method in args.Definition.DeclaringInterface.TargetClass.Methods)
        {
          if (method.Name == methodName)
          {
            args.Log.Fail (args.Self);
            return;
          }
        }
      }
      args.Log.Succeed (args.Self);
    }

    private void PublicMethodNameMustBeUniqueInOtherMixins (DelegateValidationRule<MethodIntroductionDefinition>.Args args)
    {
      if (args.Definition.Visibility == MemberVisibility.Public)
      {
        string methodName = args.Definition.InterfaceMember.Name;
        foreach (InterfaceIntroductionDefinition interfaceIntroduction in args.Definition.DeclaringInterface.TargetClass.IntroducedInterfaces)
        {
          foreach (MethodIntroductionDefinition method in interfaceIntroduction.IntroducedMethods)
          {
            if (method != args.Definition && method.InterfaceMember.Name == methodName)
            {
              args.Log.Fail (args.Self);
              return;
            }
          }
        }
        args.Log.Succeed (args.Self);
      }
    }
  }
}
