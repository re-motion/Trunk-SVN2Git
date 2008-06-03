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
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;

namespace OBWTest.Design
{
public class DesignTestCheckBoxForm : DesignTestWxeBasePage
{
  protected BindableObjectDataSourceControl CurrentObject;
  protected WebButton PostBackButton;
  protected BocCheckBox BocCheckBox1;
  protected BocCheckBox BocCheckBox2;
  protected BocCheckBox BocCheckBox3;
  protected BocCheckBox BocCheckBox4;
  protected BocCheckBox BocCheckBox17;
  protected BocCheckBox BocCheckBox18;
  protected BocCheckBox BocCheckBox5;
  protected BocCheckBox BocCheckBox6;
  protected BocCheckBox BocCheckBox7;
  protected BocCheckBox BocCheckBox8;
  protected BocCheckBox BocCheckBox19;
  protected BocCheckBox BocCheckBox9;
  protected BocCheckBox BocCheckBox10;
  protected BocCheckBox BocCheckBox11;
  protected BocCheckBox BocCheckBox12;
  protected BocCheckBox BocCheckBox22;
  protected BocCheckBox BocCheckBox23;
  protected BocCheckBox BocCheckBox13;
  protected BocCheckBox BocCheckBox14;
  protected BocCheckBox BocCheckBox15;
  protected BocCheckBox BocCheckBox16;
  protected BocCheckBox BocCheckBox20;
  protected BocCheckBox BocCheckBox21;
  protected BocCheckBox BocCheckBox24;
  protected BocCheckBox BocCheckBox25;
  protected BocCheckBox BocCheckBox26;
  protected BocCheckBox BocCheckBox27;
  protected BocCheckBox BocCheckBox28;
  protected BocCheckBox BocCheckBox29;
  protected BocCheckBox BocCheckBox30;
  protected BocCheckBox BocCheckBox31;
  protected BocCheckBox BocCheckBox32;
  protected BocCheckBox BocCheckBox33;
  protected BocCheckBox BocCheckBox34;
  protected BocCheckBox BocCheckBox35;
  protected HtmlHeadContents HtmlHeadContents;

  private void Page_Load(object sender, EventArgs e)
	{
    Guid personID = new Guid(0,0,0,0,0,0,0,0,0,0,1);
    Person person = Person.GetObject (personID);
    Person partner = person.Partner;

    CurrentObject.BusinessObject = (IBusinessObject) person;
    CurrentObject.LoadValues (IsPostBack);
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
    this.EnableAbort = false;
    this.ShowAbortConfirmation = Remotion.Web.UI.ShowAbortConfirmation.Always;
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion
}

}
