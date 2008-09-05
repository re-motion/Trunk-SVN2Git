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
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.ExecutionEngine;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Infrastructure;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Remotion.Web.Utilities;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.WxePageStepIntegrationTests
{
  [TestFixture]
  public class ExecuteFunctionNoRepost : TestBase
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

      _pageMock = _mockRepository.DynamicMock<IWxePage>();
      _postBackCollection = new NameValueCollection
                            {
                                { "Key", "Value" },
                                { ControlHelper.PostEventSourceID, "TheEventSource" },
                                { ControlHelper.PostEventArgumentID, "TheEventArgument" },
                                { "TheUnqiueID", "Value" }
                            };
      _wxeHandler = new WxeHandler ();
    }

    [Test]
    public void Test_PrepareOnly_SuppressSender_IPostBackEventHandler ()
    {
      WxeContextMock.SetCurrent (_wxeContext);

      _postBackCollection.Add ("TheUnqiueID", "Value");
      Control senderMock = _mockRepository.PartialMultiMock<Control> (typeof (IPostBackEventHandler));

      using (_mockRepository.Ordered())
      {
        using (_mockRepository.Unordered ())
        {
          _pageMock.Expect (mock => mock.GetPostBackCollection()).Return (_postBackCollection);
          senderMock.Expect (mock => mock.UniqueID).Return ("TheUniqueID");
          _pageMock.Expect (mock => mock.SaveAllState());
          _pageMock.Expect (mock => mock.WxeHandler).Return (_wxeHandler);
        }
        _pageStep.Expect (mock => mock.Execute (_wxeContext)).Do (
            invocation =>
            {
              Assert.That (_pageStep.PostBackCollection.AllKeys, List.Contains ("Key"));
              Assert.That (_pageStep.PostBackCollection.AllKeys, List.Not.Contains ("TheUniqueID"));
              Assert.That (_pageStep.SubFunction, Is.SameAs (_subFunction));
              Assert.That (_subFunction.ParentStep, Is.SameAs (_pageStep));
              Assert.That (PrivateInvoke.GetNonPublicField (_pageStep, "_wxeHandler"), Is.SameAs (_wxeHandler));
            });
      }

      _mockRepository.ReplayAll();

      _pageStep.ExecuteFunctionNoRepost (_pageMock, _subFunction, WxePermaUrlOptions.Null, new WxeRepostOptions (senderMock, false));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Test_PrepareOnly_SuppressSender_IPostBackDataHandler ()
    {
      WxeContextMock.SetCurrent (_wxeContext);

      _postBackCollection.Add ("TheUnqiueID", "Value");
      Control senderMock = _mockRepository.PartialMultiMock<Control> (typeof (IPostBackDataHandler));

      using (_mockRepository.Ordered())
      {
        using (_mockRepository.Unordered ())
        {
          _pageMock.Expect (mock => mock.GetPostBackCollection()).Return (_postBackCollection);
          senderMock.Expect (mock => mock.UniqueID).Return ("TheUniqueID");
          _pageMock.Expect (mock => mock.SaveAllState());
          _pageMock.Expect (mock => mock.WxeHandler).Return (_wxeHandler);
        }
        _pageStep.Expect (mock => mock.Execute (_wxeContext)).Do (
            invocation =>
            {
              Assert.That (_pageStep.PostBackCollection.AllKeys, List.Contains ("Key"));
              Assert.That (_pageStep.PostBackCollection.AllKeys, List.Not.Contains ("TheUniqueID"));
              Assert.That (_pageStep.SubFunction, Is.SameAs (_subFunction));
              Assert.That (_subFunction.ParentStep, Is.SameAs (_pageStep));
              Assert.That (PrivateInvoke.GetNonPublicField (_pageStep, "_wxeHandler"), Is.SameAs (_wxeHandler));
            });
      }

      _mockRepository.ReplayAll();

      _pageStep.ExecuteFunctionNoRepost (_pageMock, _subFunction, WxePermaUrlOptions.Null, new WxeRepostOptions (senderMock, false));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Test_PrepareOnly_UseEventTarget ()
    {
      WxeContextMock.SetCurrent (_wxeContext);
      Control senderStub = _mockRepository.Stub<Control> ();

      using (_mockRepository.Ordered ())
      {
        using (_mockRepository.Unordered ())
        {
          _pageMock.Expect (mock => mock.GetPostBackCollection()).Return (_postBackCollection);
          _pageMock.Expect (mock => mock.SaveAllState());
          _pageMock.Expect (mock => mock.WxeHandler).Return (_wxeHandler);
        }
        _pageStep.Expect (mock => mock.Execute (_wxeContext)).Do (
            invocation =>
            {
              Assert.That (_pageStep.PostBackCollection.AllKeys, List.Contains ("Key"));
              Assert.That (_pageStep.PostBackCollection.AllKeys, List.Not.Contains (ControlHelper.PostEventSourceID));
              Assert.That (_pageStep.PostBackCollection.AllKeys, List.Not.Contains (ControlHelper.PostEventArgumentID));
              Assert.That (_pageStep.SubFunction, Is.SameAs (_subFunction));
              Assert.That (_subFunction.ParentStep, Is.SameAs (_pageStep));
              Assert.That (PrivateInvoke.GetNonPublicField (_pageStep, "_wxeHandler"), Is.SameAs (_wxeHandler));
            });
      }

      _mockRepository.ReplayAll ();

      _pageStep.ExecuteFunctionNoRepost (_pageMock, _subFunction, WxePermaUrlOptions.Null, new WxeRepostOptions (senderStub, true));

      _mockRepository.VerifyAll ();
    }
  }
}