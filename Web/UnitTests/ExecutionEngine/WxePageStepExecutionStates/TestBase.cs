using System;
using System.Collections.Specialized;
using NUnit.Framework;
using Remotion.Context;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Web.ExecutionEngine.WxePageStepExecutionStates;
using Remotion.Web.Infrastructure;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.WxePageStepExecutionStates
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

    [SetUp]
    public virtual void SetUp ()
    {
      _mockRepository = new MockRepository();
      _executionStateContextMock = MockRepository.StrictMock<IExecutionStateContext>();

      _rootFunction = new TestFunction();
      _subFunction = CreateSubFunction();

      _httpContextMock = MockRepository.DynamicMock<IHttpContext>();
      _functionState = new WxeFunctionState (_rootFunction, true);
      _wxeContext = new WxeContext (_httpContextMock, _functionState, new NameValueCollection());

      _responseMock = MockRepository.StrictMock<IHttpResponse>();
      _httpContextMock.Stub (stub => stub.Response).Return (_responseMock).Repeat.Any();

      _requestMock = MockRepository.StrictMock<IHttpRequest>();
      _httpContextMock.Stub (stub => stub.Request).Return (_requestMock).Repeat.Any();

      _postBackCollection = new NameValueCollection();
    }

    protected virtual OtherTestFunction CreateSubFunction ()
    {
      return new OtherTestFunction ("Value");
    }

    [TearDown]
    public virtual void TearDown ()
    {
      WxeContext.SetCurrent (null);
      UrlMappingConfiguration.SetCurrent (null);
      SafeContext.Instance.SetData (typeof (WxeFunctionStateManager).AssemblyQualifiedName, null);
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
  }
}