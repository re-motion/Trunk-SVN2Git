/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Web;
using System.Web.SessionState;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.Test.ExecutionEngine
{
  public class SessionWxeFunction: WxeFunction
  {
    public SessionWxeFunction ()
    {
    }

    public SessionWxeFunction (params object[] args)
        : base (args)
    {
    }

    [WxeParameter (0, true)]
    public bool ReadOnly
    {
      get { return (bool) Variables["ReadOnly"]; }
      set { Variables["ReadOnly"] = value; }
    }

    // steps

    void Step1()
    {
    }

    class Step2: WxeStepList
    {
      SessionWxeFunction Function { get { return (SessionWxeFunction) ParentFunction; } }
      WxeStep Step1_ = new WxePageStep ("~/ExecutionEngine/SessionForm.aspx");
    }

    class Step3: WxeStepList
    {
      SessionWxeFunction Function { get { return (SessionWxeFunction) ParentFunction; } }
      WxeStep Step1_ = new WxePageStep ("~/ExecutionEngine/SessionForm.aspx");
    }
  }
}
