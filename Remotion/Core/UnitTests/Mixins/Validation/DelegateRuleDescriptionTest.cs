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

namespace Remotion.UnitTests.Mixins.Validation
{
  [TestFixture]
  public class DelegateRuleDescriptionTest
  {
    private void NonDescribedSampleRule (DelegateValidationRule<TargetClassDefinition>.Args args)
    {
    }

    [DelegateRuleDescription]
    private void DescribedSampleRule_NoChanges (DelegateValidationRule<TargetClassDefinition>.Args args)
    {
    }

    [DelegateRuleDescription (RuleName = "Fritz", Message = "Echo?")]
    private void DescribedSampleRule_Changes (DelegateValidationRule<TargetClassDefinition>.Args args)
    {
    }

    [Test]
    public void DefaultDescription ()
    {
      IValidationRule rule = new DelegateValidationRule<TargetClassDefinition> (NonDescribedSampleRule);
      Assert.AreEqual ("Remotion.UnitTests.Mixins.Validation.DelegateRuleDescriptionTest.NonDescribedSampleRule", rule.RuleName);
      Assert.AreEqual ("Non described sample rule", rule.Message);
    }

    [Test]
    public void DescriptionAttribute_NoChanges ()
    {
      IValidationRule rule = new DelegateValidationRule<TargetClassDefinition> (DescribedSampleRule_NoChanges);
      Assert.AreEqual ("Remotion.UnitTests.Mixins.Validation.DelegateRuleDescriptionTest.DescribedSampleRule_NoChanges", rule.RuleName);
      Assert.AreEqual ("Described sample rule_ no changes", rule.Message);
    }

    [Test]
    public void DescriptionAttribute_Changes ()
    {
      IValidationRule rule = new DelegateValidationRule<TargetClassDefinition> (DescribedSampleRule_Changes);
      Assert.AreEqual ("Fritz", rule.RuleName);
      Assert.AreEqual ("Echo?", rule.Message);
    }
  }
}
