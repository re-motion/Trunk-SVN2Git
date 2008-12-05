using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.Test.ExecutionEngine
{
  public class ShowFourthUserControlFormFunction:WxeFunction
  {
    public ShowFourthUserControlFormFunction ()
      : base (new NoneTransactionMode ())
    {      
    }

    private WxeUserControlStep Step1 = new WxeUserControlStep ("~/ExecutionEngine/FourthControl.ascx");
  }
}
