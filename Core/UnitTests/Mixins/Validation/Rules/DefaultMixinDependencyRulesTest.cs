// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Validation.Rules
{
  [TestFixture]
  public class DefaultMixinDependencyRulesTest : ValidationTestBase
  {
    [Test]
    public void FailsIfClassMixinDependencyNotFulfilled ()
    {
      ClassContext context = new ClassContextBuilder (typeof (TargetClassWithAdditionalDependencies)).AddMixin<MixinWithAdditionalClassDependency> ().WithDependency<MixinWithNoAdditionalDependency> ().BuildClassContext ();

      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (context);
      DefaultValidationLog log = Validator.Validate (definition.Mixins[typeof (MixinWithAdditionalClassDependency)]);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMixinDependencyRules.DependencyMustBeSatisfiedByAnotherMixin", log));
    }

    [Test]
    public void FailsIfInterfaceMixinDependencyNotFulfilled ()
    {
      ClassContext context = new ClassContextBuilder (typeof (TargetClassWithAdditionalDependencies)).AddMixin<MixinWithAdditionalInterfaceDependency> ().WithDependency<IMixinWithAdditionalClassDependency> ().BuildClassContext ();

      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (context);
      DefaultValidationLog log = Validator.Validate (definition.Mixins[typeof (MixinWithAdditionalInterfaceDependency)]);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMixinDependencyRules.DependencyMustBeSatisfiedByAnotherMixin", log));
    }

  }
}
