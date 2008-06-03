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
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Text;
using Remotion.Web.UI.Controls;

namespace OBWTest
{

public class ClientForm : TestWxeBasePage
{
  protected HtmlHeadContents HtmlHeadContents;
  protected WebTabStrip PagesTabStrip;
  protected ValidationStateViewer ValidationStateViewer;
  protected TabbedMultiView MultiView;
  protected BocDateTimeValue BocDateTimeValue1;
  protected BocBooleanValue BocBooleanValue1;
  private PlaceHolder _wxeControlsPlaceHolder;
  private IDataEditControl[] _dataEditControls;
  private DropDownMenu _ddm = new DropDownMenu();

  protected ClientFormWxeFunction Function
  {
    get { return (ClientFormWxeFunction) CurrentFunction; }
  }

	private void Page_Load(object sender, EventArgs e)
	{
    List<IDataEditControl> dataEditControls = new List<IDataEditControl>();
    // load editor pages
    IDataEditControl dataEditControl;
    dataEditControl = AddPage ("TestTabbedPersonDetailsUserControl", "Person Details", new IconInfo ("Images/Remotion.ObjectBinding.Sample.Person.gif"), "TestTabbedPersonDetailsUserControl.ascx");
    if (dataEditControl != null)
      dataEditControls.Add (dataEditControl);
    dataEditControl = AddPage ("TestTabbedPersonJobsUserControl", "Jobs", new IconInfo ("Images/Remotion.ObjectBinding.Sample.Job.gif"), "TestTabbedPersonJobsUserControl.ascx");
    if (dataEditControl != null)
      dataEditControls.Add (dataEditControl);
    _dataEditControls = (IDataEditControl[]) dataEditControls.ToArray();

    Response.Cache.SetMaxAge (TimeSpan.Zero);
    Response.Cache.SetCacheability (HttpCacheability.NoCache);
  }

  protected override void OnPreRender(EventArgs e)
  {
    base.OnPreRender (e);
  }


  private IDataEditControl AddPage (string id, string title, IconInfo icon, string path)
  {
    TabView view = new TabView();
    view.ID = id+ "_View";
    view.Title = title;
    view.Icon = icon;
    MultiView.Views.Add (view);

    UserControl control = (UserControl) this.LoadControl (path);
    control.ID = IdentifierGenerator.HtmlStyle.GetValidIdentifier (Path.GetFileNameWithoutExtension (path));

    //EgoFormPageUserControl formPageControl = control as EgoFormPageUserControl;
    //if (formPageControl != null)
    //  formPageControl.FormPageObject = formPage;

    view.Controls.Add (control);

    IDataEditControl dataEditControl = control as IDataEditControl;
    if (dataEditControl != null)
    {
      dataEditControl.BusinessObject = (IBusinessObject) Function.Object;
      dataEditControl.LoadValues (IsPostBack);
      dataEditControl.Mode = Function.ReadOnly ? DataSourceMode.Read : DataSourceMode.Edit;
      return dataEditControl;
    }

    return null;
  }

  override protected void OnInit(EventArgs e)
	{
		//
		// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		//
		InitializeComponent();
		
    _ddm.ID = "ddm";
    _ddm.Height = new Unit("1%");
    _ddm.Width = new Unit("1%");
    _ddm.TitleText = "Options Menu";
    MultiView.TopControls.Add (_ddm);

    WebButton saveButton = new WebButton ();
    saveButton.ID = "SaveButton";
    saveButton.Text = "Save";
    saveButton.Style["margin-right"] = "10pt";
    saveButton.Click += new EventHandler(SaveButton_Click);
    MultiView.TopControls.Add (saveButton);

    WebButton cancelButton = new WebButton ();
    cancelButton.ID = "CancelButton";
    cancelButton.Text = "Cancel";
    cancelButton.Style["margin-right"] = "10pt";
    cancelButton.Click += new EventHandler(CancelButton_Click);
    cancelButton.UseSubmitBehavior = false;
    MultiView.TopControls.Add (cancelButton);

    _wxeControlsPlaceHolder = new PlaceHolder();
    MultiView.BottomControls.Add (_wxeControlsPlaceHolder);

    base.OnInit(e);
	}
	#region Web Form Designer generated code

	
	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{    
    this.Unload += new System.EventHandler(this.Page_Unload);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  private void Page_Unload (object sender, EventArgs e)
  {
  }

  private void CancelButton_Click(object sender, EventArgs e)
  {
    ExecuteNextStep();
  }

  private void SaveButton_Click(object sender, EventArgs e)
  {
    ExecuteNextStep();
  }

  protected override ControlCollection WxeControls
  {
    get { return _wxeControlsPlaceHolder.Controls; }
  }

}

}

