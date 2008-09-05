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

namespace Remotion.Web.UnitTests.ExecutionEngine.WxePageStepIntegrationTests
{
  [TestFixture]
  public class Execute_Self : TestBase
  {
    private MockRepository _mockRepository;
    private WxePageStep _pageStep;
    private IHttpContext _httpContextMock;
    private WxeContext _wxeContext;
    private IWxePageExecutor _pageExecutorMock;
    private TestFunction _subFunction;
    private TestFunction _rootFunction;
    private WxeFunctionState _functionState;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();

      _rootFunction = new TestFunction();
      _subFunction = _mockRepository.PartialMock<TestFunction>();

      _httpContextMock = _mockRepository.DynamicMock<IHttpContext>();
      _pageExecutorMock = _mockRepository.StrictMock<IWxePageExecutor>();
      _functionState = new WxeFunctionState (_rootFunction, true);
      _wxeContext = new WxeContext (_httpContextMock, _functionState, new NameValueCollection());

      _pageStep = _mockRepository.PartialMock<WxePageStep> ("ThePage");
      _pageStep.SetPageExecutor (_pageExecutorMock);
    }

    [Test]
    public void Execute ()
    {
      _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "ThePage")).Do (
          invocation =>
          {
            Assert.That (_wxeContext.PostBackCollection, Is.Null);
            Assert.That (_wxeContext.IsReturningPostBack, Is.False);
          });

      _mockRepository.ReplayAll();

      _pageStep.Execute (_wxeContext);
      _mockRepository.VerifyAll();
    }

    [Test]
    public void HandlesWxeExecuteUserControlNextStepException ()
    {
      PrivateInvoke.SetNonPublicField (_pageStep, "_isReturningInnerFunction", false);
      PrivateInvoke.SetNonPublicField (_pageStep, "_innerFunction", _subFunction);
      PrivateInvoke.SetNonPublicField (_pageStep, "_userControlID", "TheUserControlID");
      PrivateInvoke.SetNonPublicField (_pageStep, "_userControlState", "TheUserControlState");

      using (_mockRepository.Ordered ())
      {
        _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "ThePage")).Throw (new WxeExecuteUserControlNextStepException());

        _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "ThePage")).Do (
            invocation => Assert.That (_pageStep.IsReturningInnerFunction, Is.True));
      }

      _mockRepository.ReplayAll();

      _pageStep.Execute (_wxeContext);

      _mockRepository.VerifyAll();
      Assert.That (_pageStep.IsReturningInnerFunction, Is.False);
      Assert.That (_pageStep.InnerFunction, Is.Null);
      Assert.That (_pageStep.UserControlID, Is.Null);
      Assert.That (_pageStep.UserControlState, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ApplicationException))]
    public void CleanUpAndRethrowException ()
    {
      PrivateInvoke.SetNonPublicField (_pageStep, "_isReturningInnerFunction", false);
      PrivateInvoke.SetNonPublicField (_pageStep, "_innerFunction", _subFunction);
      PrivateInvoke.SetNonPublicField (_pageStep, "_userControlID", "TheUserControlID");
      PrivateInvoke.SetNonPublicField (_pageStep, "_userControlState", "TheUserControlState");
     
      using (_mockRepository.Ordered ())
      {
        _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "ThePage")).Throw (new WxeExecuteUserControlNextStepException());

        _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "ThePage")).Throw (new ApplicationException());
      }
      _mockRepository.ReplayAll();

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