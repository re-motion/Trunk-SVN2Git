using System;
using System.Web;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UnitTests.AspNetFramework;

namespace Remotion.Web.UnitTests.ExecutionEngine
{

public class WxeTest
{
  private HttpContext _currentHttpContext;
  private WxeContextMock _currentWxeContext;

  public HttpContext CurrentHttpContext
  {
    get { return _currentHttpContext; }
  }

  public WxeContextMock CurrentWxeContext
  {
    get { return _currentWxeContext; }
  }

  [SetUp]
  public virtual void SetUp()
  {
    _currentHttpContext = HttpContextHelper.CreateHttpContext ("GET", "default.html", null);
    HttpContextHelper.SetCurrent (_currentHttpContext);

    _currentWxeContext = new WxeContextMock (_currentHttpContext);
    PrivateInvoke.InvokeNonPublicStaticMethod (typeof (WxeContext), "SetCurrent", _currentWxeContext);
  }

  [TearDown]
  public virtual void TearDown()
  {
  }
}

}
