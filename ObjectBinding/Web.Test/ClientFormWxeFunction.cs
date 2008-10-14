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
using Remotion.ObjectBinding.Sample;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace OBWTest
{

public class ClientFormWxeFunction: WxeFunction
{
  public ClientFormWxeFunction ()
    : base (new NoneTransactionMode ())
  {
    Object = Person.GetObject (new Guid (0,0,0,0,0,0,0,0,0,0,1));
    ReturnUrl = "javascript:window.close();";
  }

  // parameters
  public BindableXmlObject Object 
  {
    get { return (BindableXmlObject) Variables["Object"]; }
    set { Variables["Object"] = value; }
  }

  [WxeParameter (1, true)]
  public bool ReadOnly
  {
    get { return (bool) Variables["ReadOnly"]; }
    set { Variables["ReadOnly"] = value; }
  }

  // steps

  void Step1()
  {
    HttpContext.Current.Session["key"] = 123456789;
  }

  class Step2: WxeStepList
  {
    ClientFormWxeFunction Function { get { return (ClientFormWxeFunction) ParentFunction; } }
    WxeStep Step1_ = new WxePageStep ("ClientForm.aspx");
  }

  class Step3: WxeStepList
  {
    ClientFormWxeFunction Function { get { return (ClientFormWxeFunction) ParentFunction; } }
    WxeStep Step1_ = new WxePageStep ("ClientForm.aspx");
  }
}

public class ClientFormClosingWxeFunction: WxeFunction
{
  public ClientFormClosingWxeFunction ()
    : base (new NoneTransactionMode ())
  {
  }

  void Step1()
  {
    object val = HttpContext.Current.Session["key"];
    if (val != null)
    {
      int i = (int) val;
    }
  }
}

public class ClientFormKeepAliveWxeFunction: WxeFunction
{
  public ClientFormKeepAliveWxeFunction ()
    : base (new NoneTransactionMode ())
  {
  }

  void Step1()
  {
    object val = HttpContext.Current.Session["key"];
    if (val != null)
    {
      int i = (int) val;
    }
  }
}

}
