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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.UnitTests.Mixins.ValidationTests.ValidationSampleTypes;

namespace Remotion.UnitTests.Mixins.ValidationTests
{
  [TestFixture]
  public class GeneralValidationTest : ValidationTestBase
  {
    [Test]
    public void ValidationVisitsSomething ()
    {
      IValidationLog log = MixinConfiguration.ActiveConfiguration.Validate ();
      Assert.IsTrue (log.ResultCount > 1);
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
      IValidationLog log = MixinConfiguration.ActiveConfiguration.Validate ();

      IEnumerator<ValidationResult> results = log.GetResults ().GetEnumerator ();
      Assert.IsTrue (results.MoveNext ());
      ValidationResult firstResult = results.Current;
      Assert.IsNotNull (firstResult.Definition);
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
      Assert.IsTrue (log.GetNumberOfRulesExecuted () > 0);
    }

    [Test]
    public void CollectsUnexpectedExceptions ()
    {
      TargetClassDefinition bc = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (DateTime));
      DefaultValidationLog log = Validator.Validate (bc, new ThrowingRuleSet ());
      Assert.IsTrue (log.GetNumberOfUnexpectedExceptions () > 0);
      List<ValidationResult> results = new List<ValidationResult> (log.GetResults ());
      Assert.IsTrue (results[0].Exceptions[0].Exception is InvalidOperationException);
    }


