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
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;

namespace OBWTest
{
	/// <summary>
	/// Summary description for WebForm3.
	/// </summary>
	public class WebForm3: Page
	{
    protected BocTextValue FirstNameField;
    protected SmartLabel BocPropertyLabel1;
    protected Button SaveButton;
    protected BocTextValue HeightField;
    protected SmartLabel BocPropertyLabel2;
    protected BocTextValueValidator BocTextValueValidator1;
    protected RadioButtonList RadioButtonList1;
    protected BindableObjectDataSourceControl reflectionBusinessObjectDataSource1;
  
		private void Page_Load (object sender, EventArgs e)
		{
      Person p = Person.CreateObject();
      p.FirstName = "Hugo";
      p.LastName = "Meier";
      p.DateOfBirth = new DateTime (1973, 10, 21);
      p.Height = 170;
      reflectionBusinessObjectDataSource1.BusinessObject = (IBusinessObject) p;

      this.DataBind();
      reflectionBusinessObjectDataSource1.LoadValues (false);
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
      this.reflectionBusinessObjectDataSource1 = new BindableObjectDataSourceControl();
      this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
      // 
      // reflectionBusinessObjectDataSource1
      // 
      this.reflectionBusinessObjectDataSource1.BusinessObject = null;
      this.reflectionBusinessObjectDataSource1.Mode = DataSourceMode.Edit;
      this.reflectionBusinessObjectDataSource1.Type = typeof (Person);
      this.Load += new System.EventHandler(this.Page_Load);

    }
		#endregion

    private void SaveButton_Click (object sender, EventArgs e)
    {
      Page.Validate();
      if (IsValid)
      {
        reflectionBusinessObjectDataSource1.SaveValues (false);
        string s = ((Person)reflectionBusinessObjectDataSource1.BusinessObject).FirstName;
      }
    }
	}
}
