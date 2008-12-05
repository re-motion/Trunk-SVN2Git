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
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.UnitTests.Mixins.Validation.ValidationSampleTypes;

namespace Remotion.UnitTests.Mixins.Validation.Rules
{
  [TestFixture]
  public class DefaultAttributeIntroductionRulesTest : ValidationTestBase
  {
    [Test]
    public void SucceedsIfTargetClassWinsWhenDefiningAttributes ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1),
          typeof (MixinAddingBT1Attribute));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }

    [Test]
    public void FailsTwiceIfDuplicateAttributeAddedByMixin ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType2), typeof (MixinAddingBT1Attribute),
          typeof (MixinAddingBT1Attribute2));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure (
          "Remotion.Mixins.Validation.Rules.DefaultAttributeIntroductionRules.AllowMultipleRequiredIfAttributeIntroducedMultipleTimes", log));
      Assert.AreEqual (2, log.GetNumberOfFailures ());
    }

    [Test]
    public void FailsTwiceIfDuplicateAttributeAddedByMixinToMember ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassWithVirtualMethod),
          typeof (MixinAddingBT1AttributeToMember), typeof (MixinAddingBT1AttributeToMember2));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (
          HasFailure (
              "Remotion.Mixins.Validation.Rules.DefaultAttributeIntroductionRules.AllowMultipleRequiredIfAttributeIntroducedMultipleTimes", log));
      Assert.AreEqual (2, log.GetNumberOfFailures ());
    }

    [Test]
    public void SucceedsIfDuplicateAttributeAddedByMixinAllowsMultiple ()
    {
      TargetClassDefinition definition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseTypeWithAllowMultiple), typeof (MixinAddingAllowMultipleToClassAndMember));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }
  }
}
