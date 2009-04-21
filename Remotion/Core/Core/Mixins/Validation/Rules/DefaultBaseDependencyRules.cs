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
