// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Context;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates;
using Remotion.Web.Infrastructure;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure.WxePageStepExecutionStates
{
  public class TestBase
  {
    private MockRepository _mockRepository;
    private IExecutionStateContext _executionStateContextMock;
    private TestFunction _rootFunction;
    private OtherTestFunction _subFunction;
    private IHttpContext _httpContextMock;
    private WxeFunctionState _functionState;
    private WxeContext _wxeContext;
    private IHttpResponse _responseMock;
    private IHttpRequest _requestMock;
    private NameValueCollection _postBackCollection;
    private WxeFunctionStateManager _functionStateManager;

    [SetUp]
    public virtual void SetUp ()
    {
      _mockRepository = new MockRepository();
      _executionStateContextMock = MockRepository.StrictMock<IExecutionStateContext>();

      _rootFunction = new TestFunction ("Value");
      _subFunction = CreateSubFunction();

      _httpContextMock = MockRepository.DynamicMock<IHttpContext>();
      _functionState = new WxeFunctionState (_rootFunction, true);

      _responseMock = MockRepository.StrictMock<IHttpResponse>();
      _httpContextMock.Stub (stub => stub.Response).Return (_responseMock).Repeat.Any();

      _requestMock = MockRepository.StrictMock<IHttpRequest>();
      _httpContextMock.Stub (stub => stub.Request).Return (_requestMock).Repeat.Any();

      _postBackCollection = new NameValueCollection();

      IHttpSessionState sessionStub = _mockRepository.DynamicMock<IHttpSessionState>();
      sessionStub.Stub (stub => stub[Arg<string>.Is.NotNull]).PropertyBehavior();

      _functionStateManager = new WxeFunctionStateManager (sessionStub);
      _wxeContext = new WxeContext (_httpContextMock, _functionStateManager, _functionState, new NameValueCollection ());
    }

    protected virtual OtherTestFunction CreateSubFunction ()
    {
      return new OtherTestFunction ("OtherValue");
    }

    [TearDown]
    public virtual void TearDown ()
    {
      WxeContext.SetCurrent (null);
      UrlMappingConfiguration.SetCurrent (null);
    }

    protected MockRepository MockRepository
    {
      get { return _mockRepository; }
    }

    protected WxeFunctionState FunctionState
    {
      get { return _functionState; }
    }

    protected IExecutionStateContext ExecutionStateContextMock
    {
      get { return _executionStateContextMock; }
    }

    protected TestFunction RootFunction
    {
      get { return _rootFunction; }
    }

    protected OtherTestFunction SubFunction
    {
      get { return _subFunction; }
    }

    protected WxeContext WxeContext
    {
      get { return _wxeContext; }
    }

    protected IHttpContext HttpContextMock
    {
      get { return _httpContextMock; }
    }

    protected IHttpRequest RequestMock
    {
      get { return _requestMock; }
    }

    protected IHttpResponse ResponseMock
    {
      get { return _responseMock; }
    }

    public NameValueCollection PostBackCollection
    {
      get { return _postBackCollection; }
    }

    protected T CheckExecutionState<T> (T executionState)
        where T: IExecutionState
    {
      Assert.That (executionState, Is.Not.Null);
      Assert.That (executionState.ExecutionStateContext, Is.SameAs (ExecutionStateContextMock));
      Assert.That (executionState.Parameters.SubFunction, Is.SameAs (SubFunction));

      return executionState;
    }
  }
}