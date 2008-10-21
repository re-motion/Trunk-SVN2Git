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
using System.Web.SessionState;
using System.Web.UI.WebControls;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
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

      IHttpSessionState sessionStub = _mockRepository.DynamicMock<IHttpSessionState> ();
      sessionStub.Stub (stub => stub[Arg<string>.Is.NotNull]).PropertyBehavior ();

      _functionStateManager = new WxeFunctionStateManager (sessionStub);
      _wxeContext = new WxeContext (_httpContextMock, _functionStateManager, _functionState, new NameValueCollection ());
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
      var userControlExecutorMock = _mockRepository.StrictMock<IUserControlExecutor> ();
      _pageStep.SetUserControlExecutor (userControlExecutorMock);

      using (_mockRepository.Ordered ())
      {
        userControlExecutorMock.Expect (mock => mock.Execute (_wxeContext));
        _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "ThePage")).Throw (new WxeExecuteUserControlNextStepException (userControlExecutorMock));

        userControlExecutorMock.Expect (mock => mock.BeginReturn(_wxeContext));
        _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "ThePage"));
        userControlExecutorMock.Expect (mock => mock.EndReturn (_wxeContext));
      }

      _mockRepository.ReplayAll();

      _pageStep.Execute (_wxeContext);

      _mockRepository.VerifyAll();
    }

    [Test]
    [ExpectedException (typeof (ApplicationException))]
    public void CleanUpAndRethrowException ()
    {
      var userControlExecutorMock = _mockRepository.StrictMock<IUserControlExecutor> ();
      _pageStep.SetUserControlExecutor (userControlExecutorMock);
     
      using (_mockRepository.Ordered ())
      {
        userControlExecutorMock.Expect (mock => mock.Execute (_wxeContext));
        _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "ThePage")).Throw (new WxeExecuteUserControlNextStepException (userControlExecutorMock));

        userControlExecutorMock.Expect (mock => mock.BeginReturn (_wxeContext));
        _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "ThePage")).Throw (new ApplicationException ());
      }
      _mockRepository.ReplayAll();

      try
      {
        _pageStep.Execute (_wxeContext);
      }
      finally
      {
        _pageExecutorMock.VerifyAllExpectations();
      }
    }
  }
}