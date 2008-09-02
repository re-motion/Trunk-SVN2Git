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
    protected MockRepository _mockRepository;
    protected IWxePageStepExecutionStateContext _executionStateContextMock;
    protected TestFunction _rootFunction;
    protected OtherTestFunction _subFunction;
    protected IHttpContext _httpContextMock;
    protected WxeFunctionState _functionState;
    protected WxeContext _wxeContext;

    [SetUp]
    public virtual void SetUp ()
    {
      _mockRepository = new MockRepository ();
      _executionStateContextMock = MockRepository.StrictMock<IWxePageStepExecutionStateContext> ();

      _rootFunction = new TestFunction ();
      _subFunction = CreateSubFunction();

      _httpContextMock = MockRepository.DynamicMock<IHttpContext> ();
      _functionState = new WxeFunctionState (RootFunction, true);
      _wxeContext = new WxeContext (HttpContextMock, _functionState, new NameValueCollection ());
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

    protected IWxePageStepExecutionStateContext ExecutionStateContextMock
    {
      get { return _executionStateContextMock; }
    }

    protected MockRepository MockRepository
    {
      get { return _mockRepository; }
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
  }
}