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
using Remotion.ObjectBinding;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace OBWTest
{

[Serializable]
public class ViewPersonsWxeFunction: WxeFunction
{
  static readonly WxeParameterDeclaration[] s_parameters =  { 
      new WxeParameterDeclaration ("objects", true, WxeParameterDirection.In, typeof (IBusinessObject[]))};

  public ViewPersonsWxeFunction()
    : base (new NoneTransactionMode(), s_parameters)
  {
  }

  // parameters and local variables

  [WxeParameter (1, true, WxeParameterDirection.In)]
  public IBusinessObject[] Objects
  {
    get { return (IBusinessObject[]) Variables["objects"]; }
    set { Variables["objects"] = value; }
  }

  // steps

  private WxeStep Step1 = new WxePageStep ("PersonsForm.aspx");
}
}
