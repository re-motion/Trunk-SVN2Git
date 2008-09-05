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
using Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.ExecuteExternalByRedirect;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.WxePageStepExecutionStates.ExecuteExternalByRedirect
{
  [TestFixture]
  public class RedirectingToSubFunctionStateTest : TestBase
  {
    private IExecutionState _executionState;

    public override void SetUp ()
    {
      base.SetUp();

      _executionState = new RedirectingToSubFunctionState (
          ExecutionStateContextMock, new RedirectingToSubFunctionStateParameters (SubFunction, PostBackCollection, "~/destination.wxe"));
    }

    [Test]
    public void IsExecuting ()
    {
      Assert.That (_executionState.IsExecuting, Is.True);
    }

    [Test]
    public void ExecuteSubFunction_GoesToExecutingSubFunction ()
    {
      using (MockRepository.Ordered())
      {
        ResponseMock.Expect (mock => mock.Redirect ("~/destination.wxe")).Do (invocation => Thread.CurrentThread.Abort());
        ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<PostProcessingSubFunctionState>.Is.NotNull))
            .Do (invocation => CheckExecutionState ((PostProcessingSubFunctionState) invocation.Arguments[0]));
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

      MockRepository.VerifyAll();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Redirect to '~/destination.wxe' failed.",
        MatchType = MessageMatch.Contains)]
    public void ExecuteSubFunction_WithFailedRedirect ()
    {
      ResponseMock.Expect (mock => mock.Redirect ("~/destination.wxe"));

      MockRepository.ReplayAll();

      _executionState.ExecuteSubFunction (WxeContext);
    }
  }
}