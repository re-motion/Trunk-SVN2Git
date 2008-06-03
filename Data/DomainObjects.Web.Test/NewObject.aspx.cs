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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Web.Test.Domain;
using Remotion.Data.DomainObjects.Web.Test.WxeFunctions;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.Test
{
	public class NewObjectPage : WxePage
	{
    protected Remotion.Web.UI.Controls.HtmlHeadContents Htmlheadcontents1;

    protected ControlWithAllDataTypes ControlWithAllDataTypesControl;

    private NewObjectFunction MyFunction
    {
      get { return (NewObjectFunction) CurrentFunction; }
    }

		private void Page_Load(object sender, System.EventArgs e)
		{
      ControlWithAllDataTypesControl.ObjectWithAllDataTypes = MyFunction.ObjectWithAllDataTypes;
      if (!IsPostBack)
        MyFunction.ObjectWithAllDataTypes.ByteProperty = MyFunction.ObjectWithAllDataTypes.ByteProperty;
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
      this.Load += new System.EventHandler(this.Page_Load);

    }
    #endregion

  }
}
