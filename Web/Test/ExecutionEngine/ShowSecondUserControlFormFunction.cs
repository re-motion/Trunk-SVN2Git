using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.Test.ExecutionEngine
{
  public class ShowSecondUserControlFormFunction:WxeFunction
  {
    private WxeUserControlStep Step1 = new WxeUserControlStep ("~/ExecutionEngine/SecondControl.ascx");
  }
}
