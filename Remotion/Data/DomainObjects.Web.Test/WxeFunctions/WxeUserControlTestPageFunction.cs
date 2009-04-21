// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
