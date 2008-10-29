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
using Remotion.Data.DomainObjects.Web.Test.Domain;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.Test.WxeFunctions
{
  [Serializable]
  public class WxeUserControlTestPageFunction : WxeFunction
  {
    public WxeUserControlTestPageFunction()
        : base (WxeTransactionMode.CreateRoot)
    {
      ReturnUrl = "default.aspx";
    }

    // methods and properties

    private void Step1()
    {
      ObjectPassedIntoSecondControl = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ObjectWithAllDataTypes1);
    }

    private WxePageStep Step2 = new WxePageStep ("WxeUserControlTestPage.aspx");

    public ClassWithAllDataTypes ObjectPassedIntoSecondControl
    {
      get { return (ClassWithAllDataTypes) Variables["ObjectPassedIntoSecondControl"]; }
      set { Variables["ObjectPassedIntoSecondControl"] = value; }
    }

    public ClassWithAllDataTypes ObjectReadFromSecondControl
    {
      get { return (ClassWithAllDataTypes) Variables["ObjectReadFromSecondControl"]; }
      set { Variables["ObjectReadFromSecondControl"] = value; }
    }
  }
}