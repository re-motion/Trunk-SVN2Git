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
using System.Reflection;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins.Validation.Rules
{
  public class DefaultEventIntroductionRules: RuleSetBase
  {
    private readonly ContextStoreMemberLookupUtility<EventDefinition> _memberLookupUtility = new ContextStoreMemberLookupUtility<EventDefinition> ();
    private readonly ContextStoreMemberIntroductionLookupUtility<EventIntroductionDefinition> _introductionLookupUtility =
        new ContextStoreMemberIntroductionLookupUtility<EventIntroductionDefinition> ();

    public override void Install (ValidatingVisitor visitor)
    {
      visitor.EventIntroductionRules.Add (new DelegateValidationRule<EventIntroductionDefinition> (PublicEventNameMustBeUniqueInTargetClass));
      visitor.EventIntroductionRules.Add (new DelegateValidationRule<EventIntroductionDefinition> (PublicEventNameMustBeUniqueInOtherMixins));
    }

    [DelegateRuleDescription (Message = "An event introduced by a mixin cannot be public if the target class already has an event of the same name.")]
    private void PublicEventNameMustBeUniqueInTargetClass (DelegateValidationRule<EventIntroductionDefinition>.Args args)
    {
      if (args.Definition.Visibility == MemberVisibility.Public)
      {
        EventInfo introducedMember = args.Definition.InterfaceMember;
        if (EnumerableUtility.FirstOrDefault (_memberLookupUtility.GetCachedMembersByName (
            args.Log.ContextStore, args.Definition.DeclaringInterface.TargetClass, introducedMember.Name)) != null)
        {
          args.Log.Fail (args.Self);
          return;
        }
      }
      args.Log.Succeed (args.Self);
    }

    [DelegateRuleDescription (Message = "An event introduced by a mixin cannot be public if another mixin also introduces a public event of the same name.")]
    private void PublicEventNameMustBeUniqueInOtherMixins (DelegateValidationRule<EventIntroductionDefinition>.Args args)
    {
      if (args.Definition.Visibility == MemberVisibility.Public)
      {
        EventInfo introducedEvent = args.Definition.InterfaceMember;
        IEnumerable<EventIntroductionDefinition> otherIntroductionsWithSameName =
            _introductionLookupUtility.GetCachedPublicIntroductionsByName (
            args.Log.ContextStore, args.Definition.DeclaringInterface.TargetClass, introducedEvent.Name);

        foreach (EventIntroductionDefinition eventIntroductionDefinition in otherIntroductionsWithSameName)
        {
          if (eventIntroductionDefinition != args.Definition)
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
