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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Infrastructure;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  [TestFixture]
  public class WxePageStepIntegrationTest
  {
    private MockRepository _mockRepository;
    private WxePageStep _pageStep;
    private IHttpContext _httpContextMock;
    private WxeContext _wxeContext;
    private IWxePageExecutor _pageExecutorMock;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _httpContextMock = _mockRepository.DynamicMock<IHttpContext>();
      _pageExecutorMock = _mockRepository.StrictMock<IWxePageExecutor>();
      _wxeContext = new WxeContext (_httpContextMock, new WxeFunctionState (new TestFunction(), true), new NameValueCollection());
      _pageStep = new WxePageStep ("ThePage");
      _pageStep.SetPageExecutor (_pageExecutorMock);
    }

    [Test]
    public void Execute_Self ()
    {
      _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "ThePage")).Do (
          invocation =>
          {
            Assert.That (_wxeContext.PostBackCollection, Is.Null);
            Assert.That (_wxeContext.IsReturningPostBack, Is.False);
          });

      _pageExecutorMock.Replay();

      _pageStep.Execute (_wxeContext);
      _pageExecutorMock.VerifyAllExpectations();
    }

    [Test]
    public void Execute_Self_HandlesWxeExecuteUserControlNextStepException ()
    {
      PrivateInvoke.SetNonPublicField (_pageStep, "_isReturningInnerFunction", false);
      PrivateInvoke.SetNonPublicField (_pageStep, "_innerFunction", new TestFunction());
      PrivateInvoke.SetNonPublicField (_pageStep, "_userControlID", "TheUserControlID");
      PrivateInvoke.SetNonPublicField (_pageStep, "_userControlState", "TheUserControlState");

      _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "ThePage")).Throw (new WxeExecuteUserControlNextStepException());

      _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "ThePage")).Do (
          invocation => Assert.That (_pageStep.IsReturningInnerFunction, Is.True));

      _pageExecutorMock.Replay();

      _pageStep.Execute (_wxeContext);
      _pageExecutorMock.VerifyAllExpectations();
      Assert.That (_pageStep.IsReturningInnerFunction, Is.False);
      Assert.That (_pageStep.InnerFunction, Is.Null);
      Assert.That (_pageStep.UserControlID, Is.Null);
      Assert.That (_pageStep.UserControlState, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ApplicationException))]
    public void Execute_Self_CleanUpAndRethrowException ()
    {
      PrivateInvoke.SetNonPublicField (_pageStep, "_isReturningInnerFunction", false);
      PrivateInvoke.SetNonPublicField (_pageStep, "_innerFunction", new TestFunction());
      PrivateInvoke.SetNonPublicField (_pageStep, "_userControlID", "TheUserControlID");
      PrivateInvoke.SetNonPublicField (_pageStep, "_userControlState", "TheUserControlState");

      _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "ThePage")).Throw (new WxeExecuteUserControlNextStepException());

      _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "ThePage")).Throw (new ApplicationException());

      _pageExecutorMock.Replay();

      try
      {
        _pageStep.Execute (_wxeContext);
      }
      finally
      {
        _pageExecutorMock.VerifyAllExpectations();
        Assert.That (_pageStep.IsReturningInnerFunction, Is.False);
        Assert.That (_pageStep.InnerFunction, Is.Null);
        Assert.That (_pageStep.UserControlID, Is.Null);
        Assert.That (_pageStep.UserControlState, Is.Null);
      }
    }
  }
}