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
using System.Web.UI.WebControls;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;

namespace OBWTest
{

public class WebForm2: Page
{
  protected Button SaveButton;
  protected BocTextValue DateOfBirthField;
  protected SmartLabel BocPropertyLabel3;
  protected BocTextValueValidator BocTextValueValidator1;
  protected BindableObjectDataSourceControl reflectionBusinessObjectDataSource1;

	private void Page_Load (object sender, EventArgs e)
	{
    Person p = Person.CreateObject();
    p.FirstName = "Hugo";
    p.LastName = "Meier";
    p.DateOfBirth = new DateTime (1959, 4, 15);
    p.Height = 179;

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
    reflectionBusinessObjectDataSource1.SaveValues (false);
    string s = ((Person)reflectionBusinessObjectDataSource1.BusinessObject).FirstName;
  }
}

}
