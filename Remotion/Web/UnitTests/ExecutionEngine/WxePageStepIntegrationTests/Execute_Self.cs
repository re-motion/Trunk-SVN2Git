// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
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
      _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "ThePage", false)).WhenCalled (
          invocation =>
          {
            Assert.That (_pageStep.PostBackCollection, Is.Null);
            Assert.That (_pageStep.IsReturningPostBack, Is.False);
          });

      _mockRepository.ReplayAll();

      _pageStep.Execute (_wxeContext);
      _mockRepository.VerifyAll();
      Assert.That (_pageStep.IsPostBack, Is.False);
    }

    [Test]
    public void Execute_WithPostBack ()
    {
      _pageExecutorMock.Stub (stub => stub.ExecutePage (_wxeContext, "ThePage", false));
      _mockRepository.ReplayAll ();
      _pageStep.Execute (_wxeContext);
      _mockRepository.BackToRecordAll();

      _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "ThePage", true)).WhenCalled (
          invocation =>
          {
            Assert.That (_pageStep.PostBackCollection, Is.Null);
            Assert.That (_pageStep.IsReturningPostBack, Is.False);
          });

      _mockRepository.ReplayAll ();

      _pageStep.Execute (_wxeContext);

      _mockRepository.VerifyAll ();
      Assert.That (_pageStep.IsPostBack, Is.True);
    }
  }
}
