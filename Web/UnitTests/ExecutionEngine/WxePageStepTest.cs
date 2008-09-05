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
using System.Web.UI;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Context;
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.ExecutionEngine;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.WxePageStepExecutionStates;
using Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.Execute;
using Remotion.Web.Infrastructure;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

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

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();

      _rootFunction = new TestFunction();
      _subFunction = _mockRepository.PartialMock<TestFunction>();

      _httpContextMock = _mockRepository.DynamicMock<IHttpContext>();
      _functionState = new WxeFunctionState (_rootFunction, true);
      _wxeContext = new WxeContext (_httpContextMock, _functionState, new NameValueCollection());

      _pageStep = _mockRepository.PartialMock<WxePageStep> ("ThePage");

      _pageMock = _mockRepository.DynamicMock<IWxePage>();
      _wxeHandler = new WxeHandler();
    }

    [TearDown]
    public virtual void TearDown ()
    {
      WxeContext.SetCurrent (null);
      SafeContext.Instance.SetData (typeof (WxeFunctionStateManager).AssemblyQualifiedName, null);
    }

    [Test]
    public void ExecuteFunction ()
    {
      WxeContextMock.SetCurrent (_wxeContext);

      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions();
      WxeRepostOptions repostOptions = new WxeRepostOptions (MockRepository.GenerateStub<Control>(), true);

      using (_mockRepository.Ordered())
      {
        using (_mockRepository.Unordered())
        {
          _pageMock.Expect (mock => mock.SaveAllState());
          _pageMock.Expect (mock => mock.WxeHandler).Return (_wxeHandler);
        }
        _pageStep.Expect (mock => mock.Execute (_wxeContext)).Do (
            invocation =>
            {
              var executionState = (PreProcessingSubFunctionState) ((IExecutionStateContext) _pageStep).ExecutionState;
              Assert.That (executionState.Parameters.SubFunction, Is.SameAs (_subFunction));
              Assert.That (executionState.Parameters.PermaUrlOptions, Is.SameAs (permaUrlOptions));
              Assert.That (executionState.Parameters.RepostOptions, Is.SameAs (repostOptions));
              Assert.That (PrivateInvoke.GetNonPublicField (_pageStep, "_wxeHandler"), Is.SameAs (_wxeHandler));
            });
      }

      _mockRepository.ReplayAll();

      _pageStep.ExecuteFunction (_pageMock, _subFunction, permaUrlOptions, repostOptions);

      _mockRepository.VerifyAll();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot execute function while another function executes.")]
    public void ExecuteFunction_IsAlreadyExecutingSubFunction ()
    {
      WxeContextMock.SetCurrent (_wxeContext);

      _pageMock.Stub (stub => stub.GetPostBackCollection ()).Return (new NameValueCollection ()).Repeat.Any ();
      _pageMock.Stub (stub => stub.SaveAllState ()).Repeat.Any ();
      _pageMock.Stub (stub => stub.WxeHandler).Return (_wxeHandler).Repeat.Any ();

      _subFunction.Expect (mock => mock.Execute (_wxeContext)).Throw (new ApplicationException ());
      
      _mockRepository.ReplayAll ();

      try
      {
        _pageStep.ExecuteFunction (_pageMock, _subFunction, WxePermaUrlOptions.Null, WxeRepostOptions.Null);
      }
      catch (ApplicationException)
      {
      }
      _pageStep.ExecuteFunction (_pageMock, _subFunction, WxePermaUrlOptions.Null, WxeRepostOptions.Null);
    }
  }
}