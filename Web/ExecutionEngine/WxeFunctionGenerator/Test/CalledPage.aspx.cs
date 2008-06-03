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
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Remotion.Web.ExecutionEngine;

namespace Test
{
	//[WxePageFunction ("CalledPage.aspx")]
	//[WxePageParameter (1, "input", typeof (string))]
	//[WxePageParameter (2, "output", typeof (string), WxeParameterDirection.Out, IsReturnValue = true)]

	// <WxePageFunction pageType="Test.CalledPage" aspxFile="CalledPage.aspx">
	//   <Parameter name="input" type="String" />
	//   <Parameter name="output" type="String" direction="Out" returnValue="true" />
	// </WxePageFunction>
  public partial class CalledPage: WxePage
  {
    protected void Page_Load (object sender, EventArgs e)
    {
    }

		protected void Button1_Click (object sender, EventArgs e)
		{
			output = "thank you";
			Return ();
		}
  }
}