    [Test]
    public void DefaultConfiguration_AllIsVisitedOnce ()
    {
      IValidationLog log = MixinConfiguration.ActiveConfiguration.Validate ();

      Dictionary<IVisitableDefinition, IVisitableDefinition> visitedDefinitions = new Dictionary<IVisitableDefinition, IVisitableDefinition> ();
      foreach (ValidationResult result in log.GetResults ())
      {
        Assert.IsNotNull (result.Definition);
        Assert.IsFalse (visitedDefinitions.ContainsKey (result.Definition));
        visitedDefinitions.Add (result.Definition, result.Definition);
      }

      TargetClassDefinition bt1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1));
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt1));
      TargetClassDefinition bt3 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3));
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt3));
      TargetClassDefinition btWithAdditionalDependencies = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (TargetClassWithAdditionalDependencies));
      Assert.IsTrue (visitedDefinitions.ContainsKey (btWithAdditionalDependencies));

      MixinDefinition bt1m1 = bt1.Mixins[typeof (BT1Mixin1)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt1m1));
      MixinDefinition bt1m2 = bt1.Mixins[typeof (BT1Mixin2)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt1m2));
      MixinDefinition bt3m1 = bt3.Mixins[typeof (BT3Mixin1)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt3m1));
      MixinDefinition bt3m2 = bt3.Mixins[typeof (BT3Mixin2)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt3m2));
      MixinDefinition bt3m3 = bt3.GetMixinByConfiguredType (typeof (BT3Mixin3<,>));
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt3m3));
      MixinDefinition bt3m4 = bt3.Mixins[typeof (BT3Mixin4)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt3m4));
      MixinDefinition bt3m5 = bt3.Mixins[typeof (BT3Mixin5)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt3m5));

      MethodDefinition m1 = bt1.Methods[typeof (BaseType1).GetMethod ("VirtualMethod", Type.EmptyTypes)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (m1));
      MethodDefinition m2 = bt1.Methods[typeof (BaseType1).GetMethod ("VirtualMethod", new Type[] { typeof (string) })];
      Assert.IsTrue (visitedDefinitions.ContainsKey (m2));
      MethodDefinition m3 = bt1m1.Methods[typeof (BT1Mixin1).GetMethod ("VirtualMethod")];
      Assert.IsTrue (visitedDefinitions.ContainsKey (m3));
      MethodDefinition m4 = bt1m1.Methods[typeof (BT1Mixin1).GetMethod ("IntroducedMethod")];
      Assert.IsTrue (visitedDefinitions.ContainsKey (m4));

      PropertyDefinition p1 = bt1.Properties[typeof (BaseType1).GetProperty ("VirtualProperty")];
      Assert.IsTrue (visitedDefinitions.ContainsKey (p1));
      MethodDefinition m5 = p1.GetMethod;
      Assert.IsTrue (visitedDefinitions.ContainsKey (m5));
      MethodDefinition m6 = p1.SetMethod;
      Assert.IsTrue (visitedDefinitions.ContainsKey (m6));
      PropertyDefinition p2 = bt1m1.Properties[typeof (BT1Mixin1).GetProperty ("VirtualProperty")];
      Assert.IsTrue (visitedDefinitions.ContainsKey (p2));

      EventDefinition e1 = bt1.Events[typeof (BaseType1).GetEvent ("VirtualEvent")];
      Assert.IsTrue (visitedDefinitions.ContainsKey (e1));
      MethodDefinition m7 = e1.AddMethod;
      Assert.IsTrue (visitedDefinitions.ContainsKey (m7));
      MethodDefinition m8 = e1.RemoveMethod;
      Assert.IsTrue (visitedDefinitions.ContainsKey (m8));
      EventDefinition e2 = bt1m1.Events[typeof (BT1Mixin1).GetEvent ("VirtualEvent")];
      Assert.IsTrue (visitedDefinitions.ContainsKey (e2));

      InterfaceIntroductionDefinition i1 = bt1m1.InterfaceIntroductions[typeof (IBT1Mixin1)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (i1));
      MethodIntroductionDefinition im1 = i1.IntroducedMethods[typeof (IBT1Mixin1).GetMethod ("IntroducedMethod")];
      Assert.IsTrue (visitedDefinitions.ContainsKey (im1));
      PropertyIntroductionDefinition im2 = i1.IntroducedProperties[typeof (IBT1Mixin1).GetProperty ("IntroducedProperty")];
      Assert.IsTrue (visitedDefinitions.ContainsKey (im2));
      EventIntroductionDefinition im3 = i1.IntroducedEvents[typeof (IBT1Mixin1).GetEvent ("IntroducedEvent")];
      Assert.IsTrue (visitedDefinitions.ContainsKey (im3));

      AttributeDefinition a1 = bt1.CustomAttributes.GetFirstItem (typeof (BT1Attribute));
      Assert.IsTrue (visitedDefinitions.ContainsKey (a1));
      AttributeDefinition a2 = bt1m1.CustomAttributes.GetFirstItem (typeof (BT1M1Attribute));
      Assert.IsTrue (visitedDefinitions.ContainsKey (a2));
      AttributeDefinition a3 = m1.CustomAttributes.GetFirstItem (typeof (BT1Attribute));
      Assert.IsTrue (visitedDefinitions.ContainsKey (a3));
      AttributeDefinition a4 = p1.CustomAttributes.GetFirstItem (typeof (BT1Attribute));
      Assert.IsTrue (visitedDefinitions.ContainsKey (a4));
      AttributeDefinition a5 = e1.CustomAttributes.GetFirstItem (typeof (BT1Attribute));
      Assert.IsTrue (visitedDefinitions.ContainsKey (a5));
      AttributeDefinition a6 = im1.ImplementingMember.CustomAttributes.GetFirstItem (typeof (BT1M1Attribute));
      Assert.IsTrue (visitedDefinitions.ContainsKey (a6));
      AttributeDefinition a7 = im2.ImplementingMember.CustomAttributes.GetFirstItem (typeof (BT1M1Attribute));
      Assert.IsTrue (visitedDefinitions.ContainsKey (a7));
      AttributeDefinition a8 = im3.ImplementingMember.CustomAttributes.GetFirstItem (typeof (BT1M1Attribute));
      Assert.IsTrue (visitedDefinitions.ContainsKey (a8));

      AttributeIntroductionDefinition ai1 = bt1.IntroducedAttributes.GetFirstItem (typeof (BT1M1Attribute));
      Assert.IsTrue (visitedDefinitions.ContainsKey (ai1));
      AttributeIntroductionDefinition ai2 = m1.IntroducedAttributes.GetFirstItem (typeof (BT1M1Attribute));
      Assert.IsTrue (visitedDefinitions.ContainsKey (ai2));

      RequiredBaseCallTypeDefinition bc1 = bt3.RequiredBaseCallTypes[typeof (IBaseType34)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (bc1));
      RequiredMethodDefinition bcm1 = bc1.Methods[typeof (IBaseType34).GetMethod ("IfcMethod")];
      Assert.IsTrue (visitedDefinitions.ContainsKey (bcm1));

      RequiredFaceTypeDefinition ft1 = bt3.RequiredFaceTypes[typeof (IBaseType32)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (ft1));
      RequiredMethodDefinition fm1 = ft1.Methods[typeof (IBaseType32).GetMethod ("IfcMethod")];
      Assert.IsTrue (visitedDefinitions.ContainsKey (fm1));

      RequiredMixinTypeDefinition rmt1 = btWithAdditionalDependencies.RequiredMixinTypes[typeof (IMixinWithAdditionalClassDependency)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (rmt1));
      RequiredMixinTypeDefinition rmt2 = btWithAdditionalDependencies.RequiredMixinTypes[typeof (MixinWithNoAdditionalDependency)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (rmt2));

      ThisDependencyDefinition td1 = bt3m1.ThisDependencies[typeof (IBaseType31)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (td1));

      BaseDependencyDefinition bd1 = bt3m1.BaseDependencies[typeof (IBaseType31)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (bd1));

      MixinDependencyDefinition md1 = btWithAdditionalDependencies.Mixins[typeof (MixinWithAdditionalClassDependency)].MixinDependencies[typeof (MixinWithNoAdditionalDependency)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (md1));
    }

    [Test]
    public void ValidationException ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassOverridingSingleMixinMethod), typeof (AbstractMixinWithoutBase));
      DefaultValidationLog log = Validator.Validate (definition);

      ValidationException exception = new ValidationException (log);
      Assert.AreEqual ("Some parts of the mixin configuration could not be validated." + Environment.NewLine + "Remotion.UnitTests.Mixins."
          + "ValidationTests.ValidationSampleTypes.AbstractMixinWithoutBase.AbstractMethod (Remotion.UnitTests.Mixins.ValidationTests.ValidationSampleTypes."
          + "AbstractMixinWithoutBase -> Remotion.UnitTests.Mixins.SampleTypes.ClassOverridingSingleMixinMethod): There were 1 errors, 0 warnings, and 0 unexpected "
          + "exceptions. First error: A target class overrides a method from one of its mixins, but the mixin is not derived from one of the "
          + "Mixin<...> base classes." + Environment.NewLine + "See Log.GetResults() for a full list of issues.", exception.Message);

      Assert.AreSame (log, exception.ValidationLog);
    }

    [Test]
    public void Merge ()
    {
      IValidationLog sourceLog = new DefaultValidationLog ();
      Exception exception = new Exception ();

      TargetClassDefinition bt1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1), GenerationPolicy.ForceGeneration);
      TargetClassDefinition bt2 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType2), GenerationPolicy.ForceGeneration);
      TargetClassDefinition bt3 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3), GenerationPolicy.ForceGeneration);
      TargetClassDefinition bt4 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType4), GenerationPolicy.ForceGeneration);

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
      Assert.AreEqual (5, resultLog.GetNumberOfSuccesses ());
      Assert.AreEqual (5, resultLog.GetNumberOfWarnings ());
      Assert.AreEqual (5, resultLog.GetNumberOfFailures ());
      Assert.AreEqual (5, resultLog.GetNumberOfUnexpectedExceptions ());

      List<ValidationResult> results = new List<ValidationResult> (resultLog.GetResults ());

      Assert.AreEqual (4, results.Count);

      Assert.AreEqual (bt2, results[0].Definition);
      Assert.AreEqual (1, results[0].Successes.Count);
      Assert.AreEqual (1, results[0].Failures.Count);
      Assert.AreEqual (1, results[0].Warnings.Count);
      Assert.AreEqual (1, results[0].Exceptions.Count);

      Assert.AreEqual (bt1, results[1].Definition);

      Assert.AreEqual (2, results[1].Successes.Count);
      Assert.AreEqual ("4", results[1].Successes[0].Message);
      Assert.AreEqual ("Success", results[1].Successes[1].Message);

      Assert.AreEqual (2, results[1].Warnings.Count);
      Assert.AreEqual ("5", results[1].Warnings[0].Message);
      Assert.AreEqual ("Warn", results[1].Warnings[1].Message);

      Assert.AreEqual (2, results[1].Failures.Count);
      Assert.AreEqual ("6", results[1].Failures[0].Message);
      Assert.AreEqual ("Fail", results[1].Failures[1].Message);

      Assert.AreEqual (2, results[1].Exceptions.Count);
      Assert.AreEqual (exception, results[1].Exceptions[0].Exception);
      Assert.AreEqual (exception, results[1].Exceptions[1].Exception);

      Assert.AreEqual (bt3, results[2].Definition);
      Assert.AreEqual (1, results[2].Successes.Count);
      Assert.AreEqual (1, results[2].Failures.Count);
      Assert.AreEqual (1, results[2].Warnings.Count);
      Assert.AreEqual (1, results[2].Exceptions.Count);

      Assert.AreEqual (bt4, results[3].Definition);

      Assert.AreEqual (1, results[3].Successes.Count);
      Assert.AreEqual ("Success2", results[3].Successes[0].Message);

      Assert.AreEqual (1, results[3].Warnings.Count);
      Assert.AreEqual ("Warn2", results[3].Warnings[0].Message);

      Assert.AreEqual (1, results[3].Failures.Count);
      Assert.AreEqual ("Fail2", results[3].Failures[0].Message);

      Assert.AreEqual (1, results[3].Exceptions.Count);
      Assert.AreEqual (exception, results[3].Exceptions[0].Exception);
    }
  }
}
