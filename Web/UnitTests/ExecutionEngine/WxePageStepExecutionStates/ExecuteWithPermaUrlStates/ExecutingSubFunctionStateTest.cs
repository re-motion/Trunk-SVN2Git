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
using Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.ExecuteWithPermaUrlStates;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.WxePageStepExecutionStates.ExecuteWithPermaUrlStates
{
  [TestFixture]
  public class ExecutionSubFunctionStateTest : TestBase
  {
    private IExecutionState _executionState;

    public override void SetUp ()
    {
      base.SetUp();
      _executionState = new ExecutingSubFunctionState (ExecutionStateContextMock, new ReturningFromSubFunctionStateParameters (SubFunction, PostBackCollection, "/resumeUrl.wxe"));
    }

    protected override OtherTestFunction CreateSubFunction ()
    {
      return MockRepository.StrictMock<OtherTestFunction>();
    }

    [Test]
    public void ExecuteSubFunction_GoesToReturningFromSubFunction ()
    {
      using (MockRepository.Ordered())
      {
        SubFunction.Expect (mock => mock.Execute (WxeContext));
        ExecutionStateContextMock.Expect (
            mock => mock.SetExecutionState (Arg<IExecutionState>.Is.NotNull))
            .Do (
            invocation =>
            {
              Assert.That (invocation.Arguments[0], Is.InstanceOfType (typeof (ReturningFromSubFunctionState)));
              var nextState = (ReturningFromSubFunctionState) invocation.Arguments[0];
              Assert.That (nextState.ExecutionStateContext, Is.SameAs (ExecutionStateContextMock));
              Assert.That (nextState.Parameters.SubFunction, Is.SameAs (SubFunction));
              Assert.That (nextState.Parameters.ResumeUrl, Is.EqualTo ("/resumeUrl.wxe"));
            });
      }

      MockRepository.ReplayAll();

      bool isContinuing = _executionState.ExecuteSubFunction (WxeContext);

      MockRepository.VerifyAll();
      Assert.That (isContinuing, Is.True);
    }

    [Test]
    public void ExecuteSubFunction_ReEntrancy_GoesToReturningFromSubFunction ()
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
    [ExpectedException (typeof (InvalidOperationException))]
    public void PostProcessSubFunction ()
    {
      _executionState.PostProcessSubFunction (WxeContext);
    }
  }
}