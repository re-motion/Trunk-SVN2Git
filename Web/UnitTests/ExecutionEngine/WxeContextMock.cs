using System;
using System.Collections.Specialized;
using System.Web;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{

/// <summary> Provides a <see cref="WxeContext"/> for simualating ASP.NET request life cycles. </summary>
public class WxeContextMock: WxeContext
{
  public WxeContextMock (HttpContext context)
    : base (context, new WxeFunctionState (new TestFunction (), false), null)
  {
  }

  public WxeContextMock (HttpContext context, NameValueCollection queryString)
    : base (context, new WxeFunctionState (new TestFunction (), false), queryString)
  {
  }
}

}