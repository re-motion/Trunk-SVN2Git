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
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Validation.Rules
{
  public class DefaultPropertyIntroductionRules: RuleSetBase
  {
    private readonly ContextStoreMemberLookupUtility<PropertyDefinition> _memberLookupUtility = new ContextStoreMemberLookupUtility<PropertyDefinition> ();
    private readonly ContextStoreMemberIntroductionLookupUtility<PropertyIntroductionDefinition> _introductionLookupUtility =
        new ContextStoreMemberIntroductionLookupUtility<PropertyIntroductionDefinition> ();

    public override void Install (ValidatingVisitor visitor)
    {
      visitor.PropertyIntroductionRules.Add (new DelegateValidationRule<PropertyIntroductionDefinition> (PublicPropertyNameMustBeUniqueInTargetClass));
      visitor.PropertyIntroductionRules.Add (new DelegateValidationRule<PropertyIntroductionDefinition> (PublicPropertyNameMustBeUniqueInOtherMixins));
    }

    [DelegateRuleDescription (Message = "A property introduced by a mixin cannot be public if the target class already has a property of the same name.")]
    private void PublicPropertyNameMustBeUniqueInTargetClass (DelegateValidationRule<PropertyIntroductionDefinition>.Args args)
    {
      if (args.Definition.Visibility == MemberVisibility.Public)
      {
        PropertyInfo introducedMember = args.Definition.InterfaceMember;
        if (EnumerableUtility.FirstOrDefault (_memberLookupUtility.GetCachedMembersByName (
            args.Log.ContextStore, args.Definition.DeclaringInterface.TargetClass, introducedMember.Name)) != null)
        {
          args.Log.Fail (args.Self);
          return;
        }
      }
      args.Log.Succeed (args.Self);
    }

    [DelegateRuleDescription (Message = "A property introduced by a mixin cannot be public if another mixin also introduces a public property of the same name.")]
    private void PublicPropertyNameMustBeUniqueInOtherMixins (DelegateValidationRule<PropertyIntroductionDefinition>.Args args)
    {
      if (args.Definition.Visibility == MemberVisibility.Public)
      {
        PropertyInfo introducedProperty = args.Definition.InterfaceMember;
        IEnumerable<PropertyIntroductionDefinition> otherIntroductionsWithSameName =
            _introductionLookupUtility.GetCachedPublicIntroductionsByName (
            args.Log.ContextStore, args.Definition.DeclaringInterface.TargetClass, introducedProperty.Name);

        foreach (PropertyIntroductionDefinition property in otherIntroductionsWithSameName)
        {
          if (property != args.Definition)
          {
            args.Log.Fail (args.Self);
            return;
          }
        }
        args.Log.Succeed (args.Self);
      }
    }
  }
}
