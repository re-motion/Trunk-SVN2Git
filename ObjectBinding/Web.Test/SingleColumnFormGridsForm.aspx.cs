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
using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;

namespace OBWTest
{
public class SingleColumnFormGridsForm : Page
{
  protected HtmlHeadContents HtmlHeadContents;
  protected SmartLabel SmartLabel1;
  protected BocTextValue BocTextValue1;
  protected HtmlTable LeftFormGrid;
  protected SmartLabel Smartlabel2;
  protected BocTextValue Boctextvalue2;
  protected FormGridManager FormgridManager;
  protected HtmlTable RightFormGrid;
  protected SmartLabel Smartlabel3;
  protected BocList BocList1;
  protected FormGridManager FormGridManager;

	private void Page_Load(object sender, EventArgs e)
	{
		// Put user code to initialize the page here
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
