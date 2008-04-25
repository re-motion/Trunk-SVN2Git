using System;
using NUnit.Framework;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.UnitTests.Mixins.ValidationTests.ValidationSampleTypes;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;

namespace Remotion.UnitTests.Mixins.ValidationTests.Rules
{
  [TestFixture]
  public class DefaultTargetClassRulesTest : ValidationTestBase
  {
    [Test]
    public void FailsIfSealedTargetClass ()
    {
      TargetClassDefinition bc = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (DateTime));
      DefaultValidationLog log = Validator.Validate (bc);
      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultTargetClassRules.TargetClassMustNotBeSealed", log));
      Assert.AreEqual (0, log.GetNumberOfWarnings ());
    }

    [Test]
    public void SucceedsIfAbstractTargetClass ()
    {
      TargetClassDefinition bc = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (MixinWithAbstractMembers));
      DefaultValidationLog log = Validator.Validate (bc);
      AssertSuccess (log);
    }

    [Test]
    public void FailsIfTargetClassDefinitionIsInterface ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (IBaseType2));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultTargetClassRules.TargetClassMustNotBeAnInterface", log));
    }

    [Test]
    public void FailsIfNoPublicOrProtectedCtorInTargetClass ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassWithPrivateCtor),
          typeof (NullMixin));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultTargetClassRules.TargetClassMustHavePublicOrProtectedCtor", log));
    }

		[Test]
		public void FailsIfTargetClassIsNotPublic ()
		{
			TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (InternalClass),
					typeof (NullMixin));
			DefaultValidationLog log = Validator.Validate (definition);

			Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultTargetClassRules.TargetClassMustBePublic", log));
		}

		[Test]
		public void FailsIfNestedTargetClassIsNotPublic ()
		{
			TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (PublicNester.InternalNested),
					typeof (NullMixin));
			DefaultValidationLog log = Validator.Validate (definition);

			Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultTargetClassRules.TargetClassMustBePublic", log));
		}

		[Test]
		public void SucceedsIfNestedTargetClassIsPublic ()
		{
			TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (PublicNester.PublicNested),
					typeof (NullMixin));
			DefaultValidationLog log = Validator.Validate (definition);

			AssertSuccess (log);
		}
  }
}