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
using Remotion.Web.Infrastructure;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Remotion.Web.Utilities;
using Rhino.Mocks;
using Mocks_Is = Rhino.Mocks.Constraints.Is;

namespace Remotion.Web.UnitTests.ExecutionEngine.WxePageStepExecutionStates.ExecuteWithPermaUrlStates
{
  [TestFixture]
  public class RedirectToSubFunctionStateTest : TestBase
  {
    private MockRepository _mockRepository;
    private TestFunction _rootFunction;
    private OtherTestFunction _subFunction;
    private IHttpContext _httpContextMock;
    private WxeFunctionState _functionState;
    private WxeContext _wxeContext;
    private IWxePageStepExecutionStateContext _executionStateContextMock;
    private IHttpResponse _responseMock;
    private IHttpRequest _requestMock;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();

      _rootFunction = new TestFunction();
      _subFunction = new OtherTestFunction ("Value");

      _httpContextMock = _mockRepository.DynamicMock<IHttpContext>();
      _functionState = new WxeFunctionState (_rootFunction, true);
      _wxeContext = new WxeContext (_httpContextMock, _functionState, new NameValueCollection());

      _executionStateContextMock = _mockRepository.StrictMock<IWxePageStepExecutionStateContext>();

      UrlMappingConfiguration.Current.Mappings.Add (new UrlMappingEntry (_rootFunction.GetType(), "~/root.wxe"));
      UrlMappingConfiguration.Current.Mappings.Add (new UrlMappingEntry (_subFunction.GetType(), "~/sub.wxe"));

      Uri uri = new Uri ("http://localhost/root.wxe");

      _responseMock = _mockRepository.StrictMock<IHttpResponse>();
      _responseMock.Stub (stub => stub.ApplyAppPathModifier ("~/sub.wxe")).Return ("~/session/sub.wxe").Repeat.Any();
      _responseMock.Stub (stub => stub.ApplyAppPathModifier ("~/session/sub.wxe")).Return ("~/session/sub.wxe").Repeat.Any();
      _responseMock.Stub (stub => stub.ApplyAppPathModifier ("~/root.wxe")).Return ("/session/root.wxe").Repeat.Any();
      _responseMock.Stub (stub => stub.ApplyAppPathModifier ("/root.wxe")).Return ("/session/root.wxe").Repeat.Any();
      _responseMock.Stub (stub => stub.ApplyAppPathModifier ("/session/root.wxe")).Return ("/session/root.wxe").Repeat.Any();
      _responseMock.Stub (stub => stub.ContentEncoding).Return (Encoding.Default).Repeat.Any();
      _httpContextMock.Stub (stub => stub.Response).Return (_responseMock).Repeat.Any();

      _requestMock = _mockRepository.StrictMock<IHttpRequest>();
      _requestMock.Stub (stub => stub.Url).Return (uri).Repeat.Any();
      _requestMock.Stub (stub => stub.ContentEncoding).Return (Encoding.Default).Repeat.Any();
      _httpContextMock.Stub (stub => stub.Request).Return (_requestMock).Repeat.Any();
    }

    [Test]
    public void RedirectToSubFunction_WithPermaUrl ()
    {
      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions();
      IWxePageStepExecutionState executionState = new RedirectingToSubFunctionState (_executionStateContextMock, _subFunction, permaUrlOptions);

      using (_mockRepository.Ordered())
      {
        _responseMock.Expect (mock => mock.Redirect ("~/session/sub.wxe?Parameter1=Value&WxeFunctionToken=" + _wxeContext.FunctionToken))
            .Do (invocation => Thread.CurrentThread.Abort());
        _executionStateContextMock.Expect (
            mock => mock.SetExecutionState (Arg<IWxePageStepExecutionState>.Is.NotNull))
            .Do (
            invocation =>
            {
              Assert.That (invocation.Arguments[0], Is.InstanceOfType (typeof (ExecutingSubFunctionState)));
              ExecutingSubFunctionState nextState = (ExecutingSubFunctionState) invocation.Arguments[0];
              Assert.That (nextState.ExecutionStateContext, Is.SameAs (_executionStateContextMock));
              Assert.That (nextState.SubFunction, Is.SameAs (_subFunction));
              Assert.That (nextState.ResumeUrl, Is.EqualTo ("/session/root.wxe?WxeFunctionToken=" + _wxeContext.FunctionToken));
            });
      }

      _mockRepository.ReplayAll();

      try
      {
        //Redirect to subfunction
        executionState.RedirectToSubFunction (_wxeContext);
        Assert.Fail();
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort ();
      }
      finally
      {
        _mockRepository.VerifyAll();
      }
    }

