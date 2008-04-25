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