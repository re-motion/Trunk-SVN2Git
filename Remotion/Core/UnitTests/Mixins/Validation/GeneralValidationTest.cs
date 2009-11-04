// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;
using Remotion.Mixins.Validation.Rules;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.UnitTests.Mixins.Validation.ValidationSampleTypes;
using System.Linq;

namespace Remotion.UnitTests.Mixins.Validation
{
  [TestFixture]
  public class GeneralValidationTest : ValidationTestBase
  {
    [Test]
    public void ValidationVisitsSomething ()
    {
      IValidationLog log = MixinConfiguration.ActiveConfiguration.Validate ();
      Assert.That (log.ResultCount > 1, Is.True);
    }

    [Test]
    public void ValidationDump ()
    {
      IValidationLog log = MixinConfiguration.ActiveConfiguration.Validate ();
      ConsoleDumper.DumpValidationResults (log.GetResults ());
    }

    [Test]
    public void ValidationResultDefinition ()
    {
      IValidationLog log = MixinConfiguration.ActiveConfiguration.Validate();

      using (IEnumerator<ValidationResult> results = log.GetResults().GetEnumerator())
      {
        Assert.That (results.MoveNext (), Is.True);
        ValidationResult firstResult = results.Current;
        Assert.That (firstResult.Definition, Is.Not.Null);
      }
    }

    [Test]
    public void DefaultConfiguration_IsValid ()
    {
      IValidationLog log = MixinConfiguration.ActiveConfiguration.Validate ();
      AssertSuccess (log);
    }

    [Test]
    public void HasDefaultRules ()
    {
      IValidationLog log = MixinConfiguration.ActiveConfiguration.Validate ();
      Assert.That (log.GetNumberOfRulesExecuted () > 0, Is.True);
    }

    [Test]
    public void CollectsUnexpectedExceptions ()
    {
      TargetClassDefinition bc = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (DateTime));
      DefaultValidationLog log = Validator.Validate (bc, new ThrowingRuleSet ());
      Assert.That (log.GetNumberOfUnexpectedExceptions () > 0, Is.True);
      var results = new List<ValidationResult> (log.GetResults ());
      Assert.That (results[0].Exceptions[0].Exception is InvalidOperationException, Is.True);
    }


