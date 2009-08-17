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
using NUnit.Framework;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.UnitTests.Mixins.Validation.ValidationSampleTypes;

namespace Remotion.UnitTests.Mixins.Validation.Rules
{
  [TestFixture]
  public class DefaultTargetClassRulesTest : ValidationTestBase
  {
    [Test]
    public void FailsIfSealedTargetClass ()
    {
      TargetClassDefinition bc = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (DateTime));
      DefaultValidationLog log = Validator.Validate (bc);
      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultTargetClassRules.TargetClassMustNotBeSealed", log));
      Assert.AreEqual (0, log.GetNumberOfWarnings ());
    }

    [Test]
    public void SucceedsIfAbstractTargetClass ()
    {
      TargetClassDefinition bc = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (MixinWithAbstractMembers));
      DefaultValidationLog log = Validator.Validate (bc);
      AssertSuccess (log);
    }

    [Test]
    public void FailsIfTargetClassDefinitionIsInterface ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (IBaseType2));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultTargetClassRules.TargetClassMustNotBeAnInterface", log));
    }

    [Test]
    public void FailsIfNoPublicOrProtectedCtorInTargetClass ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (ClassWithPrivateCtor),
          typeof (NullMixin));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultTargetClassRules.TargetClassMustHavePublicOrProtectedCtor", log));
    }

		[Test]
		public void FailsIfTargetClassIsNotPublic ()
		{
			TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (InternalClass),
					typeof (NullMixin));
			DefaultValidationLog log = Validator.Validate (definition);

			Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultTargetClassRules.TargetClassMustBePublic", log));
		}

		[Test]
		public void FailsIfNestedTargetClassIsNotPublic ()
		{
			TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (PublicNester.InternalNested),
					typeof (NullMixin));
			DefaultValidationLog log = Validator.Validate (definition);

			Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultTargetClassRules.TargetClassMustBePublic", log));
		}

		[Test]
		public void SucceedsIfNestedTargetClassIsPublic ()
		{
			TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (PublicNester.PublicNested),
					typeof (NullMixin));
			DefaultValidationLog log = Validator.Validate (definition);

			AssertSuccess (log);
		}
  }
}
