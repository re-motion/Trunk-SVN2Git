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
using NUnit.Framework;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;

namespace Remotion.UnitTests.Mixins.ValidationTests.Rules
{
  [TestFixture]
  public class DefaultRequiredMethodRulesTest : ValidationTestBase
  {
    [Test]
    public void FailsIfRequiredBaseMethodIsExplit ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass<ClassFulfillingAllMemberRequirementsExplicitly> ().Clear ().AddMixins (typeof (MixinRequiringAllMembersBase)).EnterScope ())
      {
        TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
            typeof (ClassFulfillingAllMemberRequirementsExplicitly), typeof (MixinRequiringAllMembersBase));
        DefaultValidationLog log = Validator.Validate (definition);

        Assert.IsTrue (
            HasFailure ("Remotion.Mixins.Validation.Rules.DefaultRequiredMethodRules.RequiredBaseCallMethodMustBePublicOrProtected", log));
      }
    }

    [Test]
    public void SucceedsIfRequiredFaceMethodIsExplit ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass<ClassFulfillingAllMemberRequirementsExplicitly> ().Clear ().AddMixins (typeof (MixinRequiringAllMembersFace)).EnterScope ())
      {
        TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
            typeof (ClassFulfillingAllMemberRequirementsExplicitly), typeof (MixinRequiringAllMembersFace));
        DefaultValidationLog log = Validator.Validate (definition);

        AssertSuccess (log);
      }
    }
  }
}
