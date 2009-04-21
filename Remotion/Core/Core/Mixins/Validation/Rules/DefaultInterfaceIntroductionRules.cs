// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.Validation.Rules
{
  public class DefaultInterfaceIntroductionRules: RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.InterfaceIntroductionRules.Add (new DelegateValidationRule<InterfaceIntroductionDefinition> (IMixinTargetCannotBeIntroduced));
      visitor.InterfaceIntroductionRules.Add (new DelegateValidationRule<InterfaceIntroductionDefinition> (IntroducedInterfaceMustBePublic));
    }

    [DelegateRuleDescription (Message = "The interface 'IMixinTarget' is part of the mixin infrastructure and cannot be introduced by a mixin.")]
    private void IMixinTargetCannotBeIntroduced (DelegateValidationRule<InterfaceIntroductionDefinition>.Args args)
    {
      SingleMust (!typeof (IMixinTarget).Equals (args.Definition.InterfaceType), args.Log, args.Self);
    }

    [DelegateRuleDescription (Message = "An interface introduced by a mixin does not have public visibility.")]
    private void IntroducedInterfaceMustBePublic (DelegateValidationRule<InterfaceIntroductionDefinition>.Args args)
    {
      SingleMust (args.Definition.InterfaceType.IsVisible, args.Log, args.Self);
    }
  }
}