    [Test]
    public void DefaultConfiguration_AllIsVisitedOnce ()
    {
      IValidationLog log = MixinConfiguration.ActiveConfiguration.Validate ();

      var visitedDefinitions = new HashSet<IVisitableDefinition> ();
      foreach (ValidationResult result in log.GetResults ())
      {
        Assert.That (result.Definition, Is.Not.Null);
        Assert.That (visitedDefinitions.Contains (result.Definition), Is.False);
        visitedDefinitions.Add (result.Definition);
      }

      TargetClassDefinition bt1 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));
      AssertVisitedEquivalent (visitedDefinitions, bt1);
      TargetClassDefinition bt3 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType3));
      AssertVisitedEquivalent (visitedDefinitions, bt3);
      TargetClassDefinition btWithAdditionalDependencies = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (TargetClassWithAdditionalDependencies));
      AssertVisitedEquivalent (visitedDefinitions, btWithAdditionalDependencies);
      TargetClassDefinition targetWithSuppressAttribute = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (TargetClassSuppressingBT1Attribute));
      AssertVisitedEquivalent (visitedDefinitions, targetWithSuppressAttribute);
      TargetClassDefinition targetWithNonIntroducedAttribute = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (TargetClassWithMixinNonIntroducingSimpleAttribute));
      AssertVisitedEquivalent (visitedDefinitions, targetWithSuppressAttribute);
      TargetClassDefinition targetClassWinningOverMixinAddingBT1AttributeToMember = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (TargetClassWinningOverMixinAddingBT1AttributeToMember));
      AssertVisitedEquivalent (visitedDefinitions, targetClassWinningOverMixinAddingBT1AttributeToMember);

      MixinDefinition bt1m1 = bt1.Mixins[typeof (BT1Mixin1)];
      AssertVisitedEquivalent (visitedDefinitions, bt1m1);
      MixinDefinition bt1m2 = bt1.Mixins[typeof (BT1Mixin2)];
      AssertVisitedEquivalent (visitedDefinitions, bt1m2);
      MixinDefinition bt3m1 = bt3.Mixins[typeof (BT3Mixin1)];
      AssertVisitedEquivalent (visitedDefinitions, bt3m1);
      MixinDefinition bt3m2 = bt3.Mixins[typeof (BT3Mixin2)];
      AssertVisitedEquivalent (visitedDefinitions, bt3m2);
      MixinDefinition bt3m3 = bt3.GetMixinByConfiguredType (typeof (BT3Mixin3<,>));
      AssertVisitedEquivalent (visitedDefinitions, bt3m3);
      MixinDefinition bt3m4 = bt3.Mixins[typeof (BT3Mixin4)];
      AssertVisitedEquivalent (visitedDefinitions, bt3m4);
      MixinDefinition bt3m5 = bt3.Mixins[typeof (BT3Mixin5)];
      AssertVisitedEquivalent (visitedDefinitions, bt3m5);
      MixinDefinition mixinWithSuppressedAttribute = targetWithSuppressAttribute.Mixins[typeof (MixinAddingBT1Attribute)];
      AssertVisitedEquivalent (visitedDefinitions, mixinWithSuppressedAttribute);
      MixinDefinition mixinWithNonIntroducedAttribute = targetWithNonIntroducedAttribute.Mixins[typeof (MixinNonIntroducingSimpleAttribute)];
      AssertVisitedEquivalent (visitedDefinitions, mixinWithNonIntroducedAttribute);

      MethodDefinition m1 = bt1.Methods[typeof (BaseType1).GetMethod ("VirtualMethod", Type.EmptyTypes)];
      AssertVisitedEquivalent (visitedDefinitions, m1);
      MethodDefinition m2 = bt1.Methods[typeof (BaseType1).GetMethod ("VirtualMethod", new[] { typeof (string) })];
      AssertVisitedEquivalent (visitedDefinitions, m2);
      MethodDefinition m3 = bt1m1.Methods[typeof (BT1Mixin1).GetMethod ("VirtualMethod")];
      AssertVisitedEquivalent (visitedDefinitions, m3);
      MethodDefinition m4 = bt1m1.Methods[typeof (BT1Mixin1).GetMethod ("IntroducedMethod")];
      AssertVisitedEquivalent (visitedDefinitions, m4);
      MethodDefinition memberWinningOverMixinAddingAttribute = targetClassWinningOverMixinAddingBT1AttributeToMember.Methods[typeof (TargetClassWinningOverMixinAddingBT1AttributeToMember).GetMethod ("VirtualMethod")];
      AssertVisitedEquivalent (visitedDefinitions, memberWinningOverMixinAddingAttribute);

      PropertyDefinition p1 = bt1.Properties[typeof (BaseType1).GetProperty ("VirtualProperty")];
      AssertVisitedEquivalent (visitedDefinitions, p1);
      MethodDefinition m5 = p1.GetMethod;
      AssertVisitedEquivalent (visitedDefinitions, m5);
      MethodDefinition m6 = p1.SetMethod;
      AssertVisitedEquivalent (visitedDefinitions, m6);
      PropertyDefinition p2 = bt1m1.Properties[typeof (BT1Mixin1).GetProperty ("VirtualProperty")];
      AssertVisitedEquivalent (visitedDefinitions, p2);

      EventDefinition e1 = bt1.Events[typeof (BaseType1).GetEvent ("VirtualEvent")];
      AssertVisitedEquivalent (visitedDefinitions, e1);
      MethodDefinition m7 = e1.AddMethod;
      AssertVisitedEquivalent (visitedDefinitions, m7);
      MethodDefinition m8 = e1.RemoveMethod;
      AssertVisitedEquivalent (visitedDefinitions, m8);
      EventDefinition e2 = bt1m1.Events[typeof (BT1Mixin1).GetEvent ("VirtualEvent")];
      AssertVisitedEquivalent (visitedDefinitions, e2);

      InterfaceIntroductionDefinition i1 = bt1m1.InterfaceIntroductions[typeof (IBT1Mixin1)];
      AssertVisitedEquivalent (visitedDefinitions, i1);
      MethodIntroductionDefinition im1 = i1.IntroducedMethods[typeof (IBT1Mixin1).GetMethod ("IntroducedMethod")];
      AssertVisitedEquivalent (visitedDefinitions, im1);
      PropertyIntroductionDefinition im2 = i1.IntroducedProperties[typeof (IBT1Mixin1).GetProperty ("IntroducedProperty")];
      AssertVisitedEquivalent (visitedDefinitions, im2);
      EventIntroductionDefinition im3 = i1.IntroducedEvents[typeof (IBT1Mixin1).GetEvent ("IntroducedEvent")];
      AssertVisitedEquivalent (visitedDefinitions, im3);

      AttributeDefinition a1 = bt1.CustomAttributes.GetFirstItem (typeof (BT1Attribute));
      AssertVisitedEquivalent (visitedDefinitions, a1);
      AttributeDefinition a2 = bt1m1.CustomAttributes.GetFirstItem (typeof (BT1M1Attribute));
      AssertVisitedEquivalent (visitedDefinitions, a2);
      AttributeDefinition a3 = m1.CustomAttributes.GetFirstItem (typeof (BT1Attribute));
      AssertVisitedEquivalent (visitedDefinitions, a3);
      AttributeDefinition a4 = p1.CustomAttributes.GetFirstItem (typeof (BT1Attribute));
      AssertVisitedEquivalent (visitedDefinitions, a4);
      AttributeDefinition a5 = e1.CustomAttributes.GetFirstItem (typeof (BT1Attribute));
      AssertVisitedEquivalent (visitedDefinitions, a5);
      AttributeDefinition a6 = im1.ImplementingMember.CustomAttributes.GetFirstItem (typeof (BT1M1Attribute));
      AssertVisitedEquivalent (visitedDefinitions, a6);
      AttributeDefinition a7 = im2.ImplementingMember.CustomAttributes.GetFirstItem (typeof (BT1M1Attribute));
      AssertVisitedEquivalent (visitedDefinitions, a7);
      AttributeDefinition a8 = im3.ImplementingMember.CustomAttributes.GetFirstItem (typeof (BT1M1Attribute));
      AssertVisitedEquivalent (visitedDefinitions, a8);

      AttributeIntroductionDefinition ai1 = bt1.ReceivedAttributes.GetFirstItem (typeof (BT1M1Attribute));
      AssertVisitedEquivalent (visitedDefinitions, ai1);
      AttributeIntroductionDefinition ai2 = m1.ReceivedAttributes.GetFirstItem (typeof (BT1M1Attribute));
      AssertVisitedEquivalent (visitedDefinitions, ai2);

      RequiredBaseCallTypeDefinition bc1 = bt3.RequiredBaseCallTypes[typeof (IBaseType34)];
      AssertVisitedEquivalent (visitedDefinitions, bc1);
      RequiredMethodDefinition bcm1 = bc1.Methods[typeof (IBaseType34).GetMethod ("IfcMethod")];
      AssertVisitedEquivalent (visitedDefinitions, bcm1);

      RequiredFaceTypeDefinition ft1 = bt3.RequiredFaceTypes[typeof (IBaseType32)];
      AssertVisitedEquivalent (visitedDefinitions, ft1);
      RequiredMethodDefinition fm1 = ft1.Methods[typeof (IBaseType32).GetMethod ("IfcMethod")];
      AssertVisitedEquivalent (visitedDefinitions, fm1);

      RequiredMixinTypeDefinition rmt1 = btWithAdditionalDependencies.RequiredMixinTypes[typeof (IMixinWithAdditionalClassDependency)];
      AssertVisitedEquivalent (visitedDefinitions, rmt1);
      RequiredMixinTypeDefinition rmt2 = btWithAdditionalDependencies.RequiredMixinTypes[typeof (MixinWithNoAdditionalDependency)];
      AssertVisitedEquivalent (visitedDefinitions, rmt2);

      ThisDependencyDefinition td1 = bt3m1.ThisDependencies[typeof (IBaseType31)];
      AssertVisitedEquivalent (visitedDefinitions, td1);

      BaseDependencyDefinition bd1 = bt3m1.BaseDependencies[typeof (IBaseType31)];
      AssertVisitedEquivalent (visitedDefinitions, bd1);

      MixinDependencyDefinition md1 = btWithAdditionalDependencies.Mixins[typeof (MixinWithAdditionalClassDependency)].MixinDependencies[typeof (MixinWithNoAdditionalDependency)];
      AssertVisitedEquivalent (visitedDefinitions, md1);

      SuppressedAttributeIntroductionDefinition suppressedAttribute1 = mixinWithSuppressedAttribute.SuppressedAttributeIntroductions.GetFirstItem (typeof (BT1Attribute));
      AssertVisitedEquivalent (visitedDefinitions, suppressedAttribute1);

      NonAttributeIntroductionDefinition nonIntroducedAttribute1 = mixinWithNonIntroducedAttribute.NonAttributeIntroductions.GetFirstItem (typeof (SimpleAttribute));
      AssertVisitedEquivalent (visitedDefinitions, nonIntroducedAttribute1);
      NonAttributeIntroductionDefinition nonIntroducedAttribute2 = memberWinningOverMixinAddingAttribute.Overrides[0].NonAttributeIntroductions[0];
      AssertVisitedEquivalent (visitedDefinitions, nonIntroducedAttribute2);
    }

    [Test]
    public void ValidationException ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (ClassOverridingSingleMixinMethod), typeof (AbstractMixinWithoutBase));

      var log = new DefaultValidationLog();
      var visitor = new ValidatingVisitor (log);
      new DefaultMethodRules().Install (visitor);
      definition.Accept (visitor);

      var exception = new ValidationException (log);
      Assert.That (exception.Message, 
          Is.EqualTo ("Some parts of the mixin configuration could not be validated." 
              + Environment.NewLine
              + "Remotion.UnitTests.Mixins.Validation.ValidationSampleTypes.AbstractMixinWithoutBase.AbstractMethod (Remotion.UnitTests.Mixins."
              + "Validation.ValidationSampleTypes.AbstractMixinWithoutBase -> Remotion.UnitTests.Mixins.SampleTypes.ClassOverridingSingleMixinMethod):"
              + Environment.NewLine
              + "Error: A target class overrides a method from one of its mixins, but the mixin is not derived from one of the Mixin<...> base classes."
              + Environment.NewLine));

      Assert.That (exception.ValidationLog, Is.SameAs (log));
    }

    [Test]
    public void Merge ()
    {
      IValidationLog sourceLog = new DefaultValidationLog ();
      var exception = new Exception ();

      TargetClassDefinition bt1 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));
      TargetClassDefinition bt2 = DefinitionObjectMother.GetActiveTargetClassDefinition_Force (typeof (BaseType2));
      TargetClassDefinition bt3 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType3));
      TargetClassDefinition bt4 = DefinitionObjectMother.GetActiveTargetClassDefinition_Force (typeof (BaseType4));

      sourceLog.ValidationStartsFor (bt1);
      sourceLog.Succeed (new DelegateValidationRule<TargetClassDefinition> (delegate { }, "Success", "Success"));
      sourceLog.Warn (new DelegateValidationRule<TargetClassDefinition> (delegate { }, "Warn", "Warn"));
      sourceLog.Fail (new DelegateValidationRule<TargetClassDefinition> (delegate { }, "Fail", "Fail"));
      sourceLog.UnexpectedException (new DelegateValidationRule<TargetClassDefinition> (delegate { }, "Except", "Except"), exception);
      sourceLog.ValidationEndsFor (bt1);

      sourceLog.ValidationStartsFor (bt4);
      sourceLog.Succeed (new DelegateValidationRule<TargetClassDefinition> (delegate { }, "Success2", "Success2"));
      sourceLog.Warn (new DelegateValidationRule<TargetClassDefinition> (delegate { }, "Warn2", "Warn2"));
      sourceLog.Fail (new DelegateValidationRule<TargetClassDefinition> (delegate { }, "Fail2", "Fail2"));
      sourceLog.UnexpectedException (new DelegateValidationRule<TargetClassDefinition> (delegate { }, "Except2", "Except2"), exception);
      sourceLog.ValidationEndsFor (bt4);

      IValidationLog resultLog = new DefaultValidationLog ();
      resultLog.ValidationStartsFor (bt2);
      resultLog.Succeed (new DelegateValidationRule<TargetClassDefinition> (delegate { }, "0", "0"));
      resultLog.Warn (new DelegateValidationRule<TargetClassDefinition> (delegate { }, "1", "1"));
      resultLog.Fail (new DelegateValidationRule<TargetClassDefinition> (delegate { }, "2", "2"));
      resultLog.UnexpectedException (new DelegateValidationRule<TargetClassDefinition> (delegate { }, "3", "3"), exception);
      resultLog.ValidationEndsFor (bt2);

      resultLog.ValidationStartsFor (bt1);
      resultLog.Succeed (new DelegateValidationRule<TargetClassDefinition> (delegate { }, "4", "4"));
      resultLog.Warn (new DelegateValidationRule<TargetClassDefinition> (delegate { }, "5", "5"));
      resultLog.Fail (new DelegateValidationRule<TargetClassDefinition> (delegate { }, "6", "6"));
      resultLog.UnexpectedException (new DelegateValidationRule<TargetClassDefinition> (delegate { }, "7", "7"), exception);
      resultLog.ValidationEndsFor (bt1);

      resultLog.ValidationStartsFor (bt3);
      resultLog.Succeed (new DelegateValidationRule<TargetClassDefinition> (delegate { }, "8", "8"));
      resultLog.Warn (new DelegateValidationRule<TargetClassDefinition> (delegate { }, "9", "9"));
      resultLog.Fail (new DelegateValidationRule<TargetClassDefinition> (delegate { }, "10", "10"));
      resultLog.UnexpectedException (new DelegateValidationRule<TargetClassDefinition> (delegate { }, "11", "11"), exception);
      resultLog.ValidationEndsFor (bt3);

      resultLog.MergeIn (sourceLog);
      Assert.That (resultLog.GetNumberOfSuccesses (), Is.EqualTo (5));
      Assert.That (resultLog.GetNumberOfWarnings (), Is.EqualTo (5));
      Assert.That (resultLog.GetNumberOfFailures (), Is.EqualTo (5));
      Assert.That (resultLog.GetNumberOfUnexpectedExceptions (), Is.EqualTo (5));

      var results = new List<ValidationResult> (resultLog.GetResults ());

      Assert.That (results.Count, Is.EqualTo (4));

      Assert.That (results[0].Definition, Is.EqualTo (bt2));
      Assert.That (results[0].Successes.Count, Is.EqualTo (1));
      Assert.That (results[0].Failures.Count, Is.EqualTo (1));
      Assert.That (results[0].Warnings.Count, Is.EqualTo (1));
      Assert.That (results[0].Exceptions.Count, Is.EqualTo (1));

      Assert.That (results[1].Definition, Is.EqualTo (bt1));

      Assert.That (results[1].Successes.Count, Is.EqualTo (2));
      Assert.That (results[1].Successes[0].Message, Is.EqualTo ("4"));
      Assert.That (results[1].Successes[1].Message, Is.EqualTo ("Success"));

      Assert.That (results[1].Warnings.Count, Is.EqualTo (2));
      Assert.That (results[1].Warnings[0].Message, Is.EqualTo ("5"));
      Assert.That (results[1].Warnings[1].Message, Is.EqualTo ("Warn"));

      Assert.That (results[1].Failures.Count, Is.EqualTo (2));
      Assert.That (results[1].Failures[0].Message, Is.EqualTo ("6"));
      Assert.That (results[1].Failures[1].Message, Is.EqualTo ("Fail"));

      Assert.That (results[1].Exceptions.Count, Is.EqualTo (2));
      Assert.That (results[1].Exceptions[0].Exception, Is.EqualTo (exception));
      Assert.That (results[1].Exceptions[1].Exception, Is.EqualTo (exception));

      Assert.That (results[2].Definition, Is.EqualTo (bt3));
      Assert.That (results[2].Successes.Count, Is.EqualTo (1));
      Assert.That (results[2].Failures.Count, Is.EqualTo (1));
      Assert.That (results[2].Warnings.Count, Is.EqualTo (1));
      Assert.That (results[2].Exceptions.Count, Is.EqualTo (1));

      Assert.That (results[3].Definition, Is.EqualTo (bt4));

      Assert.That (results[3].Successes.Count, Is.EqualTo (1));
      Assert.That (results[3].Successes[0].Message, Is.EqualTo ("Success2"));

      Assert.That (results[3].Warnings.Count, Is.EqualTo (1));
      Assert.That (results[3].Warnings[0].Message, Is.EqualTo ("Warn2"));

      Assert.That (results[3].Failures.Count, Is.EqualTo (1));
      Assert.That (results[3].Failures[0].Message, Is.EqualTo ("Fail2"));

      Assert.That (results[3].Exceptions.Count, Is.EqualTo (1));
      Assert.That (results[3].Exceptions[0].Exception, Is.EqualTo (exception));
    }

    private void AssertVisitedEquivalent (HashSet<IVisitableDefinition> visitedDefinitions, IVisitableDefinition expectedDefinition)
    {
      var match = visitedDefinitions.Where (def => DefinitionsMatch (expectedDefinition, def)).Any ();
      var message = string.Format ("Expected {0} '{1}' to be visited.", expectedDefinition.GetType ().Name, expectedDefinition.FullName);
      Assert.That (match, message);
    }

    private bool DefinitionsMatch (IVisitableDefinition expectedDefinition, IVisitableDefinition actualDefinition)
    {
      return actualDefinition.GetType () == expectedDefinition.GetType ()
          && actualDefinition.FullName == expectedDefinition.FullName
          && ((actualDefinition.Parent == null && expectedDefinition.Parent == null)
              || DefinitionsMatch (actualDefinition.Parent, expectedDefinition.Parent));
    }
  }
}
