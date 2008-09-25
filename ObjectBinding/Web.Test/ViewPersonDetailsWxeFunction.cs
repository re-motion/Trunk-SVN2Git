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

namespace OBWTest
{

[Serializable]
public class ViewPersonDetailsWxeFunction: WxeFunction
{
  static readonly WxeParameterDeclaration[] s_parameters =  { 
      new WxeParameterDeclaration ("id", false, WxeParameterDirection.In, typeof (string))};

  public ViewPersonDetailsWxeFunction()
    :base (s_parameters)
  {
  }

  // parameters and local variables

  [WxeParameter (1, false, WxeParameterDirection.In)]
  public string ID
  {
    get { return (string) Variables["id"]; }
    set { Variables["id"] = value; }
  }

  // steps

  private WxeStep Step1 = new WxePageStep ("PersonDetailsForm.aspx");
}
}