    [Test]
    public void RedirectToSubFunction_WithPermaUrl_WithCustumUrlParamters ()
    {
      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions (false, new NameValueCollection { { "Key", "Value" } });
      IWxePageStepExecutionState executionState = new RedirectingToSubFunctionState (_executionStateContextMock, _subFunction, permaUrlOptions);

      using (_mockRepository.Ordered())
      {
        _responseMock.Expect (mock => mock.Redirect ("~/session/sub.wxe?Key=Value&WxeFunctionToken=" + _wxeContext.FunctionToken))
            .Do (invocation => Thread.CurrentThread.Abort());
        _executionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<IWxePageStepExecutionState>.Is.NotNull));
      }

      _mockRepository.ReplayAll();

      try
      {
        //Redirect to subfunction
        executionState.RedirectToSubFunction (_wxeContext);
        Assert.Fail ();
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort ();
      }
      finally
      {
        _mockRepository.VerifyAll ();
      }
    }

    [Test]
    public void RedirectToSubFunction_WithPermaUrl_WithParentPermaUrl ()
    {
      _wxeContext.QueryString.Add ("Key", "Value");

      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions (true);
      IWxePageStepExecutionState executionState = new RedirectingToSubFunctionState (_executionStateContextMock, _subFunction, permaUrlOptions);

      string redirectUrl = UrlUtility.AddParameters (
          "~/session/sub.wxe",
          new NameValueCollection
          {
              { "Parameter1", "Value" },
              { WxeHandler.Parameters.WxeFunctionToken, _wxeContext.FunctionToken },
              { WxeHandler.Parameters.ReturnUrl, "/root.wxe?Key=Value" }
          },
          Encoding.Default);

      using (_mockRepository.Ordered())
      {
        _responseMock.Expect (mock => mock.Redirect (redirectUrl)).Do (invocation => Thread.CurrentThread.Abort());
        _executionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<IWxePageStepExecutionState>.Is.NotNull));
      }

      _mockRepository.ReplayAll();

      try
      {
        //Redirect to subfunction
        executionState.RedirectToSubFunction (_wxeContext);
        Assert.Fail();
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort ();
      }
      finally
      {
        _mockRepository.VerifyAll ();
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void ExecuteSubFunction ()
    {
      IWxePageStepExecutionState executionState = new RedirectingToSubFunctionState (
          _executionStateContextMock, _subFunction, WxePermaUrlOptions.Null);
      executionState.ExecuteSubFunction (_wxeContext);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void ReturnFromSubFunction ()
    {
      IWxePageStepExecutionState executionState = new RedirectingToSubFunctionState (
          _executionStateContextMock, _subFunction, WxePermaUrlOptions.Null);
      executionState.ReturnFromSubFunction (_wxeContext);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void PostProcessSubFunction ()
    {
      IWxePageStepExecutionState executionState = new RedirectingToSubFunctionState (
          _executionStateContextMock, _subFunction, WxePermaUrlOptions.Null);
      executionState.PostProcessSubFunction (_wxeContext);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void Cleanup ()
    {
      IWxePageStepExecutionState executionState = new RedirectingToSubFunctionState (
          _executionStateContextMock, _subFunction, WxePermaUrlOptions.Null);
      executionState.Cleanup (_wxeContext);
    }
  }
}