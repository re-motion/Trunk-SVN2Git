using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.Test.ExecutionEngine
{
  public class ShowFirstUserControlFormFunction:WxeFunction
  {
    public ShowFirstUserControlFormFunction ()
      : base (new NoneTransactionMode ())
    {      
    }

    private WxeUserControlStep Step1 = new WxeUserControlStep ("~/ExecutionEngine/FirstControl.ascx");
  }
}
