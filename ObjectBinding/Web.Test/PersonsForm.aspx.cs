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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;


namespace OBWTest
{

public class PersonsForm : SingleBocTestWxeBasePage
{
  protected HtmlTable FormGrid;
  protected FormGridManager FormGridManager;
  protected BindableObjectDataSourceControl CurrentObject;
  protected HtmlHeadContents HtmlHeadContents;
  protected BocList PersonList;
  protected Button PostBackButton;

	private void Page_Load(object sender, EventArgs e)
	{
    PersonList.Value = (IBusinessObject[]) Variables["objects"];
  }

	override protected void OnInit(EventArgs e)
	{
		//
		// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		//
		InitializeComponent();
		base.OnInit(e);

    if (!IsPostBack)
      XmlReflectionBusinessObjectStorageProvider.Current.Reset();
	}

	#region Web Form Designer generated code
	
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
