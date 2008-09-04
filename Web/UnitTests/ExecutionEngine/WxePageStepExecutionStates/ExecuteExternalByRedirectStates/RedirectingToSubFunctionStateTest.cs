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
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Web.ExecutionEngine.WxePageStepExecutionStates;
using Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.ExecuteExternalByRedirectStates;
using Remotion.Web.Utilities;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.WxePageStepExecutionStates.ExecuteExternalByRedirectStates
{
  [TestFixture]
  public class RedirectingToSubFunctionStateTest : TestBase
  {
    private IExecutionState _executionState;

    public override void SetUp ()
    {
      base.SetUp();

      UrlMappingConfiguration.Current.Mappings.Add (new UrlMappingEntry (RootFunction.GetType(), "~/root.wxe"));
      UrlMappingConfiguration.Current.Mappings.Add (new UrlMappingEntry (SubFunction.GetType(), "~/sub.wxe"));

      Uri uri = new Uri ("http://localhost/root.wxe");

      ResponseMock.Stub (stub => stub.ApplyAppPathModifier ("~/sub.wxe")).Return ("~/session/sub.wxe").Repeat.Any();
      ResponseMock.Stub (stub => stub.ApplyAppPathModifier ("~/session/sub.wxe")).Return ("~/session/sub.wxe").Repeat.Any();
      ResponseMock.Stub (stub => stub.ApplyAppPathModifier ("~/root.wxe")).Return ("~/session/root.wxe").Repeat.Any();
      ResponseMock.Stub (stub => stub.ApplyAppPathModifier ("~/session/root.wxe")).Return ("~/session/root.wxe").Repeat.Any();
      ResponseMock.Stub (stub => stub.ApplyAppPathModifier ("/root.wxe")).Return ("/session/root.wxe").Repeat.Any();
      ResponseMock.Stub (stub => stub.ApplyAppPathModifier ("/session/root.wxe")).Return ("/session/root.wxe").Repeat.Any();
      ResponseMock.Stub (stub => stub.ContentEncoding).Return (Encoding.Default).Repeat.Any();

      RequestMock.Stub (stub => stub.Url).Return (uri).Repeat.Any();
      RequestMock.Stub (stub => stub.ContentEncoding).Return (Encoding.Default).Repeat.Any();

      _executionState = new RedirectingToSubFunctionState (
          ExecutionStateContextMock, new RedirectingToSubFunctionExecutionStateParameters (SubFunction, PostBackCollection, "~/destination.wxe"));
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
        ResponseMock.Expect (mock => mock.Redirect (Arg<string>.Is.NotNull))
            .Do (
            invocation =>
            {
              Assert.That (invocation.Arguments[0], Is.EqualTo ("~/destination.wxe"));
              Thread.CurrentThread.Abort();
            });
        ExecutionStateContextMock.Expect (
            mock => mock.SetExecutionState (Arg<IExecutionState>.Is.NotNull))
            .Do (
            invocation =>
            {
              Assert.That (invocation.Arguments[0], Is.InstanceOfType (typeof (ExecutingSubFunctionState)));
              var nextState = (ExecutingSubFunctionState) invocation.Arguments[0];
              Assert.That (nextState.ExecutionStateContext, Is.SameAs (ExecutionStateContextMock));
              Assert.That (nextState.Parameters.SubFunction, Is.SameAs (SubFunction));
            });
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
      using (MockRepository.Ordered())
      {
        ResponseMock.Expect (mock => mock.Redirect (Arg.Text.StartsWith ("~/destination.wxe")));
      }

      MockRepository.ReplayAll();

      _executionState.ExecuteSubFunction (WxeContext);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void PostProcessSubFunction ()
    {
      _executionState.PostProcessSubFunction (WxeContext);
    }
  }
}