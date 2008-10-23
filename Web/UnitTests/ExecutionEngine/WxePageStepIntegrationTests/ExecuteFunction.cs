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
using System.Web.SessionState;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.ExecutionEngine;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.Execute;
using Remotion.Web.Infrastructure;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.WxePageStepIntegrationTests
{
  [TestFixture]
  public class ExecuteFunction : TestBase
  {
    private MockRepository _mockRepository;
    private WxePageStep _pageStep;
    private IHttpContext _httpContextMock;
    private WxeContext _wxeContext;
    private IWxePageExecutor _pageExecutorMock;
    private IWxePage _pageMock;
    private TestFunction _subFunction;
    private TestFunction _rootFunction;
    private NameValueCollection _postBackCollection;
    private WxeHandler _wxeHandler;
    private WxeFunctionState _functionState;
    private WxeFunctionStateManager _functionStateManager;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();

      _rootFunction = new TestFunction();
      _subFunction = _mockRepository.PartialMock<TestFunction>();

      _httpContextMock = _mockRepository.DynamicMock<IHttpContext>();
      _pageExecutorMock = _mockRepository.StrictMock<IWxePageExecutor>();
      _functionState = new WxeFunctionState (_rootFunction, true);

      _pageStep = _mockRepository.PartialMock<WxePageStep> ("ThePage");
      _pageStep.SetPageExecutor (_pageExecutorMock);

      _pageMock = _mockRepository.DynamicMock<IWxePage>();
      _postBackCollection = new NameValueCollection { { "Key", "Value" } };
      _wxeHandler = new WxeHandler();

      UrlMappingConfiguration.Current.Mappings.Add (new UrlMappingEntry (_rootFunction.GetType(), "~/root.wxe"));
      UrlMappingConfiguration.Current.Mappings.Add (new UrlMappingEntry (_subFunction.GetType(), "~/sub.wxe"));

      IHttpSessionState sessionStub = _mockRepository.DynamicMock<IHttpSessionState> ();
      sessionStub.Stub (stub => stub[Arg<string>.Is.NotNull]).PropertyBehavior ();

      _functionStateManager = new WxeFunctionStateManager (sessionStub);
      _wxeContext = new WxeContext (_httpContextMock, _functionStateManager, _functionState, new NameValueCollection ());
    }

    [Test]
    public void Test_SubFunction ()
    {
      WxeContextMock.SetCurrent (_wxeContext);

      using (_mockRepository.Ordered())
      {
        using (_mockRepository.Unordered ())
        {
          _pageMock.Expect (mock => mock.GetPostBackCollection()).Return (_postBackCollection);
          _pageMock.Expect (mock => mock.SaveAllState());
          _pageMock.Expect (mock => mock.WxeHandler).Return (_wxeHandler);
        }

        _subFunction.Expect (mock => mock.Execute (_wxeContext)).Do (
            invocation =>
            {
              PrivateInvoke.SetNonPublicField (_functionState, "_postBackID", 100);
              _wxeContext.PostBackCollection = new NameValueCollection();
            });

        _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "ThePage", false)).Do (
            invocation =>
            {
              Assert.That (((IExecutionStateContext) _pageStep).ExecutionState, Is.SameAs (NullExecutionState.Null));
              Assert.That (_wxeContext.PostBackCollection[WxePageInfo<WxePage>.PostBackSequenceNumberID], Is.EqualTo ("100"));
              Assert.That (_wxeContext.PostBackCollection.AllKeys, List.Contains ("Key"));
              Assert.That (_pageStep.ReturningFunction, Is.SameAs (_subFunction));
              Assert.That (_pageStep.IsReturningPostBack, Is.True);
            });
      }

      _mockRepository.ReplayAll();

      WxePermaUrlOptions permaUrlOptions = WxePermaUrlOptions.Null;
      WxeRepostOptions repostOptions = WxeRepostOptions.Null;
      _pageStep.ExecuteFunction (new PreProcessingSubFunctionStateParameters (_pageMock, _subFunction, permaUrlOptions), repostOptions);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Test_SubFunctionCompleted_ReEntrancy ()
    {
      WxeContextMock.SetCurrent (_wxeContext);

      using (_mockRepository.Ordered())
      {
        using (_mockRepository.Unordered ())
        {
          _pageMock.Expect (mock => mock.GetPostBackCollection()).Return (_postBackCollection);
          _pageMock.Expect (mock => mock.SaveAllState());
          _pageMock.Expect (mock => mock.WxeHandler).Return (_wxeHandler);
        }

        _subFunction.Expect (mock => mock.Execute (_wxeContext)).Do (invocation => Thread.CurrentThread.Abort());

        _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "ThePage", true));
      }

      _mockRepository.ReplayAll();

      try
      {
        WxePermaUrlOptions permaUrlOptions = WxePermaUrlOptions.Null;
        WxeRepostOptions repostOptions = WxeRepostOptions.Null;
        _pageStep.ExecuteFunction (new PreProcessingSubFunctionStateParameters (_pageMock, _subFunction, permaUrlOptions), repostOptions);
        Assert.Fail();
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort();
      }
      _pageStep.Execute();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Test_SubFunction_ReEntrancy ()
    {
      WxeContextMock.SetCurrent (_wxeContext);

      using (_mockRepository.Ordered ())
      {
        using (_mockRepository.Unordered ())
        {
          _pageMock.Expect (mock => mock.GetPostBackCollection()).Return (_postBackCollection);
          _pageMock.Expect (mock => mock.SaveAllState());
          _pageMock.Expect (mock => mock.WxeHandler).Return (_wxeHandler);
        }

        _subFunction.Expect (mock => mock.Execute (_wxeContext)).Do (invocation => Thread.CurrentThread.Abort());

        _subFunction.Expect (mock => mock.Execute (_wxeContext));

        _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "ThePage", true));
      }

      _mockRepository.ReplayAll();

      try
      {
        WxePermaUrlOptions permaUrlOptions = WxePermaUrlOptions.Null;
        WxeRepostOptions repostOptions = WxeRepostOptions.Null;
        _pageStep.ExecuteFunction (new PreProcessingSubFunctionStateParameters (_pageMock, _subFunction, permaUrlOptions), repostOptions);
        Assert.Fail();
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort();
      }
      _pageStep.Execute();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Test_SubFunction_RedirectToPermaUrl ()
    {
      WxeContextMock.SetCurrent (_wxeContext);
      Uri uri = new Uri ("http://localhost/root.wxe");

      IHttpResponse responseMock = _mockRepository.StrictMock<IHttpResponse>();
      responseMock.Stub (stub => stub.ApplyAppPathModifier ("~/sub.wxe")).Return ("~/session/sub.wxe").Repeat.Any();
      responseMock.Stub (stub => stub.ApplyAppPathModifier ("~/session/sub.wxe")).Return ("~/session/sub.wxe").Repeat.Any();
      responseMock.Stub (stub => stub.ApplyAppPathModifier ("/root.wxe")).Return ("/session/root.wxe").Repeat.Any();
      responseMock.Stub (stub => stub.ApplyAppPathModifier ("/session/root.wxe")).Return ("/session/root.wxe").Repeat.Any();
      responseMock.Stub (stub => stub.ContentEncoding).Return (Encoding.Default).Repeat.Any();
      _httpContextMock.Stub (stub => stub.Response).Return (responseMock).Repeat.Any();

      IHttpRequest requestMock = _mockRepository.StrictMock<IHttpRequest>();
      requestMock.Stub (stub => stub.Url).Return (uri).Repeat.Any();
      _httpContextMock.Stub (stub => stub.Request).Return (requestMock).Repeat.Any();

      using (_mockRepository.Ordered())
      {
        using (_mockRepository.Unordered ())
        {
          _pageMock.Expect (mock => mock.GetPostBackCollection()).Return (_postBackCollection);
          _pageMock.Expect (mock => mock.SaveAllState());
          _pageMock.Expect (mock => mock.WxeHandler).Return (_wxeHandler);
        }

        //Redirect to subfunction
        responseMock.Expect (mock => mock.Redirect ("~/session/sub.wxe?WxeFunctionToken=" + _wxeContext.FunctionToken))
            .Do (invocation => Thread.CurrentThread.Abort());

        //Show sub function
        _subFunction.Expect (mock => mock.Execute (_wxeContext)).Do (invocation => Thread.CurrentThread.Abort());

        //Return from sub function
        _subFunction.Expect (mock => mock.Execute (_wxeContext)).Throw (new WxeExecuteNextStepException());

        //Return from sub function
        responseMock.Expect (mock => mock.Redirect ("/session/root.wxe?WxeFunctionToken=" + _wxeContext.FunctionToken))
            .Do (
            invocation =>
            {
              PrivateInvoke.SetNonPublicField (_functionState, "_postBackID", 100);
              _wxeContext.PostBackCollection = new NameValueCollection ();
              Thread.CurrentThread.Abort();
            });

        _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "ThePage", true)).Do (
            invocation =>
            {
              Assert.That (((IExecutionStateContext) _pageStep).ExecutionState, Is.SameAs (NullExecutionState.Null));
              Assert.That (_wxeContext.PostBackCollection[WxePageInfo<WxePage>.PostBackSequenceNumberID], Is.EqualTo ("100"));
              Assert.That (_wxeContext.PostBackCollection.AllKeys, List.Contains ("Key"));
              Assert.That (_pageStep.ReturningFunction, Is.SameAs (_subFunction));
              Assert.That (_pageStep.IsReturningPostBack, Is.True);
            });
      }

      _mockRepository.ReplayAll();

      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions();
      try
      {
        //Redirect to subfunction
        WxeRepostOptions repostOptions = WxeRepostOptions.Null;
        _pageStep.ExecuteFunction (new PreProcessingSubFunctionStateParameters (_pageMock, _subFunction, permaUrlOptions), repostOptions);
        Assert.Fail();
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort();
      }

      try
      {
        //Show sub function
        _pageStep.Execute();
        Assert.Fail();
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort();
      }

      try
      {
        //Return from sub function
        _pageStep.Execute();
        Assert.Fail();
      }
      catch (WxeExecuteNextStepException)
      {
      }

      try
      {
        //Return from sub function
        _pageStep.Execute ();
        Assert.Fail ();
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort ();
      }

      //Show current page
      _pageStep.Execute();

      _mockRepository.VerifyAll();
    }
  }
}