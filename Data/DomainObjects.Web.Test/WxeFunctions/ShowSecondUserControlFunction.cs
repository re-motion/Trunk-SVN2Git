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
