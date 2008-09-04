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
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Web.ExecutionEngine.WxePageStepExecutionStates;
using Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.ExecuteWithPermaUrlStates;
using Remotion.Web.Utilities;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.WxePageStepExecutionStates.ExecuteWithPermaUrlStates
{
  [TestFixture]
  public class RedirectingToSubFunctionStateTest : TestBase
  {
    public override void SetUp ()
    {
      base.SetUp();

      UrlMappingConfiguration.Current.Mappings.Add (new UrlMappingEntry (RootFunction.GetType(), "~/root.wxe"));
      UrlMappingConfiguration.Current.Mappings.Add (new UrlMappingEntry (SubFunction.GetType(), "~/sub.wxe"));

      Uri uri = new Uri ("http://localhost/root.wxe");

      ResponseMock.Stub (stub => stub.ApplyAppPathModifier ("~/sub.wxe")).Return ("~/session/sub.wxe").Repeat.Any();
      ResponseMock.Stub (stub => stub.ApplyAppPathModifier ("~/session/sub.wxe")).Return ("~/session/sub.wxe").Repeat.Any();
      ResponseMock.Stub (stub => stub.ApplyAppPathModifier ("~/root.wxe")).Return ("~/session/root.wxe").Repeat.Any();
      ResponseMock.Stub (stub => stub.ApplyAppPathModifier ("/root.wxe")).Return ("/session/root.wxe").Repeat.Any();
      ResponseMock.Stub (stub => stub.ApplyAppPathModifier ("/session/root.wxe")).Return ("/session/root.wxe").Repeat.Any();
      ResponseMock.Stub (stub => stub.ContentEncoding).Return (Encoding.Default).Repeat.Any();

      RequestMock.Stub (stub => stub.Url).Return (uri).Repeat.Any();
      RequestMock.Stub (stub => stub.ContentEncoding).Return (Encoding.Default).Repeat.Any();
    }

    [Test]
    public void IsExecuting ()
    {
      IExecutionState executionState = CreateExecutionState(new WxePermaUrlOptions());
      Assert.That (executionState.IsExecuting, Is.True);
    }

    [Test]
    public void ExecuteSubFunction_WithPermaUrl_GoesToExecutingSubFunction ()
    {
      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions();
      IExecutionState executionState = CreateExecutionState (permaUrlOptions);

      using (MockRepository.Ordered())
      {
        ResponseMock.Expect (mock => mock.Redirect ("~/session/sub.wxe?Parameter1=OtherValue&WxeFunctionToken=" + WxeContext.FunctionToken))
            .Do (invocation => Thread.CurrentThread.Abort());
        ExecutionStateContextMock.Expect (
            mock => mock.SetExecutionState (Arg<IExecutionState>.Is.NotNull))
            .Do (
            invocation =>
            {
              Assert.That (invocation.Arguments[0], Is.InstanceOfType (typeof (ExecutingSubFunctionState)));
              var nextState = (ExecutingSubFunctionState) invocation.Arguments[0];
              Assert.That (nextState.ExecutionStateContext, Is.SameAs (ExecutionStateContextMock));
              Assert.That (nextState.Parameters.SubFunction, Is.SameAs (SubFunction));
              Assert.That (nextState.Parameters.ResumeUrl, Is.EqualTo ("/session/root.wxe?WxeFunctionToken=" + WxeContext.FunctionToken));
            });
      }

      MockRepository.ReplayAll();

      try
      {
        executionState.ExecuteSubFunction (WxeContext);
        Assert.Fail();
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort();
      }

      MockRepository.VerifyAll();
    }

    [Test]
    public void ExecuteSubFunction_WithPermaUrl_WithCustumUrlParamters_GoesToExecutingSubFunction ()
    {
      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions (false, new NameValueCollection { { "Key", "Value" } });
      IExecutionState executionState = CreateExecutionState (permaUrlOptions);

      using (MockRepository.Ordered())
      {
        ResponseMock.Expect (mock => mock.Redirect ("~/session/sub.wxe?Key=Value&WxeFunctionToken=" + WxeContext.FunctionToken))
            .Do (invocation => Thread.CurrentThread.Abort());
        ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<IExecutionState>.Is.NotNull));
      }

      MockRepository.ReplayAll();

      try
      {
        executionState.ExecuteSubFunction (WxeContext);
        Assert.Fail();
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort();
      }

      MockRepository.VerifyAll();
    }

    [Test]
    public void ExecuteSubFunction_WithPermaUrl_WithParentPermaUrl_GoesToExecutingSubFunction ()
    {
      WxeContext.QueryString.Add ("Key", "Value");

      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions (true);
      IExecutionState executionState = CreateExecutionState (permaUrlOptions);

      string redirectUrl = UrlUtility.AddParameters (
          "~/session/sub.wxe",
          new NameValueCollection
          {
              { "Parameter1", "Value" },
              { WxeHandler.Parameters.WxeFunctionToken, WxeContext.FunctionToken },
              { WxeHandler.Parameters.ReturnUrl, "/root.wxe?Key=Value" }
          },
          Encoding.Default);

      using (MockRepository.Ordered())
      {
        ResponseMock.Expect (mock => mock.Redirect (redirectUrl)).Do (invocation => Thread.CurrentThread.Abort());
        ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<IExecutionState>.Is.NotNull));
      }

      MockRepository.ReplayAll();

      try
      {
        executionState.ExecuteSubFunction (WxeContext);
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
        ExpectedMessage = "Redirect to '~/session/sub.wxe?Parameter1=OtherValue&WxeFunctionToken=",
        MatchType = MessageMatch.Contains)]
    public void ExecuteSubFunction_WithFailedRedirect ()
    {
      IExecutionState executionState = CreateExecutionState (new WxePermaUrlOptions());
      using (MockRepository.Ordered())
      {
        ResponseMock.Expect (mock => mock.Redirect ("~/session/sub.wxe?Parameter1=OtherValue&WxeFunctionToken=" + WxeContext.FunctionToken));
      }

      MockRepository.ReplayAll();

      executionState.ExecuteSubFunction (WxeContext);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void PostProcessSubFunction ()
    {
      IExecutionState executionState = CreateExecutionState (new WxePermaUrlOptions());
      executionState.PostProcessSubFunction (WxeContext);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The 'RedirectingToSubFunctionState' type only supports WxePermaUrlOptions with the UsePermaUrl-flag set to true.\r\n"
        +"Parameter name: parameters",
        MatchType = MessageMatch.Contains)]
    public void Initialize_WithoutPermaUrl ()
    {
      CreateExecutionState (WxePermaUrlOptions.Null);
    }

    private RedirectingToSubFunctionState CreateExecutionState (WxePermaUrlOptions permaUrlOptions)
    {
      return new RedirectingToSubFunctionState (
          ExecutionStateContextMock, new PreparingSubFunctionStateParameters (SubFunction, PostBackCollection, permaUrlOptions));
    }
  }
}