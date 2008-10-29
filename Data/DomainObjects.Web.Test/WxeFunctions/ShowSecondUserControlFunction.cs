/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.Data.DomainObjects.Web.Test.Domain;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.Test.WxeFunctions
{
  public class ShowSecondUserControlFunction : WxeFunction
  {
    public ShowSecondUserControlFunction(ITransactionMode transactionMode, params object[] actualParameters) : base(transactionMode, actualParameters)
    {
    }

    [WxeParameter(0, true, WxeParameterDirection.In)]
    public ClassWithAllDataTypes ObjectWithAllDataTypes
    {
      get { return (ClassWithAllDataTypes) Variables["ObjectWithAllDataTypes"]; }
      set { Variables["ObjectWithAllDataTypes"] = value; }
    }

    [WxeParameter (1, WxeParameterDirection.Out)]
    public ClassWithAllDataTypes ReturnedObjectWithAllDataTypes
    {
      get { return (ClassWithAllDataTypes) Variables["ReturnedObjectWithAllDataTypes"]; }
      set { Variables["ReturnedObjectWithAllDataTypes"] = value; }
    }

    private WxeStep Step1 = new WxeUserControlStep ("~/SecondControl.ascx");
  }
}