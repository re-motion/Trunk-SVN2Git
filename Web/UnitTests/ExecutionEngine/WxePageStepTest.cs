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
using System.Web.UI;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.ExecutionEngine;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Web.ExecutionEngine.WxePageStepExecutionStates;
using Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.Execute;
using Remotion.Web.Infrastructure;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Rhino.Mocks;
using PreProcessingSubFunctionState_WithRedirect =
    Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.ExecuteExternalByRedirect.PreProcessingSubFunctionState;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  [TestFixture]
  public class WxePageStepTest
  {
    private MockRepository _mockRepository;
    private WxePageStep _pageStep;
    private IHttpContext _httpContextMock;
    private WxeContext _wxeContext;
    private IWxePage _pageMock;
    private TestFunction _subFunction;
    private TestFunction _rootFunction;
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
      _functionState = new WxeFunctionState (_rootFunction, true);

      _pageStep = _mockRepository.PartialMock<WxePageStep> ("ThePage");

      _pageMock = _mockRepository.DynamicMock<IWxePage>();
      _wxeHandler = new WxeHandler();

      IHttpSessionState sessionStub = _mockRepository.DynamicMock<IHttpSessionState>();
      sessionStub.Stub (stub => stub[Arg<string>.Is.NotNull]).PropertyBehavior();

      _functionStateManager = new WxeFunctionStateManager (sessionStub);
      _wxeContext = new WxeContext (_httpContextMock, _functionStateManager, _functionState, new NameValueCollection());
    }

    [TearDown]
    public virtual void TearDown ()
    {
      WxeContext.SetCurrent (null);
      UrlMappingConfiguration.SetCurrent (null);
    }

    [Test]
    public void Initialize_WithPath ()
    {
      WxePageStep step = new WxePageStep ("page.aspx");
      Assert.That (step.Page, Is.EqualTo("page.aspx"));
    }

    [Test]
    public void Initialize_WithVariableReference ()
    {
      WxeFunction function = new TestFunction();
      WxePageStep step = new WxePageStep (new WxeVariableReference("ThePage"));
      function.Add (step);
      function.Variables["ThePage"] = "page.aspx";

      Assert.That (step.Page, Is.EqualTo ("page.aspx"));
    }

    [Test]
    public void ExecuteFunction ()
    {
      WxeContextMock.SetCurrent (_wxeContext);

      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions();
      WxeRepostOptions repostOptions = new WxeRepostOptions (MockRepository.GenerateStub<Control>(), true);

      using (_mockRepository.Ordered())
      {
        _pageMock.Expect (mock => mock.WxeHandler).Return (_wxeHandler);
        _pageStep.Expect (mock => mock.Execute (_wxeContext)).Do (
            invocation =>
            {
              var executionState = (PreProcessingSubFunctionState) ((IExecutionStateContext) _pageStep).ExecutionState;
              Assert.That (executionState.Parameters.SubFunction, Is.SameAs (_subFunction));
              Assert.That (executionState.Parameters.PermaUrlOptions, Is.SameAs (permaUrlOptions));
              Assert.That (executionState.RepostOptions, Is.SameAs (repostOptions));
              Assert.That (PrivateInvoke.GetNonPublicField (_pageStep, "_wxeHandler"), Is.SameAs (_wxeHandler));
            });
      }

      _mockRepository.ReplayAll();

      _pageStep.ExecuteFunction (new PreProcessingSubFunctionStateParameters (_pageMock, _subFunction, permaUrlOptions), repostOptions);

      _mockRepository.VerifyAll();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot execute function while another function executes.")]
    public void ExecuteFunction_IsAlreadyExecutingSubFunction ()
    {
      WxeContextMock.SetCurrent (_wxeContext);

      _pageMock.Stub (stub => stub.GetPostBackCollection()).Return (new NameValueCollection()).Repeat.Any();
      _pageMock.Stub (stub => stub.SaveAllState()).Repeat.Any();
      _pageMock.Stub (stub => stub.WxeHandler).Return (_wxeHandler).Repeat.Any();

      _subFunction.Expect (mock => mock.Execute (_wxeContext)).Throw (new ApplicationException());

      _mockRepository.ReplayAll();

      try
      {
        _pageStep.ExecuteFunction (
            new PreProcessingSubFunctionStateParameters (_pageMock, _subFunction, WxePermaUrlOptions.Null), WxeRepostOptions.Null);
      }
      catch (ApplicationException)
      {
      }
      _pageStep.ExecuteFunction (
          new PreProcessingSubFunctionStateParameters (_pageMock, _subFunction, WxePermaUrlOptions.Null), WxeRepostOptions.Null);
    }

    [Test]
    public void ExecuteFunctionExternalByRedirect ()
    {
      WxeContextMock.SetCurrent (_wxeContext);

      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions();
      WxeReturnOptions returnOptions = new WxeReturnOptions();

      using (_mockRepository.Ordered())
      {
        _pageMock.Expect (mock => mock.WxeHandler).Return (_wxeHandler);
        _pageStep.Expect (mock => mock.Execute (_wxeContext)).Do (
            invocation =>
            {
              var executionState = (PreProcessingSubFunctionState_WithRedirect) ((IExecutionStateContext) _pageStep).ExecutionState;
              Assert.That (executionState.Parameters.SubFunction, Is.SameAs (_subFunction));
              Assert.That (executionState.Parameters.PermaUrlOptions, Is.SameAs (permaUrlOptions));
              Assert.That (executionState.ReturnOptions, Is.SameAs (returnOptions));
              Assert.That (PrivateInvoke.GetNonPublicField (_pageStep, "_wxeHandler"), Is.SameAs (_wxeHandler));
            });
      }

      _mockRepository.ReplayAll();

      _pageStep.ExecuteFunctionExternalByRedirect (
          new PreProcessingSubFunctionStateParameters (_pageMock, _subFunction, permaUrlOptions), returnOptions);

      _mockRepository.VerifyAll();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot execute function while another function executes.")]
    public void ExecuteFunctionExternalByRedirect_IsAlreadyExecutingSubFunction ()
    {
      WxeContextMock.SetCurrent (_wxeContext);

      _pageMock.Stub (stub => stub.GetPostBackCollection()).Return (new NameValueCollection()).Repeat.Any();
      _pageMock.Stub (stub => stub.SaveAllState()).Repeat.Any();
      _pageMock.Stub (stub => stub.WxeHandler).Return (_wxeHandler).Repeat.Any();

      _pageStep.Expect (mock => mock.Execute (_wxeContext)).Throw (new ApplicationException());

      _mockRepository.ReplayAll();

      try
      {
        _pageStep.ExecuteFunctionExternalByRedirect (
            new PreProcessingSubFunctionStateParameters (_pageMock, _subFunction, WxePermaUrlOptions.Null), WxeReturnOptions.Null);
      }
      catch (ApplicationException)
      {
      }
      _pageStep.ExecuteFunctionExternalByRedirect (
          new PreProcessingSubFunctionStateParameters (_pageMock, _subFunction, WxePermaUrlOptions.Null), WxeReturnOptions.Null);
    }

    [Test]
    public void IExecutionStateContext_GetAndSetExecutionState ()
    {
      IExecutionStateContext executionStateContext = _pageStep;
      IExecutionState newExecutionState = MockRepository.GenerateStub<IExecutionState> ();
      
      Assert.That (executionStateContext.ExecutionState, Is.SameAs (NullExecutionState.Null));
      
      executionStateContext.SetExecutionState (newExecutionState);
      
      Assert.That (executionStateContext.ExecutionState, Is.SameAs (newExecutionState));
    }

    [Test]
    public void IExecutionStateContext_GetCurrentStep ()
    {
      IExecutionStateContext executionStateContext = _pageStep;
      Assert.That (executionStateContext.CurrentStep, Is.SameAs (_pageStep));
    }

    [Test]
    public void IExecutionStateContext_GetCurrentFunction ()
    {
      _pageStep.SetParentStep (_rootFunction);
      IExecutionStateContext executionStateContext = _pageStep;

      Assert.That (executionStateContext.CurrentFunction, Is.SameAs (_rootFunction));
    }
  }
}