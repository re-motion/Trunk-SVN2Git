/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
