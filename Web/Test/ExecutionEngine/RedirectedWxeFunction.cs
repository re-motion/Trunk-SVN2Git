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
using System.Web;
using System.Web.SessionState;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.Utilities;

namespace Remotion.Web.Test.ExecutionEngine
{
  public class RedirectedSubWxeFunction: WxeFunction
  {
    public RedirectedSubWxeFunction ()
      : base (new NoneTransactionMode ())
    {
    }

    void Step1 (WxeContext context)
    {
      context.HttpContext.Response.Redirect ("~/Start.aspx?Redirected");
    }
  }

  public class RedirectedWxeFunction: WxeFunction
  {
    public RedirectedWxeFunction ()
      : base (new NoneTransactionMode ())
    {
    }

    public RedirectedWxeFunction (params object[] args)
        : base (new NoneTransactionMode(), args)
    {
    }

    // steps

    WxeStep Step1 = new RedirectedSubWxeFunction();

    WxeStep Step2 = new WxePageStep ("~/ExecutionEngine/SessionForm.aspx");
  }
}
