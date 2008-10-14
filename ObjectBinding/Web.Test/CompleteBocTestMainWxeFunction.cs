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
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace OBWTest
{

[Serializable]
public class CompleteBocTestMainWxeFunction: WxeFunction
{
  public CompleteBocTestMainWxeFunction ()
    : base (new NoneTransactionMode ())
  {
    ReturnUrl = "StartForm.aspx";
    Variables["id"] = new Guid(0,0,0,0,0,0,0,0,0,0,1).ToString();
  }

  // steps

  private WxeStep Step1 = new WxePageStep ("CompleteBocTestForm.aspx");
  private WxeStep Step2 = new WxePageStep ("CompleteBocTestUserControlForm.aspx");
  private WxeStep Step3 = new WxePageStep ("PersonDetailsForm.aspx");
}

}
