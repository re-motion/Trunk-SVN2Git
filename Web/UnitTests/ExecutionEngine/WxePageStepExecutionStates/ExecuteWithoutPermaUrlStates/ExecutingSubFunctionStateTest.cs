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
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Web.ExecutionEngine.WxePageStepExecutionStates;
using Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.ExecuteWithoutPermaUrlStates;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.WxePageStepExecutionStates.ExecuteWithoutPermaUrlStates
{
  [TestFixture]
  public class ExecutionSubFunctionStateTest : TestBase
  {
    private IExecutionState _executionState;

    public override void SetUp ()
    {
      base.SetUp();
      _executionState = new ExecutingSubFunctionState (ExecutionStateContextMock, new ExecutionStateParameters (SubFunction, PostBackCollection));
    }

    protected override OtherTestFunction CreateSubFunction ()
    {
      return MockRepository.StrictMock<OtherTestFunction>();
    }

    [Test]
    public void IsExecuting ()
    {
      Assert.That (_executionState.IsExecuting, Is.True);
    }

    [Test]
    public void ExecuteSubFunction_GoesToPostProcessingSubFunction ()
    {
      using (MockRepository.Ordered())
      {
        SubFunction.Expect (mock => mock.Execute (WxeContext));
        ExecutionStateContextMock.Expect (
            mock => mock.SetExecutionState (Arg<IExecutionState>.Is.NotNull))
            .Do (
            invocation =>
            {
              Assert.That (invocation.Arguments[0], Is.InstanceOfType (typeof (PostProcessingSubFunctionState)));
              var nextState = (PostProcessingSubFunctionState) invocation.Arguments[0];
              Assert.That (nextState.ExecutionStateContext, Is.SameAs (ExecutionStateContextMock));
              Assert.That (nextState.Parameters.SubFunction, Is.SameAs (SubFunction));
            });
      }

      MockRepository.ReplayAll();

      _executionState.ExecuteSubFunction (WxeContext);

      MockRepository.VerifyAll();
    }

    [Test]
    public void ExecuteSubFunction_ReEntrancy_GoesToPostProcessingSubFunction ()
    {
      using (MockRepository.Ordered())
      {
        SubFunction.Expect (mock => mock.Execute (WxeContext)).Do (invocation => Thread.CurrentThread.Abort());
        SubFunction.Expect (mock => mock.Execute (WxeContext));
        ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<IExecutionState>.Is.NotNull));
      }

      MockRepository.ReplayAll();

      try
      {
        _executionState.ExecuteSubFunction (WxeContext);
        Assert.Fail();
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort();
      }

      _executionState.ExecuteSubFunction (WxeContext);
      
      MockRepository.VerifyAll();
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void PostProcessSubFunction ()
    {
      _executionState.PostProcessSubFunction (WxeContext);
    }
  }
}