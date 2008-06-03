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
public class DesignTestBooleanValueForm : DesignTestWxeBasePage
{
  protected BindableObjectDataSourceControl CurrentObject;
  protected WebButton PostBackButton;
  protected BocBooleanValue BocBooleanValue1;
  protected BocBooleanValue BocBooleanValue2;
  protected BocBooleanValue BocBooleanValue3;
  protected BocBooleanValue BocBooleanValue4;
  protected BocBooleanValue BocBooleanValue17;
  protected BocBooleanValue BocBooleanValue18;
  protected BocBooleanValue BocBooleanValue5;
  protected BocBooleanValue BocBooleanValue6;
  protected BocBooleanValue BocBooleanValue7;
  protected BocBooleanValue BocBooleanValue8;
  protected BocBooleanValue BocBooleanValue19;
  protected BocBooleanValue BocBooleanValue9;
  protected BocBooleanValue BocBooleanValue10;
  protected BocBooleanValue BocBooleanValue11;
  protected BocBooleanValue BocBooleanValue12;
  protected BocBooleanValue BocBooleanValue22;
  protected BocBooleanValue BocBooleanValue23;
  protected BocBooleanValue BocBooleanValue13;
  protected BocBooleanValue BocBooleanValue14;
  protected BocBooleanValue BocBooleanValue15;
  protected BocBooleanValue BocBooleanValue16;
  protected BocBooleanValue BocBooleanValue20;
  protected BocBooleanValue BocBooleanValue21;
  protected BocBooleanValue BocBooleanValue24;
  protected BocBooleanValue BocBooleanValue25;
  protected BocBooleanValue BocBooleanValue26;
  protected BocBooleanValue BocBooleanValue27;
  protected BocBooleanValue BocBooleanValue28;
  protected BocBooleanValue BocBooleanValue29;
  protected BocBooleanValue BocBooleanValue30;
  protected BocBooleanValue BocBooleanValue31;
  protected BocBooleanValue BocBooleanValue32;
  protected BocBooleanValue BocBooleanValue33;
  protected BocBooleanValue BocBooleanValue34;
  protected BocBooleanValue BocBooleanValue35;
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
