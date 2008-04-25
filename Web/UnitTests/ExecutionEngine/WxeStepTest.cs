using System;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{

[TestFixture]
public class WxeStepTest: WxeTest
{
  private TestFunction _rootFunction;
  private TestFunction _nestedLevel1Function;
  private TestFunction _nestedLevel2Function;
  private TestStep _rootFunctionStep;
  private TestStep _nestedLevel1FunctionStep;
  private TestStep _nestedLevel2FunctionStep;
  private TestStep _standAloneStep;

  [SetUp]
  public override void SetUp()
  {
    base.SetUp();

    _rootFunction = new TestFunction();
    _nestedLevel1Function = new TestFunction();
    _nestedLevel2Function = new TestFunction();
    _rootFunctionStep = new TestStep();
    _nestedLevel1FunctionStep = new TestStep();
    _nestedLevel2FunctionStep = new TestStep();
    _standAloneStep = new TestStep();

    _rootFunction.Add (new TestStep());
    _rootFunction.Add (new TestStep());
    _rootFunction.Add (_nestedLevel1Function);
    _rootFunction.Add (_rootFunctionStep);

    _nestedLevel1Function.Add (new TestStep());
    _nestedLevel1Function.Add (new TestStep());
    _nestedLevel1Function.Add (_nestedLevel2Function);
    _nestedLevel1Function.Add (_nestedLevel1FunctionStep);

    _nestedLevel2Function.Add (new TestStep());
    _nestedLevel2Function.Add (new TestStep());
    _nestedLevel2Function.Add (_nestedLevel2FunctionStep);
  }

  [Test]
  public void GetStepByTypeForNull()
  {
    WxeStep step = TestStep.GetStepByType (null, typeof (WxeStep));
    Assert.IsNull (step);    
  }

  [Test]
  public void GetStepByTypeForTestStep()
  {
    WxeStep step = TestStep.GetStepByType (_standAloneStep, typeof (TestStep));
    Assert.AreSame (_standAloneStep, step);    
  }

  [Test]
  public void GetStepByTypeForWxeFunction()
  {
    WxeStep step = TestStep.GetStepByType (_nestedLevel2FunctionStep, typeof (WxeFunction));
    Assert.AreSame (_nestedLevel2Function, step);    
  }

  [Test]
  public void GetStepByTypeForWrongType()
  {
    WxeStep step = TestStep.GetStepByType (_nestedLevel2FunctionStep, typeof (TestFunctionWithInvalidSteps));
    Assert.IsNull (step);    
  }

  [Test]
  public void GetFunctionForStep()
  {
    WxeFunction function = WxeStep.GetFunction (_nestedLevel1FunctionStep);
    Assert.AreSame (_nestedLevel1Function, function);    
  }

  [Test]
  public void GetFunctionForNestedFunction()
  {
    WxeFunction function = WxeStep.GetFunction (_nestedLevel1Function);
    Assert.AreSame (_nestedLevel1Function, function);    
  }

  [Test]
  public void GetParentStepForStep()
  {
    WxeStep parentStep = _nestedLevel2FunctionStep.ParentStep;
    Assert.AreSame (_nestedLevel2Function, parentStep);    
  }

  [Test]
  public void GetParentFunctionForStep()
  {
    WxeFunction parentFunction = _nestedLevel2FunctionStep.ParentFunction;
    Assert.AreSame (_nestedLevel2Function, parentFunction);    
  }

  [Test]
  public void GetParentFunctionForNestedFunction()
  {
    WxeFunction parentFunction = _nestedLevel2Function.ParentFunction;
    Assert.AreSame (_nestedLevel1Function, parentFunction);    
  }

  [Test]
  public void GetParentStepForStandAloneStep()
  {
    WxeStep parentStep = _standAloneStep.ParentStep;
    Assert.IsNull(parentStep);    
  }

  [Test]
  public void GetParentFunctionForStandAloneStep()
  {
    WxeFunction parentFunction = _standAloneStep.ParentFunction;
    Assert.IsNull (parentFunction);    
  }

  [Test]
  public void GetRootFunctionForStep()
  {
    WxeFunction rootFunction = _nestedLevel2FunctionStep.RootFunction;
    Assert.AreSame (_rootFunction, rootFunction);    
  }

  [Test]
  public void GetRootFunctionForStandAloneStep()
  {
    WxeFunction rootFunction = _standAloneStep.RootFunction;
    Assert.IsNull (rootFunction);    
  }

  [Test]
  public void GetRootFunctionForRootFunction()
  {
    WxeFunction rootFunction = _rootFunction.RootFunction;
    Assert.AreSame (_rootFunction, rootFunction);    
  }

  [Test]
  public void AbortStep()
  {
    _standAloneStep.Abort ();
    Assert.IsTrue (_standAloneStep.IsAbortRecursiveCalled);
    Assert.IsTrue (_standAloneStep.IsAborted);
  }

  [Test]
  public void ExecuteStep()
  {
    _standAloneStep.Execute ();
    Assert.IsTrue (_standAloneStep.IsExecuteCalled);
    Assert.AreSame (CurrentWxeContext, _standAloneStep.WxeContext);
  }

  [Test]
  public void SetParentStep()
  {
    TestStep parentStep = new TestStep();
    _standAloneStep.SetParentStep (parentStep);
    Assert.AreSame (parentStep, _standAloneStep.ParentStep);
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void SetParentStepNull()
  {
    _standAloneStep.SetParentStep (null);
  }

  [Test]
  public void GetVariablesForFunctionStep()
  {
    NameObjectCollection variables = _nestedLevel2FunctionStep.Variables;
    Assert.AreSame (_nestedLevel2Function.Variables, variables);
  }

  [Test]
  public void GetVariablesForStandAloneStep()
  {
    NameObjectCollection variables = _standAloneStep.Variables;
    Assert.IsNull (variables);
  }
}

}
