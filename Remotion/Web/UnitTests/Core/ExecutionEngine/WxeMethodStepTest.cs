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
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;

namespace Remotion.Web.UnitTests.ExecutionEngine
{

[TestFixture]
public class WxeMethodStepTest: WxeTest
{
  private TestFunction _function;
  private TestFunctionWithInvalidSteps _functionWithInvalidSteps;

  [SetUp]
  public override void SetUp()
  {
    base.SetUp();

    _function = new TestFunction();
    _functionWithInvalidSteps = new TestFunctionWithInvalidSteps();
  }

  [Test]
  [ExpectedException (typeof (WxeException))]
  public void CheckCtorArgumentNotInstanceMember()
  {
    Type functionType = typeof (TestFunctionWithInvalidSteps);
    MethodInfo step1 = functionType.GetMethod ("InvalidStep1", BindingFlags.Static | BindingFlags.NonPublic);
    WxeMethodStep methodStep = new WxeMethodStep (_functionWithInvalidSteps, step1);
  }

  [Test]
  [ExpectedException (typeof (WxeException))]
  public void CheckCtorArgumentWrongParameterType()
  {
    Type functionType = typeof (TestFunctionWithInvalidSteps);
    MethodInfo step2 = functionType.GetMethod ("InvalidStep2", BindingFlags.Instance | BindingFlags.NonPublic);
    WxeMethodStep methodStep = new WxeMethodStep (_functionWithInvalidSteps, step2);
  }

  [Test]
  [ExpectedException (typeof (WxeException))]
  public void CheckCtorArgumentTooManyParameters()
  {
    Type functionType = typeof (TestFunctionWithInvalidSteps);
    MethodInfo step3 = functionType.GetMethod ("InvalidStep3", BindingFlags.Instance | BindingFlags.NonPublic);
    WxeMethodStep methodStep = new WxeMethodStep (_functionWithInvalidSteps, step3);
  }

  [Test]
  [ExpectedException (typeof (WxeException))]
  public void CheckCtorArgumentWrongStepListInstance()
  {
    Type functionType = typeof (TestFunction);
    MethodInfo step1 = functionType.GetMethod ("Step1", BindingFlags.Instance | BindingFlags.NonPublic);
    WxeMethodStep methodStep = new WxeMethodStep (_functionWithInvalidSteps, step1);
  }

  [Test]
  public void ExecuteMethodStep()
  {
    Type functionType = typeof (TestFunction);
    MethodInfo step1 = functionType.GetMethod ("Step1", BindingFlags.Instance | BindingFlags.NonPublic);
    WxeMethodStep methodStep = new WxeMethodStep (_function, step1);
    
    methodStep.Execute (CurrentWxeContext);
    
    Assert.AreEqual ("1", _function.LastExecutedStepID);
  }

  [Test]
  public void MethodStepWithDelegate ()
  {
    WxeMethodStep methodStep = new WxeMethodStep (_function.PublicStepMethod);
    Assert.AreEqual ("PublicStepMethod", PrivateInvoke.GetNonPublicField (methodStep, "_methodName"));
  }

  [Test]
  public void MethodStepWithDelegateWithContext ()
  {
    WxeMethodStep methodStep = new WxeMethodStep (_function.PublicStepMethodWithContext);
    Assert.AreEqual ("PublicStepMethodWithContext", PrivateInvoke.GetNonPublicField (methodStep, "_methodName"));
  }

  [Test]
  [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The delegate must contain a single method.", MatchType = MessageMatch.Contains)]
  public void MethodStepWithDelegateThrowsOnMultiDelegate ()
  {
    Action multiDelegate = _function.PublicStepMethod;
    multiDelegate += _function.PublicStepMethod;
    new WxeMethodStep (multiDelegate);
  }

  [Test]
  [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The delegate's target must be a non-null WxeStepList.",
      MatchType = MessageMatch.Contains)]
  public void MethodStepWithDelegateThrowsOnInvalidTarget ()
  {
    new WxeMethodStep (MethodStepWithDelegate);
  }

  [Test]
  public void ExecuteMethodStepWithDelegate ()
  {
    WxeMethodStep methodStep = new WxeMethodStep (_function.PublicStepMethod);

    Assert.AreNotEqual ("1", _function.LastExecutedStepID);

    methodStep.Execute (CurrentWxeContext);

    Assert.AreEqual ("1", _function.LastExecutedStepID);
  }

  [Test]
  public void ExecuteMethodStepWithContext()
  {
    Type functionType = typeof (TestFunction);
    MethodInfo step2 = functionType.GetMethod ("Step2", BindingFlags.Instance | BindingFlags.NonPublic);
    WxeMethodStep methodStepWithContext = new WxeMethodStep (_function, step2);
    
    methodStepWithContext.Execute (CurrentWxeContext);
    
    Assert.AreEqual ("2", _function.LastExecutedStepID);
    Assert.AreSame (CurrentWxeContext, _function.WxeContextStep2);
  }

  [Test]
  public void ExecuteMethodStepWithDelegateWithContext()
  {
    WxeMethodStep methodStepWithContext = new WxeMethodStep (_function.PublicStepMethodWithContext);
    
    methodStepWithContext.Execute (CurrentWxeContext);
    
    Assert.AreEqual ("2", _function.LastExecutedStepID);
    Assert.AreSame (CurrentWxeContext, _function.WxeContextStep2);
  }
}

}
