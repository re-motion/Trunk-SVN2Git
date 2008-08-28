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
using System.Threading;
using System.Web;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.ExecutionEngine;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Infrastructure;
using Rhino.Mocks;
using TestFunction=Remotion.Web.UnitTests.ExecutionEngine.TestFunctions.TestFunction;

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
    private IWxePage _page;
    private TestFunction _subFunction;
    private TestFunction _rootFunction;
    private NameValueCollection _postBackCollection;
    private WxeHandler _wxeHandler;
    private WxeFunctionState _functionState;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository ();
      
      _rootFunction = new TestFunction ();
      _subFunction = _mockRepository.PartialMock<TestFunction>();
     
      _httpContextMock = _mockRepository.DynamicMock<IHttpContext>();
      _pageExecutorMock = _mockRepository.StrictMock<IWxePageExecutor>();
      _functionState = new WxeFunctionState (_rootFunction, true);
      _wxeContext = new WxeContext (_httpContextMock, _functionState, new NameValueCollection());
      
      _pageStep = _mockRepository.PartialMock<WxePageStep> ("ThePage");
      _pageStep.SetPageExecutor (_pageExecutorMock);

      _page = _mockRepository.DynamicMock<IWxePage>();
      _postBackCollection = new NameValueCollection { { "Key", "Value" } };
      _wxeHandler = new WxeHandler();
      
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

      _mockRepository.ReplayAll ();

      _pageStep.Execute (_wxeContext);
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Execute_Self_HandlesWxeExecuteUserControlNextStepException ()
    {
      PrivateInvoke.SetNonPublicField (_pageStep, "_isReturningInnerFunction", false);
      PrivateInvoke.SetNonPublicField (_pageStep, "_innerFunction", _subFunction);
      PrivateInvoke.SetNonPublicField (_pageStep, "_userControlID", "TheUserControlID");
      PrivateInvoke.SetNonPublicField (_pageStep, "_userControlState", "TheUserControlState");

      _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "ThePage")).Throw (new WxeExecuteUserControlNextStepException());

      _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "ThePage")).Do (
          invocation => Assert.That (_pageStep.IsReturningInnerFunction, Is.True));

      _mockRepository.ReplayAll ();

      _pageStep.Execute (_wxeContext);
      _mockRepository.VerifyAll ();
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
      PrivateInvoke.SetNonPublicField (_pageStep, "_innerFunction", _subFunction);
      PrivateInvoke.SetNonPublicField (_pageStep, "_userControlID", "TheUserControlID");
      PrivateInvoke.SetNonPublicField (_pageStep, "_userControlState", "TheUserControlState");

      _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "ThePage")).Throw (new WxeExecuteUserControlNextStepException());

      _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "ThePage")).Throw (new ApplicationException());

      _mockRepository.ReplayAll ();

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

    [Test]
    public void ExecuteFunction_PrepareOnly ()
    {
      WxeContextMock.SetCurrent (_wxeContext);

      using (_mockRepository.Ordered())
      {
        _page.Expect (mock => mock.GetPostBackCollection()).Return (_postBackCollection);
        _page.Expect (mock => mock.SaveAllState());
        _page.Expect (mock => mock.WxeHandler).Return (_wxeHandler);
        _pageStep.Expect (mock => mock.Execute (_wxeContext)).Do (
            invocation =>
            {
              Assert.That (_pageStep.PostBackCollection, Is.EquivalentTo (_postBackCollection));
              Assert.That (_pageStep.SubFunction, Is.SameAs (_subFunction));
              Assert.That (_subFunction.ParentStep, Is.SameAs (_pageStep));
              Assert.That (PrivateInvoke.GetNonPublicField (_pageStep, "_wxeHandler"), Is.SameAs (_wxeHandler));
            });
      }

      _mockRepository.ReplayAll();

      _pageStep.ExecuteFunction (_page, _subFunction, WxePermaUrlOptions.Null);

      _mockRepository.VerifyAll ();
    }


    [Test]
    public void ExecuteFunction_SubFunction ()
    {
      WxeContextMock.SetCurrent (_wxeContext);

      using (_mockRepository.Ordered ())
      {
        _page.Expect (mock => mock.GetPostBackCollection()).Return (_postBackCollection);
        _page.Expect (mock => mock.SaveAllState());
        _page.Expect (mock => mock.WxeHandler).Return (_wxeHandler);

        _subFunction.Expect (mock => mock.Execute (_wxeContext)).Do (
            invocation =>
            {
              PrivateInvoke.SetNonPublicField (_functionState, "_postBackID", 100);
              _wxeContext.PostBackCollection = new NameValueCollection();
            });

        _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "ThePage")).Do (
            invocation =>
            {
              Assert.That (_wxeContext.ReturningFunction, Is.SameAs (_subFunction));
              Assert.That (_pageStep.SubFunction, Is.Null);
              Assert.That (_wxeContext.IsPostBack, Is.True);
              Assert.That (_wxeContext.PostBackCollection[WxePageInfo<WxePage>.PostBackSequenceNumberID], Is.EqualTo ("100"));
              Assert.That (_wxeContext.PostBackCollection.AllKeys, List.Contains ("Key"));
              Assert.That (PrivateInvoke.GetNonPublicField (_pageStep, "_postBackCollection"), Is.Null);
              Assert.That (_wxeContext.IsReturningPostBack, Is.True);
            });
      }

      _mockRepository.ReplayAll ();

      _pageStep.ExecuteFunction (_page, _subFunction, WxePermaUrlOptions.Null);

      _mockRepository.VerifyAll ();    
    }

    [Test]
    public void ExecuteFunction_SubFunctionCompleted_ReEntrancy ()
    {
      WxeContextMock.SetCurrent (_wxeContext);

      using (_mockRepository.Ordered ())
      {
        _page.Expect (mock => mock.GetPostBackCollection ()).Return (_postBackCollection);
        _page.Expect (mock => mock.SaveAllState ());
        _page.Expect (mock => mock.WxeHandler).Return (_wxeHandler);

        _subFunction.Expect (mock => mock.Execute (_wxeContext)).Do (invocation => Thread.CurrentThread.Abort());

        _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "ThePage"));
      }

      _mockRepository.ReplayAll ();

      try
      {
        _pageStep.ExecuteFunction (_page, _subFunction, WxePermaUrlOptions.Null);
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort();
      }
      _pageStep.Execute();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ExecuteFunction_SubFunction_ReEntrancy ()
    {
      WxeContextMock.SetCurrent (_wxeContext);

      using (_mockRepository.Ordered ())
      {
        _page.Expect (mock => mock.GetPostBackCollection ()).Return (_postBackCollection);
        _page.Expect (mock => mock.SaveAllState ());
        _page.Expect (mock => mock.WxeHandler).Return (_wxeHandler);

        _subFunction.Expect (mock => mock.Execute (_wxeContext)).Do (invocation => Thread.CurrentThread.Abort ());

        _subFunction.Expect (mock => mock.Execute (_wxeContext));
        
        _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "ThePage"));
      }

      _mockRepository.ReplayAll ();

      try
      {
        _pageStep.ExecuteFunction (_page, _subFunction, WxePermaUrlOptions.Null);
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort ();
      }
      _pageStep.Execute ();

      _mockRepository.VerifyAll ();
    }


    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot execute function while another function executes.")]
    public void ExecuteFunction_IsAlreadyExecutingSubFunction ()
    {
      WxeContextMock.SetCurrent (_wxeContext);
      
      using (_mockRepository.Ordered ())
      {
        _page.Expect (mock => mock.GetPostBackCollection()).Return (_postBackCollection);
        _page.Expect (mock => mock.SaveAllState());
        _page.Expect (mock => mock.WxeHandler).Return (_wxeHandler);

        _subFunction.Expect (mock => mock.Execute (_wxeContext)).Throw (new ApplicationException());
        
        _page.Expect (mock => mock.GetPostBackCollection ()).Return (_postBackCollection);
        _page.Expect (mock => mock.SaveAllState ());
        _page.Expect (mock => mock.WxeHandler).Return (_wxeHandler);

      }
      _mockRepository.ReplayAll ();

      try
      {
        _pageStep.ExecuteFunction (_page, _subFunction, WxePermaUrlOptions.Null);
      }
      catch (ApplicationException)
      {
      }
      _pageStep.ExecuteFunction (_page, _subFunction, WxePermaUrlOptions.Null);
     }
  }
}