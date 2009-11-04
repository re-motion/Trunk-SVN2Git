// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace OBWTest
{

[Serializable]
public class ViewPersonDetailsWxeFunction: WxeFunction
{
  static readonly WxeParameterDeclaration[] s_parameters =  { 
      new WxeParameterDeclaration ("id", false, WxeParameterDirection.In, typeof (string))};

  public ViewPersonDetailsWxeFunction()
    :base (new NoneTransactionMode(), s_parameters)
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
