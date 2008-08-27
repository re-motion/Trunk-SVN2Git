﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.Test.ExecutionEngine
{
  public class ShowUserControlFormFunction:WxeFunction
  {
    public ShowUserControlFormFunction ()
    {
      ReturnUrl = "Start.aspx";
    }

    private WxePageStep Step1 = new WxePageStep ("~/ExecutionEngine/UserControlForm.aspx");
  }
}
