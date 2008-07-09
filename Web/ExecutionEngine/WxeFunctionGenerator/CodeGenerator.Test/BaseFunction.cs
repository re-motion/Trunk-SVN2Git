using System;
using Remotion.Web.ExecutionEngine;

namespace Test
{
  public class BaseFunction: WxeFunction
  {
    public BaseFunction (params object[] arguments)
      : base (arguments)
    {
    }
  }
}
