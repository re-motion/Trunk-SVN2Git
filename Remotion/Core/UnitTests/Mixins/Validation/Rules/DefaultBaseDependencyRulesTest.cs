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
using NUnit.Framework;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.UnitTests.Mixins.Validation.ValidationSampleTypes;

namespace Remotion.UnitTests.Mixins.Validation.Rules
{
  [TestFixture]
  public class DefaultBaseDependencyRulesTest : ValidationTestBase
  {
    [Test]
    public void FailsIfEmptyBaseDependencyNotFulfilled ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (MixinWithUnsatisfiedEmptyBaseDependency));
      DefaultValidationLog log = Validator.Validate (
          definition.Mixins[typeof (MixinWithUnsatisfiedEmptyBaseDependency)].BaseDependencies[typeof (IEmptyInterface)]);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultBaseDependencyRules.DependencyMustBeSatisfied", log));
    }

    [Test]
    public void SucceedsIfDuckBaseDependency ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (ClassFulfillingAllMemberRequirementsDuck),
          typeof (MixinRequiringAllMembersBase));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfAggregateBaseDependencyIsFullyImplemented ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin4), typeof (BT3Mixin7Base));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }

    [Test]
    public void FailsIfEmptyAggregateBaseDependencyIsNotAvailable ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (NullTarget), typeof (MixinWithUnsatisfiedEmptyAggregateBaseDependency));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultBaseDependencyRules.DependencyMustBeSatisfied", log));
    }
  }
}
